#include <cc430x513x.h>
#include "libs/radio.h"
#include <stdint.h>

unsigned char RxUartBuffer[5];
unsigned char tx_data[5] = {0x01, 0xAD, 0x44, 0xAA, 0xAD};
int USCI_A1_Init(void);

int main(void) {
    WDTCTL = WDTPW | WDTHOLD;	// Stop watchdog timer
    SetVCore(2);
    ResetRadioCore();
	radio_init();
	USCI_A1_Init();
	radio_set_addr(0x02);
	radio_receive_on();
	gpio_init();
	while(1){
		if(P1IN == 0x7E){
			P1OUT |= BIT0;
			radio_send_data(tx_data, 5);
			__delay_cycles(500000);
			P1OUT &= ~BIT0;
		}
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

int gpio_init(){
	P1DIR &= ~BIT7;
	P1REN |= BIT7;
	P1OUT |= BIT7;

	P1DIR |= BIT0;
	P1OUT &= ~BIT0;
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


void radio_incoming_pack(unsigned char *buffer, unsigned char len){
	P1OUT ^= BIT0;
	uart_send_array(buffer, len);
}


