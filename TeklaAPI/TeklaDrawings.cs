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
        /// <summary>
        /// Create Report from all objects in the model depending on type of the template used
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="fileName"></param>
        /// <param name="title1"></param>
        /// <param name="title2"></param>
        /// <param name="title3"></param>
        /// <returns></returns>
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
            //get width & height of view
            double width = Math.Abs((boundingBox.MaxPoint.X - boundingBox.MinPoint.X)*scale);
            double height = Math.Abs((boundingBox.MaxPoint.Y - boundingBox.MinPoint.Y)*scale);
            //Closes any active drawing if exists after saving it.
            _handler.CloseActiveDrawing(true);
            //create an empty GA Drawing with predifined name and predifined attribute file
            TSD.GADrawing drawing = new TSD.GADrawing(name, "AUTRA");
            //insert drawing in DB
            drawing.Insert();
            //make the drawing active
            _handler.SetActiveDrawing(drawing);
            //craeting a view by a bounding box
            TSD.View view = new TSD.View(drawing.GetSheet(), coords, coords, boundingBox);
            //setting origin of View
            view.Origin = new T3D.Point((drawing.Layout.SheetSize.Width / 2) - width/2, (drawing.Layout.SheetSize.Height / 2) /*- height/2*/);//to be revisted
            view.Name = name;
            //setting view attributes
            TSD.View.ViewAttributes viewAttributes = new TSD.View.ViewAttributes("AUTRA");
            viewAttributes.Scale = scale;
            view.Attributes = viewAttributes;
            //insert view in DB
            view.Insert();
            //insert view in drawings
            if (drawing.PlaceViews())
                _drawings.Add(drawing);
            _handler.CloseActiveDrawing(true);
            return drawing;
        }
       
        /// <summary>
        /// Create Dimension lines along beams of predefined assembly prefix
        /// </summary>
        /// <param name="drawing"></param>
        /// <param name="assemblyPrefix"></param>
        public void CreateDimsAlongSecBeams(TSD.Drawing drawing , string assemblyPrefix)
        {
            //close and save any open drawing
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                //Get Parts from the sheet
                List<TSD.Part> parts = drawing.GetSheet().GetAllObjects(typeof(TSD.Part)).ToList().OfType<TSD.Part>().ToList(); //TODO:to be optimized using filter expressions
                List<SecondaryBeam> secBeams = new List<SecondaryBeam>();
                //get all secondary beams in the sheet in a list
                foreach (TSD.Part part in parts)
                {
                    SecondaryBeam temp = new SecondaryBeam(part,_model);
                    if (temp.ModelBeam != null && temp.ModelBeam.AssemblyNumber.Prefix == assemblyPrefix)
                    {   
                        temp.ModelBeam.Select();
                        secBeams.Add(temp);
                    }
                }
                //Get the first beam
               SecondaryBeam orgBeam= secBeams.FirstOrDefault(b => b.ModelBeam.StartPoint.X == 0 && b.ModelBeam.StartPoint.Y == 0); //TODO: to be revisted (not robust)
                _model.SetPlane(orgBeam?.ModelBeam.GetCoordinateSystem());
                secBeams.ForEach(b => {
                    b.ModelBeam.Select(); //get points relative to the new coordinate system
                    b.X = b.ModelBeam.StartPoint.X; //clone X-Value
                    });
                //sort list by x
                secBeams.Sort(SecondaryBeam.SortByX());

                TSD.ViewBase viewBase = null;
                TSD.View view = null;
                TSD.PointList pointList = null;
                T3D.Point previousPoint = null;
                T3D.Point currentPoint = null;
                SecondaryBeam previousBeam = null;
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

        /// <summary>
        /// Create Dimension Line among Grids
        /// </summary>
        /// <param name="drawing"></param>
        public void CreateDimsAlongGrids(TSD.Drawing drawing)
        {
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                //Get the Grid Entity in the sheet
                TSD.Grid grid = drawing.GetSheet().GetAllObjects(typeof(TSD.Grid)).ToList().FirstOrDefault() as TSD.Grid; //to be optimized
                List<TSD.GridLine> gridLines = grid.GetObjects().ToList().OfType<TSD.GridLine>().ToList();
                //Get The view which the grid is in
                TSD.View view = grid.GetView() as TSD.View;

                //transform from whatever coordinate system to the view coordinate system
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
                _handler.CloseActiveDrawing(true);
            }
        }


        public void CreateHatch(TSD.Drawing drawing)
        {
            //this method is used to create PC & RC & Steel Hatch
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                List<TSD.Part> dparts = drawing.GetSheet().GetAllObjects() //TODO: to be optimized using Filter expressions
                      .ToList()
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
            _handler.CloseActiveDrawing(true);
        }

        private bool PrintActiveDrawing( TSD.Drawing drawing,  TSD.DotPrintPaperSize paperSize)
        {
            bool result = false; ;
            _handler.CloseActiveDrawing(true);
            if (_handler.SetActiveDrawing(drawing))
            {
                TSD.DPMPrinterAttributes attrs = new TSD.DPMPrinterAttributes
                {
                    OutputType = TSD.DotPrintOutputType.PDF,                                                 //Type of Printer 
                    PaperSize = paperSize,                                                                   //Size of Paper
                    ScalingMethod = TSD.DotPrintScalingType.Auto,                                              //Scaling Method*                                     
                    Orientation = TSD.DotPrintOrientationType.Landscape,                                     //Orientation of Paper 
                    ColorMode = TSD.DotPrintColor.BlackAndWhite,                                             //Print in Color or black and white
                    PrintToMultipleSheet = TSD.DotPrintToMultipleSheet.Off,                                    //Whether to print to multiple sheets or not
                    ScaleFactor = 1.0,                                                                       //scale factor          
                    NumberOfCopies = 1,                                                                      //Number of Copies              
                    OutputFileName = $"{_model.GetInfo().ModelPath}/PlotFiles/{drawing.Name}.pdf",                    //Output File Name 
                    OpenFileWhenFinished = true                                                              //Open file when printing is finished 
                };
                result = _handler.PrintDrawing(drawing, attrs);
            }
            _handler.CloseActiveDrawing(true);
            return result;
        }
        public void PrintDrawings(TSD.DotPrintPaperSize paperSize)
        {
            _drawings.ForEach(d => PrintActiveDrawing(d, paperSize));
        }
        #endregion
    }
}
