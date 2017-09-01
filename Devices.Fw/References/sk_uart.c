#include "sk_uart.h"
#include <msp430.h>


void uart_init(void)
{
    P1MAP5 = PM_UCA0RXD;                      // Map UCA0RXD output to P1.5 
    P1MAP6 = PM_UCA0TXD;                      // Map UCA0TXD output to P1.6 
    PMAPPWD = 0;                              // Lock port mapping registers  
    P1DIR |= BIT6;                            // Set P1.6 as TX output
    P1SEL |= BIT5 + BIT6;                     // Select P1.5 & P1.6 to UART function  
    UCA0CTL1 |= UCSWRST;                      // **Put state machine in reset**
    UCA0CTL1 |= UCSSEL_2;                     // SMCLK
    UCA0BR0 = 113;                            // 12MHz 19200 (see User's Guide)
    UCA0BR1 = 2;                              // 12MHz 19200
    UCA0MCTL |= UCBRS_1 + UCBRF_0;            // Modulation UCBRSx=1, UCBRFx=0
    UCA0CTL1 &= ~UCSWRST; 
    uart_write_byte(0xAA);
    uart_write_byte(0xAA);
    uart_write_byte(0xAA);
}

void uart_write_byte(char data)
{
    while (!(UCA0IFG&UCTXIFG));             
    UCA0TXBUF = data; 
}

void uart_write_array(char *arr, unsigned char len)
{
    while(len--){
	uart_write_byte(*arr++);
    }
}

char uart_received()
{
    return UCA0IFG&UCRXIFG>0?1:0;
}

char uart_read_byte()
{
     return UCA0RXBUF;
}
