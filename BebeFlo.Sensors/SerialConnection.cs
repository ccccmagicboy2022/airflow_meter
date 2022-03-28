using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using BebeFlo.Sensors.Cld88Protocol;
using BebeFlo.Sensors.X2Protocol;
using log4net;

namespace BebeFlo.Sensors
{
	public class SerialConnection : IDisposable, IX2Port, ISensorPort
	{
		public string PortName
		{
			get
			{
				if (this._port == null)
				{
					return "";
				}
				return this._port.PortName;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this._port != null && this._port.IsOpen;
			}
		}

		public SerialConnection()
		{
		}

		public SerialConnection(string portName, int readTimeoutMilliseconds)
		{
			if (portName == null)
			{
				throw new ArgumentNullException("portName");
			}
			this.ResetPort(portName, readTimeoutMilliseconds);
		}

		~SerialConnection()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			this.Close();
			if (this._port != null)
			{
				this._port.Dispose();
			}
			this._port = null;
		}

		public object SyncRoot
		{
			get
			{
				return this._syncRoot;
			}
		}

		public void Write(byte[] buffer, int count)
		{
			SerialConnection._log.DebugFormat("WRITE: Data=0x[{0}], Length={1}", BitConverter.ToString(buffer, 0, count).Replace('-', ' '), count);
			this._port.Write(buffer, 0, count);
		}

		public void Read(byte[] buffer, int offset, int count)
		{
			SerialConnection._log.DebugFormat("calling 'Read()' for number of bytes={0} ...", count);
			int num = offset;
			while (count > 0)
			{
				int num2 = this._port.Read(buffer, num, count);
				SerialConnection._log.DebugFormat("READ : Data=0x[{0}], Length={1}", BitConverter.ToString(buffer, num, num2).Replace('-', ' '), num2);
				num += num2;
				count -= num2;
			}
		}

		public void Close()
		{
			if (this._port != null)
			{
				this._port.Close();
			}
		}

		public void Open()
		{
			this._port.Open();
		}

		public void ResetPort(string portName, int readTimeoutMilliseconds)
		{
			if (portName == null)
			{
				throw new ArgumentNullException("portName");
			}
			this.Close();
			if (this._port != null)
			{
				this._port.Dispose();
			}
			this._port = new SerialConnection.ExSerialPort(portName);
			this._port.ReadTimeout = readTimeoutMilliseconds;
		}

		public void PurgeInBuffer()
		{
			this._port.DiscardInBuffer();
		}

		public void ReconfigureForX2()
		{
			this.Close();
			this._port.BaudRate = 115200;
			this._port.DataBits = 8;
			this._port.DtrEnable = false;
			this._port.RtsEnable = false;
			this._port.Open();
		}

		public void ReconfigureForCld88()
		{
			this.Close();
			this._port.BaudRate = 9600;
			this._port.DataBits = 7;
			this._port.DtrEnable = true;
			this._port.RtsEnable = true;
			this._port.Open();
		}

		public void ReconfigureForExternalDevice(int baudRate, int dataBits)
		{
			this.Close();
			this._port.BaudRate = baudRate;
			this._port.DataBits = dataBits;
			this._port.DtrEnable = true;
			this._port.RtsEnable = true;
			this._port.Open();
		}

		private static readonly ILog _log = LogManager.GetLogger(typeof(SerialConnection));

		private readonly object _syncRoot = new object();

		private SerialConnection.ExSerialPort _port;

		private class ExSerialPort : SerialPort
		{
			public ExSerialPort(string name) : base(name)
			{
			}

			protected override void Dispose(bool disposing)
			{
				Stream stream = (Stream)typeof(SerialPort).GetField("internalSerialStream", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
				if (stream != null)
				{
					try
					{
						stream.Dispose();
					}
					catch
					{
					}
				}
				base.Dispose(disposing);
			}
		}
	}
}
