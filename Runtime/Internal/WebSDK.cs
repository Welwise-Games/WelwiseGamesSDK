using System;
using UnityEngine;
using WelwiseGamesSDK.Internal.Advertisement;
using WelwiseGamesSDK.Internal.Analytics;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.GameSaves;
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
        
        
        private readonly WebGameSaves _webGameSaves;
        private bool _initializeRunning;

        public WebSDK(SDKSettings sdkSettings)
        {
            IsInitialized = false;
            Environment = new WebEnvironment();
            Analytics = new WebAnalytics();
            Advertisement = new WebAdvertisement();
            _webGameSaves = new WebGameSaves(sdkSettings);
        }
        
        
        public void Initialize()
        {
            if (IsInitialized) return;
            if (_initializeRunning) return;
            
            _initializeRunning = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.JsInit(() =>
            {
                _webGameSaves.Ready += OnReady;
                _webGameSaves.Load();
            }, Debug.LogError);
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