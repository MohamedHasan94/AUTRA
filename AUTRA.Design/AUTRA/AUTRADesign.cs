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
        public Group ColumnsGroup { get; set; }
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
        public void Design()
        {
            //TODO:The following two Functions basically ca run in different threads
            SecondaryGroups = Analysis.InitBeamsForDesign(Project.SecondaryBeams);
            ColumnsGroup = Analysis.InitColumnsForDesign(Project.Columns);
            SecondaryGroups.ForEach(g => DesignGroup(g));
            MainGroups = Analysis.InitBeamsForDesign(Project.MainBeams);
            MainGroups.ForEach(g => DesignGroup(g));
            Section colSection = ColumnsGroup.DesignValues.CriticalElement.Section;
            bool columnResult = false;
            while (!columnResult)
            {
                columnResult = DesignCode.DesignColumn( ColumnsGroup, BracingCondition.BRACED);
                if (!columnResult)
                {
                    if(!GetNextSection(ref colSection))
                    {
                        Console.WriteLine("No bigger Section");
                        throw new Exception("No bigger section");
                    }

                }
            }


            Project.SecondaryBeams.Sort(FrameElement.SortByID<Beam>());
            Project.MainBeams.Sort(FrameElement.SortByID<Beam>());
            Project.Columns.Sort(FrameElement.SortByID<Column>());
        }
        public void CreateReports(string folderPath,string userName)
        {
            Report report = new Report(folderPath ,  userName,Project.ProjectProperties);
            report.Create("Design Calculation Sheet.pdf", SecondaryGroups,MainGroups,ColumnsGroup);
        }
        #endregion

        

        #region Helper Methods
        private void DesignGroup(Group group)
        {
            bool stopFlag = false;
            bool isSmallerUnsafe = false;
            BeamDesignStatus result;
            Section section = group.Section;
            while (!stopFlag)
            {
                result = DesignCode.DesignBeam(section,group, BeamType.FLOOR);
                switch (result)
                {
                    case BeamDesignStatus.VERY_SAFE:
                        //if smallerisChecked => break
                        //is smaller is not checked => get smaller section
                        if (isSmallerUnsafe) stopFlag=true;
                        else
                        {
                            if (!GetPreviousSection(ref section)) isSmallerUnsafe = true;
                        }
                        break;
                    case BeamDesignStatus.SAFE:
                        stopFlag = true;
                        break;
                    case BeamDesignStatus.UNSAFE:
                        //get larger section
                        //if you cant get larger section throw exception
                        isSmallerUnsafe = true;
                        if (!GetNextSection(ref section))
                        {
                            throw new Exception("No bigger section");
                        }
                        break;
                }
            }
            if (section.H < 20) group.Section = Sections.FirstOrDefault(s => s.Name == "IPE200");
            else group.Section = section;
        }
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
            if ((id - 1) < Sections.Count && (id-1)>0)
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
        
        #region Connections
        public List<Connection> DesignConnections()
        {
            int weldSize = 6; 
            int plateThickness = 10;
            SecondaryGroups.ForEach(g =>
            {
                SimpleConnection(g,ref weldSize ,ref plateThickness );
            });
            MainGroups.ForEach(g =>
            {
                SimpleConnection(g,ref weldSize , ref plateThickness);
            });
            Connections.ForEach(c => {
                //make sure that plate thickness and weld size are same for project
                c.SimpleConnection.Tp = plateThickness; 
                c.SimpleConnection.Sw = weldSize;
            });
            SecondaryGroups.AssignSectionToElement();
            MainGroups.AssignSectionToElement();
            ColumnsGroup.AssignSectionToElement();
            GetMainPart();
            return Connections;
        }
        private void SimpleConnection(Group group , ref int weldSize ,ref int plateThickness )
        {
            //This method will be done for all secondary beams and main beams
            Section section;
            SimpleConnection conn = null;
            //As Long as Connection is null (Pitch is less than 3*Dia) => Go and Get the bigger section and check again
            do
            {
                conn = DesignCode.DesignSimpleConnection(group.DesignValues.Vd, Bolt, group.Section,ref weldSize ,ref plateThickness);
                if (conn == null)
                {
                    section = group.Section;
                    if (!GetNextSection(ref section))
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
            group.Connection = conn;
            group.Elements.ForEach(b =>
            {
                var startConnection = new Connection();
                startConnection.SimpleConnection = conn;
                startConnection.SecondaryPart = b;
                startConnection.Position = b.StartNode.Position;
                Connections.Add(startConnection);

                var endConnection = new Connection();
                endConnection.SimpleConnection = conn;
                endConnection.SecondaryPart = b;
                endConnection.Position = b.EndNode.Position;
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
                c.MainPart = Project.Columns.FirstOrDefault(col => p.IsOnLine(col));
                if (c.MainPart == null)
                {
                    /*
                     * Logic=> Get main beams that share this point
                     * remove the main beam that is in the secondary part of the connection if exists
                     *  if count ==1 => mainpart is the first
                     *  if count > 1 get main part that is prependicular to the secondary part
                     */
                    var mains = Project.MainBeams.Where(b => p.IsOnLine(b)).Where(b=>b!=c.SecondaryPart).ToList(); //return list of main beams that p is on them
                    if(mains != null)
                    {
                        if (mains.Count == 1)
                            c.MainPart = mains[0];
                        else
                           c.MainPart=mains.FirstOrDefault(m => c.SecondaryPart.IsPrependicular(m));
                        //check that main beam is greater or equal the secondary beam
                        if (c.MainPart.Section < c.SecondaryPart.Section) c.MainPart.Section = c.SecondaryPart.Section;
                    }
                }
            }
        }
        #endregion
    }
}
