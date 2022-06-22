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


/*****************************************************************
函数名：
入口参数：
参数说明: LCDMX缓存，data显示的转义数据
出口参数：
参数说明：
功能:  往对应的LCD缓存中放数据，用来显示数据
代码编制：
******************************************************************/
void LCDdisplay_Number(int LCDMX,uint8_t data)
{
	switch(LCDMX)
	{	
		case 1:
		       LCDM1=data;
		break;
		case 2:
			LCDM2=data;
		break;
		case 3:
			LCDM3=data;
		break;
		case 4:
			LCDM4=data;
		break;
		case 5:
			LCDM5=data;
		break;
		case 6:
			LCDM6=data;
		break;
		case 7:
			LCDM7=data;
		break;
		case 8:
			LCDM8=data;
		break;
		case 9:
		        LCDM9=data;
		case 10:
		       LCDM10=data;
		default:
		 break;
		}		
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  LCD全屏清空不显示
代码编制：
******************************************************************/
void Empty_AllSeg(void)
{
	unsigned char i;
	for(i=0; i<10; i++)
	{
		LCDMEM[i] = 0;
	}
}


/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  LCD全屏显示
代码编制：
******************************************************************/
void Set_AllSeg(void)
{
	unsigned char i;
	for(i=0; i<10; i++)
	{
		LCDMEM[i] = 0xff;
	}
}



/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  LCD上显示1,2,3,4,5,6,7,8
代码编制：
******************************************************************/
void LCDdisplay_Series(void)
{
	LCDdisplay_Number(1,LcdNumber1[1]);
	LCDdisplay_Number(2,LcdNumber1[2]);
	LCDdisplay_Number(3,LcdNumber1[3]);
	LCDdisplay_Number(4,LcdNumber1[4]);
	LCDdisplay_Number(5,LcdNumber1[5]);
	LCDdisplay_Number(6,LcdNumber1[6]);
	LCDdisplay_Number(7,LcdNumber1[7]);
	LCDdisplay_Number(8,LcdNumber1[8]);  
}
/*****************************************************************
函数名：
入口参数：DisplayNum根据液晶LCD转义过后的数据，如0对应0xed
参数说明: 
出口参数：
参数说明：
功能:  显示数据0，1，2.......
代码编制：
******************************************************************/
void Key_Display(uint8_t DisplayNum)
{
	LCDdisplay_Number(8,DisplayNum);
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示功率
代码编制：
******************************************************************/
void Power_Display(void)
{
    LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
    LCDdisplay_Number(6,LcdNumber1[0]);
    LCDdisplay_Number(7,LcdNumber1[0]);
    LCDdisplay_Number(8,LcdNumber1[0]|0x02);
    LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示顺流时间
代码编制：
******************************************************************/
void Time_Display_up(void)
{
  
        LCDdisplay_Number(1,0x68);
        if(((uint8_t)(time_up_result/100000000%10))==0)
        {
           LCDdisplay_Number(2,0x00); 
           if(((uint8_t)(time_up_result/10000000%10))==0)
           {
                LCDdisplay_Number(3,0x00); 
            }
            else
            {
                LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_up_result/10000000%10)]); 
            } 
        }
        else
        {
           LCDdisplay_Number(2,LcdNumber1[(uint8_t)(time_up_result/100000000%10)]); 
           LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_up_result/10000000%10)]);
        }
        LCDdisplay_Number(4,LcdNumber1[((uint8_t)(time_up_result/1000000%10))]|0x80);
        LCDdisplay_Number(5,LcdNumber1[(uint8_t)(time_up_result/100000%10)]);
        LCDdisplay_Number(6,LcdNumber1[(uint8_t)(time_up_result/10000%10)]);
        LCDdisplay_Number(7,LcdNumber1[(uint8_t)(time_up_result/1000%10)]);
        LCDdisplay_Number(8,LcdNumber1[(uint8_t)(time_up_result/100%10)]);
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x00);
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示逆流时间
代码编制：
******************************************************************/
void Time_Display_down(void)
{
        LCDdisplay_Number(1,0x0d);
        if(((uint8_t)(time_down_result/100000000%10))==0)
        {
           LCDdisplay_Number(2,0x00); 
           if(((uint8_t)(time_down_result/10000000%10))==0)
           {
                LCDdisplay_Number(3,0x00); 
            }
            else
            {
                LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_down_result/10000000%10)]); 
            } 
        }
        else
        {
           LCDdisplay_Number(2,LcdNumber1[(uint8_t)(time_down_result/100000000%10)]); 
           LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_down_result/10000000%10)]);
        }
        LCDdisplay_Number(4,LcdNumber1[((uint8_t)(time_down_result/1000000%10))]|0x80);
        LCDdisplay_Number(5,LcdNumber1[(uint8_t)(time_down_result/100000%10)]);
        LCDdisplay_Number(6,LcdNumber1[(uint8_t)(time_down_result/10000%10)]);
        LCDdisplay_Number(7,LcdNumber1[(uint8_t)(time_down_result/1000%10)]);
        LCDdisplay_Number(8,LcdNumber1[(uint8_t)(time_down_result/100%10)]);
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x00);
}


