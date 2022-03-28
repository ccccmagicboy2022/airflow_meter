using System;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public struct CldMeasurements
	{
		public override string ToString()
		{
			return ReflectionHelper.DbgDumpPublicAttributes(this);
		}

		public double NOChannelA;

		public double NOxChannelA;

		public double NOChannelB;

		public double NOxChannelB;

		public double NO2;

		public double NH3;
	}
}
