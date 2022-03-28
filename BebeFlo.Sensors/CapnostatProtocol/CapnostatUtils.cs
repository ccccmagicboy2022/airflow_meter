using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public static class CapnostatUtils
	{
		public static byte Checksum(byte[] buffer)
		{
			return CapnostatUtils.Checksum(buffer, buffer.Length);
		}

		public static byte Checksum(byte[] buffer, int length)
		{
            if (buffer.Length < length)
            {
                throw new ArgumentException("Index would exceed buffer.", "length");
            }
            int index = 0;
            int num2 = 0;
            while (index < length)
            {
                num2 += buffer[index];
                index++;
            }
            return (byte)((~((byte)num2) + 1) & 0x7f);
		}
	}
}
