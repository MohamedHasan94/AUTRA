using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace AUTRA.Design
{
   public static class AUTRA
    {
        //Entry point for AUTRADesign & AUTRA_TEKLA
        public static void Init(Project model)
        {
            /*
             * Main Goal For this function is: 
             * 1-Take the model from front end
             * 2-Do some processing on the model & add things that not implemented in the front end
             * 3-Run AUTRADesign
             * 4-Return Analysis Result to Front End
             * 5-Run AUTRA_TEKLA
             */
            #region Reading Inputs
            var assembly = Assembly.GetExecutingAssembly();
            string sectionsPath = assembly.GoToPath(@"Resources\sections.json");
            string materialPath = assembly.GoToPath(@"Resources\steel.json");
            string boltsPath = assembly.GoToPath(@"Resources\bolts.json");
            string boltGradesPath = assembly.GoToPath(@"Resources\boltGrades.json"); 
            string equalAnglePath = assembly.GoToPath(@"Resources\equalAngle.json"); 
            var materials = Reader.Read<Material>(materialPath);
            var sections = Reader.Read<Section>(sectionsPath);
            var bolts = Reader.Read<Bolt>(boltsPath);
            var boltGrades = Reader.Read<BoltGrade>(boltGradesPath);
            var equalAngles = Reader.Read<EqualAngle>(equalAnglePath);
            #endregion

            #region Assign Material , Sections , Bolt & Grade
            //Material name is only read from front end so we have to assign its value from input file
            materials.AssignMaterialValues(model.Material);
            //for every section used in the model get its value from the sections file and assign its value
            model.Sections.ForEach(s => sections.AssignSectionValues(s));
            model.Columns.AssignID();
            model.MainBeams.AssignID();
            model.SecondaryBeams.AssignID();
            //Alert:This is hard coded Code 
            //TODO: bolts to be used should specified in the project also grade.
            var bolt = bolts.FirstOrDefault(b => b.Name == "M20");
            var grade = boltGrades.FirstOrDefault(g => g.Name == "G8.8");
            bolt.Grade = grade;
            #endregion

            //TODO:Assign Section Data
            #region Code Specifications
            var egASDCode = new ECP_ASD(); //Instatiate Egyption code
            var patterns= new List<LoadPattern> { LoadPattern.DEAD, LoadPattern.LIVE }; //This should be sent with model
            #endregion
            #region Define Supports
            model.Supports = new List<Support>();
            model.Columns.AssignSupports(model.Supports);
            #endregion

            #region AUTRA Design Module
            AUTRADesign designer = new AUTRADesign(model,egASDCode, sections,patterns,bolt);
            designer.CreateCombo("1.2D+1.4L", new FactoredPattern() { Pattern = LoadPattern.DEAD, ScaleFactor = 1.2 }, new FactoredPattern() { Pattern = LoadPattern.LIVE, ScaleFactor = 1.4 });
            //designer.CreateCombo("1.2D+1.4L", new FactoredPattern() { Pattern = LoadPattern.DEAD, ScaleFactor = 1.2 }, new FactoredPattern() { Pattern = LoadPattern.LIVE, ScaleFactor = 1.4 });
            designer.RunAnalysis();
            designer.Design(equalAngles);

            
            designer.CreateReports(@".\wwwroot\Outputs\Reports");//To be changed
            #endregion



           

            //// serialize JSON directly to a file
            //using (StreamWriter file = File.CreateText(@"c:\nodes.json"))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serializer.Serialize(file, model.Nodes);
            //}
        }
    }
}
