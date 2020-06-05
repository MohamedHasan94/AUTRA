using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AUTRA.Models.StructuralElements;
using AUTRA.Models.EnumHelpers;
using AUTRA.Models.Design;

namespace AUTRA.Models.Analysis
{
    public static class Analyze
    {

        public static void SolveBeam(Beam beam, List<LoadPattern> patterns)
        {
            //Used to Define Stations For beam and Calculate the straining actions associated with the beam
            DefineStations(beam, patterns);
            SolveLineLoad(beam, patterns);
            SolvePointLoad(beam, patterns);
        }

        public static void SolveColumn(Column column, List<LoadPattern> patterns)
        {
            /*
             * 1-Define two stations one at start and one at end point
             * 2-solve for point load 
             * 3-solve for line load(Own weight)
             */
            DefineStations(column, patterns);
            //List<PointLoad> pointLoads= column.EndNode.PointLoads; 
            List<PointLoad> pointLoads = column.StartNode.PointLoads;//Must be switched back after front end mod!!!
            foreach (var load in pointLoads)
            {
                var mag = load.Magnitude;
                var pattern = load.Pattern;
                StrainingAction sa = column.StrainingActions.FirstOrDefault(a => a.Pattern == pattern);
                double rs, re;

                //////////////////Change
                if (column.EndNode.PointLoads.Count == 0) //check if lower node has no loads in it
                {
                    column.EndNode.PointLoads = PointLoad.CreateZeroPointLoadList(patterns);
                }
                rs = re = column.StartNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude;
                column.EndNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += rs;
                double w = 0;
                if (pattern == LoadPattern.DEAD)
                {
                    w = -1 * column.Section.W / 1000;//w=> will be the weight of column only in Dead load pattern otherwise =0
                    rs = column.EndNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += w * column.Length; //add own weight of column to the reaction at base
                }
                foreach (var station in sa.Stations)
                {
                    station.No += re + w * station.X;
                }
            }
        }
        public static void SolveReaction(Support support)
        {
            //used to calculate Reactions on support for Load Patterns
            List<PointLoad> loads = support.Node.PointLoads;
            foreach (var load in loads)
            {
                double mag = load.Magnitude;
                LoadPattern pattern = load.Pattern;
                Reaction react = new Reaction()
                {
                    Combo = null,
                    Pattern = pattern,
                    Rv = -1 * mag,
                    Rh = 0,
                    Rm = 0
                };
                support.Reactions.Add(react);
            }
        }
        private static void DefineStations(FrameElement frame, List<LoadPattern> patterns)
        {
            //all loads have same x in stations
            int nstations = 7;
            if (frame.GetType() == typeof(Beam))
            {
                //Beam
                Beam beam = frame as Beam;
                int count = beam.InnerNodes != null ? beam.InnerNodes.Count : 0;
                if (count == 0 || count == 1 || count == 3)
                    nstations = 5;
            }
            else
            {
                //Column 
                nstations = 2;
            }

            foreach (var pattern in patterns)
            {
                StrainingAction sa = frame.StrainingActions.FirstOrDefault(a => a.Pattern == pattern);
                if (sa == null)
                {
                    //No Straining action is defined for this pattern:
                    //1-Define new SA 
                    sa = new StrainingAction() { Pattern = pattern };
                    frame.StrainingActions.Add(sa);
                }
                double x = 0;
                double dx = frame.Length / (nstations - 1);
                Station s;
                sa.Stations = new List<Station>();
                for (int i = 0; i < nstations; i++)
                {
                    s = new Station
                    {
                        X = x,
                        No = 0,
                        Vo = 0,
                        Vf = 0,
                        Mo = 0
                    };
                    sa.Stations.Add(s);
                    x += dx;
                }
            }

        }
        private static void SolveLineLoad(Beam beam, List<LoadPattern> patterns)
        {
            //suppose to solve distributed load on beam for each pattern
            List<LineLoad> loads = beam.LineLoads; //maybe equal null
            LoadPattern pattern;
            double mag;
            if (loads.Count == 0)
            {
                loads = beam.LineLoads = LineLoad.CreateZeroLineLoadList(patterns);
            }

            if (beam.StartNode.PointLoads.Count == 0)
            {
                beam.StartNode.PointLoads = PointLoad.CreateZeroPointLoadList(patterns);
            }

            if (beam.EndNode.PointLoads.Count == 0)
            {
                beam.EndNode.PointLoads = PointLoad.CreateZeroPointLoadList(patterns);
            }

            foreach (var load in loads)
            {
                pattern = load.Pattern;
                if (pattern == LoadPattern.DEAD)
                {
                    load.Magnitude += -1 * beam.Section.W / 1000;
                }
                mag = load.Magnitude;
                StrainingAction sa = beam.StrainingActions.FirstOrDefault(a => a.Pattern == pattern);
                double rs, re; //reaction at start & reaction at end;
                rs = re = 1 * (mag * beam.Length) / 2;
                beam.StartNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += rs; //assign reaction at startNode
                beam.EndNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += re;    //assign reaction at EndNode
                foreach (var station in sa.Stations)
                {
                    station.Vo += -1 * rs + mag * station.X;
                    station.Vf += -1 * rs + mag * station.X;
                    station.Mo += -1 * rs * station.X + (mag * station.X * station.X) / 2;
                }
            }
        }
        private static void SolvePointLoad(Beam beam, List<LoadPattern> patterns)
        {
            if (beam.InnerNodes.Count != 0) //Beam.InnerNodes is never null
            {
                if (beam.StartNode.PointLoads == null)
                {
                    beam.StartNode.PointLoads = PointLoad.CreateZeroPointLoadList(patterns);
                }
                if (beam.EndNode.PointLoads == null)
                {
                    beam.EndNode.PointLoads = PointLoad.CreateZeroPointLoadList(patterns);
                }
                foreach (var node in beam.InnerNodes)
                {
                    double span = beam.Length;
                    double ds = node.Position.Distance(beam.StartNode.Position); //distance from this node and Start Node
                    double de = span - ds; //distance from this node and End Node

                    foreach (var load in node.PointLoads)
                    {
                        var mag = load.Magnitude;
                        var pattern = load.Pattern;
                        var sa = beam.StrainingActions.FirstOrDefault(a => a.Pattern == pattern);
                        double rs, re; //reaction at start & reaction at end;
                        rs = 1 * (mag * de) / span;
                        re = 1 * (mag * ds) / span;

                        beam.StartNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += rs; //assign reaction at startNode
                        beam.EndNode.PointLoads.FirstOrDefault(l => l.Pattern == pattern).Magnitude += re;    //assign reaction at EndNode
                        foreach (var station in sa.Stations)
                        {
                            double x = station.X;
                            if (Math.Abs(x - ds) < Tolerance.DIST_TOL)
                            {
                                //station is at the Node
                                station.Vo += -1 * rs;
                                station.Vf += -1 * rs + mag;
                                station.Mo += -1 * rs * x;
                            }
                            else if (x < ds)
                            {
                                //Station is before Node
                                station.Vo += -1 * rs;
                                station.Vf += -1 * rs;
                                station.Mo += -1 * rs * x;
                            }
                            else
                            {
                                //station is after Node
                                station.Vo += -1 * rs + mag;
                                station.Vf += -1 * rs + mag;
                                station.Mo += -1 * rs * x + mag * (x - ds);
                            }
                        }
                    }
                }
            }
        }

