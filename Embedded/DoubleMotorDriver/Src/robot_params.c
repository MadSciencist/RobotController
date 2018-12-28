#include "robot_params.h"


void init_params(RobotParams_t* params){
  if(params->timing.keepAlivePeriod == 0 || params->timing.keepAlivePeriod == 65535)
    params->timing.keepAlivePeriod = 1000;
  
  if(params->timing.feedbackVoltageTemperaturePeriod == 0 || params->timing.feedbackVoltageTemperaturePeriod == 65535)
    params->timing.feedbackVoltageTemperaturePeriod = 250;
  
  if(params->timing.feedbackSpeedCurrentPeriod == 0 || params->timing.feedbackSpeedCurrentPeriod == 65535)
    params->timing.feedbackSpeedCurrentPeriod = 50;
  
  params->requests.readEeprom = 0;
  params->requests.saveEeprom = 0;
  params->requests.allowMovementChanged = 0;
  
  params->driveLeft.setpoint = 0;
  params->driveLeft.speed = 0;
  params->driveRight.setpoint = 0;
  params->driveRight.speed = 0;
  
  params->state.isEnabled = 0;
}

