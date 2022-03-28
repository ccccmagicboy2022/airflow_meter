using System;
using System.Globalization;
using System.Text;

namespace BebeFlo.Sensors.Cld88Protocol
{
	public static class CldUtils
	{
		public static int ParseHexString(string value, int offset, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return int.Parse(value.Substring(offset, length), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		}

		public static bool ParseBit(char val, int bitNumber)
		{
			if (bitNumber < 0 || bitNumber > 7)
			{
				throw new ArgumentException("invalid value for the bit number to test", "bitNumber");
			}
			return ((int)val & 1 << bitNumber) != 0;
		}

		public static double ParseFloat(string value, int offset, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return double.Parse(value.Substring(offset, length), NumberStyles.Float, CultureInfo.InvariantCulture);
		}

		public static double ParseFloat(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CldUtils.ParseFloat(value, 0, value.Length);
		}

		public static int ParseInt(string value, int offset, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return int.Parse(value.Substring(offset, length), CultureInfo.InvariantCulture);
		}

		public static int ParseInt(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CldUtils.ParseInt(value, 0, value.Length);
		}

		public static string GetNDigitFloatString(double val, int numDigits)
		{
			int num = 1;
			while (Math.Pow(10.0, (double)num) <= val)
			{
				num++;
			}
			if (num > numDigits)
			{
				throw new ArgumentOutOfRangeException("numDigits", "value not representable in specified number of digits");
			}
			double num2 = Math.Round(val, Math.Max(0, numDigits - 1 - num));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < numDigits - 1; i++)
			{
				stringBuilder.Append("#");
			}
			stringBuilder.Append("0.");
			for (int j = 0; j < numDigits - 2; j++)
			{
				stringBuilder.Append("0");
			}
			return num2.ToString(stringBuilder.ToString(), CultureInfo.InvariantCulture).Substring(0, numDigits);
		}

		public static string[] SplitParts(string value, params int[] expectedNumberOfParts)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string[] array = value.Split(new char[]
			{
				','
			});
			if (Array.IndexOf<int>(expectedNumberOfParts, array.Length) < 0)
			{
				throw new CldProtocolException(string.Format(CultureInfo.InvariantCulture, "the given string contains '{0}' parts, which is not expected.", new object[]
				{
					array.Length
				}));
			}
			return array;
		}

		public static double ParseFloatOrStar(string value, int offset, int length, double starValueEquivalent)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string text = value.Substring(offset, length).Trim();
			while (text.StartsWith("- "))
			{
				text = text.Remove(1, 1);
			}
			if (!(text == "*"))
			{
				return CldUtils.ParseFloat(text, 0, text.Length);
			}
			return starValueEquivalent;
		}

		public static double ParseFloatOrStar(string value, double starValueEquivalent)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CldUtils.ParseFloatOrStar(value, 0, value.Length, starValueEquivalent);
		}

		public static int ParseIntOrStar(string value, int offset, int length, int starValueEquivalent)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string text = value.Substring(offset, length).Trim();
			while (text.StartsWith("- "))
			{
				text = text.Remove(1, 1);
			}
			if (!(text == "*"))
			{
				return CldUtils.ParseInt(text, 0, text.Length);
			}
			return starValueEquivalent;
		}

		public static int ParseIntOrStar(string value, int starValueEquivalent)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return CldUtils.ParseIntOrStar(value, 0, value.Length, starValueEquivalent);
		}

		public static byte BlockCheckCode(byte[] buffer, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (length < 0 || length > buffer.Length)
			{
				throw new ArgumentException("invalid argument for length parameter. (value is negative or too big.)", "length");
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num ^= (int)buffer[i];
			}
			return (byte)(num & 127);
		}
	}
}
