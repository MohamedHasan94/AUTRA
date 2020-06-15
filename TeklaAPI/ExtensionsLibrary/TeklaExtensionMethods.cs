using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using Tekla.Structures.Filtering;
using Tekla.Structures.Filtering.Categories;
using TSM=Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;

namespace AUTRA.Tekla
{
    public static class TeklaExtensionMethods
    {
        public static TSM.Position.RotationEnum GetColumnDirection(this List<TSM.Beam> mainBeams)
        {
            var beam=mainBeams.OrderByDescending(b=>b.GetLengthSquared()).First();
           var vec= beam.GetVector();
            var xVec = new T3D.Vector(1, 0, 0);
            if (vec.Cross(xVec) == new T3D.Vector())
                return TSM.Position.RotationEnum.TOP;
            return TSM.Position.RotationEnum.FRONT;
        }
        public static List<TSM.Beam> SelectColumns(this List<TSM.Beam> parts)
        {
            //This method is used to select columns from list of beams
            List<TSM.Beam> columns = parts.Where((p) =>
            {
                T3D.Vector lenVec = p.GetVector();
                var zVec = new T3D.Vector(0, 0, 1);
                return lenVec.Cross(zVec) == new T3D.Vector() ? true : false;
            }).ToList();
            return columns;
        }
        

        /// <summary>
        /// extended method to change from 'DrawingObjectenumerator' to 'List'
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static List<TSD.DrawingObject> ToList(this TSD.DrawingObjectEnumerator enumerator)
        {
            List<TSD.DrawingObject> drawingObjs = new List<TSD.DrawingObject>();
            while (enumerator.MoveNext())
            {
                var drawingObject = enumerator.Current;
                if (drawingObject == null)
                    continue;
                drawingObjs.Add(drawingObject);
            }
            return drawingObjs;
        }

        #region Part
        public static Hashtable GetStringProperties(this TSM.Part part, ArrayList list)
        {
            var values = new Hashtable();
            bool result = part.GetStringReportProperties(list, ref values);
            if (result == false)
                return null;

            return values;
        }

        public static Hashtable GetDoubleProperties(this TSM.Part part, ArrayList list)
        {
            var values = new Hashtable();
            bool result = part.GetDoubleReportProperties(list, ref values);
            if (result == false)
                return null;

            return values;
        }
        public static Hashtable GetIntProperties(this TSM.Part part, ArrayList list)
        {
            var values = new Hashtable();
            bool result = part.GetIntegerReportProperties(list, ref values);
            if (result == false)
                return null;

            return values;
        }
        #endregion

        #region  Beam
        public static bool InZDirection(this TSM.Beam beam) => beam.GetVector().Cross(new T3D.Vector(0, 0, 1)) == new T3D.Vector();
        /// <summary>
        /// Get square of length of given beam
        /// </summary>
        /// <param name="beam"></param>
        /// <returns></returns>
        public static double GetLengthSquared(this TSM.Beam beam) => ((beam.StartPoint.X - beam.EndPoint.X) * (beam.StartPoint.X - beam.EndPoint.X)) + ((beam.StartPoint.Y - beam.EndPoint.Y) * (beam.StartPoint.Y - beam.EndPoint.Y)) + ((beam.StartPoint.Z - beam.EndPoint.Z) * (beam.StartPoint.Z - beam.EndPoint.Z));
        /// <summary>
        /// Get vector from beam object
        /// </summary>
        /// <param name="beam"></param>
        /// <returns></returns>
        public static T3D.Vector GetVector(this TSM.Beam beam) => new T3D.Vector(beam.EndPoint - beam.StartPoint);
        #endregion


    }
}
