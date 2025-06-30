namespace WelwiseGamesSDK.Shared.Types
{
    /// <summary>
    /// Represents possible states of an interstitial advertisement.
    /// Used in interstitial ad callbacks.
    /// </summary>
    public enum InterstitialState
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
        /// Ad was closed
        /// </summary>
        Closed,
    }
}