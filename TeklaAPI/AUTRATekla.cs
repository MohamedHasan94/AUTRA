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
                    project.CreatePlanDWG();
                   project.CreateBasePlateDWG();
                    project.CreateElevationsParallelY();
                   project.CreateElevationsParallelX();
                    project.PrintDrawings();
                   project.CompressFolder();
                    result = true;
                }
            }
            return result;
        }
    }
}
