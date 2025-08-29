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
        private float _previousVolume;

        public WebAdvertisement(bool isAvailable, IEnvironment environment)
        {
            IsAvailable = isAvailable;
            _environment = environment;
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            var wasShown = true;

            PluginRuntime.ShowInterstitial(CallbackProxy);
            return;
            void CallbackProxy(InterstitialState state)
            {
                switch (state)
                {
                    case InterstitialState.Opened:
                        wasShown = true;
                        AdOpened();
                        break;
                    default:
                        RestoreAfterAd(wasShown);
                        break;
                }
                callbackState(state);
            }
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
            var wasShown = true;
            
            PluginRuntime.ShowRewarded(CallbackProxy);
            return;

            void CallbackProxy(RewardedState state)
            {
                switch (state)
                {
                    case RewardedState.Opened:
                        wasShown = true;
                        AdOpened();
                        break;
                    default:
                        RestoreAfterAd(wasShown);
                        break;
                }
                callbackState(state);
            }
        }

        private void AdOpened()
        {
            if (!_environment.IsAvailable || _environment.DeviceType != DeviceType.Desktop) return;
            
            _previousCursorVisible = Cursor.visible;
            _previousCursorLockMode = Cursor.lockState;
            _previousVolume = AudioListener.volume;
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            AudioListener.volume = 0;
        }

        private void RestoreAfterAd(bool wasShown)
        {
            if (!wasShown) return;
            if (!_environment.IsAvailable || _environment.DeviceType != DeviceType.Desktop) return;

            Cursor.visible = _previousCursorVisible;
            Cursor.lockState = _previousCursorLockMode;
            AudioListener.volume = _previousVolume;
        }
    }
}