using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTRA.Design;
using AUTRA.Helper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AUTRA.Controllers
{
    public class EditorController : Controller
    {
        public IActionResult New()
        {
            return View("Editor");
        }

        public IActionResult Open(/*string path*/)
        {
            return View("Open");
        }


        [HttpPost]
        public string Solve([FromBody] Project project)
        {
            project.Nodes.ModifyCoordinates();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            AUTRA.Init(project, @"D:\ITI\GraduationPoject\AUTRA\AUTRA\wwwroot\Inputs\ToTekla02.json"); //Harded coded path and where tekla save also hardcoded=> in AUTRA.Tekla=>Project=>project=> Init
            stopwatch.Stop();
            string response = JsonConvert.SerializeObject(project,new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                Converters = new List<JsonConverter> { new StringEnumConverter(new CamelCaseNamingStrategy()) }
            }); 

            return response;
        }

        [HttpPost]
        public string Save()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                using (var writer = new StreamWriter("./wwwroot/Outputs/Saved/Model.json"))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }

            return "Model saved successfully";
        }

        //public IActionResult Model() //Model on Tekla
        //{
        //    return null;
        //}
    }
}