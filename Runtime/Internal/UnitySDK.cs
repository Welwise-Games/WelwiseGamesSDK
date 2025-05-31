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
        public IPlayerData PlayerData => _unityPlayerData;
        public IEnvironment Environment { get; }
        public IAdvertisement Advertisement => _unityAdvertisement;
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }

        private readonly UnityAdvertisement _unityAdvertisement;
        private readonly UnityPlayerData _unityPlayerData;
        private readonly SDKSettings _settings;

        private bool _isSimulatingInitialize;
        
        public UnitySDK(SDKSettings settings)
        {
            Environment = new UnityEnvironment(settings.DebugPlayerId, settings.DebugDeviceType,
                settings.DebugLanguageCode);
            _unityAdvertisement = UnityAdvertisement.Create();
            Analytics = new UnityAnalytics();
            PlatformNavigation = new UnityPlatformNavigation();
            _unityPlayerData = new UnityPlayerData();
            _settings = settings;
        }
        public void Initialize()
        {
            if (IsInitialized) return;
            SoundMute.CreateIfNeeded(_settings);
            _unityPlayerData.Load();
            if (!_isSimulatingInitialize) _unityAdvertisement.StartCoroutine(InitializeSimulation());
        }


        private IEnumerator InitializeSimulation()
        {
            _isSimulatingInitialize = true;
            yield return new WaitForSeconds(1.5f);
            _isSimulatingInitialize = false;
            Initialized?.Invoke();
            IsInitialized = true;
        }
    }
}