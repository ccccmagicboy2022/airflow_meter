#include "spi.hpp"
#include "sys.h"

uint8_t SPI_MASTER_Buffer_Rx[32];

Spi::Spi()
{
    init_int();
    init_pin();
    init_spi();
}

Spi::~Spi()
{
    //
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
    SPI_InitStructure.CLKPHA        = SPI_CLKPHA_FIRST_EDGE;
    SPI_InitStructure.NSS           = SPI_NSS_HARD;
    SPI_InitStructure.BaudRatePres  = SPI_BR_PRESCALER_64;   //  64/64=1MHz
    SPI_InitStructure.FirstBit      = SPI_FB_MSB;
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
}
