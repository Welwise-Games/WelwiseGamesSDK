using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.ModuleSupport;
using WelwiseGamesSDK.Internal.Payments;
using WelwiseGamesSDK.Internal.PlatformNavigation;
using WelwiseGamesSDK.Internal.PlayerData;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;

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
        public IPayments Payments { get; }

        private readonly SDKSettings _settings;

        private bool _isSimulatingInitialize;
        
        public UnitySDK(SDKSettings settings)
        {
            var moduleSupport = new UnityModuleSupport(settings);
            Environment = new UnityEnvironment(settings, moduleSupport.CheckModule(SupportedModuleKeys.EnvironmentModuleKey));
            Advertisement = new UnityAdvertisement(settings, moduleSupport.CheckModule(SupportedModuleKeys.AdvertisementModuleKey));
            Analytics = new UnityAnalytics(moduleSupport.CheckModule(SupportedModuleKeys.AnalyticsModuleKey));
            PlatformNavigation = new UnityPlatformNavigation(moduleSupport.CheckModule(SupportedModuleKeys.PlatformNavigationModuleKey));
            PlayerData = new UnityPlayerData(moduleSupport.CheckModule(SupportedModuleKeys.PlayerDataModuleKey), 
                moduleSupport.CheckModule(SupportedModuleKeys.GameDataModuleKey),
                moduleSupport.CheckModule(SupportedModuleKeys.MetaverseDataModuleKey));
            Payments = new UnityPayments(moduleSupport.CheckModule(SupportedModuleKeys.PaymentsModuleKey));
            _settings = settings;
        }
        public void Initialize()
        {
            if (IsInitialized) return;
            PlayerData.Initialize();
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