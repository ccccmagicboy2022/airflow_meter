using System;
using System.Globalization;
using System.Threading;
using BebeFlo.Sensors.Cld88Protocol;

namespace BebeFlo.Sensors.X2Protocol
{
    //传感器到X2板包装
	public class SensorToX2Wrapper : ISensorPort
	{
		public SensorToX2Wrapper(IX2ControllerDirectCommands x2, int readTimeoutMilliseconds) : this(SensorToX2Wrapper.CldExtDeviceConfig, x2, readTimeoutMilliseconds)
		{
		}

		public SensorToX2Wrapper(X2ExternalDevice device, IX2ControllerDirectCommands x2, int readTimeoutMilliseconds)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (x2 == null)
			{
				throw new ArgumentNullException("x2");
			}
			if (readTimeoutMilliseconds < 0)
			{
				throw new ArgumentException("negative read timeout", "readTimeoutMilliseconds");
			}
			this._device = device;
			this._x2 = x2;
			this._readTimeoutMilliseconds = readTimeoutMilliseconds;
		}

		public object SyncRoot
		{
			get
			{
				return this._x2.SyncRoot;
			}
		}

		public void Write(byte[] buffer, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0 || count > buffer.Length)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "number of bytes to send is negative or bigger than the buffer size. (count={0}, bufLen={1})", new object[]
				{
					count,
					buffer.Length
				}), "count");
			}
			if (count == 0)
			{
				return;
			}
			byte[] array = new byte[count];
			Array.Copy(buffer, array, count);
			lock (this._syncRoot)
			{
				this._x2.SendDirectCommand(this._device, array);
				this._reply = SensorToX2Wrapper.ZeroReply;
				this._replyReturned = 0;
			}
		}

		public void Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset >= buffer.Length)
			{
				throw new ArgumentException("offset negative of larger than buffer size", "offset");
			}
			if (count < 0 || offset + count >= buffer.Length)
			{
				throw new ArgumentException("count value negative or in combination with offset larger than the buffer size.", "count");
			}
			if (count == 0)
			{
				return;
			}
			lock (this._syncRoot)
			{
				DateTime now = DateTime.Now;
				while (this._reply.Length < this._replyReturned + count)
				{
					if (this._readTimeoutMilliseconds != 0 && (DateTime.Now - now).TotalMilliseconds > (double)this._readTimeoutMilliseconds)
					{
						throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "(buffer content={0} bytes)", new object[]
						{
							this._reply.Length
						}));
					}
					this._reply = this.ReadX2Buffer();
				}
				Array.Copy(this._reply, this._replyReturned, buffer, offset, count);
				this._replyReturned += count;
			}
		}

		public byte[] ReadX2Buffer()
		{
			byte[] result;
			lock (this._syncRoot)
			{
				Thread.Sleep(50);
				result = this._x2.ReadDirectAnswer(this._device);
			}
			return result;
		}

		public void PurgeInBuffer()
		{
			lock (this._syncRoot)
			{
				this._reply = SensorToX2Wrapper.ZeroReply;
				this._replyReturned = 0;
			}
		}

		private const int ReadPollIntervalMilliseconds = 50;

		private static readonly byte[] ZeroReply = new byte[0];

		private static X2ExternalDevice CldExtDeviceConfig = new X2ExternalDevice
		{
			X2Port = 2,
			BaudRate = 9600,
			Settings = 2
		};

		private readonly object _syncRoot = new object();

		private readonly IX2ControllerDirectCommands _x2;

		private readonly int _readTimeoutMilliseconds;

		private readonly X2ExternalDevice _device;

		private byte[] _reply = SensorToX2Wrapper.ZeroReply;

		private int _replyReturned;
	}
}
