using System;

namespace BebeFlo.Sensors
{
	public struct MeasurementChannelSettings
	{
		public MeasurementType MeasurementType;

		public int ChannelNr;

		public double Gain;

		public double Offset;

		public string Unit;
	}
}
