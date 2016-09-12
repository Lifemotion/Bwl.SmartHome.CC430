#include <string.h>
#include <stdbool.h>

#include "bsp.h"
#include "mrfi.h"
#include "bsp_leds.h"
#include "bsp_buttons.h"
#include "nwk_types.h"
#include "nwk_api.h"
#include "nwk_frame.h"
#include "nwk.h"
#include "nv_obj.h"


static linkID_t sLID[NUM_CONNECTIONS] = {0};
static uint8_t  sNumCurrentPeers = 0;
static uint8_t sCB(linkID_t);
extern void nwkGetRemotePeerAddr(linkID_t sLinkId, addr_t *peerAddr);
static void processMessage(linkID_t, uint8_t *, uint8_t);
void uart_init();

static volatile uint8_t sPeerFrameSem = 0;
static volatile uint8_t sJoinSem = 0;
static volatile uint8_t sBlinky = 0;

#define SPIN_ABOUT_A_QUARTER_SECOND   NWK_DELAY(250)

void main (void)
{
  P1OUT ^= BIT0;
  void *nwk_cfg_flash;
  ioctlNVObj_t nwk_cfg_ram;
  uint8_t *data = NULL;
  
  bspIState_t intState;
  BSP_Init();
  uart_init();
  SMPL_Init(sCB);
  
  nwk_cfg_flash = nv_obj_read_nwk_cfg();
  //восстанавливаем линки
  if(nwk_cfg_flash != NULL)
  {
  	int i = 0;
  	nwk_cfg_ram.objPtr = &data;
    if(SMPL_Ioctl(IOCTL_OBJ_NVOBJ, IOCTL_ACT_GET, &nwk_cfg_ram) == SMPL_SUCCESS)
    {
      memcpy(data, nwk_cfg_flash, nwk_cfg_ram.objLen);
    }
    sNumCurrentPeers = (uint8_t)nv_obj_read_lnk_id(0);
    for(i=0;i<sNumCurrentPeers;i++){
    	sLID[i] = nv_obj_read_lnk_id(i+1);
    }
  }
  
  while (1)
  {
    if (sJoinSem && (sNumCurrentPeers < NUM_CONNECTIONS)){
      while (1){
        if (SMPL_SUCCESS == SMPL_LinkListen(&sLID[sNumCurrentPeers])){
          break;
        }
      }
      BSP_ENTER_CRITICAL_SECTION(intState);
      nwk_cfg_ram.objPtr = &data;
      if(SMPL_Ioctl(IOCTL_OBJ_NVOBJ, IOCTL_ACT_GET, &nwk_cfg_ram) == SMPL_SUCCESS){
           if(nv_obj_write_nwk_cfg(data,nwk_cfg_ram.objLen) != true){
             __no_operation(); // for debugging
           }
         }
      sNumCurrentPeers++;
      nv_obj_write_lnk_id(0, sNumCurrentPeers);
      nv_obj_write_lnk_id(sNumCurrentPeers, sLID[sNumCurrentPeers-1]);
      sJoinSem--;
      BSP_EXIT_CRITICAL_SECTION(intState);
    }
    if (sPeerFrameSem)
    {
      uint8_t     msg[MAX_APP_PAYLOAD], len, i;
      for (i=0; i<sNumCurrentPeers; ++i)
      {
        if (SMPL_SUCCESS == SMPL_Receive(sLID[i], msg, &len))
        {
          processMessage(sLID[i], msg, len);
          BSP_ENTER_CRITICAL_SECTION(intState);
          sPeerFrameSem--;
          BSP_EXIT_CRITICAL_SECTION(intState);
        }
      }
    }
  }

}

void rtc_init(){
  RTCCTL01 = RTCTEVIE + RTCSSEL_2 + RTCTEV_0; // Counter Mode, RTC1PS, 8-bit ovf
  RTCPS0CTL = RT0PSDIV_2;                   // ACLK, /8, start timer
  RTCPS1CTL = RT1SSEL_2 + RT1PSDIV_3; 	
}



#pragma vector=RTC_VECTOR
__interrupt void RTC_ISR(void){
  switch(__even_in_range(RTCIV,16))
  {
    case 0: break;                          // No interrupts
    case 2: break;                          // RTCRDYIFG
    case 4:                                 // RTCEVIFG
      P1OUT ^= 0x01;
      break;
    case 6: break;                          // RTCAIFG
    case 8: break;                          // RT0PSIFG
    case 10: break;                         // RT1PSIFG
    case 12: break;                         // Reserved
    case 14: break;                         // Reserved
    case 16: break;                         // Reserved
    default: break;
  }
}


void uart_init(){
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
  UCA0CTL1 &= ~UCSWRST;                     // **Initialize USCI state machine**	
}

void uart_write_byte(unsigned char ch){
	 while (!(UCA0IFG&UCTXIFG));             
   	 UCA0TXBUF = ch; 	
}


void uart_write_array(uint8_t *data, uint8_t num){
	while(num--){
		uart_write_byte(*data++);
	}	
}


uint8_t uart_read_byte(){
	 int i = 0;
	 while (!(UCA0IFG&UCRXIFG) && i<65000)i++;             // USCI_A0 TX buffer ready?
   	 return UCA0RXBUF;	
}

void uart_read_array(uint8_t *data, uint8_t num){
	while(num--)*data++ = uart_read_byte();
}

static uint8_t sCB(linkID_t lid)
{
  if (lid)
  {
    sPeerFrameSem++;
  } else {
  	P1OUT ^= BIT0;
    sJoinSem++;
  }
  return 0;
}

static void processMessage(linkID_t lid, uint8_t *msg, uint8_t len)
{
  uint8_t resp; 
  addr_t peerAddr;
  int i = 0;
  uint8_t pack[50];
  
  nwkGetRemotePeerAddr(lid, &peerAddr);
  pack[0] = len+5;
  pack[1] = 0x02;
  pack[2] = peerAddr.addr[0];
  pack[3] = peerAddr.addr[1];
  pack[4] = peerAddr.addr[2];
  pack[5] = peerAddr.addr[3];
  for(i=0; i<len;i++){
  		pack[i+6] = msg[i];
  }
  uart_write_array(pack, len+6);
  resp = uart_read_byte();
  if(resp>0){
  	    uint8_t buf[5];
  	    uint8_t addr_to  = uart_read_byte();
  	    uart_read_array(buf, resp-1);
  		SMPL_SendOpt(addr_to, buf, resp-1, SMPL_TXOPTION_NONE);   
  		SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RXON, 0);
  }
  return;
}

