using System;
using System.Globalization;
using System.Text;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldController
	{
		public CldController(ISensorPort serial)
		{
			if (serial == null)
			{
				throw new ArgumentNullException("serial");
			}
			this._serial = serial;
		}

		public string GetVersion()
		{
			return this.LockedExecution<string>(delegate
			{
				this.SendMessage("RV");
				return this.ReadReply();
			});
		}

		public CldStatus GetStatus()
		{
			string text = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RS");
				return this.ReadReplyWithPayloadSize(25);
			});
			return new CldStatus
			{
				OperationMode = (CldOperationMode)(text[11] & '\u001c'),
				InRemoteMode = CldUtils.ParseBit(text[11], 0),
				ComponentStatus = (CldComponentState)((text[8] & '?') | (text[9] & '?') << 8),
				ErrorStatus = (CldInstrumentError)CldUtils.ParseHexString(text, 13, 4),
				WarningStatus = (CldInstrumentWarning)CldUtils.ParseHexString(text, 18, 4)
			};
		}

		public double GetNO()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RD1");
				return this.ReadReplyWithPayloadSize(5);
			});
			return CldUtils.ParseFloatOrStar(value, -1.0);
		}

		public CldCounts GetCounts()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("TD");
				return this.ReadReplyWithPayloadSize(13);
			});
			return new CldCounts
			{
				ChannelB = CldUtils.ParseFloatOrStar(value, 0, 6, -1.0),
				ChannelA = CldUtils.ParseFloatOrStar(value, 7, 6, -1.0)
			};
		}

		public CldOperatingTime GetOperatingHours()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RZ");
				return this.ReadReplyWithPayloadSize(17);
			});
			return new CldOperatingTime
			{
				VacuumPumpHours = CldUtils.ParseInt(value, 0, 5),
				AnalyzerHours = CldUtils.ParseInt(value, 6, 5),
				ConverterHours = CldUtils.ParseInt(value, 12, 5)
			};
		}

		public CldPressures GetCurrentPressures()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RP");
				return this.ReadReply();
			});
			string[] parts = CldUtils.SplitParts(value, new int[]
			{
				6
			});
			Func<int, int> func = (int i) => CldUtils.ParseIntOrStar(parts[i], -1);
			return new CldPressures
			{
				BypassRegulation = func(0),
				ReactionChamber = func(1),
				ZeroCalibrationGas = func(2),
				SpanCalibrationGas = func(3),
				ReactionChamberWithClosedInledValve = func(4),
				TubeType = func(5)
			};
		}

		public CldTemperatures GetCurrentTemperatures()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RT");
				return this.ReadReply();
			});
			string[] parts = CldUtils.SplitParts(value, new int[]
			{
				8
			});
			Func<int, double> func = (int i) => CldUtils.ParseFloatOrStar(parts[i], double.NaN);
			return new CldTemperatures
			{
				InstrumentInternal = func(0),
				PeltierCooling = func(1),
				ReactionChamber = func(2),
				Converter = func(3),
				HotTubing = func(4),
				OzoneDestroyer = func(5),
				OzoneGenerator = func(6),
				VacuumPump = func(7)
			};
		}

		public CldFilterConfig GetFilterConfig()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RI");
				return this.ReadReply();
			});
			string[] array = CldUtils.SplitParts(value, new int[]
			{
				4,
				5
			});
			return new CldFilterConfig
			{
				FilterTimeSlow = CldUtils.ParseFloat(array[0]),
				FilterTimeMedium = CldUtils.ParseFloat(array[1]),
				FilterTimeFast = CldUtils.ParseFloat(array[2]),
				ChannelB = (CldFilterStatus)CldUtils.ParseInt(array[3]),
				ChannelA = (CldFilterStatus)((array.Length == 5) ? CldUtils.ParseInt(array[4]) : 3)
			};
		}

		public CldCalibration GetCalibration()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RC1,1");
				return this.ReadReplyWithPayloadSize(11);
			});
			return new CldCalibration
			{
				Zero = CldUtils.ParseFloat(value, 0, 5),
				Slope = CldUtils.ParseFloat(value, 6, 5)
			};
		}

		public double GetCalibrationGasConcentration(bool NOx)
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RB{0}", new object[]
				{
					NOx ? 1 : 0
				});
				return this.ReadReplyWithPayloadSize(7);
			});
			return CldUtils.ParseFloat(value, 2, 5);
		}

		public double GetSampleGasFlowMlMin()
		{
			string value = this.LockedExecution<string>(delegate
			{
				this.SendMessage("RF");
				return this.ReadReplyWithPayloadSize(4);
			});
			return CldUtils.ParseFloat(value);
		}

		public void InitializeIfNecessary()
		{
			if (!this._isInitialized)
			{
				this.SetRemoteMode(false);
				this.SetStandby(true);
				this._isInitialized = true;
			}
		}

		public void SetRemoteMode(bool on)
		{
			this.LockedExecution(delegate
			{
				this.SendMessage("HR{0}", new object[]
				{
					on ? "1" : "0"
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void SetStandby(bool on)
		{
			this.LockedExecution(delegate
			{
				this.SendMessage("SS{0}", new object[]
				{
					on ? "1" : "0"
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void StartZeroPointCalibration(int durationSec)
		{
			if (durationSec < 45 || 999 < durationSec)
			{
				throw new ArgumentOutOfRangeException("durationSec");
			}
			this.LockedExecution(delegate
			{
				this.SendMessage("CP0,{0}", new object[]
				{
					durationSec.ToString("000")
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void StartSpanCalibration(int durationSec)
		{
			if (durationSec < 60 || 999 < durationSec)
			{
				throw new ArgumentOutOfRangeException("durationSec");
			}
			this.LockedExecution(delegate
			{
				this.SendMessage("CA{0}", new object[]
				{
					durationSec.ToString("000")
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void SetMeasurementRange(int range)
		{
			if (range < 5 || 5000 < range)
			{
				throw new ArgumentOutOfRangeException("range");
			}
			this.LockedExecution(delegate
			{
				this.SendMessage(string.Format("SX0,{0}", range.ToString("0:00000")));
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void SetFilterConfig(CldFilterConfig config)
		{
			this.LockedExecution(delegate
			{
				this.SendMessage("SI{0},{1}", new object[]
				{
					(int)config.ChannelB,
					(int)config.ChannelA
				});
				this.ReadReplyWithPayloadSize(0);
				double num = Math.Min(Math.Min(config.FilterTimeFast, config.FilterTimeMedium), config.FilterTimeSlow);
				double num2 = Math.Max(Math.Max(config.FilterTimeFast, config.FilterTimeMedium), config.FilterTimeSlow);
				if (num < 0.1 || 180.0 < num2)
				{
					throw new ArgumentOutOfRangeException("config", "FilterTimes out of range");
				}
				this.SendMessage("SF{0},{1}", new object[]
				{
					0,
					CldUtils.GetNDigitFloatString(config.FilterTimeSlow, 5)
				});
				this.ReadReplyWithPayloadSize(0);
				this.SendMessage("SF{0},{1}", new object[]
				{
					1,
					CldUtils.GetNDigitFloatString(config.FilterTimeMedium, 5)
				});
				this.ReadReplyWithPayloadSize(0);
				this.SendMessage("SF{0},{1}", new object[]
				{
					2,
					CldUtils.GetNDigitFloatString(config.FilterTimeFast, 5)
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void SetCalibrationGasConcentration(bool NOx, double concentrationPpm)
		{
			this.LockedExecution(delegate
			{
				this.SendMessage("SB{0},{1}", new object[]
				{
					NOx ? 1 : 0,
					CldUtils.GetNDigitFloatString(concentrationPpm, 5)
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void StopCalibration(bool saveValues)
		{
			this.LockedExecution(delegate
			{
				this.SendMessage("CE{0}", new object[]
				{
					saveValues ? "1" : "0"
				});
				this.ReadReplyWithPayloadSize(0);
			});
		}

		public void SetCalibration(CldCalibration calibration)
		{
			this.LockedExecution(delegate
			{
				bool inRemoteMode = this.GetStatus().InRemoteMode;
				if (!inRemoteMode)
				{
					this.SetRemoteMode(true);
				}
				for (int i = 0; i < 4; i++)
				{
					this.SendMessage("SC1,{0},{1},{2}", new object[]
					{
						i,
						CldUtils.GetNDigitFloatString(calibration.Zero, 5),
						CldUtils.GetNDigitFloatString(calibration.Slope, 5)
					});
					this.ReadReplyWithPayloadSize(0);
				}
				if (!inRemoteMode)
				{
					this.SetRemoteMode(false);
				}
			});
		}

		private static void AppendBcc(byte[] buffer, int len)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			buffer[len] = CldUtils.BlockCheckCode(buffer, len);
		}

		private void SendMessage(string payload)
		{
			if (payload == null)
			{
				throw new ArgumentNullException("payload");
			}
			int count = 0;
			this._outbuf[count++] = 2;
			this._outbuf[count++] = 48;
			this._outbuf[count++] = 49;
			for (int i = 0; i < payload.Length; i++)
			{
				this._outbuf[count++] = (byte)payload[i];
			}
			this._outbuf[count++] = 3;
			CldController.AppendBcc(this._outbuf, count++);
			this._serial.Write(this._outbuf, count);
		}

		private void SendMessage(string format, params object[] args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			this.SendMessage(string.Format(format, args));
		}

		private string ReadReply()
		{
			this._serial.Read(this._inbuf, 0, 2);
			byte b = CldUtils.BlockCheckCode(this._inbuf, 2);
			if (this._inbuf[0] == 21)
			{
				throw new CldProtocolException(string.Format(CultureInfo.InvariantCulture, "protocol error: device returned code '{0}'.", new object[]
				{
					(CldReturnCode)(this._inbuf[1] & 15)
				}));
			}
			if (this._inbuf[0] != 6)
			{
				throw new CldProtocolException(string.Format(CultureInfo.InvariantCulture, "protocol error: reply does not start with ACK or NAK. (received 0x{0:x2}')", new object[]
				{
					this._inbuf[0]
				}));
			}
			int num = 0;
			this._serial.Read(this._inbuf, num++, 1);
			if (this._inbuf[0] == 3)
			{
				return "";
			}
			if (this._inbuf[0] != 2)
			{
				throw new CldProtocolException(string.Format(CultureInfo.InvariantCulture, "protocol error: message payload does not start with STX. (received 0x{0:x2})", new object[]
				{
					this._inbuf[0]
				}));
			}
			do
			{
				this._serial.Read(this._inbuf, num++, 1);
			}
			while (this._inbuf[num - 1] != 3);
			int num2 = (int)(b ^ CldUtils.BlockCheckCode(this._inbuf, num));
			this._serial.Read(this._inbuf, num++, 1);
			if (num2 != (int)this._inbuf[num - 1])
			{
				throw new CldProtocolException("protocol error: checksum error for payload of received message");
			}
			return Encoding.ASCII.GetString(this._inbuf, 1, num - 3);
		}

		private string ReadReplyWithPayloadSize(int expectedPayloadSize)
		{
			string text = this.ReadReply();
			if (text.Length != expectedPayloadSize)
			{
				throw new CldProtocolException(string.Format(CultureInfo.InvariantCulture, "protocol error: received reply has not the expected payload size. (expected={0}, read={1})", new object[]
				{
					expectedPayloadSize,
					text.Length
				}));
			}
			return text;
		}

		private T LockedExecution<T>(Func<T> fun)
		{
			T result;
			lock (this._serial.SyncRoot)
			{
				result = fun();
			}
			return result;
		}

		private void LockedExecution(Action action)
		{
			lock (this._serial.SyncRoot)
			{
				action();
			}
		}

		public const byte STX = 2;

		public const byte ETX = 3;

		public const byte ACK = 6;

		public const byte NAK = 21;

		public const byte RelevantDataBits = 63;

		private ISensorPort _serial;

		private byte[] _outbuf = new byte[40];

		private byte[] _inbuf = new byte[100];

		private bool _isInitialized;

		public delegate bool CountsNOSamplingCallback(CldCounts counts, double no, CldPressures pressures, bool cldInCalibrationMode);
	}
}
