#include <cc430f5137.h>
#include <msp430.h>
#include <stdbool.h>

#include "bsp.h"
#include "mrfi.h"
#include "nwk_types.h"
#include "nwk_api.h"
#include "bsp_leds.h"
#include "bsp_buttons.h"
#include "app_remap_led.h"
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
  int ch = 0;	
  BSP_Init();
  gpio_init();
  for(ch=0;ch<16;ch++){
  		P1OUT ^= BIT0;
  		__delay_cycles(1200000);
  }

  relay_off();
  while (SMPL_SUCCESS != SMPL_Init(RxCallBack))
  {
  	SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_SLEEP, 0);
    sleep_mode();
    SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_AWAKE, 0);  
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
  }
  P1OUT &= ~BIT0;
  linkTo();
  while (1) ;
}

void gpio_init(){
	P1DIR &= ~(BIT4|BIT0);
	P2DIR |= (BIT4|BIT3|BIT5|BIT6|BIT2|BIT7);
	P2DS  |= (BIT4|BIT3|BIT5|BIT6|BIT2|BIT7);
	P1DIR |= BIT0;
	P1REN |= BIT4;
	P1OUT |= BIT4;	
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
  void *nwk_cfg_flash;
  ioctlNVObj_t nwk_cfg_ram;
 uint8_t *data = NULL;
  
  nwk_cfg_flash = nv_obj_read_nwk_cfg();
  if(nwk_cfg_flash == NULL)
  {
 	 while (SMPL_SUCCESS != SMPL_Link(&sLinkID1))
	  {
  	  SPIN_ABOUT_A_SECOND;
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
 	 }
 	 //Подключились? сохраняемся
     new_link = 1;
  } else {
  	//загружаем предыдущий конфиг в память, кидаем указатель в SiTI
    nwk_cfg_ram.objPtr = &data;
    if(SMPL_Ioctl(IOCTL_OBJ_NVOBJ, IOCTL_ACT_GET, &nwk_cfg_ram) == SMPL_SUCCESS)
    {
      memcpy(data, nwk_cfg_flash, nwk_cfg_ram.objLen);
    }
    sLinkID1 = nv_obj_read_lnk_id(0);
    
  }
  //всё, работаем в сети
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
  	  
  	  if(radio_cmd == 0x01){
  	  		relay_on();
  	  }
  	  if(radio_cmd == 0x02){
  	  		relay_off();
  	  }
  	  radio_cmd = 0;
  	  msg[0] = 0x01;
  	  msg[1] = current_relay_state;
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_AWAKE, 0);       
      SMPL_SendOpt(sLinkID1, msg, 2, SMPL_TXOPTION_ACKREQ) ;
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXON, 0);
	  NWK_REPLY_DELAY();
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXIDLE, 0); 
      SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_SLEEP, 0);
      if(new_link) save_link();
      sleep_mode();
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
