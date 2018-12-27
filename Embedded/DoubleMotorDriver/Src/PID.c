/****************************************
*                                       *
*       Title:  PID LIBRARY             *
*       Author: Mateusz Kryszczak       *
*       Date: 19.01.2017                *
*       MCU: STM32F401                  *
*                                       *
*****************************************/

#include "PID.h"

uint8_t PID(PID_Properties_t* PID_Properties, float setpoint, float feedback, float* pOutput, derivative_t derivativeType, PID_Reverse_t pidReverse){
  
  if(PID_Properties == NULL || pOutput == NULL) return 0;
  
  float error = setpoint - feedback;
  float derivativeOutput = 0.0f;
  float output;
  
  //proportional part
  float proportionalOutput = PID_Properties->kp * error;
  
  //integral part
  PID_Properties->integralSum += (PID_Properties->ki * error);
  //anti wind-up
  if (PID_Properties->integralSum > PID_Properties->posIntegralLimit) PID_Properties->integralSum = PID_Properties->posIntegralLimit;
  else if (PID_Properties->integralSum < PID_Properties->negIntegralLimit) PID_Properties->integralSum = PID_Properties->negIntegralLimit;
  
  //derivative part
  switch(derivativeType){
  case derivativeOnFeedback:
    derivativeOutput = PID_Properties->kd * (-1.0f) * (feedback - PID_Properties->lastFeedback);
    break;
    
  case derivativeOnError:
    derivativeOutput = PID_Properties->kd * (error - PID_Properties->lastError);
    break;
    
  default:
    break;
  }
  
  output = proportionalOutput + PID_Properties->integralSum + derivativeOutput;
  if(pidReverse) output *= -1.0f;
  
  //check if output is within bounds
  if(output > PID_Properties->posOutputLimit) output = PID_Properties->posOutputLimit;
  else if(output < PID_Properties->negOutputLimit) output = PID_Properties->negOutputLimit;
  
  *pOutput = output;
  
  PID_Properties->lastFeedback = feedback;
  PID_Properties->lastError = error;
  
  return 1;
}

uint8_t PidSetParams(PID_Properties_t* PID_Properties, float _kp, float _ki, float _kd){
  if(_kp < 0 || _ki < 0 || _kd < 0 || PID_Properties == NULL) return 0;
  
  //check if PID_Properties are different
  if( ! Compare(PID_Properties->kp, _kp, 1E-4)){ 
    PID_Properties->kp = _kp;
  }
  
  if( ! Compare(PID_Properties->ki, _ki, 1E-4)){ 
    PID_Properties->ki = _ki * ((float)PID_Properties->period / 1000.0f);
  }
  
  if( ! Compare(PID_Properties->kd, _kd, 1E-4)){ 
    if(PID_Properties->period > 0)
      // PID_Properties->kd = _kd / ((float)PID_Properties->period / 1000.0f);
      PID_Properties->kd = _kd;
  }
  return 1;
}

static uint8_t Compare(float a, float b, float epsilon){
  return fabs(a - b) < epsilon;
}