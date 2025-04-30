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

#if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void JsShowInterstitial(string onClose, string onError);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void JsShowRewarded(
            string onOpen, 
            string onRewarded, 
            string onClose, 
            string onError
        );
#endif

        public static WebAdvertisement Create()
        {
            var go = new GameObject("WebAdvertisement");
            DontDestroyOnLoad(go);
            return go.AddComponent<WebAdvertisement>();
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
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
#else
            Debug.LogError("showing interstitial");
            _interstitialCallback?.Invoke(InterstitialState.Opened);
#endif
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
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
#else
            Debug.LogError("showing rewarded");
            _rewardedCallback?.Invoke(RewardedState.Rewarded);
#endif
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