using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections; // Tekla Use Old DataStructure
//Tekla API Refrences
using Tekla.Structures;
using TSM=Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSMUI = Tekla.Structures.Model.UI;
using System.IO.Compression;
using System.IO;

namespace AUTRA.Tekla
{
    public  class Project
    {
        #region Private Fields
        //contains helper methods for drawings & reports
        private TSD.DrawingHandler _drawingHandler;
        private TeklaDrawings _drawings;
        private TSM.ModelHandler _modelHandler; //is used to (open , save & create new model)
        #endregion
        #region Properties
        //Model data from json
        public TeklaModelData Data { get; set; }
        //Model object
        public TSM.Model Model { get; set; }
        //total distance in X
        public double TotalX { get; set; }
        //total distance in Y
        public double TotalY { get; set; }
        //current transformation plane
        public TSM.TransformationPlane Current { get; set; }
        public List<TSM.Beam> MainBeams { get; set; }
        public List<TSM.Beam> SecondaryBeams { get; set; }
        public List<TSM.Beam> Columns { get; set; }
        public TSM.Grid Grids;

        #endregion
        #region Constructor
        public Project(TeklaModelData data)
        {
            Data = data;
            if (Init())
            {
                Model = new TSM.Model();
                _drawingHandler = new TSD.DrawingHandler();
                _drawings = new TeklaDrawings(Model);
                TSM.ModelObjectEnumerator.AutoFetch = true;
                MainBeams = new List<TSM.Beam>();
                SecondaryBeams = new List<TSM.Beam>();
                Columns = new List<TSM.Beam>();//TODO:to be revisted
                //intializing somthings in project such as(Grids,Directories,Project Properties,.....)
                SettingUpProject();
            }
           
        }
        #endregion
        #region Init
        public bool Init()
        {
            string modelfolder = @"D:\ITI\GraduationPoject\Test Tekla\";//TODO:To be changed
            _modelHandler = new TSM.ModelHandler();
            _modelHandler.Save();
            if(_modelHandler.CreateNewSingleUserModel(Data.ProjectProperties.Name, modelfolder) &&
                _modelHandler.Save())
            {
                return true;
            }
            return false;
            
        }
        private void SettingUpProject()
        {
            TotalX = 0;
            TotalY = 0;
            foreach (var x in Data.Model.Grids.CXS)
            {
                TotalX += x;
            }
            foreach (var y in Data.Model.Grids.CYS)
            {
                TotalY += y;
            }
            //setting the work plane to global
            Model.SetPlaneToGlobal();
            //setting project properties
            Model.AssignProjectProperties(Data.ProjectProperties);
            //delete existing grids  if found
            Model.DeleteAllGrids();
            //create new grids for project
            Grids = Model.CreateGrids(Data.Model.Grids.CXS.ToArray(), Data.Model.Grids.CYS.ToArray(), Data.Model.Grids.CZS.ToArray());
            //create some directories for plots and Reports in model folder 
            Model.CreateModelFolderDirectory("attributes");
            Model.CreateModelFolderDirectory("PlotFiles");
            Model.CreateModelFolderDirectory("Reports");
            Helper.GetFiles(Model); //get attributes files
            
        }
        #endregion
        #region Main Methods for Modeling
        public void CreateFootings()
        {
            Model.SetPlaneToGlobal();
            double pcDepth = 250.0;
            foreach (var f in Data.Model.Footings)
            {
                Model.CreatePadFooting(f.Point.X, f.Point.Y, f.Point.Z, f.Depth, TSM.Position.RotationEnum.TOP, f.Name, f.Profile, f.Material);//Create RC Footings
                Model.CreatePadFooting(f.Point.X, f.Point.Y, f.Point.Z - f.Depth, pcDepth, TSM.Position.RotationEnum.TOP, "PC Footing", $"{f.Length+2*pcDepth}X{f.Width+2*pcDepth}", f.Material, "5"); //Create PC Footings
            }
            Model.CommitChanges();
        }
        public void CreateColumns( )
        {
            Model.SetPlaneToGlobal();
            var rotation= MainBeams.GetColumnDirection();
            TSM.Beam column;
            foreach (var c in Data.Model.Columns)
            {
                string id = string.Format($"{c.AssemblyPrefix}{c.Id}");
                column = Model.CreateColumn(c.Point.X, c.Point.Y, c.Point.Z, c.Height, rotation, c.Name, c.Profile,c.AssemblyPrefix ,c.Material) as TSM.Beam;
                Columns.Add(column);
                Data.Model.Connections.Where(conn => conn.MainPartId == id).ToList().ForEach(conn=>conn.Main=column);
            }
            Model.CommitChanges();
        }
        private List<TSM.Beam> CreateBeams(List<Beam> beams , TSM.Position.DepthEnum depth)
        {
            var beamList = new List<TSM.Beam>();
            Model.SetPlaneToGlobal();
            foreach (var b in beams)
            {
                string id = string.Format($"{b.AssemblyPrefix}{b.Id}");
               var beam= Model.CreateBeam(b.StartPoint.X, b.StartPoint.Y, b.StartPoint.Z, b.EndPoint.X, b.EndPoint.Y, b.EndPoint.Z, depth, b.Name, b.AssemblyPrefix, b.Profile, b.Material,b.Class);
                Data.Model.Connections.Where(c => c.MainPartId == id).ToList().ForEach(c=>c.Main=beam);
                Data.Model.Connections.Where(c => c.SecondaryPartId == id).ToList().ForEach(c => c.Secondary = beam);
                beamList.Add(beam);
            }
            Model.CommitChanges();
            return beamList;
        }
        //Create Main Beams in the project 
        public void CreateMainBeams(TSM.Position.DepthEnum depth) =>MainBeams= CreateBeams(Data.Model.MainBeams, depth);
        //Create Secondary Beams in the project 
        public void CreateSecondaryBeams(TSM.Position.DepthEnum depth)=>SecondaryBeams= CreateBeams(Data.Model.SecondaryBeams, depth);
        public void CreateBaseConnections()//TODO: to be revisted
        {
            foreach (var column in Columns)
            {
                TSM.Beam footing = Model.SelectByBoundingBox<TSM.Beam>(column.StartPoint,new T3D.Point(100,100,100),new T3D.Point(-100,-100,-100)).Where(b => b.GetLengthSquared() < column.GetLengthSquared()).FirstOrDefault();
                if (footing != null)
                {
                    Model.createBasePlate(column, footing);
                }
            }
        }
        public void CreateConnections()
        {
            Model.SetPlaneToGlobal();
            foreach (var connection in Data.Model.Connections)
            {
                Model.SimpleShearPlate(connection.Main, connection.Secondary, connection.Top, connection.Hp, connection.Tp, connection.Edge, connection.PitchLayout, connection.Dia, connection.BoltType, connection.Sw);
                //List<TSM.Beam> beams = Model.SelectByBoundingBox<TSM.Beam>(new T3D.Point(connection.Node.X,connection.Node.Y,connection.Node.Z));
                //Model.CreateSimpleShearPlateConnectionAtNode(beams,connection);
            }
            Model.CommitChanges();
        }
        #endregion

        

