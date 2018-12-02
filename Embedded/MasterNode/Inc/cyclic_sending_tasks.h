#ifndef _CYCLIC_SENDING_TASKS_H_

#include "stm32f0xx.h"

void UpdateTimer();
void Start();
void Stop();

static void SendVoltageTemperature(uint16_t voltage, uint16_t temperature);
static void SendVelocityCurrent(uint16_t currentLeft, uint16_t currentRight, uint16_t velocityLeft, uint16_t velocityRight);
static void SendKeepAlive();

#define _CYCLIC_SENDING_TASKS_H_
#endif