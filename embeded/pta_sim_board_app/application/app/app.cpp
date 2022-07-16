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
    
    static int i = 162;
    //unsigned short val = 162;
    
    /*
    switch (i)
    {
        case    0:
            //val = 162;      //30 C
            val = 540;      //99 C
            break;
        case    1:
            val = 560;      //99 C
            break;
        case    2:
            val = 580;
            break;
        case    3:
            val = 600;
            break;
        case    4:
            val = 620;
            break;
        case    5:
            val = 640;
            break;
        case    6:
            val = 3340;
            break;
        case    7:
            val = 3370;
            break;
        case    8:
            val = 3400;
            break;
        case    9:
            val = 3450;
            break;
        case    10:
            val = 3500;
            break;
        case    11:
            val = 3650;     //658 C
            break;
        default:
            val = 3650;
            break;
    }
    
    if (0 <= i && i <=11)
    {
        pta_sim.dac_ch1.set_dac_raw_value(val);
    }
    */
    
    pta_sim.dac_ch1.set_dac_raw_value(i);

    CV_LOG("dac new output!!! - %d\r\n", i);
    
    if (3600 >= i)  //651 C
    {
        i++;
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
    tick.delay_ms(20);
    
	m_state = m_next_state;
    m_next_state = SIM_WITH_DAC;
}