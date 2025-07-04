﻿using System;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class WelwiseSDK : ISDK
    {
        public static SDKBuilder Construct() => new (SDKSettings.LoadOrCreateSettings());

        internal static void SetSDK(ISDK sdk) => _sdk = sdk;
        public static ISDK Instance => _sdk;
        private static ISDK _sdk;
        public IPlayerData PlayerData => _sdk.PlayerData;
        public IEnvironment Environment => _sdk.Environment;
        public IAdvertisement Advertisement => _sdk.Advertisement;
        public IAnalytics Analytics => _sdk.Analytics;
        public IPlatformNavigation PlatformNavigation => _sdk.PlatformNavigation;
        public IPayments Payments => _sdk.Payments;
        public void Initialize() => _sdk.Initialize();
        public event Action Initialized
        {
            add => _sdk.Initialized += value;
            remove => _sdk.Initialized -= value;
        }
        public bool IsInitialized => _sdk.IsInitialized;
    }
}