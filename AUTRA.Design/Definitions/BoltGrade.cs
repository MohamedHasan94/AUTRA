using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class BoltGrade
    {
        public string Name { get; set; }
        public double  Fyb { get; set; }
        public double  Fub { get; set; }
        public double ShearFactor { get; set; } //factor which is basically either 0.25 or 0.2
    }
}
