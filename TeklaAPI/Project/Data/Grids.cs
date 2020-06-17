using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTRA.Tekla
{
  public  class Grids
    {
        //list of spacings between grids in X-direction
        public List<double> CXS { get; set; }
        //list of spacings between grids in Y-dirction
        public List<double> CYS { get; set; }
        //list of spacings betweenn grids in Z-direction
        public List<double> CZS { get; set; }

        public IEnumerable<Rectangle> CreateRectangles()
        {
            double aggX = 0;
            double aggY = 0;
            for (int i = 0; i < CXS.Count-1; i++)
            {
                var lx1 = CXS[i];
                var lx2 = CXS[i+1];
                for (int j = 0; j < CYS.Count-1; j++)
                {
                    var ly1 = CYS[j];
                    var ly2 = CYS[j + 1];
                    yield return new Rectangle((aggX + (lx2) / 2), (aggY + (ly2 / 2)), lx2 / 2, ly2 / 2);
                    aggY += ly2;
                }
                aggX += lx2;
                aggY = 0;
            }
        }
    }
}
