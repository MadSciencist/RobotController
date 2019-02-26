#ifndef _MSG_REC_H_
#define _MSG_REC_H_

#include "stm32f4xx.h"
#include "stm32f4xx_it.h"
#include "stm32f4xx_hal_uart.h"
#include "main.h"
#include "usart.h"
#include "msg_gen.h"
#include "PID.h"
#include "converters.h"
#include "Vendor/checksum.h"
#include "robot_params.h"

#define SIZEOF_RECEIVING_BUFFER 14
#define FRAME_START_CHAR '<'
#define FRAME_STOP_CHAR '>'

#define TIMEOUT_MS (500 / 5) // update in 5ms IRQ

typedef enum {  //data from PC to robot, r means robot
  KeepAlive = 0,
  Controls = 1,
  EepromRead = 5,
  EepromWrite = 6,
  AllowMovement = 10,
  StopMovement = 11,
  ControlType = 12,
  RegenerativeBreaking = 13,
  
  PidKp = 100,
  PidKi = 101,
  PidKd = 102,
  PidIntegralLimit = 103,
  PidClamping = 104,
  PidDeadband = 105,
  PidPeriod = 106,
  
  FuzzyKp = 110,
  FuzzyKi = 111,
  FuzzyKd = 112,
  FuzzyIntegralLimit = 113,
  FuzzyClamping = 114,
  FuzzyDeadband = 115,
  FuzzyPeriod = 116,
  
  VoltageAlarm = 130,
  CriticalVoltageAlarm = 131,
  TemperatureAlarm = 132,
  CriticalTemperatureAlarm = 133,
  CurrentLeftAlarm = 134,
  CurrentRightAlarm = 135,
  
  EncoderLeftFilterCoef = 140,
  EncoderRightFilterCoef = 141,
  EncoderLeftIsReversed = 142,
  EncoderRightIsReversed = 143,
  EncoderLeftScaleCoef = 144,
  EncoderRightScaleCoef = 145,
  EncoderLeftFilterIsEnabled = 146,
  EncoderRightFilterIsEnabled = 147,
  
  Hello = 255
} gui2rob_t;


//an artifact after changing architecture from distrbuted RS to signle controller
//still might be helpful while extending architecture to other controllers
typedef enum {
  Master = 0,
  Left = 10,
  Right = 20
} addresses_t;

void start_receiver();

static void parse_data(addresses_t addr, gui2rob_t cmd, uint8_t* payload);

#endif