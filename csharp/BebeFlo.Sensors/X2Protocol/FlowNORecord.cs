using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BebeFlo.Sensors.X2Protocol
{
	public struct FlowNORecord : IDumpable<FlowNORecord>, IDumpable
	{
		public float NO
		{
			get
			{
				return X2Utils.Float(this.NORaw);
			}
		}

		public override string ToString()
		{
			return string.Format("Flow={0}, NORaw={1}", this.Flow, this.NORaw);
		}

		public string ToDumpString(string separator)
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}{2}", new object[]
			{
				(double)this.Flow / -1000.0,
				separator,
				this.NO
			});
		}

		public string[] GetHeaders()
		{
			return new string[]
			{
				"Flow",
				"NO"
			};
		}

		public FlowNORecord Average(IList<FlowNORecord> records)
		{
			FlowNORecord result = default(FlowNORecord);
			result.Flow = Convert.ToInt32((from r in records
			select r.Flow).Average());
			result.NORaw = Convert.ToInt32((from val in records
			select val.NORaw).Average());
			return result;
		}

		public int Flow;

		public int NORaw;
	}
}
