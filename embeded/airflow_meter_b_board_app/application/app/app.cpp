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
    float cal_rate = 0.0f;
    float temperature = 0.0f;
    float flow = 0.0f;
    uint16_t status = 0x0000;
    
    //uint8_t temp = 0;
    
    //tick.delay_ms(1000);
    
    //log_set_level(LOG_INFO);
    //log_trace("Hello %s\r\n", "world");
    //log_debug("Hello %s\r\n", "world");
    //log_info("Hello %s\r\n", "world");
    //log_warn("Hello %s\r\n", "world");
    //log_error("Hello %s\r\n", "world");
    //log_fatal("Hello %s\r\n", "world");
    
    airflow_meter_b.ms1030.MS1030_Flow();        //basic ok
    flow = airflow_meter_b.ms1030.get_flow();
    //log_info("flow: %3.5lf\r\n", flow);
    status = airflow_meter_b.ms1030.get_status();
    //log_info("status reg: 0x%04X\r\n", status);
    
    //airflow_meter_b.ms1030.MS1030_Temper();       //ok
    //temperature = airflow_meter_b.ms1030.get_temp();
    //log_info("PT1000 temperature: %3.5lf\r\n", temperature);
    //status = airflow_meter_b.ms1030.get_status();
    //log_info("status reg: 0x%04X\r\n", status);
    
    //cal_rate = airflow_meter_b.ms1030.MS1030_Time_check();     //ok
    //log_info("cal_rate: %3.5lf\r\n", cal_rate);
    
    printf("/*airflow_meter_b,%.5lf,%.5lf,%.5lf,%d*/", flow, temperature, cal_rate, status>>4);
    
	m_state = m_next_state;
    m_next_state = UART_SEND_DATA;
}


