using System;
using System.Collections.Generic;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.Saves;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    internal sealed class SDKBuilder
    {
        internal class Build : ISDK
        {
            public ISaves GameSaves { get; internal set; }
            public ISaves MetaverseSaves { get; internal set; }
            public ISDKConfig Config { get; internal set; }
            public IEnvironment Environment { get; internal set; }
            public readonly List<INeedInitializeService> NeedInitializeServices = new();
        }

        private SDKSettings _sdkSettings;
        private WebSender _webSender;

        internal Build Create()
        {
            _sdkSettings = SDKSettings.LoadOrCreateSettings();
            _webSender = WebSender.Create();
            _webSender.Initialize(_sdkSettings.ApiAuthKey);

            return _sdkSettings.SupportedSDKType switch
            {
                SupportedSDKType.WelwiseGames => CreateWelwiseSDKBuild(),
                _ => throw new NotImplementedException()
            };
        }

        private Build CreateWelwiseSDKBuild()
        {
            var build = new Build();
            
            var env = EnvironmentFactory.Create(_sdkSettings.Mode == SDKMode.Debug,
                _sdkSettings.DebugPlayerId, _sdkSettings.IsMetaverseConnected);
            var gameSaves = SavesFactory.CreateMetaverseGameSaves(_webSender, _sdkSettings, env);
            var metaverseSaves = SavesFactory.CreateGameSaves(_webSender, _sdkSettings, env);
            build.NeedInitializeServices.Add(gameSaves);
            build.NeedInitializeServices.Add(metaverseSaves);
            
            build.GameSaves = gameSaves;
            build.MetaverseSaves = metaverseSaves;
            build.Environment = env;
            build.Config = _sdkSettings;
            return build;
        }
    }
}