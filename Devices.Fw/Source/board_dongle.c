#include "board_dongle.h"

static linkID_t sLID[NUM_CONNECTIONS] = {0};
static uint8_t  sNumCurrentPeers = 0;
static uint8_t RadioCallback(linkID_t);
static volatile uint8_t sPeerFrameSem = 0;
static volatile uint8_t sJoinSem = 0;
bspIState_t intState;
ioctlNVObj_t nwk_cfg_ram;
uint8_t *data = NULL;;

static void processMessage(linkID_t lid, uint8_t *msg, uint8_t len);
unsigned short crc16(volatile unsigned char *sbuf,unsigned char len);

extern void nwkGetRemotePeerAddr(linkID_t sLinkId, addr_t *peerAddr);

void radio_init()
{
  void *nwk_cfg_flash;
  ioctlNVObj_t nwk_cfg_ram;
  uint8_t *data = NULL;
  BSP_Init();
  SMPL_Init(RadioCallback);
  nwk_cfg_flash = nv_obj_read_nwk_cfg();
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
}

void radio_poll()
{
   if (sJoinSem && (sNumCurrentPeers < NUM_CONNECTIONS))
    {
      while (1)
      {
        if (SMPL_SUCCESS == SMPL_LinkListen(&sLID[sNumCurrentPeers]))
        {
          break;
        }
      }
      sNumCurrentPeers++;
      BSP_ENTER_CRITICAL_SECTION(intState);
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

static uint8_t RadioCallback(linkID_t lid)
{
  if (lid)
  {
    sPeerFrameSem++;
  } else {
    sJoinSem++;
  }
  return 0;
}


static char send_uart_packet(uint8_t *msg, uint8_t len){
    unsigned int i = 0;
    unsigned short crc = crc16(msg, len);
    uart_write_byte(0x55);
    uart_write_byte(0xAA);
    uart_write_byte(len);
    uart_write_array(msg, len);
    uart_write_byte(crc>>8);
    uart_write_byte(crc&0xFF);
    uart_write_byte(0xAA);
    uart_write_byte(0x55);
    while(uart_received()==0 && i++<6000);
    if(uart_received()){
      if(uart_read_byte() == 0x01)return 1;
    }
    return 0;
}

static void processMessage(linkID_t lid, uint8_t *msg, uint8_t len)
{
  char resp; 
  addr_t peerAddr;
  int i = 0;
  char pack[50];
  nwkGetRemotePeerAddr(lid, &peerAddr);
  pack[0] = peerAddr.addr[0];
  pack[1] = peerAddr.addr[1];
  pack[2] = peerAddr.addr[2];
  pack[3] = peerAddr.addr[3];
  rssi_t rssi_value;
  SMPL_Ioctl( IOCTL_OBJ_RADIO, IOCTL_ACT_RADIO_RSSI, &rssi_value);
  pack[4] = rssi_value;
  for(i=0;i<len;i++) pack[i+5] = *msg++;
  if(send_uart_packet(pack, len+5)==0){
    send_uart_packet(pack, len+5);
  }
  return;
}

unsigned short crc16(volatile unsigned char *sbuf,unsigned char len)
{
    unsigned short crc=0xFFFF;

    while(len){
        crc=(unsigned char)(crc >> 8) | (crc << 8);
        crc^=(unsigned char) *sbuf;
        crc^=(unsigned char)(crc & 0xff) >> 4;
        crc^=(crc << 8) << 4;
        crc^=((crc & 0xff) << 4) << 1;
        len--;
        sbuf++;
    }
    return crc;
}
