using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BebeFlo.Sensors.Cld88Protocol;
using BebeFlo.Sensors.X2Protocol;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public class CapnostatController : DeviceController
	{
		public CapnostatController(ISensorPort serial) : base(serial, ExternalDevices.Capnostat)
		{
		}

		public override void SetupDevice(double? pressureHPa)
		{
			base.LockedExecution(delegate
			{
				DateTime now = DateTime.Now;
				bool flag = false;
				while (DateTime.Now - now <= TimeSpan.FromSeconds(5.0) && !flag)
				{
					CapnostatResponse capnostatResponse = this.StopContinuousMode();
					if (!capnostatResponse.IsError())
					{
						flag = true;
					}
					else if (capnostatResponse.Data[0] != 0)
					{
						throw new CapnostatProtocolException(string.Format("Startup fails. NACK Error-Code non-zero: {0}", capnostatResponse.ErrorName()));
					}
					Thread.Sleep(500);
				}
				if (!flag)
				{
					throw new CapnostatProtocolException("Sensor faulty. Startup fails. No valid response within 5 seconds.");
				}
				if (pressureHPa.HasValue)
				{
					this.SetPressure(pressureHPa.Value);
				}
				this.SetCO2UnitToPercent();
				this.SetO2Compensation(0);
				this.CO2WaveformDataMode();
			});
		}

		public override void StopDevice()
		{
			base.LockedExecution(delegate
			{
				this.StopContinuousMode();
			});
		}

		public bool InitializeDevice(double pressureHPa)
		{
			return base.LockedExecution<bool>(delegate
			{
				bool result;
				try
				{
					this.SetPressure(pressureHPa);
					this.SetCO2UnitToPercent();
					this.SetO2Compensation(0);
					result = true;
				}
				catch (TimeoutException)
				{
					result = false;
				}
				catch (CapnostatProtocolException)
				{
					result = false;
				}
				return result;
			});
		}

		public bool SystemResponding()
		{
			return base.LockedExecution<bool>(delegate
			{
				bool result;
				try
				{
					this.StopDevice();
					this.GetAvailableCapabilities();
					result = true;
				}
				catch (TimeoutException)
				{
					result = false;
				}
				catch (CapnostatProtocolException)
				{
					result = false;
				}
				return result;
			});
		}

		public CapnostatPrioritizedStatus GetStatus()
		{
			if (this._serial.GetType() != typeof(SensorToX2Wrapper))
			{
				throw new CapnostatProtocolException("GetStatus expects _serial of Type SensorToX2Wrapper");
			}
			return base.LockedExecution<CapnostatPrioritizedStatus>(delegate
			{
				this.ResetReadContinous();
				this.CO2WaveformDataMode();
				DateTime now = DateTime.Now;
				CapnostatPrioritizedStatus result = CapnostatPrioritizedStatus.StatusNotReceived;
				bool flag = false;
				while (DateTime.Now - now < TimeSpan.FromSeconds(5.0) && !flag)
				{
					this.ReadContinous();
					foreach (CapnostatResponse current in this._continousModeResponses)
					{
						if (current.Data.Length > 4 && current.Data[3] == 1 && current.Data.Length == 9)
						{
							byte[] array = new byte[5];
							Array.Copy(current.Data, 4, array, 0, 5);
							result = new CapnostatStatus(array).GetPrioritizedStatus();
							flag = true;
						}
					}
					this._continousModeResponses.Clear();
				}
				this.ResetReadContinous();
				this.StopContinuousMode();
				return result;
			});
		}

		public void StartZeroing()
		{
			base.LockedExecution(delegate
			{
				this.SetZeroGasTypeToN2();
				this.SendMessage(130, new byte[0]);
				CapnostatResponse capnostatResponse = this.CheckResponse(130);
				if (capnostatResponse.Data.Length != 1)
				{
					throw new CapnostatProtocolException("exactly 1 data byte expected");
				}
				if (capnostatResponse.Data[0] != 0)
				{
					throw new CapnostatProtocolException(string.Format("Zeroing not started. Zero status byte: {0}", capnostatResponse.Data[0]));
				}
			});
		}

		public CapnostatConstantProperties GetConstantProperties()
		{
			return new CapnostatConstantProperties
			{
				CO2Unit = this.GetCurrentCO2Unit(),
				LastZeroTimeMinutes = this.GetLastZeroTimeMinutesAgo(),
				SensorPartNumber = this.GetSensorPartNumber(),
				SensorSerialNumber = this.GetSensorSerialNumber(),
				SleepMode = this.GetSensorSleepMode(),
				TotalUseTimeMinutes = this.GetTotalUseTimeMinutes()
			};
		}

		public string GetSensorPartNumber()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(18, 11);
			byte[] array = new byte[10];
			Array.Copy(sensorSetting.Data, 1, array, 0, 10);
			return this.ByteArrayToString(array);
		}

		private string ByteArrayToString(byte[] arr)
		{
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			return aSCIIEncoding.GetString(arr);
		}

		public string GetSensorSerialNumber()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(20, 6);
			return this.ConvertNByteData(sensorSetting.Data, 1, 5).ToString();
		}

		public CapnostatSleepMode GetSensorSleepMode()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(8, 2);
			return (CapnostatSleepMode)sensorSetting.Data[1];
		}

		public CapnostatCO2Units GetCurrentCO2Unit()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(7, 2);
			return (CapnostatCO2Units)sensorSetting.Data[1];
		}

		public List<CapnostatCapabilities> GetAvailableCapabilities()
		{
			return this.GetCapabilities(0);
		}

		public List<CapnostatCapabilities> GetEnabledCapabilities()
		{
			return this.GetCapabilities(1);
		}

		public int GetTotalUseTimeMinutes()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(23, 6);
			return this.ConvertNByteData(sensorSetting.Data, 1, 5);
		}

		public int GetLastZeroTimeMinutesAgo()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(24, 6);
			return this.ConvertNByteData(sensorSetting.Data, 1, 5);
		}

		public CapnostatZeroGasType GetZeroGasType()
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(9, 2);
			return (CapnostatZeroGasType)sensorSetting.Data[1];
		}

		public void SetPressure(double pressureHPa)
		{
			this.SetBarometricPressure(this.ConvertPressureToMmHg(pressureHPa));
		}

		private int ConvertPressureToMmHg(double pressureHPa)
		{
			return (int)Math.Round(pressureHPa * 0.75006157584565625);
		}

		private void SetBarometricPressure(int pressureMmHg)
		{
			this.SetSensorSetting(1, new byte[]
			{
				(byte)(pressureMmHg / 128 & 127),
				(byte)(pressureMmHg & 127)
			});
		}

		private void SetCO2UnitToPercent()
		{
			this.SetSensorSetting(7, new byte[]
			{
				2
			});
		}

		private void SetO2Compensation(int percent)
		{
			if (percent < 0 || 100 < percent)
			{
				throw new ArgumentException("Compensation must be in per cent", "percent");
			}
			byte arg_29_1 = 11;
			byte[] array = new byte[4];
			array[0] = (byte)percent;
			this.SetSensorSetting(arg_29_1, array);
		}

		private void SetZeroGasTypeToN2()
		{
			byte arg_0B_1 = 9;
			byte[] data = new byte[1];
			this.SetSensorSetting(arg_0B_1, data);
		}

		private void CO2WaveformDataMode()
		{
			byte arg_0E_1 = 128;
			byte[] args = new byte[1];
			this.SendMessage(arg_0E_1, args);
			if (!this.IsSampling())
			{
				throw new CapnostatProtocolException("No data sampling");
			}
		}

		private CapnostatResponse StopContinuousMode()
		{
			this.SendMessage(201, new byte[0]);
			for (int i = 0; i < 10; i++)
			{
				try
				{
					return this.CheckResponse(201);
				}
				catch (Exception ex)
				{
					if (i == 9)
					{
						throw ex;
					}
				}
			}
			throw new CapnostatProtocolException("Stop continous mode failed");
		}

		private CapnostatResponse GetSensorSetting(byte settingId)
		{
			this.SendMessage(132, new byte[]
			{
				settingId
			});
			return this.CheckResponse(132);
		}

		private CapnostatResponse GetSensorSetting(byte settingId, int expectedDataLength)
		{
			CapnostatResponse sensorSetting = this.GetSensorSetting(settingId);
			if (sensorSetting.Data.Length > 0 && sensorSetting.Data[0] != settingId)
			{
				throw new CapnostatProtocolException(string.Format("Response not for expected Setting {0} but for {1}", settingId, sensorSetting.Data[0]));
			}
			if (sensorSetting.Data.Length != expectedDataLength)
			{
				throw new CapnostatProtocolException(string.Format("Response with {0} data bytes expected, got {1} bytes", expectedDataLength, sensorSetting.Data.Length));
			}
			return sensorSetting;
		}

		private void SetSensorSetting(byte settingId, byte[] data)
		{
			byte[] array = new byte[data.Length + 1];
			array[0] = settingId;
			data.CopyTo(array, 1);
			this.SendMessage(132, array);
			this.CheckResponse(132);
		}

		private List<CapnostatCapabilities> GetCapabilities(byte sensorCapabilitiesIndex)
		{
			this.SendMessage(203, new byte[]
			{
				sensorCapabilitiesIndex
			});
			CapnostatResponse capnostatResponse = this.CheckResponse(203);
			if (capnostatResponse.Data.Length < 2)
			{
				throw new CapnostatProtocolException("at least 2 data bytes expected");
			}
			List<CapnostatCapabilities> list = new List<CapnostatCapabilities>();
			IEnumerator enumerator = capnostatResponse.Data.GetEnumerator();
			enumerator.MoveNext();
			while (enumerator.MoveNext())
			{
				list.Add((CapnostatCapabilities)enumerator.Current);
			}
			return list;
		}

		private void SendMessage(byte cmd, byte[] args)
		{
			int num = 0;
			this._outbuf[num++] = cmd;
			this._outbuf[num++] = (byte)(args.Length + 1);
			for (int i = 0; i < args.Length; i++)
			{
				byte b = args[i];
				this._outbuf[num++] = b;
			}
			this._outbuf[num++] = CapnostatUtils.Checksum(this._outbuf, num - 1);
			this._serial.Write(this._outbuf, num);
		}

		private CapnostatResponse CheckResponse(byte cmd)
		{
			CapnostatResponse capnostatResponse = this.ReadReply();
			if (capnostatResponse.Command == cmd)
			{
				return capnostatResponse;
			}
			if (capnostatResponse.IsError())
			{
				throw new CapnostatProtocolException(string.Format("Protocol Error: {0}", capnostatResponse.ErrorName()));
			}
			throw new CapnostatProtocolException(string.Format("Unexpected response command (0x{0:x2}), expected (0x{1:x2})", capnostatResponse.Command, cmd));
		}

		private CapnostatResponse CheckResponse(byte cmd, int length)
		{
			if (length < 2)
			{
				throw new ArgumentException("Response length must be at least 2", "length");
			}
			CapnostatResponse capnostatResponse = this.CheckResponse(cmd);
			if (length != capnostatResponse.Length())
			{
				throw new CapnostatProtocolException("Unexpected response length");
			}
			return capnostatResponse;
		}

		private bool IsSampling()
		{
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				try
				{
					this._serial.Read(this._inbuf, 0, 6);
					for (int j = 0; j < 6; j++)
					{
						if (this._inbuf[j] == 128)
						{
							flag = true;
							break;
						}
					}
				}
				catch (Exception ex)
				{
					if (i == 2)
					{
						throw ex;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return flag;
		}

		private int ConvertNByteData(byte[] nByteData)
		{
			int num = 0;
			int num2 = nByteData.Length;
			for (int i = 0; i < num2; i++)
			{
				num += (int)nByteData[i] << (num2 - 1 - i) * 7;
			}
			return num;
		}

		private int ConvertNByteData(byte[] data, int startIndex, int length)
		{
			byte[] array = new byte[length];
			Array.Copy(data, startIndex, array, 0, length);
			return this.ConvertNByteData(array);
		}

		private void ReadContinous()
		{
			for (int i = 0; i < 10; i++)
			{
				this._x2Buffer.AddRange(((SensorToX2Wrapper)this._serial).ReadX2Buffer());
			}
			while (this._x2Buffer.Count > 1)
			{
				byte b = this._x2Buffer.First<byte>();
				if (CapnostatResponse.IsCommandByte(b))
				{
					try
					{
						int num = (int)(this._x2Buffer[1] + 2);
						if (this._x2Buffer.Count < num)
						{
							break;
						}
						CapnostatResponse capnostatResponse = this.ReadResponse(this._x2Buffer.GetRange(0, num));
						if (capnostatResponse.IsValidResponse())
						{
							this._continousModeResponses.Add(capnostatResponse);
							this._x2Buffer.RemoveRange(0, num);
						}
						else
						{
							this._x2Buffer.RemoveAt(0);
						}
						continue;
					}
					catch
					{
						this._x2Buffer.RemoveAt(0);
						continue;
					}
				}
				this._x2Buffer.RemoveAt(0);
			}
		}

		private void ResetReadContinous()
		{
			this._continousModeResponses.Clear();
			this._x2Buffer.Clear();
		}

		private CapnostatResponse ReadReply()
		{
			for (int i = 0; i < 100; i++)
			{
				this._serial.Read(this._inbuf, 0, 1);
				if (this._inbuf[0] >= 128)
				{
					break;
				}
				if (i == 99)
				{
					throw new TimeoutException("Failed to read reply");
				}
			}
			List<byte> list = new List<byte>();
			list.Add(this._inbuf[0]);
			this._serial.Read(this._inbuf, 0, 1);
			list.Add(this._inbuf[0]);
			int num = (int)list.Last<byte>();
			this._serial.Read(this._inbuf, 0, num);
			for (int j = 0; j < num; j++)
			{
				list.Add(this._inbuf[j]);
			}
			CapnostatResponse capnostatResponse = this.ReadResponse(list);
			if (!capnostatResponse.IsValidResponse())
			{
				throw new CapnostatProtocolException("Invalid response");
			}
			return capnostatResponse;
		}

		private CapnostatResponse ReadResponse(List<byte> responseBytes)
		{
			if (responseBytes.Count < 3)
			{
				throw new ArgumentException("Invalid response length: at least 3 Bytes expected", "responseBytes");
			}
			if ((int)(responseBytes[1] + 2) != responseBytes.Count)
			{
				throw new ArgumentException(string.Format("Invalid response lenght: expected ({0}), actual ({1})", responseBytes.Count, (int)(responseBytes[1] + 2)), "responseBytes");
			}
			CapnostatResponse capnostatResponse = new CapnostatResponse();
			capnostatResponse.Command = responseBytes[0];
			capnostatResponse.NumberOfBytesToFollow = (int)responseBytes[1];
			capnostatResponse.Data = responseBytes.GetRange(2, capnostatResponse.NumberOfBytesToFollow - 1).ToArray();
			capnostatResponse.CheckSum = responseBytes.Last<byte>();
			return capnostatResponse;
		}

		private readonly byte[] _outbuf = new byte[40];

		private readonly byte[] _inbuf = new byte[100];

		private readonly List<CapnostatResponse> _continousModeResponses = new List<CapnostatResponse>();

		private readonly List<byte> _x2Buffer = new List<byte>();
	}
}
