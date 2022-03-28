using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public enum CapnostatPrioritizedStatus : byte
	{
		NoErrorsWarnings,
		SensorOverTemp,
		SensorFaulty,
		SetBarometricPressureAndCompensation,
		SleepMode,
		ZeroInProgress,
		SensorWarmUp,
		PneumaticPressureOutsideRange = 10,
		ZeroRequired = 7,
		CO2OutOfRange,
		CheckAirwayAdapter,
		StatusNotReceived = 133
	}
}
