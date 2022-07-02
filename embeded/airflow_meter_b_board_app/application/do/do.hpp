#ifndef _4BCABF14_3681_4796_A8EC_94EA4B79D6BB_
#define _4BCABF14_3681_4796_A8EC_94EA4B79D6BB_

#include "sys.h"

class Do {
	public:
        GPIO_Module* m_port;
        uint16_t     m_pin;
		
	public:
        Do();
        Do(GPIO_Module* port, uint16_t pin);
        ~Do();
    
    public:
        void init_pin(GPIO_Module* port, uint16_t pin);
        void low(void);
        void high(void);
        void toggle(void);
};



#endif//_4BCABF14_3681_4796_A8EC_94EA4B79D6BB_


