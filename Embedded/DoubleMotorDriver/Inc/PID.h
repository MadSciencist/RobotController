  /****************************************
  *                                       *
  *       Title:  PID LIBRARY             *
  *       Author: Mateusz Kryszczak       *
  *       Date: 19.01.2017                *
  *       MCU: STM32F401                  *
  *                                       *
  *****************************************/

#ifndef _PID_H_
    
#include "stm32f4xx.h"
#include <stdbool.h>
#include "math.h"

typedef struct{
  uint16_t period;              //interval between PID computations
  float deadband;
  float error;
  float integralSum;
  float kp;                     //do not change Kp Ki Kd manually, use PidSetParams(); 
  float ki;
  float kd;
  float posIntegralLimit;       //integral anti-windup
  float negIntegralLimit;       //integral anti-windup
  float lastError;      
  float lastFeedback;
  float posOutputLimit;         //bound the output
  float negOutputLimit;
} PID_Properties_t;

//derivative type calculation
typedef enum{
  noDerivative,
  derivativeOnError,
  derivativeOnFeedback,
} derivative_t;


//compute PID
//return 1 if null pointers
//return 0 if ok
uint8_t PID(PID_Properties_t* PID_Properties, float setpoint, float feedback, float* pOutput, derivative_t derivativeType, bool pidReverse);

uint8_t ResetIntegrator(PID_Properties_t* props);

#define _PID_H_
#endif