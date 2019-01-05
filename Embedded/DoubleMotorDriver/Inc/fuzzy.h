#ifndef _FUZZY_H_
    
#include "stm32f4xx.h"
#include <stdbool.h>
#include "math.h"

typedef struct{
  uint16_t period;
  float deadband;
  float error;
  float integralSum;
  float kp;
  float ki;
  float kd;
  float posIntegralLimit;       //integral anti-windup
  float negIntegralLimit;       //integral anti-windup
  float lastError;      
  float lastFeedback;
  float posOutputLimit;         //bound the output
  float negOutputLimit;
} Fuzzy_Properties_t;

#define _FUZZY_H_
#endif

uint8_t fuzzy(Fuzzy_Properties_t* props, float setpoint, float feedback, float* pOutput, bool reverse);

static float nb(float in);
static float ze(float in);
static float pb(float in);

static float saturation(float in, float neg_lim, float pos_lim);
static float tribas(float n);