/********************************************************************************************
  Copyright 2007-2012 Texas Instruments Incorporated. All rights reserved.

  IMPORTANT: Your use of this Software is limited to those specific rights granted under
  the terms of a software license agreement between the user who downloaded the software,
  his/her employer (which must be your employer) and Texas Instruments Incorporated (the
  "License"). You may not use this Software unless you agree to abide by the terms of the
  License. The License limits your use, and you acknowledge, that the Software may not be
  modified, copied or distributed unless embedded on a Texas Instruments microcontroller
  or used solely and exclusively in conjunction with a Texas Instruments radio frequency
  transceiver, which is integrated into your product. Other than for the foregoing purpose,
  you may not use, reproduce, copy, prepare derivative works of, modify, distribute,
  perform, display or sell this Software and/or its documentation for any purpose.

  YOU FURTHER ACKNOWLEDGE AND AGREE THAT THE SOFTWARE AND DOCUMENTATION ARE PROVIDED “AS IS”
  WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION, ANY
  WARRANTY OF MERCHANTABILITY, TITLE, NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
  IN NO EVENT SHALL TEXAS INSTRUMENTS OR ITS LICENSORS BE LIABLE OR OBLIGATED UNDER CONTRACT,
  NEGLIGENCE, STRICT LIABILITY, CONTRIBUTION, BREACH OF WARRANTY, OR OTHER LEGAL EQUITABLE
  THEORY ANY DIRECT OR INDIRECT DAMAGES OR EXPENSES INCLUDING BUT NOT LIMITED TO ANY
  INCIDENTAL, SPECIAL, INDIRECT, PUNITIVE OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST
  DATA, COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY, SERVICES, OR ANY CLAIMS BY
  THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE THEREOF), OR OTHER SIMILAR COSTS.

  Should you have any questions regarding your right to use this Software,
  contact Texas Instruments Incorporated at www.TI.com.
**************************************************************************************************/
/******************************************************************************
 * INCLUDES
 */
#include <msp430.h>
#include <stdbool.h>
#include "bsp.h"
#include "mrfi.h"
#include "nwk_types.h"
#include "nv_obj.h"


/******************************************************************************
 * MACROS
 */

/******************************************************************************
 * CONSTANTS AND DEFINES
 */

// segment C, D for storing network configuration
#define INFO_FLASH_NWK_CFG_START_ADDR         (0x1000)
#define INFO_FLASH_NWK_CFG_SIZE               (64 * 2)

// segment B for storing link handle
#define INFO_FLASH_LNK_ID_START_ADDR          (0x1080)
#define INFO_FLASH_LNK_ID_SIZE                (64)

/******************************************************************************
 * TYPEDEFS
 */

/******************************************************************************
 * LOCAL VARIABLES
 */


/******************************************************************************
 * LOCAL FUNCTIONS
 */


/******************************************************************************
 * GLOBAL VARIABLES
 */

/******************************************************************************
 * GLOBAL FUNCTIONS
 */

/******************************************************************************
 * @fn         nv_obj_init
 *
 * @brief      initialize nv obj module
 *
 * @param      -
 * 
 * @return     -
 */
void nv_obj_init(void)
{
  //FCTL2 = FWKEY + FSSEL_1 + 20;      // MCLK(8MHz)/20 for Flash Timing Generator
}

/******************************************************************************
 * @fn         nv_obj_read_nwk_cfg
 *
 * @brief      read/check flash memory if an existing network configuration exists
 *
 * @param      -
 * 
 * @return     NULL if no existing network configuration exists, otherwise 
 *             valid pointer to existing network configuration
 */
void* nv_obj_read_nwk_cfg(void)
{
  void* ret = NULL; 
  uint8_t *byte = (uint8_t*) INFO_FLASH_NWK_CFG_START_ADDR;
  
  /* check for the first for bytes which should be the signature if valid 
     network configuration exists */
  if((byte[0] == 'S') && (byte[1] == 'M') && 
     (byte[2] == 'P') && (byte[3] == 'L'))
  {
    ret = (void*) &byte[4];
  }
  
  return ret;
}

