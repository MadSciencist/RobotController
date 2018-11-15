#include "msg_rec.h"

uint8_t raw_received[14]; //received buffer

void start_receiver(){
  HAL_UART_Receive_DMA(&huart1, raw_received, 14); 
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
{
  if(raw_received[0] == (uint8_t)FRAME_START_CHAR
     && raw_received[SIZEOF_RECEIVING_BUFFER-1] == (uint8_t)FRAME_STOP_CHAR){
       
       uint16_t frame_crc = (raw_received[12] << 8) | raw_received[11];
       uint16_t calculated_crc = crc_modbus(&raw_received[1], 10);
       
       if(frame_crc == calculated_crc){
         parse_data(raw_received[1], raw_received[2], &raw_received[3]);
       }
     }
}

 double test = 0;
   
static void parse_data(uint8_t addr, uint8_t cmd, uint8_t* payload)
{
  switch (cmd)
  {
  case rAllowMovement:
    test = get_double(payload, 0, LITTLE_ENDIAN);
    int a = 0;
    break;
    
  default:
    break;
  }
}