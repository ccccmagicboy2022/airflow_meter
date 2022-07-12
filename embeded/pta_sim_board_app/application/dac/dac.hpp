#ifndef _3FAB9870_7B59_424B_84F0_1B29A7F40488_
#define _3FAB9870_7B59_424B_84F0_1B29A7F40488_

#include "sys.h"

class Dac 
{
	public:
		unsigned int test;
		
	public:
        Dac();
        ~Dac();
		
		void init(void);
		
	private:
		void init_pin(GPIO_Module* port, unsigned int pin);
		void init_dac(void);
		
	public:
		void set_dac_raw_value(unsigned short val);
};

#endif//_3FAB9870_7B59_424B_84F0_1B29A7F40488_


