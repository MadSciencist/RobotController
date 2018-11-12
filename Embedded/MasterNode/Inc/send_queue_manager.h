#ifndef _QUEUE_MANAGER_H

#define SIZEOF_QUEUE_REC 8

#include "stm32f0xx.h"
#include "stm32f0xx_it.h"
#include "stm32f0xx_hal_uart.h"
#include "main.h"
#include "usart.h"
#include "vendor/cQueue.h"


typedef struct SendQueueRecord {
  uint8_t buffer[22];
} SendQueueRec_t;

//public functions
void InitSendQueue();
void UartSendQueued(SendQueueRec_t* rec);

//private
static void DequeueAndSend();

#define _QUEUE_MANAGER_H
#endif