namespace WelwiseGamesSDK.Shared.Types
{
    /// <summary>
    /// Represents possible states of a rewarded advertisement.
    /// Used in rewarded ad callbacks.
    /// </summary>
    public enum RewardedState
    {
        /// <summary>
        /// Ad failed to load or display
        /// </summary>
        Error,
        
        /// <summary>
        /// Ad was successfully displayed
        /// </summary>
        Opened,
        
        /// <summary>
        /// Ad was closed without reward
        /// </summary>
        Closed,
        
        /// <summary>
        /// User earned reward by completing the ad
        /// </summary>
        Rewarded,
    }
}