#ifndef _641B28EF_04FC_4B43_832E_6906F69B6F01_
#define _641B28EF_04FC_4B43_832E_6906F69B6F01_

#include "sys.h"

class Systick {
	public:
        //
		//
		
	public:
        Systick();
        ~Systick();
    
    public:
        void init(void);
		void delay_ms(uint32_t count);
};


#endif//_641B28EF_04FC_4B43_832E_6906F69B6F01_
