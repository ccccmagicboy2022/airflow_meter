using System;
using System.Globalization;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public class CldFilterConfig
	{
		public double FilterTimeSlow
		{
			get;
			set;
		}

		public double FilterTimeMedium
		{
			get;
			set;
		}

		public double FilterTimeFast
		{
			get;
			set;
		}

		public CldFilterStatus ChannelA
		{
			get;
			set;
		}

		public CldFilterStatus ChannelB
		{
			get;
			set;
		}

		public string ChannelAStatusString
		{
			get
			{
				return this.GetStatusString(this.ChannelA);
			}
		}

		public string ChannelBStatusString
		{
			get
			{
				return this.GetStatusString(this.ChannelB);
			}
		}

		public double GetFilterTimeByStatus(CldFilterStatus fs)
		{
			switch (fs)
			{
			case CldFilterStatus.Slow:
				return this.FilterTimeSlow;
			case CldFilterStatus.Medium:
				return this.FilterTimeMedium;
			case CldFilterStatus.Fast:
				return this.FilterTimeFast;
			case CldFilterStatus.Off:
				return 0.0;
			default:
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "unexpected value '{0}' for filter status", new object[]
				{
					fs
				}), "fs");
			}
		}

		public override string ToString()
		{
			return string.Format("ChA={0}, ChB={1} (Slow={2}, Medium={3}, Fast={4})", new object[]
			{
				this.ChannelA,
				this.ChannelB,
				this.FilterTimeSlow,
				this.FilterTimeMedium,
				this.FilterTimeFast
			});
		}

		private string GetStatusString(CldFilterStatus fs)
		{
			if (fs == CldFilterStatus.Off)
			{
				return fs.ToString();
			}
			return string.Format("{0} ({1} s)", fs, this.GetFilterTimeByStatus(fs));
		}
	}
}
