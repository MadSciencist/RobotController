#ifndef _MSG_REC_H_

#include "stm32f0xx.h"
#include "stm32f0xx_it.h"
#include "stm32f0xx_hal_uart.h"
#include "main.h"
#include "usart.h"
#include "converters.h"
#include "Vendor/checksum.h"

#define SIZEOF_RECEIVING_BUFFER 14
#define FRAME_START_CHAR '<'
#define FRAME_STOP_CHAR '>'

typedef enum {  //data from PC to robot, r means robot
  rRequestAllData = 0,
  
  rEepromRead = 5,
  rEepromWrite = 6,
  
  rAllowMovement = 10,
  rStopMovement,
  
  rMotorP = 100,
  rMotorI,
  rMotorILimit,
  rMotorD,
  rMotorDType,
  rMotorMinSpeed,
  rMotorMaxSpeed,
  rMotorSpeedOffset,
}EGui2rob_t;

void start_receiver();

static void parse_data(uint8_t addr, uint8_t cmd, uint8_t* payload);

#define _MSG_REC_H_
#endif