using System;
using System.Collections.Generic;

namespace BebeFlo.Sensors.X2Protocol
{
	public interface IDumpable<T> : IDumpable
	{
		T Average(IList<T> records);
	}
}
