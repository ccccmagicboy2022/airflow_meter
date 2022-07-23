#include "app.hpp"
#include "sys.h"
#include "sys.hpp"

extern Hardware airflow_meter_b;
extern DWTDelay dwt;
extern Systick tick;

App::App()
{
    m_state = UART_SEND_DATA;
    m_next_state = UART_SEND_DATA;
}

App::~App()
{
    //
}

void App::run(void)
{
	switch (m_state)
	{
		case    UART_SEND_DATA:
			sent_sample_data();
			break;
		case    UART_PROTOCOL:
			uart_process();
			break;
		case	IDLE:
			idle_process();
			break;
		case    ERROR_ERROR:
			error_process();
			break;
		default:
			error_process();
			break;
	}
}

void App::sent_sample_data(void)
{
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
    uint8_t temp = 0;
    
    tick.delay_ms(100);
    
    log_set_level(LOG_INFO);
    log_trace("Hello %s\r\n", "world");
    log_debug("Hello %s\r\n", "world");
    //log_info("Hello %s\r\n", "world");
    //log_warn("Hello %s\r\n", "world");
    //log_error("Hello %s\r\n", "world");
    //log_fatal("Hello %s\r\n", "world");
    
    airflow_meter_b.led.toggle();
    
	m_state = m_next_state;
    m_next_state = UART_SEND_DATA;
}