using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BebeFlo.Sensors.CapnostatProtocol;
using BebeFlo.Utils;
using log4net;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2Controller : X2Messaging, IX2ControllerDirectCommands
	{
		public object SyncRoot
		{
			get
			{
				return this._serial.SyncRoot;
			}
		}

		public bool InvertFlow
		{
			get
			{
				return this._invertFlow;
			}
			set
			{
				this._invertFlow = value;
			}
		}

		public int IncomingMessagesCount
		{
			get
			{
				return this._incomingMessagesCount;
			}
		}

		public TimeSpan SendExternalDeviceStringThrottleDelay
		{
			get
			{
				return this._sendExternalDeviceStringThrottleDelay;
			}
			set
			{
				this._sendExternalDeviceStringThrottleDelay = value;
			}
		}

		public X2Configuration ChannelConfiguration
		{
			get
			{
				return this._channelConfig;
			}
		}

		public IX2Port X2Port
		{
			get
			{
				return this._serial as IX2Port;
			}
		}

		public bool IsMainASofTypeAsicA
		{
			get
			{
				if (this._asMainVersions == null)
				{
					this._asMainVersions = this.GetMainASVersions();
				}
				return this._asMainVersions.Hardware < 32;
			}
		}

		public bool IsMainASFirmwareV1x
		{
			get
			{
				if (this._asMainVersions == null)
				{
					this._asMainVersions = this.GetMainASVersions();
				}
				return this._asMainVersions.Firmware[0] < 2;
			}
		}

		public X2ValveStatus OxygenValveStatus
		{
			get;
			private set;
		}

		public X2Controller(IX2Port serial, bool reverseValve2Selection, int x2PortFlowAs, int x2PortCapnostat, double CO2Gain, double CO2Offset, double O2Gain, double O2Offset, double MMssGain, double MMssOffset) : base(serial)
		{
			this._reverseValve2Selection = reverseValve2Selection;
            //设备通道配置
			this._channelConfig.ConfigureForExhalyzerD(CO2Gain, CO2Offset, O2Gain, O2Offset, MMssGain, MMssOffset, x2PortFlowAs, x2PortCapnostat);
		}
        //X2的版本信息
		public X2Versions GetMainASVersions()
		{
			byte[] array = new byte[5];
			array[0] = 1;
			array[1] = 1;
			array[2] = 202;
			X2Messaging.AppendCrc(array, 3);
			X2Versions result;
			lock (this._serial.SyncRoot)
			{
				this._serial.PurgeInBuffer();
				this.SendExternalDeviceString(3, 0, array);
				byte[] array2 = this.ReadExternalDeviceString(3);
				result = new X2Versions
				{
					BootLoader = new int[]
					{
						(int)array2[1],
						(int)array2[2],
						(int)array2[3],
						(int)array2[4]
					},
					Firmware = new int[]
					{
						(int)array2[5],
						(int)array2[6],
						(int)array2[7],
						(int)array2[8]
					},
					Hardware = (int)array2[9],
					ModuleInfo1 = (int)array2[10],
					ModuleInfo2 = (int)array2[11],
					SerialNo2 = (int)X2Utils.UInt16(array2[12], array2[13]),
					SerialNo1 = (int)X2Utils.UInt16(array2[14], array2[15]),
					FirmwareCrc = (int)X2Utils.UInt16(array2[16], array2[17]),
					SensorStatus = (int)array2[19]
				};
			}
			return result;
		}
        //获得OpC判断是否流速精确度
		public X2OpCodes GetOpCodes()
		{
			X2OpCodes result;
			lock (this._serial.SyncRoot)
			{
				X2OpCodes x2OpCodes = new X2OpCodes();
				base.SendMessage(219, 19);
				base.ReadMessageOfSize(5);
				x2OpCodes.MSB = (int)X2Utils.Int16(this._inbuf[1], this._inbuf[2]);
				base.SendMessage(219, 20);
				base.ReadMessageOfSize(5);
				x2OpCodes.LSB = (int)X2Utils.Int16(this._inbuf[1], this._inbuf[2]);
				base.SendMessage(219, 2);
				base.ReadMessageOfSize(5);
				x2OpCodes.XSB = (int)X2Utils.Int16(this._inbuf[1], this._inbuf[2]);
				result = x2OpCodes;
			}
			return result;
		}

		public void InitializeIfNecessary()
		{
			if (!this._isInitialized)
			{
				this.InitializeFlowResolution();
				this.Reset();
				this._isInitialized = true;
			}
		}
        //流速精确度
		public void InitializeFlowResolution()
		{
			X2SensorSize sensorSize = this.GetOpCodes().SensorSize;
			switch (sensorSize)
			{
			case X2SensorSize.Small:
				this._flowResolution = 1.25f;
				return;
			case X2SensorSize.Medium:
				this._flowResolution = 5f;
				return;
			case X2SensorSize.Large:
				this._flowResolution = 10f;
				return;
			case X2SensorSize.XLarge:
				this._flowResolution = 80f;
				return;
			default:
				throw new X2ProtocolException(string.Format("Unknown flow resolution for sensor size: {0}", sensorSize));
			}
		}

		public void Reset()
		{
			lock (this._serial.SyncRoot)
			{
				base.StandbyOn();
			}
		}

		public void StartDirectLinkToCld()
		{
			lock (this._serial.SyncRoot)
			{
				byte arg_28_1 = 30;
				byte arg_28_2 = 2;
				byte arg_28_3 = 96;
				byte[] array = new byte[2];
				array[0] = 2;
				base.SendMessage(arg_28_1, arg_28_2, arg_28_3, array);
				this.X2Port.ReconfigureForCld88();
			}
		}

		public void StartDirectLink(ExternalDevices deviceType, bool isDsrSet1)
		{
			if (deviceType.Equals(ExternalDevices.Cld))
			{
				this.StartDirectLinkToCld();
				return;
			}
			lock (this._serial.SyncRoot)
			{
				this.OperateValve7Air(!isDsrSet1);
				if (!deviceType.Equals(ExternalDevices.FlowAS))
				{
					this.ActivatePump();
				}
				this.StartDirectLink(this._channelConfig.GetSerialDevice(deviceType));
			}
		}

		public void StartDirectLink(X2ExternalDevice device)
		{
			lock (this._serial.SyncRoot)
			{
				IDeviceController deviceController = IDeviceControllerFactory.CreateControllerForDevice(device, this);
				deviceController.SetupDevice(null);
				byte arg_55_1 = 30;
				byte arg_55_2 = device.X2Port;
				byte arg_55_3 = X2Utils.X2BaudRateRepresentation(device.BaudRate);
				byte[] array = new byte[2];
				array[0] = device.Settings;
				base.SendMessage(arg_55_1, arg_55_2, arg_55_3, array);
				this.X2Port.ReconfigureForExternalDevice(device.BaudRate, device.DataBits());
				this.X2Port.Close();
			}
		}

		public void StopDirectLink(ExternalDevices deviceType)
		{
			lock (this._serial.SyncRoot)
			{
				this._outbuf[0] = 43;
				for (int i = 0; i < 3; i++)
				{
					this._serial.Write(this._outbuf, 1);
					Thread.Sleep(300);
				}
				Thread.Sleep(4000);
				this.X2Port.ReconfigureForX2();
				if (!deviceType.Equals(ExternalDevices.Cld))
				{
					for (int j = 0; j < 20; j++)
					{
						if (base.GetVersion().SensorStatus != 0)
						{
							if (!deviceType.Equals(ExternalDevices.FlowAS))
							{
								this.DeactivatePump();
							}
							this.OperateValve7Air(false);
							break;
						}
						Thread.Sleep(500);
					}
				}
			}
		}

		public void SendDirectCommand(X2ExternalDevice device, byte[] command)
		{
			X2Controller._log.DebugFormat("SendDirectCommand: DeviceType={0}, Command=0x[{1}], Length={2}", device.DeviceType, BitConverter.ToString(command).Replace('-', ' '), command.Length);
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			lock (this._serial.SyncRoot)
			{
				this.LinkExternalDevice(device.X2Port, 17, device.BaudRate, device.Settings);
				this.SendExternalDeviceString(device.X2Port, 0, command);
			}
		}

		public byte[] ReadDirectAnswer(X2ExternalDevice device)
		{
			X2Controller._log.DebugFormat("ReadDirectAnswer: DeviceType={0}", device.DeviceType);
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (this._externalDeviceLinkDeviceType != 17)
			{
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "cannot read a direct answer since the external link is not in GENERIC mode. (device type={0})", new object[]
				{
					this._externalDeviceLinkDeviceType
				}));
			}
			byte[] result;
			lock (this._serial.SyncRoot)
			{
				result = this.ReadExternalDeviceString(device.X2Port);
			}
			return result;
		}

		public void SetRoomTemperatureCalibration(double roomTempCOld, double roomTempCNew)
		{
			this.SetTemperatureCalibration(roomTempCOld, roomTempCNew, 11);
		}

		public void SetCaseTemperatureCalibration(double caseTempCOld, double caseTempCNew)
		{
			this.SetTemperatureCalibration(caseTempCOld, caseTempCNew, 12);
		}

		private void SetTemperatureCalibration(double tempCOld, double tempCNew, byte calVal)
		{
			lock (this._serial.SyncRoot)
			{
				using (this.TemporarilyRaiseUserLevel(X2UserLevel.CalibrationAccess))
				{
					double num = (double)this.GetCalibrationValueSigned(calVal);
					double num2 = num + (tempCNew - tempCOld) * 100.0;
					if (Math.Abs(num2) >= 1000.0)
					{
						throw new ArgumentOutOfRangeException("tempCNew", "Correction too large for single point calibration");
					}
					this.SetCalibrationValue(calVal, (short)num2);
					base.SendMessage(13);
					base.CheckStatus(13);
				}
			}
		}

		public void SetPressureCalibration(double pressureHPaOld, double pressureHPaNew)
		{
			lock (this._serial.SyncRoot)
			{
				using (this.TemporarilyRaiseUserLevel(X2UserLevel.CalibrationAccess))
				{
					double num = (double)this.GetCalibrationValueUnsigned(18);
					double num2 = (double)this.GetCalibrationValueSigned(24);
					double num3 = (pressureHPaOld - num2) / num;
					double num4 = (pressureHPaNew - num2) / num3;
					this.SetCalibrationValue(18, (ushort)num4);
					base.SendMessage(13);
					base.CheckStatus(13);
				}
			}
		}

		public void SetPressureCapnostat(double pressureHPa)
		{
			CapnostatController capnostatController = (CapnostatController)IDeviceControllerFactory.CreateControllerForDevice(this.ChannelConfiguration.GetSerialDevice(ExternalDevices.Capnostat), this);
			lock (this._serial.SyncRoot)
			{
				capnostatController.SetPressure(pressureHPa);
			}
		}

		public X2FlowCalibration GetFlowCalibration()
		{
			X2FlowCalibration result;
			lock (this._serial.SyncRoot)
			{
				X2FlowCalibration x2FlowCalibration = new X2FlowCalibration();
				byte b = 0;
				while (b < 64)
				{
					byte b2 = b;
					switch (b2)
					{
					case 11:
					case 12:
					case 13:
						goto IL_46;
					default:
						if (b2 == 24 || b2 == 27)
						{
							goto IL_46;
						}
						x2FlowCalibration.Values[(int)b] = (int)this.GetCalibrationValueUnsigned(b);
						break;
					}
					IL_66:
					b += 1;
					continue;
					IL_46:
					x2FlowCalibration.Values[(int)b] = (int)this.GetCalibrationValueSigned(b);
					goto IL_66;
				}
				result = x2FlowCalibration;
			}
			return result;
		}

		public void SetFlowCalibration(X2FlowCalibration calib)
		{
			if (calib == null)
			{
				throw new ArgumentNullException("calib");
			}
			lock (this._serial.SyncRoot)
			{
				using (this.TemporarilyRaiseUserLevel(X2UserLevel.SuperUserAccess))
				{
					byte b = 0;
					while (b < 64)
					{
						byte b2 = b;
						if (b2 <= 18)
						{
							switch (b2)
							{
							case 11:
							case 12:
								break;
							case 13:
								goto IL_7D;
							default:
								if (b2 != 18)
								{
									goto IL_8F;
								}
								break;
							}
						}
						else
						{
							if (b2 == 24)
							{
								goto IL_7D;
							}
							switch (b2)
							{
							case 27:
								goto IL_7D;
							case 28:
							case 29:
								goto IL_8F;
							case 30:
							case 31:
								break;
							default:
								goto IL_8F;
							}
						}
						IL_9F:
						b += 1;
						continue;
						IL_7D:
						this.SetCalibrationValue(b, (short)calib.Values[(int)b]);
						goto IL_9F;
						IL_8F:
						this.SetCalibrationValue(b, (ushort)calib.Values[(int)b]);
						goto IL_9F;
					}
					base.SendMessage(13);
					base.CheckStatus(13);
				}
			}
			this.InitializeFlowResolution();
		}

		public double SetFlowBase()
		{
			double result;
			lock (this._serial.SyncRoot)
			{
				try
				{
					base.StandbyOff();
					Thread.Sleep(TimeSpan.FromSeconds(3.0));
					base.SendMessage(16);
					double num = (double)base.CheckStatus(16) * 0.02;
					X2Controller.FlowHeadCalRecord singleFlowHeadCalRecord = this.GetSingleFlowHeadCalRecord(27);
					using (this.TemporarilyRaiseUserLevel(X2UserLevel.CalibrationAccess))
					{
						this.SetCalibrationValue(27, singleFlowHeadCalRecord.EepromValue);
					}
					result = num;
				}
				catch
				{
					base.StandbyOn();
					throw;
				}
			}
			return result;
		}

		public ushort GetAmplitude()
		{
			if (this.IsMainASFirmwareV1x)
			{
				lock (this._serial.SyncRoot)
				{
					base.StandbyOff();
					base.SendMessage(215);
					base.ReadMessageOfSize(5);
					return X2Utils.UInt16(this._inbuf[1], this._inbuf[2]);
				}
			}
			X2Controller.AmplitudeAndPoti amplitudeAndPotiReal = this.GetAmplitudeAndPotiReal();
			return X2Utils.UInt16((byte)(amplitudeAndPotiReal.Amplitude1Absolute / 2), (byte)(amplitudeAndPotiReal.Amplitude2Absolute / 2));
		}

		private X2Controller.AmplitudeAndPoti GetAmplitudeAndPotiReal()
		{
			byte[] array = new byte[5];
			array[0] = 1;
			array[1] = 1;
			array[2] = 224;
			X2Messaging.AppendCrc(array, 3);
			X2Controller.AmplitudeAndPoti result;
			lock (this._serial.SyncRoot)
			{
				base.StandbyOff();
				this.SendExternalDeviceString(3, 0, array);
				byte[] array2 = this.ReadExternalDeviceString(3);
				result = new X2Controller.AmplitudeAndPoti
				{
					ErrorCode = array2[1],
					Amplitude1Absolute = X2Utils.UInt16(array2[2], array2[3]),
					Amplitude2Absolute = X2Utils.UInt16(array2[4], array2[5]),
					Poti1Absolute = X2Utils.UInt16(array2[6], array2[7]),
					Poti2Absolute = X2Utils.UInt16(array2[8], array2[9]),
					Amplitude1Deviation = (sbyte)array2[10],
					Amplitude2Deviation = (sbyte)array2[11],
					Poti1Relative = array2[12],
					Poti2Relative = array2[13]
				};
			}
			return result;
		}

		public void SetAmplitude(ushort voltage)
		{
            voltage = (ushort)(voltage * (this.IsMainASofTypeAsicA ? ((ushort) 1) : ((ushort) 2)));
			this.SetAmplitudeVoltage(voltage);
			this.SetAmplitude();
		}

		private void SetAmplitude()
		{
			lock (this._serial.SyncRoot)
			{
				base.StandbyOff();
				Thread.Sleep(TimeSpan.FromSeconds(1.0));
				using (this.TemporarilyRaiseUserLevel(X2UserLevel.CalibrationAccess))
				{
					base.SendMessage(204);
					Thread.Sleep(TimeSpan.FromSeconds(2.0));
					for (int i = 0; i < 2; i++)
					{
						try
						{
							ushort val = base.CheckStatus(204);
							X2Controller._log.DebugFormat("Set amplitude returned digital potentiometer values: {0} / {1}", X2Utils.High(val), X2Utils.Low(val));
							break;
						}
						catch (Exception ex)
						{
							if (i == 1)
							{
								X2Controller._log.Error("Failed to set amplitude level of Spiroson-AS sensor!", ex);
								throw;
							}
							X2Controller._log.WarnFormat("{0} in 'Get_Status' after 'Set_Amplitude': {1} --> RETRYING!", ex.GetType(), ex.Message);
						}
					}
					using (this.TemporarilyRaiseUserLevel(X2UserLevel.SuperUserAccess))
					{
						base.SendMessage(211, 64);
						base.CheckStatus(211);
						base.SendMessage(211, 65);
						base.CheckStatus(211);
					}
				}
			}
		}

		private void SetAmplitudeVoltage(ushort val)
		{
			lock (this._serial.SyncRoot)
			{
				using (this.TemporarilyRaiseUserLevel(X2UserLevel.SuperUserAccess))
				{
                    this.SetCalibrationValue(70, (ushort)(val / 10));
					base.SendMessage(13);
					base.CheckStatus(13);
				}
			}
		}

		private void FlowHeadCalSampling(X2Controller.FlowHeadCalSamplingCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			lock (this._serial.SyncRoot)
			{
				base.SendMessage(11);
				try
				{
					do
					{
						this._serial.Read(this._inbuf, 0, 4);
					}
					while (cb(new X2Controller.FlowHeadCalRecord
					{
						Index = this._inbuf[1],
						EepromValue = X2Utils.UInt16(this._inbuf[2], this._inbuf[3]),
						FixcalValue = this._inbuf[0]
					}));
				}
				finally
				{
					base.StopSampling();
				}
			}
		}

		private X2Controller.FlowHeadCalRecord GetSingleFlowHeadCalRecord(byte idx)
		{
			if (idx < 0 || idx > 63)
			{
				throw new ArgumentException("invalid record index to look for", "idx");
			}
			bool flag = false;
			X2Controller.FlowHeadCalRecord value;
			try
			{
				object syncRoot;
				Monitor.Enter(syncRoot = this._serial.SyncRoot, ref flag);
				X2Controller.FlowHeadCalRecord? result = null;
				this.FlowHeadCalSampling(delegate(X2Controller.FlowHeadCalRecord record)
				{
					if (record.Index != idx)
					{
						return true;
					}
					result = new X2Controller.FlowHeadCalRecord?(record);
					return false;
				});
				value = result.Value;
			}
			finally
			{
				if (flag)
				{
					object syncRoot;
                    syncRoot = this._serial.SyncRoot;
					Monitor.Exit(syncRoot);
				}
			}
			return value;
		}

		public void ActivateO2()
		{
			base.SendMessage(8);
		}

		public void DeactivateO2()
		{
			base.SendMessage(9);
		}

		public void OperateValve1Trigger(bool open)
		{
			this.OperateValve(open, 1);
		}
        //打开关闭电磁开关
		public void OperateValve2Selection(bool open)
		{
			this.OperateValve(this._reverseValve2Selection ? (!open) : open, 2);
		}

		public void OperateValve5Oxygen(bool open)
		{
			this.OperateValve(open, 5);
			this.OxygenValveStatus = (open ? X2ValveStatus.Opened : X2ValveStatus.Closed);
		}

		public void OperateValve7Air(bool open)
		{
			this.OperateValve(open, 7);
		}

		public WashoutRecord WashoutMeasurement()
		{
			int nrSamples = 10;
			WashoutRecord washoutRec = default(WashoutRecord);
			this.WashoutSampling(delegate(IList<WashoutRecord> lst)
			{
				foreach (WashoutRecord current in lst)
				{
					washoutRec = current;
					nrSamples -= lst.Count;
				}
				return nrSamples > 0;
			}, null);
			return washoutRec;
		}
        //washout采样函数
		private void WashoutSampling(Func<IList<WashoutRecord>, bool> cb, Func<X2ProtocolException, bool> dataStreamErrorHandler)
		{
			this.WashoutSampling(null, 200, 4.0, cb, dataStreamErrorHandler);
		}

		internal void WashoutSampling(double? ambientPressureHPa, int samplingFrequency, double callbackFrequency, Func<IList<WashoutRecord>, bool> cb, Func<X2ProtocolException, bool> dataStreamErrorHandler)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
            //优先级
			Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            //压缩比
			byte compressionRate = X2Utils.CompressionRate(200, samplingFrequency);
            //队列长度
			int cbBufferLength = X2Controller.CalcCallbackBufferLength((double)samplingFrequency, callbackFrequency);
			lock (this._serial.SyncRoot)
			{
				this.OperateValve2Selection(false);
				try
				{
					X2DataStreamReader x2DataStreamReader = new X2DataStreamReader(this.X2Port);
					x2DataStreamReader.SetSpecialErrorHandler(dataStreamErrorHandler);
					bool rv = true;
					float o2 = -1f;
					float co2 = -1f;
					float mmss = -1f;
					float mmms = -1f;
					float sampleFlow = -1f;
					Queue<WashoutRecord> buffer = new Queue<WashoutRecord>();
					base.SendMessage(1);
					base.CheckStatus(1);
                    //通道号
					List<X2ChannelNumber> list = new List<X2ChannelNumber>
					{
						X2ChannelNumber.Flow
					};
                    //通道标志
					X2ChannelFlags x2ChannelFlags = X2ChannelFlags.Flow;
					foreach (X2Configuration.AnalogChannelConfig current in this._channelConfig.SampledChannels())
					{
						x2ChannelFlags |= X2Configuration.AnalogChannelFlag(current.ChannelNumber);
						list.Add(current.ChannelNumber);
					}
					foreach (X2ExternalDevice current2 in this._channelConfig.SamplingSerialDevices())
					{
                        //创建设备
						IDeviceController deviceController = IDeviceControllerFactory.CreateControllerForDevice(current2, this);
                        //设备准备
						deviceController.SetupDevice(ambientPressureHPa);
                        //连接外部设备
						this.LinkExternalDevice(current2.X2Port, (byte)current2.X2DeviceType, current2.BaudRate, current2.Settings);
						foreach (X2Configuration.AnalogChannelConfig current3 in this._channelConfig.SampledChannels(current2))
						{
                            //连接外部通道
							this.LinkExternalChannel(current2.GetExternalPort(current3.MeasurementType), current3.ChannelNumber);
						}
					}
					bool[] channelsSampled = new bool[this._channelConfig.SampledChannels().Count];
					int num = 0;
					foreach (X2Configuration.AnalogChannelConfig current4 in this._channelConfig.SampledChannels())
					{
						IExternalDevice device = current4.Device;
						int nr = num++;
						channelsSampled[nr] = false;
						Func<int, float> converter = device.GetConverter(current4.MeasurementType);
						X2DataStreamReader.ValueCallback cb2;
						switch (current4.MeasurementType)
						{
						case MeasurementType.SampleFlow:
							cb2 = delegate(int v)
							{
								sampleFlow = converter(v);
								channelsSampled[nr] = true;
							};
							break;
						case MeasurementType.NO:
							goto IL_376;
						case MeasurementType.O2:
							cb2 = delegate(int v)
							{
								o2 = converter(v);
								channelsSampled[nr] = true;
							};
							break;
						case MeasurementType.CO2:
							cb2 = delegate(int v)
							{
								co2 = converter(v);
								channelsSampled[nr] = true;
							};
							break;
						case MeasurementType.MMss:
							cb2 = delegate(int v)
							{
								mmss = converter(v);
								channelsSampled[nr] = true;
							};
							break;
						case MeasurementType.MMms:
							cb2 = delegate(int v)
							{
								mmms = converter(v);
								channelsSampled[nr] = true;
							};
							break;
						default:
							goto IL_376;//异常
						}
						x2DataStreamReader.SetValueCallback(current4.ChannelNumber, cb2);
						continue;
						IL_376:
						throw new ArgumentException(string.Format("unexpected Measurement ({0}), no ValueCallback exists", current4.MeasurementType), "measurement");
					}
					bool channelsReady = false;
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.Flow, delegate(int v)
					{
						if (!channelsReady)
						{
							channelsReady = channelsSampled.All((bool b) => b);
						}
						if (channelsReady)
						{
							buffer.Enqueue(new WashoutRecord
							{
								Flow = this.ConvertFlowSignal(v),
								SampleFlow = sampleFlow,
								O2 = o2,
								CO2 = co2,
								MMss = mmss,
								MMms = mmms
							});
						}
						if (buffer.Count >= cbBufferLength)
						{
							if (!rv)
							{
								return;
							}
							WashoutRecord[] arg = buffer.ToArray();
							buffer.Clear();
                            //执行回调函数
							rv &= cb(arg);
						}
					});
					this.StartSampling(compressionRate, list, x2ChannelFlags);
					while (rv)
					{
                        //读取传感器值，为true循环
						x2DataStreamReader.Run();
					}
				}
				finally
				{
					base.StandbyOn();
					this.DeactivatePump();
					foreach (X2ExternalDevice current5 in this._channelConfig.SamplingSerialDevices())
					{
						IDeviceController deviceController2 = IDeviceControllerFactory.CreateControllerForDevice(current5, this);
						deviceController2.StopDevice();
						this.UnlinkExternalDevice(current5.X2Port);
					}
				}
			}
		}

		private void StartSampling(byte compressionRate, List<X2ChannelNumber> sampledChannelsNumber, X2ChannelFlags sampledChannels)
		{
			X2Controller._log.DebugFormat("Sampled X2-Channels: flags=[{0}], numbers=[{1}]", sampledChannels, sampledChannelsNumber.ConvertAll<byte>((X2ChannelNumber n) => (byte)n).JoinToString(','));
			this.ActivatePump();
			base.StandbyOff();
			base.SetChannels(sampledChannels);
			base.ResetCompressionDivisionRates();
			foreach (X2ChannelNumber current in sampledChannelsNumber)
			{
				base.SetCompressionRate(current, compressionRate);
			}
			base.SendMessage(2);
			Thread.Sleep(this._startSamplingReadDelay);
		}

		public X2Controller.EnvironmentRecord EnvironmentMeasurement()
		{
            //最三采样三组数值
            int nrSamples = 3;
            //最多采样三次
            int num = 3;
            //初始值，无效
			X2Controller.EnvironmentRecord retRecord = new X2Controller.EnvironmentRecord
			{
				CaseTemp = -300.0,
				RoomTemp = -300.0,
				Pressure = -1.0
			};
			while (num > 0 && X2Controller.InvalidEnvironmentRecord(retRecord))//环境参数无效返回true
			{
				num--;
				try
				{
					this.EnvironmentSampling(delegate(IList<X2Controller.EnvironmentRecord> lst)//回调函数
					{
						foreach (X2Controller.EnvironmentRecord current in lst)
						{
                            //最后一组值
							retRecord = current;
						}
						nrSamples -= lst.Count;//至少获得两组数据nrSamples =nrSamples- lst.Count
                        return nrSamples > 0;
					});
				}
				catch (Exception ex)
				{
					if (num == 0)
					{
						throw ex;
					}
				}
			}
			if (X2Controller.InvalidEnvironmentRecord(retRecord))
			{
				throw new X2ProtocolException("invalid Environment Record");
			}
			return retRecord;
		}

		public void EnvironmentSampling(X2Controller.EnvironmentSamplingCallback cb)
		{
			this.EnvironmentSampling(200, 4, cb);
		}
        //通过X2板子测量室温，工作室温度，压力
		internal void EnvironmentSampling(int samplingFrequency, int callbackFrequency, X2Controller.EnvironmentSamplingCallback cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
            //压缩率
			byte rate = X2Utils.CompressionRate(200, samplingFrequency);
            //buffer长度，采样频率/回调频率
			int cbBufferLength = X2Controller.CalcCallbackBufferLength((double)samplingFrequency, (double)callbackFrequency);
			lock (this._serial.SyncRoot)
			{
				try
				{
					X2DataStreamReader x2DataStreamReader = new X2DataStreamReader(this.X2Port);
					bool rv = true;
					double caseTemp = 0.0;
					double roomTemp = 0.0;
					Queue<X2Controller.EnvironmentRecord> buffer = new Queue<X2Controller.EnvironmentRecord>();
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.CaseTemp, delegate(int v)
					{
                        //温度变换*0.02
                        caseTemp = (double)v * 0.02;
					});
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.RoomTemp, delegate(int v)
					{
                        //温度变换*0.02
                        roomTemp = (double)v * 0.02;
					});
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.Pressure, delegate(int v)
					{
						buffer.Enqueue(new X2Controller.EnvironmentRecord
						{
							CaseTemp = caseTemp,
							RoomTemp = roomTemp,
							Pressure = (double)v
						});
						if (buffer.Count >= cbBufferLength)
						{
							if (!rv)
							{
								return;
							}
							X2Controller.EnvironmentRecord[] er = buffer.ToArray();//
							buffer.Clear();
							rv &= cb(er);
						}
					});
                    //设置采样通道
					base.SetChannels(X2ChannelFlags.CaseTemp | X2ChannelFlags.RoomTemp | X2ChannelFlags.Pressure);
                    //清除通道压缩率
                    base.ResetCompressionDivisionRates();
                    //设置通道压缩率
					base.SetCompressionRate(X2ChannelNumber.Pressure, rate);
					base.SetCompressionRate(X2ChannelNumber.RoomTemp, rate);
					base.SetCompressionRate(X2ChannelNumber.CaseTemp, rate);
                    //停止
					base.StandbyOff();
                    //启动
                    base.SendMessage(10);
					while (rv)
					{
                        //获取数据
						x2DataStreamReader.Run();
					}
				}
				finally
				{
                    //启动
					base.StandbyOn();
				}
			}
		}
        //环境参数是否有效，无效返回true
		private static bool InvalidEnvironmentRecord(X2Controller.EnvironmentRecord rec)
		{
			return rec.Pressure < 0.0 || rec.RoomTemp < -280.0 || rec.CaseTemp < -280.0;
		}

		public void FlowSampling(Func<IList<FlowRecord>, bool> cb)
		{
			this.FlowSampling(200, 4.0, cb);
		}

		internal void FlowSampling(int samplingFrequency, double callbackFrequency, Func<IList<FlowRecord>, bool> cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			byte rate = X2Utils.CompressionRate(200, samplingFrequency);
			int cbBufferLen = X2Controller.CalcCallbackBufferLength((double)samplingFrequency, callbackFrequency);
			lock (this._serial.SyncRoot)
			{
				try
				{
					X2DataStreamReader x2DataStreamReader = new X2DataStreamReader(this.X2Port);
					bool rv = true;
					Queue<FlowRecord> buffer = new Queue<FlowRecord>();
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.Flow, delegate(int v)
					{
						buffer.Enqueue(new FlowRecord
						{
							Flow = (int)Math.Round((double)this.ConvertFlowSignal(v))
						});
						if (buffer.Count >= cbBufferLen)
						{
							if (!rv)
							{
								return;
							}
							FlowRecord[] arg = buffer.ToArray();
							buffer.Clear();
							rv &= cb(arg);
						}
					});
					base.SetChannels(X2ChannelFlags.Flow);
					base.ResetCompressionDivisionRates();
					base.SetCompressionRate(X2ChannelNumber.Flow, rate);
					base.SendMessage(2);
					Thread.Sleep(this._startSamplingReadDelay);
					while (rv)
					{
						x2DataStreamReader.Run();
					}
				}
				finally
				{
					base.StandbyOn();
				}
			}
		}
        //一样化氮测量处理现场
		public void FlowNOSampling(Func<IList<FlowNORecord>, bool> cb)
		{
			this.FlowNOSampling(200, 4.0, cb);
		}

		internal void FlowNOSampling(int samplingFrequency, double callbackFrequency, Func<IList<FlowNORecord>, bool> cb)
		{
			if (cb == null)
			{
				throw new ArgumentNullException("cb");
			}
			byte rate = X2Utils.CompressionRate(200, samplingFrequency);
			int cbBufferLen = X2Controller.CalcCallbackBufferLength((double)samplingFrequency, callbackFrequency);
			lock (this._serial.SyncRoot)
			{
				this.OperateValve2Selection(true);//打开电磁开关
				try
				{
					X2DataStreamReader x2DataStreamReader = new X2DataStreamReader(this.X2Port);
					bool rv = true;
					int noraw = 0;
					Queue<FlowNORecord> buffer = new Queue<FlowNORecord>();
                    //设置读取线程的回调函数
					x2DataStreamReader.SetValueCallback(X2ChannelNumber.Analog0, delegate(int v)
					{
						noraw = v;
					});
                    //设置读取线程的回调函数
                    x2DataStreamReader.SetValueCallback(X2ChannelNumber.Flow, delegate(int v)
					{
						buffer.Enqueue(new FlowNORecord
						{
							Flow = (int)Math.Round((double)this.ConvertFlowSignal(v)),
							//NORaw = 0
                           NORaw = noraw
                        });
						if (buffer.Count >= cbBufferLen)//执行数据出来回调函数
						{
							if (!rv)
							{
								return;
							}
							FlowNORecord[] arg = buffer.ToArray();
							buffer.Clear();
							rv &= cb(arg);
						}
					});
					this.LinkExternalDevice(2, 7, 9600, 2);
					this.LinkExternalChannel(16, X2ChannelNumber.Analog0);
					this.SendExternalDeviceCommand(130, 255, 1);
					base.SetChannels(X2ChannelFlags.Analog0 | X2ChannelFlags.Flow);
					base.ResetCompressionDivisionRates();
					base.SetCompressionRate(X2ChannelNumber.Flow, rate);
					base.SetCompressionRate(X2ChannelNumber.Analog0, rate);
					base.SendMessage(2);
					Thread.Sleep(this._startSamplingReadDelay);
					while (rv)
					{
						x2DataStreamReader.Run();
					}
				}
				finally
				{
					base.StandbyOn();
					this.SendExternalDeviceCommand(130, 255, 0);
					this.UnlinkExternalDevice(2);
				}
			}
		}

		public void ActivatePump()
		{
			lock (this._serial.SyncRoot)
			{
				base.SendMessage(37);
				base.CheckStatus(37);
			}
		}

		public void DeactivatePump()
		{
			lock (this._serial.SyncRoot)
			{
				base.SendMessage(38);
				base.CheckStatus(38);
			}
		}

		public void DbgRestart()
		{
			lock (this._serial.SyncRoot)
			{
				base.StandbyOn();
				this.SetUserLevel(X2UserLevel.SuperUserAccess);
				base.SendMessage(200, 101, 31, 61);
			}
		}

		public void DbgTestAnalogChannel()
		{
			X2DataStreamReader x2DataStreamReader = new X2DataStreamReader(this.X2Port);
			bool rv = true;
			int count = 1;
			try
			{
				base.StandbyOff();
				base.SetChannels(X2ChannelFlags.Analog0 | X2ChannelFlags.Flow);
				x2DataStreamReader.SetValueCallback(X2ChannelNumber.Analog0, delegate(int v)
				{
				});
				x2DataStreamReader.SetValueCallback(X2ChannelNumber.Flow, delegate(int v)
				{
					count++;
					if (count > 2000)
					{
						rv = false;
					}
				});
				base.SendMessage(2);
				Thread.Sleep(200);
				while (rv)
				{
					x2DataStreamReader.Run();
				}
			}
			finally
			{
				base.StopSampling();
			}
		}

		private void LockedExecution(Action action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			lock (this._serial.SyncRoot)
			{
				action();
			}
		}

		private void LinkExternalDevice(byte portId, byte deviceType, int baudRate, byte settings)
		{
			X2Controller._log.DebugFormat("LinkExternalDevice: Port={0}, DeviceType={1}, BaudRate={2}, Settings={3}", new object[]
			{
				portId,
				deviceType,
				baudRate,
				settings
			});
			ushort num = 0;
			for (int i = 0; i < 5; i++)
			{
				base.SendMessage(31, portId, deviceType, new byte[]
				{
					X2Utils.X2BaudRateRepresentation(baudRate),
					settings
				});
				num = base.CheckStatus(31);
				if (num == 0)
				{
					this._externalDeviceLinkDeviceType = (int)deviceType;
					return;
				}
			}
			throw new X2ProtocolException(string.Format("could not link external device. Return value not 0 but {0}", num));
		}

		private void UnlinkExternalDevice(byte portId)
		{
			X2Controller._log.DebugFormat("UnlinkExternalDevice: Port={0}", portId);
			base.SendMessage(32, portId);
			base.CheckStatus(32);
			this._externalDeviceLinkDeviceType = -1;
		}

		private void SendExternalDeviceCommand(byte portId, byte command, byte handshake)
		{
			X2Controller._log.DebugFormat("SendExternalDeviceCommand: Port={0}, Command={1}, Handshake={2}", portId, command, handshake);
			base.SendMessage(29, portId, command, handshake);
			base.CheckStatus(29);
		}

		private void SendExternalDeviceString(byte portId, byte handshake, byte[] data)
		{
			X2Controller._log.DebugFormat("SendExternalDeviceString: Port={0}, Handshake={1}, Data=0x[{2}]", portId, handshake, BitConverter.ToString(data).Replace('-', ' '));
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			int i = 0;
			byte[] array = null;
			while (i < data.Length)
			{
				int num = Math.Min(15, data.Length - i);
				if (array == null || array.Length != num)
				{
					array = new byte[num];
				}
				Array.Copy(data, i, array, 0, num);
				base.SendMessage(51, portId, handshake, array);
				base.CheckStatus(51);
				Thread.Sleep(this.SendExternalDeviceStringThrottleDelay);
				i += num;
			}
		}

		private byte[] ReadExternalDeviceString(byte portId)
		{
			X2Controller._log.DebugFormat("ReadExternalDeviceString: Port={0}", portId);
			base.SendMessage(54, portId);
			base.ReadMessage();
			byte[] array = new byte[(int)(this._inbuf[0] - 3)];
			Array.Copy(this._inbuf, 1, array, 0, array.Length);
			return array;
		}

		private void LinkExternalChannel(byte externalChannelId, X2ChannelNumber internalChannelId)
		{
			X2Controller._log.DebugFormat("LinkExternalChannel: ExternalChannel={0}, InternalChannel={1}", externalChannelId, internalChannelId);
			base.SendMessage(27, externalChannelId, (byte)internalChannelId);
			base.CheckStatus(27);
		}

		private void UnlinkExternalChannel(byte portId)
		{
			base.SendMessage(28, portId);
			base.CheckStatus(28);
		}

		private ushort GetCalibrationValueUnsigned(byte id)
		{
			this._serial.PurgeInBuffer();
			base.SendMessage(219, id);
			base.ReadMessageOfSize(5);
			return X2Utils.UInt16(this._inbuf[1], this._inbuf[2]);
		}

		private short GetCalibrationValueSigned(byte id)
		{
			this._serial.PurgeInBuffer();
			base.SendMessage(219, id);
			base.ReadMessageOfSize(5);
			return X2Utils.Int16(this._inbuf[1], this._inbuf[2]);
		}

		private void SetCalibrationValue(byte id, ushort val)
		{
			base.SendMessage(12, id, X2Utils.High(val), X2Utils.Low(val));
			base.CheckStatus(12);
		}

		private void SetCalibrationValue(byte id, short val)
		{
			base.SendMessage(12, id, X2Utils.High(val), X2Utils.Low(val));
			base.CheckStatus(12);
		}

		private void SetUserLevel(X2UserLevel lvl)
		{
			base.SendMessage(206, 153, 136, (byte)lvl);
			base.CheckStatus(206);
		}

		private void PeriodicInterruptTimer(byte portId, int timeIn5ms)
		{
            this.SendExternalDeviceCommand((byte)(portId & 128), 255, (byte)timeIn5ms);
		}

		private IDisposable TemporarilyRaiseUserLevel(X2UserLevel lvl)
		{
			this.SetUserLevel(lvl);
			return new CleanupWrapper(delegate
			{
				this.SetUserLevel(X2UserLevel.NormalAccess);
			});
		}
        //打开关闭电磁开关
		private void OperateValve(bool open, byte port)
		{
			if (port < 0 || port > 7)
			{
				throw new ArgumentException("Only 8 valve ports exist (0..7)", "port");
			}
			if (open)
			{
				base.SendMessage(49, port);
				return;
			}
			base.SendMessage(50, port);
		}

		private static int CalcCallbackBufferLength(double samplingFrequency, double callbackFrequency)
		{
			if (samplingFrequency <= 0.0)
			{
				throw new ArgumentException("negative or zero sampling frequency", "samplingFrequency");
			}
			if (callbackFrequency <= 0.0)
			{
				throw new ArgumentException("negative or zero callback frequency", "callbackFrequency");
			}
			return Math.Max(1, (int)(samplingFrequency / callbackFrequency + 0.5));
		}

		private static int FlowSignCorrection(int rawFlow, bool changeSign)
		{
			if (rawFlow >= 2048)
			{
				rawFlow -= 4096;
			}
			if (changeSign)
			{
				rawFlow = -rawFlow;
			}
			return rawFlow;
		}

		private float ConvertFlowSignal(int rawFlow)
		{
			if (this._flowResolution.Equals(0f))
			{
				throw new InvalidOperationException("_flowResolution has not been initialized");
			}
			return (float)X2Controller.FlowSignCorrection(rawFlow, this._invertFlow) * this._flowResolution;
		}

		private const int NativeSamplingFrequency = 200;

		private const int NativeCallbackFrequency = 4;

		private static readonly ILog _log = LogManager.GetLogger(typeof(X2Controller));

		private readonly TimeSpan _startSamplingReadDelay = TimeSpan.FromMilliseconds(1000.0);

		private TimeSpan _sendExternalDeviceStringThrottleDelay = TimeSpan.FromMilliseconds(150.0);

		private int _externalDeviceLinkDeviceType = -1;

		private bool _invertFlow;

		private float _flowResolution;

		private readonly X2Configuration _channelConfig = new X2Configuration();

		private X2Versions _asMainVersions;

		private bool _isInitialized;

		private readonly bool _reverseValve2Selection;

		private struct AmplitudeAndPoti
		{
			public byte ErrorCode;

			public ushort Amplitude1Absolute;

			public ushort Amplitude2Absolute;

			public ushort Poti1Absolute;

			public ushort Poti2Absolute;

			public sbyte Amplitude1Deviation;

			public sbyte Amplitude2Deviation;

			public byte Poti1Relative;

			public byte Poti2Relative;
		}

		private struct FlowHeadCalRecord
		{
			public byte Index;

			public ushort EepromValue;

			public byte FixcalValue;
		}

		private delegate bool FlowHeadCalSamplingCallback(X2Controller.FlowHeadCalRecord record);

		public struct EnvironmentRecord
		{
			public double CaseTemp;

			public double RoomTemp;

			public double Pressure;
		}

		public delegate bool EnvironmentSamplingCallback(IList<X2Controller.EnvironmentRecord> er);
	}
}
