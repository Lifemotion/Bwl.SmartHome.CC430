#include <string.h>
#include <stdbool.h>
//SimpliciTI libs
#include "bsp.h"
#include "mrfi.h"
#include "bsp_leds.h"
#include "bsp_buttons.h"
#include "nwk_types.h"
#include "nwk_api.h"
#include "nwk_frame.h"
#include "nwk.h"
#include "nv_obj.h"
//User libs
#include "sk_uart.h"

void radio_init();
void radio_poll();