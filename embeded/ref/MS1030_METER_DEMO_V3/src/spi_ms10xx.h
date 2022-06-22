#ifndef _SPI_MS10XX_H_
#define _SPI_MS10XX_H_





#define SSN_1             P6OUT |= BIT4; //P2.4=1
#define SSN_0             P6OUT &= (uint8_t)~BIT4;//P2.4=0

#define SCK_1             P6OUT |= BIT3; //P2.5=1
#define SCK_0             P6OUT &= (uint8_t)~BIT3;//P2.5=0

#define SI_1              P6OUT |= BIT2; //P2.6=1
#define SI_0              P6OUT &= (uint8_t)~BIT2;//P2.6=0

#define SO_0_1		  (P6IN&BIT0)		//端口电平查询

#define RST_1             P4OUT |= BIT0; 
#define RST_0             P4OUT &= (uint8_t)~BIT0;




#define start_1             P1OUT |= BIT5; 
#define start_0             P1OUT &= (uint8_t)~BIT5;

#define ACLK_ON            P1SEL |= BIT5;
#define ACLK_OFF           P1SEL &= (uint8_t)~BIT5;

extern void SPI_WRITE8(uint8_t wbuf8);

extern void SPI_WriteData(uint8_t *WriteData,uint8_t WriteCnt);
extern void SPI_ReadData(uint8_t *ReadData,uint8_t ReadCnt);
extern void Write_Reg(uint8_t RegNum,uint32_t RegData);
extern void Write_test(uint8_t RegNum,uint32_t RegData);

extern uint32_t Read_Reg(uint8_t RegNum);
extern uint16_t Read_PW_First(void);
extern uint16_t Read_PW_STOP1(void);
extern uint8_t Read_REG0_L(void);
extern uint16_t Read_STAT(void);
extern uint8_t MS1030_Config(void);
extern void Write_Order(uint8_t Order,uint8_t Order_Num);
extern uint32_t MS1030_Flow(void);
extern void MS1030_Temper(void);
extern uint32_t data_average(uint32_t *dtatzz,uint8_t num);
extern void MS1030_Time_check(void);


#define INIT                     0X01
#define POWER_ON_RESET           0X02
#define START_TOF                0X03
#define START_TEMP               0X04
#define START_CAL_TDC            0X06
#define START_TOF_RESTART        0X07


#define Init()                   Write_Order(0X70,INIT)
#define Power_On_Reset()         Write_Order(0X50,POWER_ON_RESET)         
#define Start_TOF_UP()           Write_Order(0X01,START_TOF)
#define Start_TOF_UP_DOWN()      Write_Order(0X03,START_TOF)
#define Start_Temp()             Write_Order(0X04,START_TEMP)
#define START_TEMP_RESTART()     Write_Order(0X05,START_TEMP)
#define START_CAL_RESONATOR()    Write_Order(0X06,START_CAL_TDC)

#endif