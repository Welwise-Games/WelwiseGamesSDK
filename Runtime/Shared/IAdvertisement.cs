using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IAdvertisement
    {
        public void ShowInterstitial(Action<InterstitialState> callbackState = null);
        public void ShowRewarded(Action<RewardedState> callbackState = null);
    }
}