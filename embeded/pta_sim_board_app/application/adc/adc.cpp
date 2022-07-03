#include "adc.hpp"
#include "sys.h"
#include "sys.hpp"

extern volatile uint16_t g_radar_if_adc_value;     //radar if adc value
extern volatile uint16_t g_light_adc_value;		//light sensor adc value

void Adc::init(void)
{
	init_pin(GPIOC, GPIO_PIN_4);
	init_pin(GPIOA, GPIO_PIN_6);
	
	init_dma();
	init_adc();
    init_timer_pin(3);//for pwm cc output
	init_timer();
    enable_timer_pwm();
    start_timer();
}

void Adc::init_pin(GPIO_Module* port, unsigned int pin)
{
	GPIO_InitType GPIO_InitStructure;
	
	GPIO_InitStructure.Pin       = pin;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_InitPeripheral(port, &GPIO_InitStructure);	
}

void Adc::init_dma(void)
{
    DMA_InitType DMA_InitStructure;
	
    DMA_DeInit(DMA1_CH1);
    DMA_InitStructure.PeriphAddr     = (uint32_t)&ADC1->DAT;
    DMA_InitStructure.MemAddr        = (uint32_t)&g_light_adc_value;
    DMA_InitStructure.Direction      = DMA_DIR_PERIPH_SRC;
    DMA_InitStructure.BufSize        = 1;
    DMA_InitStructure.PeriphInc      = DMA_PERIPH_INC_DISABLE;
    DMA_InitStructure.DMA_MemoryInc  = DMA_MEM_INC_DISABLE;
    DMA_InitStructure.PeriphDataSize = DMA_PERIPH_DATA_SIZE_HALFWORD;
    DMA_InitStructure.MemDataSize    = DMA_MemoryDataSize_HalfWord;
    DMA_InitStructure.CircularMode   = DMA_MODE_CIRCULAR;
    DMA_InitStructure.Priority       = DMA_PRIORITY_VERY_HIGH;
    DMA_InitStructure.Mem2Mem        = DMA_M2M_DISABLE;
	
    DMA_Init(DMA1_CH1, &DMA_InitStructure);
	DMA_EnableChannel(DMA1_CH1, ENABLE);	//for adc1
    
    DMA_DeInit(DMA1_CH8);
    DMA_InitStructure.PeriphAddr     = (uint32_t)&ADC2->DAT;
    DMA_InitStructure.MemAddr        = (uint32_t)&g_radar_if_adc_value;
    DMA_InitStructure.Direction      = DMA_DIR_PERIPH_SRC;
    DMA_InitStructure.BufSize        = 1;
    DMA_InitStructure.PeriphInc      = DMA_PERIPH_INC_DISABLE;
    DMA_InitStructure.DMA_MemoryInc  = DMA_MEM_INC_DISABLE;
    DMA_InitStructure.PeriphDataSize = DMA_PERIPH_DATA_SIZE_HALFWORD;
    DMA_InitStructure.MemDataSize    = DMA_MemoryDataSize_HalfWord;
    DMA_InitStructure.CircularMode   = DMA_MODE_CIRCULAR;
    DMA_InitStructure.Priority       = DMA_PRIORITY_LOW;
    DMA_InitStructure.Mem2Mem        = DMA_M2M_DISABLE;
	
    DMA_Init(DMA1_CH8, &DMA_InitStructure);
    DMA_EnableChannel(DMA1_CH8, ENABLE);	//for adc2
}

void Adc::init_adc(void)
{
    ADC_InitType ADC_InitStructure;
	
    ADC_InitStructure.WorkMode       = ADC_WORKMODE_INDEPENDENT;
    ADC_InitStructure.MultiChEn      = ENABLE;
    ADC_InitStructure.ContinueConvEn = ENABLE;
    ADC_InitStructure.ExtTrigSelect  = ADC_EXT_TRIGCONV_NONE;
    ADC_InitStructure.DatAlign       = ADC_DAT_ALIGN_R;
    ADC_InitStructure.ChsNumber      = 1;
    
	ADC_Init(ADC1, &ADC_InitStructure);
    ADC_ConfigRegularChannel(ADC1, ADC1_Channel_03_PA6, 1, ADC_SAMP_TIME_55CYCLES5);

    ADC_EnableDMA(ADC1, ENABLE);
    ADC_Enable(ADC1, ENABLE);

    while(ADC_GetFlagStatusNew(ADC1,ADC_FLAG_RDY) == RESET)
        ;
    ADC_StartCalibration(ADC1);
    while (ADC_GetCalibrationStatus(ADC1))
        ;
    
    ADC_EnableSoftwareStartConv(ADC1, ENABLE);
	
////////////////////////////////////////////////////////////
    ADC_InitStructure.WorkMode       = ADC_WORKMODE_INDEPENDENT;
    ADC_InitStructure.MultiChEn      = ENABLE;
    ADC_InitStructure.ContinueConvEn = ENABLE;
    ADC_InitStructure.ExtTrigSelect  = ADC_EXT_TRIGCONV_NONE;
    ADC_InitStructure.DatAlign       = ADC_DAT_ALIGN_R;
    ADC_InitStructure.ChsNumber      = 1;
    
	ADC_Init(ADC2, &ADC_InitStructure);
    ADC_ConfigRegularChannel(ADC2, ADC2_Channel_05_PC4, 1, ADC_SAMP_TIME_55CYCLES5);

    ADC_EnableDMA(ADC2, ENABLE);
    ADC_Enable(ADC2, ENABLE);

    while(ADC_GetFlagStatusNew(ADC2,ADC_FLAG_RDY) == RESET)
        ;
    ADC_StartCalibration(ADC2);
    while (ADC_GetCalibrationStatus(ADC2))
        ;
    
    ADC_EnableSoftwareStartConv(ADC2, ENABLE);
}

