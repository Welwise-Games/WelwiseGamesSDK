using System;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.GameSaves;
using WelwiseGamesSDK.Internal.PlatformNavigation;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    internal sealed class WebSDK : ISDK
    {
        public event Action Initialized;
        public bool IsInitialized { get; private set; }
        public ISaves GameSaves => _webGameSaves;
        public IEnvironment Environment { get; }
        public IAdvertisement Advertisement { get; }
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }


        private readonly WebGameSaves _webGameSaves;
        private bool _initializeRunning;

        public WebSDK(SDKSettings sdkSettings)
        {
            IsInitialized = false;
            Environment = new WebEnvironment();
            Analytics = new WebAnalytics();
            Advertisement = new WebAdvertisement();
            _webGameSaves = new WebGameSaves(sdkSettings);
            PlatformNavigation = new WebPlatformNavigation();
        }
        
        
        public void Initialize()
        {
            if (IsInitialized) return;
            if (_initializeRunning) return;
            
            _initializeRunning = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.Init(
                onSuccess: () => 
                {
                    _webGameSaves.Ready += OnReady;
                    _webGameSaves.Load();
                },
                onError: error => Debug.LogError(error)
            );
#endif

            
            return;
            void OnReady()
            {
                _initializeRunning = false;
                Initialized?.Invoke();
                _webGameSaves.Ready -= OnReady;
                IsInitialized = true;
            }
        }
    }
}