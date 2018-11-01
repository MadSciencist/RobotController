using System;
using RobotController.Gamepad.Models;

namespace RobotController.Gamepad.Converters
{
    public class OutputMixer
    {
        public RobotControlModel Process(GamepadModel gamepadState)
        {
            bool reverse = false, reverseLR = false;

            short tempLR = gamepadState.LeftThumbstick.X;
            short tempLTrigger = gamepadState.LeftTrigger; //0-255 back
            short tempRTrigger = gamepadState.RightTrigger; //0-255 forward
            short motorLeft = 0, motorRight = 0;
            short deadzone = 20, center = 255;

            short tempFWD = 0, tempBWD = 0, tempLEFT = 0, tempRIGHT = 0;


            //reverse LR
            if (!reverseLR)
            {
                if (tempLR > 0) tempRIGHT = tempLR;
                else tempLEFT = Math.Abs(tempLR);
            }
            else
            {
                if (tempLR > 0) tempLEFT = tempLR;
                else tempRIGHT = Math.Abs(tempLR);
            }

            //reverse Back - fordwards
            if (!reverse)
            {
                tempFWD = tempRTrigger;
                tempBWD = tempLTrigger;
            }
            else
            {
                tempBWD = tempRTrigger;
                tempFWD = tempLTrigger;
            }

            //clamp values
            tempLEFT = Helpres.ConstrainNonnegative(tempLEFT, 255);
            tempRIGHT = Helpres.ConstrainNonnegative(tempRIGHT, 255);
            tempFWD = Helpres.ConstrainNonnegative(tempFWD, 255);
            tempBWD = Helpres.ConstrainNonnegative(tempBWD, 255);

            if (!gamepadState.IsRightPressed)
            {
                if (tempLEFT < deadzone && tempRIGHT < deadzone) //jazda do przodu i tylu
                {
                    motorLeft = (short) (tempFWD -tempBWD + center);
                    motorRight = motorLeft;
                }
                if (tempRIGHT > deadzone)
                {
                    motorLeft = (short) (center + tempFWD - tempBWD);
                    motorRight = (short) (center + tempRIGHT + tempFWD - tempBWD);
                }
                if (tempLEFT > deadzone)
                {
                    motorRight = (short) (center + tempFWD - tempBWD);
                    motorLeft = (short) (center + tempLEFT + tempFWD - tempBWD);
                }
            }
            else //krecenie sie w miejscu
            {
                if (tempRIGHT > deadzone)
                {
                    motorLeft = (short) (center - tempRIGHT);
                    motorRight = (short) (center + tempRIGHT);
                }
                if (tempLEFT > deadzone)
                {
                    motorLeft = (short) (center + tempLEFT);
                    motorRight = (short) (center - tempLEFT);
                }
            }

            //clamp values
            motorLeft = Helpres.ConstrainNonnegative(motorLeft, 510);
            motorRight = Helpres.ConstrainNonnegative(motorRight, 510);

            return new RobotControlModel(motorLeft, motorRight);
        }
    }
}
