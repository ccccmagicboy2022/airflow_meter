using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BebeFlo.Sensors.X2Protocol
{
	public struct WashoutRecordSF6 : IDumpable<WashoutRecordSF6>, IDumpable
	{
		public override string ToString()
		{
			return string.Format("Flow={0}, O2={1}, CO2={2}, N2={3}, SF6={4}, Ar={5}, MMss={6}, SampleFlow={7}, MMms={8}", new object[]
			{
				this.Flow,
				this.O2,
				this.CO2,
				this.N2,
				this.SF6,
				this.Ar,
				this.MMss,
				this.SampleFlow,
				this.MMms
			});
		}

		public string ToDumpString(string separator)
		{
			return string.Format(NumberFormatInfo.InvariantInfo, "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", new object[]
			{
				separator,
				(double)this.Flow / -1000.0,
				this.O2,
				this.CO2,
				this.N2,
				this.SF6,
				this.Ar,
				this.MMss,
				(double)this.SampleFlow / -1000.0,
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
				"N2",
				"SF6",
				"Ar",
				"MMss",
				"SampleFlow",
				"MMms"
			};
		}

		public WashoutRecordSF6 Average(IList<WashoutRecordSF6> records)
		{
			WashoutRecordSF6 result = default(WashoutRecordSF6);
			result.Flow = (from r in records
			select r.Flow).Average();
			result.SampleFlow = (this.Flow = (from r in records
			select r.SampleFlow).Average());
			result.O2 = (from r in records
			select r.O2).Average();
			result.CO2 = (from r in records
			select r.CO2).Average();
			result.N2 = (from r in records
			select r.N2).Average();
			result.SF6 = (from r in records
			select r.SF6).Average();
			result.Ar = (from r in records
			select r.Ar).Average();
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

		public float N2;

		public float SF6;

		public float Ar;

		public float MMss;

		public float MMms;

		public float DelayO2;

		public float DelayMMss;
	}
}
