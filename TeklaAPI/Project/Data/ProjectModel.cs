using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUTRA.Tekla
{
    public class ProjectModel
    {
        public Grids Grids { get; set; }
        public List<Column> Columns { get; set; }
        public List<Footing> Footings { get; set; }
        public List<Beam> MainBeams { get; set; }
        public List<Beam> SecondaryBeams { get; set; }
        public List<Connection> Connections { get; set; }
    }
}
