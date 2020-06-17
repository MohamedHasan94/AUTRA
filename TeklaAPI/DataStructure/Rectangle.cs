using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3D = Tekla.Structures.Geometry3d;

namespace AUTRA.Tekla
{
  public class Rectangle
    {
        // x & y => are coordinates of rectangle center
        // w & h are distance from center to each boundary
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
        public Rectangle(double totalWidth , double totalHeight)
        {
            X = totalWidth / 2;
            Y = totalHeight / 2;
            Width = X;
            Height = Y;
        }
        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        public bool InsertContains(T3D.Point point)
        {
            return (point.X >= X - Width && point.X < X + Width &&
            point.Y >= Y - Height && point.Y < Y + Height);
        }
        public bool QueryContains(T3D.Point point)
        {
            return (point.X > X - Width && point.X < X + Width &&
            point.Y > Y - Height && point.Y < Y + Height);
        }
        public bool Intersects(Rectangle range)
        {
            return !(range.X - range.Width > X + Width ||
            range.X + range.Width < X - Width ||
            range.Y - range.Height > Y + Height ||
            range.Y + range.Height < Y - Height);
        }
        public (T3D.Point Left, T3D.Point Right) GetXBoundaries(double z) => (new T3D.Point(X - Width, Y, z), new T3D.Point(X + Width, Y, z));
        public (T3D.Point Down, T3D.Point Up) GetYBoundaries(double z) => (new T3D.Point(X, Y-Height, z), new T3D.Point(X, Y+Height, z));

    }
}
