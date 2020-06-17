using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD = Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
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
        public static void CreateDimsFromPointList(this TSD.PointList ps)
        {

        }
        public static TSD.PointList GetPointList(this List<T3D.Point> points)
        {
            TSD.PointList ps = new TSD.PointList();
            points.ForEach(p => ps.Add(p));
            return ps;
        }
        public static TSD.PointList GetPointList(this List<SecondaryBeam> beams)
        {
            TSD.PointList ps = new TSD.PointList();
            beams.ForEach(b => ps.Add(b.ModelBeam.GetMidPoint()));
            return ps;
        }
        public static TSD.Grid GetGrid(this TSD.Drawing drawing) => drawing.GetSheet().GetAllObjects(typeof(TSD.Grid)).ToIEnumerable().OfType<TSD.Grid>().FirstOrDefault();
        public static List<TSD.GridLine> GetGridLines(this TSD.Grid grid) => grid.GetObjects().ToIEnumerable().OfType<TSD.GridLine>().ToList();
        public static IEnumerable<SecondaryBeam> GetModelBeams(this List<TSD.Part> parts ,TSM.Model model ,string assemblyPrefix)
        {
            foreach (TSD.Part part in parts)
            {
                SecondaryBeam temp = new SecondaryBeam(part, model);
                if (temp.ModelBeam != null && temp.ModelBeam.AssemblyNumber.Prefix == assemblyPrefix)
                {
                    temp.ModelBeam.Select();
                    yield return temp;
                }
            }
        }
        public static IEnumerable<TSD.DrawingObject> ToIEnumerable(this TSD.DrawingObjectEnumerator enumerator)
        {
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
            handler.SetActiveDrawing(ga);
            return ga;
        }
        public static TSD.View CreateView(this TSD.Drawing drawing , T3D.CoordinateSystem coords , T3D.AABB box,double scale ,string viewName , string attributeFile)
        {
            var view = new TSD.View(drawing.GetSheet(), coords, coords, box);
            view.Origin = new T3D.Point((drawing.Layout.SheetSize.Width / 2) - box.GetLWH().Length*scale / 2, (drawing.Layout.SheetSize.Height / 2) - box.GetLWH().Width * scale / 2);//TODO:To be revisted
            view.Name = viewName;
            //setting view attributes
            TSD.View.ViewAttributes viewAttributes = new TSD.View.ViewAttributes(attributeFile);
            viewAttributes.Scale = scale;
            view.Attributes = viewAttributes;
            //insert view in DB
            view.Insert();
            return view;
        }
    }
}
