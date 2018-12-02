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