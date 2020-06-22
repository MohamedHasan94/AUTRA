using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3D = Tekla.Structures.Geometry3d;

namespace AUTRA.Tekla
{
    
   public static class Geometry
    {
        public static List<T3D.Point> FilterNodesByHeight(this List<T3D.Point> points , double val)
        {
            return points.Where(p => Math.Abs(p.Z - val) < 1).ToList();
        }
        public static List<T3D.Point> SwitchCoords(this List<T3D.Point> points,LineDirection dir)
        {
            var lst = new List<T3D.Point>();
            switch (dir)
            {
                case LineDirection.InX:
                    //x=>x , y=>z , z=>y
                    //switch only y with z
                    points.ForEach(p => lst.Add(new T3D.Point(p.X,p.Z,p.Y)));
                    break;
                case LineDirection.InY:
                    //x=>y , y=>z , z=>x
                    //switch only y with z
                    points.ForEach(p => lst.Add(new T3D.Point(p.Y, p.Z, p.X)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir));
            }
            return lst;
        }
        public static List<Point> GetOnlyUnique(this List<Connection> conns)
        {
            List<Point> points = new List<Point>();
            points.Add(conns[0].Node);
            bool flag = true;
            for (int i = 1; i < conns.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (conns[j].Node == conns[i].Node) flag = false;
                }
                if (flag) points.Add(conns[i].Node);
                flag = true;
            }
            return points;
        }
        public static List<T3D.Point> QueryPointsOnLine(this List<T3D.Point> points,LineDirection dir , double val)
        {
            switch (dir)
            {
                case LineDirection.InX:
                    return points.Where(p => Math.Abs(p.Y - val) < 1/*1mm*/).ToList();
                case LineDirection.InY:
                  return points.Where(p => Math.Abs(p.X - val) < 1/*1mm*/).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir));
            }
        }
        public static IEnumerable<T3D.Point> ToTeklaPoints(this List<Point> points)
        {
            foreach (var p in points)
            {
                yield return new T3D.Point(p.X, p.Y, p.Z);
            }
        }
        public static (List<ContainerBeam> ParallelX , List<ContainerBeam> ParallelY ) GetParallelXY(this List<ContainerBeam> beams)
        {
            List<ContainerBeam> px = new List<ContainerBeam>();
            List<ContainerBeam> py = new List<ContainerBeam>();
            beams.ForEach(b =>
            {
                if (b.ModelBeam.InX()) px.Add(b);
                else if (b.ModelBeam.InY()) py.Add(b);
            });
            return (px.Count > 0 ? px : null, py.Count > 0 ? py : null);
        }
        public static (double Length ,double Width ,double Height) GetLWH(this T3D.AABB boundingBox)
        {
            double length = Math.Abs(boundingBox.MaxPoint.X - boundingBox.MinPoint.X);
            double width = Math.Abs(boundingBox.MaxPoint.Y - boundingBox.MinPoint.Y);
            double height = Math.Abs(boundingBox.MaxPoint.Z - boundingBox.MinPoint.Z);
            var lwh = (Length:length, Width:width, Height:height);
            return lwh;
        }
    }
}
