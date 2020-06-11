using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
namespace AUTRA.Design
{
   public class StrainingAction
    {
        #region Constructor
        public StrainingAction()
        {
            Stations = new List<Station>();
        }
        #endregion
        public LoadCombination Combo { get; set; }
        public LoadPattern Pattern { get; set; }
        public List<Station> Stations { get; set; }

    }



}
