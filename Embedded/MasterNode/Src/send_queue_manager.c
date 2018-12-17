#include "send_queue_manager.h"

Queue_t sendQueue;
SendQueueRec_t dequeuedRec;

void InitSendQueue(){
  q_init(&sendQueue, sizeof(SendQueueRec_t), 30, FIFO, false);
};


void UartSendQueued(SendQueueRec_t* rec){
  q_push(&sendQueue, rec);
  if(huart1.gState != HAL_UART_STATE_BUSY_TX){
    DequeueAndSend();
  }
}

static void DequeueAndSend(){
  if(sendQueue.cnt > 0){
    if(huart1.gState != HAL_UART_STATE_BUSY_TX){
      q_pop(&sendQueue, &dequeuedRec);
      HAL_UART_Transmit_IT(&huart1, dequeuedRec.buffer, sizeof(dequeuedRec.buffer));
    }
  }
}

void HAL_UART_TxCpltCallback(UART_HandleTypeDef *huart){
  if (huart->Instance == USART1){
    DequeueAndSend();
  }
}

void HAL_UART_ErrorCallback(UART_HandleTypeDef *huart){
  if (huart->Instance == USART1){
    static uint8_t error_count = 0;
    error_count++;
  }
}