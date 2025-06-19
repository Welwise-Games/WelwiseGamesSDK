using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        public void ShowInterstitial(Action<InterstitialState> callbackState) => PluginRuntime.ShowInterstitial(callbackState);
        public void ShowRewarded(Action<RewardedState> callbackState) => PluginRuntime.ShowRewarded(callbackState);
    }
}