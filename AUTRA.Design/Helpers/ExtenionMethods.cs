using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace AUTRA.Design
{
   public static class ExtenionMethods
    {
        public static void AssignGrade(this List<Bolt> bolts, BoltGrade grade) => bolts.ForEach(b => b.Grade = grade); //Note: Not Used At all
        public static EqualAngle GetEqualAngle(this List<EqualAngle> angles , double a)=> angles.Find(ea => (a - ea.A) < 0);
        public static void AssignMaterialValues(this List<Material> materials ,  Material material)
        {
            var mat = materials.FirstOrDefault(m => m.Name == material.Name);
            material.Density = mat.Density;
            material.Fy = mat.Fy;
            material.Fu = mat.Fu;
            material.E = mat.E;
        }
        public static void AssignSectionValues(this List<Section> sections , Section section)
        {
            Section sec = sections.FirstOrDefault(s => s.Name == section.Name);
            section.Id = sec.Id;
            section.H = sec.H;
            section.B = sec.B;
            section.Tw = sec.Tw;
            section.Tf = sec.Tf;
            
            section.Area = sec.Area;
            section.Ix = sec.Ix;
            section.Sx = sec.Sx;
            section.Rx = sec.Rx;
            section.Iy = sec.Iy;
            section.Sy = sec.Sy;
            section.Ry = sec.Ry;
            section.W = sec.W;
            section.DwTw = sec.DwTw;
            section.CTf = sec.CTf;
        }
        public static void AssignSupports(this List<Column> columns, List<Support> supports) => columns.ForEach(c => supports.Add(new Support(c.StartNode)));
        public static string GoToPath(this Assembly assembly , string fileRelativePath)
        {
            string assemblyPath = assembly.Location;
            int index = assemblyPath.LastIndexOf('\\');
            assemblyPath = assemblyPath.Substring(0, index + 1);
            return string.Format($"{assemblyPath}{fileRelativePath}");
        }
        public static void AssignID<T>(this List<T> elements) where T:FrameElement
        {
            int id = 1;
            elements.ForEach(e=>
            {
                e.Id = id;
                id++;
            });
        }
        public static double GetMaxMoment(this List<Station> stations) => stations.Max(s => Math.Abs(s.Mo));
        public static double GetMaxMoment(this List<StrainingAction> sas) => sas.Max(sa => sa.Stations.GetMaxMoment());
        public static double GetMaxMoment<T>(this List<T> eles) where T :FrameElement => eles.Max(b => b.CombinedSA.GetMaxMoment());
        public static double GetMaxShear(this List<Station> stations) => stations.Max(s => Math.Max(Math.Abs(s.Vo),Math.Abs(s.Vf)));
        public static double GetMaxShear(this List<StrainingAction> sas) => sas.Max(sa => sa.Stations.GetMaxShear());
        public static double GetMaxShear<T>(this List<T> eles) where T :FrameElement => eles.Max(b => b.CombinedSA.GetMaxShear());
        public static double GetMaxCompression(this List<Station> stations) => stations.Min(s => s.No);
        public static double GetMaxCompression(this List<StrainingAction> sas) => sas.Min(sa => sa.Stations.GetMaxCompression());
        public static double GetMaxCompression<T>(this List<T> eles) where T : FrameElement => eles.Min(b => b.CombinedSA.GetMaxCompression());
    }
}
