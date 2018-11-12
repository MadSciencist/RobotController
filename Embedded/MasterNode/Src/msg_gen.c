#include "msg_gen.h"

//MSG:   < CMD 10_BYTE_PAYLOAD 2_BYTE_CHECKSUM > 
void uart_write_byte(uint8_t cmd, uint8_t data)
{
  uint8_t payload[] = {data, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
  gen_message(cmd, payload);
}

void uart_write_two_int16( uint8_t cmd, int16_t va1, int16_t val2)
{   
  uint8_t payload[] = {va1, va1 >> 8, val2, val2 >> 8, 0, 0, 0, 0, val2, val2 >> 8, 0, 0, 0, 0, 0, 0};
 gen_message(cmd, payload);
}

SendQueueRec_t msg;

static void gen_message(uint8_t cmd, uint8_t* payload)
{
  static uint8_t cntr = 0;
  if(cntr == 255) cntr = 0;
  
  //msg start
  msg.buffer[0] = '<';
  
  //header
  msg.buffer[1] = cntr++;
  msg.buffer[2] = cmd;
  
  //10 byte payload
  memcpy(&msg.buffer[3], payload, 16);
  
  //crc - similairy as MODBUS RTU, the msb of CRC is send as second
  uint16_t crc = crc_modbus(&msg.buffer[1], 16);
  uint8_t crc_lsb = crc;
  uint8_t crc_msb = crc >> 8;
  msg.buffer[19] = crc_lsb;
  msg.buffer[20] = crc_msb;
  
  //msg stop
  msg.buffer[21] = '>';
  
  UartSendQueued(&msg);
}