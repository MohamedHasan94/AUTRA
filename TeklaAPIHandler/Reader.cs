using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklaAPIHandler
{
   public static class Reader
    {
        private static string Read(string filePath)
        {
            string jsonFromFile;
            using (var reader = new StreamReader(filePath))
            {
                jsonFromFile = reader.ReadToEnd();
            }
            return jsonFromFile;
        }
        public static List<T> ReadList<T>(string filePath) => JsonConvert.DeserializeObject<List<T>>(Read(filePath));
        public static T Read<T>(string filePath) => JsonConvert.DeserializeObject<T>(Read(filePath));
    }
}
