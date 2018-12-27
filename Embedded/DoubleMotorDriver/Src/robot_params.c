#include "robot_params.h"


void init_params(RobotParams_t* params){
  params->timing.keepAlivePeriod = 1000;
  params->timing.feedbackVoltageTemperaturePeriod = 100;
  params->timing.feedbackSpeedCurrentPeriod = 50;
}

