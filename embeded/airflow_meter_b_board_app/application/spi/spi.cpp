#include "spi.hpp"
#include "sys.h"
#include "sys.hpp"

extern DWTDelay dwt;

uint8_t Intn_flag = 0;

Spi::Spi()
{
    init_int();
    init_pin();
    init_spi();
    init_ss_pin();
    init_rst_pin();
    init_int_pin();
}

Spi::~Spi()
{
    //
}

void Spi::init_int_pin()
{
    spi_int.init_pin(GPIOA, GPIO_PIN_9);
    spi_int.init_irq();
}

void Spi::init_rst_pin()
{
    spi_rstn.init_pin(GPIOA, GPIO_PIN_10);
    spi_rstn.high();
}

void Spi::init_pin()
{
    GPIO_InitType GPIO_InitStructure;

    GPIO_InitStructure.Pin        = GPIO_PIN_5 | GPIO_PIN_7;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_AF_PP;
    GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
    
    GPIO_InitStructure.Pin       = GPIO_PIN_6;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IPU;
    GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
    
}

void Spi::init_spi()
{
    SPI_InitType SPI_InitStructure;
    
    SPI_InitStructure.DataDirection = SPI_DIR_DOUBLELINE_FULLDUPLEX;
    SPI_InitStructure.SpiMode       = SPI_MODE_MASTER;
    SPI_InitStructure.DataLen       = SPI_DATA_SIZE_8BITS;
    SPI_InitStructure.CLKPOL        = SPI_CLKPOL_LOW;
    SPI_InitStructure.CLKPHA        = SPI_CLKPHA_SECOND_EDGE;
    SPI_InitStructure.NSS           = SPI_NSS_HARD;
    SPI_InitStructure.BaudRatePres  = SPI_BR_PRESCALER_16;   //  64/16=4MHz
    SPI_InitStructure.FirstBit      = SPI_FB_MSB;       //MSB first
    SPI_InitStructure.CRCPoly       = 7;
    
    SPI_Init(SPI1, &SPI_InitStructure);
    
    //SPI_I2S_EnableInt(SPI1, SPI_I2S_INT_TE, ENABLE);   //send irq
    //SPI_I2S_EnableInt(SPI1, SPI_I2S_INT_RNE, ENABLE);   //rev irq
    
    SPI_SSOutputEnable(SPI1, ENABLE);
    SPI_Enable(SPI1, ENABLE);
}

void Spi::init_int()
{
    NVIC_InitType NVIC_InitStructure;

    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_1);

    NVIC_InitStructure.NVIC_IRQChannel                   = SPI1_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 1;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority        = 2;
    NVIC_InitStructure.NVIC_IRQChannelCmd                = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
}

void Spi::write8(uint8_t wbuf8)
{
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, wbuf8);
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
}

uint8_t Spi::read8()
{
    uint16_t result = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result = SPI_I2S_ReceiveData(SPI1);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    return (uint8_t)result;
}

uint16_t Spi::read16()
{
    uint16_t result0 = 0x00;
    uint16_t result1 = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //byte0
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result1 = SPI_I2S_ReceiveData(SPI1); //byte1    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    return result0*0x100 + result1;
}

uint32_t Spi::read32()
{
    uint16_t result0 = 0x00;
    uint16_t result1 = 0x00;
    uint16_t result2 = 0x00;
    uint16_t result3 = 0x00;
    uint32_t result = 0x00;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA);
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //dummy
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result0 = SPI_I2S_ReceiveData(SPI1); //byte0
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result1 = SPI_I2S_ReceiveData(SPI1); //byte1    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result2 = SPI_I2S_ReceiveData(SPI1); //byte2    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    SPI_I2S_TransmitData(SPI1, 0xAA); 

    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_RNE_FLAG) == RESET)
        ;
    result3 = SPI_I2S_ReceiveData(SPI1); //byte3    
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_TE_FLAG) == RESET)
        ;
    
    while (SPI_I2S_GetStatus(SPI1, SPI_I2S_BUSY_FLAG) == SET)
        ;
    
    result = result0*0x1000000 + result1*0x10000 + result2*0x100 + result3;
    return result;
}

void Spi::init_ss_pin()
{
    spi_ss.init_pin(GPIOA, GPIO_PIN_4);
    spi_ss.high();
}

void Spi::Write_Reg(uint8_t RegNum, uint32_t RegData)
{
    spi_ss.low();
    
    write8(RegNum);
    
    write8((0xFF000000 & RegData)>>24);
    write8((0x00FF0000 & RegData)>>16);
    write8((0x0000FF00 & RegData)>>8);
    write8(0x000000FF & RegData);
    
    spi_ss.high();
}

void Spi::Write_Order(uint8_t Order)
{
    spi_ss.low();
    write8(Order);
    spi_ss.high();
}

uint8_t Spi::Read_REG0_L()
{
    uint8_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_COMM_REG);
	ReadData = read8();
    spi_ss.high();
    
	return ReadData;
}

uint16_t Spi::Read_STAT()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_STATUS_REG);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData;
}

uint32_t Spi::Read_Reg(uint8_t RegNum)
{
    uint32_t ReadData = 0;
    
    spi_ss.low();
	write8(RegNum);
	ReadData = read32();
    spi_ss.high();
    
	return ReadData;
}

uint16_t Spi::Read_PW_First()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_PW_FIRST);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData>>5;
}

