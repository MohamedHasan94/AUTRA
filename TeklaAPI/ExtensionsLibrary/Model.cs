using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM=Tekla.Structures.Model;
using T3D = Tekla.Structures.Geometry3d;
using Tekla.Structures;
using System.IO;

namespace AUTRA.Tekla
{
   public static class Model
    {
        public static void CreateModelFolderDirectory(this TSM.Model model,string dirName)
        {
            string dirPath = $"{model.GetInfo().ModelPath}/{dirName}";
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
        }
        public static void AssignProjectProperties(this TSM.Model model,ProjectProperties projectProperties)
        {
            var projectInfo = model.GetProjectInfo();
            projectInfo.ProjectNumber = projectProperties.Number;
            projectInfo.Name =projectProperties.Name;
            projectInfo.Location =projectProperties.Location;
            projectInfo.Town = projectProperties.City;
            projectInfo.Country =projectProperties.Country;
            projectInfo.Modify();
        }
        public static TSM.TransformationPlane GetCurrentPlane(this TSM.Model model) => model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
        public static bool SetPlaneToGlobal(this TSM.Model model) => model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane());
        public static bool SetPlane(this TSM.Model model, TSM.TransformationPlane plane) => model.GetWorkPlaneHandler().SetCurrentTransformationPlane(plane);
        public static bool SetPlane(this TSM.Model model, T3D.CoordinateSystem coords) => model.SetPlane(new TSM.TransformationPlane(coords));
        public static List<TSM.ModelObject> ToList(this TSM.ModelObjectEnumerator enumerator)
        {
            //This Method is used to convert ModelObjectEnumerator to easily deal with it in Linq

            //ObjectFilterExpressions.Type objectType = new ObjectFilterExpressions.Type();
            //NumericConstantFilterExpression type = new NumericConstantFilterExpression(TeklaStructuresDatabaseTypeEnum.PART);
            //var expression = new BinaryFilterExpression(objectType, NumericOperatorType.IS_EQUAL, type);
            //BinaryFilterExpressionCollection collection = new BinaryFilterExpressionCollection
            //{
            //    new BinaryFilterExpressionItem(expression)
            //};
            List<TSM.ModelObject> modelObjs = new List<TSM.ModelObject>();
            while (enumerator.MoveNext())
            {
                var modelObject = enumerator.Current;
                if (modelObject == null)
                    continue;
                modelObjs.Add(modelObject);
            }
            return modelObjs;
        }
        public static TSM.Beam CreatePadFooting(this TSM.Model model,double posX, double posY, double level, double depth, TSM.Position.RotationEnum rotation, string name, string profile, string material, string classValue = "8")
        {
            //This method is used to Create Pad Footing
            TSM.Beam footing = new TSM.Beam
            {
                Name = name,
                Profile = new TSM.Profile { ProfileString = profile },
                Material = new TSM.Material { MaterialString = material },
                Class = classValue,
                CastUnitType = TSM.Part.CastUnitTypeEnum.CAST_IN_PLACE,
                StartPoint = new T3D.Point(posX, posY, level),
                EndPoint = new T3D.Point(posX, posY, (level - depth)),
                Position = new TSM.Position
                {
                    Rotation = rotation,
                    Depth = TSM.Position.DepthEnum.MIDDLE,
                    Plane = TSM.Position.PlaneEnum.MIDDLE,
                }
            };
            footing.Insert();
            return footing;
        }
        public static TSM.Beam CreateColumn(this TSM.Model model,double posX, double posY, double level, double height, TSM.Position.RotationEnum rotation, string name, string profile, string assemblyPrefix, string material)
        {
            TSM.Beam column = new TSM.Beam
            {
                Name = name,
                AssemblyNumber = new TSM.NumberingSeries
                {
                    Prefix = assemblyPrefix
                },
                Profile = new TSM.Profile { ProfileString = profile },
                Material = new TSM.Material { MaterialString = material },
                Class = "3", //TODO: to be edited
                StartPoint = new T3D.Point(posX, posY, level),
                EndPoint = new T3D.Point(posX, posY, (level + height)),
                Position = new TSM.Position
                {
                    Rotation = rotation,
                    Depth = TSM.Position.DepthEnum.MIDDLE,
                    Plane = TSM.Position.PlaneEnum.MIDDLE,
                }

            };
            column.Insert();
            return column;
        }
        public static TSM.Connection createBasePlate(this TSM.Model model,TSM.ModelObject primaryObj, TSM.ModelObject secondaryObj)
        {
            TSM.Connection basePlate = new TSM.Connection
            {
                Name = "Stiffened Base Plate",
                Number = 1042, // number of the connection
                UpVector = new T3D.Vector(0, 0, 1000),
                PositionType = PositionTypeEnum.COLLISION_PLANE
            };
            basePlate.LoadAttributesFromFile("standard");
            basePlate.SetPrimaryObject(primaryObj);
            basePlate.SetSecondaryObject(secondaryObj);
            basePlate.SetAttribute("cut", 1);  //Enable anchor rods
            basePlate.Insert();
            return basePlate;
        }
        public static void DeleteAllGrids(this TSM.Model model)
        {
            var grids = model.GetModelObjectSelector().GetAllObjectsWithType(TSM.ModelObject.ModelObjectEnum.GRID).ToList();
            if (grids != null)
                grids.ForEach(g => g.Delete());
        }
        public static TSM.Grid CreateGrids(this TSM.Model model, double[] coordinateX, double[] coordinateY, double[] coordinateZ)
        {
            TSM.Grid grids = new TSM.Grid
            {
                Name = "Grid",
                CoordinateX = Helper.StringFromArray(coordinateX),
                CoordinateY = Helper.StringFromArray(coordinateY),
                CoordinateZ = Helper.StringFromArray(coordinateZ),
                LabelX = Helper.CreateCapitalChars(coordinateX),
                LabelY = Helper.CreateNumbers(coordinateY),
                LabelZ = Helper.CreateSignedValues(coordinateZ),
                ExtensionLeftX = 2000.0,
                ExtensionLeftY = 2000.0,
                ExtensionLeftZ = 2000.0,
                ExtensionRightX = 2000.0,
                ExtensionRightY = 2000.0,
                ExtensionRightZ = 2000.0,
                IsMagnetic = true
            };
            grids.Insert();
            return grids;
        }
        public static TSM.Beam CreateBeam(this TSM.Model model, double SX, double SY, double SZ, double EX, double EY, double EZ, TSM.Position.DepthEnum depth, string name, string prefix, string profile, string material, string classValue = "10")
        {
            TSM.Beam beam = new TSM.Beam
            {
                Name = name,
                AssemblyNumber = new TSM.NumberingSeries
                {
                    Prefix = prefix
                },
                Profile = new TSM.Profile { ProfileString = profile },
                Material = new TSM.Material { MaterialString = material },
                Class = classValue,
                StartPoint = new T3D.Point(SX, SY, SZ),
                EndPoint = new T3D.Point(EX, EY, EZ),
                Position = new TSM.Position
                {
                    Rotation = TSM.Position.RotationEnum.TOP,
                    Depth = depth,
                    Plane = TSM.Position.PlaneEnum.MIDDLE,
                }
            };
            beam.Insert();
            return beam;
        }
        public static TSM.Connection ClipAngle(this TSM.Model model , TSM.ModelObject primary, TSM.ModelObject secondary)
        {
            //This method is used to create One Sided Clip angle => component number 120
            TSM.Connection connection = new TSM.Connection
            {
                Name = "Clip Angle",
                Number = 120,
                Class = 99
            };
            connection.LoadAttributesFromFile("standard");
            //connection.SetAttribute("e1", 100.0); //distance from angle edge to top section edge
            //connection.SetAttribute("e2", 100.0); //distance from angle edge to bottom section edge
            connection.UpVector = new T3D.Vector(0, 0, 1000);
            connection.PositionType = PositionTypeEnum.COLLISION_PLANE;
            connection.SetPrimaryObject(primary);
            connection.SetSecondaryObject(secondary);
            connection.Insert();
            return connection;
        }
        public static TSM.Connection SimpleShearPlate(this TSM.Model model,TSM.ModelObject primary, TSM.ModelObject secondary,double e1 , double hp , double tp ,double edge , string pitchLayout , double dia , string boltType , double sw )
        {
            //This method is used to create Simple shear plate connection => component number (103 || 146)
            /*
             * e1 => distance from edge of plate to the top of Beam Section
             * hpl1 | hp => height of plate
             * tpl1 | tp => thicknes of plate
             * rb1 | edge => top bolt edge distance 
             * lbd | innerBoltsLayout => bolts bitch layout => EX: (3*50 or 50 60 50)
             * rw1 => distance from bolts to the end of plate in main beam direction 
             * diameter | dia => bolts diameter
             * screwdin | boltType => type of bolt
             * w1_size & w1_size2 | sw => weld Size
             */
            TSM.Connection connection = new TSM.Connection
            {
                Name = "Simple Shear Plate",
                Number = 103,
                Class = 99
            };
            connection.LoadAttributesFromFile("standard");
            connection.SetAttribute("e1", e1);
            connection.SetAttribute("tpl1", tp);
            connection.SetAttribute("hpl1", hp);
            connection.SetAttribute("rb1", edge); 
            connection.SetAttribute("rw1", 50.00); 
            connection.SetAttribute("lbd", pitchLayout);
            connection.SetAttribute("diameter", dia);
            connection.SetAttribute("screwdin", boltType);
            connection.SetAttribute("w1_size",sw);
            connection.SetAttribute("w1_size2", sw);
            connection.UpVector = new T3D.Vector(0, 0, 1000);
            connection.PositionType = PositionTypeEnum.COLLISION_PLANE;
            connection.SetPrimaryObject(primary);
            connection.SetSecondaryObject(secondary);
            connection.Insert();
            return connection;
        }
        public static TSM.Connection TwoSidedClipAngle(this TSM.Model model,TSM.ModelObject primary, ArrayList secondaries)
        {
            //This method is used to Create two sided Clip angle => component number (117)
            TSM.Connection connection = new TSM.Connection
            {
                Name = "Two Sided Clip Angle",
                Number = 117,
                Class = 99
            };
            connection.LoadAttributesFromFile("standard");
            //connection.SetAttribute("e1", 100.0); //distance from angle edge to top section edge
            //connection.SetAttribute("e2", 100.0); //distance from angle edge to bottom section edge
            connection.UpVector = new T3D.Vector(0, 0, 1000);
            connection.PositionType = PositionTypeEnum.COLLISION_PLANE;
            connection.SetPrimaryObject(primary);
            connection.SetSecondaryObjects(secondaries);
            connection.Insert();
            return connection;
        }
        public static List<T> SelectByBoundingBox<T>(this TSM.Model model,T3D.Point origin) where T : TSM.ModelObject
        {
            model.SetPlaneToGlobal(); //transform the plane to global
            var xVec = new T3D.Vector(1, 0, 0);
            var yVec = new T3D.Vector(0, 1, 0);
            model.SetPlane(new T3D.CoordinateSystem(origin, xVec, yVec)); //Move work plane handler to this point
            List<T> objs = model.GetModelObjectSelector().GetObjectsByBoundingBox(new T3D.Point(-100, -100, -100), new T3D.Point(100, 100, 100)).ToList().OfType<T>().ToList();
            return objs;
        }
        public static void CreateFramingConnectionAtNode(this TSM.Model model, List<TSM.Beam> parts)
        {
            //check if there is column => column is main part
            TSM.Beam column = parts.SelectColumns().FirstOrDefault();
            TSM.ModelObject main = null;
            if (column != null)
            {
                main = column;
                /*there are two cases:
                 * case1: corner connection  -- o
                 *                              |
                 *                                 |
                 * case2: inner connection      -- o -- 
                 *                                 |
                 *                                 
                 * case3: edge connection       -- o -- 
                 *                                 |
                 */
                List<TSM.Beam> secondaries = parts.Where(x => x != column).ToList();
                if (secondaries.Count == 2)
                {
                    //case 1 corner connection
                    model.ClipAngle(main, secondaries[0]);
                    model.ClipAngle(main, secondaries[1]);
                }
                else if (secondaries.Count > 2)
                {
                    ArrayList inWeak = new ArrayList();
                    ArrayList inStrong = new ArrayList();
                    T3D.Vector z = new T3D.Vector(0, 0, 1);
                    //case 3 edge connection:
                    /*
                     case a: two main and one secondary
                     case b: one main  and two secondary
                    */
                    model.SetPlane(column.GetCoordinateSystem()); //move the current work plane to column
                    secondaries.ForEach(s => {
                        s.Select();
                        switch (s.InZDirection())
                        {
                            case true:
                                inWeak.Add(s);
                                break;
                            case false:
                                inStrong.Add(s);
                                break;
                        }
                    });
                    switch (inWeak.Count)
                    {
                        case 1:
                            model.ClipAngle(main, inWeak[0] as TSM.ModelObject);

                            break;
                        case 2:
                            model.TwoSidedClipAngle(main, inWeak);
                            break;
                    }
                    foreach (TSM.ModelObject beam in inStrong)
                    {
                        model.ClipAngle(main, beam);
                    }
                }
            }
            else
            {
                /*there are two cases:
                 * case1: edge connection  -----------
                 *                              |
                 *                                 |
                 * case2: inner connection      ------- 
                 *                                 |
                 */
                if (parts.Count == 2)
                {
                    TSM.ModelObject secondary = null;
                    //case 1 (edge connection)
                    T3D.Vector lenVect = parts[0].GetVector();
                    T3D.Vector startstart = new T3D.Vector(parts[0].StartPoint - parts[1].StartPoint);
                    T3D.Vector startend = new T3D.Vector(parts[0].StartPoint - parts[1].EndPoint);
                    if (lenVect.Cross(startstart) == new T3D.Vector() || lenVect.Cross(startend) == new T3D.Vector())
                    {
                        //objs[0] is the main beam
                        main = parts[0];
                        secondary = parts[1];
                    }
                    else
                    {
                        //objs[1] is the main beam
                        main = parts[1];
                        secondary = parts[0];
                    }
                    model.ClipAngle(main, secondary);
                }
                else if (parts.Count > 2)
                {
                    ArrayList secondaries = new ArrayList();
                    //case 2 (inner connection)
                    ArrayList inZ = new ArrayList();
                    ArrayList notInZ = new ArrayList();
                    model.SetPlaneToGlobal();
                    model.SetPlane(parts[0].GetCoordinateSystem());
                    parts.ForEach(p =>
                    {
                        p.Select();
                        if (p.InZDirection())
                            inZ.Add(p);
                        else notInZ.Add(p);
                    });
                    switch (inZ.Count)
                    {
                        case 1:
                            //selected beam is secondary 
                            model.TwoSidedClipAngle(inZ[0] as TSM.ModelObject, notInZ);
                            break;
                        case 2:
                            //selected beam is main beam
                            model.TwoSidedClipAngle(parts[0], inZ);
                            break;
                    }
                    model.SetPlaneToGlobal();
                }
            }
        }
        public static void CreateSimpleShearPlateConnectionAtNode(this TSM.Model model, List<TSM.Beam> parts , Connection connection)
        {
            /*
             * Logic: => Separate main from secondaries
             * Secenarios: 
             *      Scenario 1: Column exist => column is the main part & others are secondary
             *      Scenario 2: No column => 
             */
            TSM.Beam column = parts.SelectColumns().FirstOrDefault();
            TSM.ModelObject main = null;
            if (column != null)
            {
                main = column;
                /*there are two cases:
                 * case1: corner connection  -- o
                 *                              |
                 *                                 |
                 * case2: inner connection      -- o -- 
                 *                                 |
                 *                                 
                 * case3: edge connection       -- o -- 
                 *                                 |
                 */
                List<TSM.Beam> secondaries = parts.Where(x => x != column).ToList();
                secondaries.ForEach(s => model.SimpleShearPlate(main, s,connection.Top,connection.Hp,connection.Tp,connection.Edge,connection.PitchLayout,connection.Dia,connection.BoltType,connection.Sw));
            }
            else
            {
                if (parts.Count == 2)
                {
                    TSM.ModelObject secondary = null;
                    //case 1 (edge connection)
                    T3D.Vector lenVect = parts[0].GetVector();
                    T3D.Vector startstart = new T3D.Vector(parts[0].StartPoint - parts[1].StartPoint);
                    T3D.Vector startend = new T3D.Vector(parts[0].StartPoint - parts[1].EndPoint);
                    if (lenVect.Cross(startstart) == new T3D.Vector() || lenVect.Cross(startend) == new T3D.Vector())
                    {
                        //objs[0] is the main beam
                        main = parts[0];
                        secondary = parts[1];
                    }
                    else
                    {
                        //objs[1] is the main beam
                        main = parts[1];
                        secondary = parts[0];
                    }
                    model.SimpleShearPlate(main, secondary, connection.Top, connection.Hp, connection.Tp, connection.Edge, connection.PitchLayout, connection.Dia, connection.BoltType, connection.Sw);
                }
                else if (parts.Count > 2)
                {
                    ArrayList secondaries = new ArrayList();
                    //case 2 (inner connection)
                    List<TSM.Beam> inZ = new List<TSM.Beam>();
                    List<TSM.Beam> notInZ = new List<TSM.Beam>();
                    model.SetPlaneToGlobal();
                    model.SetPlane(parts[0].GetCoordinateSystem());
                    parts.ForEach(p =>
                    {
                        p.Select();
                        if (p.InZDirection())
                            inZ.Add(p);
                        else notInZ.Add(p);
                    });
                    switch (inZ.Count)
                    {
                        case 1:
                            //selected beam is secondary 
                            notInZ.ForEach(s => model.SimpleShearPlate(inZ[0], s, connection.Top, connection.Hp, connection.Tp, connection.Edge, connection.PitchLayout, connection.Dia, connection.BoltType, connection.Sw));
                            break;
                        case 2:
                            //selected beam is main beam
                            inZ.ForEach(s => model.SimpleShearPlate(parts[0], s, connection.Top, connection.Hp, connection.Tp, connection.Edge, connection.PitchLayout, connection.Dia, connection.BoltType, connection.Sw));
                            break;
                    }
                    model.SetPlaneToGlobal();
                }
            }
        }
    }
}
