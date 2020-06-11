using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Bolt
    {
        //all dimension is in cm
        public BoltGrade Grade { get; set; }
        public string Name { get; set; }
        public double Dia { get; set; }
        public double HoleDia { get; set; }
        public double Ag { get; set; }
        public double As { get; set; }
    }
}
