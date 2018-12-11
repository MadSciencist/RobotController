#include "sender.h"

void send_feedback(RobotParams_t* params){
  static unsigned long keepAliveprevTicks = 0;
  static unsigned long voltageTemperaturePrevTicks = 0;
  static unsigned long speedCurrentPrevTicks = 0;
  
  unsigned long ticks = HAL_GetTick();
  
  if ((ticks - keepAliveprevTicks) >= params->timing.keepAlivePeriod) {
    keepAliveprevTicks = ticks;
    
    uart_write_dummy( KeepAlive );
  }
  
  if ((ticks - voltageTemperaturePrevTicks) >= params->timing.feedbackVoltageTemperaturePeriod) {
    voltageTemperaturePrevTicks = ticks;
    
    uart_write_two_int16( FeedbackVoltageTemperature, params->state.voltage, params->state.temperature );
  }
  
  if ((ticks - speedCurrentPrevTicks) >= params->timing.feedbackSpeedCurrentPeriod) {
    speedCurrentPrevTicks = ticks;
    
    uart_write_four_int16( FeedbackSpeedCurrent, 
                          (int16_t)params->driveLeft.speed, 
                          (int16_t)params->driveLeft.current, 
                          (int16_t)params->driveRight.speed, 
                          (int16_t)params->driveRight.current );
  }
}

void process_requests(RobotParams_t* params){
  if(params->requests.readEeprom){
    uart_write_int16(SendControlType, (uint8_t)params->controlType);
    uart_write_int16(SendRegenerativeBreaking, (uint8_t)params->useRegenerativeBreaking);
    
    uart_write_float(PidKp_1, params->driveLeft.pid.kp);
    uart_write_float(PidKi_1, params->driveLeft.pid.ki);
    uart_write_float(PidKd_1, params->driveLeft.pid.kd);
    uart_write_float(PidIntegralLimit_1, params->driveLeft.pid.posIntegralLimit);
    uart_write_float(PidDeadband_1, params->driveLeft.deadband);
    uart_write_int16(PidPeriod_1, params->driveLeft.pid.period);
    
    uart_write_float(PidKp_2, params->driveRight.pid.kp);
    uart_write_float(PidKi_2, params->driveRight.pid.ki);
    uart_write_float(PidKd_2, params->driveRight.pid.kd);
    uart_write_float(PidIntegralLimit_2, params->driveRight.pid.posIntegralLimit);
    uart_write_float(PidDeadband_2, params->driveRight.deadband);
    uart_write_int16(PidPeriod_2, params->driveRight.pid.period);
    
    //clear flag as we already processed this request
    params->requests.readEeprom = 0;
  }
}