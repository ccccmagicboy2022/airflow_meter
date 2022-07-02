#include "clk.hpp"
#include "sys.h"

Clk::Clk()
{
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOB, ENABLE);
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOC, ENABLE);
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_USART3, ENABLE);
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_AFIO, ENABLE);
        
    RCC_EnableAHBPeriphClk(RCC_AHB_PERIPH_ADC1, ENABLE);
    RCC_EnableAHBPeriphClk(RCC_AHB_PERIPH_ADC2, ENABLE);
    RCC_EnableAHBPeriphClk(RCC_AHB_PERIPH_DMA1, ENABLE);
    
    RCC_ConfigPclk1(RCC_HCLK_DIV4);     //APB1 CLK      144/4=36MHz(max p86)
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_TIM1, ENABLE);      //timer1 72*2=144MHz
    RCC_EnableAPB2PeriphClk(RCC_APB2_PERIPH_GPIOA, ENABLE);     //PA10
    
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_TIM2, ENABLE);      //timer2 72MHz not use
    RCC_EnableAPB1PeriphClk(RCC_APB1_PERIPH_TIM3, ENABLE);      //not use

    /* RCC_ADCHCLK_DIV16*/
    ADC_ConfigClk(ADC_CTRL3_CKMOD_AHB,RCC_ADCHCLK_DIV16);   // 144/16 = 9M
    RCC_ConfigAdc1mClk(RCC_ADC1MCLK_SRC_HSE, RCC_ADC1MCLK_DIV8);  //  8/8  = 1M
    
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
    CV_LOG("adc_hclk: %d\r\n", RCC_ClockFreq.AdcHclkFreq);
}
