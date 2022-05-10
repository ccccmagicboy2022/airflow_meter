using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
	public class CrcCalculation
	{
		// Token: 0x06000033 RID: 51 RVA: 0x000028AC File Offset: 0x00000AAC
		public static int CalculateCRC(byte[] data, int len)
		{
			int num = 0;
			int num2 = 0;
			if (len == 0)
			{
				return 0;
			}
			while (len-- > 0)
			{
				num2 = (num2 >> 8 ^ (int)CrcCalculation.CRC16_table_refl[(num2 & 255) ^ (int)data[num++]]);
			}
			return num2;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000028E8 File Offset: 0x00000AE8
		public static int CalculateCRC_FF_Initialized(byte[] data, int len)
		{
			int num = 0;
			int num2 = 65535;
			if (len == 0)
			{
				return 0;
			}
			while (len-- > 0)
			{
				num2 = (num2 >> 8 ^ (int)CrcCalculation.CRC16_table_refl[(num2 & 255) ^ (int)data[num++]]);
			}
			return num2;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002928 File Offset: 0x00000B28
		public static int CalculateCRC(char[] data, int len)
		{
			int num = 0;
			int num2 = 0;
			if (len == 0)
			{
				return 0;
			}
			while (len-- > 0)
			{
				num2 = (num2 >> 8 ^ (int)CrcCalculation.CRC16_table_refl[(num2 & 255) ^ (int)data[num++]]);
			}
			return num2;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002964 File Offset: 0x00000B64
		public static ushort CalculateCRC2(byte[] data, int len, ushort crc)
		{
			int num = 0;
			ushort num2 = crc;
			if (len == 0)
			{
				return 0;
			}
			while (len-- > 0)
			{
				num2 = (ushort)(num2 >> 8 ^ (int)CrcCalculation.CRC16_table_refl[(int)((num2 & 255) ^ (ushort)data[num++])]);
			}
			return num2;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000029A0 File Offset: 0x00000BA0
		public static void CalculateCRC(object structure, ref ushort crc)
		{
			Marshal.SizeOf(structure);
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
			Marshal.StructureToPtr(structure, intPtr, false);
			byte[] array = new byte[Marshal.SizeOf(structure) - 2];
			Marshal.Copy(intPtr, array, 0, Marshal.SizeOf(structure) - 2);
			Marshal.FreeHGlobal(intPtr);
			crc = (ushort)CrcCalculation.CalculateCRC(array, Marshal.SizeOf(structure) - 2);
		}

		// Token: 0x04000007 RID: 7
		private static ushort[] CRC16_table_refl = new ushort[]
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
