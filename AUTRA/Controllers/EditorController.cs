﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AUTRA.Data;
using AUTRA.Design;
using AUTRA.Helper;
using AUTRA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AUTRA.Controllers
{
    public class EditorController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EditorController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult New()
        {
            return View("Editor");
        }

        public IActionResult MyProjects()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Models.Project> projects = _context.Projects.Where(p => p.Fk_UserId == userId).ToList();
            return View(projects);
        }
        public IActionResult Open(string id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["projectName"] = $"{userId}/{id}/{id}.json";
            return View("Editor");
        }


        [HttpPost]
        public string Solve([FromBody] Design.Project project)
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
        public bool Save([FromRoute] string id)//id is projectName (for routing to bind the parameter)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string path = $"./wwwroot/Users/{userId}/{id}";
            Directory.CreateDirectory(path); //if the folder exists it will continue
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                using (var writer = new StreamWriter(Path.Combine(path, $"{id}.json")))
                {
                    writer.Write(reader.ReadToEnd());
                }
            }
            Models.Project project = new Models.Project { Fk_UserId = userId, Name = id};
            _context.Projects.Add(project);
            
            return _context.SaveChanges() > 0;
        }

        //public IActionResult Model() //Model on Tekla
        //{
        //    return null;
        //}
    }
}