using System;

namespace BebeFlo.Sensors
{
	public class X2ExternalDeviceFactory
	{
		public static X2ExternalDevice CreateStandardDevice(ExternalDevices deviceType, double gain, double offset, int x2Port)
		{
			if (x2Port < 0 || 3 < x2Port)
			{
				throw new ArgumentOutOfRangeException("X2 Port for device must be within [0..3]", "x2Port");
			}
			X2ExternalDevice result;
			switch (deviceType)
			{
			case ExternalDevices.Oxigraf:
				result = X2ExternalDeviceFactory.StandardOxigraf(gain, offset, x2Port);
				break;
			case ExternalDevices.Capnostat:
				result = X2ExternalDeviceFactory.StandardCapnostat(x2Port);
				break;
			case ExternalDevices.FlowAS:
				result = X2ExternalDeviceFactory.StandardFlowAs(gain, offset, x2Port);
				break;
			default:
				throw new ArgumentException(string.Format("No standard X2ExternalDevice for {0} exists", deviceType));
			}
			return result;
		}

		public static X2ExternalDevice CreateATSVersionCapnostatDevice(double gain, double offset, int x2Port)
		{
			if (x2Port < 0 || 3 < x2Port)
			{
				throw new ArgumentOutOfRangeException("X2 Port for device must be within [0..3]", "x2Port");
			}
			X2ExternalDevice x2ExternalDevice = new X2ExternalDevice
			{
				X2Port = (byte)x2Port,
				X2DeviceType = 16,
				PeriodicInterruptTimerRate = 0,
				DeviceType = ExternalDevices.Capnostat,
				BaudRate = 19200,
				Settings = 3
			};
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.CO2, new MeasurementChannelSettings
			{
				ChannelNr = 0,
				Gain = gain,
				Offset = offset,
				MeasurementType = MeasurementType.CO2,
				Unit = "%"
			});
			return x2ExternalDevice;
		}

		private static X2ExternalDevice StandardCapnostat(int x2Port)
		{
			X2ExternalDevice x2ExternalDevice = new X2ExternalDevice
			{
				X2Port = (byte)x2Port,
				X2DeviceType = 16,
				PeriodicInterruptTimerRate = 0,
				DeviceType = ExternalDevices.Capnostat,
				BaudRate = 19200,
				Settings = 3
			};
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.CO2, new MeasurementChannelSettings
			{
				ChannelNr = 0,
				Gain = 0.08,
				Offset = 163.8,
				MeasurementType = MeasurementType.CO2,
				Unit = "%"
			});
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.O2, new MeasurementChannelSettings
			{
				ChannelNr = 1,
				Gain = 0.05,
				Offset = 102.4,
				MeasurementType = MeasurementType.O2,
				Unit = "%"
			});
			return x2ExternalDevice;
		}

		private static X2ExternalDevice StandardOxigraf(double gain, double offset, int x2Port)
		{
			X2ExternalDevice x2ExternalDevice = new X2ExternalDevice
			{
				X2Port = (byte)x2Port,
				X2DeviceType = 0,
				PeriodicInterruptTimerRate = 0,
				DeviceType = ExternalDevices.Oxigraf,
				BaudRate = 9600,
				Settings = 3
			};
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.O2, new MeasurementChannelSettings
			{
				ChannelNr = 0,
				Gain = gain,
				Offset = offset,
				MeasurementType = MeasurementType.O2,
				Unit = "%"
			});
			return x2ExternalDevice;
		}

		private static X2ExternalDevice StandardFlowAs(double gain, double offset, int x2Port)
		{
			X2ExternalDevice x2ExternalDevice = new X2ExternalDevice
			{
				X2Port = (byte)x2Port,
				X2DeviceType = 12,
				PeriodicInterruptTimerRate = 0,
				DeviceType = ExternalDevices.FlowAS,
				BaudRate = 57600,
				Settings = 131
			};
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.MMss, new MeasurementChannelSettings
			{
				ChannelNr = 1,
				Gain = gain,
				Offset = offset,
				MeasurementType = MeasurementType.MMss,
				Unit = "g/mol"
			});
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.MMms, new MeasurementChannelSettings
			{
				ChannelNr = 12,
				Gain = 0.01,
				Offset = 28.96,
				MeasurementType = MeasurementType.MMms,
				Unit = "g/mol"
			});
			x2ExternalDevice.SetMeasurementChannel(MeasurementType.SampleFlow, new MeasurementChannelSettings
			{
				ChannelNr = 0,
				Gain = -0.125,
				Offset = 0.0,
				MeasurementType = MeasurementType.SampleFlow,
				Unit = "ml/s"
			});
			return x2ExternalDevice;
		}
	}
}
