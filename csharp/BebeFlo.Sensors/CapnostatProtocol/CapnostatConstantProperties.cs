using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public class CapnostatConstantProperties
	{
		public string SensorPartNumber
		{
			get;
			set;
		}

		public string SensorSerialNumber
		{
			get;
			set;
		}

		public CapnostatCO2Units CO2Unit
		{
			get;
			set;
		}

		public CapnostatSleepMode SleepMode
		{
			get;
			set;
		}

		public int TotalUseTimeMinutes
		{
			get;
			set;
		}

		public int LastZeroTimeMinutes
		{
			get;
			set;
		}
	}
}
