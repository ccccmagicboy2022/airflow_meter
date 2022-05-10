using System;
using BebeFlo.Sensors.Cld88Protocol;

namespace BebeFlo.Sensors.X2Protocol
{
	public interface IX2Port : ISensorPort
	{
		void Close();

		void ReconfigureForX2();

		void ReconfigureForCld88();

		void ReconfigureForExternalDevice(int baudRate, int dataBits);
	}
}
