using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Models.StructuralElements;
using AUTRA.Models.Definitions;
using AUTRA.Models.Design;
using AUTRA.Models.Reports;


namespace AUTRA.Models.Analysis
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
