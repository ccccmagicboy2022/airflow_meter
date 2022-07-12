#ifndef _D112205B_D20C_4521_BC21_7E1E7A1955E5_
#define _D112205B_D20C_4521_BC21_7E1E7A1955E5_

#include "clk.hpp"
#include "uart.hpp"
#include "dac.hpp"

class Hardware {
	public:
        Clk rcc;
        Uart uart3;		//serial to usb
        Dac dac_ch1;
		
	public:
        Hardware();
        ~Hardware();
};

#endif//_D112205B_D20C_4521_BC21_7E1E7A1955E5_


