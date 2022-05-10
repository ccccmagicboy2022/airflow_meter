using System;
using System.Globalization;
using System.Threading;
using BebeFlo.Sensors.Cld88Protocol;
using log4net;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2Messaging
	{
		protected X2Messaging(ISensorPort serial)
		{
			if (serial == null)
			{
				throw new ArgumentNullException("serial");
			}
			this._serial = serial;
		}
        //获得版本号信息
		public X2Versions GetVersion()
		{
			X2Messaging._log.DebugFormat("calling 'GetVersion()'...", new object[0]);
			X2Versions result;
			lock (this._serial.SyncRoot)
			{
				this._serial.PurgeInBuffer();
				this.SendMessage(1);
				this.SendMessage(202);
				this.ReadMessageOfSize(26);
				result = new X2Versions
				{
					BootLoader = new int[]
					{
						(int)this._inbuf[1],
						(int)this._inbuf[2],
						(int)this._inbuf[3],
						(int)this._inbuf[4]
					},
					Firmware = new int[]
					{
						(int)this._inbuf[5],
						(int)this._inbuf[6],
						(int)this._inbuf[7],
						(int)this._inbuf[8]
					},
					Hardware = (int)this._inbuf[9],
					ModuleInfo1 = (int)this._inbuf[10],
					ModuleInfo2 = (int)this._inbuf[11],
					SerialNo2 = (int)X2Utils.UInt16(this._inbuf[12], this._inbuf[13]),
					SerialNo1 = (int)X2Utils.UInt16(this._inbuf[14], this._inbuf[15]),
					FirmwareCrc = (int)X2Utils.UInt16(this._inbuf[16], this._inbuf[17]),
					SensorStatus = (int)this._inbuf[19]
				};
			}
			return result;
		}

		public X2DeviceStatus GetDeviceStatus()
		{
			X2Messaging._log.DebugFormat("calling 'GetDeviceStatus()'...", new object[0]);
			X2DeviceStatus result;
			lock (this._serial.SyncRoot)
			{
				this._serial.PurgeInBuffer();
				this.SendMessage(203);
				this.ReadMessageOfSize(7);
				X2DeviceStatus x2DeviceStatus = (X2DeviceStatus)(this._inbuf[2] & 127);
				result = x2DeviceStatus;
			}
			return result;
		}

		protected void StandbyOff()
		{
			this.SendMessage(18);
			this.CheckStatus(18);
		}

		protected void StandbyOn()
		{
			this.StopSampling();
			this.SendMessage(17);
			this.CheckStatus(17);
		}

		protected void StopSampling()
		{
			this.SendMessage(1);
			Thread.Sleep(this.StopSamplingConfirmDelay);
			this._serial.PurgeInBuffer();
			this.SendMessage(1);
			this.CheckStatus(1);
		}

		protected void SetChannels(X2ChannelFlags flags)
		{
            int num = (int)flags;
            this.SendMessage(3, (byte)(num >> 8), (byte)(num & 0xff));
            this.CheckStatus(3);
		}

		protected void ResetCompressionDivisionRates()
		{
			this.SendMessage(14);
			this.CheckStatus(14);
		}

		protected void SetCompressionRate(X2ChannelNumber ch, byte rate)
		{
			this.SendMessage(6, (byte)ch, rate);
			this.CheckStatus(6);
		}

		protected void SetDivisionRate(X2ChannelNumber ch, byte rate)
		{
			this.SendMessage(7, (byte)ch, rate);
			this.CheckStatus(7);
		}

		protected static void AppendCrc(byte[] buffer, int len)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (len < 1)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "zero or negative length not allowed ({0})", new object[]
				{
					len
				}), "len");
			}
			ushort num = X2Utils.Crc(buffer, len);
			buffer[len] = (byte)(num >> 8);
			buffer[len + 1] = (byte)(num & 255);
		}

		protected void SendMessage(byte cmd)
		{
			this._outbuf[0] = 1;
			this._outbuf[1] = 1;
			this._outbuf[2] = cmd;
			X2Messaging.AppendCrc(this._outbuf, 3);
			this._serial.Write(this._outbuf, 5);
		}

		protected void SendMessage(byte cmd, byte arg1)
		{
			this._outbuf[0] = 1;
			this._outbuf[1] = 2;
			this._outbuf[2] = cmd;
			this._outbuf[3] = arg1;
			X2Messaging.AppendCrc(this._outbuf, 4);
			this._serial.Write(this._outbuf, 6);
		}

		protected void SendMessage(byte cmd, byte arg1, byte arg2)
		{
			this._outbuf[0] = 1;
			this._outbuf[1] = 3;
			this._outbuf[2] = cmd;
			this._outbuf[3] = arg1;
			this._outbuf[4] = arg2;
			X2Messaging.AppendCrc(this._outbuf, 5);
			this._serial.Write(this._outbuf, 7);
		}

		protected void SendMessage(byte cmd, byte arg1, byte arg2, byte arg3)
		{
			this._outbuf[0] = 1;
			this._outbuf[1] = 4;
			this._outbuf[2] = cmd;
			this._outbuf[3] = arg1;
			this._outbuf[4] = arg2;
			this._outbuf[5] = arg3;
			X2Messaging.AppendCrc(this._outbuf, 6);
			this._serial.Write(this._outbuf, 8);
		}

		protected void SendMessage(byte cmd, byte arg1, byte arg2, params byte[] argMore)
		{
			if (argMore == null)
			{
				throw new ArgumentNullException("argMore");
			}
			this._outbuf[0] = 1;
			this._outbuf[1] = (byte)(3 + argMore.Length);
			this._outbuf[2] = cmd;
			this._outbuf[3] = arg1;
			this._outbuf[4] = arg2;
			Array.Copy(argMore, 0, this._outbuf, 5, argMore.Length);
			X2Messaging.AppendCrc(this._outbuf, 5 + argMore.Length);
			this._serial.Write(this._outbuf, 7 + argMore.Length);
		}

		protected void ReadMessage()
		{
			this._serial.Read(this._inbuf, 0, 1);
			int num = (int)this._inbuf[0];
			if (num < 3)
			{
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "unexpected message length value ({0})", new object[]
				{
					this._inbuf[0]
				}));
			}
			this._serial.Read(this._inbuf, 1, num - 1);
			int num2 = (int)X2Utils.Crc(this._inbuf, num - 2);
			int num3 = ((int)this._inbuf[num - 2] << 8) + (int)this._inbuf[num - 1];
			this._incomingMessagesCount++;
			if (num2 != num3)
			{
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "message crc error. (read=0x{0:x}, expected=0x{1:x})", new object[]
				{
					num3,
					num2
				}));
			}
		}

		protected void ReadMessageOfSize(int expectedMsgLen)
		{
			X2Messaging._log.DebugFormat("ReadMessageOfSize: Expected message length={0}", expectedMsgLen);
			if (expectedMsgLen < 3)
			{
				throw new ArgumentException("invalid parameter for expected message length", "expectedMsgLen");
			}
			this.ReadMessage();
			if (expectedMsgLen != (int)this._inbuf[0])
			{
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "unexpected message length (received={0}, expected={1}", new object[]
				{
					this._inbuf[0],
					expectedMsgLen
				}));
			}
		}

		protected ushort CheckStatus(byte lastCommand)
		{

            X2Messaging._log.DebugFormat("CheckStatus: Command={0} (0x{0:x}) ...", lastCommand);
			this._serial.PurgeInBuffer();
			this.SendMessage(203);
			this.ReadMessageOfSize(7);
			if (this._inbuf[1] != lastCommand)
			{
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "unexpected command id ack (received=0x{0:x}, expected=0x{1:x})", new object[]
				{
					this._inbuf[1],
					lastCommand
				}));
			}
			if (this._inbuf[2] != 0)
			{
				X2DeviceStatus x2DeviceStatus = (X2DeviceStatus)(this._inbuf[2] & 127);
				bool flag = (this._inbuf[2] & 128) != 0;
				throw new X2ProtocolException(string.Format(CultureInfo.InvariantCulture, "device status error (error={0}, isSensorError={1})", new object[]
				{
					x2DeviceStatus,
					flag
				}))
				{
					DeviceErrorStatus = x2DeviceStatus
				};
			}
			ushort num = X2Utils.UInt16(this._inbuf[3], this._inbuf[4]);
			X2Messaging._log.DebugFormat("CheckStatus: Command={0} ... returned Status={1}", lastCommand, num);
			return num;

        }

		private static readonly ILog _log = LogManager.GetLogger(typeof(X2Messaging));

		public TimeSpan StopSamplingConfirmDelay = TimeSpan.FromMilliseconds(300.0);

		protected ISensorPort _serial;

		protected byte[] _outbuf = new byte[100];

		protected byte[] _inbuf = new byte[300];

		protected int _incomingMessagesCount;
	}
}
