using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace AUTRA.Design
{
    //Egyption Code using Allowable Stress Design Method
    public class ECP_ASD : ICodeDesigner
    {
        public double GetAllowableBoltShear(BoltedConnectionCategory cat, Bolt bolt, int nShearPlanes)
        {
            //Return value is the Rshear with 'ton' as its unit.
            double Rshear = 0;
            switch (cat)
            {
                case BoltedConnectionCategory.BEARING_NON_PRETENSIONED:
                    //here We don't check for if the threads are either included or excluded => (we use the conservative way and assume they will always be included)
                    Rshear= bolt.Grade.Fub * bolt.Grade.ShearFactor * bolt.As * nShearPlanes;
                    break;
                case BoltedConnectionCategory.SLIP_PRETENSIONED:
                    throw new NotImplementedException("Still not implemented, Work in progress");
                case BoltedConnectionCategory.BEARING_PRETENSIONED:
                    throw new NotImplementedException("Still not implemented, Work in progress");
            }
            return Rshear;
        }
        public double GetAllowableBoltBearing(double dia , double tmin,double fu)
        {
            double alpha = 0.6; //this value basically depends on the edge distance (0.6,0.8,1,1.2).However,We can be conservative and use the minimum value.
            double Rb = alpha * fu * dia * tmin;
            return Rb;
        }
        public double GetAllowableWeldStress(double fu) => 0.2 * fu;
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
                designResult.FlangeLocalBuckling = String.Format($"c/tf= {Math.Sqrt(ctfsq).Round()} < {Math.Sqrt(flangeLowerLimitSq).Round()} => Compact Flange");
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
                designResult.FlangeLocalBuckling = String.Format($"{Math.Sqrt(flangeUpperLimitSq).Round()} > c/tf= {Math.Sqrt(ctfsq).Round()} > {Math.Sqrt(flangeLowerLimitSq).Round()} => Non-Compact Flange");
            }
            //2-Check Compactness for Web
            if (dwtwsq < webLowerLimitSq)
            {
                web = Compactness.COMPACT;
                designResult.WebLocalBuckling = String.Format($"dw/tw= {Math.Sqrt(dwtwsq).Round()} < {Math.Sqrt(webLowerLimitSq).Round()} => Compact Web");
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
                designResult.WebLocalBuckling = String.Format($"{Math.Sqrt(webUpperLimitSq).Round()} > c/tf= {Math.Sqrt(dwtwsq).Round()} > {Math.Sqrt(webLowerLimitSq).Round()} => Non-Compact Web");
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
                designResult.Lu = string.Format($"Luact= {luact.Round()} m < Lumax= {Math.Sqrt(lumaxsq).Round()} m => Supported (No LTB)");
                return UnSupportedLength.SUPPORTED;
            }
            else 
            {
                designResult.Lu = string.Format($"Luact= {luact.Round()} m > Lumax= {Math.Sqrt(lumaxsq).Round()} m => Un-Supported (LTB Occurs)");
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
        public double GetAllowableCompressionStress(double lambda,SteelType steel,DesignResult result)
        {
            double FC;
            if (lambda < 100)
            {
                result.Lambda = String.Format($"lambda = {lambda.Round()} < 100");
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
                result.Lambda = String.Format($"lambda = {lambda.Round()} > 100");
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
        public double CalcEquivalentStress(double normal, double shear) => Math.Sqrt(normal * normal + 3 * shear * shear);
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
                double iy = section.Ry;
                double lambda = KL / iy;
                double Fc = GetAllowableCompressionStress(lambda, section.Material.Name,result);
                double fc = CalcAxialStress(section, Nd);
                result.Fcact = fc;
                result.Fcall = Fc;
                return fc < Fc;
            }
            return false;
        }
        public bool DesignBeam(Group group, BeamType beamType) => DesignBeam(group.Section, group.ServiceValue.CriticalBeam.Length, 0, beamType, group.ServiceValue.WLL, group.DesignValues.Vd, group.DesignValues.Md, group.DesignResult);
        public bool DesignColumn(Group group, BracingCondition bracing) => DesignColumn(group.Section, group.DesignValues.CriticalElement.Length, bracing, group.DesignValues.Nd, group.DesignResult);
        #region To be Removed
        //public SimpleConnection DesignSimpleConnection(double vd ,BoltedConnectionCategory cat ,Bolt bolt , Section section,List<EqualAngle> angles)
        //{
        //    /*TODO:
        //     * 1-N1 Group (Shop Bolted)=> Number , Grade(most likely will be same for project) , Diameter 
        //     * 2-N2 Group (Field Bolted => Their number will be increased by 15%)=> same requirments as N1 Group
        //     * 3-Angle => Section , Length , Layout of bolts in it
        //     */
        //    /*Resources Needed:
        //     * 1-Design shear Force
        //     * 2-Isections that will be connected => Note:Basically There are two types of simple connections according to type(Name) of connecting members
        //     *      a-Secondary beam & Main Beam (Web & Web)
        //     *      b-Main Beam & Column (Web & Flange)
        //     *   So, Conservatively the parameter section will be the Secondary beam section for 'a' & Main beam section for 'b' and bearing strength will be determined based on it 
        //     * 3-Bolts we are going to use for this connection
        //     */

        //    //For N1 Group(Shop group , Double shear plane)
        //    double Rleast1 = Math.Min(GetAllowableBoltShear(cat,bolt,2), GetAllowableBoltBearing(bolt.Dia,section.Tw,section.Material.Fu));
        //    int n1 =Math.Max((int) Math.Ceiling(vd / Rleast1),3); //minimum in code is 2. However, It is more safer to use min as 3 or 4
        //    //For N2 Group(Field group , single shear plane)
        //    double Rleast2 = Math.Min(GetAllowableBoltShear(cat, bolt, 1), GetAllowableBoltBearing(bolt.Dia, section.Tw, section.Material.Fu));
        //    double vdRow = ((vd / 2) * 1.15);
        //    int n2= Math.Max((int)Math.Ceiling(vdRow / Rleast2), 3);
        //    //For Angle
        //    double a = 3 * bolt.Dia * 1.2; //(3*dia + t)
        //    //Get equal angle with leg greater than 'a'
        //    EqualAngle angle = angles.GetEqualAngle(a);
        //    //To get length of the angle => L <= 0.8 h => we have to swicth from 'cm' to 'mm' and deal with integer values
        //    int l = (int)(section.H * 10 * 0.8);
        //    int p1 = l/n1;
        //    int p2 = l/n2;
        //    int largerPitch = Math.Max(p1, p2); //check not exceeding 6*Dia
        //    int smallerPitch = Math.Min(p1, p2);//check not less than 3*Dia
        //    //Three scenarios can occur:(1-Pitch less than 3*Dia)(2-Pitch Greater Than 6*Dia)(3-Picth between 3-6 *Dia)
        //    //Case1 => Increase Section (rarley will occur but must be implemented), Increase bolt diameter , increase grade
        //    //Case2 => Take pitch with value equal to 4*Dia and recalculate the length again
        //    //Case3 => Ok
        //    int minPitch =(int)(3 * bolt.Dia * 10); //unit is mm
        //    int maxPitch = minPitch * 2;
        //    if (smallerPitch < minPitch)
        //    {
        //        //case1
        //        return null;
        //    }
        //    else if (largerPitch > maxPitch)
        //    {
        //        //case2
        //        p1 = 4 * (int)bolt.Dia * 10;
        //        p2 = 4 * (int)bolt.Dia * 10;
        //        int l1 = n1 * p1;
        //        int l2 = n2 * p2;
        //        l = Math.Max(l1, l2);
        //        //Note: frist edge distance will be equal = p/2 & last edge distance will be equal = L-(n-0.5)*p
        //       //case2 now it is changed to be case3
        //    }
        //    SimpleConnection connection = new SimpleConnection();
        //    connection.N1 = n1;
        //    connection.N2 = n2;
        //    connection.Pitch1 = p1;
        //    connection.Pitch2 = p2;
        //    connection.Length = l;
        //    connection.ConnectingAngle = angle;
        //    return connection;
        //}

        #endregion
        public SimpleConnection DesignSimpleConnection(double vd , Bolt bolt , Section section,ref int weldSize ,ref int plateThickness)
        {
            //This method is used to design simple shear plate connection 
            /*
             * Logic:
             * Get number of field bolts => check pitch
             * check weld size at plane 1-1   => Note weld size is per project
             * check weld size at plane 2-2
             * chec thickness of plate        => Plate thickness is per project
             */
             //Note: Units we are going to use are in mm
            double Rleast = Math.Min(GetAllowableBoltShear(BoltedConnectionCategory.BEARING_NON_PRETENSIONED,bolt,1), GetAllowableBoltBearing(bolt.Dia,section.Tw,section.Material.Fu));
            int n = Math.Max((int)Math.Ceiling(vd / Rleast),3); //number of bolts required
            int length = (int)(0.7 * 10 * section.H);
            int pitch = length / n;
            /*check pitch aganist the following scenarios:
             * 1-pitch is less than 3*Dia => return null
             * 2-pitch is greater than 6*Dia => choose pitch = 4*Dia and then recalculate
             * 3-pitch is between 3 -> 6 => Okay
             */
            int minPitch =(int)(3 * 10 * bolt.Dia);
            int maxPitch = 2 * minPitch;
            if (pitch < minPitch)
            {
                //scenario 1
                return null;
            }else if(pitch > maxPitch)
            {
                //scenario 2
                n += 1;
                pitch =(int)( 4 * 10 * bolt.Dia);
                length = n * pitch;
                //scenario 2 will now convert to scenario 3
            }
            //scenario 3
            //Check Weld
            //First Plane 1-1
            int e = 50; // distance between bolts and end of the plate (mm)
            SimpleConnection conn = new SimpleConnection();
            conn.Rleast = Rleast;
            double feqall = 1.1 * GetAllowableWeldStress(section.Material.Fu);
            do
            {
                double f = (3 * vd * (e / 10)) / ((length * length * weldSize) / 1000); //units are in t/cm^2
                double q = vd / ((2 * length * weldSize) / 100);
                double feq = CalcEquivalentStress(f, q);
                if (feq <feqall )
                {
                    conn.Plane11Check = String.Format($"f = {f.Round()} t/cm^2 & q = {q.Round()} t/cm^2 => feq = (f^2 + 3q^2)^0.5 = {feq.Round()} t/cm^2 < 1.1 * 0.2Fu = {feqall.Round()} t/cm^2 => OK");
                    break;
                }
                weldSize += 2; //assumption that weld will be increment of two and no 5 (6,8,10,12,14,16,....etc)
            } while (true);
            double qall = GetAllowableWeldStress(section.Material.Fu);
            do
            {
                double qmt = (3 * vd * (e / 10)) / ((length * length * weldSize) / 1000); //units are in t/cm^2
                double qp = vd / ((2 * length * weldSize) / 100);
                double qr = Math.Sqrt(qp * qp + qmt * qmt);
                if (qr <  qall)
                {
                    conn.Plane22Check = String.Format($"q = {qp.Round()} t/cm^2 & qmt = {qmt.Round()} t/cm^2 => qres = (q^2 + qmt^2)^0.5 = {qr.Round()} t/cm^2 <  0.2Fu = {qall.Round()} t/cm^2 => OK");
                    break;
                }
                weldSize += 2; //assumption that weld will be increment of two and no 5 (6,8,10,12,14,16,....etc)
            } while (true);
            //Check plate thickness
            if (plateThickness < section.Tw)
            {
                plateThickness = (section.Tw * 10).GetNextEvenInt(); //units are in mm
            }
            do
            {
                double f = (6 * vd * (e / 10)) / ((length * length * plateThickness) / 1000); //units are in t/cm^2
                if (f < 0.72 * section.Material.Fy)
                {
                    conn.PlateThicknessCheck = String.Format($"f = (6*Vd*e)/(tp*L^2) = {f.Round()} t/cm^2  <  0.72*Fy = {(0.72 * section.Material.Fy).Round()} t/cm^2 => OK {section.Name}");
                    break;
                }
                plateThickness += 2;
            } while (true);

            conn.N = n;
            conn.Pitch = pitch;
            conn.Length = length;
            conn.Sw = weldSize;
            conn.Tp = plateThickness;
            conn.Bolt = bolt;
            
            
            return conn;
        }
    }
}