        public static void LinearAddCombineSA(FrameElement frame, List<LoadCombination> combos)
        {
            foreach (var combo in combos)
            {
                LinearAddCombineSA(frame, combo);
            }
        }
        public static void LinearAddCombineReactions(Support support, List<LoadCombination> combos)
        {
            foreach (var combo in combos)
            {
                LinearAddCombineReactions(support, combo);
            }
        }
        private static void LinearAddCombineSA(FrameElement frame, LoadCombination combo)
        {
            var temp = frame.CombinedSA.FirstOrDefault(csa => csa.Combo == combo);
            if (temp == null)
            {
                //create a new instance of combinedSA
                var sa = new StrainingAction()
                {
                    Combo = combo,
                    Pattern = LoadPattern.COMBINATION,
                    Stations = new List<Station>()
                };
                var frameSA = frame.StrainingActions[0];
                //inializing stations in combination
                foreach (var station in frameSA.Stations)
                {
                    var s = new Station()
                    {
                        Mo = 0,
                        Vo = 0,
                        Vf = 0,
                        No = 0,
                        X = station.X
                    };
                    sa.Stations.Add(s);
                }

                foreach (var factoredLoad in combo.Combo)
                {
                    double factor = factoredLoad.ScaleFactor;
                    LoadPattern pattern = factoredLoad.Pattern;
                    //select list of stations associated with this pattern
                    var strainingAction = frame.StrainingActions.FirstOrDefault(sa => sa.Pattern == pattern);
                    int nStations = strainingAction.Stations.Count;
                    var stations = strainingAction.Stations;
                    for (int i = 0; i < nStations; i++)
                    {
                        sa.Stations[i].Mo += factor * stations[i].Mo;
                        sa.Stations[i].Vo += factor * stations[i].Vo;
                        sa.Stations[i].Vf += factor * stations[i].Vf;
                        sa.Stations[i].No += factor * stations[i].No;
                    }
                }
                frame.CombinedSA.Add(sa);
            }
        }
        private static void LinearAddCombineReactions(Support support, LoadCombination combo)
        {
            //1-Copy point load 
            //2-Combine point load

            List<PointLoad> loads = support.Node.PointLoads;
            Reaction reaction = new Reaction()
            {
                Combo = combo,
                Pattern = LoadPattern.COMBINATION,
                Rv = 0,
                Rh = 0,
                Rm = 0
            };
            support.Reactions.Add(reaction);
            foreach (var factoredPattern in combo.Combo)
            {
                LoadPattern pattern = factoredPattern.Pattern;
                double scaleFactor = factoredPattern.ScaleFactor;
                var pl = loads.FirstOrDefault(l => l.Pattern == pattern);
                if (pl != null)
                {
                    reaction.Rv += scaleFactor * -1 * pl.Magnitude;
                }

            }
        }

