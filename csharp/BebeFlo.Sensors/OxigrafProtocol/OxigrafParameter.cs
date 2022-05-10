using System;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public enum OxigrafParameter : byte
	{
		SystemStatus = 48,
		OxygenConcentration,
		SampleCellPressure,
		SampleCellTemperature,
		OxygenPartialPressure,
		TimeStamp,
		CO2Concentration = 55,
		CO2SampleCellPressure,
		CO2SampleCellTemperature
	}
}
