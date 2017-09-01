/*
 * Ѕиблиотека дл€ работы с UART на микроконтроллерер CC430
 * 
 *
 */
 
void uart_init(void); // P1.6 - TX, P1.5 - RX, Buadrate: 19200
void uart_write_byte(char data);
void uart_write_array(char *arr, unsigned char len);
char uart_received();
char uart_read_byte();