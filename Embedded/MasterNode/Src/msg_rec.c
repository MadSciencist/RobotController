#include "msg_rec.h"

uint8_t raw_received[14]; //received buffer
extern driver_t *drv1, *drv2;

void start_receiver(){
  HAL_UART_Receive_DMA(&huart1, raw_received, 14); 
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart){
  if(raw_received[0] == (uint8_t)FRAME_START_CHAR
     && raw_received[SIZEOF_RECEIVING_BUFFER-1] == (uint8_t)FRAME_STOP_CHAR){
       
       uint16_t frame_crc = (raw_received[12] << 8) | raw_received[11];
       uint16_t calculated_crc = crc_modbus(&raw_received[1], 10);
       
       if(frame_crc == calculated_crc){
         parse_data((addresses_t)raw_received[1], raw_received[2], &raw_received[3]);
       }
     }
}

static void parse_data(addresses_t addr, uint8_t cmd, uint8_t* payload){
  switch ((gui2rob_t)cmd){
  case AllowMovement:
    break;
    
  case MotorP:
    if(addr == Driver1)
      drv1->kp = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Driver2)
      drv2->kp = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case MotorI:
    if(addr == Driver1)
      drv1->ki = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Driver2)
      drv2->ki = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case MotorD:
    if(addr == Driver1)
      drv1->kd = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Driver2)
      drv2->kd = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case MotorILimit:
    if(addr == Driver1)
      drv1->i_lim = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Driver2)
      drv2->i_lim = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  default:
    break;
  }
}