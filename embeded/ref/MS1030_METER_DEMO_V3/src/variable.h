#ifndef _VARIABLE_H_
#define _VARIABLE_H_

/*!< Signed integer types  */
typedef   signed char     int8_t;
typedef   signed short    int16_t;
typedef   signed long     int32_t;

/*!< Unsigned integer types  */
typedef unsigned char     uint8_t;
typedef unsigned int      uint16_t;
typedef unsigned long     uint32_t;

extern  uint8_t METER_ADDR[7];
extern  uint8_t uartcntback;

extern uint16_t time_up; //调试用

extern uint32_t TimeValue_down_up_dec;
extern uint32_t TimeValue_down_up_int;
extern uint32_t Q_TimeValue_down_up_dec;

extern uint32_t TimeValue_up_dec;
extern uint32_t TimeValue_up_int;

extern uint32_t TimeValue_down_dec;
extern uint32_t TimeValue_down_int;


extern uint32_t temper_red;
extern uint32_t temper_bule;

extern uint8_t rx_flag;

/********** 通讯数据类型定义 ************/
typedef struct
{
    unsigned char Com_Time;                // 通讯允许最长时间
    unsigned char Com_Ptr;                // 多字节计数指针
    unsigned char Buffer[50];             // 收发缓冲区
}Com_Type;
extern Com_Type         USART_Data_Pack;          //USART通讯数据包变量
extern Com_Type         *USART_Data_Ptr;
/*************main.c*************************/
extern uint8_t readtemp;


extern uint8_t read_reg_1;
/*************spi_ms10xx.c*************************/
extern uint8_t IDx[7];
extern uint32_t REG0;
extern uint32_t Reg0;
extern uint32_t REG1;
extern uint32_t Reg1;
extern uint8_t  pw1st;
extern uint8_t  MS1022_STATE;

extern uint32_t Result_Reg0;
extern uint32_t Result_Reg1;
extern uint32_t Result_Reg2;
extern uint32_t Result_Reg3;
extern uint32_t Result_Reg4;
extern uint32_t Result_Reg5;
extern uint32_t Result_Reg6;
extern uint32_t Result_Reg7;
extern uint32_t Result_Reg8;

extern uint32_t Result_Reg0_0;
extern uint32_t Result_Reg1_1;
extern uint32_t Result_Reg2_2;
extern uint32_t Result_Reg3_3;
extern uint32_t Result_Reg4_4;
extern uint32_t Result_Reg5_5;
extern uint32_t Result_Reg6_6;
extern uint32_t Result_Reg7_7;
extern uint32_t Result_Reg8_8;

extern uint32_t time_up_result;
extern uint32_t time_down_result;
extern uint32_t time_up_down_result;

extern uint32_t time_check_temp;
extern uint8_t  SPI_check_temp;
extern uint16_t Result_PW_First;
extern uint16_t Result_PW_STOP1;

extern uint16_t Result_STAT;
extern uint8_t  StartTof_flag;

extern uint32_t TimeValue;  //简易显示用
extern uint16_t TimeValue_int;
extern uint16_t TimeValue_dec;

extern uint32_t liuliang;
extern uint32_t liusu;
extern uint32_t liusu_temp1;

/*************i2c.c*************************/



/*************lcd_key.c*************************/
//extern uint8_t LcdNumber[16];
extern uint8_t LcdNumber1[16];

extern uint8_t Key_flag;  //key按键按下标志
extern uint8_t Intn_flag; //intn中断标志
extern uint8_t KeyCnt;    //按键技术
extern uint8_t Display_flag;
/*************it***************************/
extern uint16_t TimeCnt;
extern uint16_t DisplayCnt;


#endif