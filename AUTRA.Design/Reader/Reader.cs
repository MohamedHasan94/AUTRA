using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace AUTRA.Design
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
        public static List<T> Read<T>(string filePath)=> JsonConvert.DeserializeObject<List<T>>(Read(filePath));

    }
}
