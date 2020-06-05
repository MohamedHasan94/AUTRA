using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq;
using AUTRA.Models.Analysis;
using AUTRA.Models.Definitions;


namespace AUTRA.Models.StructuralElements
{
   public class FrameElement
    {
        public FrameElement()
        {
            StrainingActions = new List<StrainingAction>();
            CombinedSA = new List<StrainingAction>();
        }
        public int Id { get; set; }  
        public Node StartNode { get; set; }
        public Node EndNode { get; set; }
        public Section Section { get; set; }
        public Node LocalStartNode { get; set; }
        public Node LocalEndNode { get; set; }  
        public List<LineLoad> LineLoads { get; set; }
        public List<Node> InnerNodes { get; set; }
        public List<StrainingAction> StrainingActions { get; set; }
        public List<StrainingAction> CombinedSA { get; set; }
        public  double Length { get; set; }

        
    }
}
