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
