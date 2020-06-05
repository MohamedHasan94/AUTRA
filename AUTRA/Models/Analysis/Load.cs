using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Models.EnumHelpers;

namespace AUTRA.Models.Analysis
{
    public abstract class Load
    {
        public Load()
        {

        }
        public Load(double mag , LoadPattern pattern)
        {
            Magnitude = mag;
            Pattern = pattern;
        }
        //copy constructor
        public Load(Load other) : this(other.Magnitude, other.Pattern) { }

        public virtual Load Clone()=>null;
        public double Magnitude { get; set; }
        public LoadPattern Pattern { get; set; }
    }
    public class LineLoad : Load
    {
        public LineLoad()
        {

        }
        public LineLoad(double mag, LoadPattern pattern) : base(mag, pattern) { }
        public LineLoad(LineLoad other) : base(other) { }
        public override Load Clone() => new LineLoad(this);
        public static List<LineLoad> CreateZeroLineLoadList(List<LoadPattern> patterns)
        {
            List<LineLoad> loads = new List<LineLoad>();
            foreach (var pattern in patterns)
            {
                loads.Add(new LineLoad(0, pattern));
            }
            return loads;
        }
    }
    public class PointLoad : Load
    {
        public PointLoad()
        {
                
        }
        
        public PointLoad(double mag, LoadPattern pattern) : base(mag, pattern) { }
        public PointLoad(PointLoad other) : base(other) { }
        public override Load Clone() => new PointLoad(this);
        public static List<PointLoad> CreateZeroPointLoadList(List<LoadPattern> patterns)
        {
            List<PointLoad> loads = new List<PointLoad>();
            foreach (var pattern in patterns)
            {
                loads.Add(new PointLoad (0,pattern));
            }
            return loads;
        }
    }
}
