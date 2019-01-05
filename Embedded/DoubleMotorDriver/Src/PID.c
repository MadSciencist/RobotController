/****************************************
*                                       *
*       Title:  PID LIBRARY             *
*       Author: Mateusz Kryszczak       *
*       Date: 19.01.2017                *
*       MCU: STM32F401                  *
*                                       *
*****************************************/

#include "PID.h"

uint8_t PID(PID_Properties_t* props, float setpoint, float feedback, float* pOutput, derivative_t derivativeType, bool reverse){
  
  if(props == NULL || pOutput == NULL) return 1;
  
  float error = setpoint - feedback;
  float derivativeOutput = 0.0f;
  float output;
  
  
  // if (fabs(error) < props->deadband) {
  if (fabs(setpoint) > props->deadband) {
    //proportional part
    float proportionalOutput = props->kp * error;
    
    //integral part
    props->integralSum += props->ki * error;
    //anti wind-up
    if (props->integralSum > props->posIntegralLimit) props->integralSum = props->posIntegralLimit;
    else if (props->integralSum < props->negIntegralLimit) props->integralSum = props->negIntegralLimit;
    
    //derivative part
    switch(derivativeType){
    case derivativeOnFeedback:
      derivativeOutput = props->kd * (-1.0f) * (feedback - props->lastFeedback);
      break;
      
    case derivativeOnError:
      derivativeOutput = props->kd * (error - props->lastError);
      break;
      
    default:
      break;
    }
    
    output = proportionalOutput + props->integralSum + derivativeOutput;
  } else {
    output = 0;
  }
  
  
  if(reverse == true) output *= -1.0f;
  
  //check if output is within bounds
  if(output > props->posOutputLimit) output = props->posOutputLimit;
  else if(output < props->negOutputLimit) output = props->negOutputLimit;
  
  *pOutput = output;
  
  props->lastFeedback = feedback;
  props->lastError = error;
  
  return 0;
}

uint8_t GetKi(PID_Properties_t* props, float* ki){
  if(props == NULL) return 0;
  
  *ki = props->ki / ((float)props->period / 1000.0f);
  
  return 1;
}

uint8_t GetKd(PID_Properties_t* props, float* kd){
  if(props == NULL) return 0;
  
  *kd = props->kd * ((float)props->period / 1000.0f);
  
  return 1;
}

uint8_t ResetIntegrator(PID_Properties_t* props){
  if(props == NULL) return 0;
  
  props->integralSum = 0;
  return 1;
}

uint8_t PidSetParams(PID_Properties_t* PID_Properties, float _kp, float _ki, float _kd){
  if(_kp < 0 || _ki < 0 || _kd < 0 || PID_Properties == NULL) return 0;
  
  //check if PID_Properties are different
  if( ! Compare(PID_Properties->kp, _kp, 1E-5)){ 
    PID_Properties->kp = _kp;
  }
  
  if( ! Compare(PID_Properties->ki, _ki, 1E-5)){ 
    PID_Properties->ki = _ki * ((float)PID_Properties->period / 1000.0f);
  }
  
  if( ! Compare(PID_Properties->kd, _kd, 1E-5)){ 
    if(PID_Properties->period > 0)
      PID_Properties->kd = _kd / ((float)PID_Properties->period / 1000.0f);
    //PID_Properties->kd = _kd;
  }
  return 1;
}

static uint8_t Compare(float a, float b, float epsilon){
  return fabs(a - b) < epsilon;
}