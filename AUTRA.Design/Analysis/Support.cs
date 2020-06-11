using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
    public class Reaction
    {
        public LoadCombination Combo { get; set; }
        public LoadPattern Pattern { get; set; }
        public double Rv { get; set; }
        public double Rh { get; set; }
        public double Rm { get; set; }
    }

    public  class Support
    {
        public Support(Node node)
        {
            Node = node;
            Reactions = new List<Reaction>();
        }
        public Node Node { get; set; }
        public List<Reaction> Reactions { get; set; }
    }
}
