using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public enum CapnostatSettings : byte
	{
		InvalidSetting,
		BarometricPressure,
		GasTemperature = 4,
		CurrentETCO2Period,
		NoBreathsDetectedTimeout,
		CurrentCO2Units,
		SleepMode,
		ZeroGasType,
		GasCompensations = 11,
		GetSensorPartNumber = 18,
		GetOEMid,
		GetSensorSerialNumber,
		GetHardwareRevisionNumber,
		GetTotalUseTime = 23,
		GetLastZeroTime
	}
}
