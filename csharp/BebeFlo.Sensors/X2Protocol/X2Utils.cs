using System;
using System.Globalization;

namespace BebeFlo.Sensors.X2Protocol
{
	public static class X2Utils
	{
		public static ushort UInt16(byte hi, byte lo)
		{
			return (ushort)(((int)hi << 8) + (int)lo);
		}

		public static byte High(ushort val)
		{
			return (byte)(val >> 8);
		}

		public static byte Low(ushort val)
		{
			return (byte)(val & 255);
		}

		public static short Int16(byte hi, byte lo)
		{
			int num = ((int)hi << 8) + (int)lo;
			if (num > 32767)
			{
				num -= 65536;
			}
			return (short)num;
		}

		public static byte High(short val)
		{
			return (byte)(val >> 8);
		}

		public static byte Low(short val)
		{
			return (byte)(val & 255);
		}

		public static float Float(byte hi, byte lo)
		{
			if ((hi & 240) != 0)
			{
				throw new OverflowException("high byte does not comply with 12bit value range.");
			}
			if (hi == 0 && lo == 0)
			{
				return 0f;
			}
			return BitConverter.ToSingle(new byte[]
			{
				0,
				0,
				lo,
				(byte)(hi + 56)
			}, 0);
		}

		public static float Float(int val)
		{
			if (val < 0 || val >= 8192)
			{
				throw new OverflowException(string.Format(CultureInfo.InvariantCulture, "given value '{0}' does not fit into the 12bit unsigned range.", new object[]
				{
					val
				}));
			}
			return X2Utils.Float((byte)(val >> 8 & 255), (byte)(val & 255));
		}

		public static int FloatInv(float val)
		{
			if (val < 0f)
			{
				throw new ArgumentException("cannot convert negative values", "val");
			}
			byte[] bytes = BitConverter.GetBytes(val);
			if (bytes[2] == 0 && bytes[3] == 0)
			{
				return 0;
			}
			if (bytes[3] < 56)
			{
				return 0;
			}
			if (bytes[3] - 56 > 32)
			{
				throw new OverflowException("the number is too big to be represented as X2 12-bit float");
			}
			byte b = bytes[2];
            byte b2 = (byte)(bytes[3] - 56);
			return ((int)b2 << 8) + (int)b;
		}

		public static int TwoComplementSignedSignal(int signal, int nrBits)
		{
			if (nrBits > 32 || nrBits < 2)
			{
				throw new ArgumentOutOfRangeException("nrBits", "Int signal can represent max. 32 bit, signed needs at least 2 bits.");
			}
			if (signal >= 1 << nrBits)
			{
				throw new ArgumentOutOfRangeException("signal", "signal can not be representet by nrBits.");
			}
			if (signal >= 1 << nrBits - 1)
			{
				signal -= 1 << nrBits;
			}
			return signal;
		}
        //压缩率
		public static byte CompressionRate(int sourceFrequency, int targetFrequency)
		{
			if (sourceFrequency <= 0)
			{
				throw new ArgumentException("source frequency cannot be zero or negative", "sourceFrequency");
			}
			if (targetFrequency <= 0)
			{
				throw new ArgumentException("target frequency cannot be zero or negative", "targetFrequency");
			}
			if (sourceFrequency % targetFrequency != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "division factor between source frequency [{0}] and target frequency [{1}] cannot expressed as an integer.", new object[]
				{
					sourceFrequency,
					targetFrequency
				}), "targetFrequency");
			}
			return (byte)(sourceFrequency / targetFrequency);
		}

		public static byte X2BaudRateRepresentation(int baudRate)
		{
			baudRate /= 100;
			if (baudRate > 192)
			{
				baudRate /= 10;
			}
			return (byte)baudRate;
		}

		public static ushort Crc(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return X2Utils.Crc(buffer, buffer.Length);
		}

		public static ushort Crc(byte[] buffer, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (length <= 0 || length > buffer.Length)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "invalid value for length parameter ({0})", new object[]
				{
					length
				}), "length");
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = (num >> 8 ^ (int)X2Utils.CrcTable[(num & 255) ^ (int)buffer[i]]);
			}
			return (ushort)num;
		}

		public static readonly float X2FloatMinValue = 0f;

		public static readonly float X2FloatEpsilon = X2Utils.Float(1);

		public static readonly float X2FloatMaxValue = X2Utils.Float(4095);

		private static readonly ushort[] CrcTable = new ushort[]
		{
			0,
			4489,
			8978,
			12955,
			17956,
			22445,
			25910,
			29887,
			35912,
			40385,
			44890,
			48851,
			51820,
			56293,
			59774,
			63735,
			4225,
			264,
			13203,
			8730,
			22181,
			18220,
			30135,
			25662,
			40137,
			36160,
			49115,
			44626,
			56045,
			52068,
			63999,
			59510,
			8450,
			12427,
			528,
			5017,
			26406,
			30383,
			17460,
			21949,
			44362,
			48323,
			36440,
			40913,
			60270,
			64231,
			51324,
			55797,
			12675,
			8202,
			4753,
			792,
			30631,
			26158,
			21685,
			17724,
			48587,
			44098,
			40665,
			36688,
			64495,
			60006,
			55549,
			51572,
			16900,
			21389,
			24854,
			28831,
			1056,
			5545,
			10034,
			14011,
			52812,
			57285,
			60766,
			64727,
			34920,
			39393,
			43898,
			47859,
			21125,
			17164,
			29079,
			24606,
			5281,
			1320,
			14259,
			9786,
			57037,
			53060,
			64991,
			60502,
			39145,
			35168,
			48123,
			43634,
			25350,
			29327,
			16404,
			20893,
			9506,
			13483,
			1584,
			6073,
			61262,
			65223,
			52316,
			56789,
			43370,
			47331,
			35448,
			39921,
			29575,
			25102,
			20629,
			16668,
			13731,
			9258,
			5809,
			1848,
			65487,
			60998,
			56541,
			52564,
			47595,
			43106,
			39673,
			35696,
			33800,
			38273,
			42778,
			46739,
			49708,
			54181,
			57662,
			61623,
			2112,
			6601,
			11090,
			15067,
			20068,
			24557,
			28022,
			31999,
			38025,
			34048,
			47003,
			42514,
			53933,
			49956,
			61887,
			57398,
			6337,
			2376,
			15315,
			10842,
			24293,
			20332,
			32247,
			27774,
			42250,
			46211,
			34328,
			38801,
			58158,
			62119,
			49212,
			53685,
			10562,
			14539,
			2640,
			7129,
			28518,
			32495,
			19572,
			24061,
			46475,
			41986,
			38553,
			34576,
			62383,
			57894,
			53437,
			49460,
			14787,
			10314,
			6865,
			2904,
			32743,
			28270,
			23797,
			19836,
			50700,
			55173,
			58654,
			62615,
			32808,
			37281,
			41786,
			45747,
			19012,
			23501,
			26966,
			30943,
			3168,
			7657,
			12146,
			16123,
			54925,
			50948,
			62879,
			58390,
			37033,
			33056,
			46011,
			41522,
			23237,
			19276,
			31191,
			26718,
			7393,
			3432,
			16371,
			11898,
			59150,
			63111,
			50204,
			54677,
			41258,
			45219,
			33336,
			37809,
			27462,
			31439,
			18516,
			23005,
			11618,
			15595,
			3696,
			8185,
			63375,
			58886,
			54429,
			50452,
			45483,
			40994,
			37561,
			33584,
			31687,
			27214,
			22741,
			18780,
			15843,
			11370,
			7921,
			3960
		};
	}
}
