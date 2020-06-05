using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AUTRA.Models.Analysis
{
   public class Station
    {
        public double X { get; set; }
        public double Mo { get; set; }
        public double No { get; set; }
        public double Vo { get; set; }
        public double Vf { get; set; }

        public double GetMaxShear() => Math.Max(Math.Abs(Vo), Math.Abs(Vf));
        #region Sorting
        public static IComparer<Station> SortMomentDescendingly() => new CompareByM();
        public static IComparer<Station> SortNormalAscendingly() => new CompareByN();
        class CompareByN : IComparer<Station>
        {
            public int Compare( Station x,  Station y)
            {
                //Sort Normal ascendingly from smallest to largest (from compression to tension) beacause basically compression is with negative value
                if (x.No < y.No) return -1;
                else if (x.No > y.No) return 1;
                else return 0;
            }
        }
        class CompareByM : IComparer<Station>
        {
            //sort stations descendingly by moment
            public int Compare(Station x, Station y)
            {
                if (Math.Abs(x.Mo) > Math.Abs(y.Mo))
                {
                    return -1;
                }
                else if (x.Mo < y.Mo)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion



    }
}
