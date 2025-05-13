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
        public IEnvironment Environment => _webEnvironment;
        public IAdvertisement Advertisement { get; }
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }

        private readonly WebGameSaves _webGameSaves;
        private readonly WebEnvironment _webEnvironment;
        private bool _initializeRunning;
        private bool _environmentReady;
        private bool _savesReady;

        public WebSDK(SDKSettings sdkSettings)
        {
            IsInitialized = false;
            Analytics = new WebAnalytics();
            Advertisement = new WebAdvertisement();
            PlatformNavigation = new WebPlatformNavigation();
            _webGameSaves = new WebGameSaves(sdkSettings);
            _webEnvironment = new WebEnvironment();
        }

        public void Initialize()
        {
            if (IsInitialized || _initializeRunning) return;

            _initializeRunning = true;
            ResetState();

#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.Init(
                onSuccess: () =>
                {
                    _webEnvironment.Ready += HandleEnvironmentReady;
                    _webEnvironment.Load();

                    _webGameSaves.Ready += HandleSavesReady;
                    _webGameSaves.Load();
                },
                onError: error =>
                {
                    Debug.LogError($"[WebSDK] Init failed: {error}");
                    _initializeRunning = false;
                }
            );
#endif
        }

        private void ResetState()
        {
            _environmentReady = false;
            _savesReady = false;
        }

        private void HandleEnvironmentReady()
        {
            _environmentReady = true;
            Debug.Log("[WebSDK] Environment ready");
            CheckInitializationComplete();
        }

        private void HandleSavesReady()
        {
            _savesReady = true;
            Debug.Log("[WebSDK] Game saves ready");
            CheckInitializationComplete();
        }

        private void CheckInitializationComplete()
        {
            if (_environmentReady && _savesReady)
            {
                FinalizeInitialization();
            }
        }

        private void FinalizeInitialization()
        {
            IsInitialized = true;
            _initializeRunning = false;
            Initialized?.Invoke();
            CleanupEventHandlers();
            
            Debug.Log("[WebSDK] Full initialization complete");
        }

        private void CleanupEventHandlers()
        {
            _webEnvironment.Ready -= HandleEnvironmentReady;
            _webGameSaves.Ready -= HandleSavesReady;
        }

        private void OnDestroy()
        {
            CleanupEventHandlers();
        }
    }
}