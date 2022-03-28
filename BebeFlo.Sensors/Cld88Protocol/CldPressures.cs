using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldPressures
	{
		public int BypassRegulation
		{
			get;
			set;
		}

		public int ReactionChamber
		{
			get;
			set;
		}

		public int ZeroCalibrationGas
		{
			get;
			set;
		}

		public int SpanCalibrationGas
		{
			get;
			set;
		}

		public int ReactionChamberWithClosedInledValve
		{
			get;
			set;
		}

		public int TubeType
		{
			get;
			set;
		}

		public override string ToString()
		{
			return ReflectionHelper.DbgDumpPublicAttributes(this);
		}
	}
}
