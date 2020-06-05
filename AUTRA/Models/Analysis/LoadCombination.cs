using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AUTRA.Models.EnumHelpers;

namespace AUTRA.Models.Analysis
{
  public  class LoadCombination
    {
        public LoadCombination(string name)
        {
            Name = name;
            Combo = new List<FactoredPattern>();
        }

        public string Name { get; set; }
        public List<FactoredPattern> Combo { get;  }

        public bool Add(FactoredPattern factoredPattern)
        {
            bool result = false;
          var temp =  Combo.FirstOrDefault(fp => fp.Pattern == factoredPattern.Pattern);
            if (temp == null)
            {
                Combo.Add(factoredPattern);
                result = true;
            }
            return result;
        }
        public bool Delete(LoadPattern pattern)
        {
            bool result = false;
            var temp = Combo.FirstOrDefault(fp => fp.Pattern == pattern);
            if (temp != null)
            {
                Combo.Remove(temp);
                result = true;
            }
            return result;
        }
        public void Modify(FactoredPattern factoredPattern)
        {
            var temp = Combo.FirstOrDefault(fp => fp.Pattern == factoredPattern.Pattern);
            if (temp != null)
                Combo.Remove(temp);
                
            Combo.Add(factoredPattern);
        }
    }
}
