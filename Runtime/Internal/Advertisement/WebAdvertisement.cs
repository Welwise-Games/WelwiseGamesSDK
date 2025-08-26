using System;
using UnityEngine;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseGamesSDK.Shared.Types;
using DeviceType = WelwiseGamesSDK.Shared.Types.DeviceType;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        public bool IsAvailable { get; }
        
        private readonly IEnvironment _environment;

        private CursorLockMode _previousCursorLockMode;
        private bool _previousCursorVisible;

        public WebAdvertisement(bool isAvailable, IEnvironment environment)
        {
            IsAvailable = isAvailable;
            _environment = environment;
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            HideCursor();
            
            PluginRuntime.ShowInterstitial(CallbackProxy);
            
            return;
            void CallbackProxy(InterstitialState state)
            {
                RestoreCursor();
                callbackState(state);
            }
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
            HideCursor();
            
            PluginRuntime.ShowRewarded(CallbackProxy);
            return;

            void CallbackProxy(RewardedState state)
            {
                RestoreCursor();
                callbackState(state);
            }
        }

        private void HideCursor()
        {
            if (!_environment.IsAvailable || _environment.DeviceType != DeviceType.Desktop) return;
            
            _previousCursorVisible = Cursor.visible;
            _previousCursorLockMode = Cursor.lockState;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void RestoreCursor()
        {
            if (!_environment.IsAvailable || _environment.DeviceType != DeviceType.Desktop) return;

            Cursor.visible = _previousCursorVisible;
            Cursor.lockState = _previousCursorLockMode;
        }
    }
}