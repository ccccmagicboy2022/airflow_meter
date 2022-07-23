#ifndef _8352EE21_E264_4387_8018_7E8C83AE8709_
#define _8352EE21_E264_4387_8018_7E8C83AE8709_

#include "sys.h"
#include "do.hpp"
#include "di.hpp"

enum spi_cmd
{
    START_TOF_UP = 0x01,
    START_TOF_RESTART = 0x03,
    START_TEMP,
    START_TEMP_RESTART,
    START_CAL_RESONATOR,
    POR = 0x50,
    INITIAL = 0x70,
	WRITE_REG0 = 0x80,
	WRITE_REG1,
	WRITE_REG2,
	WRITE_REG3,
    WRITE_REG4,
    READ_TOF_UP_STOP1 = 0xB0,
    READ_TOF_UP_STOP2,
    READ_TOF_UP_STOP3,
    READ_TOF_UP_STOP4,
    READ_TOF_UP_STOP5,
    READ_TOF_UP_STOP6,
    READ_TOF_UP_STOP7,
    READ_TOF_UP_STOP8,
    READ_TOF_UP_SUM,
    READ_TOF_DOWN_STOP1,
    READ_TOF_DOWN_STOP2,
    READ_TOF_DOWN_STOP3,
    READ_TOF_DOWN_STOP4,
    READ_TOF_DOWN_STOP5,
    READ_TOF_DOWN_STOP6,
    READ_TOF_DOWN_STOP7,
    READ_TOF_DOWN_STOP8,
    READ_TOF_DOWN_SUM,
    READ_TEMP_PT1,
    READ_TEMP_PT2,
    READ_TEMP_PT3,
    READ_TEMP_PT4,
    READ_PW_FIRST = 0xD0,
    READ_PW_STOP1,
    READ_STATUS_REG,
    READ_COMM_REG,
    READ_CAL_REG
};

class Spi {
	public:
        Spi();
        ~Spi();
    
    public:
        Do spi_ss;      //cs for spi
        Do spi_rstn;    //ms1030 reset
        Di spi_int;     //ms1030 interrupt output
    
    public:
        void init_pin();
        void init_spi();
        void init_int();
        void write8(uint8_t wbuf8);
        uint8_t read8();
        uint16_t read16();
        void init_ss_pin();
        void Write_Reg(uint8_t RegNum, uint32_t RegData);
        uint8_t config();
        void init_rst_pin();
        void Write_Order(uint8_t Order);
        uint8_t Read_REG0_L();
        uint16_t Read_STAT();
        uint32_t MS1030_Flow(void);
        void init_int_pin();
    
    
};



#endif//_8352EE21_E264_4387_8018_7E8C83AE8709_


