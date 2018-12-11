#include "msg_gen.h"

//MSG:   < CMD 8_BYTE_PAYLOAD 2_BYTE_CHECKSUM > 

void uart_write_dummy( GuiParser_t cmd){   
  uint8_t payload[16];
  gen_message(cmd, payload);
}
void uart_write_int16(GuiParser_t cmd, int16_t va1){   
  gen_message(cmd, get_bytes_from_uint16(va1, LITTLE_ENDIAN));
}

void uart_write_two_int16(GuiParser_t cmd, int16_t va1, int16_t val2){  
  uint8_t payload[8];
  memcpy(&payload[0], get_bytes_from_int16(va1, LITTLE_ENDIAN), sizeof(int16_t));
  memcpy(&payload[2], get_bytes_from_int16(val2, LITTLE_ENDIAN), sizeof(int16_t));
  gen_message(cmd, payload);
}

void uart_write_four_int16(GuiParser_t cmd, int16_t va1, int16_t val2, int16_t val3, int16_t val4){   
  uint8_t payload[] = {va1, va1 >> 8, val2, val2 >> 8, val3, val3 >> 8, val4, val4 >> 8};
  gen_message(cmd, payload);
}

void uart_write_float(GuiParser_t cmd, float val){
  uint8_t* payload = get_bytes_from_float(val, LITTLE_ENDIAN);
  gen_message(cmd, payload);
}


static void gen_message(GuiParser_t cmd, uint8_t* payload){
  static SendQueueRec_t msg;
  static uint8_t cntr = 0;
  if(cntr == 255) cntr = 0;
  
  //msg start
  msg.buffer[0] = '<';
  
  //header
  msg.buffer[1] = cntr++;
  msg.buffer[2] = cmd;
  
  //8 byte payload
  memcpy(&msg.buffer[3], payload, 8);
  
  //crc - similairy as MODBUS RTU, the msb of CRC is send as second
  uint16_t crc = crc_modbus(&msg.buffer[1], 8);
  uint8_t crc_lsb = crc;
  uint8_t crc_msb = crc >> 8;
  msg.buffer[11] = crc_lsb;
  msg.buffer[12] = crc_msb;
  
  //msg stop
  msg.buffer[13] = '>';
  
  UartSendQueued(&msg);
}