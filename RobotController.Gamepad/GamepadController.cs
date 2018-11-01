﻿using J2i.Net.XInputWrapper;
using RobotController.Gamepad.Converters;
using RobotController.Gamepad.Models;
using System;
using System.Timers;
using RobotController.Gamepad.Config;
using RobotController.Gamepad.EventArguments;
using RobotController.Gamepad.Interfaces;

namespace RobotController.Gamepad
{
    public class GamepadController : IDisposable, IGamepadController
    {
        public event EventHandler<GamepadEventArgs> GamepadStateChanged;
        public event EventHandler<GamepadEventArgs> RobotControlChanged; 
        public event EventHandler<GamepadErrorEventArgs> GamepadErrorOccured;

        public ISteeringConfig SteeringConfig { get; set; }

        private readonly GamepadModel _gamepadModel;
        private readonly RangeConverter _rangeConverter;
        private readonly Timer _lowPassFilterTimer;

        public GamepadController(int controllerIndex, int updateFrequency)
        {
            if (updateFrequency <= 0) throw new ArgumentException("Update frequency should be positive");

            SteeringConfig = new SteeringConfig();
            //divide by 128 to get -255 <=> 255 range on thumbstick
            _rangeConverter = new RangeConverter(128f, 255);
            _gamepadModel = new GamepadModel();

            var controller = XboxController.RetrieveController(controllerIndex);
            controller.StateChanged += StateChanged;
            XboxController.UpdateFrequency = updateFrequency;
               
            _lowPassFilterTimer = new Timer(5);
            _lowPassFilterTimer.Elapsed += LowPassFilterTimerOnElapsed;
        }

        public void Start()
        {
            XboxController.StartPolling();
           // _lowPassFilterTimer.Start();
        }

        public void Stop()
        {
            XboxController.StartPolling();
            _lowPassFilterTimer.Stop();
        }

        private void LowPassFilterTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("ELAPSED");
        }


        public void StateChanged(object sender, XboxControllerStateChangedEventArgs e)
        {
            #region INTPUT_MAPPINGS
            //analogs
            _gamepadModel.LeftThumbstick.X = _rangeConverter.ScaleThumbstick(e.CurrentInputState.Gamepad.sThumbLX);
            _gamepadModel.LeftThumbstick.Y = _rangeConverter.ScaleThumbstick(e.CurrentInputState.Gamepad.sThumbLY);
            _gamepadModel.RightThumbstick.X = _rangeConverter.ScaleThumbstick(e.CurrentInputState.Gamepad.sThumbRX);
            _gamepadModel.RightThumbstick.Y = _rangeConverter.ScaleThumbstick(e.CurrentInputState.Gamepad.sThumbRY);

            //triggers are already in 0-255 range
            _gamepadModel.LeftTrigger = e.CurrentInputState.Gamepad.bLeftTrigger;
            _gamepadModel.RightTrigger = e.CurrentInputState.Gamepad.bRightTrigger;

            //buttons - on thumbstick
            _gamepadModel.LeftThumbstick.IsPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_THUMB);
            _gamepadModel.RightThumbstick.IsPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_THUMB);

            //buttons - on cross
            _gamepadModel.Cross.IsLeftPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_LEFT);
            _gamepadModel.Cross.IsRightPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_RIGHT);
            _gamepadModel.Cross.IsUpPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_UP);
            _gamepadModel.Cross.IsDownPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_DPAD_DOWN);

            //buttons - on shoulder (above triggers)
            _gamepadModel.IsLeftPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_LEFT_SHOULDER);
            _gamepadModel.IsRightPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_RIGHT_SHOULDER);

            //button - colored
            _gamepadModel.ActionButtons.IsAPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_A);
            _gamepadModel.ActionButtons.IsBPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_B);
            _gamepadModel.ActionButtons.IsXPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_X);
            _gamepadModel.ActionButtons.IsYPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_Y);

            //buttons - start & back
            _gamepadModel.IsStartPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_START);
            _gamepadModel.IsBackPressed =
                e.CurrentInputState.Gamepad.IsButtonPressed((int)ButtonFlags.XINPUT_GAMEPAD_BACK);
            #endregion

            //TODO
            //probably these events needs some refactoring to prevent nulls
            //consider creating separate event args
            GamepadStateChanged?.Invoke(this, new GamepadEventArgs { GamepadModel = _gamepadModel, RobotControl = null });

            //this method also rises robot controll event
            TryToProcessMixing();
        }

        private void TryToProcessMixing()
        {
            try
            {
                var mixer = new OutputMixer(SteeringConfig);
                var robotControls = mixer.Process(_gamepadModel);

                RobotControlChanged?.Invoke(this, new GamepadEventArgs { GamepadModel = null, RobotControl = robotControls });
            }
            catch (Exception exception)
            {
                GamepadErrorOccured?.Invoke(this, new GamepadErrorEventArgs(exception)); ;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lowPassFilterTimer.Start();
                _lowPassFilterTimer.Dispose();
                XboxController.StopPolling();
            }
        }
    }
}
