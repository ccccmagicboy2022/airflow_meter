#include "systick.hpp"

volatile uint32_t delay;

Systick::Systick()
{
	init();
}

Systick::~Systick()
{
	//
}

void Systick::init(void)
{
    /* setup systick timer for 1000Hz interrupts */
    if (SysTick_Config(SystemCoreClock / 1000U)){
        /* capture error */
        while (1){
        }
    }
    /* configure the systick handler priority */
    NVIC_SetPriority(SysTick_IRQn, 0x00U);
}

void Systick::delay_ms(uint32_t count)
{
    delay = count;

    while(0U != delay){
    }
}


