using System;

namespace BebeFlo.Sensors
{
	public interface IDeviceController
	{
		ExternalDevices GetDeviceType();

		void SetupDevice(double? ambientPressureHPa);

		void StopDevice();
	}
}
