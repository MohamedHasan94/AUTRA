using AUTRA.Tekla;
using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
    public class Project
    {
        //Things will be binded with THREE.js
        public ProjectProperties ProjectProperties { get; set; }
        public List<Node> Nodes { get; set; }
        public Material Material { get; set; }
        public List<Section> Sections { get; set; }
        public List<Beam> SecondaryBeams { get; set; }
        public List<Beam> MainBeams { get; set; }
        public List<Column> Columns { get; set; }
        public List<Support> Supports { get; set; }
        public Grids Grids { get; set; }
    }

    public struct Grids
    {
        public double[] CXS { get; set; }
        public double[] CYS { get; set; }
        public double[] Levels { get; set; }

        //For model reading from json in openning
        public double[] CoordX { get; set; }
        public double[] CoordZ { get; set; }
    }
}
