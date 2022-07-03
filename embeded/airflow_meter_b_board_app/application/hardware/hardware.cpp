#include "hardware.hpp"

Hardware::Hardware()
{
    led.init_pin(GPIOA, GPIO_PIN_8);
    
    led.high();         //initial led off
}

Hardware::~Hardware()
{
    //
}
