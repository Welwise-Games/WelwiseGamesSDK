using System;
using System.Collections.Generic;
using WelwiseGamesSDK.Internal.Saves.WelwiseGames;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal static class SavesFactory
    {
        private static CombinedSync _combinedSync;
        
        public static ISaves CreateGameSaves(
            WebSender webSender, 
            ISDKConfig config, 
            IEnvironment environment, 
            List<INeedInitializeService> initializeServices)
        {
            InitializeCombinedSyncIfNeeded(config, webSender);

            switch (config.Mode)
            {
                case SDKMode.Debug:
                    var debugSaves = new PlayerPrefsSaves(webSender, false);
                    initializeServices.Add(debugSaves);
                    return debugSaves;
                
                case SDKMode.Development:
                case SDKMode.Production:
                    var productionSaves = new GamePlatformSaves(
                        webSender, 
                        config.SyncDelay, 
                        environment, 
                        config.UseMetaverse,
                        config.MetaverseId, 
                        config.GameId, 
                        _combinedSync);
                    initializeServices.Add(productionSaves);
                    return productionSaves;
                
                default:
                    throw new NotImplementedException();
            }
        }

        public static ISaves CreateMetaverseGameSaves(
            WebSender webSender, 
            ISDKConfig config, 
            IEnvironment environment, 
            List<INeedInitializeService> initializeServices)
        {
            if (!config.UseMetaverse)
                return new NotSupportedSaves();

            InitializeCombinedSyncIfNeeded(config, webSender);

            switch (config.Mode)
            {
                case SDKMode.Debug:
                    var metaverseDebugSaves = new PlayerPrefsSaves(webSender, true);
                    initializeServices.Add(metaverseDebugSaves);
                    return metaverseDebugSaves;
                
                case SDKMode.Development:
                case SDKMode.Production:
                    var metaverseProductionSaves = new MetaversePlatformSaves(
                        webSender, 
                        config.SyncDelay, 
                        environment,
                        config.UseMetaverse,
                        config.MetaverseId, 
                        config.GameId, 
                        _combinedSync);
                    initializeServices.Add(metaverseProductionSaves);
                    return metaverseProductionSaves;
                
                default:
                    throw new NotImplementedException();
            }
        }

        private static void InitializeCombinedSyncIfNeeded(ISDKConfig config, WebSender webSender)
        {
            if (config.UseMetaverse && _combinedSync == null)
            {
                _combinedSync = new CombinedSync(webSender);
            }
        }
    }
}