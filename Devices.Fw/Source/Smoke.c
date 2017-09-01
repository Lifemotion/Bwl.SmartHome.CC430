#include <cc430f5137.h>
#include <msp430.h>
#include <stdbool.h>

#include "bsp.h"
#include "mrfi.h"
#include "nwk_types.h"
#include "nwk_api.h"
#include "bsp_leds.h"
#include "bsp_buttons.h"
#include "nv_obj.h"

#define NVOBJECT_SUPPORT

#ifndef APP_AUTO_ACK
#error ERROR: Must define the macro APP_AUTO_ACK for this application.
#endif

void toggleLED(uint8_t);

uint8_t relay_state = 0; //реле разомкнуто
uint8_t current_switch_state = 0x01;
uint8_t current_relay_state = 0x00;
uint8_t radio_cmd = 0x00;
unsigned char counter = 0;

static void linkTo(void);
void gpio_init();
void relay_on();
void relay_off();
void sleep_mode();
void save_link();

static uint8_t RxCallBack(linkID_t lid);

static linkID_t sLinkID1 = 0;

#define SPIN_ABOUT_A_SECOND   NWK_DELAY(1000)
#define SPIN_ABOUT_A_QUARTER_SECOND   NWK_DELAY(250)
#define MISSES_IN_A_ROW  2

uint8_t send_counter = 0;
char new_link = 0;
void main (void)
{
  	
  BSP_Init();
  gpio_init();
  P1OUT |= BIT0;
  while (SMPL_SUCCESS != SMPL_Init(RxCallBack))
  {
      SPIN_ABOUT_A_SECOND;
      P1OUT ^= BIT0;
  }
  P1OUT &= ~BIT0;
  linkTo();
  while (1);
}

void gpio_init(){
	P1DIR |= BIT0;
}


void relay_off(){
	P2OUT &= ~(BIT3|BIT4|BIT2);
	current_relay_state = 0x00;
}

void relay_on(){
	P2OUT |=  (BIT3|BIT4|BIT2);	
	current_relay_state = 0x01;
}

static void linkTo()
{
  uint8_t     msg[2];
  uint8_t      misses, done;
  while (SMPL_SUCCESS != SMPL_Link(&sLinkID1))
  {
    SPIN_ABOUT_A_SECOND; /* calls nwk_pllBackgrounder for us */
  }
  while (1)
  {  
      uint8_t      noAck;
      smplStatus_t rc;
      msg[1] = 'a';
      msg[0] = 'e';
      done = 0;
      while (!done)
      {
        noAck = 0;
        SPIN_ABOUT_A_SECOND;
        for (misses=0; misses < MISSES_IN_A_ROW; ++misses)
        {
          if (SMPL_SUCCESS == (rc=SMPL_SendOpt(sLinkID1, msg, sizeof(msg), SMPL_TXOPTION_ACKREQ)))
          {
            break;
          }
          if (SMPL_NO_ACK == rc)
          {
            noAck++;
          }
        }
        if (MISSES_IN_A_ROW == noAck)
        {
          done = 1;
        }
        else
        {
          done = 1;
        }
      }
    }
}


void save_link(){
	 uint8_t *data = NULL;
     ioctlNVObj_t nwk_cfg_ram;
 	 nwk_cfg_ram.objPtr = &data;
     if(SMPL_Ioctl(IOCTL_OBJ_NVOBJ, IOCTL_ACT_GET, &nwk_cfg_ram) == SMPL_SUCCESS){
          if(nv_obj_write_nwk_cfg(data,nwk_cfg_ram.objLen) != true){
            
          }
     }     
     if(nv_obj_write_lnk_id(0, sLinkID1) == false){
          __no_operation(); 
     }
     new_link = 0;
}

static uint8_t RxCallBack(linkID_t lid){
	uint8_t len;
	uint8_t msg[2];
	
	if (SMPL_SUCCESS == SMPL_Receive(sLinkID1, msg, &len)){
			if(msg[0] == 0x01){
				if(msg[1] == 0x00){
					radio_cmd = 0x02;
				}else{
					radio_cmd = 0x01;
				}	
			}
	}
	return 0;
}


#pragma vector = WDT_VECTOR
__interrupt void WDT_ISR(void){
		/*counter++;
		if(counter==2){
	     	SFRIE1 &= ~WDTIE; 
        	__bic_SR_register_on_exit(LPM3_bits);
        	counter = 0;
		} */
}
