#include "board_dongle.h"

int main( void )
{
  radio_init();
  uart_init();
  P1DIR |= BIT0; 
  while(1){
    radio_poll();
  }
}
