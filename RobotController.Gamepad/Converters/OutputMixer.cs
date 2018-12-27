using RobotController.Gamepad.Interfaces;
using RobotController.Gamepad.Models;
using System;
using RobotController.RobotModels;

namespace RobotController.Gamepad.Converters
{
    public class OutputMixer
    {
        private readonly ISteeringConfig _config;

        public OutputMixer(ISteeringConfig config)
        {
            _config = config;
        }

        public ControlsModel Process(GamepadModel gamepadState)
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

                //process expo curve (inifinite impulse response filter)
                ProcessLowPassFilter(ref tempLeft, ref tempRight, ref tempFwd, ref tempBwd);

                //constrain inputs
                tempLeft = Helpers.ConstrainNonnegative(tempLeft, 255);
                tempRight = Helpers.ConstrainNonnegative(tempRight, 255);
                tempFwd = Helpers.ConstrainNonnegative(tempFwd, 255);
                tempBwd = Helpers.ConstrainNonnegative(tempBwd, 255);

                (motorLeft, motorRight) = ProcessMixing(gamepadState, tempFwd, tempBwd, tempLeft, tempRight);

                //clamp values
                motorLeft = Helpers.ConstrainNonnegative(motorLeft, 510);
                motorRight = Helpers.ConstrainNonnegative(motorRight, 510);

                //to -255 - 255 range
                motorLeft -= 255;
                motorRight -= 255;

                //to -100 - 100 range
                motorLeft = (short) ((float) motorLeft / 2.55f);
                motorRight = (short)((float) motorRight / 2.55f);
            }

            return new ControlsModel(motorLeft, motorRight);
        }

        private void ProcessLowPassFilter(ref short tempLeft, ref short tempRight, ref short tempFwd, ref short tempBwd)
        {
            if (!_config.UseLowPassFilter) return;

            double filteredFwd = tempFwd;
            double filteredBwd = tempBwd;
            double filteredLeft = tempLeft;
            double filteredRight = tempRight;

            //we are accepting coefficient in 0-99 range (int, from slider) and scaling it into double
            var coefficient = _config.LowPassCoefficient / 100.0;
            var coefficientFulfillant = (1 - coefficient + 0.001);

            filteredFwd = coefficient * _lastFwd + coefficientFulfillant * filteredFwd; //0.001 is solution to casting loose of resolution
            _lastFwd = filteredFwd;
            if (filteredFwd < 5.0) filteredFwd = 0; //fast cutoff

            filteredBwd = coefficient * _lastBwd + coefficientFulfillant * filteredBwd;
            _lastBwd = filteredBwd;
            if (filteredBwd < 5.0) filteredBwd = 0;

            filteredLeft = coefficient * _lastLeft + coefficientFulfillant * filteredLeft;
            _lastLeft = filteredLeft;
            if (filteredLeft < 8.0) filteredLeft = 0;

            filteredRight = coefficient * _lastRight + coefficientFulfillant * filteredRight;
            _lastRight = filteredRight;
            if (filteredRight < 8.0) filteredRight = 0;

            tempFwd = (short)filteredFwd;
            tempBwd = (short)filteredBwd;
            tempLeft = (short)filteredLeft;
            tempRight = (short)filteredRight;
        }

        private static double _lastFwd, _lastBwd, _lastLeft, _lastRight;

        private void ProcessExponentialLookup(ref short tempLeft, ref short tempRight, ref short tempFwd, ref short tempBwd)
        {
            if (!_config.UseExponentialCurve) return;

            tempFwd = ExponentialCurve.PerformLookup(tempFwd);
            tempBwd = ExponentialCurve.PerformLookup(tempBwd);
            tempLeft = ExponentialCurve.PerformLookup(tempLeft);
            tempRight = ExponentialCurve.PerformLookup(tempRight);
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
            else //rotate around point
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
