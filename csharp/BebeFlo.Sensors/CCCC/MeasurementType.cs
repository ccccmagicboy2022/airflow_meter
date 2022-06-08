using System;

namespace BebeFlo.Sensors
{
	public enum MeasurementType : byte
	{
		Flow,
		SampleFlow,
		NO,
		O2,
		CO2,
		MMss,
		MMms,
		Volume,
		Ar,
		He,
		N2,
		SF6,
		ValidationType1,
		ValidationType2,
		ValidationType3,
		ValidationType4,
		DelayO2,
		DelayMMss,
		Undefined = 255
	}
}
