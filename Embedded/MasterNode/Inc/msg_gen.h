#ifndef _MSG_GEN_H_

#include <string.h>
#include "send_queue_manager.h"
#include "vendor/checksum.h"

typedef enum //robot to PC
{
  Broadcast = 0,
  KeepAlive = 1,
  Feedback = 10
} GuiParser_t;

void uart_write_byte(uint8_t cmd, uint8_t data);
void uart_write_two_int16(uint8_t cmd, int16_t va1, int16_t val2);
static void gen_message(uint8_t cmd, uint8_t* payload);

#define _MSG_GEN_H_
#endif