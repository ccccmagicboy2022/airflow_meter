using System;

namespace BebeFlo.Sensors
{
	public interface IExternalDevice
	{
		bool IsSerialDevice();

		bool IsAnalogDevice();

		ExternalDevices DeviceType
		{
			get;
		}

		double Gain(MeasurementType m);

		double Offset(MeasurementType m);

		Func<int, float> GetConverter(MeasurementType m);
	}
}
