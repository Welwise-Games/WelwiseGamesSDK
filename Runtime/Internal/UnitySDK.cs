using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.GameSaves;
using WelwiseGamesSDK.Internal.PlatformNavigation;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    public class UnitySDK : ISDK
    {
        public event Action Initialized;
        public bool IsInitialized { get; private set; }
        public ISaves GameSaves { get; }
        public IEnvironment Environment { get; }
        public IAdvertisement Advertisement => _unityAdvertisement;
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }

        private readonly UnityAdvertisement _unityAdvertisement;
        private bool _isSimulatingInitialize;
        
        public UnitySDK(SDKSettings settings)
        {
            GameSaves = new UnityGameSaves();
            Environment = new UnityEnvironment(settings.DebugPlayerId, settings.DebugDeviceType,
                settings.DebugLanguageCode);
            _unityAdvertisement = UnityAdvertisement.Create();
            Analytics = new UnityAnalytics();
            PlatformNavigation = new UnityPlatformNavigation();
        }
        public void Initialize()
        {
            if (IsInitialized) return;
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