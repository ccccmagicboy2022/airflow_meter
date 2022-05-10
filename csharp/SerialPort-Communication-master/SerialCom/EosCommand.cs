using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCom
{
	public enum EosCommand : byte
	{
		// Token: 0x04000002 RID: 2
		Stop_Sampling = 1,
		// Token: 0x04000003 RID: 3
		Start_Sampling,
		// Token: 0x04000004 RID: 4
		Set_Channel,
		// Token: 0x04000005 RID: 5
		Set_Sample_Time,
		// Token: 0x04000006 RID: 6
		Set_Driver_Ports,
		// Token: 0x04000007 RID: 7
		Set_Compression_Rate,
		// Token: 0x04000008 RID: 8
		Set_Division_Rate,
		// Token: 0x04000009 RID: 9
		Activate_O2,
		// Token: 0x0400000A RID: 10
		Deactivate_O2,
		// Token: 0x0400000B RID: 11
		Start_Env_Sampling,
		// Token: 0x0400000C RID: 12
		Start_Cal_Sampling,
		// Token: 0x0400000D RID: 13
		Send_Cal_Word,
		// Token: 0x0400000E RID: 14
		Save_Cal_Values,
		// Token: 0x0400000F RID: 15
		Reset_ComprDiv_Rate,
		// Token: 0x04000010 RID: 16
		Set_Offset_Channel,
		// Token: 0x04000011 RID: 17
		Set_Flow_Base,
		// Token: 0x04000012 RID: 18
		Standby_On,
		// Token: 0x04000013 RID: 19
		Standby_Off,
		// Token: 0x04000014 RID: 20
		Clear_Offset_Channel,
		// Token: 0x04000015 RID: 21
		MM_Calibrate,
		// Token: 0x04000016 RID: 22
		Temp_Calibrate,
		// Token: 0x04000017 RID: 23
		Press_Calibrate,
		// Token: 0x04000018 RID: 24
		Start_Peak_Sampling,
		// Token: 0x04000019 RID: 25
		Stop_Peak_Sampling,
		// Token: 0x0400001A RID: 26
		Set_Fast_Sample_Time,
		// Token: 0x0400001B RID: 27
		Set_Sample_Mode,
		// Token: 0x0400001C RID: 28
		Link_Analog_Port,
		// Token: 0x0400001D RID: 29
		Unlink_Analog_Port,
		// Token: 0x0400001E RID: 30
		Send_Comm_Port_Cmd,
		// Token: 0x0400001F RID: 31
		Direct_Link_Port,
		// Token: 0x04000020 RID: 32
		Link_Comm_Port,
		// Token: 0x04000021 RID: 33
		UnLink_Comm_Port,
		// Token: 0x04000022 RID: 34
		Set_Shutter_Param,
		// Token: 0x04000023 RID: 35
		Activate_Shutter,
		// Token: 0x04000024 RID: 36
		Activate_Heating,
		// Token: 0x04000025 RID: 37
		Deactivate_Heating,
		// Token: 0x04000026 RID: 38
		Activate_Pump,
		// Token: 0x04000027 RID: 39
		Deactivate_Pump,
		// Token: 0x04000028 RID: 40
		SelectCalibTab,
		// Token: 0x04000029 RID: 41
		Set_DACA_Offset,
		// Token: 0x0400002A RID: 42
		Set_DACB_Offset,
		// Token: 0x0400002B RID: 43
		Save_Ctrl_Values,
		// Token: 0x0400002C RID: 44
		Cancel_Ctrl_Values,
		// Token: 0x0400002D RID: 45
		WinNTHandshake,
		// Token: 0x0400002E RID: 46
		Change_SensorPort,
		// Token: 0x0400002F RID: 47
		Set_DACA_Value,
		// Token: 0x04000030 RID: 48
		Set_DACB_Value,
		// Token: 0x04000031 RID: 49
		Send_SpxValve_Cmd,
		// Token: 0x04000032 RID: 50
		Set_Single_Port,
		// Token: 0x04000033 RID: 51
		Clear_Single_Port,
		// Token: 0x04000034 RID: 52
		Send_Comm_Port_Str,
		// Token: 0x04000035 RID: 53
		Set_MainValvePar,
		// Token: 0x04000036 RID: 54
		SetCoSensorOffset,
		// Token: 0x04000037 RID: 55
		Read_Comm_Port_Str,
		// Token: 0x04000038 RID: 56
		SEND_ASIC_CMD,
		// Token: 0x04000039 RID: 57
		READ_ASIC_CMD,
		// Token: 0x0400003A RID: 58
		SetLED2,
		// Token: 0x0400003B RID: 59
		SetLED3,
		// Token: 0x0400003C RID: 60
		Reboot,
		// Token: 0x0400003D RID: 61
		Set_Protocol_Version,
		// Token: 0x0400003E RID: 62
		Set_Channel_Resolution = 62,
		// Token: 0x0400003F RID: 63
		Send_SerialNr = 141,
		// Token: 0x04000040 RID: 64
		DownloadFirmware,
		// Token: 0x04000041 RID: 65
		SelfTestX2,
		// Token: 0x04000042 RID: 66
		Restart = 200,
		// Token: 0x04000043 RID: 67
		Test_External_RS232,
		// Token: 0x04000044 RID: 68
		Get_Version,
		// Token: 0x04000045 RID: 69
		GetSpASStatus,
		// Token: 0x04000046 RID: 70
		Set_Amplitude,
		// Token: 0x04000047 RID: 71
		Set_User_Level = 206,
		// Token: 0x04000048 RID: 72
		Send_Config_Word,
		// Token: 0x04000049 RID: 73
		Save_Config_Eeprom,
		// Token: 0x0400004A RID: 74
		Get_Config_Values,
		// Token: 0x0400004B RID: 75
		Save_Factory_Cal_Config,
		// Token: 0x0400004C RID: 76
		Save_Cal_Word_Eeprom,
		// Token: 0x0400004D RID: 77
		Read_Cal_Word_Eeprom,
		// Token: 0x0400004E RID: 78
		Save_Config_Word_Eeprom,
		// Token: 0x0400004F RID: 79
		Read_Config_Word_Eeprom,
		// Token: 0x04000050 RID: 80
		Get_Amplitude,
		// Token: 0x04000051 RID: 81
		Set_Port,
		// Token: 0x04000052 RID: 82
		Clear_Port,
		// Token: 0x04000053 RID: 83
		Get_Cal_Word = 219,
		// Token: 0x04000054 RID: 84
		Get_FixCal_Byte,
		// Token: 0x04000055 RID: 85
		Set_Pwm_Mode,
		// Token: 0x04000056 RID: 86
		Set_Pwm_HiLevelWidth,
		// Token: 0x04000057 RID: 87
		Set_Pwm_CycleTime,
		// Token: 0x04000058 RID: 88
		Send_SensorValve_Cmd,
		// Token: 0x04000059 RID: 89
		Set_Frc_Valves,
		// Token: 0x0400005A RID: 90
		Set_Frc_Threshold,
		// Token: 0x0400005B RID: 91
		SwitchCoToBootMode,
		// Token: 0x0400005C RID: 92
		ResetCoSensor,
		// Token: 0x0400005D RID: 93
		Get_Fan_Speed,
		// Token: 0x0400005E RID: 94
		Set_PC_WatchDog,
		// Token: 0x0400005F RID: 95
		Trigger_PC_WatchDog,
		// Token: 0x04000060 RID: 96
		Get_RealAmplitudeAndPoti,
		// Token: 0x04000061 RID: 97
		Write_USER_Data_EEPROM,
		// Token: 0x04000062 RID: 98
		Read_USER_Data_EEPROM,
		// Token: 0x04000063 RID: 99
		Get_Stack = 240,
		// Token: 0x04000064 RID: 100
		ReadCriticalErrLog
	}
}
