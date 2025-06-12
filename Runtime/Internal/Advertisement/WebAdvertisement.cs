using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                JsLibProvider.ShowInterstitial(callbackState);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(WebAdvertisement)}] {nameof(ShowInterstitial)} failed: {e}");
                callbackState?.Invoke(InterstitialState.Error);
            }
#endif
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                JsLibProvider.ShowRewarded(callbackState);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(WebAdvertisement)}] {nameof(ShowRewarded)} failed: {e}");
                callbackState?.Invoke(RewardedState.Error);
            }
#endif
        }
    }
}