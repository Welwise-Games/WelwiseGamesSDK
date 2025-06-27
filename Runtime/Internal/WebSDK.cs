using System;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.Payments;
using WelwiseGamesSDK.Internal.PlatformNavigation;
using WelwiseGamesSDK.Internal.PlayerData;
using WelwiseGamesSDK.Shared;

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
        private readonly WebPlayerData _webPlayerData;
        private readonly SDKSettings _settings;
        
        private bool _initializeRunning;

        public WebSDK(SDKSettings sdkSettings)
        {
            IsInitialized = false;
            Analytics = new WebAnalytics();
            Advertisement = new WebAdvertisement();
            PlatformNavigation = new WebPlatformNavigation();
            _webEnvironment = new WebEnvironment();
            _webPlayerData = new WebPlayerData(sdkSettings, _webEnvironment);
            Payments = new WebPayments();
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

            if (_settings.LoadSaveOnInitialize && !_webPlayerData.IsLoaded)
            {
                _webPlayerData.Loaded += HandleSavesReady;
                _webPlayerData.Load();
            }
            else
            {
                CompleteInitialization();
            }
        }

        private void HandleSavesReady()
        {
            Debug.Log("[WebSDK] Game saves ready");
            _webPlayerData.Loaded -= HandleSavesReady;
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