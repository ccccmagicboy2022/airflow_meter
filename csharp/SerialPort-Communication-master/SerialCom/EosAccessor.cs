using System;
using System.IO.Ports;


namespace SerialCom
{
	public class EosAccessor 
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public bool IsOpen
		{
			get
			{
				return this._SerialPort.IsOpen;
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205D File Offset: 0x0000025D
		public EosAccessor(SerialPort serialPort)
		{
			this._SerialPort = serialPort;
		}

	

		// Token: 0x06000006 RID: 6 RVA: 0x0000215D File Offset: 0x0000035D
		public void WriteData(byte[] data)
		{
			if (!this._SerialPort.IsOpen)
			{
				throw new InvalidOperationException("Port must be open to write data.");
			}
			this._SerialPort.Write(data, 0, data.Length);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002187 File Offset: 0x00000387
		public int GetNumberOfBytesToRead()
		{
			return this._SerialPort.BytesToRead;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002194 File Offset: 0x00000394
		public byte[] ReadData(int nofBytesToRead)
		{
			byte[] array = new byte[nofBytesToRead];
			int num = 0;
			while (nofBytesToRead > 0)
			{
				int num2 = this._SerialPort.Read(array, num, nofBytesToRead);
				num += num2;
				nofBytesToRead -= num2;
			}
			return array;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021C9 File Offset: 0x000003C9
		public void PurgeBuffer()
		{
			if (!this._SerialPort.IsOpen)
			{
				return;
			}
			this._SerialPort.DiscardOutBuffer();
			this._SerialPort.DiscardInBuffer();
		}

		// Token: 0x04000001 RID: 1
		private readonly SerialPort _SerialPort;
	}
}
