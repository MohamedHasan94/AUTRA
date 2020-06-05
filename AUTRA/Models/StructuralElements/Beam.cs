using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUTRA.Models.Analysis;

namespace AUTRA.Models.StructuralElements
{
    public class Beam : FrameElement
    {
        #region Sorting
        public static IComparer<Beam> SortMomentDescendingly() => new CompareByM();
        class CompareByM : IComparer<Beam>
        {
            public int Compare(Beam x, Beam y)
            {
                x.CombinedSA.ForEach(c => c.Stations = c.Stations.OrderByDescending(s => Math.Abs(s.Mo)).ToList()); //Sorted the stations inside every combo descendingly by moment
                x.CombinedSA = x.CombinedSA.OrderByDescending(c => Math.Abs(c.Stations[0].Mo)).ToList(); //Sorted the combos according to the first station's moment(the maximum one) 
                double a = Math.Abs(x.CombinedSA[0].Stations[0].Mo);

                y.CombinedSA.ForEach(c => c.Stations = c.Stations.OrderByDescending(s => Math.Abs(s.Mo)).ToList());
                y.CombinedSA = y.CombinedSA.OrderByDescending(c => Math.Abs(c.Stations[0].Mo)).ToList();
                double b = Math.Abs(y.CombinedSA[0].Stations[0].Mo);

                if (a > b)
                {
                    return -1;
                }
                else if (a < b)
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
