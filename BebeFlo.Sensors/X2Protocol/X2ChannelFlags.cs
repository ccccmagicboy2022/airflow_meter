using System;

namespace BebeFlo.Sensors.X2Protocol
{
	[Flags]
	public enum X2ChannelFlags : ushort
	{
		Analog0 = 1,
		Analog1 = 2,
		Analog2 = 4,
		Analog3 = 8,
		Analog4 = 16,
		Analog5 = 32,
		Analog6 = 64,
		Analog7 = 128,
		CaseTemp = 256,
		RoomTemp = 512,
		Pressure = 1024,
		Flow = 2048,
		MolarMass = 4096,
		TransitTime1 = 8192,
		TransitTime2 = 16384,
		Control = 32768
	}
}