void Adc::init_timer(void)
{
    unsigned int fs = 0;
    unsigned int wave_freq = 0;
    
	//init interrupt
    NVIC_InitType NVIC_InitStructure;

    NVIC_InitStructure.NVIC_IRQChannel                   = TIM1_CC_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority        = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd                = ENABLE;

    NVIC_Init(&NVIC_InitStructure);
    
	//init timer
    TIM_TimeBaseInitType TIM_TimeBaseStructure;
    OCInitType TIM_OCInitStructure;

    /* Time base configuration */
    TIM_TimeBaseStructure.Period    = 1000 - 1;
    TIM_TimeBaseStructure.Prescaler = 72-1; //36 for 2k wave out, 72 for 1k wave out
    TIM_TimeBaseStructure.ClkDiv    = TIM_CLK_DIV1; //144MHz
    TIM_TimeBaseStructure.CntMode   = TIM_CNT_MODE_UP;

    TIM_InitTimeBase(TIM1, &TIM_TimeBaseStructure);

    /* Output Compare Active Mode configuration: Channel3 */
    TIM_OCInitStructure.OcMode      = TIM_OCMODE_TOGGLE;
    TIM_OCInitStructure.OutputState = TIM_OUTPUT_STATE_ENABLE;
    TIM_OCInitStructure.Pulse       = 250 - 1;
    TIM_OCInitStructure.OcPolarity  = TIM_OC_POLARITY_HIGH;

    TIM_InitOc3(TIM1, &TIM_OCInitStructure);
    TIM_ConfigOc3Preload(TIM1, TIM_OC_PRE_LOAD_DISABLE);

    /* Output Compare Active Mode configuration: Channel4 */
    TIM_OCInitStructure.OcMode      = TIM_OCMODE_TOGGLE;
    TIM_OCInitStructure.OutputState = TIM_OUTPUT_STATE_ENABLE;
    TIM_OCInitStructure.Pulse       = 750 - 1;
    TIM_OCInitStructure.OcPolarity  = TIM_OC_POLARITY_HIGH;

    TIM_InitOc4(TIM1, &TIM_OCInitStructure);
    TIM_ConfigOc4Preload(TIM1, TIM_OC_PRE_LOAD_DISABLE);

    //////////////////////////////////////////////////////
    TIM_ConfigArPreload(TIM1, ENABLE);
    TIM_ConfigInt(TIM1, TIM_INT_CC4, ENABLE);       //use CC4 irq
}

void Adc::enable_timer_pwm(void)
{
    init_timer_pin(3);
    TIM_EnableCtrlPwmOutputs(TIM1, ENABLE);
}

void Adc::disable_timer_pwm(void)
{
    TIM_EnableCtrlPwmOutputs(TIM1, DISABLE);
}

void Adc::init_timer_pin(unsigned int mode)
{
	//init pin
    GPIO_InitType GPIO_InitStructure;
 
    switch (mode)
    {
        case 0:
            GPIO_InitStructure.Pin        = GPIO_PIN_10;        //TIM1_CH3
            GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_Out_PP;   //gpio use
            GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
            GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
            GPIO_ResetBits(GPIOA, GPIO_PIN_10);   //low
            break;
        case 1:
            GPIO_InitStructure.Pin        = GPIO_PIN_10;        //TIM1_CH3
            GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_Out_PP;   //gpio use
            GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
            GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
            GPIO_SetBits(GPIOA, GPIO_PIN_10);  //high
            break;
        case 2:
            GPIO_InitStructure.Pin        = GPIO_PIN_10;        //TIM1_CH3
            GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_Out_OD;   //gpio use
            GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
            GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
            break;
        case 3:
            GPIO_InitStructure.Pin        = GPIO_PIN_10;        //TIM1_CH3
            GPIO_InitStructure.GPIO_Mode  = GPIO_Mode_AF_PP;    //not gpio use
            GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
            GPIO_InitPeripheral(GPIOA, &GPIO_InitStructure);
            break;
        default:
            break;
    }
    
    if (GPIO_ReadInputDataBit(GPIOA, GPIO_PIN_10) == Bit_RESET)
	{
		CV_LOG("[%s] - PA10 = 0\r\n", __func__);
	}
    else
    {
        CV_LOG("[%s] - PA10 = 1\r\n", __func__);
    }
}

void Adc::stop_timer(void)
{
    //
    TIM_Enable(TIM1, DISABLE);
}

void Adc::start_timer(void)
{
    //
    TIM_Enable(TIM1, ENABLE);
}

Adc::Adc()
{
    init();
}

Adc::~Adc()
{
    //
}


