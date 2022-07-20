#include "clk.hpp"
#include "sys.h"

Clk::Clk()
{
    RCC_ConfigPclk1(RCC_HCLK_DIV4);     //APB1 CLK      128/4=32MHz(max p86)
    RCC_ConfigPclk2(RCC_HCLK_DIV2);     //APB2 CLK      128/2=64MHz
    
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOA, ENABLE);    
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOB, ENABLE);
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOC, ENABLE);
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_USART3, ENABLE);
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_AFIO, ENABLE);
            
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_TIM1, ENABLE);      //timer1 64*2=128MHz not use
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_TIM2, ENABLE);      //timer2 64MHz not use
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_TIM3, ENABLE);      //not use
    
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_SPI1, ENABLE);      //SPI1
        
    print_clock();
}

Clk::~Clk()
{
    //
}

void Clk::print_clock(void)
{
    RCC_ClocksType RCC_ClockFreq;
    
    RCC_GetClocksFreqValue(&RCC_ClockFreq);
    
    CV_LOG("sysclk: %d\r\n", RCC_ClockFreq.SysclkFreq);
    CV_LOG("hclk: %d\r\n", RCC_ClockFreq.HclkFreq);
    CV_LOG("pclk1: %d\r\n", RCC_ClockFreq.Pclk1Freq);
    CV_LOG("pclk2: %d\r\n", RCC_ClockFreq.Pclk2Freq);
    //CV_LOG("adc_hclk: %d\r\n", RCC_ClockFreq.AdcHclkFreq);
}
