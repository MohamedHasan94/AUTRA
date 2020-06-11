using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
    public class Point
    {
        public Point()
        {
            X = Y = Z = 0;
        }
        public  double X  { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static bool operator == (Point p1,Point p2)
        {
            bool result1 = Math.Abs(p1.X - p2.X)<Tolerance.DIST_TOL;
            bool result2 = Math.Abs(p1.Y - p2.Y)<Tolerance.DIST_TOL; 
            bool result3 = Math.Abs(p1.Z - p2.Z)<Tolerance.DIST_TOL;
            return result1 && result2 && result3;
        }
        public static bool operator !=(Point p1, Point p2)=> !(p1 == p2);
        public double Distance(Point p)
        {
            double dx = X - p.X;
            double dy = Y - p.Y;
            double dz = Z - p.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public override string ToString() => string.Format($"({X},{Y},{Z})");
        public Point Cross(Point other)
        {
            Point a = this;
            Point b = other;
            return new Point()
            {
                X = a.Y * b.Z - a.Z * b.Y,
                Y=(a.X*b.Z-b.X*a.Z)*-1,
                Z= a.X*b.Y-b.X*a.Y
            };
        }
        public bool IsOnLine(FrameElement ele)
        {
            Point start = ele.StartNode.Position;
            Point end = ele.EndNode.Position;
            bool resultX = IsBetween(X, start.X, end.X);
            bool resultY= IsBetween(Y, start.Y, end.Y);
            bool resultZ = IsBetween(Z, start.Z, end.Z);

            return resultX && resultY && resultZ;
        }
        private static bool IsBetween(double p , double s , double e)
        {
            if (Math.Abs(e - s) < Tolerance.DIST_TOL)
            {
                return Math.Abs(p - s) < Tolerance.DIST_TOL;
            }
            double t = (p - s) / (e - s);
            return (Math.Abs(t - 1) < Tolerance.DIST_TOL || Math.Abs(t) < Tolerance.DIST_TOL || (t > 0 && t < 1)) ?true:false;
        }
    }
   public class Node
   {
        public Point Position { get; set; }
        public List<PointLoad> PointLoads { get; set; }
        public List<Connection> Connections { get; set; }
    }
}
