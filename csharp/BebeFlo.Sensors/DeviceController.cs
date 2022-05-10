using System;
using BebeFlo.Sensors.Cld88Protocol;

namespace BebeFlo.Sensors
{
	public abstract class DeviceController : IDeviceController
	{
		protected DeviceController(ISensorPort serial, ExternalDevices device)
		{
			if (serial == null)
			{
				throw new ArgumentNullException("serial");
			}
			this._serial = serial;
			this._device = device;
		}

		public abstract void SetupDevice(double? ambientPressureHPa);

		public abstract void StopDevice();

		public ExternalDevices GetDeviceType()
		{
			return this._device;
		}

		protected T LockedExecution<T>(Func<T> fun)
		{
			T result;
			lock (this._serial.SyncRoot)
			{
				result = fun();
			}
			return result;
		}

		protected void LockedExecution(Action action)
		{
			lock (this._serial.SyncRoot)
			{
				action();
			}
		}

		protected readonly ISensorPort _serial;

		protected readonly ExternalDevices _device;
	}
}
