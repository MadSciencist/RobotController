#ifndef _ROBOT_PARAMS_H_

#include "stm32f4xx.h"
#include "main.h"
#include "PID.h"
#include <stdio.h>

typedef struct {
  uint8_t isEncoderReversed; //bool
  float encoderFilterCoef;
} EncoderParams_t;

typedef struct {
  uint8_t isReversed;
  float speed;
  uint16_t current;
  uint16_t currentLimit;
  PID_Properties_t pid;
  EncoderParams_t encoder;
  float deadband;
} DriveParams_t;

typedef struct {
  uint8_t saveEeprom; //bool
  uint8_t readEeprom; //bool
} Requests_t;

typedef struct {
  uint8_t isEnabled; //bool
  uint16_t voltage;
  uint16_t temperature;
} State_t;

typedef struct {
  uint16_t keepAlivePeriod;
  uint16_t feedbackSpeedCurrentPeriod;
  uint16_t feedbackVoltageTemperaturePeriod;
} Timing_t;

typedef enum {
  openLoop = 0,
  closedLoopPID = 1,
  closedLoopFuzzy = 2
} controlType_t;

typedef struct {
  uint16_t voltage;
  uint16_t criticalVoltage;
  uint16_t temperature;
  uint16_t criticalTemperature;
} Alarms_t;

typedef struct{
  controlType_t controlType;
  uint8_t useRegenerativeBreaking; //bool
  State_t state;
  Requests_t requests;
  Alarms_t alarms;
  DriveParams_t driveLeft;
  DriveParams_t driveRight;
  Timing_t timing;
} RobotParams_t;

void init_params(RobotParams_t* params);

#define _ROBOT_PARAMS_H_
#endif