using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BebeFlo.Sensors.X2Protocol
{
	public struct WashoutRecord : IDumpable<WashoutRecord>, IDumpable
	{
		public override string ToString()
		{
			return string.Format("Flow={0}, O2={1}, CO2={2}, MMss={3}, SampleFlow={4}, MMms={5}", new object[]
			{
				this.Flow,
				this.O2,
				this.CO2,
				this.MMss,
				this.SampleFlow,
				this.MMms
			});
		}

		public string ToDumpString(string separator)
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", new object[]
			{
				(double)this.Flow / -1000.0,
				separator,
				this.O2,
				separator,
				this.CO2,
				separator,
				this.MMss,
				separator,
				(double)this.SampleFlow / -1000.0,
				separator,
				this.MMms
			});
		}

		public string[] GetHeaders()
		{
			return new string[]
			{
				"Flow",
				"O2",
				"CO2",
				"MMss",
				"SampleFlow",
				"MMms"
			};
		}

		public WashoutRecord Average(IList<WashoutRecord> records)
		{
			WashoutRecord result = default(WashoutRecord);
			result.Flow = (from r in records
			select r.Flow).Average();
			result.SampleFlow = (this.Flow = (from r in records
			select r.SampleFlow).Average());
			result.O2 = (from r in records
			select r.O2).Average();
			result.CO2 = (from r in records
			select r.CO2).Average();
			result.MMss = (from r in records
			select r.MMss).Average();
			result.MMms = (from r in records
			select r.MMms).Average();
			return result;
		}

		public float Flow;

		public float SampleFlow;

		public float O2;

		public float CO2;

		public float MMss;

		public float MMms;
	}
}
