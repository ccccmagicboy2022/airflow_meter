/*****************************************************************
** 版权:   
** 文件名: 
** 版本：
** 工作环境:
** 描述：
** 作者:
** 生成日期:
*****************************************************************/
#include "headerfile.h"

uint32_t time_up_down_temp0 = 0;//时差0
uint32_t time_up_down_temp1 = 0;//时差1
uint32_t time_up_down_temp2 = 0;//时差2
uint32_t time_up_down_temp3 = 0;//时差3
uint32_t time_up_down_temp4 = 0;//时差4
uint32_t time_up_down_temp5 = 0;//时差5
uint32_t time_up_down_temp6 = 0;//时差6
uint32_t time_up_down_temp7 = 0;//时差7
uint16_t Result_status = 0;//时差7

uint8_t time_down_up_flag = 0;

float TimeValue_down_up_temp;
uint32_t TimeValue_down_up_dec;
uint32_t TimeValue_down_up_int;

float TimeValue_up_temp;
uint32_t TimeValue_up_dec;
uint32_t TimeValue_up_int;

float TimeValue_down_temp;
uint32_t TimeValue_down_dec;
uint32_t TimeValue_down_int;

uint32_t time_up_down_temp[42]={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

float temper_red_temp;
float temper_bule_temp;
uint32_t temper_red;
uint32_t temper_bule;
uint32_t temper_red1;
uint32_t temper_bule1;
uint32_t temper_red2;
uint32_t temper_bule2;

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:   延时函数
代码编制：
******************************************************************/
void Delay_1us(unsigned int t)
{
	while(t--)
	{
	//	_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();
	//	_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();
	//	_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();_NOP();
	}
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI初始化
代码编制：
******************************************************************/
void SPI_Init(void)
{	
	SSN_1;	//SSN置高、关闭与MS1030通讯
	SI_0;   //默认MS1030数据输入为低
	SCK_0;  //默认MS1030时钟为低
    Delay_1us(2);        
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI通讯使能
代码编制：
******************************************************************/
void SPI_ENABLE(void)
{
	SSN_0;  //SSN置低、开始与MS1030通讯
	Delay_1us(2);	
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI通讯关闭 0-1-0
代码编制：
******************************************************************/
void SPI_DISABLE(void)
{
	SSN_0;
	Delay_1us(1);
	SSN_1;
	Delay_1us(1);
	SSN_0;
	Delay_1us(1);
	SSN_1;	
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  发送位“0”
代码编制：
******************************************************************/
void SEND_0(void)
{
	SI_0;
	SCK_1;
	SCK_0;
	//_NOP();
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  发送位“1”
代码编制：
******************************************************************/
void SEND_1(void)
{
	SI_1;
	SCK_1;
	SCK_0;
	//_NOP();
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI写一个字节 8位
代码编制：
******************************************************************/
void SPI_WRITE8(uint8_t wbuf8)
{
	uint8_t cnt,MSB8 = 0x80;
	//SPI_ENABLE();
	SCK_0;
	for(cnt = 8;cnt > 0;cnt--)
	{
		if(wbuf8 & MSB8)
		   SEND_1();
		else
		   SEND_0();
		wbuf8 <<= 1;
	}
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI读一个字节 8位
代码编制：
******************************************************************/
uint8_t SPI_READ8(void)
{
	uint8_t cnt;
	uint8_t LSB8 = 0x01;
	uint8_t rbuf8 = 0x00;
	
	for(cnt = 8;cnt > 0;cnt--)
	{
		rbuf8 <<= 1;
		SCK_1;
		//_NOP();
		if( SO_0_1 )
		   rbuf8 |= LSB8;
		//_NOP();
		SCK_0;
		//_NOP();
	}
	return rbuf8;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI写4个字节 32位
代码编制：
******************************************************************/
void SPI_WRITE32(uint32_t wbuf32)
{
	uint8_t  cnt;
	uint32_t MSB32 = 0x80000000;
	for(cnt=32;cnt>0;cnt--)
	{
		if(wbuf32 & MSB32)
		   SEND_1();
		else
		   SEND_0();
		wbuf32 <<= 1;
	}
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI都4个字节，32位
代码编制：
******************************************************************/
uint32_t SPI_READ32(void)
{
	uint8_t cnt;
	uint32_t LSB32 = 0x00000001;
	uint32_t rbuf32 = 0x00000000;
	
	for(cnt=32;cnt>0;cnt--)
	{
		rbuf32 <<=1;
		SCK_1;
		//_NOP();
		if( SO_0_1 )
		rbuf32 |= LSB32;
		//_NOP();
		SCK_0;
		//_NOP();
	}
	return rbuf32;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  SPI读2个字节，16位
代码编制：
******************************************************************/
uint16_t SPI_READ16(void)
{
	uint8_t cnt;
	uint16_t LSB16 = 0x0001;
	uint16_t rbuf16 = 0x0000;
	
	for(cnt=16;cnt>0;cnt--)
	{
		rbuf16 <<=1;
		SCK_1;
		if( SO_0_1 )
		   rbuf16 |= LSB16;
		SCK_0;
	}
	return rbuf16;
}



/*****************************************************************
函数名：
入口参数：
参数说明: RegNum 寄存器号：0、1~7；RegData 往寄存器内写的数据
出口参数：
参数说明：
功能:  写寄存器
代码编制：
******************************************************************/
void Write_Reg(uint8_t RegNum,uint32_t RegData)
{
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(0x80|RegNum);
	SPI_WRITE32(RegData);
	SPI_DISABLE();
}

/*****************************************************************
函数名：
入口参数：
参数说明: RegNum 寄存器号：0、1~7；RegData 往寄存器内写的数据
出口参数：
参数说明：
功能:  写寄存器
代码编制：
******************************************************************/
void Write_test(uint8_t RegNum,uint32_t RegData)
{
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(RegNum);
	SPI_WRITE32(RegData);
	SPI_DISABLE();
}

/*****************************************************************
函数名：
入口参数：
参数说明: RegNum 寄存器号 0-7
出口参数：
参数说明：ReadData 寄存器数据
功能:  读寄存器
代码编制：
******************************************************************/
uint32_t Read_Reg(uint8_t RegNum)
{
        uint32_t ReadData = 0;
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(RegNum);
	ReadData = SPI_READ32();
	SPI_DISABLE();
	return ReadData;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：ReadData PW1ST寄存器数据
功能:  读第一波脉宽寄存器，11位数据
代码编制：
******************************************************************/
uint16_t Read_PW_First(void)
{
        uint16_t ReadData = 0;
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(0xD0);
	ReadData = (SPI_READ16())>>5;
	SPI_DISABLE();
	return ReadData;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：ReadData PW1ST寄存器数据
功能:  读第一STOP脉宽寄存器，11位数据；
代码编制：
******************************************************************/
uint16_t Read_PW_STOP1(void)
{
        uint16_t ReadData = 0;
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(0xD1);
	ReadData = (SPI_READ16())>>5;
	SPI_DISABLE();
	return ReadData;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：ReadData 寄存器数据
参数说明：
功能:  读寄存器0的低8位，SPI通讯测试；
代码编制：
******************************************************************/
uint8_t Read_REG0_L(void)
{
        uint8_t ReadData = 0;
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(0xd3);
	ReadData = SPI_READ8();
	SPI_DISABLE();
	return ReadData;
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数： ReadData返回状态寄存器STAT的数据
参数说明：
功能:  读状态寄存器STAT
代码编制：
******************************************************************/
uint16_t Read_STAT(void)
{
        uint16_t ReadData = 0;
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(0xd2);
	ReadData = SPI_READ16();
	SPI_DISABLE();
	return ReadData;
}

/*****************************************************************
函数名：
入口参数：Order 命令；Order_Num 命令序号 用作其他程序用（自定义的）
参数说明: 
出口参数：
参数说明：
功能:  写命令，如0x02 温度测量
代码编制：
******************************************************************/
void Write_Order(uint8_t Order,uint8_t Order_Num)
{
	SPI_Init();
	SPI_ENABLE();
	SPI_WRITE8(Order);
	SPI_DISABLE();
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  MS1030 5个寄存器初始化
代码编制：
******************************************************************/
uint8_t MS1030_Config(void)
{
        uint32_t REG0,REG1,REG2,REG3,REG4;
        SPI_check_temp=0;
        REG0=0;
        REG1=0;
        REG2=0;
        REG3=0;
        REG4=0;
      
        REG0=0x1e188930;      
        REG1=0xa00f0000; 
        REG2=0x83105187;     
        REG3=0x20928480;        
        REG4=0x47ec0500;
      
        RST_1;
        Delay_1us(1);
        RST_0;
        Delay_1us(10000);
        RST_1;
        Delay_1us(100);
        Power_On_Reset();
        Write_Reg(0,REG0);
	Write_Reg(1,REG1);
	Write_Reg(2,REG2);
	Write_Reg(3,REG3);
	Write_Reg(4,REG4);
        
        Init();
        SPI_check_temp= Read_REG0_L();
        return SPI_check_temp;        
}


/********************************************************************************
说明:	将测试浮点数进行排序
*/
uint32_t data_average(uint32_t *dtatzz,uint8_t num) /*定义两个参数：数组首地址与数组大小*/ 
{ 
      uint8_t i,j;
      uint32_t temp; 
      uint32_t data_temp0=0;
      for(i=0;i<num-1;i++) 
      for(j=i+1;j<num;j++) /*注意循环的上下限*/ 
      if(dtatzz[i]>dtatzz[j]) 
      { 
            temp=dtatzz[i]; 
            dtatzz[i]=dtatzz[j]; 
            dtatzz[j]=temp; 
      }
      for(i=2;i<num-2;i++)
      data_temp0=data_temp0+dtatzz[i];
      data_temp0>>=2;
      return data_temp0;
} 

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  MS1030中断处理，通过MS1030_STATE状态来判断；如温度测量命令序号为START_TEMP
代码编制：
******************************************************************/
uint32_t MS1030_Flow(void)
{
                 
                  float Result_up=0;
                  float Result_down=0;
                  float Result_up_down=0;
                  Result_Reg0=0;
                  Result_Reg1=0;
                  Result_Reg2=0;
                  Result_Reg3=0;
                  Result_Reg4=0;
                  Result_Reg5=0;
                  Result_Reg6=0;
                  Result_Reg7=0;
                  Result_Reg8=0;

                  Result_Reg0_0=0;
                  Result_Reg1_1=0;
                  Result_Reg2_2=0;
                  Result_Reg3_3=0;
                  Result_Reg4_4=0;
                  Result_Reg5_5=0;
                  Result_Reg6_6=0;
                  Result_Reg7_7=0;
                  Result_Reg8_8=0; 
                  
                  time_up_down_temp0=0;
                  time_up_down_temp1=0;
                  time_up_down_temp2=0;
                  time_up_down_temp3=0;
                  time_up_down_temp4=0;
                  time_up_down_temp5=0;
                  time_up_down_temp6=0;
                  time_up_down_temp7=0;
                  
                  time_up_result=0;
                  time_down_result=0;
                  time_up_down_result=0;
                  Result_PW_First=0;
                  Result_PW_STOP1=0;
                  Result_status=0;
                  
                  //Write_Reg(4,0x47ec0700);//up
                  //Write_Reg(4,0x27ec0700);//down
                  Delay_ms(1);
                  Init();
                  Start_TOF_UP_DOWN();
                  while(Intn_flag == 0);
                  Intn_flag = 0;
                  Result_status = Read_STAT();
                  Result_PW_First=Read_PW_First();
                  Result_PW_STOP1=Read_PW_STOP1();
                  Result_Reg0 = Read_Reg(0xb0);
	          Result_Reg1 = Read_Reg(0xb1);
	          Result_Reg2 = Read_Reg(0xb2);
                  Result_Reg3 = Read_Reg(0xb3);
	          Result_Reg4 = Read_Reg(0xb4);
	          Result_Reg5 = Read_Reg(0xb5);
                  Result_Reg6 = Read_Reg(0xb6);
	          Result_Reg7 = Read_Reg(0xb7);
	          Result_Reg8 = Read_Reg(0xb8);
                 
    	          Result_Reg0_0 = Read_Reg(0xb9);
                  Result_Reg1_1 = Read_Reg(0xba);
                  Result_Reg2_2 = Read_Reg(0xbb);
                  Result_Reg3_3 = Read_Reg(0xbc);
	          Result_Reg4_4 = Read_Reg(0xbd);
	          Result_Reg5_5 = Read_Reg(0xbe);
                  Result_Reg6_6 = Read_Reg(0xbf);
	          Result_Reg7_7 = Read_Reg(0xc0);
	          Result_Reg8_8 = Read_Reg(0xc1);
    	          time_down_up_flag = 1;
    	          _NOP(); 
                  if(time_down_up_flag == 1)
                  {
                     
                      if(Result_Reg0 >= Result_Reg0_0)
                      {
                            time_up_down_temp[0]= Result_Reg0 - Result_Reg0_0;
                      }
                      else
                      {
                            time_up_down_temp[0]= Result_Reg0_0 - Result_Reg0;
                      }
                      if(Result_Reg1 >= Result_Reg1_1)
                      {
                            time_up_down_temp[1]= Result_Reg1 - Result_Reg1_1;
                      }                      
                      else
                      {
                            time_up_down_temp[1] = Result_Reg1_1 - Result_Reg1;
                      }
                      if(Result_Reg2 >= Result_Reg2_2)
                      {
                            time_up_down_temp[2]= Result_Reg2 - Result_Reg2_2;
                      }
                      else
                      {
                            time_up_down_temp[2]= Result_Reg2_2 - Result_Reg2;
                      }
                      if(Result_Reg3 >= Result_Reg3_3)
                      {
                            time_up_down_temp[3]= Result_Reg3 - Result_Reg3_3;
                      }
                      else
                      {
                            time_up_down_temp[3]= Result_Reg3_3 - Result_Reg3;
                      }
                      if(Result_Reg4 >= Result_Reg4_4)
                      {
                            time_up_down_temp[4]= Result_Reg4 - Result_Reg4_4;
                      }
                      else
                      {
                            time_up_down_temp[4]= Result_Reg4_4 - Result_Reg4;
                      }
                      if(Result_Reg5 >= Result_Reg5_5)
                      {
                            time_up_down_temp[5]= Result_Reg5 - Result_Reg5_5;
                      }
                      else
                      {
                            time_up_down_temp[5]= Result_Reg5_5 - Result_Reg5;
                      }
                      if(Result_Reg6 >= Result_Reg6_6)
                      {
                            time_up_down_temp[6]= Result_Reg6 - Result_Reg6_6;
                      }
                      else
                      {
                            time_up_down_temp[6]= Result_Reg6_6 - Result_Reg6;
                      }
                      if(Result_Reg7 >= Result_Reg7_7)
                      {
                            time_up_down_temp[7]= Result_Reg7 - Result_Reg7_7;
                      }
                      else
                      {
                            time_up_down_temp[7]= Result_Reg7_7 - Result_Reg7;
                      }
                      time_down_up_flag=0;
                  }
    time_up_result=Result_Reg8;
    time_down_result=Result_Reg8_8;
    time_up_down_result=data_average(time_up_down_temp,8); 
    //顺流处理-LCD
    time_up_result >>= 5;
    
    Result_up = ((float)(time_up_result))/65536*1000000;
    time_up_result = (uint32_t)(Result_up);
    
    //逆流处理-LCD
    time_down_result >>= 5;
    Result_down = ((float)(time_down_result))/65536*1000000;
    time_down_result = (uint32_t)(Result_down);
    
    //时差-LCD
    time_up_down_result>>=2;
    Result_up_down = ((float)(time_up_down_result))/65536*1000000;
    time_up_down_result = (uint32_t)(Result_up_down);
    
    return  time_up_down_result; 
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  MS1030温度测量
代码编制：
******************************************************************/
void MS1030_Temper(void)
{
                  Result_Reg0=0;
                  Result_Reg1=0;
                  Result_Reg2=0;
                  Result_Reg3=0;
                  temper_red_temp=0;
                  temper_bule_temp=0;
                  temper_red1=0;
                  temper_bule1=0;
                  temper_red2=0;
                  temper_bule2=0;
                  temper_red=0;
                  temper_bule=0;
                  Result_status=0;

                  Init();
                  Result_status = Read_STAT();
                  Start_Temp();
    		  while(Intn_flag == 0);
                  Intn_flag = 0;
                  Result_status = Read_STAT();
                  Result_Reg0 = Read_Reg(0xc2);
	          Result_Reg1 = Read_Reg(0xc3);
                  Result_Reg2 = Read_Reg(0xc4);
	          Result_Reg3 = Read_Reg(0xc5);
                  
                  temper_red_temp=((float)(Result_Reg1))/((float)(Result_Reg0));
                  temper_bule_temp=((float)(Result_Reg2))/((float)(Result_Reg0));
                  temper_red1=(uint32_t)(temper_red_temp*100000);
                  temper_bule1=(uint32_t)(temper_bule_temp*100000);
                  
                  temper_red_temp=0;
                  temper_bule_temp=0;
                  
                  temper_red_temp=((float)(Result_Reg1))/((float)(Result_Reg3));
                  temper_bule_temp=((float)(Result_Reg2))/((float)(Result_Reg3));
                  temper_red2=(uint32_t)(temper_red_temp*150000);
                  temper_bule2=(uint32_t)(temper_bule_temp*150000);
                  
                  temper_red=(temper_red1+temper_red2)>>1;
                  temper_bule=(temper_bule1+temper_bule2)>>1;
                  Delay_us(100);
                 
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  MS1030时钟校验            
代码编制：
******************************************************************/
void MS1030_Time_check(void)
{
                  
                  Delay_us(100);
                  START_CAL_RESONATOR();
    		  while(Intn_flag == 0);
                  
                  Delay_ms(20);
                  Intn_flag = 0;
                  Result_Reg0 = Read_Reg(0xd4);
                  time_check_temp=Result_Reg0;
                  Delay_us(100);
                 
}


