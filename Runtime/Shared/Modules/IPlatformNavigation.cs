﻿using System;

namespace WelwiseGamesSDK.Shared.Modules
{
    /// <summary>
    /// Provides platform navigation capabilities.
    /// Implement this interface for platform game navigation.
    /// </summary>
    public interface IPlatformNavigation : IModule
    {
        /// <summary>
        /// Navigates to another game in the platform ecosystem.
        /// </summary>
        /// <param name="id">Target slug game identifier</param>
        /// <param name="onError">Callback for handling navigation errors</param>
        public void GoToGame(string id, Action<string> onError);
    }
}