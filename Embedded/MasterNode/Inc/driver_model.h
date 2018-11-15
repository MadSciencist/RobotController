#ifndef _DRIVER_MODEL_H

#include <stdlib.h>
#include <stdio.h> 

typedef struct DriverModel {
  double kp;
  double ki;
  double kd;
  double i_lim;
} driver_t;


driver_t* init_driver();
void free_driver(driver_t* driver);

#define _DRIVER_MODEL_H
#endif