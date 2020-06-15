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
    class SecondaryBeam
    {
        //beam in model
        public TSM.Beam ModelBeam { get; set; } 
        //beam in drawing
        public TSD.Part DrawingBeam { get; set; }

        public double X { get; set; }

        public SecondaryBeam(TSD.Part part, TSM.Model model)
        {
            DrawingBeam = part;
            Identifier id = part.ModelIdentifier;
            ModelBeam=   model.SelectModelObject(id) as TSM.Beam;
        }
        public static IComparer<SecondaryBeam> SortByX()
        {
            return new CompareByX();
        }
        public static IComparer<SecondaryBeam> SortByY()
        {
            return new CompareByY();
        }
        public static IComparer<SecondaryBeam> SortByZ()
        {
            return new CompareByZ();
        }
        class CompareByX : IComparer<SecondaryBeam>
        {
            public int Compare(SecondaryBeam a, SecondaryBeam b)
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
        class CompareByY : IComparer<SecondaryBeam>
        {
            public int Compare(SecondaryBeam a, SecondaryBeam b)
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
        class CompareByZ : IComparer<SecondaryBeam>
        {
            public int Compare(SecondaryBeam a, SecondaryBeam b)
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
