using System;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public class OxigrafMeasurementReport
	{
		public double O2Concentration
		{
			get;
			set;
		}

		public double CO2Concentration
		{
			get;
			set;
		}

		public double O2SampleCellPressure
		{
			get;
			set;
		}

		public double CO2SampleCellPressure
		{
			get;
			set;
		}

		public double O2SampleCellTemperature
		{
			get;
			set;
		}

		public double CO2SampleCellTemperature
		{
			get;
			set;
		}
	}
}
