using RobotController.Gamepad.Interfaces;
using RobotController.Gamepad.Models;
using System;

namespace RobotController.Gamepad.Converters
{
    public class OutputMixer
    {
        private readonly ISteeringConfig _config;

        public OutputMixer(ISteeringConfig config)
        {
            _config = config;
        }

        public RobotControlModel Process(GamepadModel gamepadState)
        {
            var lr = gamepadState.LeftThumbstick.X; //-255-255 left-right
            var leftTrigger = gamepadState.LeftTrigger; //0-255 back
            var rightTrigger = gamepadState.RightTrigger; //0-255 forward
            short motorLeft = _config.Centervalue, motorRight = _config.Centervalue;
            short tempLeft = 0, tempRight = 0;

            checked
            {
                //process reverses
                ProcessLeftRightReverse(lr, ref tempLeft, ref tempRight);
                ProcessBackForwardReverse(leftTrigger, rightTrigger, out var tempFwd, out var tempBwd);

                //process expo curve
                ProcessExponentialLookup(ref tempLeft, ref tempRight, ref tempFwd, ref tempBwd);

                //constrain inputs
                tempLeft = Helpres.ConstrainNonnegative(tempLeft, 255);
                tempRight = Helpres.ConstrainNonnegative(tempRight, 255);
                tempFwd = Helpres.ConstrainNonnegative(tempFwd, 255);
                tempBwd = Helpres.ConstrainNonnegative(tempBwd, 255);

                (motorLeft, motorRight) = ProcessMixing(gamepadState, tempFwd, tempBwd, tempLeft, tempRight);

                //clamp values
                motorLeft = Helpres.ConstrainNonnegative(motorLeft, 510);
                motorRight = Helpres.ConstrainNonnegative(motorRight, 510);
            }

            return new RobotControlModel(motorLeft, motorRight);
        }

        private void ProcessExponentialLookup(ref short tempLeft, ref short tempRight, ref short tempFwd, ref short tempBwd)
        {
            checked
            {
                if (!_config.UseExponentialCurve) return;
                tempFwd = ExponentialCurve.PerformLookup(tempFwd, _config.ExponentialCurveCoefficient);
                tempBwd = ExponentialCurve.PerformLookup(tempBwd, _config.ExponentialCurveCoefficient);
                tempLeft = ExponentialCurve.PerformLookup(tempLeft, _config.ExponentialCurveCoefficient);
                tempRight = ExponentialCurve.PerformLookup(tempRight, _config.ExponentialCurveCoefficient);
            }
        }

        private (short, short) ProcessMixing(GamepadModel gamepadState, short tempFwd, short tempBwd, short tempLeft, short tempRight)
        {
            short mLeft = _config.Centervalue, mRight = _config.Centervalue;

            /* We are not holding the right button:
             back and forwards
             left trigger -> move backwards both motors
             right trigger -> move forwards both motors
             left thumbstick left -> move by arc to the left
             left thumbstich right -> move tby arc to the right
             */
            if (!gamepadState.IsRightPressed)
            {
                if (tempLeft < _config.Deadband && tempRight < _config.Deadband)
                {
                    mLeft = (short)(tempFwd - tempBwd + _config.Centervalue);
                    mRight = mLeft;
                }
                if (tempRight > _config.Deadband)
                {
                    mLeft = (short)(_config.Centervalue + tempFwd - tempBwd);
                    mRight = (short)(_config.Centervalue + tempRight + tempFwd - tempBwd);
                }

                if (tempLeft <= _config.Deadband) return (mLeft, mRight);
                mRight = (short)(_config.Centervalue + tempFwd - tempBwd);
                mLeft = (short)(_config.Centervalue + tempLeft + tempFwd - tempBwd);
            }
            /* while holding right button:
             left thumbstick left -> rotate left
             left thumbstik right -> rotate right
             */
            else //krecenie sie w miejscu
            {
                if (tempRight > _config.Deadband)
                {
                    mLeft = (short)(_config.Centervalue - tempRight);
                    mRight = (short)(_config.Centervalue + tempRight);
                }

                if (tempLeft <= _config.Deadband) return (mLeft, mRight);
                mLeft = (short)(_config.Centervalue + tempLeft);
                mRight = (short)(_config.Centervalue - tempLeft);
            }

            return (mLeft, mRight);
        }

        private void ProcessBackForwardReverse(short lTrigger, short rTrigger, out short tempFwd, out short tempBwd)
        {
            if (!_config.IsReversed)
            {
                tempFwd = rTrigger;
                tempBwd = lTrigger;
            }
            else
            {
                tempBwd = rTrigger;
                tempFwd = lTrigger;
            }
        }

        private void ProcessLeftRightReverse(short lr, ref short tempLeft, ref short tempRight)
        {
            if (!_config.IsLeftRightReverse)
            {
                if (lr > 0) tempRight = lr;
                else tempLeft = Math.Abs(lr);
            }
            else
            {
                if (lr > 0) tempLeft = lr;
                else tempRight = Math.Abs(lr);
            }
        }
    }
}
