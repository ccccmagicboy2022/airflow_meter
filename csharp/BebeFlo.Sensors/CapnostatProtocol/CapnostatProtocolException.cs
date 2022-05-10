using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	internal class CapnostatProtocolException : ApplicationException
	{
		public CapnostatProtocolException(string msg) : base(msg)
		{
		}
	}
}
