using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AUTRA.Models.Analysis;
using AUTRA.Models.Main;
using AUTRA.Models.Definitions;
using AUTRA.Models.Design;
using AUTRA.Models.EnumHelpers;
using System.IO;
using Newtonsoft.Json;
using AUTRA.Models;

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
            string path = @"D:\ITI\Graduation Project\AUTRA\AUTRA\wwwroot\Text.json";

            string js;

            using (StreamReader reader = new StreamReader(path))
            {
                js = reader.ReadToEnd();
            }

            var model = JsonConvert.DeserializeObject<Project>(js,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            List<Section> sections;
            using (StreamReader reader = new StreamReader(@"D:\ITI\Graduation Project\AUTRA\AUTRA\wwwroot\Resources\sections.json"))
            {
                string text = reader.ReadToEnd();
                sections = JsonConvert.DeserializeObject<List<Section>>(text);
            }

            using (StreamReader reader = new StreamReader(@"D:\ITI\Graduation Project\AUTRA\AUTRA\wwwroot\Resources\steel.json"))
            {
                string text = reader.ReadToEnd();
                List<Material> materials = JsonConvert.DeserializeObject<List<Material>>(text);
                model.Material.SetData(ref materials);
            }

            sections.ForEach(s => s.Material = model.Material);

            model.Sections.ForEach(s => s.SetData(ref sections));

            Engine project = new Engine(model, sections, new ECP_ASD());
            project.CreateCombo("D+L", new FactoredPattern() { Pattern = LoadPattern.DEAD, ScaleFactor = 1.0 }, new FactoredPattern() { Pattern = LoadPattern.LIVE, ScaleFactor = 1.0 });
            project.RunAnalysis();
            project.Design();
            project.CreateReports(@"D:\ITI\Graduation Project\AUTRA\AUTRA\wwwroot\Outputs");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
