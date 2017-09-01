#include "device_radio_hal.h"
#define work_delay 120

unsigned char sensor_state     = 0;
unsigned int  work_cycle_count = 0;    

void main (void)
{
  unsigned int i= 0;
  unsigned int battery_voltage  = 0;
  uint8_t msg[3];
  work_cycle_count = work_delay + 1;
  WDTCTL = (WDTPW + WDTHOLD);
  BSP_Init();
  radio_connect();
  SMPL_Ioctl (IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_SLEEP, 0);
  P1SEL = 0x00;    
  P2SEL = 0x00; 
  P3SEL = 0x00;  
  P1DIR = 0xFF; 
  P2DIR = 0xFF; 
  P3DIR = 0xFF; 
  PJDIR = 0xFF;
  P2IE  |= BIT1;
  P2IES |= BIT1;
  P2IFG  = 0;
  P2DIR &= ~BIT1;
  while (1){
    if(work_cycle_count++>work_delay){ 
      msg[0] = (P2IN & BIT1);
      msg[1] = battery_voltage >> 8;
      msg[2] = battery_voltage &  0xFF;
      SMPL_Ioctl (IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_AWAKE, 0);
      SMPL_Ioctl (IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXON, 0);
      if(radio_send_data(msg, 3, 1)==0)i++;  else i = 0; 
      if(i==3)SW_RESET();
      SMPL_Ioctl (IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_SLEEP, 0);
      if(P2IN & BIT1) work_cycle_count = 0; else work_cycle_count -=10;    
    } 
    sleep_mode();
  }
}

#pragma vector=PORT2_VECTOR
__interrupt void port_2_handler(void)
{  
    P2IFG = 0;   
    work_cycle_count = work_delay + 1;
    _BIC_SR_IRQ(LPM3_bits + GIE);   
}

#pragma vector=WDT_VECTOR
__interrupt void watchdog_timer (void)
{
    _BIC_SR_IRQ(LPM3_bits + GIE);   
}

void radio_data_handler(uint8_t *msg, uint8_t len){
    //incoming packet handler
}