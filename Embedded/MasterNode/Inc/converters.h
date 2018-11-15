#ifndef _CONVERTERS_H_

#include "stm32f0xx.h"
#include "stm32f0xx_it.h"
#include "stm32f0xx_hal_uart.h"
#include "main.h"
#include <stdio.h>
#include <string.h>

#define BIG_ENDIAN 0
#define LITTLE_ENDIAN 1

uint16_t get_int8(uint8_t* buff, uint8_t offset, uint8_t endian);
uint16_t get_uint8(uint8_t* buff, uint8_t offset, uint8_t endian);

uint16_t get_int16(uint8_t* buff, uint8_t offset, uint8_t endian);
uint16_t get_uint16(uint8_t* buff, uint8_t offset, uint8_t endian);

uint32_t get_int32(uint8_t* buff, uint8_t offset, uint8_t endian);
uint32_t get_uint32(uint8_t* buff, uint8_t offset, uint8_t endian);

float get_float(uint8_t* buff, uint8_t offset, uint8_t endian);
double get_double(uint8_t* buff, uint8_t offset, uint8_t endian);

#define _CONVERTERS_H_
#endif