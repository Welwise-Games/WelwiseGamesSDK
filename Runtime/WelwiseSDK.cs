using System;
using System.Collections.Generic;
using UnityEngine;
using WelwiseGamesSDK.Exceptions;
using WelwiseGamesSDK.Internal;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK
{
    public static class WelwiseSDK
    {
        private static bool _isInitialized = false;
        private static ISDK _sdk;
        private static readonly List<INeedInitializeService> Waiters = new ();

        public static event Action Ready;

        public static void Construct()
        {
            var build = new SDKBuilder().Create();
            ConstructWithThirdPartySDK(build);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void ConstructWithThirdPartySDK(ISDK sdk)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SDK is already initialized.");
                return;
            }
            foreach (var service in sdk.NeedInitializeServices)
            {
                AddToWaitQueue(service);
            }
            
            Application.quitting += () =>
            {
                sdk.GameSessionTracker.SessionEnded();
                sdk.GameSessionTracker.Send();
            };
            
            _sdk = sdk;
        }

        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SDK is already initialized.");
                return;
            }
            Debug.Log($"[SDK] Initializing... (need wait {Waiters.Count} services)]");
            foreach (var service in Waiters)
            {
                service.Initialize();
            }
            
            _isInitialized = true;
            _sdk.GameSessionTracker.SessionStarted();
        }

        public static IPlatformNavigation PlatformNavigation
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.PlatformNavigation;
            }
        }

        public static IAdvertisement Advertisement
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.Advertisement;
            }
        }

        public static IAnalytics Analytics
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.Analytics;
            }
        }
        
        public static IEnvironment Environment
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.Environment;
            }
        }

        public static ISaves MetaverseSaves
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.MetaverseSaves;
            }
        }
        public static ISaves GameSaves
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.GameSaves;
            }
        }

        public static ISDKConfig Config
        {
            get
            {
                if (!_isInitialized) throw new SDKNotInitialized();
                return _sdk.Config;
            }
        }

        private static void AddToWaitQueue(INeedInitializeService needInitializeService)
        {
            Waiters.Add(needInitializeService);
            needInitializeService.Initialized += () =>
            {
                Waiters.Remove(needInitializeService);

                if (Waiters.Count != 0) return;
                Debug.Log("[SDK] Ready");
                Ready?.Invoke();
            };
        }
    }
}