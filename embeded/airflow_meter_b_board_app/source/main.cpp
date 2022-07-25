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
    uint8_t test_reg = 0x00;
    // put your setup code here, to run once:
    memory_init();
    tick.delay_ms(10);  //initial wait for analog chip
    
    test_reg = airflow_meter_b.ms1030.config();
    
    log_info("ms1030 config test byte: 0x%02X\r\n", test_reg);
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


