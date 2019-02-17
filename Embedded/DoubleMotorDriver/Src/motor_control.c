#include "motor_control.h"

void disable_motors(){
  MOTOR_RIGHT_DIS;
  MOTOR_LEFT_DIS;
}

void enable_motors(){
  MOTOR_LEFT_ENA;
  MOTOR_RIGHT_ENA;
}

void motor_left_start_PWM() {
  HAL_TIM_PWM_Start(&htim2, TIM_CHANNEL_1);
  HAL_TIM_PWM_Start(&htim2, TIM_CHANNEL_2);
}

void motor_right_start_PWM() {
  HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_3);
  HAL_TIM_PWM_Start(&htim4, TIM_CHANNEL_4);
}

void motor_left_stop_PWM() {
  HAL_TIM_PWM_Stop(&htim2, TIM_CHANNEL_1);
  HAL_TIM_PWM_Stop(&htim2, TIM_CHANNEL_2);
}

void motor_right_stop_PWM() {
  HAL_TIM_PWM_Stop(&htim4, TIM_CHANNEL_3);
  HAL_TIM_PWM_Stop(&htim4, TIM_CHANNEL_4);
}

volatile static uint16_t slewrate_left_left = SLEWRATE_CNT, slewrate_left_right = SLEWRATE_CNT;
volatile static uint16_t slewrate_right_left = SLEWRATE_CNT, slewrate_right_right = SLEWRATE_CNT;

void update_slewrate_cnt(){
  if (slewrate_left_left > 0) slewrate_left_left--; //deadtime
  if (slewrate_left_right > 0) slewrate_left_right--;
  if (slewrate_right_left > 0) slewrate_right_left--; //deadtime
  if (slewrate_right_right > 0) slewrate_right_right--;
}

void drive_motor_left(int16_t value){
  if(value > 100 || value < -100) return;
  
 if((value > 0) && (slewrate_left_left == 0)){
    slewrate_left_right = SLEWRATE_CNT;
    
    value = map(value, 0, 100, 5, 100);
    drive_left_motor_right(0);
    drive_left_motor_left(value);
  }else if((value < 0) && (slewrate_left_right == 0)) {
    slewrate_left_left = SLEWRATE_CNT;
    
    value = map(-value, 0, 100, 5, 100);
    drive_left_motor_left(0);
    drive_left_motor_right(value);
  }else if(value == 0){
    drive_left_motor_right(0);
    drive_left_motor_left(0);
  }
}

void drive_motor_right(int16_t value){
  if(value > 100 || value < -100) return;
  
  if(value == 0){
    drive_right_motor_right(0);
    drive_right_motor_left(0);
  }else if((value > 0) && (slewrate_right_left == 0)){
    slewrate_right_right = SLEWRATE_CNT;
    
    value = map(value, 0, 100, 5, 100); //the min value depends highly on the torque load, needs to be tuned on real robot
    drive_right_motor_right(0);
    drive_right_motor_left(value);
  }else if((value < 0) && (slewrate_right_right == 0)) {
    slewrate_right_left = SLEWRATE_CNT;
    
    value = map(-value, 0, 100, 5, 100);
    drive_right_motor_left(0);
    drive_right_motor_right(value);
  }
}

static void drive_left_motor_left(uint16_t value){
  if(value > 100) value = 100;
  TIM4->CCR3 = value;
}

static void drive_left_motor_right(uint16_t value){
  if(value > 100) value = 100;
  TIM4->CCR4 = value;
}

static void drive_right_motor_left(uint16_t value){
  if(value > 100) value = 100;
  TIM2->CCR2 = value;
}

static void drive_right_motor_right(uint16_t value){
  if(value > 100) value = 100;
  TIM2->CCR1 = value;
}

static int16_t map(int16_t x, int16_t in_min, int16_t in_max, int16_t out_min, int16_t out_max)
{
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}