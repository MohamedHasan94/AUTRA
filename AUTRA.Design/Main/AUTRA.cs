using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using AUTRA.Design;
using T = AUTRA.Tekla;
using System.Diagnostics;

namespace AUTRA
{
    public static class AUTRA
    {
        //Entry point for AUTRADesign & AUTRA_TEKLA
        public static void Init(Project model, string teklaModelPath, string owner)
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
            var materials = Reader.ReadList<Material>(materialPath);
            var sections = Reader.ReadList<Section>(sectionsPath);
            var bolts = Reader.ReadList<Bolt>(boltsPath);
            var boltGrades = Reader.ReadList<BoltGrade>(boltGradesPath);
            //var equalAngles = Reader.ReadList<EqualAngle>(equalAnglePath);
            #endregion

            #region Assign Material , Sections , Bolt & Grade
            //Material name is only read from front end so we have to assign its value from input file
            materials.AssignMaterialValues(model.Material);
            //for every section used in the model get its value from the sections file and assign its value
            model.Sections.ForEach(s => sections.AssignSectionValues(s));
            sections.ForEach(s => s.Material = model.Material);
            model.Columns.AssignID("C");
            model.MainBeams.AssignID("MB");
            model.SecondaryBeams.AssignID("SB");
            //Alert:This is hard coded Code 
            //TODO: bolts to be used should specified in the project also grade.
            var bolt = bolts.FirstOrDefault(b => b.Name == "M20");
            var grade = boltGrades.FirstOrDefault(g => g.Name == "8.8");
            bolt.Grade = grade;
            #endregion

            //TODO:Assign Section Data
            #region Code Specifications
            var egASDCode = new ECP_ASD(); //Instatiate Egyption code
            var patterns = new List<LoadPattern> { LoadPattern.DEAD, LoadPattern.LIVE }; //This should be sent with model
            #endregion
            #region Define Supports
            model.Supports = new List<Support>();
            model.Columns.AssignSupports(model.Supports);
            #endregion

            #region AUTRA Design Module
            AUTRADesign designer = new AUTRADesign(model, egASDCode, sections, patterns, bolt);
            //designer.CreateCombo("D+L", new FactoredPattern() { Pattern = LoadPattern.DEAD, ScaleFactor = 1.0 }, new FactoredPattern() { Pattern = LoadPattern.LIVE, ScaleFactor = 1.0 });
            //designer.CreateCombo("1.2D+1.4L", new FactoredPattern() { Pattern = LoadPattern.DEAD, ScaleFactor = 1.2 }, new FactoredPattern() { Pattern = LoadPattern.LIVE, ScaleFactor = 1.4 });
            //designer.Combos.Add(model.loadCombination);
            designer.RunAnalysis();
            designer.Design();
            var connections = designer.DesignConnections();

