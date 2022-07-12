#include "app.hpp"
#include "sys.h"
#include "sys.hpp"

extern Hardware pta_sim;
extern DWTDelay dwt;
extern Systick tick;

App::App()
{
    m_state = SIM_WITH_DAC;
    m_next_state = SIM_WITH_DAC;
}

App::~App()
{
    //
}

void App::run(void)
{
	switch (m_state)
	{
		case	SIM_WITH_DAC:
			sim_pt100();
			break;
		case	UART_PROTOCOL:
			uart_process();
			break;
		case	IDLE:
			idle_process();
			break;
		case	ERROR_ERROR:
			error_process();
			break;
		default:
			error_process();
			break;
	}
}

void App::sim_pt100(void)
{   
    //do something here!
    
    static int i = 0;
    unsigned short val = 0;
    
    switch (i)
    {
        case    0:
            val = 162;
            break;
        case    1:
            val = 540;
            break;
        case    2:
            val = 1110;
            break;
        case    3:
            val = 1650;
            break;
        case    4:
            val = 2190;
            break;
        case    5:
            val = 2760;
            break;
        case    6:
            val = 3340;
            break;
        case    7:
            val = 3600;
            break;
        case    8:
            val = 3340;
            break;
        case    9:
            val = 3600;
            break;
        case    10:
            val = 3340;
            break;
        case    11:
            val = 3600;
            break;
        default:
            val = 3600;
            break;
    }
    
    if (0 <= i && i <=11)
    {
        pta_sim.dac_ch1.set_dac_raw_value(val);
    }

    CV_LOG("dac new output!!! - %d\r\n", i);
    
    i++;
    
    if (12 == i)
    {
        //i = 0;
    }
    
    m_state = IDLE;
    m_next_state = UART_PROTOCOL;
}

void App::error_process(void)
{
	//do some print
	CV_LOG("error!!!\r\n");
    printf("error!!!\r\n");
}

void App::uart_process(void)
{
	//
	m_state = m_next_state;
}

void App::idle_process(void)
{
    tick.delay_ms(2000);
    
	m_state = m_next_state;
    m_next_state = SIM_WITH_DAC;
}