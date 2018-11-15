#include "driver_model.h"



driver_t *init_driver(){
  driver_t *drv = (driver_t*)malloc(sizeof(driver_t));
  drv->kp = 0.0;
  drv->ki = 0.0;
  drv->kd = 0.0;
  drv->i_lim = 0.0;
  
  return drv;
}

void free_driver(driver_t *driver){
  free(driver);
}