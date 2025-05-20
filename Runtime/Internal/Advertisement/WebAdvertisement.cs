using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            Debug.Log($"<! NOT IMPL !> [{nameof(WebAdvertisement)}] {nameof(ShowInterstitial)}");
            callbackState?.Invoke(InterstitialState.Closed);
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
            Debug.Log($"<! NOT IMPL !> [{nameof(WebAdvertisement)}] {nameof(ShowRewarded)}");
            callbackState?.Invoke(RewardedState.Rewarded);
        }
    }
}