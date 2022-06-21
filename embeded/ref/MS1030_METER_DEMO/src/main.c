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
uint8_t  intn_cnt = 0;
uint32_t Result_Reg0 = 0;
uint32_t Result_Reg1 = 0;
uint32_t Result_Reg2 = 0;
uint32_t Result_Reg3 = 0;
uint32_t Result_Reg4 = 0;
uint32_t Result_Reg5 = 0;
uint32_t Result_Reg6 = 0;
uint32_t Result_Reg7 = 0;
uint32_t Result_Reg8 = 0;



uint32_t Result_Reg0_0 = 0;
uint32_t Result_Reg1_1 = 0;
uint32_t Result_Reg2_2 = 0;
uint32_t Result_Reg3_3 = 0;
uint32_t Result_Reg4_4 = 0;
uint32_t Result_Reg5_5 = 0;
uint32_t Result_Reg6_6 = 0;
uint32_t Result_Reg7_7 = 0;
uint32_t Result_Reg8_8 = 0;

uint32_t time_check_temp=0;

uint32_t liusu;
uint32_t liusu_temp1;
uint8_t biaozhi;
uint32_t liuliang = 0;
uint8_t uartback;
uint8_t uartcntback;
//uint8_t UartTemp[100];
uint8_t UartSendData1[10]={0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0a};
uint32_t liusu_temp[4]={0,0,0,0};
uint8_t  liusu_num=0;

uint16_t time_up = 0;//调试用

uint8_t rx_flag = 0;
uint32_t TimeValue_Array[8];
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:
代码编制：
******************************************************************/
void main(void)
{
        WDTCTL = WDTPW + WDTHOLD; 
        System_Config(); //系统初始化
        Empty_AllSeg();
        Set_AllSeg();//LCD全屏开启
        _EINT();//GIE使能  中断使能 
        MS1030_Config(); //MS1030初始化
        while(1)
        {
            uartback = USART_Data_Pack.Buffer[0];
    	    if(Key_flag)
    	    {    //按键处理函数
    		  Key_Progress();
    		  Key_flag = 0;
    	    }
    	    if(StartTof_flag)            
            {     //1s测试一次流速时间                
                  ACLK_ON;
                  Delay_ms(1);
                  MS1030_Flow();
                  Delay_us(100);
                  MS1030_Temper();
                  Delay_us(100);
                  ACLK_OFF;
                  StartTof_flag = 0; 
    	    }
            if(Display_flag)
    	    {//1s刷一次屏
    		  Key_Progress();
    		  Display_flag = 0;
    	    }
            if((Key_flag == 0)&&(StartTof_flag == 0)&&(Display_flag == 0))
            {
                LPM3;
            }

    	
    } 
     
}




