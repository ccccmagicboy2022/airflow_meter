#include "hardware.hpp"

Hardware::Hardware()
{
    led.init_pin(GPIOA, GPIO_PIN_8);
    
    led.high();         //initial led off
    
    ms1030_rstn.init_pin(GPIOA, GPIO_PIN_10);
    
    ms1030_rstn.high();
    
    spi_ss.init_pin(GPIOA, GPIO_PIN_4);
    spi_ss.high();
}

Hardware::~Hardware()
{
    //
}
