using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Tekla;
using AUTRA.Design;
namespace AUTRA
{
    public class Project
    {
        //Things will be binded with THREE.js
        public ProjectProperties ProjectProperties { get; set; }
        public List<Node> Nodes { get; set; }
        public Material Material { get; set; }
        public List<Section> Sections { get; set; }
        public List<Design.Beam> SecondaryBeams { get; set; }
        public List<Design.Beam> MainBeams { get; set; }
        public List<Design.Column> Columns { get; set; }
    }


   
}
