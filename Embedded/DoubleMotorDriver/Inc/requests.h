#ifndef _REQUESTS_H_

#include "stm32f4xx.h"
#include "main.h"
#include "robot_params.h"
#include "eeprom_emulator.h"
#include "msg_gen.h"
#include "motor_control.h"
#include "PID.h"

void send_feedback(RobotParams_t* params);
void process_requests(RobotParams_t* params, uint16_t params_len);

#define _REQUESTS_H_
#endif