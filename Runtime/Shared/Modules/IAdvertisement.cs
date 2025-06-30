using System;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Shared.Modules
{
    /// <summary>
    /// Provides advertisement display capabilities.
    /// Implement this interface to show interstitial and rewarded ads.
    /// </summary>
    public interface IAdvertisement : IModule
    {
        /// <summary>
        /// Displays an interstitial advertisement.
        /// </summary>
        /// <param name="callbackState">Optional callback to receive ad state notifications</param>
        public void ShowInterstitial(Action<InterstitialState> callbackState = null);
        
        /// <summary>
        /// Displays a rewarded video advertisement.
        /// </summary>
        /// <param name="callbackState">Optional callback to receive ad state and reward notifications</param>
        public void ShowRewarded(Action<RewardedState> callbackState = null);
    }
}