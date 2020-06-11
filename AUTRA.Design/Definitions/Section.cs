using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //Section Dimensions
        public double H { get; set; }
        public double B { get; set; }
        public double Tw { get; set; }
        public double Tf { get; set; }
        //section properties
        public double Area { get; set; }
        public double Ix { get; set; }
        public double Sx { get; set; }
        public double Rx { get; set; }
        public double Iy { get; set; }
        public double Sy { get; set; }
        public double Ry { get; set; }
        //Material
        public double W { get; set; }
        public Material Material { get; set; }
        //helper properties
        public double DwTw  { get; set; } //dw/tw
        public double CTf { get; set; }   //c/tf
    }
}


