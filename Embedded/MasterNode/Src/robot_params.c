#include "robot_params.h"

void init_params(RobotParams_t* params){
  params->timing.keepAlivePeriod = 250;
  params->timing.feedbackVoltageTemperaturePeriod = 300;
  params->timing.feedbackSpeedCurrentPeriod = 100;
}

