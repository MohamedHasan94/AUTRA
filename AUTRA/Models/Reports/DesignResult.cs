using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Models.Reports
{
   public class DesignResult
    {
        public double Fbact  { get; set; }
        public double  Fball { get; set; }
        public double Qact { get; set; }
        public double Qall { get; set; }
        public double Dact { get; set; }
        public double Dall { get; set; }
        public string WebLocalBuckling { get; set; }
        public string FlangeLocalBuckling { get; set; }
        public string  Lu { get; set; }
    }
}
