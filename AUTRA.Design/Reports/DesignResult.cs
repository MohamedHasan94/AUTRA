using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class DesignResult
    {
        public double Fcact { get; set; }
        public double Fcall { get; set; }
        public double Fbact  { get; set; }
        public double  Fball { get; set; }
        public double Qact { get; set; }
        public double Qall { get; set; }
        public double Dact { get; set; }
        public double Dall { get; set; }
        public string Lambda { get; set; }
        public string WebLocalBuckling { get; set; }
        public string FlangeLocalBuckling { get; set; }
        public string  Lu { get; set; }
    }
}
