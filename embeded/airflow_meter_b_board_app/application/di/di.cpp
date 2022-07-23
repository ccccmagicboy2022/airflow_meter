#include "di.hpp"
#include "sys.h"

Di::Di()
{
    //
}

Di::Di(GPIO_Module* port, uint16_t pin)
{
    m_port = port;
    m_pin  = pin;
    
    init_pin(m_port, m_pin);
}

Di::~Di()
{
    //
}

void Di::init_pin(GPIO_Module* port, uint16_t pin)
{
    GPIO_InitType GPIO_InitStructure;
    
    assert_param(IS_GPIO_ALL_PERIPH(port));
    
    if (port == GPIOA)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOA, ENABLE);
    }
    else if (port == GPIOB)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOB, ENABLE);
    }
    else if (port == GPIOC)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOC, ENABLE);
    }
    else if (port == GPIOD)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOD, ENABLE);
    }
    else if (port == GPIOE)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOE, ENABLE);
    }
    else if (port == GPIOF)
    {
        RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOF, ENABLE);
    }
    else
    {
        if (port == GPIOG)
        {
            RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOG, ENABLE);
        }
    }    
    
    if (pin <= GPIO_PIN_ALL)
    {
        GPIO_InitStructure.Pin        = pin;
        GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_IPU;
        GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
        GPIO_InitPeripheral(port, &GPIO_InitStructure);
    }
    
    m_port = port;
    m_pin  = pin;
}

void Di::init_irq(void)
{
    EXTI_InitType EXTI_InitStructure;
    NVIC_InitType NVIC_InitStructure;
    
    GPIO_ConfigEXTILine(GPIOA_PORT_SOURCE, GPIO_PIN_SOURCE9);

    EXTI_InitStructure.EXTI_Line    = EXTI_LINE9;
    EXTI_InitStructure.EXTI_Mode    = EXTI_Mode_Interrupt;
    EXTI_InitStructure.EXTI_Trigger = EXTI_Trigger_Falling;
    EXTI_InitStructure.EXTI_LineCmd = ENABLE;
    EXTI_InitPeripheral(&EXTI_InitStructure);
    
    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_0);
    
    NVIC_InitStructure.NVIC_IRQChannel                   = EXTI9_5_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority        = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd                = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
    
    //handler in n32g4fr_it.c
}

