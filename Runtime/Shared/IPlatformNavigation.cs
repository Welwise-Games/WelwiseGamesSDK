using System;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Provides platform navigation capabilities.
    /// Implement this interface for platform game navigation.
    /// </summary>
    public interface IPlatformNavigation
    {
        /// <summary>
        /// Navigates to another game in the platform ecosystem.
        /// </summary>
        /// <param name="id">Target game identifier</param>
        /// <param name="onError">Callback for handling navigation errors</param>
        public void GoToGame(int id, Action<string> onError);
    }
}