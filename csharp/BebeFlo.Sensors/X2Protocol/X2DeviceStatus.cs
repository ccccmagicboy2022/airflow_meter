using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public enum X2DeviceStatus
	{
		NoError,
		TimeoutError = 3,
		CalibrationDataError = 25,
		EepromFailure = 50,
		NoAccess = 60,
		UnknownCommand,
		CommandError,
		CommandCrcError,
		InitSensorError,
		BaselineError = 70,
		AmplitudeError,
		AmplitudeMaxError = 40,
		AmplitudeMinError
	}
}
