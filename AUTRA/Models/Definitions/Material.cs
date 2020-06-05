using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AUTRA.Models.EnumHelpers;

namespace AUTRA.Models.Definitions
{
   public class Material
    {
        #region Properties
        public SteelType Name { get; set; } 
        public double E { get; set; }
        public double Fy { get; set; }
        public double Fu { get; set; }
        public double Density { get; set; }
        #endregion
        #region Constructor
        public Material()
        {

        }
        public Material(SteelType name, double fy, double fu, double e, double density) //TODO: ask team
        {
            Name = name;
            Fy = fy;
            Fu = fu;
            E = e;
            Density = density;
        }
        #endregion


        public void SetData(ref List<Material> materials)
        {
            Material material = materials.FirstOrDefault(m => m.Name == Name);

            E = material.E;
            Fy = material.Fy;
            Fu = material.Fu;
            Density = material.Density;
        }
    }
}