uint16_t Spi::Read_PW_STOP1()
{
    uint16_t ReadData = 0;
    
    spi_ss.low();
	write8(READ_PW_STOP1);
	ReadData = read16();
    spi_ss.high();
    
	return ReadData>>5;
}

uint32_t Spi::data_average(uint32_t *dtatzz,uint8_t num) /*定义两个参数：数组首地址与数组大小*/ 
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

uint32_t Spi::MS1030_Flow(void)
{
    uint32_t time_up_down_result = 0;
    uint16_t Result_status = 0;
    uint16_t Result_PW_First = 0;
    uint16_t Result_PW_STOP1 = 0;
    uint8_t i = 0;
    uint32_t Result_up_reg[8] = {0};
    uint32_t Result_up_sum = 0;
    uint32_t Result_down_reg[8] = {0};
    uint32_t Result_down_sum = 0;
    uint32_t time_up_down_diff[8] = {0};
    
    Write_Order(INITIAL);
    
    Write_Order(START_TOF_RESTART);
    
    while(Intn_flag == 0);
    Intn_flag = 0;           //glear flag
    
    GPIOA->POD ^= GPIO_PIN_8;//blink green on board led
    
    Result_status = Read_STAT();
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    Result_PW_First = Read_PW_First();
    //CV_LOG("PW_First: 0x%04X\r\n", Result_PW_First);
    //log_info("PW_First: 0x%04X\r\n", Result_PW_First);
    
    Result_PW_STOP1=Read_PW_STOP1();
    //CV_LOG("PW_STOP1: 0x%04X\r\n", Result_PW_STOP1);
    //log_info("PW_STOP1: 0x%04X\r\n", Result_PW_STOP1);
    
    for(i=0;i<8;i++)
    {
        Result_up_reg[i] = Read_Reg(READ_TOF_UP_STOP1 + i);
    }
    Result_up_sum = Read_Reg(READ_TOF_UP_STOP1 + i);
    
    for(i=0;i<8;i++)
    {
        Result_down_reg[i] = Read_Reg(READ_TOF_DOWN_STOP1 + i);
    }
    Result_down_sum = Read_Reg(READ_TOF_DOWN_STOP1 + i);
    
    for(i=0;i<8;i++)
    {
        if(Result_up_reg[i] >= Result_down_reg[i])
        {
            time_up_down_diff[i] = Result_up_reg[i] - Result_down_reg[i];
        }
        else
        {
            time_up_down_diff[i] = Result_down_reg[i] - Result_up_reg[i];
        }
    }
    
    time_up_down_result = data_average(time_up_down_diff, 8);
    
    CV_LOG("time_up_down_result: 0x%08X\r\n", time_up_down_result);
    log_info("time_up_down_result: 0x%08X\r\n", time_up_down_result);
    
    return  time_up_down_result;
}

void Spi::MS1030_Temper(void)
{
    uint16_t Result_status = 0;
    uint32_t Result_temperature_reg[4] = {0};
    uint8_t i = 0;
    
    Write_Order(INITIAL);
    Result_status = Read_STAT();
    
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    Write_Order(START_TEMP);
    
    while(Intn_flag == 0);
    Intn_flag = 0;           //glear flag
    
    Result_status = Read_STAT();
    //CV_LOG("status: 0x%04X\r\n", Result_status);
    //log_info("status: 0x%04X\r\n", Result_status);
    
    for(i=0;i<4;i++)
    {
        Result_temperature_reg[i] = Read_Reg(READ_TEMP_PT1 + i);
        //CV_LOG("pt[%d]: 0x%08X\r\n", i+1, Result_temperature_reg[i]);
        //log_info("pt[%d]: 0x%08X\r\n", i+1, Result_temperature_reg[i]);
    }
}

void Spi::MS1030_Time_check(void)
{
    uint32_t cal_reg = 0;
    
    Write_Order(START_CAL_RESONATOR);
    
    while(Intn_flag == 0);
    dwt.delay_ms(20);
    Intn_flag = 0;           //glear flag
    
    cal_reg = Read_Reg(READ_CAL_REG);
    
    CV_LOG("cal_reg: 0x%08X\r\n", cal_reg);
    log_info("cal_reg: 0x%08X\r\n", cal_reg);
}

uint8_t Spi::config()
{
    uint32_t REG0 = 0;
    uint32_t REG1 = 0;
    uint32_t REG2 = 0;
    uint32_t REG3 = 0;
    uint32_t REG4 = 0;
    uint8_t  SPI_check_temp = 0;
        
    REG0=0x1E188930;      
    REG1=0xA00F0000; 
    REG2=0x83105187;     
    REG3=0x20928480;        
    REG4=0x47EC0500;
    
    spi_rstn.high();
    dwt.delay_us(1);
    spi_rstn.low();
    dwt.delay_ms(10);
    spi_rstn.high();
    dwt.delay_us(100);
    
    Write_Order(POR);
    
    Write_Reg(WRITE_REG0, REG0);
	Write_Reg(WRITE_REG1, REG1);
	Write_Reg(WRITE_REG2, REG2);
	Write_Reg(WRITE_REG3, REG3);
	Write_Reg(WRITE_REG4, REG4);
    
    Write_Order(INITIAL);
    
    SPI_check_temp= Read_REG0_L();
    CV_LOG("REG0_L: 0x%02x\r\n", SPI_check_temp);
    log_info("REG0_L: 0x%02x\r\n", SPI_check_temp);
    
    return SPI_check_temp;
}


