#include "device_radio_hal.h"



static linkID_t sLinkID1 = 0;
#define SPIN_ABOUT_A_SECOND   NWK_DELAY(1000)
#define SPIN_ABOUT_A_QUARTER_SECOND   NWK_DELAY(250)
#define MISSES_IN_A_ROW  2

static uint8_t RxCallBack(linkID_t lid)
{
	uint8_t len;
	uint8_t msg[50];	
	if (SMPL_SUCCESS == SMPL_Receive(sLinkID1, msg, &len)){
                          radio_data_handler(msg, len);
	}
	return 0;
}

char radio_connect()
{
  while (SMPL_SUCCESS != SMPL_Init(RxCallBack))
  {
      SPIN_ABOUT_A_SECOND;
  }  
  while (SMPL_SUCCESS != SMPL_Link(&sLinkID1))
  {
    SPIN_ABOUT_A_SECOND; /* calls nwk_pllBackgrounder for us */
  }
  return 1;
}


char radio_send_data(uint8_t *msg, uint8_t len, uint8_t repeats)
{
   uint8_t      misses;
   smplStatus_t rc;
   for (misses=0; misses < repeats; ++misses)
        {
          if (SMPL_SUCCESS == (rc=SMPL_SendOpt(sLinkID1, msg, len, SMPL_TXOPTION_ACKREQ)))
          {
            break;
          }        
   }
   if(rc == SMPL_SUCCESS)return 1;
   return 0;
}


void sleep_mode()
{
     WDTCTL = WDT_ADLY_1000;   
     SFRIE1 |= WDTIE; 
     _BIS_SR(LPM3_bits + GIE); 
    __no_operation();
}
