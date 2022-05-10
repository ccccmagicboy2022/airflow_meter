using System;
using System.Collections.Generic;
using System.Linq;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2Configuration
	{
        //肺功能ExhalyzerD的通道配置
        public void ConfigureForExhalyzerD(double CO2Gain, double CO2Offset, double O2Gain, double O2Offset, double MMssGain, double MMssOffset, int x2PortFlowAs, int x2PortCapnostat)
		{
            //设备
			X2ExternalDevice device = X2ExternalDeviceFactory.CreateStandardDevice(ExternalDevices.FlowAS, MMssGain, MMssOffset, x2PortFlowAs);
            //通道0，采样流速
            this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog0,
				MeasurementType = MeasurementType.SampleFlow,
				Device = device,
				IsSampled = true,
				ChannelName = "SampleFlow"
			});
            //通道1，MMss
            this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog1,
				MeasurementType = MeasurementType.MMss,
				Device = device,
				IsSampled = true,
				ChannelName = "MMss"
			});
            //通道2，MMms
            this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.MolarMass,
				MeasurementType = MeasurementType.MMms,
				Device = device,
				IsSampled = true,
				ChannelName = "MMms"
			});
            //二氧化碳
			X2ExternalDevice device2 = X2ExternalDeviceFactory.CreateATSVersionCapnostatDevice(CO2Gain, CO2Offset, x2PortCapnostat);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog2,
				MeasurementType = MeasurementType.CO2,
				Device = device2,
				IsSampled = true,
				ChannelName = "CO2"
			});
            //氧气
            X2ExternalDevice device3 = X2ExternalDeviceFactory.CreateStandardDevice(ExternalDevices.Oxigraf, O2Gain, O2Offset, 0);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog3,
				MeasurementType = MeasurementType.O2,
				Device = device3,
				IsSampled = true,
				ChannelName = "O2"
			});
            //外部设备4
			X2AnalogExternalDevice device4 = new X2AnalogExternalDevice(1.0, 0.0);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog4,
				MeasurementType = MeasurementType.ValidationType1,
				Device = device4,
				IsSampled = false,
				IsSynchronized = true,
				ChannelName = "channel 5 (1-8)"
			});
            //外部设备5
            X2AnalogExternalDevice device5 = new X2AnalogExternalDevice(1.0, 0.0);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog5,
				MeasurementType = MeasurementType.ValidationType2,
				Device = device5,
				IsSampled = false,
				IsSynchronized = true,
				ChannelName = "channel 6 (1-8)"
			});
            //外部设备6
            X2AnalogExternalDevice device6 = new X2AnalogExternalDevice(1.0, 0.0);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog6,
				MeasurementType = MeasurementType.ValidationType3,
				Device = device6,
				IsSampled = false,
				IsSynchronized = true,
				ChannelName = "channel 7 (1-8)"
			});
            //外部设备7
            X2AnalogExternalDevice device7 = new X2AnalogExternalDevice(1.0, 0.0);
			this.AddAnalogChannelConfig(new X2Configuration.AnalogChannelConfig
			{
				ChannelNumber = X2ChannelNumber.Analog7,
				MeasurementType = MeasurementType.ValidationType4,
				Device = device7,
				IsSampled = false,
				IsSynchronized = true,
				ChannelName = "channel 8 (1-8)"
			});
		}

		public void AddAnalogChannelConfig(X2Configuration.AnalogChannelConfig cfg)
		{
			this._channelSetting[cfg.ChannelNumber] = cfg;
		}

		public List<X2ExternalDevice> SamplingSerialDevices()
		{
			List<X2ExternalDevice> list = new List<X2ExternalDevice>();
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.Device.IsSerialDevice() && current.IsSampled && !list.Contains((X2ExternalDevice)current.Device))
				{
					list.Add((X2ExternalDevice)current.Device);
				}
			}
			return list;
		}

		public List<IExternalDevice> AllDevices()
		{
			List<IExternalDevice> list = new List<IExternalDevice>();
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (!list.Contains(current.Device))
				{
					list.Add(current.Device);
				}
			}
			return list;
		}

		public List<X2ExternalDevice> AllSerialDevices()
		{
			List<X2ExternalDevice> list = new List<X2ExternalDevice>();
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.Device.IsSerialDevice() && !list.Contains((X2ExternalDevice)current.Device))
				{
					list.Add((X2ExternalDevice)current.Device);
				}
			}
			return list;
		}

		public X2ExternalDevice GetSerialDevice(ExternalDevices deviceType)
		{
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.Device.DeviceType == deviceType && current.Device.IsSerialDevice())
				{
					return (X2ExternalDevice)current.Device;
				}
			}
			throw new ArgumentException(string.Format("not configured for deviceType {0}", deviceType), "deviceType");
		}

		public void UpdateSerialDevice(MeasurementType measurementType, double gain, double offset)
		{
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.MeasurementType == measurementType && current.Device.IsSerialDevice())
				{
					((X2ExternalDevice)current.Device).Update(measurementType, gain, offset);
					return;
				}
			}
			throw new ArgumentException(string.Format("not configured for measurement type {0}", measurementType), "measurementType");
		}

		public List<X2Configuration.AnalogChannelConfig> SampledChannels(X2ExternalDevice device)
		{
			List<X2Configuration.AnalogChannelConfig> list = new List<X2Configuration.AnalogChannelConfig>();
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.Device.DeviceType == device.DeviceType && current.IsSampled)
				{
					list.Add(current);
				}
			}
			return list;
		}

		public List<X2Configuration.AnalogChannelConfig> SampledChannels()
		{
			List<X2Configuration.AnalogChannelConfig> list = new List<X2Configuration.AnalogChannelConfig>();
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.IsSampled)
				{
					list.Add(current);
				}
			}
			return list;
		}

		public List<X2Configuration.AnalogChannelConfig> AllChannels()
		{
			return this._channelSetting.Values.ToList<X2Configuration.AnalogChannelConfig>();
		}

		public X2Configuration.AnalogChannelConfig GetOneAnalogChannelConfig(X2ChannelNumber channelNr)
		{
			return this._channelSetting[channelNr];
		}

		public X2Configuration.AnalogChannelConfig GetOneAnalogChannelConfig(MeasurementType type)
		{
			foreach (X2Configuration.AnalogChannelConfig current in this._channelSetting.Values)
			{
				if (current.MeasurementType == type)
				{
					return current;
				}
			}
			throw new ArgumentException("No channel config for type found", "type");
		}

		public bool IsSampling(MeasurementType m)
		{
			return this.GetOneAnalogChannelConfig(m).IsSampled;
		}

		public static X2ChannelNumber AnalogChannelNumber(X2ChannelFlags analogChannelFlag)
		{
			foreach (Enum @enum in Enum.GetValues(typeof(X2ChannelNumber)))
			{
				if (analogChannelFlag.ToString() == @enum.ToString())
				{
					return (X2ChannelNumber)@enum;
				}
			}
			throw new ArgumentException("X2ChannelFlags seems to have a Channel that does not have a corresponding X2ChannelNumber", "analogChannelFlag");
		}

		public static X2ChannelFlags AnalogChannelFlag(X2ChannelNumber analogChannelNr)
		{
			foreach (Enum @enum in Enum.GetValues(typeof(X2ChannelFlags)))
			{
				if (analogChannelNr.ToString() == @enum.ToString())
				{
					return (X2ChannelFlags)@enum;
				}
			}
			throw new ArgumentException("X2ChannelNumber seems to have a Channel that does not have a corresponding X2ChannelFlags", "analogChannelNr");
		}

		private readonly Dictionary<X2ChannelNumber, X2Configuration.AnalogChannelConfig> _channelSetting = new Dictionary<X2ChannelNumber, X2Configuration.AnalogChannelConfig>();

		public struct AnalogChannelConfig
		{
			public bool CanSynchronize
			{
				get
				{
					return this.IsSampled && this.IsSynchronized;
				}
			}

			public X2ChannelNumber ChannelNumber;

			public IExternalDevice Device;

			public bool IsSampled;

			public MeasurementType MeasurementType;

			public string ChannelName;

			public bool IsSynchronized;
		}
	}
}
