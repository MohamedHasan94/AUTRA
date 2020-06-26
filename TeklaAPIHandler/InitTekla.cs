using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AUTRA.Tekla;
using System.IO.Compression;
using System.IO;

namespace TeklaAPIHandler
{
    class InitTekla
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //string path = @"D:\ITI\GraduationPoject\AUTRA\AUTRA\wwwroot\Inputs\ToTekla05.json"; "../AUTRA/wwwroot/Inputs/ToTekla.json"
            //string path = "../../../AUTRA/wwwroot/Inputs/ToTekla.json";
            //var data = Reader.Read<TeklaModelData>(path/*args[0]*/);
            //AUTRATekla.InitTekla(data);
            if (args.Length > 0)
            {
                Console.WriteLine("Running tekla.....");
                try
                {
                    string solutionDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                    var data = Reader.Read<TeklaModelData>(args[0]);
                    Console.WriteLine("Data read successfully******");
                    AUTRATekla.InitTekla(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    throw;
                }
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }
    }
}
