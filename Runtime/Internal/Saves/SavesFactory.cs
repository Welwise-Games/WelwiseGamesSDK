using System;
using WelwiseGamesSDK.Internal.Saves.WelwiseGames;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal static class SavesFactory
    {
        private static CombinedSync _combinedSync;
        
        public static ISaves CreateGameSaves(WebSender webSender, ISDKConfig config, IEnvironment environment)
        {
            if (config.UseMetaverse) _combinedSync ??= new CombinedSync(webSender);
            return config.Mode switch
            {
                SDKMode.Debug => new PlayerPrefsSaves(webSender, false),
                SDKMode.Development or SDKMode.Production => new GamePlatformSaves(
                    webSender, 
                    config.SyncDelay, 
                    environment, 
                    config.UseMetaverse,
                    config.MetaverseId,
                    config.GameId,
                    _combinedSync),
                _ => throw new NotImplementedException()
            };
        }

        public static ISaves CreateMetaverseGameSaves(WebSender webSender, ISDKConfig config, IEnvironment environment)
        {
            if (config.UseMetaverse) _combinedSync ??= new CombinedSync(webSender);
            else return new NotSupportedSaves();

            return config.Mode switch
            {
                SDKMode.Debug => new PlayerPrefsSaves(webSender, true),
                SDKMode.Development or SDKMode.Production => new MetaversePlatformSaves(
                    webSender, 
                    config.SyncDelay, 
                    environment, 
                    config.UseMetaverse,
                    config.MetaverseId,
                    config.GameId,
                    _combinedSync),
                _ => throw new NotImplementedException()
            };
        }
    }
}