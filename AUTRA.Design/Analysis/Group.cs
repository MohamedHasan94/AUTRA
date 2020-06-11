using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Group
    {
        public Group()
        {
            Beams = new List<Beam>();
            DesignResult = new DesignResult();
        }
        public List<Beam> Beams { get; set; }
        public DesignLimitState DesignValues { get; set; }
        public ServiceabilityLimitState ServiceValue { get; set; }
        public Section Section { get; set; }
        public DesignResult DesignResult { get; set; }
    }
}
