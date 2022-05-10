using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
    
    public  class Common
    {
		private readonly TimeSpan _stopSamplingConfirmDelay = TimeSpan.FromMilliseconds(300.0);
		protected readonly byte[] _outbuf = new byte[100];
        protected readonly byte[] _inbuf = new byte[300];
		private static readonly ushort[] CrcTable = new ushort[256]
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

		private SerialPort serialPort1;
		public EosAccessor _EosAccessor;		 
		public Common(SerialPort serialPort)
        {
			this.serialPort1 = serialPort;
			this._EosAccessor = new EosAccessor(serialPort);

		}


		public bool CheckCrc(byte[] dataWithCrc)
		{
			int num = dataWithCrc.Length;
			int num2 = CrcCalculation.CalculateCRC(dataWithCrc, num - 2);
			return ((int)dataWithCrc[num - 2] << 8) + (int)dataWithCrc[num - 1] == num2;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000025F8 File Offset: 0x000007F8
		private void SendCommand(EosCommand command, byte[] parameters = null)
		{
			if (parameters == null)
			{
				parameters = new byte[0];
			}
			List<byte> list = new List<byte>();
			list.Add(1);
			list.Add((byte)(1 + parameters.Count<byte>()));
			list.Add((byte)command);
			list.AddRange(parameters);
			int num = CrcCalculation.CalculateCRC(list.ToArray(), list.Count);
			list.Add((byte)(num >> 8));
			list.Add((byte)(num & 255));
			this._EosAccessor.WriteData(list.ToArray());
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002678 File Offset: 0x00000878
		private byte[] SendCommandAndReceive(EosCommand command, byte[] parameters = null)
		{
			this.SendCommand(command, parameters);
			byte[] array = this._EosAccessor.ReadData(1);
			byte[] second = this._EosAccessor.ReadData((int)(array[0] - 1));
			byte[] array2 = array.Concat(second).ToArray<byte>();
			if (!this.CheckCrc(array2))
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Invalid CRC received. ",
					string.Format("Command: {0}; ", command),
					"sent data: ",
					BitConverter.ToString(parameters ?? new byte[0]),
					"; received data: ",
					BitConverter.ToString(array2)
				}));
			}
			return array2;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x0000271A File Offset: 0x0000091A
		private int GetIntOfBytes(params byte[] bytes)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			return BitConverter.ToInt32(bytes, 0);
		}


		public void PurgeBuffer()
		{
			_EosAccessor.PurgeBuffer();
		}


		public VersionResult Get_Version()
		{
			byte[] array = this.SendCommandAndReceive(EosCommand.Get_Version, null);
			Version firmwareVersion = new Version((int)array[5], (int)array[6], (int)array[7], (int)array[8]);
			int hardwareVersion = (int)array[9];
			byte[] array2 = new byte[4];
			array2[0] = array[13];
			array2[1] = array[12];
			array2[2] = array[15];
			return new VersionResult(firmwareVersion, hardwareVersion, this.GetIntOfBytes(array2), (int)(array[14] & 31), (array[14] & 128) > 0, (array[14] & 64) > 0, new Version((int)(array[19] & 240), (int)(array[19] & 15)), (int)array[20]);
		}

		public void Standby_Off()
		{
			this.SendCommand(EosCommand.Standby_Off, null);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002530 File Offset: 0x00000730
		public void Standby_On()
		{
			this.SendCommand(EosCommand.Standby_On, null);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000253B File Offset: 0x0000073B
		public void Start_Peak_Sampling()
		{
			this.SendCommand(EosCommand.Start_Peak_Sampling, null);
		}

		public void Stop_Peak_Sampling()
		{
			this.SendCommand(EosCommand.Stop_Peak_Sampling, null);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002546 File Offset: 0x00000746
		public void Stop_Sampling()
		{
			this.SendCommand(EosCommand.Stop_Sampling, null);
		}

		public void Start_Sampling()
        {
			this.SendCommand(EosCommand.Start_Sampling, null);
		}

		public bool CheckEosIsReady()
		{
			int num = 0;
			do
			{
				try
				{
					VersionResult version = this.Get_Version();
					var SerialNo = version.SerialNumber;
					var FirmwareVersion = version.FirmwareVersion;
					return true;
				}
				catch (TimeoutException)
				{
					num++;
					 
				}
			}
			while (num <= 2);
			return false;
		}


		public void SendMessage(byte cmd)
		{

			_outbuf[0] = 1;
			_outbuf[1] = 1;
			_outbuf[2] = cmd;
			AppendCrc(_outbuf, 3);
			Write(_outbuf, 5);
		}


		public void SendMessage(byte cmd, byte arg1)
		{
			_outbuf[0] = 1;
			_outbuf[1] = 2;
			_outbuf[2] = cmd;
			_outbuf[3] = arg1;
			AppendCrc(_outbuf, 4);
			Write(_outbuf, 6);
		}


		public void SendMessage(byte cmd, byte arg1, byte arg2)
		{
			_outbuf[0] = 1;
			_outbuf[1] = 3;
			_outbuf[2] = cmd;
			_outbuf[3] = arg1;
			_outbuf[4] = arg2;
			AppendCrc(_outbuf, 5);
			Write(_outbuf, 7);
		}

		public void SendMessage(byte cmd, byte arg1, byte arg2, byte arg3)
		{
			_outbuf[0] = 1;
			_outbuf[1] = 4;
			_outbuf[2] = cmd;
			_outbuf[3] = arg1;
			_outbuf[4] = arg2;
			_outbuf[5] = arg3;
			AppendCrc(_outbuf, 6);
			Write(_outbuf, 8);
		}

		public void SendMessage(byte cmd, byte arg1, byte arg2, params byte[] argMore)
		{
			if (argMore == null)
			{
				throw new ArgumentNullException("argMore");
			}
			_outbuf[0] = 1;
			_outbuf[1] = (byte)(3 + argMore.Length);
			_outbuf[2] = cmd;
			_outbuf[3] = arg1;
			_outbuf[4] = arg2;
			Array.Copy(argMore, 0, _outbuf, 5, argMore.Length);
			AppendCrc(_outbuf, 5 + argMore.Length);
			Write(_outbuf, 7 + argMore.Length);
		}

		public void ReadMessageOfSize(int expectedMsgLen)
		{

			if (expectedMsgLen < 3)
			{
				throw new ArgumentException("invalid parameter for expected message length", "expectedMsgLen");
			}
			ReadMessage();
			if (expectedMsgLen != _inbuf[0])
			{
				throw new Exception(string.Format("unexpected message length (received={0}, expected={1})", _inbuf[0], expectedMsgLen));
			}
		}


		private void Write(byte[] buffer, int count)
		{

			try
			{
				serialPort1.Write(buffer, 0, count);
			}
			catch (Exception exception)
			{

				throw;
			}
		}

		public void ReadMessage()
		{
			serialPort1.Read(_inbuf, 0, 1);
			int num = _inbuf[0];
			if (num < 3)
			{
				throw new Exception(string.Format("unexpected message length value ({0})", _inbuf[0]));
			}
			serialPort1.Read(_inbuf, 1, num - 1);
			int num2 = Crc(_inbuf, num - 2);
			int num3 = (_inbuf[num - 2] << 8) + _inbuf[num - 1];

			if (num2 != num3)
			{
				throw new Exception(string.Format("message crc error. (read=0x{0:x}, expected=0x{1:x})", num3, num2));
			}

		}

		private void AppendCrc(byte[] buffer, int len)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (len < 1)
			{
				throw new ArgumentException(string.Format("zero or negative length not allowed ({0})", len));
			}
			ushort num = Crc(buffer, len);
			buffer[len] = (byte)(num >> 8);
			buffer[len + 1] = (byte)(num & 0xFF);
		}

		private ushort Crc(byte[] buffer, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (length <= 0 || length > buffer.Length)
			{
				throw new ArgumentException(string.Format("invalid value for length parameter ({0})", length));
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				num = ((num >> 8) ^ CrcTable[(num & 0xFF) ^ buffer[i]]);
			}
			return (ushort)num;
		}

	}
}
