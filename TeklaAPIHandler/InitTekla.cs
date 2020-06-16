using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AUTRA.Tekla;
namespace TeklaAPIHandler
{
    class InitTekla
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Dell\Desktop\june15\AUTRA\AUTRA\wwwroot\Inputs\ToTekla01.json";
            Console.WriteLine(path);
            //var data = Reader.Read<TeklaModelData>(path/*args[0]*/);
            //AUTRATekla.InitTekla(data);
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                try
                {
                    Console.WriteLine("before reader");
                    var data = Reader.Read<TeklaModelData>(args[0]);
                    Console.WriteLine("before init tekla");
                    AUTRATekla.InitTekla(data);
                    Console.WriteLine("after init tekla");


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                    throw;
                }
            }
        }
    }
    }
