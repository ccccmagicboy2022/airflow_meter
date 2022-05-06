using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
	public class VersionResult
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600006E RID: 110 RVA: 0x0000243D File Offset: 0x0000063D
		public Version BootloaderVersion { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002445 File Offset: 0x00000645
		public Version FirmwareVersion { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000070 RID: 112 RVA: 0x0000244D File Offset: 0x0000064D
		public int HardwareVersion { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002455 File Offset: 0x00000655
		public string DeviceInfo { get; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000072 RID: 114 RVA: 0x0000245D File Offset: 0x0000065D
		public int SerialNumber { get; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00002465 File Offset: 0x00000665
		public string CrcFirmware { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000074 RID: 116 RVA: 0x0000246D File Offset: 0x0000066D
		public int DealerId { get; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00002475 File Offset: 0x00000675
		public bool MmEnabled { get; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000076 RID: 118 RVA: 0x0000247D File Offset: 0x0000067D
		public bool TtEnabled { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00002485 File Offset: 0x00000685
		public Version ProtocolVersion { get; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000078 RID: 120 RVA: 0x0000248D File Offset: 0x0000068D
		public int SensorType { get; }

		// Token: 0x06000079 RID: 121 RVA: 0x00002498 File Offset: 0x00000698
		public VersionResult(Version bootloaderVersion, Version firmwareVersion, int hardwareVersion, string deviceInfo, int serialNumber, string crcFirmware, int dealerId, bool mmEnabled, bool ttEnabled, Version protocolVersion, int sensorType)
		{
			this.BootloaderVersion = bootloaderVersion;
			this.FirmwareVersion = firmwareVersion;
			this.HardwareVersion = hardwareVersion;
			this.DeviceInfo = deviceInfo;
			this.SerialNumber = serialNumber;
			this.CrcFirmware = crcFirmware;
			this.DealerId = dealerId;
			this.MmEnabled = mmEnabled;
			this.TtEnabled = ttEnabled;
			this.ProtocolVersion = protocolVersion;
			this.SensorType = sensorType;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00002500 File Offset: 0x00000700
		public VersionResult(Version firmwareVersion, int hardwareVersion, int serialNumber, int dealerId, bool mmEnabled, bool ttEnabled, Version protocolVersion, int sensorType)
		{
			this.FirmwareVersion = firmwareVersion;
			this.HardwareVersion = hardwareVersion;
			this.SerialNumber = serialNumber;
			this.DealerId = dealerId;
			this.MmEnabled = mmEnabled;
			this.TtEnabled = ttEnabled;
			this.ProtocolVersion = protocolVersion;
			this.SensorType = sensorType;
		}
	}
}
