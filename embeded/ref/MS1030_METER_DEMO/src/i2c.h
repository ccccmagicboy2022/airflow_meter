#ifndef _I2C_H_
#define _I2C_H_

#define uint8_t     unsigned char 
#define uint16_t    unsigned int

//#define     ByteAdr          (uint8_t)0x10           //数据字节地址
#define     SlaveAdr         (uint8_t)0xA0
#define     Header           (uint8_t)0xF0      //10位地址的头字节
#define     SlaveAdr10       (uint16_t)0x0100      //10位地址

//#define     AdrMode10 
//#define     DoubleDataMode

#define    SDA_PxIN          P3IN
#define    SDA_PxIN_CHECK    BIT7

#define     COM_ERROR        (uint8_t)0X00;
#define     COM_SUCCESSED    (uint8_t)0X01; 

#define     WriteEeprom      (uint8_t)0X00     //写数据到EEPROM
#define     ReadEeprom       (uint8_t)0X01     //从EEPROM读数据

#define    SCL_HIGH()        {P3OUT |= BIT6;} //P3.6 = 1
#define    SCL_LOW()         {P3OUT &= (uint8_t)~BIT6;} //P3.6 = 0

#define    SDA_HIGH()        {P3OUT |= BIT7;} //P3.7 = 1
#define    SDA_LOW()         {P3OUT &= (uint8_t)~BIT7;} //P3.7 = 0
#define    SDA_STATE()       SDA_State()
#define    SDA_IN()          {P3DIR &= (uint8_t)~BIT7;}
#define    SDA_OUT()         {P3DIR |= BIT7;}

#define    BYTE_MSB          0x80                //字节最高位标识符

/************ 布尔变量类型的定义 ************/
typedef enum
{
    RESET = 0,
    SET = !RESET
}type_Bool;



//#define  EE24C01
//#define  EE24C02
//#define  EE24C04
//#define  EE24C08
#define  EE24C16
//#define  EE24C32
//#define  EE24C64
//#define  EE24C128
//#define  EE24C256
//

#if     defined(EE24C01) 
#define BlockTotal   1  //总共块数
#define BlockSize    128  //一块大小
#define PageSize     8

#elif   defined(EE24C02) 
#define EE24C0x
#define BlockTotal   1  //总共页数
#define BlockSize    256
#define PageSize     16

#elif   defined(EE24C04) 
#define EE24C0x
#define BlockTotal    2  //总共页数
#define BlockSize     512
#define PageSize     16

#elif   defined(EE24C08) 
#define EE24C0x
#define BlockTotal    4  //总共页数
#define BlockSize     1024
#define PageSize      16

#elif   defined(EE24C16)
#define EE24C0x
#define BlockTotal    8 //总共页数
#define BlockSize     2048
#define PageSize      16

#elif   defined(EE24C32) 
#define EE24Cxx
#define DoubleDataMode
#define BlockTotal    1 //总共页数
#define BlockSize     4096
#define PageSize      32

#elif   defined(EE24C64)
#define EE24Cxx
#define DoubleDataMode
#define BlockTotal    1 //总共页数
#define BlockSize     8192
#define PageSize      32

#elif   defined(EE24C128)
#define EE24Cxx
#define DoubleDataMode
#define BlockTotal    1 //总共页数
#define BlockSize     16384
#define PageSize      64

#elif   defined(EE24C256)
#define EE24Cxx
#define DoubleDataMode
#define BlockTotal    1 //总共页数
#define BlockSize     32768
#define PageSize      64

#endif



extern void Delay_5us(void);
extern uint8_t I2C_WriteByte(uint8_t WriteByteAdr,uint8_t WriteByte);
extern uint8_t I2C_ReadByte(uint8_t ReadByteAdr,uint8_t *ReadByte);



#endif