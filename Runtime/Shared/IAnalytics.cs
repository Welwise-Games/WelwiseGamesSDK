namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Provides analytics tracking capabilities for game events.
    /// Implement this interface to send custom events and gameplay milestones.
    /// </summary>
    public interface IAnalytics
    {
        /// <summary>
        /// Sends a custom analytics event without additional data.
        /// </summary>
        /// <param name="eventName">Name of the event to track (case-sensitive)</param>
        public void SendEvent(string eventName);
        
        /// <summary>
        /// Sends a custom analytics event with additional JSON data.
        /// </summary>
        /// <param name="eventName">Name of the event to track</param>
        /// <param name="data">Additional event data in JSON format</param>
        public void SendEvent(string eventName, string data);

        /// <summary>
        /// Tracks when the game has finished loading and is ready for play.
        /// Should be called after all initializations are complete.
        /// </summary>
        public void GameIsReady();
        
        /// <summary>
        /// Tracks the start of gameplay session.
        /// Call when player enters core game interaction.
        /// </summary>
        public void GameplayStart();
        
        /// <summary>
        /// Tracks the end of gameplay session.
        /// Call when player exits core game interaction.
        /// </summary>
        public void GameplayEnd();
    }
}