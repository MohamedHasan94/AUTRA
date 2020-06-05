using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUTRA.Models.Analysis;

namespace AUTRA.Models.StructuralElements
{
    public class Column : FrameElement
    {
        public static IComparer<Column> SortNormalAscendingly() => new CompareByN();
        class CompareByN : IComparer<Column>
        {
            public int Compare(Column x, Column y)
            {
                //Normal force is -ve (compression), Assending order gives the maximum comp (least by sign) first
                x.CombinedSA.ForEach(c => c.Stations = c.Stations.OrderBy(s => s.No).ToList());
                x.CombinedSA.OrderBy(c => c.Stations[0].No);
                double a = x.CombinedSA[0].Stations[0].No;

                y.CombinedSA.ForEach(c => c.Stations = c.Stations.OrderBy(s => s.No).ToList());
                y.CombinedSA.OrderBy(c => c.Stations[0].No);
                double b = y.CombinedSA[0].Stations[0].No;

                if (a < b) return -1;
                else if (a > b) return 1;
                else return 0;
            }
        }

    }
}
