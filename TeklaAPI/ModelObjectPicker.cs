using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures;
using TSM=Tekla.Structures.Model;
using T3D=Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using Tekla.Structures.Model.Operations;

namespace AUTRA.Tekla
{
	/*Note:
	 * 1-This is a wrapper for Picker object in tekla structures
	*/
   public class ModelObjectPicker
    {
        #region member fields
        private TSMUI.Picker picker;
        #endregion

        #region Constructors
        public ModelObjectPicker()
        {
            picker = new TSMUI.Picker();
        }
		#endregion

		#region Methods
		/// <summary>
		/// Pick one model object
		/// </summary>
		/// <returns></returns>
		public TSM.ModelObject PickModelObject()
		{
			TSM.ModelObject modelObject ;
			try
			{
				modelObject= picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_OBJECT,
				"Pick One Object");
			}
			catch (Exception)
			{
				modelObject = null;
				Operation.DisplayPrompt("Selection is interrupted");
				
			}
			return modelObject;
		}

		/// <summary>
		/// pick one part
		/// </summary>
		/// <returns></returns>
		public TSM.Part PickPart()
		{
			TSM.Part part;
			try
			{
				part= picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_PART,
				"Pick One Part") as TSM.Part;
			}
			catch (Exception)
			{
				part = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return part;
		}

		/// <summary>
		/// Pick one beam
		/// </summary>
		/// <returns></returns>
		public TSM.Beam PickBeam()
		{
			TSM.Beam part;
			try
			{
				part = picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_PART,
				"Pick One Beam") as TSM.Beam;
			}
			catch (Exception)
			{
				part = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return part;
		}

		/// <summary>
		/// Pick a contour plate
		/// </summary>
		/// <returns></returns>
		public TSM.ContourPlate PickContourPlate()
		{
			TSM.ContourPlate part;
			try
			{
				part = picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_PART,
				"Pick One Contour Plate") as TSM.ContourPlate;
			}
			catch (Exception)
			{
				part = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return part;
		}

		/// <summary>
		/// Pick bolt group (bolt group is the base class of all bolts).
		/// </summary>
		/// <returns></returns>
		public TSM.BoltGroup PickBoltGroup()
		{
			TSM.BoltGroup boltGroup;
			try
			{
				boltGroup= picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_BOLTGROUP,
				"Pick One Bolt Group") as TSM.BoltGroup;
			}
			catch (Exception)
			{
				boltGroup = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return boltGroup;
		}

		/// <summary>
		/// Pick list of Model objects
		/// </summary>
		/// <returns></returns>
		public  List<TSM.ModelObject> PickModelObjects()
		{
			List<TSM.ModelObject> objects;
			try
			{
				objects= picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_OBJECTS,
				"Pick Model Objects").ToList();
			}
			catch (Exception)
			{
				objects = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return objects;
		}

		/// <summary>
		/// Pick list of parts
		/// </summary>
		/// <returns></returns>
		public List<TSM.Part> PickParts()
		{
			List<TSM.Part> parts;
			try
			{
				parts= picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_PARTS,
				"Pick Parts").ToList()
				.OfType<TSM.Part>()
				.ToList();
			}
			catch (Exception)
			{
				parts = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return parts;
		}

		/// <summary>
		/// Pick list of beams
		/// </summary>
		/// <returns></returns>
		public List<TSM.Beam> PickBeams()
		{
			List<TSM.Beam> beams;
			try
			{
				beams = picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_PARTS,
				"Pick Beams").ToList()
				.OfType<TSM.Beam>()
				.ToList();
			}
			catch (Exception)
			{
				beams = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return beams;
		}

		/// <summary>
		/// Pick list of Contour Plates
		/// </summary>
		/// <returns></returns>
		public List<TSM.ContourPlate> PickContourPlates()
		{
			List<TSM.ContourPlate> plates;
			try
			{
				plates = picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_PARTS,
				"Pick Beams").ToList()
				.OfType<TSM.ContourPlate>()
				.ToList();
			}
			catch (Exception)
			{
				plates = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return plates;
		}

		/// <summary>
		/// Pick list of bolt Group
		/// </summary>
		/// <returns></returns>
		public List<TSM.BoltGroup> PickBoltGroups()
		{
			List<TSM.BoltGroup> boltGroups;
			try
			{
				boltGroups= picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_BOLTGROUPS,
				"Pick Bolt Groups").ToList()
				.OfType<TSM.BoltGroup>()
				.ToList();
			}
			catch (Exception)
			{
				boltGroups = null;
				Operation.DisplayPrompt("Selection is interrupted");
			}
			return boltGroups;
		}

		public  T3D.LineSegment PickLine()
		{
			try
			{
				var arrayList = picker.PickLine("Pick Line");
				if (arrayList == null) return null;

				return new T3D.LineSegment(arrayList[0] as T3D.Point, arrayList[1] as T3D.Point);
			}
			catch (Exception)
			{
				Operation.DisplayPrompt("Selection is interrupted");
				return null;
			}			
		}
		#endregion
	}
}
