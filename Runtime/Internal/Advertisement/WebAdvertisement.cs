using System;
using System.Runtime.InteropServices;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : MonoBehaviour, IAdvertisement
    {
        private Action<InterstitialState> _interstitialCallback;
        private Action<RewardedState> _rewardedCallback;

        [DllImport("__Internal")]
        private static extern void JsShowInterstitial(string onClose, string onError);

        [DllImport("__Internal")]
        private static extern void JsShowRewarded(
            string onOpen, 
            string onRewarded, 
            string onClose, 
            string onError
        );

        public static WebAdvertisement Create()
        {
            var go = new GameObject("WebAdvertisement");
            DontDestroyOnLoad(go);
            return go.AddComponent<WebAdvertisement>();
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            _interstitialCallback = callbackState;
            try
            {
                JsShowInterstitial(
                    "HandleInterstitialClose",
                    "HandleInterstitialError"
                );
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
                JsShowRewarded(
                    "HandleRewardedOpen",
                    "HandleRewardedReward",
                    "HandleRewardedClose",
                    "HandleRewardedError"
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error showing rewarded: {e.Message}");
                _rewardedCallback?.Invoke(RewardedState.Error);
            }
        }

        public void HandleInterstitialClose(string wasShown)
        {
            _interstitialCallback?.Invoke(InterstitialState.Closed);
        }

        public void HandleInterstitialError(string errorJson)
        {
            Debug.LogError($"Interstitial error: {errorJson}");
            _interstitialCallback?.Invoke(InterstitialState.Error);
        }

        public void HandleRewardedOpen()
        {
            Debug.Log("Rewarded ad opened");
            _rewardedCallback?.Invoke(RewardedState.Opened);
        }

        public void HandleRewardedReward()
        {
            Debug.Log("Reward earned");
            _rewardedCallback?.Invoke(RewardedState.Rewarded);
        }

        public void HandleRewardedClose()
        {
            Debug.Log("Rewarded ad closed");
            _rewardedCallback?.Invoke(RewardedState.Closed);
        }

        public void HandleRewardedError(string errorJson)
        {
            Debug.LogError($"Rewarded error: {errorJson}");
            _rewardedCallback?.Invoke(RewardedState.Error);
        }
    }
}