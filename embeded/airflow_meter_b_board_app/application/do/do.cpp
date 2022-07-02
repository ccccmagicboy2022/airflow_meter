#include "do.hpp"
#include "sys.h"

Do::Do()
{
    //
}

Do::Do(GPIO_Module* port, uint16_t pin)
{
    m_port = port;
    m_pin  = pin;
    
    init_pin(m_port, m_pin);
}

Do::~Do()
{
    //
}

void Do::init_pin(GPIO_Module* port, uint16_t pin)
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
        GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
        GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_Out_PP;
        GPIO_InitPeripheral(port, &GPIO_InitStructure);
    }
    
    m_port = port;
    m_pin  = pin;
}

void Do::low(void)
{
    m_port->PBC = m_pin;
}

void Do::high(void)
{
    m_port->PBSC = m_pin;
}

void Do::toggle(void)
{
    m_port->POD ^= m_pin;
}