        #region Main Methods for Drawings
        public void CreateAssemblyDWGS()
        {
            TSM.NumberingSeries numberingSeries = new TSM.NumberingSeries("S",1);
            Model.CommitChanges();
            List<TSM.Assembly> assemblies = Model.GetModelObjectSelector().GetAllObjectsWithType(TSM.ModelObject.ModelObjectEnum.ASSEMBLY).ToList().OfType<TSM.Assembly>()
                .Where(a=>a.Name== "Secondary Beam").ToList();
            foreach (var assembly in assemblies)
            {
                assembly.AssemblyNumber = numberingSeries;
                assembly.Modify();
                _drawings.CreateAssemblyDrawings(assembly.Identifier);
            }
        }
        private TSD.Drawing CreatePlanDWG(string name , double minHeight , double maxHeight)
        {
            Model.SetPlaneToGlobal();
            T3D.Point max = new T3D.Point(TotalX + 2000, TotalY + 2000, maxHeight);
            T3D.Point min = new T3D.Point(-2000, -2000, minHeight);

            T3D.AABB box = new T3D.AABB(min, max);
            _drawingHandler.CloseActiveDrawing(true);
            TSD.Drawing drawing= _drawings.CreateGADrawing(name, 0.02, new T3D.CoordinateSystem(), box);
            _drawings.CreateDimsAlongGrids(drawing);
            return drawing;
        }
        public void CreateElevationDWGSAlongX()
        {
            string[] labels;
            if (Grids != null)
            {
               labels= Grids.LabelX.Split(' ');
                for (int i = 0; i < Data.Model.Grids.CXS.Count; i++)
                {
                    CreateElevationAlongX($"Elev at Grid {labels[i]}", Data.Model.Grids.CXS[i]);
                }
            }
            
        }
        public void CreateElevationDWGSAlongY()
        {
            string[] labels;
            if (Grids != null)
            {
                labels = Grids.LabelY.Split(' ');
                for (int i = 0; i < Data.Model.Grids.CYS.Count; i++)
                {
                    CreateElevationAlongY($"Elev at Grid {labels[i]}", Data.Model.Grids.CYS[i]);
                }
            }

        }
        public void CreatePlanDWG()
        {
            double minHeight =Columns[0].EndPoint.Z - 500; //TODO:
            double maxHeight = Data.Model.Grids.CZS[Data.Model.Grids.CZS.Count - 1] + 1000;
             TSD.Drawing drawing= CreatePlanDWG("Plan", minHeight, maxHeight);
            var rects = Data.Model.Grids.CreateRectangles().ToList();
            _drawings.CreateDimsAlongBeams(drawing, Data.Model.MainBeams[0].AssemblyPrefix, Data.Model.SecondaryBeams[0].AssemblyPrefix,TotalX,TotalY,rects);
            _drawingHandler.CloseActiveDrawing(true);
            Model.SetPlaneToGlobal();
        }
        public void CreateBasePlateDWG()
        {
            double minHeight = Columns[0].StartPoint.Z;
            double maxHeight = Columns[0].StartPoint.Z + 500;
            CreatePlanDWG("Base Plates", minHeight, maxHeight);
        }
        public void PrintDrawings()
        {
            _drawings.PrintDrawings(TSD.DotPrintPaperSize.A3);
        }
        public void ExportIFC()
        {
           var objs= Model.GetModelObjectSelector().GetAllObjects().ToList();
            var selector =new TSMUI.ModelObjectSelector();
            selector.Select(new ArrayList(objs),false);
            var modelPath = Model.GetInfo().ModelPath;
            string outputFile=$"{modelPath}\\IFC\\OUT_{Data.ProjectProperties.Name}";

            var compInput = new TSM.ComponentInput();
            compInput.AddOneInputPosition(new T3D.Point(0, 0, 0));
            var comp = new TSM.Component(compInput)
            {
                Name = "ExportIFC", //Name of dll that contains the plugin
                Number = TSM.BaseComponent.PLUGIN_OBJECT_NUMBER
            };
            // Parameters
            comp.SetAttribute("OutputFile", outputFile);
            comp.SetAttribute("Format", 0);
            comp.SetAttribute("ExportType", 1);
            //comp.SetAttribute("AdditionalPSets", "");
            comp.SetAttribute("CreateAll", 1);  // 0 to export only selected objects

            // Advanced
            comp.SetAttribute("Assemblies", 1);
            comp.SetAttribute("Bolts", 1);
            comp.SetAttribute("Welds", 0);
            comp.SetAttribute("SurfaceTreatments", 1);

            comp.SetAttribute("BaseQuantities", 1);
            comp.SetAttribute("GridExport", 1);
            comp.SetAttribute("ReinforcingBars", 1);
            comp.SetAttribute("PourObjects", 1);

            comp.SetAttribute("LayersNameAsPart", 1);
            comp.SetAttribute("PLprofileToPlate", 0);
            comp.SetAttribute("ExcludeSnglPrtAsmb", 0);

            comp.SetAttribute("LocsFromOrganizer", 0);

            comp.Insert();
        }
        #endregion

