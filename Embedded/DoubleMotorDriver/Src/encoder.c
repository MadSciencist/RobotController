#include "encoder.h"

void get_velocity(RobotParams_t* params){
  static int16_t oldEncoderCountLeft = 0, oldEncoderCountRight = 0;
  int16_t encoderCountLeft = oldEncoderCountLeft;
  int16_t encoderCountRight = oldEncoderCountRight;
  
    encoderCountLeft = TIM1->CNT;
    TIM1->CNT = 0;
    encoderCountRight = TIM3->CNT;
    TIM3->CNT = 0;
    
    float left = (float)(encoderCountLeft - oldEncoderCountLeft) / params->driveLeft.encoder.scaleCoef;
    float right = (float)(encoderCountRight - oldEncoderCountRight) / params->driveRight.encoder.scaleCoef;
    
    if(params->driveLeft.encoder.isEncoderReversed == 1) left *= -1.0f;
    if(params->driveRight.encoder.isEncoderReversed == 1) right *= -1.0f;

    params->driveLeft.speed = left;
    params->driveRight.speed = right;
}