/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示时间差值
代码编制：
******************************************************************/
void Time_Display_diff(void)
{
        uint8_t LCD_flg=0;
        LCDdisplay_Number(1,0x78);
        
        if(((uint8_t)(time_up_down_result/1000000%10))==0)
        {
           LCDdisplay_Number(2,0x00); 
           LCD_flg=1;
           
        }
        else
        {
           LCDdisplay_Number(2,LcdNumber1[(uint8_t)(time_up_down_result/1000000%10)]); 
           LCD_flg=0;
        }
        
        if(LCD_flg==1)
        {
           if(((uint8_t)(time_up_down_result/100000%10))==0)
           {
                LCDdisplay_Number(3,0x00); 
                LCD_flg=1;
           }
           else
           {
                LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_up_down_result/100000%10)]); 
                LCD_flg=0;
           } 
        }
        else
        {
            LCDdisplay_Number(3,LcdNumber1[(uint8_t)(time_up_down_result/100000%10)]);
            LCD_flg=0;
        }
        
        if(LCD_flg==1)
        {
           if(((uint8_t)(time_up_down_result/10000%10))==0)
           {
                LCDdisplay_Number(4,0x00); 
                LCD_flg=1;
           }
           else
           {
                LCDdisplay_Number(4,LcdNumber1[(uint8_t)(time_up_down_result/10000%10)]); 
                LCD_flg=0;
           } 
        }
        else
        {
            LCDdisplay_Number(4,LcdNumber1[(uint8_t)(time_up_down_result/10000%10)]);
            LCD_flg=0;
        }
        LCDdisplay_Number(5,LcdNumber1[(uint8_t)(time_up_down_result/1000%10)]|0x80);
        LCDdisplay_Number(6,LcdNumber1[(uint8_t)(time_up_down_result/100%10)]);
        LCDdisplay_Number(7,LcdNumber1[(uint8_t)(time_up_down_result/10%10)]);
        LCDdisplay_Number(8,LcdNumber1[(uint8_t)(time_up_down_result/1%10)]);	
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x00);
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示流速
代码编制：
******************************************************************/
void FLOW_Rate_Display(void)
{

        LCDdisplay_Number(1,0x80);
        LCDdisplay_Number(2,0x00);
    	LCDdisplay_Number(3,LcdNumber1[TimeValue_down_up_int%10]);
        LCDdisplay_Number(4,LcdNumber1[Q_TimeValue_down_up_dec/100000%10]);
        LCDdisplay_Number(5,LcdNumber1[Q_TimeValue_down_up_dec/10000%10]);
        LCDdisplay_Number(6,LcdNumber1[Q_TimeValue_down_up_dec/1000%10]);
        LCDdisplay_Number(7,LcdNumber1[Q_TimeValue_down_up_dec/100%10]|0x01);
        LCDdisplay_Number(8,LcdNumber1[Q_TimeValue_down_up_dec/10%10]);	
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x00);
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示报警
代码编制：
******************************************************************/
void Alarm_Display(void)
{
     LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
	LCDdisplay_Number(6,LcdNumber1[0]|0x02);
	LCDdisplay_Number(7,LcdNumber1[0]);
	LCDdisplay_Number(8,LcdNumber1[0]);
	LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示T入、T出
代码编制：
******************************************************************/
void Tin_Tout_Display(void)
{
    LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
    LCDdisplay_Number(5,0x02);
    LCDdisplay_Number(6,LcdNumber1[0]);
    LCDdisplay_Number(7,LcdNumber1[0]);
    LCDdisplay_Number(8,LcdNumber1[0]);	
    LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示T入
代码编制：
******************************************************************/
void Blue_Display(void)
{
        LCDdisplay_Number(1,0x1d);
    	LCDdisplay_Number(2,LcdNumber1[temper_bule/100000%10]);//0
        LCDdisplay_Number(3,LcdNumber1[temper_bule/10000%10]);//0
        LCDdisplay_Number(4,LcdNumber1[temper_bule/1000%10]);//千位
        LCDdisplay_Number(5,LcdNumber1[temper_bule/100%10]|0x80);//百位
        LCDdisplay_Number(6,LcdNumber1[temper_bule/10%10]);//十位
        LCDdisplay_Number(7,LcdNumber1[temper_bule%10]);//个位
	LCDdisplay_Number(8,0x00);
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x40);
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示T出
代码编制：
******************************************************************/
void Red_Display(void)
{
        LCDdisplay_Number(1,0x42);
    	LCDdisplay_Number(2,LcdNumber1[temper_red/100000%10]);
        LCDdisplay_Number(3,LcdNumber1[temper_red/10000%10]);
        LCDdisplay_Number(4,LcdNumber1[temper_red/1000%10]);
        LCDdisplay_Number(5,LcdNumber1[temper_red/100%10]|0x80);
        LCDdisplay_Number(6,LcdNumber1[temper_red/10%10]);
        LCDdisplay_Number(7,LcdNumber1[temper_red%10]);	
        LCDdisplay_Number(8,0x00);
	LCDdisplay_Number(9,0x00);
	LCDdisplay_Number(10,0x80);
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示流量
代码编制：
******************************************************************/
void Flow_Display(void)
{
    LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
    LCDdisplay_Number(4,0x02);
	LCDdisplay_Number(6,LcdNumber1[0]);
	LCDdisplay_Number(7,LcdNumber1[0]);
	LCDdisplay_Number(8,LcdNumber1[0]);	
	LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);	
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示问拆
代码编制：
******************************************************************/
void Temperature_Display(void)
{
    LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x02);    
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
	LCDdisplay_Number(6,LcdNumber1[0]);
	LCDdisplay_Number(7,LcdNumber1[0]);
	LCDdisplay_Number(8,LcdNumber1[0]);	
	LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);	
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示冷量
代码编制：
******************************************************************/
void Cold_Display(void)
{
    LCDdisplay_Number(1,0x00);
    LCDdisplay_Number(2,0x02);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
    LCDdisplay_Number(6,LcdNumber1[0]);
    LCDdisplay_Number(7,LcdNumber1[0]);
    LCDdisplay_Number(8,LcdNumber1[0]);	
    LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);	
}
/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  显示热量
代码编制：
******************************************************************/
void Hot_Display(void)
{
    LCDdisplay_Number(1,0x02);
    LCDdisplay_Number(2,0x00);
    LCDdisplay_Number(3,0x00);
    LCDdisplay_Number(4,0x00);
    LCDdisplay_Number(5,0x00);
    LCDdisplay_Number(6,LcdNumber1[0]);
    LCDdisplay_Number(7,LcdNumber1[0]);
    LCDdisplay_Number(8,LcdNumber1[0]);	
    LCDdisplay_Number(9,0x00);
    LCDdisplay_Number(10,0x00);	
}

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  根据按键号来显示数据，如按键号为1则显示数据1
代码编制：
******************************************************************/

/*****************************************************************
函数名：
入口参数：
参数说明: 
出口参数：
参数说明：
功能:  根据按键号来显示数据，如按键号为1则显示数据1
代码编制：
******************************************************************/
void Key_Progress(void)
{
	switch(KeyCnt)
	{
		case 1:
		    Set_AllSeg();
		break;
               
		case 2://顺流时间
		    Time_Display_up();
		break;
                case 3://逆流时间
		    Time_Display_down();
		break;
                case 4://时差时间
		    Time_Display_diff();
		break;
		case 5://进水
		    Red_Display();
		break;
                case 6://出水
                    Blue_Display();
		break;
                
		default:
		break;
	}
}



