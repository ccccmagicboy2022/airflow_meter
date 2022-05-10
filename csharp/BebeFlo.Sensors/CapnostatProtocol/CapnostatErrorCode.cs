using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public enum CapnostatErrorCode : byte
	{
		Bootcode,
		InvalidCommand,
		ChecksumError,
		TimeoutError,
		InvalidByteCount,
		InvalidDataByte,
		SystemFaulty_CEB6,
		SystemFaulty_CEB7,
		SystemFaulty_CEB8,
		SystemFaulty_CEB9,
		SystemFaulty_CEB10,
		FutureError_CEB11,
		FutureError_CEB12,
		FutureError_CEB13,
		FutureError_CEB14,
		FutureError_CEB15,
		FutureError_CEB16,
		FutureError_CEB17,
		FutureError_CEB18,
		FutureError_CEB19,
		SystemFaulty_CEB20,
		SystemFaulty_CEB21,
		SystemFaulty_CEB22,
		SystemFaulty_CEB23,
		SystemFaulty_CEB24
	}
}
