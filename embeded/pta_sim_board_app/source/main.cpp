#include "sys.hpp"
#include "sys.h"

Rtt segger_cb;
Hardware pta_sim;
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
    //pta_sim.dac_ch1.set_dac_raw_value(162); //0.2V
    //pta_sim.dac_ch1.set_dac_raw_value(540); //0.66V
    //pta_sim.dac_ch1.set_dac_raw_value(1110); //1.36V
    //pta_sim.dac_ch1.set_dac_raw_value(1650); //2.04V
    //pta_sim.dac_ch1.set_dac_raw_value(2190); //2.70V
    //pta_sim.dac_ch1.set_dac_raw_value(2760); //3.40V
    //pta_sim.dac_ch1.set_dac_raw_value(3340); //4.08V
    //pta_sim.dac_ch1.set_dac_raw_value(3600); //4.44V
}

int main(void)
{
    setup();
    for(;;)loop();
}


