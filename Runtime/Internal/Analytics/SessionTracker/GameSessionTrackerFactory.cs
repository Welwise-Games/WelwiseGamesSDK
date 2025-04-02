using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics.SessionTracker
{
    internal static class GameSessionTrackerFactory
    {
        public static IGameSessionTracker CreateGameSessionTracker(
            SDKSettings settings,
            WebSender webSender,
            IEnvironment environment)
        {
            return settings.Mode switch
            {
                SDKMode.Development or SDKMode.Production => new GameSessionTracker(settings, webSender, environment),
                SDKMode.Debug => new DebugGameSessionTracker(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}