using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BebeFlo.Sensors.X2Protocol
{
	public struct FlowRecord : IDumpable<FlowRecord>, IDumpable
	{
		public override string ToString()
		{
			return string.Format("Flow={0}", this.Flow);
		}

		public string ToDumpString(string separator)
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{0}", new object[]
			{
				(double)this.Flow / -1000.0
			});
		}

		public string[] GetHeaders()
		{
			return new string[]
			{
				"Flow"
			};
		}

		public FlowRecord Average(IList<FlowRecord> records)
		{
			FlowRecord result = default(FlowRecord);
			result.Flow = Convert.ToInt32((from r in records
			select r.Flow).Average());
			return result;
		}

		public int Flow;
	}
}
