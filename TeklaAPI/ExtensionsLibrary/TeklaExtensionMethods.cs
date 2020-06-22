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

        


    }
}
