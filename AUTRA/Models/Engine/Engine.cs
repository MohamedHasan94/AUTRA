using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using AUTRA.Models.StructuralElements;
using AUTRA.Models.Definitions;
using AUTRA.Models.EnumHelpers;
using AUTRA.Models.Analysis;
using AUTRA.Models.Design;
using AUTRA.Models.Reports;

namespace AUTRA.Models.Main
{
   public class Engine
    {
        //AUTRA will be the Entry Point for the design program
        #region Constructors
        public Engine(Project project, List<Section> sections, ICodeDesigner code)//List<Beam> secbeams, List<Beam> mainBeams,List<Column> columns, List<Support> supports, ICodeDesigner code, List<Section> sections,List<LoadPattern> patterns)
        {
            SecondaryBeams = project.SecondaryBeams;// secbeams;
            MainBeams = project.MainBeams;//mainBeams;
            Columns = project.Columns;//columns;
            //Supports = supports;
            DesignCode = code;//code;
            Combos = new List<LoadCombination>();
            Sections = sections;//sections;
            Patterns = new List<LoadPattern> { LoadPattern.DEAD, LoadPattern.LIVE };//patterns;
        }
        //public AUTRA(List<Beam> beams, List<Column> columns, List<Support> supports, ICodeDesigner code) : this(beams, columns, supports, code, null)
        //{ }
        #endregion
        #region Properties
        public List<Section> Sections { get; set; }
        public List<Beam> MainBeams { get; set; }
        public List<Beam> SecondaryBeams { get; set; }
        public List<Column> Columns { get; set; }
        //public List<Support> Supports { get; set; }
        public ICodeDesigner DesignCode { get; set; }
        public List<LoadCombination> Combos { get; set; }
        public List<Group> SecondaryGroups { get; set; }
        public List<Group> MainGroups { get; set; }
        public List<LoadPattern> Patterns { get; set; }
        #endregion
        #region Main Methods
        public void CreateCombo(string name, params FactoredPattern[] facroredPatterns)
        {
            //A Wrapper method for creating combo using AUTRA Program (Still Raw not 100% sure about this method)
            LoadCombination combo = new LoadCombination(name);
            foreach (var fp in facroredPatterns)
            {
                combo.Add(fp);
            }
            Combos.Add(combo);
        }

        public void RunAnalysis()
        {
            foreach (var beam in SecondaryBeams)
            {               
                Analyze.SolveBeam(beam,Patterns);
                Analyze.LinearAddCombineSA(beam, Combos);
            }

            foreach (var beam in MainBeams)
            {
                Analyze.SolveBeam(beam, Patterns);
                Analyze.LinearAddCombineSA(beam, Combos);
            }

            foreach (var column in Columns)
            {
                Analyze.SolveColumn(column,Patterns);
                Analyze.LinearAddCombineSA(column, Combos);
            }

            /*foreach (var support in Supports)
            {
                Analyze.SolveReaction(support);
                Analyze.LinearAddCombineReactions(support, Combos);
            }*/
        }
        public void Design()
        {
            //TODO:The following two Functions basically ca run in different threads
            SecondaryGroups = Analyze.InitBeamsForDesign(SecondaryBeams);
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
            
            //MainBeams
            MainGroups = Analyze.InitBeamsForDesign(MainBeams);
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

            //Columns
            DesignLimitState designValues = Analyze.InitColumnsForDesign(Columns);
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
            int id = section.Id;
            bool result = true;
            if ((id + 1) <= Sections.Count)
            {
                section = GetSectionById(id + 1);
            }
            else
            {
                result = false;
            }
            return result;
        }
        private bool GetPreviousSection(ref Section section)
        {
            int id = section.Id;
            bool result = true;
            if ((id - 1) < Sections.Count)
            {
                section = GetSectionById(id - 1);
            }
            else
            {
                result = false;
            }
            return result;
        }
        #endregion
        #endregion

    }
}
