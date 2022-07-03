#include "DWTDelay.hpp"
#include "sys.h"
 
// 0xE000EDFC DEMCR RW Debug Exception and Monitor Control Register.
#define DEMCR           ( *(unsigned int *)0xE000EDFC )
#define TRCENA          ( 0x01 << 24) // DEMCR的DWT使能位
 
// 0xE0001000 DWT_CTRL RW The Debug Watchpoint and Trace (DWT) unit
#define DWT_CTRL        ( *(unsigned int *)0xE0001000 )
#define CYCCNTENA       ( 0x01 << 0 ) // DWT的SYCCNT使能位
// 0xE0001004 DWT_CYCCNT RW Cycle Count register, 
#define DWT_CYCCNT      ( *(unsigned int *)0xE0001004) // 显示或设置处理器的周期计数值

void DWTDelay::init(unsigned int sys_clk)
{
  DEMCR |= TRCENA;
  DWT_CTRL |= CYCCNTENA;
  
  m_sysclk = sys_clk; // 保存当前系统的时钟周期，eg. 72,000,000(72MHz). 
}

DWTDelay::DWTDelay()
{
    RCC_ClocksType RCC_ClockFreq;
    
    RCC_GetClocksFreqValue(&RCC_ClockFreq);
    init(RCC_ClockFreq.SysclkFreq);
}

DWTDelay::~DWTDelay()
{
    //
}

void DWTDelay::delay_us(unsigned int usec)
{
  int ticks_start, ticks_end, ticks_delay;
  
  ticks_start = DWT_CYCCNT;
  
  ticks_delay = ( usec * ( m_sysclk / (1000*1000) ) ); // 将微秒数换算成滴答数          
  
  ticks_end = ticks_start + ticks_delay;
  
  if ( ticks_end > ticks_start )
  {
    while( DWT_CYCCNT < ticks_end );
  }
  else // 计数溢出，翻转
  {
    while( DWT_CYCCNT >= ticks_end ); // 翻转后的值不会比ticks_end小
    while( DWT_CYCCNT < ticks_end );
  }
}

void DWTDelay::delay_ms(unsigned int msec)
{
    delay_us(msec*1000);
}

unsigned int DWTDelay::get_tick(void)
{
    return  (int)(DWT_CYCCNT);
}
