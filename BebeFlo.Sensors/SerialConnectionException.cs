using System;

namespace BebeFlo.Sensors
{
	public class SerialConnectionException : ApplicationException
	{
		public SerialConnectionException(string msg) : base(msg)
		{
		}
	}
}
