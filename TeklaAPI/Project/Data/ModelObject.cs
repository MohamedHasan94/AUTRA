using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AUTRA.Tekla
{
   public class ModelObject
    {
        //public TSM.ModelObject TeklaObject { get; set; }
        public int Id{ get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        public string Profile { get; set; }
        public string Finish { get; set; }
        public string AssemblyPrefix { get; set; }
        public string Class { get; set; }
    }
}
