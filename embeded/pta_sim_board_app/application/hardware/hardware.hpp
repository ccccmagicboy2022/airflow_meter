#ifndef _D112205B_D20C_4521_BC21_7E1E7A1955E5_
#define _D112205B_D20C_4521_BC21_7E1E7A1955E5_

#include "clk.hpp"
#include "adc.hpp"
#include "uart.hpp"
#include "do.hpp"

class Hardware {
	public:
        Clk rcc;
		Uart uart3;		//serial
		Do enrf;		//enable rf
		Do enop;		//enable op
		Do out;			//gpio output
		Do led;			//led output
		
	public:
        Hardware();
        ~Hardware();
};

#endif//_D112205B_D20C_4521_BC21_7E1E7A1955E5_


