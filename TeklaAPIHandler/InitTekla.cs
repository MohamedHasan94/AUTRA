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
            //string path = @"D:\ITI\GraduationPoject\AUTRA\AUTRA\wwwroot\Inputs\ToTekla01.json";
            //var data = Reader.Read<TeklaModelData>(path/*args[0]*/);
            //AUTRATekla.InitTekla(data);
            if (args.Length > 0)
            {
                Console.WriteLine(args[0]);
                try
                {
                    var data = Reader.Read<TeklaModelData>(args[0]);
                    AUTRATekla.InitTekla(data);

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
