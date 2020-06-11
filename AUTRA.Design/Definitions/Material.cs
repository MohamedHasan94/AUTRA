using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Material
    {
        #region Properties
        public SteelType Name { get; set; } 
        public double E { get; set; }
        public double Fy { get; set; }
        public double Fu { get; set; }
        public double Density { get; set; }
        #endregion
       
    }
}
