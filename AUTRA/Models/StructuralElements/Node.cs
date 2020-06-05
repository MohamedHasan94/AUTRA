using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Models.Analysis;


namespace AUTRA.Models.StructuralElements
{
    public class Node
    {
        public Point Position { get; set; }
        public List<PointLoad> PointLoads { get; set; }
    }

    public class Point
    {
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double Distance(Point p)
        {
            double dx = X - p.X;
            double dy = Y - p.Y;
            double dz = Z - p.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        public override string ToString() => string.Format($"({X},{Y},{Z})");

    }
}
