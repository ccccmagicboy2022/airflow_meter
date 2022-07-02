#include "hardware.hpp"

Hardware::Hardware()
{
    enrf.init_pin(GPIOB, GPIO_PIN_4);
    enop.init_pin(GPIOB, GPIO_PIN_5);
    out.init_pin(GPIOA, GPIO_PIN_8);
    led.init_pin(GPIOA, GPIO_PIN_12);
    
    enrf.high();        //rf enable
    enop.high();        //op enable
    out.low();          //not use
    led.high();         //led off
}

Hardware::~Hardware()
{
    //
}
