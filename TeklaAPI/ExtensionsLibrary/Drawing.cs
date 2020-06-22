using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD = Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using System.Collections;

namespace AUTRA.Tekla
{
   public static class Drawing
    {
        public static List<T> AddFirst<T>(this List<T> list , T item)
        {
            List<T> newList = new List<T>();
            newList.Add(item);
            list.ForEach(i => newList.Add(i));
            return newList;
        }
        public static IEnumerable<T3D.Point> GetPoints(this TSD.PointList ps)
        {
            foreach (var p in ps)
            {
                yield return p as T3D.Point;
            }
        }
        public static TSD.PointList ToPointList(this ArrayList lst)
        {
            TSD.PointList pst = new TSD.PointList();
            foreach (var item in lst)
            {
                pst.Add(item as T3D.Point);
            }
            return pst;
        }
        public static TSD.PointList CheckXBoundaries(this TSD.PointList ps , Rectangle rect)
        {
            var points = ps.GetPoints().ToList();
            var psx = rect.GetXBoundaries(ps[0].Z);
            points.Add(psx.Right);
            points.Add(psx.Left);
            return points.GetPointList();
        }
        public static TSD.PointList CheckYBoundaries(this TSD.PointList ps, Rectangle rect)
        {
            var points = ps.GetPoints().ToList();
            var psx = rect.GetYBoundaries(ps[0].Z);
             points.Add(psx.Up);
             points.Add(psx.Down);
            return points.GetPointList();
        }
        public static TSD.PointList GetPointList(this List<T3D.Point> points)
        {
            TSD.PointList ps = new TSD.PointList();
            points.ForEach(p => ps.Add(p));
            return ps;
        }
        public static TSD.PointList GetPointList(this List<ContainerBeam> beams)
        {
            TSD.PointList ps = new TSD.PointList();
            beams.ForEach(b => ps.Add(b.ModelBeam.GetMidPoint()));
            return ps;
        }
        public static TSD.Grid GetGrid(this TSD.Drawing drawing) => drawing.GetSheet().GetAllObjects(typeof(TSD.Grid)).ToIEnumerable().OfType<TSD.Grid>().FirstOrDefault();
        public static List<TSD.GridLine> GetGridLines(this TSD.Grid grid) => grid.GetObjects().ToIEnumerable().OfType<TSD.GridLine>().ToList();
        public static IEnumerable<ContainerBeam> GetModelBeams(this List<TSD.Part> parts ,TSM.Model model ,string mainAssembly , string secondaryAssembly)
        {
            foreach (TSD.Part part in parts)
            {
                ContainerBeam temp = new ContainerBeam(part, model);
                if (temp.ModelBeam != null && (temp.ModelBeam.AssemblyNumber.Prefix == mainAssembly|| temp.ModelBeam.AssemblyNumber.Prefix==secondaryAssembly))
                {
                    temp.ModelBeam.Select();
                    yield return temp;
                }
            }
        }
        public static IEnumerable<TSD.DrawingObject> ToIEnumerable(this TSD.DrawingObjectEnumerator enumerator)
        {
            TSD.DrawingObjectEnumerator.AutoFetch = true;
            while (enumerator.MoveNext())
            {
                var drawingObject = enumerator.Current;
                if (drawingObject == null)
                    continue;
                yield return drawingObject;
            }
        }
        public static TSD.Drawing CreateGASheet(this TSD.DrawingHandler handler, string sheetName ,string attributeFile )
        {
            handler.CloseActiveDrawing(true); //saves and closes the active drawing
            var ga = new TSD.GADrawing(sheetName, attributeFile); //generate GA sheet with name='sheetName' utilising attributeFile
            TSD.LayoutAttributes attr = new TSD.LayoutAttributes("AUTRA",ga);
            attr.SizeDefinitionMode = TSD.SizeDefinitionMode.AutoSize;
            ga.Title1 = sheetName;
            ga.Insert();
            handler.SetActiveDrawing(ga,false);
            return ga;
        }
        public static TSD.View CreateView(this TSD.Drawing drawing , T3D.CoordinateSystem coords , T3D.AABB box,double scale ,string viewName , string attributeFile,ViewPlacment placement)
        {
            var view = new TSD.View(drawing.GetSheet(), coords, coords, box);
            var h = placement == ViewPlacment.CENTER ? 2000 : -1000;
            view.Origin = new T3D.Point((drawing.Layout.SheetSize.Width / 2) - box.GetLWH().Length*scale / 2+2000*scale, (drawing.Layout.SheetSize.Height / 2) - box.GetLWH().Width * scale / 2+h*scale);//TODO:To be revisted
            view.Name = viewName;
            //setting view attributes
            TSD.View.ViewAttributes viewAttributes = new TSD.View.ViewAttributes(attributeFile);
            viewAttributes.Scale = scale;
            view.Attributes = viewAttributes;
            //insert view in DB
            view.Insert();
            return view;
        }
        public static List<(T3D.Point Center , T3D.Point Insertion)> GetInsertionTuple(this List<T3D.Point> points,TSD.View view,double scale )
        {
            //takes points of elements in view and return tuple of this point in view coordinate system and its coresponding point in sheet
            List<(T3D.Point Center, T3D.Point Insertion)> tuples = new List<(T3D.Point center, T3D.Point insertion)>();
            points.ForEach(p => tuples.Add((Center: p, Insertion: view.MapToSheet(p, scale))));
            return tuples;
        }
        public static List<(T3D.Point Center, T3D.Point Insertion)> MapToInsertion(this List<(T3D.Point Center, T3D.Point Insertion)> tuples,TSD.Drawing drawing)
        {
            //this method specify which point to insert the detail view
            List<(T3D.Point Center, T3D.Point Insertion)> newTuples = new List<(T3D.Point Center, T3D.Point Insertion)>();
            int count = tuples.Count;
            var height=drawing.GetSheet().Height;
            var width = drawing.GetSheet().Width-TeklaConstants.LAYOUT_MARGIN*6;
            if (count <= 6)
            {
                //Draw details on one row
                var space = width / count;
                var x = TeklaConstants.LAYOUT_MARGIN * 6;
                var y = tuples[0].Insertion.Y;
                var dy = Math.Abs(height - y) / 2; //Get the distance between the current point to the top of the sheet
                tuples.ForEach(t =>
                {
                    newTuples.Add((Center: t.Center, Insertion: new T3D.Point(x,t.Insertion.Y+dy,t.Insertion.Z)));
                    x += space;
                });
            }
            else
            {
                //Draw details on two rows
                int halfCount = count / 2;
                var space = width / halfCount;
                var x = TeklaConstants.LAYOUT_MARGIN * 6;
                var y = tuples[0].Insertion.Y;
                var dy = Math.Abs(height - y) *((double)1/3); //Get the distance between the current point to the top of the sheet
                for (int i = 0; i < halfCount; i++)
                {
                    newTuples.Add((Center: tuples[i].Center, Insertion: new T3D.Point(x, tuples[i].Insertion.Y + dy, tuples[i].Insertion.Z)));
                    x += space;
                }
                x = TeklaConstants.LAYOUT_MARGIN * 3;
               dy = Math.Abs(height - y) *( (double)2/3); //Get the distance between the current point to the top of the sheet
                for (int i = halfCount; i < count; i++)
                {
                    newTuples.Add((Center: tuples[i].Center, Insertion: new T3D.Point(x, tuples[i].Insertion.Y + dy, tuples[i].Insertion.Z)));
                    x += space;
                }
            }
            return newTuples;
        }
        public static T3D.Point MapToSheet(this  TSD.View view , T3D.Point point,double scale) => new T3D.Point(point.X* scale + view.Origin.X, point.Y* scale + view.Origin.Y, point.Z* scale + view.Origin.Z);
        public static void CreateDetailView(this TSD.Drawing drawing , TSM.Model model,List<T3D.Point> points,double scale,LineDirection dir)
        {
            model.SetPlaneToGlobal();
            TSD.View view = drawing.GetSheet().GetAllViews().ToIEnumerable().ToList()[0] as TSD.View; //temp => next will be selected by viewName
            model.SetPlane(view.ViewCoordinateSystem);
            int start = 65;
            points.GetInsertionTuple(view ,scale ).MapToInsertion(drawing).ForEach(p =>
            {
                view.CreateDetailAt(new T3D.Point(p.Center.X,p.Center.Y-100,p.Center.Z), p.Insertion, ((char)start),model,dir);
                start += 1;
            
            });
            drawing.CommitChanges();
        }
        public static void CreateDetailAt(this TSD.View view,T3D.Point center,T3D.Point insertion,char name ,TSM.Model model,LineDirection dir)
        {
            //Get view in drawing sheet
            TSD.View.ViewAttributes viewAttr = new TSD.View.ViewAttributes("AUTRADetail");
            TSD.DetailMark.DetailMarkAttributes detailAttr = new TSD.DetailMark.DetailMarkAttributes("AUTRA");
            TSD.View detailView;
            TSD.DetailMark detailMark;
            T3D.Point boundary = new T3D.Point(center.X, center.Y + 600, center.Z);
            T3D.Point label = new T3D.Point(boundary.X+100,boundary.Y+100,boundary.Z);
            TSD.View.CreateDetailView(view, center, boundary,label,insertion, viewAttr, detailAttr, out detailView, out detailMark);
            detailMark.Attributes.MarkName = name.ToString();
            if (detailView != null)
            {
               // detailView.RotateViewOnAxisY ( 45);
                detailView.Modify();
                TSD.WeldMark.WeldMarkAttributes weld = new TSD.WeldMark.WeldMarkAttributes("AUTRAWeld");
                detailView.Attributes = new TSD.View.ViewAttributes("AUTRADetail");
                detailView.Modify();
                model.SetPlaneToGlobal();
                model.SetPlane(detailView.ViewCoordinateSystem);
                detailView.GetAllObjects(typeof(TSD.Bolt)).ToIEnumerable().Cast<TSD.Bolt>().ToList().CreateBoltDimensions(detailView,model,center,dir);
            }
        }
        public static (List<TSM.BoltGroup> ModelBolts , List<TSD.Bolt> DrawingBolts) GetModelDrawingBolts(this List<TSD.Bolt> bolts,TSM.Model model)
        {
            //This method is used to convert From Drawing bolts to (ModelBolts & DrawingBolts)
            List<TSM.BoltGroup> modelBolts = new List<TSM.BoltGroup>();
            List<TSD.Bolt> drawingBolts = new List<TSD.Bolt>();
            bolts.ForEach(db=>
            {
                var mb = model.SelectModelObject(db.ModelIdentifier) as TSM.BoltGroup; //Getting Model bolts
                modelBolts.Add(mb);
                drawingBolts.Add(db);
            });
            return (ModelBolts: modelBolts, DrawingBolts: drawingBolts);
        }
        public static TSM.BoltGroup GetBoltsWhichSecInZ(this List<TSM.BoltGroup> bolts,TSD.View detailView,TSM.Model model)
        {
            //This method is used to get Bolts which Secondaries are in Z
            List<TSM.BoltGroup> placeHolder = new List<TSM.BoltGroup>(); // List of bolts that their secondaries are in Z-Direction
            model.SetPlaneToGlobal();
            model.SetPlane(detailView.ViewCoordinateSystem);
            bolts.ForEach(mb =>
            {
                var connection = mb.GetFatherComponent() as TSM.Connection;
                var sec = (connection.GetSecondaryObjects()[0] as TSM.Beam);
                sec.Select();
                var awayVec= sec.GetAwayVector();
                if(awayVec.Cross(new T3D.Vector(0, 0, 1))==new T3D.Vector())
                {
                    placeHolder.Add(mb);
                }
            });

            if (placeHolder.Count == 0) return null;
            else if (placeHolder.Count == 1) return placeHolder[0];
            else
            {
                TSM.BoltGroup final = null;
                placeHolder.ForEach(mb =>
                {
                    var connection = mb.GetFatherComponent() as TSM.Connection;
                    var sec = (connection.GetSecondaryObjects()[0] as TSM.Beam);
                    sec.Select();
                    var awayVec = sec.GetAwayVector();
                    if (awayVec.Cross(new T3D.Vector(0, 1, 0)).GetNormal()== new T3D.Vector(-1,0,0))
                    {
                        final= mb ;
                    }
                });
                return final;
            }
        }
        public static bool InWeak( this TSM.Beam sec,TSM.Beam col ,TSM.Model model)
        {
            model.SetPlaneToGlobal();
            model.SetPlane(col.GetCoordinateSystem());
            sec.Select();
            if (sec.GetVector().Cross(new T3D.Vector(0, 1, 0)) == new T3D.Vector()) return false;
            else return true;
        }
        public static List<TSM.BoltGroup> GetBoltsWhichSecInX(this List<TSM.BoltGroup> bolts, TSD.View detailView, TSM.Model model)
        {
            //This method is used to get Bolts which Secondaries are in X
            List<TSM.BoltGroup> placeHolder = new List<TSM.BoltGroup>(); // List of bolts that their secondaries are in X-Direction
            bolts.ForEach(mb =>
            {
                model.SetPlaneToGlobal();
                model.SetPlane(detailView.ViewCoordinateSystem);
                var connection = mb.GetFatherComponent() as TSM.Connection;
                var sec = (connection.GetSecondaryObjects()[0] as TSM.Beam);
                var main = (connection.GetPrimaryObject() as TSM.Beam);
                sec.Select();
                var awayVec = sec.GetAwayVector();
                model.SetPlaneToGlobal();
                model.SetPlane(detailView.ViewCoordinateSystem);
                var flag = main.AssemblyNumber.Prefix == "C" && sec.InWeak(main,model) ? false : true;
                if (awayVec.Cross(new T3D.Vector(1, 0, 0)) == new T3D.Vector()&&flag)
                {
                    placeHolder.Add(mb);
                }
            });

            return placeHolder;
        }
        public static T3D.Vector GetAwayVector(this TSM.Beam beam)
        {
            //gets the vector from connection node where the view stand to the other node
            if (Math.Abs(beam.StartPoint.Z) <= 0) return new T3D.Vector(beam.EndPoint-beam.StartPoint);
            else return new T3D.Vector(beam.StartPoint - beam.EndPoint);
        }
        public static void CreateBoltDimensions(this List<TSD.Bolt> bolts,TSD.View view,TSM.Model model,T3D.Point connNode,LineDirection dir)
        {
            List<TSM.BoltGroup> modelBolts = new List<TSM.BoltGroup>();
            var mdBolts= bolts.GetModelDrawingBolts(model);
            var boltInZ = mdBolts.ModelBolts.GetBoltsWhichSecInZ(view, model);
            if (boltInZ != null)
            {
                var dist = 55;
                var up = boltInZ.GetPlacementVector(view, model, connNode, dir);
                var editedPoints = boltInZ.GetEditedBoltsPoints(boltInZ.GetFatherComponent() as TSM.Connection);
                TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(null, "AUTRA");
                TSD.StraightDimensionSet inner = new TSD.StraightDimensionSetHandler().CreateDimensionSet(view, editedPoints.Inner, up, dist, attr);
                TSD.StraightDimensionSet outer = new TSD.StraightDimensionSetHandler().CreateDimensionSet(view, editedPoints.Outer, up, dist * 2, attr);
            }
           var boltsInX = mdBolts.ModelBolts.GetBoltsWhichSecInX(view, model);
            boltsInX.ForEach(mb => {
                var dist = 55;
                var up = mb.GetPlacementVector(view, model, connNode, dir);
                var conn = mb.GetFatherComponent() as TSM.Connection;
                var main = conn.GetPrimaryObject() as TSM.Beam;
                var sec = conn.GetSecondaryObjects()[0] as TSM.Beam;
                //if (!(main.AssemblyNumber.Prefix == "C" && sec.AssemblyNumber.Prefix == "SB"))
                //{
                    var editedPoints = mb.GetEditedBoltsPoints(conn);
                    TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(null, "AUTRA");
                    TSD.StraightDimensionSet inner = new TSD.StraightDimensionSetHandler().CreateDimensionSet(view, editedPoints.Inner, up, dist, attr);
                    TSD.StraightDimensionSet outer = new TSD.StraightDimensionSetHandler().CreateDimensionSet(view, editedPoints.Outer, up, dist * 2, attr);
                //}
                
            });
        }
        public static (TSD.PointList Inner , TSD.PointList Outer) GetEditedBoltsPoints(this TSM.BoltGroup bolts,TSM.Connection connection)
        {
            //This method is used to determine the dimensions along bolts (inner) and a dimension line from the begining of the plate to the end
            double edge = 0; //Get edge distance 
            double hp = 0; //Get plate Length
            connection.GetAttribute("rb1", ref edge);
            connection.GetAttribute("hpl1", ref hp);
            TSD.PointList inner = new TSD.PointList();
            TSD.PointList outer = new TSD.PointList();
            if (edge >0&&hp > 0)
            {
                var points = bolts.BoltPositions;
                int count = points.Count;
                var lastBolt = points[0] as T3D.Point;
                var lastPoint = new T3D.Point(lastBolt.X, lastBolt.Y + edge, lastBolt.Z);
                var firstPoint = new T3D.Point(lastPoint.X, lastPoint.Y - hp, lastPoint.Z);
                inner.Add(firstPoint);
                foreach (var p in points)
                {
                    inner.Add(p as T3D.Point);
                }
                inner.Add(lastPoint);
                outer.Add(firstPoint);
                outer.Add(lastPoint);
            }
            return (Inner: inner, Outer: outer);
        }
        public static T3D.Vector GetPlacementVector(this TSM.BoltGroup bolts ,TSD.View detailView, TSM.Model model,T3D.Point center,LineDirection dir)
        {
            model.SetPlaneToGlobal();//Moving Transformation plane to global coordinates
            switch (dir)
            {
                case LineDirection.InX:
                    model.SetPlane(new T3D.CoordinateSystem(center, new T3D.Vector(1, 0, 0), new T3D.Vector(0, 0, 1)));
                    break;
                case LineDirection.InY:
                    model.SetPlane(new T3D.CoordinateSystem(new T3D.Point(center.Z,center.X,center.Y), new T3D.Vector(0, 1, 0), new T3D.Vector(0, 0, 1)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir));
            }
            bolts.Select();
            T3D.Vector vec = null;
            //if (bolts.FirstPosition.Z > 10)
            //{
                vec = bolts.FirstPosition.X < 0 ? new T3D.Vector(-1, 0, 0) : new T3D.Vector(1, 0, 0);
                model.SetPlaneToGlobal();
                model.SetPlane(detailView.ViewCoordinateSystem);
                bolts.Select();

            //}
            return vec;
        }

    }
}

/*
 * Logic: get all secondary parts that are parallel to Z in +ve & -ve direction
 * if(one => Draw dimension)
 * if(two => Take what's in +ve z and leave the other
 */