#ifndef _MOTOR_CONTROL_H_

#include "stm32f4xx.h"
#include "gpio.h"
#include "tim.h"

#define SLEWRATE_CNT 25 // if the variable will be incremented each 0.2ms, this gives us 50ms deadband (10)
#define MOTOR_LEFT_DIS           HAL_GPIO_WritePin(GPIOC, DIS1_Pin, GPIO_PIN_SET);
#define MOTOR_LEFT_ENA           HAL_GPIO_WritePin(GPIOC, DIS1_Pin, GPIO_PIN_RESET);
#define MOTOR_RIGHT_DIS          HAL_GPIO_WritePin(DIS2_GPIO_Port, DIS2_Pin, GPIO_PIN_SET);
#define MOTOR_RIGHT_ENA          HAL_GPIO_WritePin(DIS2_GPIO_Port, DIS2_Pin, GPIO_PIN_RESET)

void disable_motors();
void enable_motors();

// start/stop PWM generation
void motor_left_start_PWM();
void motor_left_stop_PWM();

void motor_right_start_PWM();
void motor_right_stop_PWM();

// this function should be called each 0.2ms (slewrate limiter)
void update_slewrate_cnt();

void drive_motor_left(int16_t value);
void drive_motor_right(int16_t value);

// private functions, accept 0 - 100 pwm range
static void drive_left_motor_left(uint16_t value);
static void drive_left_motor_right(uint16_t value);
static void drive_right_motor_left(uint16_t value);
static void drive_right_motor_right(uint16_t value);

// helper
static int16_t map(int16_t x, int16_t in_min, int16_t in_max, int16_t out_min, int16_t out_max);

#define _MOTOR_CONTROL_H_
#endif