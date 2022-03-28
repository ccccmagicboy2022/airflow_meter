using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2ProtocolException : ApplicationException
	{
		public X2ProtocolException(string msg) : base(msg)
		{
		}

		public X2StreamStatus ErrorStatus
		{
			get;
			set;
		}

		public X2DeviceStatus DeviceErrorStatus
		{
			get;
			set;
		}
	}
}
