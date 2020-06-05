using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using AUTRA.Models.Definitions;
using AUTRA.Models.EnumHelpers;
using AUTRA.Models.Reports;
using AUTRA.Models.Analysis;

namespace AUTRA.Models.Design
{
    //Egyption Code using Allowable Stress Design Method
    public class ECP_ASD : IECP_ASD
    {
        public Compactness CheckLocalBuckling( Section section,DesignResult designResult ,StressType stress)
        {
            //check whether section is compact or not
            double fy = section.Material.Fy;
            //Square Root is an Expensive operation so try at best to avoid it
            double dwtwsq = section.DwTw * section.DwTw;
            double ctfsq = section.CTf * section.CTf;
            //Limits of flange
            double flangeLowerLimitSq = (16.9 * 16.9) / fy;
            double flangeUpperLimitSq = (23 * 23) / fy;
            //Limits of Web
            double webLowerLimitSq = (127 * 127) / fy;
            double webUpperLimitSq = (190 * 190) / fy;
            switch (stress)
            {
                case StressType.AXIAL:
                    webLowerLimitSq = (58 * 58) / fy;
                    webUpperLimitSq = (64 * 64) / fy;
                    break;
            }
            Compactness web;
            Compactness flange;
            //1-Check Compactness for flange
            if (ctfsq < flangeLowerLimitSq)
            {
                flange = Compactness.COMPACT;
                designResult.FlangeLocalBuckling = String.Format($"c/tf= {Math.Sqrt(ctfsq)} < {Math.Sqrt(flangeLowerLimitSq)} => Comapct Flange");
            }
            else if (ctfsq > flangeUpperLimitSq)
            {
                //Slender section => another section should be used
                flange = Compactness.SLENDER;
                return flange;
            }
            else
            {
                flange = Compactness.NONCOMPACT;
                designResult.FlangeLocalBuckling = String.Format($"{Math.Sqrt(flangeUpperLimitSq)} > c/tf= {Math.Sqrt(ctfsq)} > {Math.Sqrt(flangeLowerLimitSq)} => Non-Comapct Flange");
            }
            //2-Check Compactness for Web
            if (dwtwsq < webLowerLimitSq)
            {
                web = Compactness.COMPACT;
                designResult.WebLocalBuckling = String.Format($"dw/tw= {Math.Sqrt(dwtwsq)} < {Math.Sqrt(webLowerLimitSq)} => Compact Web");
            }
            else if (dwtwsq > webUpperLimitSq)
            {
                //Slender section => another section should be used
                web = Compactness.SLENDER;
                return web;
            }
            else
            {
                web = Compactness.NONCOMPACT;
                designResult.WebLocalBuckling = String.Format($"{Math.Sqrt(webUpperLimitSq)} > c/tf= {Math.Sqrt(dwtwsq)} > {Math.Sqrt(webLowerLimitSq)} => Non-Comapct Web");
            }
            return web == Compactness.COMPACT && flange == Compactness.COMPACT ? Compactness.COMPACT : Compactness.NONCOMPACT;
        }
        public UnSupportedLength CheckUnSupportedLength(Section section, DesignResult designResult,double luact)
        {
            double lusq = luact * luact; //squared actual un supported length
            double cb = 1; //It is a conservative assumption
            double fy = section.Material.Fy;
            double bf = section.B;
            double Af = bf * section.Tf;
            double d = section.H;
            double lumaxsq1 = ((1380 * Af) / (fy * d)) * cb * ((1380 * Af) / (fy * d)) * cb;
            double lumaxsq2 = (20 * bf * 20 * bf) / fy;
            double lumaxsq = Math.Min(lumaxsq1, lumaxsq2);
            if (lusq < lumaxsq) {
                designResult.Lu = string.Format($"Luact= {luact} m < Lumax= {Math.Sqrt(lumaxsq)} m => Supported (No LTB)");
                return UnSupportedLength.SUPPORTED;
            }
            else 
            {
                designResult.Lu = string.Format($"Luact= {luact} m > Lumax= {Math.Sqrt(lumaxsq)} m => Un-Supported (LTB Occurs)");
                return UnSupportedLength.UNSUPPORTED;
            }  
        }

        public double GetAllowableBendingstress( Section section, Compactness compactness, UnSupportedLength lu, double luact)
        {
            double Fb = 0.58 * section.Material.Fy; //unit:t/cm^2
            switch (lu)
            {
                case UnSupportedLength.SUPPORTED:
                    switch (compactness)
                    {
                        case Compactness.COMPACT:
                            Fb = 0.64 * section.Material.Fy;
                            break;
                        case Compactness.NONCOMPACT:
                            Fb = 0.58 * section.Material.Fy;
                            break;
                    }
                    break;
                case UnSupportedLength.UNSUPPORTED:
                    double fy = section.Material.Fy;
                    double bf = section.B;
                    double Af = bf * section.Tf;
                    double d = section.H - 2 * section.Tf;
                    double fltb1 = (800 * Af) / (luact * d);
                    double fltb2 = 0.0;
                    if (fltb1 > 0.58 * fy)
                    {
                        fltb1 = 0.58 * fy;
                    }
                    else
                    {
                        double cb = 1;
                        double Iyf = (section.Tf * Math.Pow(section.B, 3)) / 12;
                        double Aw = d * section.Tw;
                        double At = Af + Aw / 6;
                        double rt = Math.Sqrt(Iyf / At);
                        double lurt = luact / rt;
                        double sqrtcbfy = Math.Sqrt(cb / fy);
                        double lowerLimit = 84 * sqrtcbfy;
                        double upperLimit = 188 * sqrtcbfy;
                        if (lurt < lowerLimit)
                        {
                            fltb2 = 0.58 * fy;
                        }
                        else if (lurt > upperLimit)
                        {
                            fltb2 = Math.Min((12000 / (lurt * lurt)) * cb, 0.58 * fy);
                        }
                        else
                        {
                            fltb2 = Math.Min((0.64 - (((lurt * lurt) * fy) / (1.176 * 10e5 * cb))) * fy, 0.58 * fy);
                        }
                    }
                    Fb = Math.Sqrt(fltb1 * fltb1 + fltb2 * fltb2);
                    break;
            }
            return Fb;
        }

