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
  
  
  // IIR filter
  static float left_aggregate = 0.0f, right_aggregate = 0.0f;
  
  // left motor
  if(params->driveLeft.encoder.isFilterEnabled){
    float leftEncFilterCoeff_1 = params->driveLeft.encoder.encoderFilterCoef;
    float leftEncFilterCoeff_2 = 1.0f - leftEncFilterCoeff_1;
    left = leftEncFilterCoeff_1 * left_aggregate + leftEncFilterCoeff_2 * left;
    left_aggregate = left;
    if (left < 1.0f && left > -1.0f) left = 0.0f; //infinite response problem solution
  }
  
  // right motor
  if(params->driveRight.encoder.isFilterEnabled){
    float rightEncFilterCoeff_1 = params->driveRight.encoder.encoderFilterCoef;
    float rightEncFilterCoeff_2 = 1.0f - rightEncFilterCoeff_1;
    right = rightEncFilterCoeff_1 * right_aggregate + rightEncFilterCoeff_2 * right;
    right_aggregate = right;
    if (right < 2.0f && right > -2.0f) right = 0.0f; //infinite response problem solution
  }
  
  
  if(params->driveLeft.encoder.isEncoderReversed == 1) left *= -1.0f;
  if(params->driveRight.encoder.isEncoderReversed == 1) right *= -1.0f;
  
  params->driveLeft.speed = left;
  params->driveRight.speed = right;
}