using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldOperatingTime
	{
		public int VacuumPumpHours
		{
			get;
			set;
		}

		public int AnalyzerHours
		{
			get;
			set;
		}

		public int ConverterHours
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
