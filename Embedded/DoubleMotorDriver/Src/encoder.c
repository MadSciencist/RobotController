#include "encoder.h"

void get_velocity(float* left, float* right){
  static volatile int16_t oldEncoderCountLeft = 0, oldEncoderCountRight = 0;
  volatile int16_t encoderCountLeft = oldEncoderCountLeft;
  volatile int16_t encoderCountRight = oldEncoderCountRight;
  
    encoderCountLeft = TIM1->CNT;
    TIM1->CNT = 0;
    encoderCountRight = TIM3->CNT;
    TIM3->CNT = 0;
    *left = (float)(encoderCountLeft - oldEncoderCountLeft) / 1.8f;
    *right = (float)(encoderCountRight - oldEncoderCountRight) / 1.8f;
}