/******************************************************************************
 * @fn         nv_obj_write_nwk_cfg
 *
 * @brief      write the network configuration into the flash info memory
 *
 * @param      nwk_cfg    pointer to the network configuration to be written
 *                        inside the flash info memory
 * @param      len        length of the network configuration
 * 
 * @return     true if succeeds, flash if fails
*/
bool nv_obj_write_nwk_cfg(void* nwk_cfg, uint16_t len)
{
  bool ret = false;  
  uint8_t *Flash_ptr = (uint8_t*) INFO_FLASH_NWK_CFG_START_ADDR, *data;
  uint16_t i=0, blk_wrt_cnt;
  
  if((nwk_cfg != NULL) && (len <= INFO_FLASH_NWK_CFG_SIZE - 4))
  {
    // erase the memory segment
    nv_obj_erase_flash();
 
    // write the signature
    /* start block write */
    FCTL3 = FWKEY;                  // Clear Lock bit
    FCTL1 = FWKEY + WRT;            // Set WRT bit for write operation
      
    *Flash_ptr++ = 'S';             // Write signature
    *Flash_ptr++ = 'M';             // Write signature
    *Flash_ptr++ = 'P';             // Write signature
    *Flash_ptr++ = 'L';             // Write signature
  
    FCTL1 = FWKEY;                  // Clear WRT bit
    FCTL3 = FWKEY + LOCK;           // Set LOCK bit
  
    // write the data
    data = (uint8_t*) nwk_cfg;
    do
    {
      /* start block write */
      FCTL3 = FWKEY;                  // Clear Lock bit
      FCTL1 = FWKEY + WRT;            // Set BLKWRT + WRT bit for block write operation
      
      for (blk_wrt_cnt = 0 ; 
           (blk_wrt_cnt < 64) &&  (i<len); 
           i++, blk_wrt_cnt++)
      {
        *Flash_ptr++ = *data++;         // Write value to flash
      }

      FCTL1 = FWKEY;                    // Clear WRT bit
      FCTL3 = FWKEY + LOCK;             // Set LOCK bit
    }while(i<len);
    
    // return success
    ret = true;
  }
   
  return ret;
}

/******************************************************************************
 * @fn         nv_obj_erase_flash
 *
 * @brief      read/check flash memory if an existing network configuration exists
 *
 * @param      - 
 * 
 * @return     -
*/
void nv_obj_erase_flash(void)
{
  uint8_t *Flash_ptr = (uint8_t*) INFO_FLASH_NWK_CFG_START_ADDR, i;

  FCTL3 = FWKEY;                            // Clear Lock bit
  FCTL1 = FWKEY + ERASE ;              // Set Erase bit, allow interrupts

  // erase segment B, C, D
  for(i=0 ; i<3 ; i++)
  {
    *Flash_ptr = 0;                           // Dummy write to erase Flash seg
    
    Flash_ptr += 64;
  }

  FCTL1 = FWKEY;                            // Clear ERASE bit
  FCTL3 = FWKEY + LOCK;                     // Set LOCK bit
}

/******************************************************************************
 * @fn         nv_obj_read_lnk_id
 *
 * @brief      read a link ID from flash memory
 *
 * @param      index    connection index
* 
 * @return     -1 if fails, others if success
*/
linkID_t nv_obj_read_lnk_id(uint8_t index)
{
  linkID_t ret = (linkID_t) -1;
  
  // check valid index
  if(index < NUM_CONNECTIONS)
  {
    ret = ((linkID_t*)INFO_FLASH_LNK_ID_START_ADDR)[index];
  }
  
  return ret;
}


/******************************************************************************
 * @fn         nv_obj_write_nwk_cfg
 *
 * @brief      write the network configuration into the flash info memory
 *
 * @param      nwk_cfg    pointer to the network configuration to be written
 *                        inside the flash info memory
 * @param      len        length of the network configuration
 * 
 * @return     true if succeeds, flash if fails
*/
bool nv_obj_write_lnk_id(uint8_t index, linkID_t linkId)
{
  bool ret = false;  
  linkID_t *Flash_ptr = (linkID_t*) INFO_FLASH_LNK_ID_START_ADDR;
  
  if(index < NUM_CONNECTIONS)
  {
    // write the signature
    /* start block write */
    FCTL3 = FWKEY;                  // Clear Lock bit
    FCTL1 = FWKEY + WRT;            // Set WRT bit for write operation
      
    Flash_ptr[index] = linkId;      // write link ID
  
    FCTL1 = FWKEY;                  // Clear WRT bit
    FCTL3 = FWKEY + LOCK;           // Set LOCK bit
  
    ret = true;
  }
  
  return ret;
}

