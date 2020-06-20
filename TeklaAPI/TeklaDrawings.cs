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
using TSD= Tekla.Structures.Drawing;
using TSMUI=Tekla.Structures.Model.UI;
using Tekla.Structures.Model.Operations;


namespace AUTRA.Tekla
{
    public class TeklaDrawings
    {
        #region fields
        private TSD.DrawingHandler _handler;
        private TSM.Model _model;
        private List<TSD.Drawing> _drawings;
        #endregion
        #region Constructors
        public TeklaDrawings(TSM.Model model)
        {
            TSD.DrawingObjectEnumerator.AutoFetch = true; //to speed things up
            _handler = new TSD.DrawingHandler();
            _model = model;
            _drawings = new List<TSD.Drawing>();
        }
        #endregion
        #region Methods
        public bool  CreateReportFromAll(string templateName,string fileName,string title1,string title2="",string title3="")
        {
          return  Operation.CreateReportFromAll(templateName, $"{_model.GetInfo().ModelPath}/Reports/{fileName}" ,title1, title2, title3);
        }
        public TSD.Drawing CreateAssemblyDrawings(Identifier assemblyId)
        {
            //save & close any active drawings
            _handler.CloseActiveDrawing(true);
            TSD.AssemblyDrawing assembly = new TSD.AssemblyDrawing(assemblyId,"Standard");
            _handler.SetActiveDrawing(assembly);
            assembly.Insert();
            _handler.CloseActiveDrawing(true);
            return assembly;
        }
        public TSD.Drawing CreateGADrawing(string name, double scale, T3D.CoordinateSystem coords, T3D.AABB boundingBox)
        {
            var drawing = _handler.CreateGASheet(name, "AUTRA");
            var view = drawing.CreateView(coords, boundingBox, scale, drawing.Name, "AUTRA");
            if (drawing.PlaceViews())
                _drawings.Add(drawing);
            return drawing;
        }
        public void CreateDimsAlongSecBeams(TSD.Drawing drawing , string mainAssembly,string secondaryAssembly)
        {
            //close and save any open drawing
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                //Get Drawing Parts from the sheet
                List<TSD.Part> parts = drawing.GetSheet().GetAllObjects(typeof(TSD.Part)).ToIEnumerable().OfType<TSD.Part>().ToList(); //TODO:to be optimized using filter expressions
                //get all secondary beams(Drawing + Model) in the sheet in a list
                List<ContainerBeam> secBeams = parts.GetModelBeams(_model, mainAssembly,secondaryAssembly).ToList();
                //Get the first beam
                ContainerBeam orgBeam= secBeams.FirstOrDefault(b => b.ModelBeam.StartPoint.X == 0 && b.ModelBeam.StartPoint.Y == 0); //TODO: to be revisted (not robust)
                _model.SetPlane(orgBeam?.ModelBeam.GetCoordinateSystem());
                secBeams.ForEach(b => {
                    b.ModelBeam.Select(); //get points relative to the new coordinate system
                    b.X = b.ModelBeam.StartPoint.X; //clone X-Value
                    });
                //sort list by x
                secBeams.Sort(ContainerBeam.SortByX());
                TSD.ViewBase viewBase = null;
                TSD.View view = null;
                TSD.PointList pointList = null;
                T3D.Point previousPoint = null;
                T3D.Point currentPoint = null;
                ContainerBeam previousBeam = null;
                T3D.Vector up= null;
                T3D.Vector z = new T3D.Vector(0,0,1);
                foreach (var beam in secBeams)
                {
                    if (previousPoint == null)
                    {
                        //intialization
                        previousPoint = beam.ModelBeam.StartPoint;
                        currentPoint = beam.ModelBeam.StartPoint;
                        previousBeam = beam;
                    }
                    else
                    {
                        if(beam.X==previousBeam.X)
                        {
                            //beams are parallel
                            _model.SetPlaneToGlobal();
                            view = beam.DrawingBeam.GetView() as TSD.View;
                            _model.SetPlane(view.ViewCoordinateSystem);
                            previousBeam.ModelBeam.Select();
                            previousPoint = previousBeam.ModelBeam.StartPoint;
                            beam.ModelBeam.Select();
                            currentPoint = beam.ModelBeam.StartPoint;
                            pointList = new TSD.PointList();
                            pointList.Add(previousPoint);
                            pointList.Add(currentPoint);
                            viewBase = beam.DrawingBeam.GetView();
                            up = z.Cross(new T3D.Vector(currentPoint - previousPoint));
                            previousBeam = beam;
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(beam.DrawingBeam, "AUTRA");
                            TSD.StraightDimensionSet XDimensions = new TSD.StraightDimensionSetHandler().CreateDimensionSet(viewBase, pointList, up, 700, attr);
                            

                        }
                        else
                        {
                            previousPoint = beam.ModelBeam.StartPoint;
                            currentPoint = beam.ModelBeam.StartPoint;
                            previousBeam = beam;

                        }
                    }
                }
                drawing.CommitChanges();
                _handler.CloseActiveDrawing(true);
                _model.SetPlaneToGlobal();

            }
          
        }
        public void CreateDimsAlongGrids(TSD.Drawing drawing)
        {
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                TSD.Grid grid = drawing.GetGrid(); //Get Grid Entity From Drawing sheet
                List<TSD.GridLine> gridLines = grid.GetGridLines();
                //Get The view which the grid is in
                TSD.View view = grid.GetView() as TSD.View;
                //transform from whatever coordinate system to the view coordinate system
                _model.SetPlaneToGlobal();
                _model.SetPlane(view.DisplayCoordinateSystem);
                //list of points to store points to draw Dimension Lines
                TSD.PointList pointList = null;
                //To draw a Dimension Line between the first gridLine and Last gridLine => define firstPoint and lastPoint
                T3D.Point firstPoint = null;
                T3D.Point lastPoint = null;
                T3D.Point previousPoint = null;
                T3D.Point currentPoint = null;
                T3D.Vector pointsVector = null; //vector between current & previous point
                T3D.Vector up = null; //up direction
                T3D.Vector zVector = new T3D.Vector(0, 0, 1);
                double distance = 200.0;
                TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(grid, "AUTRA");//Dimesions Attributes
                /*Logic:
                 1-Start from the seocnd GridLine
                 2-While vector from first point to previous point cross vector form firstpoint to current point == zero length vector => I am in same dirction and Same UP direction
                 3-otherwise :
                 * update firstpoint = current & lastpoint = previous
                 */
                foreach (TSD.GridLine gridLine in gridLines)
                {
                    if (previousPoint == null)
                    {
                        //Intialization
                        previousPoint = gridLine.StartLabel.GridPoint;
                        currentPoint = gridLine.StartLabel.GridPoint;
                        firstPoint = gridLine.StartLabel.GridPoint;
                    }
                    else
                    {
                        previousPoint = currentPoint;
                        currentPoint = gridLine.StartLabel.GridPoint;
                        T3D.Vector firstToPrevious = new T3D.Vector(previousPoint - firstPoint);  //vector from first Point to Previous Point
                        T3D.Vector firstTocurrent = new T3D.Vector(currentPoint - firstPoint);    //vector from first point to current Point
                        if (firstTocurrent.Cross(firstToPrevious) != new T3D.Vector() )
                        {
                            //if not equal to zero length vector =>
                            //change in direction occurs
                            //Create Dimension from first grid to last grid
                            lastPoint = previousPoint;
                            distance += 250;
                            pointList = new TSD.PointList();
                            pointList.Add(firstPoint);
                            pointList.Add(lastPoint);
                            TSD.StraightDimensionSet xDims = new TSD.StraightDimensionSetHandler().CreateDimensionSet(grid.GetView(), pointList, up, distance, attr);
                            /*Note:
                             case: 'pointsVector' is in X-axis direction => X cross Z => -Y axis So, distance is postive
                             case: 'pointsVector' is in Y-axis direction => Y cross Z => +X axis So, distance is negative
                             */
                            distance = -200.0;
                            previousPoint = currentPoint;
                            firstPoint = currentPoint;
                        }
                        else
                        {
                            //Same Direction
                            pointList = new TSD.PointList();
                            pointList.Add(previousPoint);
                            pointList.Add(currentPoint);
                            pointsVector = new T3D.Vector(currentPoint - previousPoint);
                            up = pointsVector.Cross(zVector);
                            TSD.StraightDimensionSet gridDims = new TSD.StraightDimensionSetHandler().CreateDimensionSet(grid.GetView(), pointList, up, distance, attr);
                        }

                    }

                }
                lastPoint = currentPoint;
                distance -= 300;
                pointList = new TSD.PointList();
                pointList.Add(firstPoint);
                pointList.Add(lastPoint);
                new TSD.StraightDimensionSetHandler().CreateDimensionSet(grid.GetView(), pointList, up, distance, attr);
                drawing.CommitChanges();
                //TSD.StraightDimensionSet xDims = new TSD.StraightDimensionSetHandler().CreateDimensionSet(grid.GetView(), pointList, new T3D.Vector(0, 1, 0), 100, attr);
            }
        }
        public void CreateDimsAlongBeams(TSD.Drawing drawing,string mainAssembly , string secondaryAssembly,double totalX , double totalY , List<Rectangle> rects )
        {
            /*
             * Logic:
             * 1- Get all secondary beams and insert them in quadtree 
             * 2-every intersection between two grids in X and two Grids in Y are rectangle 
             * 3-Get all beams in each rectangle
             * 4-get beams in x and beams in y
             * 5-Draw dimensions for beams in X and beams in Y
             */
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                var beams = drawing.GetSheet().GetAllObjects(typeof(TSD.Part))
                    .ToIEnumerable()
                    .OfType<TSD.Part>()
                    .ToList()
                    .GetModelBeams(_model,mainAssembly,secondaryAssembly).ToList();
                var quadTree = new QuadTree(new Rectangle(totalX,totalY), 4);
                beams.ForEach(b => quadTree.Insert(b)); //insert each beam in beams in quad tree
                rects.ForEach(rect =>
                {
                    var bs = quadTree.Query(rect); //Get List of Beams in each rectangle
                    var tuple = bs.GetParallelXY();
                    if(tuple.ParallelX!= null)
                    {
                        var lstlst = FilterBeamsInX(tuple.ParallelX);
                        lstlst.ForEach(lst =>
                        {
                           var psX = lst.GetPointList().CheckYBoundaries(rect);
                           var viewBase = tuple.ParallelX[0].DrawingBeam.GetView();
                           TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(tuple.ParallelX[0].DrawingBeam, "AUTRA");
                           TSD.StraightDimensionSet XDimensions = new TSD.StraightDimensionSetHandler().CreateDimensionSet(viewBase, psX, new T3D.Vector(1, 0, 0), -400, attr);
                            
                        });
                    }
                    if (tuple.ParallelY != null)
                    {
                        var lstlst = FilterBeamsInY(tuple.ParallelY);
                        lstlst.ForEach(lst =>
                        {
                            var psY = lst.GetPointList().CheckXBoundaries(rect);
                            var viewBase = tuple.ParallelY[0].DrawingBeam.GetView();
                            TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(tuple.ParallelY[0].DrawingBeam, "AUTRA");
                            TSD.StraightDimensionSet XDimensions = new TSD.StraightDimensionSetHandler().CreateDimensionSet(viewBase, psY, new T3D.Vector(0, 1, 0),400, attr);
                        });
                    }
                });
                 drawing.CommitChanges();
            }
        }
        public void CreateHatch(TSD.Drawing drawing)
        {
            //this method is used to create PC & RC & Steel Hatch
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                List<TSD.Part> dparts = drawing.GetSheet().GetAllObjects() //TODO: to be optimized using Filter expressions
                      .ToIEnumerable()
                      .OfType<TSD.Part>()
                      .ToList();
                foreach (var dpart in dparts)
                {
                    TSM.Part mpart = _model.SelectModelObject(dpart.ModelIdentifier) as TSM.Part;
                    if (mpart.Name.ToLower().Contains("rc"))
                    {
                        //RC Foorting
                        dpart.Attributes.SectionFaceHatch.Name = "SOLID";

                    }
                    else if (mpart.Name.ToLower().Contains("pc"))
                    {
                        //RC Foorting
                        dpart.Attributes.SectionFaceHatch.Name = "AR-CONC";
                    }
                    else
                    {
                        //Steel
                        dpart.Attributes.SectionFaceHatch.Name = "hardware_SOLID";

                    }
                    dpart.Modify();
                }

            }

            drawing.Modify();
        }
        private bool PrintActiveDrawing( TSD.Drawing drawing,  TSD.DotPrintPaperSize paperSize)
        {
            bool result = false; 
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                TSD.DPMPrinterAttributes attrs = new TSD.DPMPrinterAttributes
                {
                    OutputType = TSD.DotPrintOutputType.PDF,                                                 //Type of Printer 
                    PaperSize = paperSize,                                                                   //Size of Paper
                    ScalingMethod = TSD.DotPrintScalingType.Auto,                                            //Scaling Method*                                     
                    Orientation = TSD.DotPrintOrientationType.Landscape,                                     //Orientation of Paper 
                    ColorMode = TSD.DotPrintColor.BlackAndWhite,                                             //Print in Color or black and white
                    PrintToMultipleSheet = TSD.DotPrintToMultipleSheet.Off,                                  //Whether to print to multiple sheets or not
                    ScaleFactor = 1.0,                                                                       //scale factor          
                    NumberOfCopies = 1,                                                                      //Number of Copies              
                    OutputFileName = $"{_model.GetInfo().ModelPath}/PlotFiles/{drawing.Name}.pdf",            //Output File Name 
                    OpenFileWhenFinished = false                                                              //Open file when printing is finished 
                };
                result = _handler.PrintDrawing(drawing, attrs);
            }
            return result;
        }
        public void PrintDrawings(TSD.DotPrintPaperSize paperSize)
        {
            _drawings.ForEach(d => {
                PrintActiveDrawing(d, paperSize);
            });
        }
        #endregion

        private List<List<ContainerBeam>> FilterBeamsInX(List<ContainerBeam> beams)
        {
            List<List<ContainerBeam>> bigLst = new List<List<ContainerBeam>>();
            beams = beams.OrderByDescending(b => b.ModelBeam.GetLengthSquared()).ToList(); //Beams are ordered descendingly according to their length
            var placeHolder = beams[0];
            List<ContainerBeam> small = new List<ContainerBeam>();
            bigLst.Add(small);
            foreach (var beam in beams)
            {
                if (placeHolder.XIsBetween(beam)) small.Add(beam);
                else
                {
                    placeHolder = beam;
                    small = new List<ContainerBeam>();
                    small.Add(beam);
                    bigLst.Add(small);
                }
            }
            return bigLst;
        }
        private List<List<ContainerBeam>> FilterBeamsInY(List<ContainerBeam> beams)
        {
            List<List<ContainerBeam>> bigLst = new List<List<ContainerBeam>>();
            beams = beams.OrderByDescending(b => b.ModelBeam.GetLengthSquared()).ToList(); //Beams are ordered descendingly according to their length
            var placeHolder = beams[0];
            List<ContainerBeam> small = new List<ContainerBeam>();
            bigLst.Add(small);
            foreach (var beam in beams)
            {
                if (placeHolder.YIsBetween(beam)) small.Add(beam);
                else
                {
                    placeHolder = beam;
                    small = new List<ContainerBeam>();
                    small.Add(beam);
                    bigLst.Add(small);
                }
            }
            return bigLst;
        }
    }
    
}
