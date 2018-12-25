#include "robot_params.h"

void init_params(RobotParams_t* params){
  params->timing.keepAlivePeriod = 5000;
  params->timing.feedbackVoltageTemperaturePeriod = 3000;
  params->timing.feedbackSpeedCurrentPeriod = 100;
}

