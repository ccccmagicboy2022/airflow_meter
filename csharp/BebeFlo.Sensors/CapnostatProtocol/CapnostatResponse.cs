using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public class CapnostatResponse
	{
		public bool IsValidResponse()
		{
			if (!CapnostatResponse.IsCommandByte(this.Command))
			{
				throw new CapnostatProtocolException(string.Format("Invalid Command Byte in Response ({0})", this.Command));
			}
			char c = (char)this.Command;
			c += (char)((byte)this.NumberOfBytesToFollow);
			byte[] data = this.Data;
			for (int i = 0; i < data.Length; i++)
			{
				byte b = data[i];
				if (CapnostatResponse.IsCommandByte(b))
				{
					throw new CapnostatProtocolException(string.Format("Invalid Data Byte in Response ({0})", b));
				}
				c += (char)b;
			}
			c += (char)this.CheckSum;
			c &= '\u007f';
			return c == '\0';
		}

		public int Length()
		{
			return this.NumberOfBytesToFollow + 2;
		}

		public bool IsError()
		{
			return this.Command == 200;
		}

		public string ErrorName()
		{
			string result = "Unknown Error";
			if (this.IsError() && Enum.IsDefined(typeof(CapnostatErrorCode), this.Data[0]))
			{
				result = Enum.GetName(typeof(CapnostatErrorCode), this.Data[0]);
			}
			else if (!this.IsError())
			{
				result = "Is no Error Response.";
			}
			return result;
		}

		public static bool IsCommandByte(byte b)
		{
			return b >= 128;
		}

		public byte Command;

		public int NumberOfBytesToFollow;

		public byte[] Data;

		public byte CheckSum;
	}
}
