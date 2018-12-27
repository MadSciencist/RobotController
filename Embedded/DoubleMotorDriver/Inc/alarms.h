#ifndef _ALARMS_H_

#include "robot_params.h"
#include "gpio.h"
#include "stdbool.h"

void check_monitored_params(RobotParams_t* params);
void process_alarms(RobotParams_t* params);

#define _ALARMS_H_
#endif