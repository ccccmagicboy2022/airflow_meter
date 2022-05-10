using System;
using System.Collections.Generic;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors
{
	public class X2ExternalDevice : IExternalDevice
	{
		public ExternalDevices DeviceType
		{
			get;
			set;
		}

		public byte X2Port
		{
			get;
			set;
		}

		public int X2DeviceType
		{
			get;
			set;
		}

		public int BaudRate
		{
			get;
			set;
		}

		public byte Settings
		{
			get;
			set;
		}

		public int PeriodicInterruptTimerRate
		{
			get;
			set;
		}

		public int DataBits()
		{
			return (int)((this.Settings & 3) + 5);
		}

		public void SetMeasurementChannel(MeasurementType m, MeasurementChannelSettings settings)
		{
			if (this._measurementChannels.ContainsKey(m))
			{
				this._measurementChannels[m] = settings;
				return;
			}
			this._measurementChannels.Add(m, settings);
		}

		public byte GetExternalPort(MeasurementType m)
		{
			return (byte)((int)(this.X2Port * 8) + this.ChannelSetting(m).ChannelNr);
		}

		public void Update(MeasurementType m, double gain, double offset)
		{
			MeasurementChannelSettings settings = this.ChannelSetting(m);
			settings.Gain = gain;
			settings.Offset = offset;
			this.SetMeasurementChannel(m, settings);
		}

		private MeasurementChannelSettings ChannelSetting(MeasurementType m)
		{
			if (this._measurementChannels.ContainsKey(m))
			{
				return this._measurementChannels[m];
			}
			throw new ArgumentException(string.Format("Measurement ({0}) not configured for this device ({1})", m, this.DeviceType));
		}

		public bool IsSerialDevice()
		{
			return true;
		}

		public bool IsAnalogDevice()
		{
			return false;
		}

		public double Gain(MeasurementType m)
		{
			return this.ChannelSetting(m).Gain;
		}

		public double Offset(MeasurementType m)
		{
			return this.ChannelSetting(m).Offset;
		}

		public Func<int, float> GetConverter(MeasurementType m)
		{
			MeasurementChannelSettings mSettings = this.ChannelSetting(m);
			return (int val) => (float)((double)X2Utils.TwoComplementSignedSignal(val, 12) * mSettings.Gain + mSettings.Offset);
		}

		private readonly Dictionary<MeasurementType, MeasurementChannelSettings> _measurementChannels = new Dictionary<MeasurementType, MeasurementChannelSettings>();
	}
}
