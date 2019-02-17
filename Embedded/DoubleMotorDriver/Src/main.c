
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  ** This notice applies to any and all portions of this file
  * that are not between comment pairs USER CODE BEGIN and
  * USER CODE END. Other portions of this file, whether 
  * inserted by the user or by software development tools
  * are owned by their respective copyright owners.
  *
  * COPYRIGHT(c) 2019 STMicroelectronics
  *
  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  *   1. Redistributions of source code must retain the above copyright notice,
  *      this list of conditions and the following disclaimer.
  *   2. Redistributions in binary form must reproduce the above copyright notice,
  *      this list of conditions and the following disclaimer in the documentation
  *      and/or other materials provided with the distribution.
  *   3. Neither the name of STMicroelectronics nor the names of its contributors
  *      may be used to endorse or promote products derived from this software
  *      without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
  * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
  * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
  * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
  * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
  * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
  * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */
/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "stm32f4xx_hal.h"
#include "adc.h"
#include "dma.h"
#include "tim.h"
#include "usart.h"
#include "gpio.h"

/* USER CODE BEGIN Includes */
#include "send_queue_manager.h"
#include "msg_gen.h"
#include "msg_rec.h"
#include "requests.h"
#include <stdlib.h>
#include <math.h>
#include <stdio.h> 
#include "PID.h"
#include "fuzzy.h"
#include "robot_params.h"
#include "encoder.h"
#include "motor_control.h"
#include "eeprom_emulator.h"
#include "alarms.h"

#define ADC_MEASUREMENTS 5
/* USER CODE END Includes */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */
/* Private variables ---------------------------------------------------------*/
RobotParams_t robotParams;
uint16_t ADC_RAW[ADC_MEASUREMENTS];
extern uint16_t rec_timeout_ms;
/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);

/* USER CODE BEGIN PFP */
/* Private function prototypes -----------------------------------------------*/
void check_timeout();
void assign_adc();

void pulse_gen();

void execute_closed_loop_control(){
  static float outLeft, outRight;
  
  if(robotParams.controlType == openLoop)
  {
    outLeft = robotParams.driveLeft.setpoint;
    outRight = robotParams.driveRight.setpoint;
  }
  else if(robotParams.controlType == closedLoopPID)
  {
    PID(&robotParams.driveLeft.pid,
        robotParams.driveLeft.setpoint,
        robotParams.driveLeft.speed,
        &outLeft,
        derivativeOnFeedback,
        false);
    
    PID(&robotParams.driveRight.pid,
        robotParams.driveRight.setpoint,
        robotParams.driveRight.speed,
        &outRight,
        derivativeOnFeedback,
        false);
  }
  else if(robotParams.controlType == closedLoopFuzzy)
  {
    fuzzy(&robotParams.driveLeft.fuzzy,
          robotParams.driveLeft.setpoint,
          robotParams.driveLeft.speed,
          &outLeft,
          false);
    
    fuzzy(&robotParams.driveRight.fuzzy,
          robotParams.driveRight.setpoint,
          robotParams.driveRight.speed,
          &outRight,
          false);
  } 
  
  drive_motor_left((int16_t)outLeft);
  drive_motor_right((int16_t)outRight);
}

void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim){
  if(htim->Instance == TIM10){ // each 200us
    static uint16_t cntr = 0;  
    cntr++;
    update_slewrate_cnt();
    
    if(cntr == 25){ //each  5ms
      get_velocity(&robotParams);
      execute_closed_loop_control();
      if (rec_timeout_ms > 0) rec_timeout_ms--; //if this hits 0, then we are disabling motors (var is set in uart rec)
      cntr = 0;
    }
  }
}
/* USER CODE END PFP */

/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  *
  * @retval None
  */