        #region Methods For Grouping
        /*
         * 1-Order beams by descending order based on their moment
         * 2-Get Design Values for the group
         * 3-Get the service value for the group
         */
        public static List<Group> InitBeamsForDesign(List<Beam> beams)
        {
            List<Group> groups = GroupBeams(beams);
            GetDesignValues(groups);
            GetServiceValues(groups);
            return groups;
        }
        public static DesignLimitState InitColumnsForDesign(List<Column> columns)
        {
            columns.Sort(Column.SortNormalAscendingly());
            Column column = columns[0];
            DesignLimitState designValues = new DesignLimitState
            {
                Combo = column.CombinedSA[0].Combo.Name,
                CriticalElement = column,
                Nd = column.CombinedSA[0].Stations[0].No
            };
            return designValues;
        }
        private static List<Group> GroupBeams(List<Beam> beams)
        {
            //Group beams by their moment
            List<Group> groups = new List<Group>();
            beams.Sort(Beam.SortMomentDescendingly());
            //loop from the smallest to largest
            double upperMoment = 1.2 * Math.Abs(beams[beams.Count - 1].CombinedSA[0].Stations[0].Mo); //Group max moment(1.2*minimum moment)
            Group group = new Group();
            groups.Add(group);
            for (int i = beams.Count - 1; i >= 0; i--)
            {
                double maxMoment = Math.Abs(beams[i].CombinedSA[0].Stations[0].Mo); //get max moment for each beam
                if (maxMoment > upperMoment)
                {
                    //create new group
                    group = new Group();
                    groups.Add(group);
                    upperMoment = maxMoment * 1.5; //Group max moment 1.5????
                }
                group.Beams.Add(beams[i]);
            }
            return groups;
        }
        private static double GetEquivalentLiveLoad(Beam beam)
        {
            //This function is responsible for Calculating the equivalent distributed live load
            //Equivalent distributed live load is (Live load from Line Loads + point Load in the inner nodes over the span of the beam)
            double wll = 0;
            LineLoad lineLoad = beam.LineLoads.FirstOrDefault(l => l.Pattern == LoadPattern.LIVE);
            if (lineLoad == null)
            {
                lineLoad = new LineLoad(0, LoadPattern.LIVE);
                beam.LineLoads.Add(lineLoad);
            }
            wll += lineLoad.Magnitude;
            if (beam.InnerNodes != null)
            {
                double load = 0;
                PointLoad pointLoad;
                foreach (var pl in beam.InnerNodes)
                {
                    pointLoad = pl.PointLoads.FirstOrDefault(l => l.Pattern == LoadPattern.LIVE);
                    load += pointLoad == null ? 0 : pointLoad.Magnitude;
                }
                wll += load / beam.Length; //equivalent distributed load
            }
            return wll;
        }
        private static void GetDesignValues(List<Group> groups)
        {
            //Beams in Each group are ordered ascendingly
            DesignLimitState beamDesignValues;
            foreach (var group in groups)
            {
                Beam beam = group.Beams[group.Beams.Count - 1]; //get the latest beam which has the largest moment
                group.Section = beam.Section;
                beamDesignValues = new DesignLimitState
                {
                    CriticalElement = beam,
                    Md = beam.CombinedSA[0].Stations[0].Mo,
                    Vd = beam.CombinedSA[0].GetMaxShear(),
                    Combo = beam.CombinedSA[0].Combo.Name
                };
                group.DesignValues = beamDesignValues;
            }
        }
        private static void GetServiceValues(List<Group> groups)
        {
            ServiceabilityLimitState serviceValues;
            foreach (var group in groups)
            {
                //first approach
                double span = group.Beams.Max(b => b.Length); //get max span
                Beam beam = group.Beams.FirstOrDefault(b => Math.Abs(b.Length - span) < Tolerance.DIST_TOL); //get beam corresponding to the max span
                //second approach is to sort beams based on their length
                serviceValues = new ServiceabilityLimitState()
                {
                    CriticalBeam = beam,
                    Combo = "LIVE",
                    WLL = GetEquivalentLiveLoad(beam)
                };
                group.ServiceValue = serviceValues;
            }
        }
        #endregion
    }
}
