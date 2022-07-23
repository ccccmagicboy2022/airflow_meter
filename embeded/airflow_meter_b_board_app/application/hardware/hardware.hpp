#ifndef _D112205B_D20C_4521_BC21_7E1E7A1955E5_
#define _D112205B_D20C_4521_BC21_7E1E7A1955E5_

#include "sys.hpp"
#include "clk.hpp"
#include "uart.hpp"
#include "do.hpp"
#include "spi.hpp"
#include "di.hpp"

class Hardware {
	public:
        Clk rcc;
		Uart uart3;		//serial to usb
		Do led;			//onboard green led
        Spi ms1030;     //ms1030 spi
    
	public:
        Hardware();
        ~Hardware();
};

#endif//_D112205B_D20C_4521_BC21_7E1E7A1955E5_


