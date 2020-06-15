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

        public double GetTopDistance()
        {
            //This method is used to calculate the distance from edge of the plate to top of Beam Section
            return  Math.Ceiling(((SecondaryPart.Section.H * 10 - SimpleConnection.Length) * ((double)2 / 3)));
        }
    }
}
