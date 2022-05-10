using System;
using System.Collections.Generic;

namespace BebeFlo.Sensors.CapnostatProtocol
{
	public class CapnostatStatus
	{
		public CapnostatStatus(IList<byte> statusBytes)
		{
			if (statusBytes.Count != 5)
			{
				throw new ArgumentException("5 Bytes expected", "extendedAndPrioritizedStatusBytes");
			}
			for (int i = 0; i < 4; i++)
			{
				byte b = statusBytes[i];
				for (ushort num = 0; num < 8; num += 1)
				{
					this._statusBytes.Add(this.ISBitSet(b, num));
				}
			}
			this._prioritizedStatus = statusBytes[4];
		}

		public CapnostatPrioritizedStatus GetPrioritizedStatus()
		{
			return (CapnostatPrioritizedStatus)this._prioritizedStatus;
		}

		private bool ISBitSet(byte b, ushort bitnr)
		{
			if (bitnr > 7)
			{
				throw new ArgumentOutOfRangeException("bitnr");
			}
			return ((int)b & 1 << (int)bitnr) != 0;
		}

		private readonly List<bool> _statusBytes = new List<bool>();

		private readonly byte _prioritizedStatus;
	}
}
