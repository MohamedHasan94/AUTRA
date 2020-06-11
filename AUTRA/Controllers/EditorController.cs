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

namespace AUTRA.Controllers
{
    public class EditorController : Controller
    {
        public IActionResult New()
        {
            return View("Editor");
        }

        //public IActionResult Open(string path)
        //{
        //    return null;
        //}


        [HttpPost]
        public string Solve([FromBody] Project project)
        {
            project.Nodes.ModifyCoordinates();
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Design.AUTRA.Init(project);
            stopwatch.Stop();
            string response = JsonConvert.SerializeObject(project,
                new StringEnumConverter
                {
                    CamelCaseText = true
                }); 

            return response;
            //return Json(project);
            //return "Done";
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