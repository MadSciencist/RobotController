#include "driver_model.h"

driver_t *drv1 = NULL, *drv2 = NULL;

void init_drivers(){
  drv1 = init_driver();
  drv2 = init_driver();
}

void free_drivers(){
  free_driver(drv1);
  free_driver(drv2);
}

static driver_t *init_driver(){
  driver_t *drv = (driver_t*)malloc(sizeof(driver_t));
  drv->kp = 0.0;
  drv->ki = 0.0;
  drv->kd = 0.0;
  drv->i_lim = 0.0;
  
  return drv;
}

static void free_driver(driver_t *driver){
  free(driver);
}