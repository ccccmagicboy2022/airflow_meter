using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	[Flags]
	public enum CldInstrumentError
	{
		None = 0,
		SetupAndCalibDataLost = 1,
		VacuumFailure = 2,
		DeviceHeaterCircuitError = 4,
		ScrubberHeatingFailure = 8,
		OzonatorHighVoltageFailure = 16,
		BypassPressureOutOfRange = 32,
		FlowSensorNotCalibrated = 64,
		PeltierCoolerFailure = 128,
		ConverterHeatingFailure = 256,
		ReactorHeatingFailure = 512,
		TubingHeatingFailure = 1024,
		SampleFlowOutOfRange = 2048,
		HardwareDefectITypeChanged = 4096,
		CalibrationError = 8192,
		ZeroAirPressureOutOfRange = 16384,
		ReferenceBottlePressureOutOfRange = 32768
	}
}
