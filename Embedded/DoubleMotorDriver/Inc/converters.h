#ifndef _CONVERTERS_H_

#include "stm32f4xx.h"
#include "stm32f4xx_it.h"
#include "stm32f4xx_hal_uart.h"
#include "main.h"
#include <stdio.h>
#include <string.h>

typedef enum{
  BIG_ENDIAN = 0,
  LITTLE_ENDIAN = 1
} endian_t;

uint16_t get_int8(uint8_t* buff, uint8_t offset, uint8_t endian);
uint16_t get_uint8(uint8_t* buff, uint8_t offset, uint8_t endian);

uint16_t get_int16(uint8_t* buff, uint8_t offset, uint8_t endian);
uint16_t get_uint16(uint8_t* buff, uint8_t offset, uint8_t endian);

uint32_t get_int32(uint8_t* buff, uint8_t offset, uint8_t endian);
uint32_t get_uint32(uint8_t* buff, uint8_t offset, uint8_t endian);

float get_float(uint8_t* buff, uint8_t offset, uint8_t endian);
double get_double(uint8_t* buff, uint8_t offset, uint8_t endian);

/* variable to byte array */
uint8_t* get_bytes_from_int8(int8_t val, endian_t endian);
uint8_t* get_bytes_from_uint8(uint8_t val, endian_t endian);

uint8_t* get_bytes_from_int16(int16_t val, endian_t endian);
uint8_t* get_bytes_from_uint16(uint16_t val, endian_t endian);

uint8_t* get_bytes_from_float(float val, endian_t endian);
uint8_t* get_bytes_from_double(double val, endian_t endian);
                                
#define _CONVERTERS_H_
#endif