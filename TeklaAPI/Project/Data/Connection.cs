using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;

namespace AUTRA.Tekla
{
   public class Connection
    {
        public Point Node { get; set; }
        public TSM.Beam Main { get; set; }
        public TSM.Beam Secondary { get; set; }
        public string MainPartId { get; set; }
        public string SecondaryPartId { get; set; }
        public string ConnectionNumber { get; set; } //Number of connection in Tekla
        public double Top { get; set; } //Distance from edge of plate to Top of Beam
        public double Hp { get; set; }  //Height of plate
        public double Tp { get; set; }
        public double Edge { get; set; }
        public string PitchLayout { get; set; } //Pitch Layout
        public double  Dia { get; set; }
        public string BoltType { get; set; } 
        public double Sw { get; set; }
    }
}