        #region Helper Private Methods
       
        private void CreateElevationAlongX(string name, double xCoord)
        {
            Model.SetPlaneToGlobal();
            //Moving the transformation plane at each Grid in X-direction (Note: X-Axis beacomes Y-Axis & Y-Axis bracomes Z-Axis)
            T3D.CoordinateSystem coords = new T3D.CoordinateSystem(new T3D.Point(xCoord, 0, 0), new T3D.Vector(0, 1, 0), new T3D.Vector(0, 0, 1));
            Model.SetPlane(coords);

            double minHeight = Data.Model.Grids.CZS[0] - 1000;
            double maxHeight = Data.Model.Grids.CZS[Data.Model.Grids.CZS.Count - 1] + 1000;

            T3D.Point max = new T3D.Point(TotalY + 2000, maxHeight, 500);
            T3D.Point min = new T3D.Point(-2000, minHeight, -500);

            T3D.AABB box = new T3D.AABB(min, max);
            TSD.Drawing drawing = _drawings.CreateGADrawing(name, 0.02, coords, box);
            _drawings.CreateHatch(drawing);
            _drawings.CreateDimsAlongGrids(drawing);
        }
        private void CreateElevationAlongY(string name, double yCoord)
        {
            Model.SetPlaneToGlobal();
            //Moving the transformation plane at each Grid in X-direction (Note: X-Axis beacomes X-Axis & Y-Axis becomes Z-Axis)
            T3D.CoordinateSystem coords = new T3D.CoordinateSystem(new T3D.Point(0, yCoord, 0), new T3D.Vector(1, 0, 0), new T3D.Vector(0, 0, 1));
            Model.SetPlane(coords);

            double minHeight = Data.Model.Grids.CZS[0] - 1000;
            double maxHeight = Data.Model.Grids.CZS[Data.Model.Grids.CZS.Count - 1] + 1000;

            T3D.Point max = new T3D.Point(TotalX + 2000, maxHeight, 500);
            T3D.Point min = new T3D.Point(-2000, minHeight, -500);

            T3D.AABB box = new T3D.AABB(min, max);

            TSD.Drawing drawing = _drawings.CreateGADrawing(name, 0.02, coords, box);
            _drawings.CreateHatch(drawing);
            _drawings.CreateDimsAlongGrids(drawing);

        }
        #endregion

        #region Main Methods for Reports
        public bool CreateReports()
        {
           return _drawings.CreateReportFromAll("350   Material list.pdf", "BOM.pdf", "BOM");
        }
        #endregion

        public void CompressFolder()
        {
            var path = Model.GetInfo().ModelPath.GetFolderPath();
            var newfolderName = string.Format($"{Data.ProjectProperties.Name}_User");
            string[] filePaths = Directory.GetFiles(Model.GetInfo().ModelPath);
            string newFolderPath = $@"{path}\{newfolderName}";
            Directory.CreateDirectory(newFolderPath);
            foreach (var filename in filePaths)
            {
                int index = filename.LastIndexOf('\\');
                string file = filename.Substring(index);
                //Do your job with "file"  
                if(file!= "logs")
                {
                    {
                        File.Copy(filename,string.Format($"{newFolderPath}\\{file}"),false);
                    }
                }
            }
            ZipFile.CreateFromDirectory(path, $"{path}{newfolderName}.zip");
                
                
            
        }
    }
}
