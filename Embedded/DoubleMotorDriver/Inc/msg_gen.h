#ifndef _MSG_GEN_H_

#include <string.h>
#include "send_queue_manager.h"
#include "vendor/checksum.h"
#include "converters.h"

typedef enum{ //robot to PC
  TX_KeepAlive = 1,
  TX_FeedbackSpeedCurrent = 10,
  TX_FeedbackVoltageTemperature = 11,
  TX_SendControlType = 12,
  TX_SendRegenerativeBreaking = 13,
  
  TX_VoltageAlarm = 20,
  TX_CriticalVoltageAlarm = 21,
  TX_TemperatureAlarm = 22,
  TX_CriticalTemperatureAlarm = 23,
  TX_CurrentLeftAlarm = 24,
  TX_CurrentRightAlarm = 25,
  
  TX_EepromSaved = 30,
  
  PidKp_1 = 100,
  PidKi_1 = 101,
  PidKd_1 = 102,
  PidIntegralLimit_1 = 103,
  PidClamping_1 = 104,
  PidDeadband_1 = 105,
  PidPeriod_1 = 106,
  
  PidKp_2 = 200,
  PidKi_2 = 201,
  PidKd_2 = 202,
  PidIntegralLimit_2 = 203,
  PidClamping_2 = 204,
  PidDeadband_2 = 205,
  PidPeriod_2 = 206,
} GuiParser_t;

void uart_write_dummy(GuiParser_t cmd);
void uart_write_int16(GuiParser_t cmd, int16_t va1);
void uart_write_uint16(GuiParser_t cmd, int16_t va1);
void uart_write_two_int16(GuiParser_t cmd, int16_t va1, int16_t val2);
void uart_write_four_int16(GuiParser_t cmd, int16_t va1, int16_t val2, int16_t val3, int16_t val4);

void uart_write_float(GuiParser_t cmd, float val);
void uart_write_double(GuiParser_t cmd, double val);

/* private functions */
static void gen_message(GuiParser_t cmd, uint8_t* payload);

#define _MSG_GEN_H_
#endif