using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public interface IDumpable
	{
		string ToDumpString(string separator);

		string[] GetHeaders();
	}
}
