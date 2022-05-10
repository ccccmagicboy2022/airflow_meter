using System;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public enum CapnostatSleepMode : byte
	{
		NormalOperatingMode,
		Mode1TurnOffSourceMaintainHeaters,
		Mode2MaximumPowerSavvings
	}
}
