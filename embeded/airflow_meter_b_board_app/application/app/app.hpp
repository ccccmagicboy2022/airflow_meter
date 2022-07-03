#ifndef _13469FD0_F090_488F_813F_85B823F3A7DB_
#define _13469FD0_F090_488F_813F_85B823F3A7DB_

#include "sys.h"

enum app_state
{
	UART_SEND_DATA=0,
	IDLE,
	UART_PROTOCOL,
	ERROR_ERROR,
};

class App 
{
	public:
		volatile enum app_state m_state;	//状态机变量
		volatile enum app_state m_next_state;	//状态机变量的下一个状态
    
	public:
        App();
        ~App();
		
		void run(void);
        void sent_sample_data(void);
        void error_process(void);
        void uart_process(void);
        void idle_process(void);
	
};


#endif//_13469FD0_F090_488F_813F_85B823F3A7DB_
