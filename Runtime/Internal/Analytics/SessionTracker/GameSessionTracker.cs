using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics.SessionTracker
{
    internal sealed class GameSessionTracker : IGameSessionTracker
    {
        private const string ApiUrl = "https://dev.welwise-games.ru/player-game-analytics";
        private const string CookieKey = "WS_PLATFORM_ID";
        
        private readonly WebSender _webSender;
        private readonly SDKSettings _sdkSettings;
        private readonly IEnvironment _environment;
        
        private bool _started;
        private bool _ended;
        private string _startTime;
        private string _endTime;
        private string _gameSessionId;
        private string _platformSessionId;
        
        public GameSessionTracker(SDKSettings settings, WebSender webSender, IEnvironment environment)
        {
            _webSender = webSender;
            _sdkSettings = settings;
            _environment = environment;
        }

        public void SessionStarted()
        {
            if (_started)
            {
                Debug.LogWarning($"[{nameof(GameSessionTracker)}] Session already started return.");
                return;
            }
            
            _startTime = DateTime.UtcNow.ToString("O");
            _gameSessionId = Guid.NewGuid().ToString();

            var pId = CookieHandler.LoadData(CookieKey);
            if (string.IsNullOrEmpty(pId))
            {
                Debug.LogWarning($"[{nameof(GameSessionTracker)}] Session cookie not found create new one.");
                _platformSessionId = Guid.NewGuid().ToString();
                CookieHandler.SaveData(CookieKey, _platformSessionId, 1);
            }
            else
            {
                _platformSessionId = pId;
            }
            
        }

        public void SessionEnded()
        {
            if (_ended)
            {
                Debug.LogWarning($"[{nameof(GameSessionTracker)}] Session already ended return.");
                return;
            }
            _endTime = DateTime.UtcNow.ToString("O");
            _ended = true;
        }

        public void Send()
        {
            _webSender.PostRequest(
                $"{ApiUrl}/player-activity",
                $@"
                    {{
                        ""playerId"": ""{_environment.PlayerId}"",
                        ""platformSessionId"": ""{_platformSessionId}"",
                        ""gameSessionId"": ""{_gameSessionId}"",
                        ""loginTime"": ""{_startTime}"",
                        ""logoutTime"": ""{_endTime}"",
                        ""gameId"": {_sdkSettings.GameId}
                    }}",
                    _ => {Debug.Log($@"SEND
                    {{
                        ""playerId"": ""{_environment.PlayerId}"",
                        ""platformSessionId"": ""{_platformSessionId}"",
                        ""gameSessionId"": ""{_gameSessionId}"",
                        ""loginTime"": ""{_startTime}"",
                        ""logoutTime"": ""{_endTime}"",
                        ""gameId"": {_sdkSettings.GameId}
                    }}");}
                );
        }
    }
}