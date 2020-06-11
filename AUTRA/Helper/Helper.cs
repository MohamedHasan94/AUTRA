using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUTRA.Design;

namespace AUTRA.Helper
{
    public static class Helper
    {
        public static void ModifyCoordinates(this List<Node> nodes)
        {
            nodes.ForEach(n =>
            {
                double temp = n.Position.Y;
                n.Position.Y = n.Position.Z;
                n.Position.Z = temp;
            });
        }
        
        public static void ModifyInnerNodes(this List<Beam> beams,List<Node> nodes)
        {
            beams.ForEach(b => {
                b.InnerNodes = new List<Node>();
                nodes.ForEach(n =>
                {
                    if (n.Position.X > b.StartNode.Position.X && n.Position.X < b.EndNode.Position.X && n.Position.Y == b.StartNode.Position.Y)
                    {
                        b.InnerNodes.Add(n);
                    }
                });
            });
        }
    }
}
