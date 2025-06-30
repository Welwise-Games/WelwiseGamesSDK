using System;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class WebAdvertisement : IAdvertisement
    {
        public bool IsAvailable { get; }

        public WebAdvertisement(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState) => PluginRuntime.ShowInterstitial(callbackState);
        public void ShowRewarded(Action<RewardedState> callbackState) => PluginRuntime.ShowRewarded(callbackState);
    }
}