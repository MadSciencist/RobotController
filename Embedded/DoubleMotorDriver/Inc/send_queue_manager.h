#ifndef _QUEUE_MANAGER_H

#define MAX_QUEUE_LEN 20 // warning: this cause memory allocation of SendQueueRec_t* len

#include "stm32f4xx.h"
#include "stm32f4xx_it.h"
#include "stm32f4xx_hal_uart.h"
#include "main.h"
#include "usart.h"
#include "vendor/cQueue.h"


typedef struct SendQueueRecord {
  uint8_t buffer[14];
} SendQueueRec_t;

//public functions
void InitSendQueue();
void UartSendQueued(SendQueueRec_t *rec);

//private
static void DequeueAndSend();

#define _QUEUE_MANAGER_H
#endif