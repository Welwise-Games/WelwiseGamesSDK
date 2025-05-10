using System;
using System.Collections.Generic;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal sealed class WebGameSaves : ISaves
    {
        public event Action Ready;
        
        private readonly DataContainer _dataContainer = new ();
        private readonly ISaveParser _saveParser;
        
        private bool _isSaving;

        public WebGameSaves(SDKSettings sdkSettings)
        {
            _saveParser = sdkSettings.SupportedSDKType switch
            {
                SupportedSDKType.WelwiseGames => new WelwiseGamesSaveParser(),
                SupportedSDKType.YandexGames => new YandexGamesSaveParser(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Load()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                JsLibProvider.JSGetPlayerData(
                    (v) =>
                    {
                        _saveParser.DeserializeJsonToContainer(v, _dataContainer);
                        Ready?.Invoke();
                    }, 
                    (e) =>
                    {
                        Debug.LogError(e);
                        Ready?.Invoke();
                    });
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Ready?.Invoke();
            }
        }

        public string GetPlayerName(string defaultValue) =>
            string.IsNullOrEmpty(_dataContainer.PlayerName) ? defaultValue : _dataContainer.PlayerName;
        public void SetPlayerName(string name) => _dataContainer.PlayerName = name;
        public void SetString(string key, string value) => _dataContainer.Strings[key] = value;
        public string GetString(string key, string defaultValue) => _dataContainer.Strings.GetValueOrDefault(key, defaultValue);
        public void SetInt(string key, int value) => _dataContainer.Ints[key] = value;
        public int GetInt(string key, int defaultValue) => _dataContainer.Ints.GetValueOrDefault(key, defaultValue);
        public void SetFloat(string key, float value) => _dataContainer.Floats[key] = value;
        public float GetFloat(string key, float defaultValue) => _dataContainer.Floats.GetValueOrDefault(key, defaultValue);
        public void SetBool(string key, bool value) => _dataContainer.Booleans[key] = value;
        public bool GetBool(string key, bool defaultValue) => _dataContainer.Booleans.GetValueOrDefault(key, defaultValue);

        public void Save()
        {
            if (_isSaving) return;

            var json = _saveParser.SerializeContainerToJson(_dataContainer);

            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                Debug.LogWarning($"[{nameof(WebGameSaves)}] No data to save");
                return;
            }
            
            _isSaving = true;
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.JSSetPlayerData(json, () =>
            {
                _isSaving = false;
            },
            (e) =>
            {
                _isSaving = false;
                Debug.LogError(e);
            });
#endif
        }
    }
}