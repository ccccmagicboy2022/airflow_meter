using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	[Flags]
	public enum CldOperationMode
	{
		Normal = 0,
		PowerUp = 4,
		Calibration = 8,
		StandBy = 16,
		Unknown = 128
	}
}
