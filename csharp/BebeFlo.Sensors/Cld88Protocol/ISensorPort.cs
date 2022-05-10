using System;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public interface ISensorPort
	{
		object SyncRoot
		{
			get;
		}

		void Write(byte[] buffer, int count);

		void Read(byte[] buffer, int offset, int count);

		void PurgeInBuffer();
	}
}
