using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Group
    {
        public Group()
        {
            Elements = new List<FrameElement>();
            DesignResult = new DesignResult();
        }
        public List<FrameElement> Elements { get; set; }
        public DesignLimitState DesignValues { get; set; }
        public ServiceabilityLimitState ServiceValue { get; set; }
        public Section Section { get; set; }
        public DesignResult DesignResult { get; set; }
        public SimpleConnection Connection { get; set; }

        public void AssignSectionToElement() => Elements.ForEach(e => e.Section = Section);
        
    }
}
