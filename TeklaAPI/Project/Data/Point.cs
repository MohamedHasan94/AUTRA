using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTRA.Tekla
{
   public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public static bool operator == (Point p1 , Point p2)
        {
            var x = Math.Abs(p1.X - p2.X) < 1; //in mm
            var y = Math.Abs(p1.Y - p2.Y) < 1; //in mm
            var z = Math.Abs(p1.Z - p2.Z) < 1; //in mm
            return x && y && z;
        }
        public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
        
    }
}
