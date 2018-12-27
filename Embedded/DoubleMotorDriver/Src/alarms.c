#include "alarms.h"

static bool alarm_slow = false, alarm_fast = false;

void check_monitored_params(RobotParams_t* params) {
  if (params->state.voltage < params->alarms.voltage ||
      params->state.temperature > params->alarms.temperature) alarm_slow = true;
  else alarm_slow = false;
  if (params->state.voltage < params->alarms.criticalVoltage ||
      params->state.temperature > params->alarms.criticalTemperature) alarm_fast = true;
  else alarm_fast = false;
}

void process_alarms(RobotParams_t* params) {
  static uint16_t alarm_cntr = 0;
  static unsigned long previousTime = 0;
  unsigned long ticks = HAL_GetTick();
  
  if ((ticks - previousTime) >= 1) {
    if (alarm_cntr > 0) alarm_cntr--;
    previousTime = ticks;
  }
  
  if (alarm_fast && alarm_slow) { // prioritze alarms - if both present, use fast one
    alarm_slow = false;
  }
  
  if (alarm_slow) { // slow alarms - every 2 sec, for 100ms
    if (!alarm_cntr) { // turn off
      HAL_GPIO_WritePin(GPIOC, LED_Pin, GPIO_PIN_RESET);
      if(params->alarms.useBuzzer) HAL_GPIO_WritePin(GPIOC, BUZZ_Pin, GPIO_PIN_RESET);
      alarm_cntr = 2000;
    }
    if (!(alarm_cntr - 100)) { // turn on for 100 ms
      HAL_GPIO_WritePin(GPIOC, LED_Pin, GPIO_PIN_SET);
      if(params->alarms.useBuzzer) HAL_GPIO_WritePin(GPIOC, BUZZ_Pin, GPIO_PIN_SET);
    }
  }
  
  if (alarm_fast) { // fast alarm - every 150 ms, for 60ms
    if (!alarm_cntr) { //fast alarm - turn off
      HAL_GPIO_WritePin(GPIOC, LED_Pin, GPIO_PIN_RESET);
      if(params->alarms.useBuzzer) HAL_GPIO_WritePin(GPIOC, BUZZ_Pin, GPIO_PIN_RESET);
      alarm_cntr = 150;
    }
    if (!(alarm_cntr - 60)) { //turn on for 60ms
      HAL_GPIO_WritePin(GPIOC, LED_Pin, GPIO_PIN_SET);
      if(params->alarms.useBuzzer) HAL_GPIO_WritePin(GPIOC, BUZZ_Pin, GPIO_PIN_SET);
    }
  }
  
  if (!alarm_fast && !alarm_slow) { // if there is no active alarms, shut down both
    HAL_GPIO_WritePin(GPIOC, LED_Pin, GPIO_PIN_RESET);
    if(params->alarms.useBuzzer) HAL_GPIO_WritePin(GPIOC, BUZZ_Pin, GPIO_PIN_RESET);
  }
}