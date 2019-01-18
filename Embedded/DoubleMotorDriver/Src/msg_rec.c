#include "msg_rec.h"

extern RobotParams_t robotParams;
static uint8_t raw_received[14]; //received buffer
uint16_t rec_timeout_ms;

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
         
         if(frame_crc == calculated_crc){ //got valid msg
           rec_timeout_ms = TIMEOUT_MS;
           parse_data((addresses_t)raw_received[1], (gui2rob_t)raw_received[2], &raw_received[3]);
         }
       }
  }
}

static void parse_data(addresses_t addr, gui2rob_t cmd, uint8_t* payload){
  switch (cmd){
    
  case Controls:
    robotParams.driveLeft.setpoint = (float)get_int16(payload, 0, LITTLE_ENDIAN);
    robotParams.driveRight.setpoint = (float)get_int16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case AllowMovement:
    robotParams.requests.allowMovementChanged = 1;
    robotParams.state.isEnabled = 1;
    uart_write_int16(TX_MovementEnabled, 0);
    break;
    
  case StopMovement:
    robotParams.requests.allowMovementChanged = 1;
    robotParams.state.isEnabled = 0;
    uart_write_int16(TX_MovementDisabled, 0);
    break;
    
  case Hello:
    robotParams.requests.readEeprom = 1;
    break;
    
  case EepromRead:
    robotParams.state.isEnabled = 0;
    robotParams.requests.readEeprom = 1;
    robotParams.state.isEnabled = 0;
    break;
    
  case EepromWrite:
    robotParams.state.isEnabled = 0;
    robotParams.requests.saveEeprom = 1;
    robotParams.state.isEnabled = 0;
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
      robotParams.driveLeft.pid.ki = get_double(payload, 0, LITTLE_ENDIAN);
    }
    else if (addr == Right){
      robotParams.driveRight.pid.ki = get_double(payload, 0, LITTLE_ENDIAN);
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
      robotParams.driveLeft.pid.negIntegralLimit = -1.0f * robotParams.driveLeft.pid.posIntegralLimit;
      robotParams.driveLeft.pid.negOutputLimit = -1.0f * robotParams.driveLeft.pid.posOutputLimit;
    }
    else if (addr == Right){
      robotParams.driveRight.pid.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      robotParams.driveRight.pid.posOutputLimit =  robotParams.driveRight.pid.posIntegralLimit;
      robotParams.driveRight.pid.negIntegralLimit = -1.0f * robotParams.driveRight.pid.posIntegralLimit;
      robotParams.driveRight.pid.negOutputLimit = -1.0f * robotParams.driveRight.pid.posOutputLimit;
    }
    break;
    
  case PidDeadband:
    if(addr == Left){
      robotParams.driveLeft.pid.deadband = get_double(payload, 0, LITTLE_ENDIAN);
    }else if(addr == Right){
      robotParams.driveRight.pid.deadband = get_double(payload, 0, LITTLE_ENDIAN);
    }
    break;
    
  case PidPeriod:
    if(addr == Left)
      robotParams.driveLeft.pid.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.pid.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    break;
    
  case FuzzyKp:
    if(addr == Left)
      robotParams.driveLeft.fuzzy.kp = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.fuzzy.kp = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case FuzzyKi:
    if(addr == Left)
      robotParams.driveLeft.fuzzy.ki = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.fuzzy.ki = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
  case FuzzyKd:
    if(addr == Left)
      robotParams.driveLeft.fuzzy.kd = get_double(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.fuzzy.kd = get_double(payload, 0, LITTLE_ENDIAN);
    break;
    
    
  case FuzzyIntegralLimit:
    if(addr == Left){
      robotParams.driveLeft.fuzzy.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      robotParams.driveLeft.fuzzy.posOutputLimit = robotParams.driveLeft.fuzzy.posIntegralLimit;
      robotParams.driveLeft.fuzzy.negIntegralLimit = -1.0f * robotParams.driveLeft.fuzzy.posIntegralLimit;
      robotParams.driveLeft.fuzzy.negOutputLimit = -1.0f * robotParams.driveLeft.fuzzy.posOutputLimit;
    }
    else if (addr == Right){
      robotParams.driveRight.fuzzy.posIntegralLimit = get_double(payload, 0, LITTLE_ENDIAN);
      robotParams.driveRight.fuzzy.posOutputLimit =  robotParams.driveRight.fuzzy.posIntegralLimit;
      robotParams.driveRight.fuzzy.negIntegralLimit = -1.0f * robotParams.driveRight.fuzzy.posIntegralLimit;
      robotParams.driveRight.fuzzy.negOutputLimit = -1.0f * robotParams.driveRight.fuzzy.posOutputLimit;
    }
    break;
    
  case FuzzyDeadband:
    if(addr == Left){
      robotParams.driveLeft.fuzzy.deadband = get_double(payload, 0, LITTLE_ENDIAN);
    }else if(addr == Right){
      robotParams.driveRight.fuzzy.deadband = get_double(payload, 0, LITTLE_ENDIAN);
    }
    break;
    
  case FuzzyPeriod:
    if(addr == Left)
      robotParams.driveLeft.fuzzy.period = get_uint16(payload, 0, LITTLE_ENDIAN);
    else if (addr == Right)
      robotParams.driveRight.fuzzy.period = get_uint16(payload, 0, LITTLE_ENDIAN);
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
    
  case EncoderLeftFilterIsEnabled:
    robotParams.driveLeft.encoder.isFilterEnabled = get_uint8(payload, 0, LITTLE_ENDIAN);
    break;
    
  case EncoderRightFilterIsEnabled:
    robotParams.driveRight.encoder.isFilterEnabled = get_uint8(payload, 0, LITTLE_ENDIAN);
    break;
  default:
    break;
  }
}