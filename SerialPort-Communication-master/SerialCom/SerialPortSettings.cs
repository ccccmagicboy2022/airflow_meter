using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
	public class SerialPortSettings
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002420 File Offset: 0x00000620
		public string PortName { get; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002428 File Offset: 0x00000628
		public int BaudRate { get; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002430 File Offset: 0x00000630
		public Handshake Handshake { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002438 File Offset: 0x00000638
		public int DataBits { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002440 File Offset: 0x00000640
		public StopBits StopBits { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002448 File Offset: 0x00000648
		public int ReadTimeOut { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002450 File Offset: 0x00000650
		public int WriteTimeOut { get; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00002458 File Offset: 0x00000658
		public Parity Parity { get; }

		// Token: 0x0600001D RID: 29 RVA: 0x00002460 File Offset: 0x00000660
		public SerialPortSettings(string portName)
		{
			this.PortName = portName;
			this.BaudRate = 57600;
			this.Handshake = 0;
			this.DataBits = 8;
			this.StopBits = StopBits.One;
			this.ReadTimeOut = 2000;
			this.WriteTimeOut = 1000;
			this.Parity = 0;
		}

		// Token: 0x0400000D RID: 13
		private const int NDD_FLOWIF_CMD_RECEIVE_TIMEOUT_ms = 1000;
	}
}
