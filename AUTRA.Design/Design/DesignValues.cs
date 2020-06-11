using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
    
    public class DesignLimitState
    {
        public string Combo { get; set; }
        public FrameElement CriticalElement { get; set; }//TODO
        public double Md { get; set; }
        public double Vd { get; set; }
        public double  Nd { get; set; }
    }
    public class ServiceabilityLimitState
    {
        public Beam CriticalBeam { get; set; }//TODO
        public string Combo { get; set; }
        public double WLL { get; set; }
    }
}
