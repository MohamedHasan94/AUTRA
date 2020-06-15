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
        public override string ToString()
        {
            string result = "";
            switch (Name)
            {
                case SteelType.ST_37:
                    result = "St-37";
                    break;
                case SteelType.ST_44:
                    result = "St-44";
                    break;
                case SteelType.ST_52:
                    result = "St-52";
                    break;
            }
            return result;
        }
    }
}
