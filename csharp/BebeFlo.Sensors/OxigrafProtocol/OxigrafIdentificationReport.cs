using System;

namespace BebeFlo.Sensors.OxigrafProtocol
{
	public class OxigrafIdentificationReport
	{
		public string SerialNumber
		{
			get;
			set;
		}

		public string CellSerialNumber
		{
			get;
			set;
		}

		public int OperatingTimeHours
		{
			get;
			set;
		}
	}
}
