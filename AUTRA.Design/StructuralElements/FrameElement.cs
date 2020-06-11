using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;

namespace AUTRA.Design
{
   public class FrameElement
    {
        public FrameElement()
        {
            StrainingActions = new List<StrainingAction>();
            CombinedSA = new List<StrainingAction>();
        }
        public int Id { get; set; }  
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public Section Section { get; set; }
        public Node LocalStartNode { get; set; }//TODO:To be removed
        public Node LocalEndNode { get; set; } //TODO:To be removed 
        public List<LineLoad> LineLoads { get; set; }
        public List<Node> InnerNodes { get; set; }
        public List<StrainingAction> StrainingActions { get; set; }
        public List<StrainingAction> CombinedSA { get; set; }
        public  double Length { get; set; }

        public static IComparer<T> SortByID<T>() where T:FrameElement => new CompareByID<T>(); 
        class CompareByID<T> : IComparer<T> where T : FrameElement
        {
            public int Compare([AllowNull] T x, [AllowNull] T y)
            {
                if (x.Id < y.Id) return -1;
                else if (x.Id > y.Id) return 1;
                else return 0;
            }
        }
        public Point GetVector() 
        {
            Point start = StartNode.Position;
            Point end = EndNode.Position;
            return new Point {
                X=end.X-start.X,
                Y=end.Y-start.Y,
                Z=end.Z-start.Z
            };
        }

    }
    public class Beam:FrameElement
    {
        #region Sorting
        public static IComparer<Beam> SortMomentDescendingly() => new CompareByM();
        class CompareByM : IComparer<Beam>
        {
            public int Compare( Beam x,  Beam y)
            {
                double a = x.CombinedSA.GetMaxMoment();
                double b = y.CombinedSA.GetMaxMoment();
                if (a > b) return -1;
                else if (a < b) return 1;
                else return 0;
            }
        }
        #endregion

    }
    public class Column : FrameElement
    {
        public static IComparer<Column> SortNormalAscendingly() => new CompareByN();
        class CompareByN : IComparer<Column>
        {
            public int Compare( Column x, Column y)
            {
               double a= x.CombinedSA.GetMaxCompression();
               double b= y.CombinedSA.GetMaxCompression();
                if (a < b) return -1;
                else if (a > b) return 1;
                else return 0;
            }
        }

    }
}
