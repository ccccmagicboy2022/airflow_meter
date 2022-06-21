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

uint8_t METER_ADDR[7] = {0X00,0X00,0X00,0X00,0X00,0X00,0X01};


Com_Type     USART_Data_Pack;          //USART通讯数据包变量
Com_Type     *USART_Data_Ptr;

/*******************************************
  数据表达格式 
*******************************************/
uint8_t Thermal_Power[5] = {0};          //热功率
uint8_t Current_Heat[5]  = {0};          //当前热量
uint8_t Flow[5] = {0};                   //流量
uint8_t Cur_Acc_Flow[5] = {0};           //当前累积流量
uint8_t Settlement_Heat[5] = {0};        //结算日热量
uint8_t Settlement_Acc_Heat[5] = {0};    //结算日累积流量
uint8_t Acc_Work_Time[3] = {0};          //累积工作时间
uint8_t Water_Supply_Temp[3] = {0};      //供水温度
uint8_t Water_Back_Temp[3] = {0};        //回水温度
uint8_t Real_Time[7] = {0};              //实时时间
/*uint8_t SettlementDate = 0;              //结算日期
uint8_t MeterReadDate = 0;               //抄表日期*/
//uint8_t Ser = 0;                         //序列号
uint8_t Ver = 0;                         //版本号
//uint8_t Sec[8] = {0};                    //秘钥
//uint8_t Ser_Num = 0;                     //购买序号
uint8_t Data_ID[2] = {0};                //数据标识ID
//uint8_t Figure[4] = {0};                 //金额
/*uint8_t Price1[3] = {0};                 //价格1
uint8_t Price2[3] = {0};                 //价格2
uint8_t Price3[3] = {0};                 //价格3*/
/*uint8_t Dosage1[3] = {0};                //用量1
uint8_t Dosage2[3] = {0};                //用量2*/
uint8_t State[2] = {0};                  //状态
//uint8_t Sec_V = 0;                      //秘钥版本
uint8_t Valve_State = 0;                 //阀门状态


/*************main***************************/
uint8_t readtemp = 0;

uint8_t read_reg_1 = 0;
/*************init***************************/

/*************spi_ms10xx***************************/
uint8_t IDx[7]={0,0,0,0,0,0,0};
uint32_t REG0=0X06420901;
uint32_t Reg0 = 0;
uint32_t REG1=0X20444001;
uint32_t Reg1 = 0;
uint8_t  pw1st = 0;
uint8_t  MS1022_STATE = 0;



uint16_t Result_STAT = 0;
uint8_t  StartTof_flag = 0;

uint32_t TimeValue ;  //简易显示用
uint16_t TimeValue_int;
uint16_t TimeValue_dec;
/*************i2c***************************/

/*************uart***************************/
uint8_t UartByteTemp = 0;
uint8_t UartCnt = 0;
uint8_t UartRcv_flag = 0;
uint8_t UartSendData[77]={0};
/*************lcd_key***************************/
//uint8_t LcdNumber[16] = {0x7b,0x11,0xf8,0xf1,0xa3,0xd3,0xdb,0x31,0xfb,0xf3,};
uint8_t LcdNumber1[16] = {0xFA,0x60,0xBC,0xF4,0x66,0xD6,0xDE,0x70,0xFE,0xF6};

uint8_t Key_flag = 0;  //key按键按下标志
uint8_t Intn_flag = 0; //intn中断标志
uint8_t KeyCnt = 0;    //按键计数
uint8_t Display_flag = 0;//显示标志
/*************it***************************/
uint16_t TimeCnt = 0;
uint16_t DisplayCnt = 0;