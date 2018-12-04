#include "converters.h"

/* byte array to variable converters */
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
    return buff[offset] | (buff[offset+1] << 8)  | (buff[offset+2] << 16)  |(buff[offset+3] << 24);
  else if (endian == BIG_ENDIAN)
    return (buff[offset] << 24) | (buff[offset+1] << 16)  |(buff[offset+2] << 8) | buff[offset+3];
  else return 0;
}

uint32_t get_int32(uint8_t* buff, uint8_t offset, uint8_t endian){
  if (endian == LITTLE_ENDIAN)
    return (buff[offset]  | (buff[offset+1] << 8)  | (buff[offset+2] << 16)  | (buff[offset+3] << 24));
  else if (endian == BIG_ENDIAN)
    return (buff[offset] << 24)  | (buff[offset+1] << 16)  | (buff[offset+2] << 8)  | buff[offset+3];
  else return 0;
}

//this need some rework - endianess...
typedef union {  
  float f;  
  uint8_t fBuff[sizeof(float)];  
} floatConverter_t;

typedef union {  
  double d;  
  uint8_t fBuff[sizeof(double)];  
} doubleConverter_t;

float get_float(uint8_t* buff, uint8_t offset, uint8_t endian){
    floatConverter_t conv;
    memcpy(&conv.fBuff[0], &buff[offset], sizeof(float));
    return conv.f;
}

double get_double(uint8_t* buff, uint8_t offset, uint8_t endian){
    doubleConverter_t conv;
    memcpy(&conv.fBuff[0], &buff[offset], sizeof(double));
    return conv.d;
}

/* variable to byte array converters */
uint8_t* get_bytes_from_float(float val){
  static floatConverter_t conv;
  conv.f = val;
  return conv.fBuff;
}

uint8_t* get_bytes_from_double(double val){
  static doubleConverter_t conv;
  conv.d = val;
  return conv.fBuff;
}