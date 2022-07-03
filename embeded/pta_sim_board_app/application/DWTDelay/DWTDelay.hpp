#ifndef _FF956FE3_82B4_4AEE_9689_AB611F33F190_
#define _FF956FE3_82B4_4AEE_9689_AB611F33F190_

class DWTDelay 
{
	public:
		unsigned int m_sysclk;
    
	public:
        DWTDelay();
        ~DWTDelay();
		
		void init(unsigned int sys_clk);
        void delay_us(unsigned int usec);
        void delay_ms(unsigned int msec);
        unsigned int get_tick(void);
    
};


 
#endif//_FF956FE3_82B4_4AEE_9689_AB611F33F190_


