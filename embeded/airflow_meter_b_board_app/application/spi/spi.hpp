#ifndef _8352EE21_E264_4387_8018_7E8C83AE8709_
#define _8352EE21_E264_4387_8018_7E8C83AE8709_

#include "sys.h"

class Spi {
	public:
        Spi();
        ~Spi();
    
    public:
        void init_pin();
        void init_spi();
        void init_int();
        void write8(uint8_t wbuf8);
};



#endif//_8352EE21_E264_4387_8018_7E8C83AE8709_


