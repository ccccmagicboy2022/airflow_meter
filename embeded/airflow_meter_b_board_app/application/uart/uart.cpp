#include "uart.hpp"
#include "sys.h"

Uart::Uart()
{
    init_pin();
    init(115200);
    init_nvic();
}

Uart::~Uart()
{
    //
}

void Uart::init(unsigned int bitrate)
{
    USART_InitType USART_InitStructure;
  
    USART_InitStructure.BaudRate            = bitrate;
    USART_InitStructure.WordLength          = USART_WL_8B;
    USART_InitStructure.StopBits            = USART_STPB_1;
    USART_InitStructure.Parity              = USART_PE_NO;
    USART_InitStructure.HardwareFlowControl = USART_HFCTRL_NONE;
    USART_InitStructure.Mode                = USART_MODE_RX | USART_MODE_TX;

    USART_Init(USART3, &USART_InitStructure);
    USART_ConfigInt(USART3, USART_INT_RXDNE, ENABLE);
    USART_Enable(USART3, ENABLE);
}

void Uart::deinit(void)
{
    //
}

void Uart::init_pin(void)
{
    GPIO_InitType GPIO_InitStructure;
    
    GPIO_InitStructure.Pin        = GPIO_PIN_10;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_AF_PP;
    GPIO_InitPeripheral(GPIOB, &GPIO_InitStructure);

    GPIO_InitStructure.Pin       = GPIO_PIN_11;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_InitPeripheral(GPIOB, &GPIO_InitStructure);
}

void Uart::init_nvic(void)
{
    NVIC_InitType NVIC_InitStructure;

    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_0);

    NVIC_InitStructure.NVIC_IRQChannel            = USART3_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd         = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
    
    //handler in n32g4fr_it.c
}
