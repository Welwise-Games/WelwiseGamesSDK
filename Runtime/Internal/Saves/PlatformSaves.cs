using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal abstract class PlatformSaves : ISaves
    {
        public event Action Ready;
        
        protected readonly Dictionary<string, string> _strings = new ();
        protected readonly Dictionary<string, float> _floats = new ();
        protected readonly Dictionary<string, int> _ints = new ();
        protected readonly Dictionary<string, bool> _booleans = new ();
        private readonly WebSender _webSender;
        
        private float _lastWriteTime = float.NegativeInfinity;

        protected PlatformSaves(WebSender webSender)
        {
            _webSender = webSender;

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
            
            Ready?.Invoke();
        }
        
        protected abstract void ParseSaveJson(string json);
        protected abstract string GetUrl();
        protected abstract string CreateSaveJson();

        protected void SyncCheck()
        {
            if (_lastWriteTime - Time.time > WelwiseSDK.Settings.SyncDelay) return;
            _lastWriteTime = Time.time;
            
            _webSender.PutRequest(
                GetUrl(),
                CreateSaveJson(),
                (r) => { });
        }

        public abstract string GetPlayerName();
        public abstract void SetPlayerName(string name);

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