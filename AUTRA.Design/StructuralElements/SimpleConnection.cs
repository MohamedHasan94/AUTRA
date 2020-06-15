using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class SimpleConnection
   {
        //Units here will be in 'mm'
        public int N { get; set; }
        public int Length { get; set; }
        public int Pitch { get; set; }
        public int Sw { get; set; }
        public int Tp { get; set; }
        public Bolt Bolt { get; set; }
        public double Rleast { get; set; }
        public string Plane11Check { get; set; }
        public string Plane22Check { get; set; }
        public string PlateThicknessCheck { get; set; }
        public override string ToString()
        {
            return CreateSingleLayout();
        }
        public string GetPitchLayout()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < N - 1; i++)
            {
                stringBuilder.Append($"{Pitch} ");
            }
            return stringBuilder.ToString();
        }
        private string CreateSingleLayout()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int e = Pitch / 2;
            stringBuilder.Append($"{e};");
            for (int i = 0; i < N - 1; i++)
            {
                stringBuilder.Append($"{Pitch} ");
            }
            stringBuilder.Append($"{Length-(N-0.5)*Pitch}");
            return stringBuilder.ToString();
        }
       
    }
}
