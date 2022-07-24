#ifndef _7090986F_63F7_4B1A_92CE_B3C5BE86422C_
#define _7090986F_63F7_4B1A_92CE_B3C5BE86422C_

#include <stdint.h>
#include <stdio.h>
#include <string.h>

#include "n32g4fr.h"
#include "SEGGER_RTT.h"
#include "nr_micro_shell.h"

#include "EventRecorder.h"

#include "fifo.h"
#include "bsp.h"
#include "log.h"

#include "arm_math.h"

#define CV_LOG(fmt, ...) \
do \
{ \
		SEGGER_RTT_printf(0, fmt, ##__VA_ARGS__); \
}while(0)

#define APP_VERSION             "1.0.0"                 /**< °æ±¾ºÅ */
#define ALL_UPLOAD_DELAY		100
#define BLOCK_TRANSFER_SIZE     500
/////////////////////////////////////////////////////////////////////
#define	USER_PARAMETER_START_SECTOR_ADDRESS0	0x0007A000u
#define	USER_PARAMETER_START_SECTOR_ADDRESS1	0x0007C000u
#define	MAGIC_SECTOR_ADDRESS0					0x0007E000u
/////////////////////////////////////////////////////////////////////
#endif//_7090986F_63F7_4B1A_92CE_B3C5BE86422C_

