#include <cc430f5137.h>
#include <msp430.h>

#include "bsp.h"
#include "mrfi.h"
#include "nwk_types.h"
#include "nwk_api.h"
#include "bsp_leds.h"
#include "bsp_buttons.h"
unsigned char counter = 0;
#include "app_remap_led.h"

#ifndef APP_AUTO_ACK
#error ERROR: Must define the macro APP_AUTO_ACK for this application.
#endif

void toggleLED(uint8_t);

uint8_t relay_state = 0; //реле разомкнуто
uint8_t current_switch_state = 0;
uint8_t current_relay_state = 0x01;

static void linkTo(void);
void gpio_init();
void relay_on();
void relay_off();
void sleep_mode();
static uint8_t RxCallBack(linkID_t lid);

static linkID_t sLinkID1 = 0;

#define SPIN_ABOUT_A_SECOND   NWK_DELAY(1000)
#define SPIN_ABOUT_A_QUARTER_SECOND   NWK_DELAY(250)
#define MISSES_IN_A_ROW  2

uint8_t send_counter = 0;
void main (void)
{
  BSP_Init();
  gpio_init();
  relay_off();
  relay_on();
  while (SMPL_SUCCESS != SMPL_Init(RxCallBack))
  {
    SPIN_ABOUT_A_SECOND;
  }
  P1OUT &= ~BIT0;
  linkTo();
  while (1) ;
}

void gpio_init(){
	P1DIR &= ~(BIT4|BIT0);
	P2DIR |= (BIT4|BIT3);
	P1DIR |= BIT0;
	P1REN |= BIT4;
	P1OUT |= BIT4;	
}

void relay_on(){
	P1OUT |= BIT0;
	P2OUT &= BIT3;
	P2OUT |= BIT4;	
	current_relay_state = 0x01;
}

void relay_off(){
	P1OUT &= ~BIT0;
	P2OUT |=  BIT3;
	P2OUT &=  ~BIT4;			
	current_relay_state = 0;
}

static void linkTo()
{
  uint8_t     msg[2];
  while (SMPL_SUCCESS != SMPL_Link(&sLinkID1))
  {
    SPIN_ABOUT_A_SECOND;
  }
   
  while (1)
  {  
  	  uint8_t res = P1IN & 0x10;
  	  uint8_t switch_state = (res > 0) ? 0:1;
  	  if(current_switch_state  != switch_state){
  	  		if(current_relay_state == 0){
  	  			relay_on();
  	  		}else{
  	  			relay_off();
  	  		}
  	  	    current_switch_state = switch_state;
  	  }
  	  msg[0] = 0x01;
  	  msg[1] = current_relay_state;
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_AWAKE, 0);       
      while(SMPL_SendOpt(sLinkID1, msg, 2, SMPL_TXOPTION_ACKREQ) != SMPL_SUCCESS){
      	send_counter++;
      	if(send_counter == 2){
      		//установить соединение, если оно потеряно
      		while (SMPL_SUCCESS != SMPL_Init(RxCallBack)){
      			P1OUT ^= BIT0;
   				 SPIN_ABOUT_A_SECOND;
  			}
      		while (SMPL_SUCCESS != SMPL_Link(&sLinkID1)){
 					 P1OUT ^= BIT0;
   					 SPIN_ABOUT_A_SECOND;
 			}			
			send_counter = 0;
			break;	
      	}
      }
      SMPL_SendOpt(sLinkID1, msg, 2, SMPL_TXOPTION_ACKREQ);   
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXON, 0);
	  NWK_REPLY_DELAY();
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXIDLE, 0); 
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_SLEEP, 0);
      sleep_mode();
  }
}

static uint8_t RxCallBack(linkID_t lid){
	uint8_t len;
	uint8_t msg[2];
	
	if (SMPL_SUCCESS == SMPL_Receive(sLinkID1, msg, &len)){
			if(msg[0] == 0x01){
				if(msg[1] == 0x00){
					relay_off();
				}else{
					relay_on();
				}	
			}
	}
	return 0;
}

void sleep_mode(){
    WDTCTL = WDT_ADLY_1000;   
    SFRIE1 |= WDTIE; 
    _BIS_SR(LPM3_bits + GIE);   
}


#pragma vector = WDT_VECTOR
__interrupt void WDT_ISR(void){
		counter++;
		if(counter==2){
	     	SFRIE1 &= ~WDTIE; 
        	__bic_SR_register_on_exit(LPM3_bits);
        	counter = 0;
		} 
}
