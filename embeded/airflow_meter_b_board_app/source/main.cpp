#include "sys.hpp"
#include "sys.h"

Rtt segger_cb;
Hardware airflow_meter_b;
Shell nr;
App app;
DWTDelay dwt;       //must be last location
Systick tick;

void setup()
{
    // put your setup code here, to run once:
    memory_init();
    tick.delay_ms(10);  //initial wait for analog chip
    if (0x30 == airflow_meter_b.ms1030.config())
    {
        log_info("ms1030 config ok!!!\r\n");
    }
    else
    {
        log_info("ms1030 config failed!!!\r\n");
    }
    
}

void loop()
{
    // put your main code here, to run repeatedly:
    app.run();
}

int main(void)
{
    setup();
    for(;;)loop();
}


