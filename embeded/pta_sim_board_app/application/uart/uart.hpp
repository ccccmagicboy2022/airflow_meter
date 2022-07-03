#ifndef _DBA6B598_04B8_4FD8_91E9_70462CB7D46B_
#define _DBA6B598_04B8_4FD8_91E9_70462CB7D46B_

#include "sys.h"

class Uart {
	public:
		unsigned int m_bitrate;
		
	public:
        Uart();
        ~Uart();
		void init(unsigned int bitrate);
		void deinit(void);
        void init_pin(void);
        void init_nvic(void);
};

#endif//_DBA6B598_04B8_4FD8_91E9_70462CB7D46B_


