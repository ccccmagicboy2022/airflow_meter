using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public enum X2StreamStatus : byte
	{
		InternalBufferOverflow = 1,
		SendFifoOverflow,
		StartupError,
		TransitTimeError,
		SequenceError,
		InvalidChar,
		QspiFifoError
	}
}
