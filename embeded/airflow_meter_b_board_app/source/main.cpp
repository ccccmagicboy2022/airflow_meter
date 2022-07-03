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


