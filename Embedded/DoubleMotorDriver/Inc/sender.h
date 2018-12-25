#ifndef _SENDER_H_

#include "stm32f4xx.h"
#include "main.h"
#include "robot_params.h"
#include "msg_gen.h"

void send_feedback(RobotParams_t* params);
void process_requests(RobotParams_t* params);

#define _SENDER_H_
#endif