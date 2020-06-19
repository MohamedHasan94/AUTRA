using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3D = Tekla.Structures.Geometry3d;

namespace AUTRA.Tekla
{
    public class QuadTree
    {
        public Rectangle Boundary { get; }
        public int Capacity { get; }
        public List<ContainerBeam> Beams { get; }
        public bool IsDivided { get; set; }
        public QuadTree NorthEast { get; set; }
        public QuadTree NorthWest { get; set; }
        public QuadTree SouthEast { get; set; }
        public QuadTree SouthWest { get; set; }
        public QuadTree(Rectangle boundary, int capacity)
        {
            Boundary = boundary;
            Capacity = capacity;
            Beams = new List<ContainerBeam>();
            IsDivided = false; ;
        }
        public void Subdivide()
        {
            double x = Boundary.X;
            double y = Boundary.Y;
            double w = Boundary.Width;
            double h = Boundary.Height;
            var ne = new Rectangle(x + w / 2, y + h / 2, w / 2, h / 2);
            var nw = new Rectangle(x - w / 2, y + h / 2, w / 2, h / 2);
            var se = new Rectangle(x + w / 2, y - h / 2, w / 2, h / 2);
            var sw = new Rectangle(x - w / 2, y - h / 2, w / 2, h / 2);
            NorthEast = new QuadTree(ne, Capacity);
            NorthWest = new QuadTree(nw, Capacity);
            SouthEast = new QuadTree(se, Capacity);
            SouthWest = new QuadTree(sw, Capacity);
            IsDivided = true;
        }
        public bool Insert(ContainerBeam beam)
        {
            if (!Boundary.InsertContains(beam.Point))
            {
                return false;
            }
            else
            {
                if (Beams.Count < Capacity)
                {
                    foreach (var b in Beams)
                    {
                        if (b.IsEqual(beam)) return false;
                    }
                    Beams.Add(beam);
                    return true;
                }
                else
                {
                    if (!IsDivided)
                    {
                        Subdivide();
                    }
                    if (NorthEast.Insert(beam)) return true;
                    if (NorthWest.Insert(beam)) return true;
                    if (SouthEast.Insert(beam)) return true;
                    if (SouthWest.Insert(beam)) return true;
                }
                return true;
            }
        }
        private void Query(Rectangle range, QuadTree qtree, ref List<ContainerBeam> result)
        {
            if (qtree != null)
            {
                foreach (var b in qtree.Beams)
                {
                    if (range.QueryContains(b.Point)) result.Add(b);
                }
                if (IsDivided)
                {
                    Query(range, qtree.NorthEast, ref result);
                    Query(range, qtree.NorthWest, ref result);
                    Query(range, qtree.SouthEast, ref result);
                    Query(range, qtree.SouthWest, ref result);
                }
            }
            
        }
        public List<ContainerBeam> Query(Rectangle range)
        {
            if (!Boundary.Intersects(range)) return null;
            List<ContainerBeam> beams = new List<ContainerBeam>();
            Query(range, this, ref beams);
            return beams;
        }
    }
}
