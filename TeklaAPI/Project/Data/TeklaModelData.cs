using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTRA.Tekla
{
   public class TeklaModelData
    {
        public ProjectProperties ProjectProperties { get; set; }
        public ProjectModel Model { get; set; }
        public TeklaModelData()
        {
            Model = new ProjectModel();
        }
    }
    public struct ProjectProperties
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Designer { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
