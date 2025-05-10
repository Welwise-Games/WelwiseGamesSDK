using System;
using WelwiseGamesSDK.Internal;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class WelwiseSDK : ISDK
    {
        public static SDKBuilder Construct() => new (SDKSettings.LoadOrCreateSettings());

        #region Singletone
        internal static void SetSDK(ISDK sdk) => _sdk = sdk;
        public static ISDK Instance => _sdk;
        private static ISDK _sdk;

        public ISaves GameSaves => _sdk.GameSaves;

        public IEnvironment Environment => _sdk.Environment;

        public IAdvertisement Advertisement => _sdk.Advertisement;

        public IAnalytics Analytics => _sdk.Analytics;
        public IPlatformNavigation PlatformNavigation => _sdk.PlatformNavigation;

        public void Initialize() => _sdk.Initialize();

        public event Action Initialized
        {
            add => _sdk.Initialized += value;
            remove => _sdk.Initialized -= value;
        }
        public bool IsInitialized => _sdk.IsInitialized;
        #endregion
    }
}