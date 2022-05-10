using System;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public class OxigrafResponse
	{
		public byte FirstByte
		{
			get;
			private set;
		}

		public byte Length
		{
			get;
			private set;
		}

		public OxigrafResponse(byte firstByte, byte length)
		{
			if (firstByte != 6 && firstByte != 21)
			{
				throw new OxigrafProtocolException("Invalid response. It does not start with ACK or NAK.");
			}
			if (length == 0)
			{
				throw new OxigrafProtocolException("Response with length = 0 (no command) is invalid.");
			}
			this.FirstByte = firstByte;
			this.Length = length;
		}

		public bool ValidResponse()
		{
			if (this.FirstByte == 21)
			{
				throw new OxigrafProtocolException(string.Format("Command ('{0}') has not been acknowledged. Error-Code: {1}.", (char)this.Command, this.Data[0]));
			}
			if (this.Data.Length != (int)(this.Length - 1))
			{
				throw new OxigrafProtocolException(string.Format("unexpected Length of Data ({0}, expected: {1})", this.Data.Length, (int)(this.Length - 1)));
			}
			if (this.Checksum.Length != 2)
			{
				throw new OxigrafProtocolException("Invalid Checksum byte Array, 2 bytes expected");
			}
			if (!this.ValidChecksum())
			{
				throw new OxigrafProtocolException("Invalid Checksum");
			}
			return true;
		}

		public bool ValidResponse(char expectedCommand)
		{
			this.ValidResponse();
			if (this.Command != (byte)expectedCommand)
			{
				throw new OxigrafProtocolException(string.Format("Unexpected response command '{0}' ('{1}' expected)", (char)this.Command, expectedCommand));
			}
			return true;
		}

		private bool ValidChecksum()
		{
			int num = (int)this.Command;
			byte[] data = this.Data;
			for (int i = 0; i < data.Length; i++)
			{
				byte b = data[i];
				num += (int)b;
			}
			num %= 65536;
			int num2 = ((int)this.Checksum[0] << 8) + (int)this.Checksum[1];
			return num == num2;
		}

		private const byte ACK = 6;

		private const byte NAK = 21;

		public byte Command;

		public byte[] Data = new byte[0];

		public byte[] Checksum = new byte[2];
	}
}
