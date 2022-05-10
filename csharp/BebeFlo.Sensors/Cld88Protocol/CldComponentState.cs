using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	[Flags]
	public enum CldComponentState
	{
		None = 0,
		ScrubberHeatingOn = 1,
		ReactionChamberHeatingOn = 2,
		AdditionalConverterHeatingOn = 4,
		HotTubingHeatingOn = 8,
		PeltierCoolingOn = 16,
		FlagH5 = 32,
		OzoneGeneratorOn = 256,
		CalibrationValveOpen = 512,
		VacuumPumpOn = 1024,
		PressureRegulationOn = 2048,
		RecorderOn = 4096,
		AuxDeviceOn = 8192
	}
}
