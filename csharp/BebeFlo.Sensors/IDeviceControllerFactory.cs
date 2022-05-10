using System;
using BebeFlo.Sensors.CapnostatProtocol;
using BebeFlo.Sensors.FlowAsProtocol;
using BebeFlo.Sensors.OxigrafProtocol;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors
{
	public class IDeviceControllerFactory
	{
		public static IDeviceController CreateControllerForDevice(X2ExternalDevice device, IX2ControllerDirectCommands directCommands)
		{
			SensorToX2Wrapper serial = new SensorToX2Wrapper(device, directCommands, 5000);
			IDeviceController result;
			switch (device.DeviceType)
			{
			case ExternalDevices.Oxigraf:
				result = new OxigrafController(serial);//氧气传感器控制器
				break;
			case ExternalDevices.Capnostat:
				result = new CapnostatController(serial);//二氧化碳传感器控制器
				break;
			case ExternalDevices.FlowAS:
				result = new FlowAsController(serial);//流速传感器控制器
				break;
			default:
				throw new ArgumentException(string.Format("unecpected device of type {0}", device.DeviceType), "device");
			}
			return result;
		}

		public const int sensorToX2ReadTimeOutMilliseconds = 5000;
	}
}
