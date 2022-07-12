#include "dac.hpp"
#include "sys.h"
#include "sys.hpp"

void Dac::init(void)
{
	init_pin(GPIOA, GPIO_PIN_4);
	init_dac();
    set_dac_raw_value(162);
}

void Dac::init_pin(GPIO_Module* port, unsigned int pin)
{
	GPIO_InitType GPIO_InitStructure;
	
	GPIO_InitStructure.Pin       = pin;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AIN;
	GPIO_InitPeripheral(port, &GPIO_InitStructure);	
}

void Dac::init_dac(void)
{
    DAC_InitType DAC_InitStructure;

    DAC_InitStructure.Trigger          = DAC_TRG_SOFTWARE;
    DAC_InitStructure.WaveGen          = DAC_WAVEGEN_NONE;
    DAC_InitStructure.LfsrUnMaskTriAmp = DAC_UNMASK_LFSRBIT0;
    DAC_InitStructure.BufferOutput     = DAC_BUFFOUTPUT_ENABLE;
    DAC_Init(DAC_CHANNEL_1, &DAC_InitStructure);

    DAC_Enable(DAC_CHANNEL_1, ENABLE);
}

void Dac::set_dac_raw_value(unsigned short val)
{
    DAC_SetCh1Data(DAC_ALIGN_R_12BIT, val);
	DAC_SoftTrgEnable(DAC_CHANNEL_1, ENABLE);
}

Dac::Dac()
{
    init();
}

Dac::~Dac()
{
    //
}


