#include "send_queue_manager.h"

extern RobotParams_t robotParams;
Queue_t sendQueue;
SendQueueRec_t dequeuedRec;

void InitSendQueue(){
  q_init(&sendQueue, sizeof(SendQueueRec_t), MAX_QUEUE_LEN, FIFO, false);
};


void UartSendQueued(SendQueueRec_t* rec){
  static uint32_t queue_full_lost_msg_cnt = 0;
  
  if(q_isFull(&sendQueue)){
    queue_full_lost_msg_cnt++;
    return;
  }
  
  q_push(&sendQueue, rec);
  DequeueAndSend();
}

static void DequeueAndSend(){
  if(sendQueue.cnt > 0){
    if(huart6.gState != HAL_UART_STATE_BUSY_TX){
      q_pop(&sendQueue, &dequeuedRec);
      HAL_UART_Transmit_IT(&huart6, dequeuedRec.buffer, sizeof(dequeuedRec.buffer));
    }
  }
}

void HAL_UART_TxCpltCallback(UART_HandleTypeDef *huart){
  if (huart->Instance == USART6){
    DequeueAndSend();
    start_receiver();
  }
}

void HAL_UART_ErrorCallback(UART_HandleTypeDef *huart){
  if (huart->Instance == USART1){
    static uint8_t error_count_uart1 = 0;
    static __IO uint32_t  code_uart1 = 0;
    code_uart1 = huart->ErrorCode;
    error_count_uart1++;
    robotParams.requests.restartReceiver = 1;
    //start_receiver();
  }
  else if (huart->Instance == USART6){
    static uint8_t error_count_uart6 = 0;
    static __IO uint32_t  code_uart6 = 0;
    code_uart6 = huart->ErrorCode;
    error_count_uart6++;
  }
}