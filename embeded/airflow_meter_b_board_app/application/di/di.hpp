#ifndef _93DCF035_F68F_4592_B713_B25E3BA7DCA5_
#define _93DCF035_F68F_4592_B713_B25E3BA7DCA5_

#include "sys.h"

class Di {
	public:
        GPIO_Module* m_port;
        uint16_t     m_pin;
		
	public:
        Di();
        Di(GPIO_Module* port, uint16_t pin);
        ~Di();
    
    public:
        void init_pin(GPIO_Module* port, uint16_t pin);
		void init_irq();
};



#endif//_93DCF035_F68F_4592_B713_B25E3BA7DCA5_


