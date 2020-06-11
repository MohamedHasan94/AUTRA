using System;
using System.Collections.Generic;
using System.Text;

namespace AUTRA.Design
{
   public class SimpleConnection
   {
        //Units here will be in 'mm'
        public int N1 { get; set; }
        public int N2 { get; set; }
        public EqualAngle ConnectingAngle { get; set; }
        public int Length { get; set; }
        public int Pitch1 { get; set; }
        public int Pitch2 { get; set; }


        private string CreateSingleLayout(int pitch , int n)
        {
            StringBuilder stringBuilder = new StringBuilder();
            int e = pitch / 2;
            stringBuilder.Append($"{e};");
            for (int i = 0; i < n - 1; i++)
            {
                stringBuilder.Append($"{pitch};");
            }
            stringBuilder.Append($"{Length-(n-0.5)*pitch}");
            return stringBuilder.ToString();
        }
       
    }
}