int main(void)
{
  /* USER CODE BEGIN 1 */
  InitSendQueue();
  ReadFromFlash(&robotParams, sizeof(robotParams), SECTOR5_FLASH_BEGINING);
  //fake_params();
  
  init_params(&robotParams);
  /* USER CODE END 1 */

  /* MCU Configuration----------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */
  
  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */
  
  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_DMA_Init();
  MX_ADC1_Init();
  MX_TIM1_Init();
  MX_TIM2_Init();
  MX_TIM3_Init();
  MX_TIM4_Init();
  MX_USART1_UART_Init();
  MX_USART6_UART_Init();
  MX_TIM10_Init();
  /* USER CODE BEGIN 2 */
  HAL_ADC_Start_DMA(&hadc1, (uint32_t*)ADC_RAW, ADC_MEASUREMENTS);
  disable_motors();
  start_receiver();
  HAL_TIM_Encoder_Start(&htim1, TIM_CHANNEL_ALL);
  HAL_TIM_Encoder_Start(&htim3, TIM_CHANNEL_ALL);
  HAL_TIM_Base_Start_IT(&htim10); // 200 us IRQ (motor slew-rate limit)
  motor_left_start_PWM();
  motor_right_start_PWM();
  drive_motor_left(0);
  drive_motor_right(0);  
  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  while (1){
    check_timeout();
    assign_adc();
    
    /*
    pulse_gen();
    static uint16_t size = 0;
    unsigned long ticks = HAL_GetTick();
    if ((ticks - previousTime) > 5) {
    memset(data, 0x00, sizeof(data));
    size = sprintf(data, "%.2f %.2f \n\r", robotParams.driveRight.setpoint, robotParams.driveRight.speed);
    HAL_UART_Transmit_IT(&huart6,data, size);
    previousTime = ticks;
  }*/
    
    send_feedback(&robotParams); //sending feedback 'task'
    check_monitored_params(&robotParams); // threshold checks for alarms
    process_alarms(&robotParams); // process alarms timming
    process_requests(&robotParams, sizeof(robotParams)); 
    
  /* USER CODE END WHILE */

  /* USER CODE BEGIN 3 */
    
  }
  /* USER CODE END 3 */

}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{

  RCC_OscInitTypeDef RCC_OscInitStruct;
  RCC_ClkInitTypeDef RCC_ClkInitStruct;

    /**Configure the main internal regulator output voltage 
    */
  __HAL_RCC_PWR_CLK_ENABLE();

  __HAL_PWR_VOLTAGESCALING_CONFIG(PWR_REGULATOR_VOLTAGE_SCALE2);

    /**Initializes the CPU, AHB and APB busses clocks 
    */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_ON;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLM = 8;
  RCC_OscInitStruct.PLL.PLLN = 84;
  RCC_OscInitStruct.PLL.PLLP = RCC_PLLP_DIV2;
  RCC_OscInitStruct.PLL.PLLQ = 4;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

    /**Initializes the CPU, AHB and APB busses clocks 
    */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_2) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

    /**Configure the Systick interrupt time 
    */
  HAL_SYSTICK_Config(HAL_RCC_GetHCLKFreq()/1000);

    /**Configure the Systick 
    */
  HAL_SYSTICK_CLKSourceConfig(SYSTICK_CLKSOURCE_HCLK);

  /* SysTick_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(SysTick_IRQn, 0, 0);
}

/* USER CODE BEGIN 4 */
void check_timeout(){
  if(rec_timeout_ms == 0){
    robotParams.state.isEnabled = 0;
    robotParams.requests.allowMovementChanged = 1;
  }
}

void assign_adc(){
  robotParams.state.voltage = ADC_RAW[0];
  robotParams.state.temperature = ADC_RAW[1];
  robotParams.driveLeft.current = ADC_RAW[3];
  robotParams.driveRight.current = ADC_RAW[2];
  //static uint16_t internal_cpu_temp;
  //internal_cpu_temp = ADC_RAW[4];
}

void pulse_gen(){
  static unsigned long previousTimex = 0;
  static bool isHigh = false;
  
  unsigned long ticks = HAL_GetTick();
  
  if ((ticks - previousTimex) > 200) {
    if(isHigh) robotParams.driveRight.setpoint = 10.0;
    else robotParams.driveRight.setpoint = 0.0;
    isHigh = !isHigh;
    previousTimex = ticks;
  }
}

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @param  file: The file name as string.
  * @param  line: The line in file as a number.
  * @retval None
  */
void _Error_Handler(char *file, int line)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  while(1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t* file, uint32_t line)
{ 
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
  tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/**
  * @}
  */

/**
  * @}
  */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
