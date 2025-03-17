using System;
using System.Collections.Generic;
using UnityEngine;
using WelwiseGamesSDK.Internal;
using WelwiseGamesSDK.Internal.Environment;
using WelwiseGamesSDK.Internal.Saves;

namespace WelwiseGamesSDK
{
    public static class WelwiseSDK
    {
        internal const string IdKey = "WS_PLAYER_ID";
        internal const string BaseUrl = "https://dev.welwise-games.ru/player-game-management";

        public static event Action Ready;
        
        public static IEnvironment Environment
        {
            get
            {
                LazyCheck();
                return _environment;
            }
        }

        public static ISaves MetaverseSaves
        {
            get
            {
                LazyCheck();
                return _metaverseSaves;
            }
        }
        public static ISaves GameSaves
        {
            get
            {
                LazyCheck();
                return _gameSaves;
            }
        }

        internal static WebSender WebSender
        {
            get
            {
                InitializeSettings();
                InitializeWebSender();
                return _webSender;
            }
        }

        internal static SDKSettings Settings
        {
            get
            {
                InitializeSettings();
                return _settings;
            }
        }

        private static ISaves _gameSaves;
        private static ISaves _metaverseSaves;
        private static WebSender _webSender;
        private static SDKSettings _settings;
        private static IEnvironment _environment;
        private static readonly List<IWaitService> Waiters = new ();

        internal static IEnvironment GetEnvironment()
        {
            InitializeSettings();
            InitializeEnvironment();
            return _environment;
        }

        private static void LazyCheck()
        {
            InitializeSettings();
            InitializeEnvironment();
            InitializeWebSender();
            InitializeGameSaves();
            InitializeMetaverseSaves();
        }

        private static void InitializeEnvironment()
        {
            if (_environment != null) return;
            _environment = EnvironmentFactory.Create(_settings.UseDebugID, _settings.PlayerId, _settings.UseMetaverse);
        }

        private static void InitializeWebSender()
        {
            if (_webSender != null) return;
            var go = new GameObject($"[ {nameof(WelwiseSDK)} ] -- {nameof(WebSender)}");
            _webSender = go.AddComponent<WebSender>();
            _webSender.Initialize(_settings.ApiKey);
        }

        private static void InitializeSettings()
        {
            if (_settings != null) return;
            _settings = SDKSettings.LoadOrCreateSettings();
        }

        private static void InitializeGameSaves()
        {
            if (_gameSaves != null) return;
            _gameSaves = SavesFactory.CreateGameSaves(_webSender);
            AddToWaitQueue(_gameSaves);
        }

        private static void InitializeMetaverseSaves()
        {
            if (_metaverseSaves != null) return;
            _metaverseSaves = SavesFactory.CreateMetaverseGameSaves(_webSender);
            AddToWaitQueue(_metaverseSaves);
        }

        private static void AddToWaitQueue(IWaitService waitService)
        {
            Waiters.Add(waitService);
            waitService.Ready += () =>
            {
                Waiters.Remove(waitService);

                if (Waiters.Count == 0)
                {
                    Ready?.Invoke();
                }
            };
        }
    }
}