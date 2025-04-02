using System;
using System.Runtime.InteropServices;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        private Action<InterstitialState> _interstitialCallback;
        private Action<RewardedState> _rewardedCallback;

        [DllImport("__Internal")]
        private static extern void JsShowInterstitial();

        [DllImport("__Internal")]
        private static extern void JsShowRewarded();

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            _interstitialCallback = callbackState;
            try
            {
                JsShowInterstitial();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error showing interstitial: {e.Message}");
                _interstitialCallback?.Invoke(InterstitialState.Error);
            }
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
            _rewardedCallback = callbackState;
            try
            {
                JsShowRewarded();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error showing rewarded: {e.Message}");
                _rewardedCallback?.Invoke(RewardedState.Error);
            }
        }

        public void OnInterstitialState(string state)
        {
            if (Enum.TryParse(state, out InterstitialState parsedState))
            {
                _interstitialCallback?.Invoke(parsedState);
            }
        }

        public void OnRewardedState(string state)
        {
            if (Enum.TryParse(state, out RewardedState parsedState))
            {
                _rewardedCallback?.Invoke(parsedState);
            }
        }
    }
}