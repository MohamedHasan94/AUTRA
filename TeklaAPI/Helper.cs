using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using TSM=Tekla.Structures.Model;
using T3D=Tekla.Structures.Geometry3d;
using System.Reflection;

namespace AUTRA.Tekla
{
    public enum LineDirection
    {
        InX, InY
    }
    public enum ViewPlacment
    {
        CENTER,OTHER
    }
    public struct TeklaConstants
    {
        public const double DIST_TOL = 1.0;/*mm*/
        public const double LAYOUT_MARGIN = 20; /*mm*/
    }
    internal static class Helper
    {
        public static string GoToPath(this Assembly assembly, string fileRelativePath)
        {
            string assemblyPath = assembly.Location;
            int index = assemblyPath.LastIndexOf('\\');
            assemblyPath = assemblyPath.Substring(0, index + 1);
            return string.Format($"{assemblyPath}{fileRelativePath}");
        }
        public static string GetFolderPath(this string path)
        {
            int index = path.LastIndexOf('\\');
            return path.Substring(0, index + 1);
        }
        internal static string StringFromArray<T>(T[] arr) where T : struct
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in arr)
            {
                sb.Append($"{item} ");
            }
            return sb.ToString() ;
        }
        internal static string CreateCapitalChars<T>(T[] arr) where T : struct
        {
            int index = 65;
            char c;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                c = (char)(index + i);
                sb.Append($"{c} ");
            }
            return sb.ToString();
        }
        internal static string CreateNumbers<T>(T[] arr) where T : struct
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append($"{i+1} ");
            }
            return sb.ToString();
        }
        /// <summary>
        /// generate string of space seperated Signed Values from array
        /// EX:(-100 0 +500 +1500 +2000)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        internal static string CreateSignedValues(double[] arr) 
        {
            string value;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                value =arr[i] > 0.0 ? string.Format($"+{arr[i]}"):arr[i].ToString();
                sb.Append($"{value} ");
            }
            return sb.ToString();
        }
        
        internal static void GetFiles(TSM.Model model)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string test = assembly.GoToPath("attributes");
            string[] filePaths = Directory.GetFiles(assembly.GoToPath("attributes"));
            foreach (var filename in filePaths)
            {
                string file = filename.ToString();
                int index = file.LastIndexOf('\\');
               string fileName = file.Substring(index);
                //Do your job with "file"  
                string str = $"{model.GetInfo().ModelPath}//attributes{fileName.ToString()}";
                {
                    File.Copy(file, str);
                }
            }
        }
       
    }
}
