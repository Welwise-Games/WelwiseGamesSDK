using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal static class SavesFactory
    {
        public static ISaves CreateGameSaves(WebSender webSender, ISDKConfig config, IEnvironment environment)
        {
            return config.Mode switch
            {
                SDKMode.Debug => new PlayerPrefsSaves(webSender, false),
                SDKMode.Development or SDKMode.Production => new GamePlatformSaves(webSender, config.SyncDelay,
                    config.GameId, environment),
                _ => throw new NotImplementedException()
            };
        }

        public static ISaves CreateMetaverseGameSaves(WebSender webSender, ISDKConfig config, IEnvironment environment)
        {
            if (!environment.UseMetaverse)
                return new NotSupportedSaves();
            return config.Mode switch
            {
                SDKMode.Debug => new PlayerPrefsSaves(webSender, true),
                SDKMode.Development or SDKMode.Production => new MetaversePlatformSaves(webSender, config.SyncDelay,
                    environment, config.MetaverseId),
                _ => throw new NotImplementedException()
            };
        }
    }
}