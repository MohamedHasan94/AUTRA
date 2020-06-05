using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AUTRA.Models.StructuralElements;
using AUTRA.Models.Definitions;

namespace AUTRA.Models.Main
{
    public class Project
    {
        public ProjectProperties ProjectProperties { get; set; }
        public List<Node> Nodes { get; set; }
        public Material Material { get; set; }
        public List<Section> Sections { get; set; }
        public List<Beam> SecondaryBeams { get; set; }
        public List<Beam> MainBeams { get; set; }
        public List<Column> Columns { get; set; }
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
