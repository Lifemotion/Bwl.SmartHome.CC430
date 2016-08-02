/*
 * radio.h
 *
 *  Created on: 30 июля 2016 г.
 *      Author: gus10
 */

#ifndef LIBS_RADIO_H_
#define LIBS_RADIO_H_

void radio_init();
void radio_send_data(unsigned char *buffer, unsigned char count);
void ResetRadioCore (void);
void radio_receive_on(void);
void radio_receive_off(void);
void radio_WOR_mode();
void radio_set_addr(unsigned char addr);

//callback fun in you code
void radio_incoming_pack(unsigned char *buffer, unsigned char len);


#endif /* LIBS_RADIO_H_ */
