#include <cc430x513x.h>
#include "libs/radio.h"
#include <stdint.h>

int USCI_A1_Init(void);

unsigned char RxUartBuffer[255];
unsigned char RxUartLength = 0;
unsigned char curr_pos = 0;
unsigned char d_len = 0;

/*
 * main.c for usb adapter
 */
int main(void) {
    WDTCTL = WDTPW | WDTHOLD;	// Stop watchdog timer
    SetVCore(2);
    ResetRadioCore();
   	radio_init();
   	USCI_A1_Init();
   	radio_set_addr(0x01);
   	radio_receive_on();
   	P1DIR |=  BIT0;
   	P1OUT &= ~BIT0;
   	while(1){
   		__bis_SR_register( LPM3_bits + GIE );
   		__no_operation();
   	}
}

int USCI_A1_Init(void)
{
	  PMAPPWD = 0x02D52;                        // Get write-access to port mapping regs
	  P1MAP5 = PM_UCA0RXD;                      // Map UCA0RXD output to P1.5
	  P1MAP6 = PM_UCA0TXD;                      // Map UCA0TXD output to P1.6
	  PMAPPWD = 0;                              // Lock port mapping registers
	  P1DIR |= BIT6;                            // Set P1.6 as TX output
	  P1SEL |= BIT5 + BIT6;                     // Select P1.5 & P1.6 to UART function
	  UCA0CTL1 |= UCSWRST;                      // **Put state machine in reset**
	  UCA0CTL1 |= UCSSEL_2;                     // SMCLK
	  UCA0BR0 = 9;                              // 1MHz 115200 (see User's Guide)
	  UCA0BR1 = 0;                              // 1MHz 115200
	  UCA0MCTL |= UCBRS_1 + UCBRF_0;            // Modulation UCBRSx=1, UCBRFx=0
	  UCA0CTL1 &= ~UCSWRST;                     // **Initialize USCI state machine**
	  UCA0IE |= UCRXIE;                         // Enable USCI_A0 RX interrupt
	  __enable_interrupt();
	  return 0;
}

int uart_send_byte(unsigned char data){
	  while (!(UCA0IFG&UCTXIFG));             // USCI_A0 TX buffer ready?
	  UCA0TXBUF = data;
	  return 0;
}

void uart_send_array(unsigned char *data, char num)
{
	while(num--) uart_send_byte(*data++);
}

//RF rx callback
void radio_incoming_pack(unsigned char *buffer, unsigned char len){
	uart_send_byte(len);
	uart_send_array(buffer, len);
}

#if defined(__TI_COMPILER_VERSION__) || defined(__IAR_SYSTEMS_ICC__)
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
#elif defined(__GNUC__)
void __attribute__ ((interrupt(USCI_A0_VECTOR))) USCI_A0_ISR (void)
#else
#error Compiler not supported!
#endif
{
  switch(__even_in_range(UCA0IV,4))
  {
  case 0:break;                             // Vector 0 - no interrupt
  case 2:                                   // Vector 2 - RXIFG
    while (!(UCA0IFG&UCTXIFG));             // USCI_A0 TX buffer ready?               // TX -> RXed character
       if(d_len==0){
    	   d_len = UCA0RXBUF;
       }else{
    	   RxUartBuffer[curr_pos] = UCA0RXBUF;
    	   curr_pos++;
    	   if(d_len==curr_pos){
    		   radio_send_data(RxUartBuffer, d_len);
    		   d_len = 0;
    		   curr_pos = 0;
		   }
       }
	break;
  case 4:break;
  default: break;
  }
}

