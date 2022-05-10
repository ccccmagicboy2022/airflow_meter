using System;

namespace BebeFlo.Sensors
{
	public class SerialConnectionOpenException : SerialConnectionException
	{
		public SerialConnectionOpenException(string msg) : base(msg)
		{
		}
	}
}
