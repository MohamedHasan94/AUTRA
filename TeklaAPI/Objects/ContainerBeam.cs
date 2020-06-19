using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using TSM=Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;

namespace AUTRA.Tekla
{
  public  class ContainerBeam
    {
        //beam in model
        public TSM.Beam ModelBeam { get; set; } 
        //beam in drawing
        public TSD.Part DrawingBeam { get; set; }
        public T3D.Point Point { get; set; }
        public double X { get; set; }

        public ContainerBeam(TSD.Part part, TSM.Model model)
        {
            DrawingBeam = part;
            Identifier id = part.ModelIdentifier;
            ModelBeam=   model.SelectModelObject(id) as TSM.Beam;
            Point = ModelBeam.GetMidPoint();
        }
        public bool XIsBetween(ContainerBeam other)
        {
            double x = ModelBeam.GetMidPoint().X;
            double xs = Math.Min(other.ModelBeam.StartPoint.X, other.ModelBeam.EndPoint.X);
            double xe = Math.Max(other.ModelBeam.StartPoint.X, other.ModelBeam.EndPoint.X);
            return (x > xs && x < xe);
        }
        public bool YIsBetween(ContainerBeam other)
        {
            double y = ModelBeam.GetMidPoint().Y;
            double ys = Math.Min(other.ModelBeam.StartPoint.Y, other.ModelBeam.EndPoint.Y);
            double ye = Math.Max(other.ModelBeam.StartPoint.Y, other.ModelBeam.EndPoint.Y);
            return (y > ys && y < ye);
        }
        public bool IsEqual(ContainerBeam other) => Point.IsEqual(other.Point);
        public static IComparer<ContainerBeam> SortByX()
        {
            return new CompareByX();
        }
        public static IComparer<ContainerBeam> SortByY()
        {
            return new CompareByY();
        }
        public static IComparer<ContainerBeam> SortByZ()
        {
            return new CompareByZ();
        }
        class CompareByX : IComparer<ContainerBeam>
        {
            public int Compare(ContainerBeam a, ContainerBeam b)
            {
                int result;
                if (a?.ModelBeam?.StartPoint.X > b?.ModelBeam?.StartPoint.X)
                {
                    result = 1;
                }
                else if (a?.ModelBeam.StartPoint.X < b?.ModelBeam.StartPoint.X)
                {
                    result = -1;
                }
                else
                {
                    if (a?.ModelBeam?.StartPoint.Z > b?.ModelBeam?.StartPoint.Z)
                    {
                        result = 1;
                    }
                    else if(a?.ModelBeam?.StartPoint.Z <b?.ModelBeam?.StartPoint.Z)
                    {
                        result= -1;
                    }
                    else
                    {
                        result= 0;
                    }
                }
                return result;
            }
        }
        class CompareByY : IComparer<ContainerBeam>
        {
            public int Compare(ContainerBeam a, ContainerBeam b)
            {
                int result;
                if (a?.ModelBeam?.StartPoint.Y > b?.ModelBeam?.StartPoint.Y)
                {
                    result = 1;
                }
                else if (a?.ModelBeam.StartPoint.Y < b?.ModelBeam.StartPoint.Y)
                {
                    result = -1;
                }
                else
                {
                    if (a?.ModelBeam?.StartPoint.X > b?.ModelBeam?.StartPoint.X)
                    {
                        result = 1;
                    }
                    else if (a?.ModelBeam?.StartPoint.X < b?.ModelBeam?.StartPoint.X)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                return result;
            }
        }
        class CompareByZ : IComparer<ContainerBeam>
        {
            public int Compare(ContainerBeam a, ContainerBeam b)
            {
                int result;
                if (a?.ModelBeam?.StartPoint.Z > b?.ModelBeam?.StartPoint.Z)
                {
                    result = 1;
                }
                else if (a?.ModelBeam.StartPoint.Z < b?.ModelBeam.StartPoint.Z)
                {
                    result = -1;
                }
                else
                {
                    if (a?.ModelBeam?.StartPoint.X > b?.ModelBeam?.StartPoint.X)
                    {
                        result = 1;
                    }
                    else if (a?.ModelBeam?.StartPoint.X < b?.ModelBeam?.StartPoint.X)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                return result;
            }
        }
    }
    
}
