using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class UnityAdvertisement : IAdvertisement
    {
        private readonly SDKSettings _settings;

        public UnityAdvertisement(SDKSettings settings)
        {
            _settings = settings;
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState = null) =>
            PluginRuntime.StartCoroutine(ShowInterstitialRoutine(callbackState));

        public void ShowRewarded(Action<RewardedState> callbackState = null) =>
            PluginRuntime.StartCoroutine(ShowRewardedRoutine(callbackState));
        
        private IEnumerator ShowInterstitialRoutine(Action<InterstitialState> callbackState)
        {
            if (_settings.AdSimulationDuration <= 0) yield return new WaitForEndOfFrame();
            else yield return new WaitForSeconds(_settings.AdSimulationDuration);
            callbackState?.Invoke(_settings.InterstitialAdReturnState);
        }
        
        private IEnumerator ShowRewardedRoutine(Action<RewardedState> callbackState)
        {
            if (_settings.AdSimulationDuration <= 0) yield return new WaitForEndOfFrame();
            else yield return new WaitForSeconds(_settings.AdSimulationDuration);
            callbackState?.Invoke(_settings.RewardedAdReturnState);
        }
    }
}