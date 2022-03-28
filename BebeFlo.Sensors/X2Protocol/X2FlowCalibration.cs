using System;
using System.Linq;
using BebeFlo.Utils;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2FlowCalibration
	{
		public int[] Values
		{
			get;
			private set;
		}

		public ushort FlowGainInspiration
		{
			get
			{
				return (ushort)this.Values[15];
			}
			set
			{
				this.Values[15] = (int)value;
			}
		}

		public ushort FlowGainExpiration
		{
			get
			{
				return (ushort)this.Values[16];
			}
			set
			{
				this.Values[16] = (int)value;
			}
		}

		public short TransitTimeBase
		{
			get
			{
				return (short)this.Values[27];
			}
			set
			{
				this.Values[27] = (int)value;
			}
		}

		public ushort[] LinTabIn
		{
			get
			{
				return (from val in this.Values.Skip(32).Take(16)
				select (ushort)val).ToArray<ushort>();
			}
		}

		public ushort[] LinTabEx
		{
			get
			{
				return (from val in this.Values.Skip(48).Take(16)
				select (ushort)val).ToArray<ushort>();
			}
		}

		public X2FlowCalibration()
		{
			this.Values = new int[64];
		}

		public X2FlowCalibration(int[] values)
		{
			if (values.Length != 64)
			{
				throw new ArgumentOutOfRangeException("values", "Exactly 64 calibration values required!");
			}
			this.Values = values;
		}

		public override string ToString()
		{
			return string.Format("[GainIn={0}, GainEx={1}, TTBase={2}, LinTabIn={{{3}}}, LinTabEx={{{4}}}]", new object[]
			{
				this.FlowGainInspiration,
				this.FlowGainExpiration,
				this.TransitTimeBase,
				this.LinTabIn.JoinToString(','),
				this.LinTabEx.JoinToString(',')
			});
		}

		public const short ADR_FLOWGAININ = 15;

		public const short ADR_FLOWGAINEX = 16;

		public const short ADR_TTBASE = 27;

		public const short ADR_SERIALNR1 = 30;

		public const short ADR_SERIALNR2 = 31;

		public const short ADR_LINTABIN_BASE = 32;

		public const short ADR_LINTABEX_BASE = 48;
	}
}
