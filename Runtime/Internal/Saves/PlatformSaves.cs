using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.GamesData.Web.Requests;
using WelwiseGames.PlayerGameManagement.Unity.Api.Contracts.MetaversesData.Web.Requests;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal abstract class PlatformSaves : ISaves
    {
        protected const string BaseApiUrl = "https://dev.welwise-games.ru/player-game-management";
        
        public event Action Initialized;
        
        protected readonly Dictionary<string, string> _strings = new ();
        protected readonly Dictionary<string, float> _floats = new ();
        protected readonly Dictionary<string, int> _ints = new ();
        protected readonly Dictionary<string, bool> _booleans = new ();
        protected readonly bool _useMetaverse;
        protected readonly string _metaverseId;
        protected readonly string _gameId;
        protected readonly IEnvironment _environment;
        private readonly WebSender _webSender;
        private readonly float _syncDelay;
        private readonly CombinedSync _combinedSync;


        protected string _playerName;
        private float _lastWriteTime = float.NegativeInfinity;

        protected PlatformSaves(
            WebSender webSender, 
            float syncDelay, 
            IEnvironment environment,
            bool useMetaverse,
            string metaverseId,
            string gameId,
            CombinedSync sync)
        {
            _webSender = webSender;
            _syncDelay = syncDelay;
            _environment = environment;
            _useMetaverse = useMetaverse;
            _metaverseId = metaverseId;
            _gameId = gameId;
            _combinedSync = sync;
        }
        
        public void Initialize()
        {
            if (_useMetaverse)
            {
                _combinedSync.SetURL(GetCombinedUrl());
                _combinedSync.InitialSync(InitialSync);
            }
            else
            {
                _webSender.GetRequest(
                    GetUrl(),
                    InitialSync);
            }
        }

        private void InitialSync(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseSaveJson(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError(request.error);
            }
            
            Initialized?.Invoke();
        }
        
        protected abstract void ParseSaveJson(string json);
        private string GetCombinedUrl() => $"{BaseApiUrl}/metaverses/{_metaverseId}/games/{_gameId}/players/{_environment.PlayerId}";
        protected abstract string GetUrl();
        protected abstract string CreateSaveJson();
        protected abstract SaveGameDataRequest CreateSaveGameDataRequest();
        protected abstract SaveMetaverseDataRequest CreateSaveMetaverseDataRequest();

        private void SyncCheck()
        {
            if (_lastWriteTime - Time.time > _syncDelay) return;
            _lastWriteTime = Time.time;

            if (_useMetaverse)
            {
                var saveGameDataRequest = CreateSaveGameDataRequest();
                var saveMetaverseDataRequest = CreateSaveMetaverseDataRequest();
                
                if (saveMetaverseDataRequest != null) _combinedSync.SetMetaverseDataRequest(saveMetaverseDataRequest);
                if (saveGameDataRequest != null) _combinedSync.SetSaveGameDataRequest(saveGameDataRequest);
            }
            else
            {
                _webSender.PutRequest(
                    GetUrl(),
                    CreateSaveJson(),
                    (r) => { });
            }
        }

        public string GetPlayerName() => _playerName;

        public void SetPlayerName(string name)
        {
            _playerName = name;
            SyncCheck();
        }

        public void SetString(string key, string value)
        {
            _strings[key] = value;
            SyncCheck();
        }

        public string GetString(string key, string defaultValue) => _strings.GetValueOrDefault(key, defaultValue);

        public void SetInt(string key, int value)
        {
            _ints[key] = value;
            SyncCheck();
        }

        public int GetInt(string key, int defaultValue) => _ints.GetValueOrDefault(key, defaultValue);

        public void SetFloat(string key, float value)
        {
            _floats[key] = value;
            SyncCheck();
        }

        public float GetFloat(string key, float defaultValue) => _floats.GetValueOrDefault(key, defaultValue);

        public void SetBool(string key, bool value)
        {
            _booleans[key] = value;
            SyncCheck();
        }

        public bool GetBool(string key, bool defaultValue) => _booleans.GetValueOrDefault(key, defaultValue);
    }
}