#include "msg_rec.h"

extern RobotParams_t robotParams;

static uint8_t raw_received[14]; //received buffer

void start_receiver(){
  HAL_UART_Receive_DMA(&huart6, raw_received, 14); 
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart){
  if(huart->Instance == USART2){    //RS485

  }else if(huart->Instance == USART6){ // radio modem
    if(raw_received[0] == (uint8_t)FRAME_START_CHAR
       && raw_received[SIZEOF_RECEIVING_BUFFER-1] == (uint8_t)FRAME_STOP_CHAR){
         
         uint16_t frame_crc = (raw_received[12] << 8) | raw_received[11];
         uint16_t calculated_crc = crc_modbus(&raw_received[1], 10);
         
         if(frame_crc == calculated_crc){
           parse_data((addresses_t)raw_received[1], raw_received[2], &raw_received[3]);
         }
       }
    
    start_receiver();
  }
}

static void parse_data(addresses_t addr, uint8_t cmd, uint8_t* payload){
  switch ((gui2rob_t)cmd){
    
  case Controls:
    robotParams.driveLeft.setpoint = (float)get_int16(payload, 0, LITTLE_ENDIAN);
    robotParams.driveRight.setpoint = (float)get_int16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case AllowMovement:
    robotParams.requests.allowMovementChanged = 1;
    robotParams.state.isEnabled = 1;
    break;
    
  case StopMovement:
        robotParams.requests.allowMovementChanged = 1;
    robotParams.state.isEnabled = 0;
    break;
    
  case Hello:
        robotParams.requests.readEeprom = 1;
    break;
    
  case EepromRead:
    robotParams.requests.readEeprom = 1;
    break;
    
  case EepromWrite:
    robotParams.requests.saveEeprom = 1;
    break;
    
  case ControlType:
    robotParams.controlType = (controlType_t)get_uint8(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidKp:
    if(addr == Left)
      robotParams.driveLeft.pid.kp = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.pid.kp = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidKi:
    if(addr == Left){
      float value = get_double(payload, 0, LITTLE_ENDIAN);
      PidSetParams(&robotParams.driveLeft.pid, robotParams.driveLeft.pid.kp, value, robotParams.driveLeft.pid.kd);
    }
    else if (addr == Right){
      float value = get_double(payload, 0, LITTLE_ENDIAN);
      PidSetParams(&robotParams.driveRight.pid, robotParams.driveRight.pid.kp, value, robotParams.driveRight.pid.kd);
    }
    break;
    
  case PidKd:
    if(addr == Left)
      robotParams.driveLeft.pid.kd = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.pid.kd = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case PidIntegralLimit:
    if(addr == Left){
      robotParams.driveLeft.pid.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      robotParams.driveLeft.pid.posOutputLimit = robotParams.driveLeft.pid.posIntegralLimit;
      robotParams.driveLeft.pid.negIntegralLimit = -robotParams.driveLeft.pid.posIntegralLimit;
      robotParams.driveLeft.pid.negOutputLimit = -robotParams.driveLeft.pid.posOutputLimit;
    }
    else if (addr == Right){
      robotParams.driveRight.pid.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      robotParams.driveRight.pid.posOutputLimit =  robotParams.driveRight.pid.posIntegralLimit;
      robotParams.driveRight.pid.negIntegralLimit = - robotParams.driveRight.pid.posIntegralLimit;
      robotParams.driveRight.pid.negOutputLimit = - robotParams.driveRight.pid.posOutputLimit;
    }
    break;
    
  case PidDeadband:
    //TODO
    break;
    
  case PidPeriod:
    if(addr == Left)
      robotParams.driveLeft.pid.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.pid.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case VoltageAlarm:
    robotParams.alarms.voltage = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case CriticalVoltageAlarm:
    robotParams.alarms.criticalVoltage = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case TemperatureAlarm:
    robotParams.alarms.temperature = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case CriticalTemperatureAlarm:
    robotParams.alarms.criticalTemperature = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case CurrentLeftAlarm:
    robotParams.driveLeft.currentLimit = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case CurrentRightAlarm:
    robotParams.driveRight.currentLimit = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderLeftFilterCoef:
    robotParams.driveLeft.encoder.encoderFilterCoef = get_float(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderRightFilterCoef:
    robotParams.driveRight.encoder.encoderFilterCoef = get_float(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderLeftScaleCoef:
    robotParams.driveLeft.encoder.scaleCoef = get_float(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderRightScaleCoef:
    robotParams.driveRight.encoder.scaleCoef = get_float(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderLeftIsReversed:
    robotParams.driveLeft.encoder.isEncoderReversed = get_uint8(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderRightIsReversed:
    robotParams.driveRight.encoder.isEncoderReversed = get_uint8(payload, 0, LITTLE_ENDIAN);
    break;
    
  default:
    break;
  }
}