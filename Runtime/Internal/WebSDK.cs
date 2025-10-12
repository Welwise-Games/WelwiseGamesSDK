using System;
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
    internal sealed class WebSDK : ISDK
    {
        #pragma warning disable
        public event Action Initialized;
        #pragma warning restore
        public bool IsInitialized { get; private set; }
        public IPlayerData PlayerData => _webPlayerData;
        public IEnvironment Environment => _webEnvironment;
        public IAdvertisement Advertisement { get; }
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }
        public IPayments Payments { get; }

        private readonly WebEnvironment _webEnvironment;
        private readonly PlayerData.PlayerData _webPlayerData;
        private readonly SDKSettings _settings;
        
        private bool _initializeRunning;

        public WebSDK(SDKSettings sdkSettings)
        {
            var moduleSupport = new WebModuleSupport();
            IsInitialized = false;
            Analytics = new WebAnalytics(moduleSupport.CheckModule(SupportedModuleKeys.AnalyticsModuleKey));
            _webEnvironment = new WebEnvironment(moduleSupport.CheckModule(SupportedModuleKeys.EnvironmentModuleKey));
            Advertisement = new WebAdvertisement(moduleSupport.CheckModule(SupportedModuleKeys.AdvertisementModuleKey), _webEnvironment);

            var webPlayerDataImplemented = moduleSupport.CheckModule(SupportedModuleKeys.PlayerDataModuleKey);
            if (webPlayerDataImplemented)
            {
                _webPlayerData = new WebPlayerData(sdkSettings, _webEnvironment, 
                    moduleSupport.CheckModule(SupportedModuleKeys.PlayerDataModuleKey),
                    moduleSupport.CheckModule(SupportedModuleKeys.GameDataModuleKey),
                    moduleSupport.CheckModule(SupportedModuleKeys.MetaverseDataModuleKey));
            }
            else
            {
                _webPlayerData = new UnityPlayerData(true, true, false);
            }

            PlatformNavigation = new WebPlatformNavigation(_webPlayerData, 
                moduleSupport.CheckModule(SupportedModuleKeys.PlatformNavigationModuleKey));
            Payments = new WebPayments(moduleSupport.CheckModule(SupportedModuleKeys.PaymentsModuleKey));
            _settings = sdkSettings;
        }

        public void Initialize()
        {
            if (IsInitialized || _initializeRunning) return;

            _initializeRunning = true;

            Debug.Log("Initializing WebSDK");
            PluginRuntime.Init(
                onSuccess: () =>
                {
                    Debug.Log("WebSDK init success");
                    _webEnvironment.Ready += HandleEnvironmentReady;
                    _webEnvironment.Load();
                },
                onError: error =>
                {
                    Debug.LogError($"[WebSDK] Init failed: {error}");
                    _initializeRunning = false;
                }
            );
        }
        

        private void HandleEnvironmentReady()
        {
            Debug.Log("[WebSDK] Environment ready");
            _webEnvironment.Ready -= HandleEnvironmentReady;

            if (_settings.LoadSaveOnInitialize && !_webPlayerData.IsInitialized)
            {
                _webPlayerData.Initialized += HandleSavesReady;
                _webPlayerData.Initialize();
            }
            else
            {
                CompleteInitialization();
            }
        }

        private void HandleSavesReady()
        {
            Debug.Log("[WebSDK] Game saves ready");
            _webPlayerData.Initialized -= HandleSavesReady;
            CompleteInitialization();
        }

        private void CompleteInitialization()
        {
            IsInitialized = true;
            _initializeRunning = false;
            Debug.Log("[WebSDK] Full initialization complete");
            Initialized?.Invoke();
        }
    }
}