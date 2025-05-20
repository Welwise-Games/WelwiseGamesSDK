using System;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
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

        private readonly WebEnvironment _webEnvironment;
        private readonly WebPlayerData _webPlayerData;
        
        private bool _initializeRunning;

        public WebSDK(SDKSettings sdkSettings)
        {
            IsInitialized = false;
            Analytics = new WebAnalytics();
            Advertisement = new WebAdvertisement();
            PlatformNavigation = new WebPlatformNavigation();
            _webEnvironment = new WebEnvironment();
            _webPlayerData = new WebPlayerData(sdkSettings, _webEnvironment);
        }

        public void Initialize()
        {
            if (IsInitialized || _initializeRunning) return;

            _initializeRunning = true;

#if UNITY_WEBGL &&! UNITY_EDITOR
            Debug.Log("Initializing WebSDK");
            JsLibProvider.Init(
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
#endif
        }
        

        private void HandleEnvironmentReady()
        {
            Debug.Log("[WebSDK] Environment ready");
            _webEnvironment.Ready -= HandleEnvironmentReady;
            _webPlayerData.Ready += HandleSavesReady;
            _webPlayerData.Load();
        }

        private void HandleSavesReady()
        {
            Debug.Log("[WebSDK] Game saves ready");
            _webPlayerData.Ready -= HandleSavesReady;
            IsInitialized = true;
            _initializeRunning = false;
            Debug.Log("[WebSDK] Full initialization complete");
            Initialized?.Invoke();
        }
    }
}