#ifndef _2C0B6B86_5309_4A2A_BECF_437C72D335B7_
#define _2C0B6B86_5309_4A2A_BECF_437C72D335B7_

#define SEGGER_RTT_IN_RAM   1

class Rtt 
{
	public:
		char m_JS_RTT_UpBuffer[2048*4*2];		//2k sample 4bytes single item 2s raw data
		
	public:
        Rtt();
        ~Rtt();
		
		void init(void);
	
	
};


#endif//_2C0B6B86_5309_4A2A_BECF_437C72D335B7_
