#ifndef _MSG_GEN_H_

#include <string.h>
#include "send_queue_manager.h"
#include "vendor/checksum.h"

typedef enum //robot to PC
{
  KeepAlive = 1,
  FeedbackSpeedCurrent = 10,
  FeedbackVoltageTemperature = 11
} GuiParser_t;

void uart_write_dummy( GuiParser_t cmd);
void uart_write_two_int16(GuiParser_t cmd, int16_t va1, int16_t val2);
void uart_write_four_int16( GuiParser_t cmd, int16_t va1, int16_t val2, int16_t val3, int16_t val4);

/* private functions */
static void gen_message(GuiParser_t cmd, uint8_t* payload);

#define _MSG_GEN_H_
#endif