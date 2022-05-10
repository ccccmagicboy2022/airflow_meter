using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public enum CldReturnCode : byte
	{
		None,
		BlockCheckError,
		CommandOverflow,
		InvalidCommand,
		InvalidOperation,
		CommandNotAllowedInCurrentMode = 6,
		Reserved
	}
}
