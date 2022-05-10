using System;
using System.Text;

namespace BebeFlo.Sensors.X2Protocol
{
	public class X2Versions
	{
		public int Hardware
		{
			get;
			set;
		}

		public int ModuleInfo1
		{
			get;
			set;
		}

		public int ModuleInfo2
		{
			get;
			set;
		}

		public int SerialNo1
		{
			get;
			set;
		}

		public int SerialNo2
		{
			get;
			set;
		}

		public int FirmwareCrc
		{
			get;
			set;
		}

		public int SensorStatus
		{
			get;
			set;
		}

		public string BootLoaderString
		{
			get
			{
				return X2Versions.FormatVersionString(this.BootLoader);
			}
		}

		public string FirmwareString
		{
			get
			{
				return X2Versions.FormatVersionString(this.Firmware);
			}
		}

		public string X2HardwareString
		{
			get
			{
				return string.Format("{0}.{1}", (byte)(this.Hardware >> 4 & 15), (byte)(this.Hardware & 15));
			}
		}

		public string ASHardwareString
		{
			get
			{
				return string.Format("{0}.{1} ({2})", (byte)(this.Hardware >> 4 & 15), (byte)(this.Hardware & 15), (this.Hardware < 32) ? "ASICA" : "ASICB");
			}
		}
        //获得序列号
		public string X2SerialNoString
		{
			get
			{
				return string.Format("{0}-{1}", this.SerialNo1, this.SerialNo2);
			}
		}

		public string ASSerialNoString
		{
			get
			{
				return string.Format("{0}", ((this.SerialNo1 & 255) << 16) + this.SerialNo2);
			}
		}

		private static string FormatVersionString(int[] arr)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < arr.Length; i++)
			{
				int value = arr[i];
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(".");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public int[] BootLoader = new int[4];

		public int[] Firmware = new int[4];
	}
}
