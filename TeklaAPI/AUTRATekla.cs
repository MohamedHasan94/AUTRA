using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using TSM = Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using System.IO;
using System.Diagnostics;

namespace AUTRA.Tekla
{
   public static class AUTRATekla
    {
        public static bool InitTekla(TeklaModelData data)
        {
            bool result = false;
            if (data != null)
            {
                Project project = new Project(data);
                if (project.Model.GetConnectionStatus())
                {
                    project.CreateFootings();
                    project.CreateMainBeams(TSM.Position.DepthEnum.BEHIND);
                    project.CreateColumns();
                    project.CreateBaseConnections();
                    project.CreateSecondaryBeams(TSM.Position.DepthEnum.BEHIND);
                    project.CreateConnections();
                    //project.CreateAssemblyDWGS();
                    project.CreatePlanDWG();
                    project.CreateBasePlateDWG();
                    //project.CreateElevationDWGSAlongX();
                    //.CreateElevationDWGSAlongY();
                    project.PrintDrawings();
                    bool res = project.CreateReports(); //the type of report we are creating unfortunately is in us envirnoment not in middle east
                    //project.ExportIFC();//not working
                    project.CompressFolder();
                    result = true;
                }
            }
            return result;
        }
    }
}
