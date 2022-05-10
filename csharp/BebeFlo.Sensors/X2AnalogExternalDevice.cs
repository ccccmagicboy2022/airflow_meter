using System;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors
{
	public class X2AnalogExternalDevice : IExternalDevice
	{
		public X2AnalogExternalDevice()
		{
		}

		public void Update(double gain, double offset)
		{
			this._gain = gain;
			this._offset = offset;
		}

		public X2AnalogExternalDevice(double gain, double offset)
		{
			this._gain = gain;
			this._offset = offset;
		}

		public void SetLow(int signal, double value)
		{
			this._lowSignal = X2Utils.TwoComplementSignedSignal(signal, 12);
			this._lowValue = value;
			this.ComputeGainAndOffset();
		}

		public void SetHigh(int signal, double value)
		{
			this._highSignal = X2Utils.TwoComplementSignedSignal(signal, 12);
			this._highValue = value;
			this.ComputeGainAndOffset();
		}

		public bool IsSerialDevice()
		{
			return false;
		}

		public bool IsAnalogDevice()
		{
			return true;
		}

		public ExternalDevices DeviceType
		{
			get
			{
				return this._deviceType;
			}
		}

		public double Gain(MeasurementType m)
		{
			return this._gain;
		}

		public double Offset(MeasurementType m)
		{
			return this._offset;
		}

		public Func<int, float> GetConverter(MeasurementType m)
		{
			return (int val) => (float)((double)X2Utils.TwoComplementSignedSignal(val, 12) * this._gain + this._offset);
		}

		private void ComputeGainAndOffset()
		{
			this._gain = (this._highValue - this._lowValue) / (double)(this._highSignal - this._lowSignal);
			this._offset = this._highValue - (double)this._highSignal * this._gain;
		}

		private double _highValue;

		private double _lowValue;

		private int _highSignal;

		private int _lowSignal;

		private double _gain;

		private double _offset;

		private ExternalDevices _deviceType = ExternalDevices.LinearAnalogDevice;
	}
}