            designer.CreateReports("./wwwroot/Outputs/Reports", owner);//To be changed
            #endregion
            var teklaModel = ToTekla(model, connections);
            Writer.Write(teklaModel, teklaModelPath);
        }
        public static T.TeklaModelData ToTekla(Project model, List<Connection> connections)
        {
            T.TeklaModelData teklaModel = new T.TeklaModelData();
            teklaModel.ProjectProperties = model.ProjectProperties;
            teklaModel.Model.MainBeams = GetTeklaBeamsFromDesign(model.MainBeams, "Main Beam", "Paint", "10");
            teklaModel.Model.SecondaryBeams = GetTeklaBeamsFromDesign(model.SecondaryBeams, "Secondary Beam", "Paint", "13");
            teklaModel.Model.Columns = GetTeklaColumnsFromDesign(model.Columns, "Column", "Paint", "4");
            teklaModel.Model.Footings = GetTeklaFootingsFromDesign(model.Columns, "RC Footing", "C30", "1500*1500", 700);
            teklaModel.Model.Connections = GetTeklaConnectionsFromDesign(connections);
            teklaModel.Model.Grids = GetTeklaGridsFromEditor(model.Grids);
            return teklaModel;
        }
        public static bool InitTekla(string path)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@"D:\ITI\GraduationProject\AUTRA\TeklaAPIHandler\bin\Debug\TeklaAPIHandler.exe", path)
            {
                UseShellExecute = true
            };
            p.Start();
            p.WaitForExit();
            return true;
        }
        #region Helper Methods
        private static List<T.Beam> GetTeklaBeamsFromDesign(List<Design.Beam> beams, string name, string finnish, string classNo)
        {
            List<T.Beam> teklaBeams = new List<T.Beam>();
            beams.ForEach(db =>
            {
                T.Beam tb = new T.Beam();
                tb.Id = db.Id;
                tb.Name = name;
                tb.Material = db.Section.Material.ToString();
                tb.Profile = db.Section.Name;
                tb.Finish = finnish;
                tb.AssemblyPrefix = db.Prefix;
                tb.Class = classNo;
                tb.StartPoint = new T.Point() { X = db.StartNode.Position.X * 1000, Y = db.StartNode.Position.Y * 1000, Z = db.StartNode.Position.Z * 1000 };
                tb.EndPoint = new T.Point() { X = db.EndNode.Position.X * 1000, Y = db.EndNode.Position.Y * 1000, Z = db.EndNode.Position.Z * 1000 };
                teklaBeams.Add(tb);
            });
            return teklaBeams;
        }
        private static List<T.Column> GetTeklaColumnsFromDesign(List<Design.Column> cols, string name, string finnish, string classNo)
        {
            List<T.Column> teklaColumns = new List<T.Column>();
            cols.ForEach(dc =>
            {
                T.Column tc = new T.Column();
                tc.Id = dc.Id;
                tc.Name = name;
                tc.Material = dc.Section.Material.ToString();
                tc.Profile = dc.Section.Name;
                tc.Finish = finnish;
                tc.AssemblyPrefix = dc.Prefix;
                tc.Class = classNo;
                tc.Point = new T.Point() { X = dc.StartNode.Position.X * 1000, Y = dc.StartNode.Position.Y * 1000, Z = dc.StartNode.Position.Z * 1000 };
                tc.Height = dc.Length * 1000;
                teklaColumns.Add(tc);
            });
            return teklaColumns;
        }
        private static List<T.Footing> GetTeklaFootingsFromDesign(List<Design.Column> cols, string name, string material, string profile, double depth)
        {
            List<T.Footing> teklaFootings = new List<T.Footing>();
            cols.ForEach(dc =>
            {
                T.Footing tf = new T.Footing();
                tf.Name = name;
                tf.Material = material;
                tf.Profile = profile;
                tf.Point = new T.Point() { X = dc.StartNode.Position.X * 1000, Y = dc.StartNode.Position.Y * 1000, Z = dc.StartNode.Position.Z * 1000 };
                tf.Depth = depth;
                teklaFootings.Add(tf);
            });
            return teklaFootings;
        }
        private static List<T.Connection> GetTeklaConnectionsFromDesign(List<Design.Connection> Connections)
        {
            List<T.Connection> teklaConns = new List<T.Connection>();
            Connections.ForEach(c =>
            {
                T.Connection teklaConn = new T.Connection();
                teklaConn.MainPartId = string.Format($"{c.MainPart.Prefix}{c.MainPart.Id}");
                teklaConn.SecondaryPartId = string.Format($"{c.SecondaryPart.Prefix}{c.SecondaryPart.Id}");
                teklaConn.Node = new T.Point { X = c.Position.X * 1000, Y = c.Position.Y * 1000, Z = c.Position.Z * 1000 };
                teklaConn.Dia = c.SimpleConnection.Bolt.Dia * 10;
                teklaConn.BoltType = string.Format($"{c.SimpleConnection.Bolt.Grade.Name}XOX");
                teklaConn.Edge = c.SimpleConnection.Pitch / 2;
                teklaConn.PitchLayout = c.SimpleConnection.GetPitchLayout();
                teklaConn.Hp = c.SimpleConnection.Length;
                teklaConn.Tp = c.SimpleConnection.Tp;
                teklaConn.Sw = c.SimpleConnection.Sw;
                teklaConn.ConnectionNumber = "103"; //Hard Coded
                teklaConn.Top = c.GetTopDistance();
                teklaConns.Add(teklaConn);
            });
            return teklaConns;
        }
        private static T.Grids GetTeklaGridsFromEditor(Grids grids)
        {
            T.Grids teklaGrids = new T.Grids();
            teklaGrids.CXS = grids.CXS;
            for (int i = 0; i < teklaGrids.CXS.Count; i++)
            {
                teklaGrids.CXS[i] *= 1000;
            }
            teklaGrids.CYS = grids.CYS;
            for (int i = 0; i < teklaGrids.CYS.Count; i++)
            {
                teklaGrids.CYS[i] *= 1000;
            }

            teklaGrids.CZS = new List<double>();
            teklaGrids.CZS.Add(-950);
            teklaGrids.CZS.Add(-700);
            grids.Levels.ForEach(l => teklaGrids.CZS.Add(l * 1000));
            return teklaGrids;
        }
        #endregion
    }
}
