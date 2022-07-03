#ifndef _9DB2BB1F_5CEA_4AF9_9130_2111D1BE73B5_
#define _9DB2BB1F_5CEA_4AF9_9130_2111D1BE73B5_

#include "sys.h"

enum adc_mode
{
	ADC_CONTINUE_DMA=0,	//use continue convert and dma transfer trigger by adc
	ADC_TIMER_DMA		//use timer controlled convert and dma transfer trigger by adc
};

class Adc 
{
	public:
		unsigned int mode;					//working mode for adc
		
	public:
        Adc();
        ~Adc();
		
		void init(void);
		
	private:
		void init_pin(GPIO_Module* port, unsigned int pin);
		void init_dma(void);
		void init_adc(void);
		void init_timer(void);
    public:
        void disable_timer_pwm(void);
        void enable_timer_pwm(void);
        void timer_pin_gpio(unsigned int pin_level);
        void init_timer_pin(unsigned int mode);
        void stop_timer(void);
        void start_timer(void);
};

#endif//_9DB2BB1F_5CEA_4AF9_9130_2111D1BE73B5_
