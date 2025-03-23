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
            foreach (var service in build.NeedInitializeServices)
            {
                AddToWaitQueue(service);
            }
            _sdk = build;
        }

        public static void ConstructWithThirdPartySDK(ISDK sdk)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SDK is already initialized.");
                return;
            }
            
            _sdk = sdk;
        }

        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SDK is already initialized.");
                return;
            }

            foreach (var service in Waiters)
            {
                Initialize();
            }
            
            _isInitialized = true;
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

        private static void AddToWaitQueue(INeedInitializeService needInitializeService)
        {
            Waiters.Add(needInitializeService);
            needInitializeService.Initialized += () =>
            {
                Waiters.Remove(needInitializeService);

                if (Waiters.Count == 0)
                {
                    Ready?.Invoke();
                }
            };
        }
    }
}