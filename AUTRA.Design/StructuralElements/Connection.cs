using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Connection
    {
        public SimpleConnection SimpleConnection { get; set; }
        public FrameElement MainPart { get; set; }
        public FrameElement SecondaryPart { get; set; }
        public Point Position { get; set; }
    }
}
