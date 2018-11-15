#ifndef _MSG_REC_H_

#include "stm32f0xx.h"
#include "stm32f0xx_it.h"
#include "stm32f0xx_hal_uart.h"
#include "main.h"
#include "usart.h"
#include "converters.h"
#include "Vendor/checksum.h"
#include "driver_model.h"

#define SIZEOF_RECEIVING_BUFFER 14
#define FRAME_START_CHAR '<'
#define FRAME_STOP_CHAR '>'

typedef enum {  //data from PC to robot, r means robot
  RequestAllData = 0,
  
  EepromRead = 5,
  EepromWrite = 6,
  
  AllowMovement = 10,
  StopMovement,
  
  MotorP = 100,
  MotorI,
  MotorILimit,
  MotorD,
} gui2rob_t;

typedef enum {
  Master,
  Driver1,
  Driver2
} addresses_t;

void start_receiver();

static void parse_data(addresses_t addr, uint8_t cmd, uint8_t* payload);

#define _MSG_REC_H_
#endif