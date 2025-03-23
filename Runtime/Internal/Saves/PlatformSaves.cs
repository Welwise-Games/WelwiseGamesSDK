using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
        protected readonly IEnvironment _environment;
        private readonly WebSender _webSender;
        private readonly float _syncDelay;

        protected string _playerName;
        private float _lastWriteTime = float.NegativeInfinity;

        protected PlatformSaves(WebSender webSender, float syncDelay, IEnvironment environment)
        {
            _webSender = webSender;
            _syncDelay = syncDelay;
            _environment = environment;
        }
        
        public void Initialize()
        {
            _webSender.GetRequest(
                GetUrl(), 
                InitialSync);
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
        protected abstract string GetUrl();
        protected abstract string CreateSaveJson();

        protected void SyncCheck()
        {
            if (_lastWriteTime - Time.time > _syncDelay) return;
            _lastWriteTime = Time.time;
            
            _webSender.PutRequest(
                GetUrl(),
                CreateSaveJson(),
                (r) => { });
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