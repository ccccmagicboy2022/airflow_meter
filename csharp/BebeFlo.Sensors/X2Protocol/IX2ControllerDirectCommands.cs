using System;

namespace BebeFlo.Sensors.X2Protocol
{
	public interface IX2ControllerDirectCommands
	{
		object SyncRoot
		{
			get;
		}

		void SendDirectCommand(X2ExternalDevice device, byte[] command);

		byte[] ReadDirectAnswer(X2ExternalDevice device);
	}
}
