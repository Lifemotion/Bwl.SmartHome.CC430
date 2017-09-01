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

#define SW_RESET()      PMMCTL0 = PMMPW + PMMSWPOR + (PMMCTL0 & 0x0003);

uint8_t RxCallBack(linkID_t lid);
char radio_send_data(uint8_t *msg, uint8_t len, uint8_t repeats);
void radio_data_handler(uint8_t *msg, uint8_t len);
void sleep_mode();
char radio_connect();

