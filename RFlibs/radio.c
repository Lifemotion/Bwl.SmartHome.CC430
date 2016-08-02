#include "RF1A.h"
#include <cc430x513x.h>

unsigned char *RxBuffer;
unsigned char RxBufferLength;
unsigned char transmitting = 0;
char wor = 0;

//Set setting from RF studio
void radio_init()
{
	PMMCTL0_H = 0xA5;
	PMMCTL0_L |= PMMHPMRE_L;
	PMMCTL0_H = 0x00;
	WriteSingleReg(IOCFG0, 0x06);
	WriteSingleReg(FIFOTHR, 0x47);
	WriteSingleReg(PKTCTRL0, 0x05);
	WriteSingleReg(FSCTRL1, 0x06);
	WriteSingleReg(FREQ2, 0x10);
	WriteSingleReg(FREQ1, 0xA7);
	WriteSingleReg(FREQ0, 0x62);
	WriteSingleReg(MDMCFG4, 0xF5);
	WriteSingleReg(MDMCFG3, 0x83);
	WriteSingleReg(MDMCFG2, 0x13);
	WriteSingleReg(MDMCFG1, 0xAA);
	WriteSingleReg(SYNC1, 0x52);
	WriteSingleReg(SYNC0, 0x52);
	WriteSingleReg(PKTCTRL0, 0x00);
	WriteSingleReg(PKTLEN, 0x05);
	WriteSingleReg(PKTCTRL1, 0x03);
	WriteSingleReg(DEVIATN, 0x15);
	WriteSingleReg(MCSM0, 0x10);
	WriteSingleReg(FOCCFG, 0x16);
	WriteSingleReg(WOREVT1, 0x87);
	WriteSingleReg(WOREVT0, 0x6B);
	WriteSingleReg(WORCTRL, 0xFB);
	WriteSingleReg(FSCAL3, 0xE9);
	WriteSingleReg(FSCAL2, 0x2A);
	WriteSingleReg(FSCAL1, 0x00);
	WriteSingleReg(FSCAL0, 0x1F);
	WriteSingleReg(TEST2, 0x81);
	WriteSingleReg(TEST1, 0x35);
	WriteSingleReg(TEST0, 0x09);
	WriteSingleReg(LQI, 0x7F);
	WriteSingleReg(RSSI, 0x80);
	WriteSingleReg(VCO_VC_DAC, 0x94);
	WriteSinglePATable(0x51);
}

void radio_set_addr(unsigned char addr)
{
	WriteSingleReg(ADDR, addr);
}

void radio_send_data(unsigned char *buffer, unsigned char count)
{
	radio_receive_off();
	transmitting = 1;
	RF1AIES |= BIT9;
	RF1AIFG &= ~BIT9;
	RF1AIE |= BIT9;

	WriteBurstReg(RF_TXFIFOWR, buffer, count);
	Strobe(RF_STX);
}

void radio_receive_on(void)
{
  RF1AIES |= BIT9;                          // Falling edge of RFIFG9
  RF1AIFG &= ~BIT9;                         // Clear a pending interrupt
  RF1AIE  |= BIT9;                          // Enable the interrupt
  Strobe( RF_SRX );
}

void radio_receive_off(void)
{
  RF1AIE &= ~BIT9;                          // Disable RX interrupts
  RF1AIFG &= ~BIT9;                         // Clear pending IFG
  Strobe( RF_SIDLE );
  Strobe( RF_SFRX  );
}


void radio_WOR_mode() //Sets up the RF1A in WOR mode and puts the CPU core in LPM3 mode.
{
	WriteSingleReg(WOREVT1, 0x87);
	WriteSingleReg(WOREVT0, 0x6B);
	WriteSingleReg(MCSM2, 0x00);
	WriteSingleReg(WORCTRL, (7 << 4) | 0);
	WriteSingleReg(IOCFG1, 0x29); // GDO1 = RF_RDY
	while (1)
	{
		Strobe(RF_SWOR);
		wor = 1;
		RF1AIE |= ( (BIT6) << 8 ); // BIT14 = BIT6 << 8
		__bis_SR_register(LPM3_bits + GIE);
	}
}

#pragma vector=CC1101_VECTOR
__interrupt void CC1101_ISR(void)
{
	switch(__even_in_range(RF1AIV,32)) // Prioritizing Radio Core Interrupt
	{
		case 4:                        // GDO1 = LNA_PD signal
			RF1AIE &= ~(BIT1+BIT9);
			Strobe(RF_SWOR);           // Go back to sleep
				break;
		case 20:                       // RFIFG9
			if (!transmitting)         // RX end of packet
			{
				RF1AIE &= ~(BIT1+BIT9);
				RxBufferLength = ReadSingleReg( RXBYTES );
				ReadBurstReg(RF_RXFIFORD, RxBuffer, RxBufferLength);
				radio_incoming_pack(RxBuffer, RxBufferLength);
			} else{
				RF1AIE &= ~BIT9;    // Disable TX end-of-packet interrupt
				transmitting = 0;
			}
				__bic_SR_register_on_exit(LPM3_bits);
				break;
	    case 30:                       // WOR_EVENT0
	    	   RF1AIE |= BIT9 + BIT1;
	    	   RF1AIFG &= ~(BIT9 + BIT1);
	    	   RF1AIES |= BIT9;		   // Falling edge of RFIFG9
	    	   Strobe( RF_SRX );       // Built-in 810us delay in Strobe()
	    	   break;
	    default: break;
	}
}

