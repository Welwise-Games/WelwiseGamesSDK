using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.PlatformNavigation;
using WelwiseGamesSDK.Internal.PlayerData;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    public class UnitySDK : ISDK
    {
        public event Action Initialized;
        public bool IsInitialized { get; private set; }
        public IPlayerData PlayerData { get; }
        public IEnvironment Environment { get; }
        public IAdvertisement Advertisement { get; }
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }

        private readonly SDKSettings _settings;

        private bool _isSimulatingInitialize;
        
        public UnitySDK(SDKSettings settings)
        {
            Environment = new UnityEnvironment(settings);
            Advertisement = new UnityAdvertisement(settings);
            Analytics = new UnityAnalytics();
            PlatformNavigation = new UnityPlatformNavigation();
            PlayerData = new UnityPlayerData();
            _settings = settings;
        }
        public void Initialize()
        {
            if (IsInitialized) return;
            PlayerData.Load();
            if (!_isSimulatingInitialize) PluginRuntime.StartCoroutine(InitializeSimulation());
        }


        private IEnumerator InitializeSimulation()
        {
            _isSimulatingInitialize = true;
            if (_settings.DebugInitializeTime <= 0) yield return new WaitForEndOfFrame();
            else yield return new WaitForSeconds(_settings.DebugInitializeTime);
            _isSimulatingInitialize = false;
            Initialized?.Invoke();
            IsInitialized = true;
        }
    }
}