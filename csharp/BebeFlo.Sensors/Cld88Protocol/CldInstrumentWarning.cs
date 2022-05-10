using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	[Flags]
	public enum CldInstrumentWarning
	{
		None = 0,
		ConverterLifetimeExceeded = 1,
		PumpMaintenanceRequired = 2,
		InstrumentTemperatureLow = 4,
		InstrumentTemperatureHigh = 8,
		BypassOutOfAllowedPressure = 16,
		W06 = 32,
		ServiceAccessSet = 64,
		W08 = 128,
		RangeOverflow = 256,
		O3UpOzonNotConstant = 512,
		W11 = 1024,
		W12 = 2048,
		RangeBOverflow = 4096,
		W14 = 8192,
		W15 = 16384,
		W16 = 32768
	}
}
