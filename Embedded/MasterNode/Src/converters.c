#include "converters.h"

uint16_t get_int8(uint8_t* buff, uint8_t offset, uint8_t endian){
  return buff[offset];
}

uint16_t get_uint8(uint8_t* buff, uint8_t offset, uint8_t endian){
  return buff[offset];
}

uint16_t get_uint16(uint8_t* buff, uint8_t offset, uint8_t endian){
  if (endian == LITTLE_ENDIAN)
    return (buff[offset+1] << 8) | buff[offset];
  else if(endian == BIG_ENDIAN)
    return (buff[offset] << 8) | buff[offset+1];
  else return 0;
}

uint16_t get_int16(uint8_t* buff, uint8_t offset, uint8_t endian){
  if (endian == LITTLE_ENDIAN)
    return (buff[offset+1] << 8) | buff[offset];
  else  if(endian == BIG_ENDIAN)
    return (buff[offset] << 8) | buff[offset+1];
  else return 0;
}

uint32_t get_uint32(uint8_t* buff, uint8_t offset, uint8_t endian){
  if (endian == LITTLE_ENDIAN)
    return buff[offset] + (buff[offset+1] << 8) + (buff[offset+2] << 16) + (buff[offset+3] << 24);
  else if (endian == BIG_ENDIAN)
    return (buff[offset] << 24) + (buff[offset+1] << 16) + (buff[offset+2] << 8) + buff[offset+3];
  else return 0;
}

uint32_t get_int32(uint8_t* buff, uint8_t offset, uint8_t endian){
  if (endian == LITTLE_ENDIAN)
    return (buff[offset] + (buff[offset+1] << 8) + (buff[offset+2] << 16) + (buff[offset+3] << 24));
  else if (endian == BIG_ENDIAN)
    return (buff[offset] << 24) + (buff[offset+1] << 16) + (buff[offset+2] << 8) + buff[offset+3];
  else return 0;
}