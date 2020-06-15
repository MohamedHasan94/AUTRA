using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTRA.Tekla
{
    public class Beam : ModelObject
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
    }
}
