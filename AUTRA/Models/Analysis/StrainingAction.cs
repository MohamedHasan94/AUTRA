using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using AUTRA.Models.EnumHelpers;

namespace AUTRA.Models.Analysis
{
   public class StrainingAction
    {
        #region Constructor
        public StrainingAction()
        {
            Stations = new List<Station>();
        }
        #endregion
        public LoadCombination Combo { get; set; }
        public LoadPattern Pattern { get; set; }
        public List<Station> Stations { get; set; }

        public double GetMaxShear() => Stations.Max(s => s.GetMaxShear());

        #region Sorting Methods
        public static IComparer<StrainingAction> SortMomentDescendingly() => new CompareByM();
        public static IComparer<StrainingAction> SortNormalAscendingly() => new CompareByN();

        class CompareByN : IComparer<StrainingAction>
        {
            public int Compare( StrainingAction x, StrainingAction y)
            {
                x.Stations.Sort(Station.SortNormalAscendingly());
                Station a = x.Stations[0];
                y.Stations.Sort(Station.SortNormalAscendingly());
                Station b = y.Stations[0];
                if (a.No < b.No) return -1;
                else if (a.No > b.No) return 1;
                else return 0;
            }
        }

        class CompareByM : IComparer<StrainingAction>
        {
            public int Compare(StrainingAction x, StrainingAction y)
            {
                x.Stations.Sort(Station.SortMomentDescendingly());
                Station a = x.Stations[0];
                y.Stations.Sort(Station.SortMomentDescendingly());
                Station b = y.Stations[0];
                if (Math.Abs(a.Mo) > Math.Abs(b.Mo))
                {
                    return -1;
                }
                else if (a.Mo < b.Mo)
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
