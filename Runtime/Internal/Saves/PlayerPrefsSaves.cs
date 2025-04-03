using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal sealed class PlayerPrefsSaves : ISaves, INeedInitializeService
    {
        public event Action Initialized;
        private const string PlayerNameKey = "PlayerName";

        private readonly bool _isMetaverse;
        private readonly MonoBehaviour _coroutineRunner;

        public PlayerPrefsSaves(MonoBehaviour coroutineRunner, bool isMetaverse)
        {
            _isMetaverse = isMetaverse;
            _coroutineRunner = coroutineRunner;
        }
        
        public void Initialize()
        {
            _coroutineRunner.StartCoroutine(ReadyRoutine());
        }

        private IEnumerator ReadyRoutine()
        {
            yield return new WaitForSeconds(1);
            Initialized?.Invoke();
        }

        public string GetPlayerName() => PlayerPrefs.GetString(PlayerNameKey, string.Empty);

        public void SetPlayerName(string name)
        {
            PlayerPrefs.SetString(FormatKey(PlayerNameKey), name);
            PlayerPrefs.Save();
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(FormatKey(key), value);
            PlayerPrefs.Save();
        }

        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(FormatKey(key), defaultValue);

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(FormatKey(key), value);
            PlayerPrefs.Save();
        }

        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(FormatKey(key), defaultValue);

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(FormatKey(key), value);
            PlayerPrefs.Save();
        }

        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(FormatKey(key), defaultValue);

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(FormatKey(key), value ? 1 : 0);
            PlayerPrefs.Save();
        }

        public bool GetBool(string key, bool defaultValue) => PlayerPrefs.GetInt(FormatKey(key), defaultValue ? 1 : 0) == 1;

        private string FormatKey(string key) => _isMetaverse ? $"Metaverse_{key}" : key;
    }
}