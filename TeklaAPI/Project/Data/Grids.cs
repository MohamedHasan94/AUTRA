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
    }
}
