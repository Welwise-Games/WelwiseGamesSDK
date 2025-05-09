using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.GameSaves
{
    internal sealed class UnityGameSaves : ISaves
    {
        private const string PlayerNameKey = "PLAYER-NAME";
        public string GetPlayerName(string defaultValue) => PlayerPrefs.GetString(PlayerNameKey, defaultValue);
        public void SetPlayerName(string name) => PlayerPrefs.SetString(PlayerNameKey, name);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(key, defaultValue);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);
        public void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public bool GetBool(string key, bool defaultValue) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        public void Save() => PlayerPrefs.Save();
    }
}