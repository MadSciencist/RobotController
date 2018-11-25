#include "msg_rec.h"

uint8_t raw_received[14]; //received buffer
extern PID_Properties_t PidPropsLeft, PidPropsRight;

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
    
  case PidKp:
    if(addr == Left)
      PidPropsLeft.kp = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      PidPropsRight.kp = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidKi:
    if(addr == Left)
      PidPropsLeft.ki = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      PidPropsRight.ki = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidKd:
    if(addr == Left)
      PidPropsLeft.kd = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      PidPropsRight.kd = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidIntegralLimit:
    if(addr == Left){
      PidPropsLeft.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      PidPropsLeft.posOutputLimit = PidPropsLeft.posIntegralLimit;
      PidPropsLeft.negIntegralLimit = -PidPropsLeft.posIntegralLimit;
      PidPropsLeft.negOutputLimit = -PidPropsLeft.posOutputLimit;
    }
    else if (addr == Right){
      PidPropsRight.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      PidPropsRight.posOutputLimit = PidPropsLeft.posIntegralLimit;
      PidPropsRight.negIntegralLimit = -PidPropsLeft.posIntegralLimit;
      PidPropsRight.negOutputLimit = -PidPropsLeft.posOutputLimit;
    }
    break;
    
  case PidDeadband:
    //TODO
    break;
    
  case PidPeriod:
    if(addr == Left)
      PidPropsLeft.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      PidPropsRight.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  default:
    break;
  }
}