using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace AUTRA.Design
{
   public class AUTRADesign
    {
        //AUTRA will be the Entry Point for the design module
        #region Constructors
        public AUTRADesign(Project project, ICodeDesigner code, List<Section> sections,List<LoadPattern> patterns,Bolt bolt)
        {
            Project = project;
            DesignCode = code;
            Combos = new List<LoadCombination>();
            Sections = sections;
            Patterns = patterns;
            Bolt = bolt;
            Connections = new List<Connection>();
        }
        #endregion
        #region Properties
        public Project Project { get; set; }
        public List<Section> Sections { get; set; }
        public List<Support> Supports { get; set; }
        public ICodeDesigner DesignCode { get; set; }
        public List<LoadCombination> Combos { get; set; }
        public List<Group> SecondaryGroups { get; set; }
        public List<Group> MainGroups { get; set; }
        public List<LoadPattern> Patterns { get; set; }
        public List<Connection> Connections { get; set; }
        public Bolt Bolt { get; set; }
        #endregion
        #region Main Methods
        public void CreateCombo(string name, params FactoredPattern[] facroredPatterns)//TODO: if user add a pattern that's not in the project 
        {
            //A Wrapper method for creating combo using AUTRA Program (Still Raw not 100% sure about this method)
            LoadCombination combo = new LoadCombination(name);
            foreach (var fp in facroredPatterns)
            {
                //add factored pattern in a combo
                //check whether the pattern in factored pattern is in the pattern list or not
                if(Patterns.FirstOrDefault(p => p == fp.Pattern)!=LoadPattern.NONE) combo.Add(fp);
            }
            //add combo in the list
            Combos.Add(combo);
        }
        public void RunAnalysis()
        {
            foreach (var beam in Project.SecondaryBeams)
            {
                Analysis.SolveBeam(beam,Patterns);
                Analysis.LinearAddCombineSA(beam, Combos);
            }
            foreach (var beam in Project.MainBeams)
            {
                Analysis.SolveBeam(beam, Patterns);
                Analysis.LinearAddCombineSA(beam, Combos);
            }
            foreach (var column in Project.Columns)
            {
                Analysis.SolveColumn(column,Patterns);
                Analysis.LinearAddCombineSA(column, Combos);
            }
            foreach (var support in Project.Supports)
            {
                Analysis.SolveReaction(support);
                Analysis.LinearAddCombineReactions(support, Combos);
            }
        }
        public void Design(List<EqualAngle> angles)
        {
            //TODO:The following two Functions basically ca run in different threads
            SecondaryGroups = Analysis.InitBeamsForDesign(Project.SecondaryBeams);
            DesignLimitState designValues = Analysis.InitColumnsForDesign(Project.Columns);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach (var group in SecondaryGroups)
            {
                bool beamGroupResult = false;
                Section section = group.Section;
                while (!beamGroupResult)
                {
                    beamGroupResult = DesignCode.DesignBeam(group, BeamType.FLOOR);
                    if (!beamGroupResult)
                    {
                        if (!GetNextSection(ref section))
                        {
                            Console.WriteLine("No bigger Section");
                            throw new Exception("No bigger section");
                        }
                    }
                    group.Section = section;
                }
            }
            
            Console.WriteLine(watch.ElapsedMilliseconds);
            MainGroups = Analysis.InitBeamsForDesign(Project.MainBeams);
            foreach (var group in MainGroups)
            {
                bool beamGroupResult = false;
                Section section = group.Section;
                while (!beamGroupResult)
                {
                    beamGroupResult = DesignCode.DesignBeam(group, BeamType.FLOOR);
                    if (!beamGroupResult)
                    {
                        if (!GetNextSection(ref section))
                        {
                            Console.WriteLine("No bigger Section");
                            throw new Exception("No bigger section");
                        }
                    }
                    group.Section = section;
                }
            }
            watch.Stop();
            Section colSection = designValues.CriticalElement.Section;
            bool columnResult = false;
            while (!columnResult)
            {
                columnResult = DesignCode.DesignColumn( colSection, designValues.CriticalElement.Length, BracingCondition.BRACED, designValues.Nd,new DesignResult());
                if (!columnResult)
                {
                    if(!GetNextSection(ref colSection))
                    {
                        Console.WriteLine("No bigger Section");
                        throw new Exception("No bigger section");
                    }

                }
            }

            //DesignConnections(angles);

            Project.SecondaryBeams.Sort(FrameElement.SortByID<Beam>());
            //SecondaryBeams.ForEach(b => b.CombinedSA.ForEach(sa => sa.Stations.Sort(Station.SortByX())));//Very very bad
            Project.MainBeams.Sort(FrameElement.SortByID<Beam>());
            //MainBeams.ForEach(b => b.CombinedSA.ForEach(sa => sa.Stations.Sort(Station.SortByX())));//Very very bad
            Project.Columns.Sort(FrameElement.SortByID<Column>());
        }
        public void CreateReports(string folderPath)
        {
            Report report = new Report(folderPath);
            report.Create("Secondary Beams.pdf", SecondaryGroups,MainGroups);
        }
        #endregion
        #region Helper Methods
        #region Methods For Section Selection
        private Section GetSectionById(int id) => Sections.FirstOrDefault(s => s.Id == id);

        private bool GetNextSection(ref Section section)
        {
            var material = section.Material;
            int id = section.Id;
            bool result = true;
            if ((id + 1) <= Sections.Count)
            {
                section = GetSectionById(id + 1);
                section.Material = material;
            }
            else
            {
                result = false;
            }
            return result;
        }
        private bool GetPreviousSection(ref Section section)
        {
            var material = section.Material;

            int id = section.Id;
            bool result = true;
            if ((id - 1) < Sections.Count)
            {
                section = GetSectionById(id - 1);
                section.Material = material;
            }
            else
            {
                result = false;
            }
            return result;
        }
        #endregion
        #endregion
        #region Connections
        private void DesignConnections( List<EqualAngle> angles)
        {
            SecondaryGroups.ForEach(g =>
            {
                SimpleConnection(g, angles);
            });
            MainGroups.ForEach(g =>
            {
                SimpleConnection(g, angles);
            });
            GetMainPart();
            ManageConnections();
        }
        private void SimpleConnection(Group group,List<EqualAngle> angles)
        {
            //This method will be done for all secondary beams and main beams
            Section section;
            SimpleConnection conn = null;
            //As Long as Connection is null (Pitch is less than 3*Dia) => Go and Get the bigger section and check again
            do
            {
             conn= DesignCode.DesignSimpleConnection(group.DesignValues.Vd, BoltedConnectionCategory.BEARING_NON_PRETENSIONED, Bolt, group.Section,angles);
                if (conn == null)
                {
                    section = group.Section;
                    if(!GetNextSection(ref section))
                    {
                        throw new Exception("No bigger Section for this in connection");
                    }
                    group.Section = section;
                }
                else
                {
                    break;
                }
            } while (true);
            group.Beams.ForEach(b =>
            {
                b.Section = group.Section; //Change the section of the beam as we don't change it

                var startConnection = new Connection();
                startConnection.SimpleConnection = conn;
                startConnection.SecondaryPart=b;
                startConnection.Position = b.StartNode.Position;
                Connections.Add(startConnection);

                var endConnection = new Connection();
                startConnection.SimpleConnection = conn;
                startConnection.SecondaryPart=b;
                startConnection.Position = b.EndNode.Position;
                Connections.Add(endConnection);
            });
        }
        private void GetMainPart()
        {
            //Assign Main Part in Each connection
            Point p;
            foreach (var c in Connections)
            {
                p = c.Position;
                c.MainPart = null;
                c.MainPart= Project.MainBeams.FirstOrDefault(b => p.IsOnLine(b));
                if (c.MainPart == null)
                    c.MainPart = Project.Columns.FirstOrDefault(col => p.IsOnLine(col));
            }
        }
        private void ManageConnections()
        {
            /*Note: The following Logic depends on tekla if it facilitates drawing on tekla then ok else not ok
             * Logic: for each node Get connections at that node => Check scenarios
             *      scenario 1: edge connection not at column => number of connections == 1 =>add list as it is in nodes
             *      scenario 2: Corner connection => number of connection == 2 => add list as it is in nodes
             *      scenario 3: edge connection at column with one secondary beam & two main beams => number of connections == 3 => add list as it is in node
             *      scenario 4: edge connection at column with two secondary beam & one main beam => number of connections == 3 => merge secondary beam as secondary part in one of the connections then remove the other
             *      scenario 5: Inner connection between secondary beams and Main beam => number of connections == 2 => merge then add
             *      scenario 6: Inner connection at column => number of connections == 4 merge secondary then add three
             * Handle each scenario 
             */
            Project.Nodes.ForEach(n =>
            {
               var conns= Connections.Where(c => c.Position == n.Position).ToList();
               int count = conns.Count;
                n.Connections = conns;
                //switch (count)
                //{
                //    case 1:
                //        //edge connection not at column (one secondary beam connecting with one main beam)
                //        n.Connections = conns;
                //        break;
                //    case 2:
                //        //it is either corner connection or inner connection not at column
                //        /*
                //         * Logic: if the secondary part in each connection on the same direction => inner connection not at column
                //         * else => corner connection
                //         */
                //        var beam1 = conns[0].SecondaryPart;
                //        var beam2 = conns[1].SecondaryPart;
                //        Point vec1 = beam1.GetVector();
                //        Point vec2 = beam2.GetVector();
                //        if(vec1.Cross(vec2)==new Point())
                //        {
                //            //inner Connection not at column
                            
                           
                //        }
                //        else
                //        {
                //            //corner connection
                //            n.Connections = conns;
                //        }
                //        break;
                //    case 3:
                //        break;
                //    case 4:
                //        break;

                //}
            });
        }
        #endregion
    }
}
