using System;
using System.Collections.Generic;
using System.Text;
using AUTRA.Models.EnumHelpers;
using AUTRA.Models.Definitions;
using AUTRA.Models.Analysis;
using AUTRA.Models.Reports;

namespace AUTRA.Models.Design
{
   public interface ICodeDesigner
    {
        bool DesignBeam( Section section, double span, double luact, BeamType beamType, double WLL, double Vd, double Md,DesignResult result);
        bool DesignBeam(Group group, BeamType beamType);
        bool DesignColumn( Section section, double length, BracingCondition bracing, double Nd,DesignResult designResult);

    }
    public interface IECP_ASD:ICodeDesigner
    {
        Compactness CheckLocalBuckling( Section section,DesignResult designResult ,StressType stress);
        UnSupportedLength CheckUnSupportedLength( Section section,DesignResult designResult ,double luact);
        double GetAllowableBendingstress( Section section, Compactness compactness, UnSupportedLength lu, double luact);
        double GetAllowableShearStress( Section section);
        double GetAllowableDeflection(double span, BeamType type);
        double CalcBendingStress( Section section, double Md);
        double CalcShearStress( Section section, double Vd);
        double CalcDeflection( Section section, double span, double WLL);
        double CalcBucklingLength(double length , BracingCondition bracing);
        double GetAllowableCompressionStress(double lambda, SteelType steel);
        double CalcAxialStress( Section section, double Nd);
    }
}