        public double GetAllowableDeflection(double span, BeamType type)
        {
            //span (meter) => *100 => (cm)
            double dall = 0;
            span *= 100;
            switch (type)
            {
                case BeamType.FLOOR:
                    dall = span / 300;
                    break;
                case BeamType.CRANE:
                    dall = span / 800;
                    break;
                case BeamType.PURLIN:
                    dall = span / 200;
                    break;
            }
            return dall;
        }

        public double GetAllowableShearStress(Section section) => 0.35 * section.Material.Fy;
        public double GetAllowableCompressionStress(double lambda,SteelType steel)
        {
            double FC;
            if (lambda < 100)
            {
                switch (steel)
                {
                    case SteelType.ST_44:
                        FC = 1.6 - 6.5e-5 * lambda * lambda;
                        break;
                    case SteelType.ST_52:
                        FC = 2.1 - 6.5e-5 * lambda * lambda;
                        break;
                    case SteelType.ST_37:
                    default:
                     FC=  1.4 - 6.5e-5 * lambda * lambda;
                        break;
                   
                }
            }
            else
            {
                FC = 7500 / (lambda * lambda);
            }
            return FC;
        }

        public double CalcShearStress( Section section, double Vd)
        {
            double q; //actual shear stress
            double Aw = section.H * section.Tw;
            q = Vd / Aw; //unit: t/cm^2
            return q;
        }

        public double CalcBendingStress( Section section, double Md)
        {
            //Md=> t.m => *100 => t.cm
            double fbact;
            fbact = (Md*100) / section.Sx;
            return fbact;
        }

        public double CalcDeflection( Section section, double span, double WLL)
        {
            //5*wll*span^4/384*EI
            //wll => t/m' & span => m 
            span *= 100; //unit: cm
            WLL /= 100; //unit:cm
            double dact;
            double E = section.Material.E;
            double I = section.Ix;
            dact = (5 * Math.Abs(WLL) * Math.Pow(span, 4)) / (384 * E * I);
            return dact;
        }

        public double CalcAxialStress(  Section section, double Nd) => Nd / section.Area;

        public double CalcBucklingLength(double length, BracingCondition bracing)
        {
            double k=1;
            length *= 100; //unit:cm
            switch (bracing)
            {
                case BracingCondition.BRACED:
                    k = 1;
                    break;
                case BracingCondition.UNBRACED:
                    k = 2; //TODO:Not 100% correct
                    break;
            }
            return k*length;
        }
        public bool DesignBeam( Section section, double span, double luact, BeamType beamType, double WLL, double Vd, double Md,DesignResult result)
        {
            /*
             * Check Local Buckling
             * Check unsupported length
             * check shear stress
             * check bending stress
             * check deflection
             */
            Compactness compactness = CheckLocalBuckling( section, result,StressType.FLEXURAL);
            if (compactness != Compactness.SLENDER)
            {
                UnSupportedLength unSupported = CheckUnSupportedLength(section, result,luact);
                double Fb, qall, dall;
                double fbact, qact, dact;
                qall = GetAllowableShearStress(section);
                qact = CalcShearStress(section, Vd);
                if (qact > qall) return false;
                Fb = GetAllowableBendingstress(section, compactness, unSupported, luact);
                fbact = CalcBendingStress(section, Md);
                if (fbact > Fb) return false;
                dall = GetAllowableDeflection(span, beamType);
                dact = CalcDeflection(section, span, WLL);
                if (dact > dall) return false;
                result.Fbact = fbact;
                result.Fball = Fb;
                result.Qall = qall;
                result.Qact = qact;
                result.Dact = dact;
                result.Dall = dall;
                return true;
            }
            return false;
        }
        public bool DesignColumn(Section section,double length,BracingCondition bracing,double Nd,DesignResult result)
        {
            /*
             * check local buckling
             * Get Allowable Fc
             * Check Normal Stress
             */
            Compactness compactness = CheckLocalBuckling(section, result,StressType.AXIAL);
            if (compactness != Compactness.SLENDER)
            {
                double KL = CalcBucklingLength(length, bracing);
                double ry = section.Ry;
                double lambda = KL / ry;
                double Fc = GetAllowableCompressionStress(lambda, section.Material.Name);
                double fc = CalcAxialStress(section, Nd);
                return (-1*fc) < Fc; //Compression stress is (-ve) and allowable stress is (+ve)
            }
            return false;
        }

        public bool DesignBeam(Group group, BeamType beamType) => DesignBeam(group.Section, group.ServiceValue.CriticalBeam.Length, 0, beamType, group.ServiceValue.WLL, group.DesignValues.Vd, group.DesignValues.Md, group.DesignResult);
        
    }
}
