using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AUTRA.Design
{
	public struct Tolerance
	{
		public const double DIST_TOL = 0.001;
	}
	public enum BeamDesignStatus
	{
		SAFE,VERY_SAFE ,UNSAFE
	}
	public enum BoltedConnectionCategory
	{
		BEARING_NON_PRETENSIONED,SLIP_PRETENSIONED,BEARING_PRETENSIONED
	}
	[DefaultValue(NONE)]
	public enum LoadPattern
	{
		NONE,DEAD,LIVE,WIND,COMBINATION
	}
	public enum Compactness
	{
		COMPACT,NONCOMPACT,SLENDER
	}
	public enum UnSupportedLength
	{
		SUPPORTED,UNSUPPORTED
	}
	public enum BeamType
	{
		FLOOR,CRANE,PURLIN
	}
	public enum StressType
	{
		FLEXURAL,AXIAL,COMBINED
	}
	public enum BracingCondition
	{
		BRACED,UNBRACED
	}
	public enum SteelType
	{
		ST_37,ST_44,ST_52
	}
	
}
