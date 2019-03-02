#include "requests.h"

void send_feedback(RobotParams_t* params){
  static unsigned long keepAliveprevTicks = 0;
  static unsigned long voltageTemperaturePrevTicks = 0;
  static unsigned long speedCurrentPrevTicks = 0;
  
  unsigned long ticks = HAL_GetTick();
  
  if ((ticks - keepAliveprevTicks) >= params->timing.keepAlivePeriod) {
    keepAliveprevTicks = ticks;
    
    uart_write_dummy( TX_KeepAlive );
  }
  
  if ((ticks - voltageTemperaturePrevTicks) >= params->timing.feedbackVoltageTemperaturePeriod) {
    voltageTemperaturePrevTicks = ticks;
    
    uart_write_two_int16( TX_FeedbackVoltageTemperature, params->state.voltage, params->state.temperature );
  }
  
  if ((ticks - speedCurrentPrevTicks) >= params->timing.feedbackSpeedCurrentPeriod) {
    speedCurrentPrevTicks = ticks;
    
    uart_write_four_int16( TX_FeedbackSpeedCurrent, 
                          (int16_t)params->driveLeft.speed, 
                          (int16_t)params->driveLeft.current, 
                          (int16_t)params->driveRight.speed, 
                          (int16_t)params->driveRight.current );
  }
}

void process_requests(RobotParams_t* params, uint16_t params_len){
  if(params->requests.restartReceiver){
    //MX_USART1_UART_Init();
    start_receiver();
    params->requests.restartReceiver = 0;
  }else if(params->requests.readEeprom){
    ReadFromFlash(params, params_len, SECTOR5_FLASH_BEGINING);
    
    uart_write_int16(TX_SendControlType, (uint8_t)params->controlType);
    uart_write_int16(TX_SendRegenerativeBreaking, (uint8_t)params->useRegenerativeBreaking);
    
    /* PID controller params */ 
    uart_write_float(PidKp_1, params->driveLeft.pid.kp);
    uart_write_float(PidKi_1, params->driveLeft.pid.ki);
    uart_write_float(PidKd_1, params->driveLeft.pid.kd);
    uart_write_float(PidIntegralLimit_1, params->driveLeft.pid.posIntegralLimit);
    uart_write_float(PidDeadband_1, params->driveLeft.pid.deadband);
    uart_write_int16(PidPeriod_1, params->driveLeft.pid.period);
    
    HAL_Delay(20); //small delay, so we dont overflow the 20 element queue
    
    uart_write_float(PidKp_2, params->driveRight.pid.kp);
    uart_write_float(PidKi_2, params->driveRight.pid.ki);
    uart_write_float(PidKd_2, params->driveRight.pid.kd);
    uart_write_float(PidIntegralLimit_2, params->driveRight.pid.posIntegralLimit);
    uart_write_float(PidDeadband_2, params->driveRight.pid.deadband);
    uart_write_int16(PidPeriod_2, params->driveRight.pid.period);
    /* END PID controller params */
    
    HAL_Delay(20);
    
    /* fuzzy controller params */
    uart_write_float(FuzzyKp_1, params->driveLeft.fuzzy.kp);
    uart_write_float(FuzzyKi_1, params->driveLeft.fuzzy.ki);
    uart_write_float(FuzzyKd_1, params->driveLeft.fuzzy.kd);
    uart_write_float(FuzzyIntegralLimit_1, params->driveLeft.fuzzy.posIntegralLimit);
    uart_write_float(FuzzyDeadband_1, params->driveLeft.fuzzy.deadband);
    uart_write_int16(FuzzyPeriod_1, params->driveLeft.fuzzy.period);
    
    HAL_Delay(20);
    
    uart_write_float(FuzzyKp_2, params->driveRight.fuzzy.kp);
    uart_write_float(FuzzyKi_2, params->driveRight.fuzzy.ki);
    uart_write_float(FuzzyKd_2, params->driveRight.fuzzy.kd);
    uart_write_float(FuzzyIntegralLimit_2, params->driveRight.fuzzy.posIntegralLimit);
    uart_write_float(FuzzyDeadband_2, params->driveRight.fuzzy.deadband);
    uart_write_int16(FuzzyPeriod_2, params->driveRight.fuzzy.period);
    /* END fuzzy controller params */
    
    HAL_Delay(20);
    
    uart_write_uint16(TX_VoltageAlarm, params->alarms.voltage);
    uart_write_uint16(TX_CriticalVoltageAlarm, params->alarms.criticalVoltage);
    uart_write_uint16(TX_TemperatureAlarm, params->alarms.temperature);
    uart_write_uint16(TX_CriticalTemperatureAlarm, params->alarms.criticalTemperature);
    uart_write_int16(TX_CurrentLeftAlarm, params->driveLeft.currentLimit);
    uart_write_int16(TX_CurrentRightAlarm, params->driveRight.currentLimit);
    
    HAL_Delay(20);
    
    uart_write_float(EncoderFilterCoef_1, params->driveLeft.encoder.encoderFilterCoef);
    uart_write_float(EncoderScaleCoef_1, params->driveLeft.encoder.scaleCoef);
    uart_write_uint16(EncoderIsReversed_1, params->driveLeft.encoder.isEncoderReversed);
    uart_write_uint16(EncoderIsFilterEnabled_1, params->driveLeft.encoder.isFilterEnabled);
    uart_write_float(EncoderFilterCoef_2, params->driveRight.encoder.encoderFilterCoef);
    uart_write_float(EncoderScaleCoef_2, params->driveRight.encoder.scaleCoef);
    uart_write_uint16(EncoderIsReversed_2, params->driveRight.encoder.isEncoderReversed);
    uart_write_uint16(EncoderIsFilterEnabled_2, params->driveRight.encoder.isFilterEnabled);
    
    params->requests.readEeprom = 0;
    
  }else if( params->requests.saveEeprom == 1){
    WriteToFlash(params, params_len, SECTOR5_FLASH_BEGINING, FLASH_SECTOR_5, FLASH_VOLTAGE_RANGE_3);
    uart_write_int16(TX_EepromSaved, 0);
    
    //clear flag as we already processed this request
    params->requests.saveEeprom = 0;
  }
  else if( params->requests.allowMovementChanged == 1){
    if(params->state.isEnabled == 1){
      ResetIntegrator(&(params->driveLeft.pid));
      ResetIntegrator(&(params->driveRight.pid));
      enable_motors();
    }else {
      disable_motors();
    }
    
    //clear flag as we already processed this request
    params->requests.allowMovementChanged = 0;
  }
}