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

static uint8_t slewrate_left_left = SLEWRATE_CNT, slewrate_left_right = SLEWRATE_CNT;
static uint8_t slewrate_right_left = SLEWRATE_CNT, slewrate_right_right = SLEWRATE_CNT;

void update_slewrate_cnt(){
  if (slewrate_left_left > 0) slewrate_left_left--; //deadtime
  if (slewrate_left_right > 0) slewrate_left_right--;
  if (slewrate_right_left > 0) slewrate_right_left--; //deadtime
  if (slewrate_right_right > 0) slewrate_right_right--;
}

void drive_motor_left(int16_t value){
  if(value > 100 || value < -100) return;
  
  if(value == 0){
    drive_left_motor_right(0);
    drive_left_motor_left(0);
  }else if((value > 0) && (slewrate_left_left == 0)){
    slewrate_left_right = SLEWRATE_CNT;
    drive_left_motor_right(0);
    drive_left_motor_left(value);
  }else if((value < 0) && (slewrate_left_right == 0)) {
    slewrate_left_left = SLEWRATE_CNT;
    drive_left_motor_left(0);
    drive_left_motor_right(-value);
  }
}

void drive_motor_right(int16_t value){
  if(value > 100 || value < -100) return;
  
  if(value == 0){
    drive_right_motor_right(0);
    drive_right_motor_left(0);
  }else if((value > 0) && (slewrate_right_left == 0)){
    slewrate_right_right = SLEWRATE_CNT;
    drive_right_motor_right(0);
    drive_right_motor_left(value);
  }else if((value < 0) && (slewrate_right_right == 0)) {
    slewrate_right_left = SLEWRATE_CNT;
    drive_right_motor_left(0);
    drive_right_motor_right(-value);
  }
}

static void drive_left_motor_left(uint16_t value){
  if(value > 100) value = 100;
  TIM2->CCR1 = value;
}

static void drive_left_motor_right(uint16_t value){
  if(value > 100) value = 100;
  TIM2->CCR2 = value;
}

static void drive_right_motor_left(uint16_t value){
  if(value > 100) value = 100;
  TIM4->CCR3 = value;
}

static void drive_right_motor_right(uint16_t value){
  if(value > 100) value = 100;
  TIM4->CCR4 = value;
}