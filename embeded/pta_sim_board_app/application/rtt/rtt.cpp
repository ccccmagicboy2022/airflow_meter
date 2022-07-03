#include "rtt.hpp"
#include "sys.h"

void Rtt::init(void)
{
    //must align to dword
	//SEGGER_RTT_ConfigUpBuffer(1, "JScope_U2U2U2U2", &m_JS_RTT_UpBuffer[0], sizeof(m_JS_RTT_UpBuffer), SEGGER_RTT_MODE_NO_BLOCK_SKIP);
    SEGGER_RTT_ConfigUpBuffer(1, "JScope_U2U2", &m_JS_RTT_UpBuffer[0], sizeof(m_JS_RTT_UpBuffer), SEGGER_RTT_MODE_NO_BLOCK_SKIP);
    
	SEGGER_RTT_Init();
	CV_LOG("%sMODULE: AIRFLOW_METER_B%s\r\n", RTT_CTRL_BG_BRIGHT_RED, RTT_CTRL_RESET);
	CV_LOG("compiled time: %s %s\r\n", __DATE__, __TIME__);
    
#ifdef VECT_TAB_SRAM
    CV_LOG("ramcode program begin...\r\n");
#else
    CV_LOG("flashcode program begin...\r\n");
#endif
}

Rtt::Rtt()
{
    init();
}

Rtt::~Rtt()
{
    //
}


