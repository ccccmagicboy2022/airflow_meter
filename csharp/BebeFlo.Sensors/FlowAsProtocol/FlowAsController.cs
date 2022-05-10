using System;
using BebeFlo.Sensors.Cld88Protocol;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors.FlowAsProtocol
{
	public class FlowAsController : X2Messaging, IDeviceController
	{
		public FlowAsController(ISensorPort serial) : base(serial)
		{
			this._device = ExternalDevices.FlowAS;
		}

		public ExternalDevices GetDeviceType()
		{
			return this._device;
		}

		public void SetupDevice(double? ambientPressureHPa)
		{
			lock (this._serial.SyncRoot)
			{
				base.GetVersion();
				base.GetDeviceStatus();
			}
		}

		public void StopDevice()
		{
			base.SendMessage(1);
		}

		private ExternalDevices _device;
	}
}
