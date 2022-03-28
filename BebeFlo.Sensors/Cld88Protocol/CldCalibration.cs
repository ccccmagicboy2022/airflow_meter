using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldCalibration
	{
		public double Zero
		{
			get;
			set;
		}

		public double Slope
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
