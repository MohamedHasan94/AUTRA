using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AUTRA.Tekla
{
   public class Footing
    {
        #region private fields
        private double _width=0;
        private double _length=0;
        #endregion

        //data to be fetched from json
        public string Name { get; set; }
        public string Profile { get; set; }
        public string Material { get; set; }
        public Point Point { get; set; }
        public double Depth { get; set; }

        //helper data
        public double Length
        {
            get
            {
                if (_width == 0)
                          ParseLW();
                return _width;
            }
        }
        public double Width
        {
            get
            {
                if (_length== 0)
                    ParseLW();
                return _length;
            }
        }
        private void ParseLW()
        {
            string[] arr = Profile.Split('*');
            _length = Convert.ToDouble(arr[0]);
            _width = Convert.ToDouble(arr[1]);
        }
    }
}
