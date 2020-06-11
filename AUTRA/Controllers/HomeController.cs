using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using AUTRA.Design;
using System.Text.Json;
using AUTRA.Helper;

namespace AUTRA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            string path = "../AUTRA/wwwroot/Inputs/Test01.json";

            string js;

            using (StreamReader reader = new StreamReader(path))
            {
                js = reader.ReadToEnd();
            }

            var model = JsonConvert.DeserializeObject<Project>(js,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            model.Nodes.ModifyCoordinates();
            model.MainBeams.ModifyInnerNodes(model.Nodes);
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            AUTRA.Design.AUTRA.Init(model);
            stopwatch.Stop();
            ViewData["Design"] = stopwatch.ElapsedMilliseconds / 1000.0;

            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
