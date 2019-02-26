#include "fuzzy.h"


uint8_t fuzzy(Fuzzy_Properties_t* props, float setpoint, float feedback, float* pOutput, bool reverse){
  if(props == NULL || pOutput == NULL) return 1;
  
  // we are doing the calculations in the PU (-1 : 1)
  const float scale_factor = 100.0f;
  
  float error = (setpoint/scale_factor) - (feedback/scale_factor);
  float proportional = saturation( (error * props->kp), -1.0f, 1.0f);
  double kd = props->kd / ((float)props->period / 1000.0f); // we need to take period into consideration (divide by 1000 to get seconds)
  double derivative = saturation( (kd * (error - props->lastError)), -1.0f, 1.0f);
  
  if (fabs(setpoint) > props->deadband) {
    // fuzzification
    float pnb = nb(proportional);
    float pze = ze(proportional);
    float ppb = pb(proportional);
    float dnb = nb(derivative);
    float dze = ze(derivative);
    float dpb = pb(derivative);
    
    // rules
    float pnb_dpb = pnb * dpb;
    float pnb_dnb = pnb * dnb;
    float pze_dnb = pze * dnb;
    float ppb_dnb = ppb * dnb;
    float pnb_dze = pnb * dze;
    float pze_dze = pze * dze;
    float ppb_dze = ppb * dze;
    float pze_dpb = pze * dpb;
    float ppb_dpb = ppb * dpb;
    
    // defuzzification
    // calcullation of sum(Singleton*fuzzified)/sum(fuzzified);
    const float singletons[] = { 0.0f, -1.0f, -1.0f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f, 1.0f };
    float sum_weighted = pnb_dpb * singletons[0] + pnb_dnb * singletons[1] + pze_dnb * singletons[2] + 
      ppb_dnb * singletons[3] + pnb_dze * singletons[4] + pze_dze * singletons[5] + 
        ppb_dze * singletons[6] + pze_dpb * singletons[7] + ppb_dpb * singletons[8];
    
    float sum_fuzzified = pnb_dpb + pnb_dnb + pze_dnb + ppb_dnb + pnb_dze + pze_dze + ppb_dze + pze_dpb + ppb_dpb;
    float fuzzy_output = sum_weighted / sum_fuzzified;
    
    float ki = props->ki * ((float)props->period / 1000.0f); // we need to take period into consideration (divide by 1000 to get seconds)
    props->integralSum += ki * fuzzy_output;
    
    // limit integral
    if (props->integralSum > props->posIntegralLimit) props->integralSum = props->posIntegralLimit;
    else if (props->integralSum < props->negIntegralLimit) props->integralSum = props->negIntegralLimit;
    
    //the output is actually value of the integral
    if(reverse == false) *pOutput = props->integralSum * scale_factor;
    else *pOutput = -1.0f * props->integralSum * scale_factor;
  } else { //we are withing dead band range
    props->integralSum = 0.0f;
    *pOutput = 0.0f;
  }
  
  props->lastFeedback = feedback;
  props->lastError = error;
  
  return 0;
}

static float nb(float in){
  return tribas(in + 1.0f);
}

static float ze(float in){
  return tribas(in);
}

static float pb(float in){
  return tribas(in - 1.0f);
}

/* matlab TRIBAS function
a = tribas(n) = 1 - abs(n), if -1 <= n <= 1
= 0, otherwise
*/
static float tribas(float n){
  if(n >= -1 && n <= 1){
    return 1 - fabs(n);
  }else return 0;
}

/* matlab saturation block */
static float saturation(float in, float neg_lim, float pos_lim){
  if(in > pos_lim) return pos_lim;
  else if(in < neg_lim) return neg_lim;
  else return in;
}