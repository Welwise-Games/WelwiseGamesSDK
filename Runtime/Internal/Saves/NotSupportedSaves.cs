using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Saves
{
    internal sealed class NotSupportedSaves : ISaves
    {
        public void Initialize()
        {
            Initialized?.Invoke();
        }

        public event Action Initialized;
        
        public string GetPlayerName()
        {
            Debug.LogError("Not supported saves");
            return null;
        }

        public void SetPlayerName(string name)
        {
            Debug.LogError("Not supported saves");
        }

        public void SetString(string key, string value)
        {
            Debug.LogError("Not supported saves");
        }

        public string GetString(string key, string defaultValue)
        {
            Debug.LogError("Not supported saves");
            return defaultValue;
        }

        public void SetInt(string key, int value)
        {
            Debug.LogError("Not supported saves");
        }

        public int GetInt(string key, int defaultValue)
        {
            Debug.LogError("Not supported saves");
            return defaultValue;
        }

        public void SetFloat(string key, float value)
        {
            Debug.LogError("Not supported saves");
        }

        public float GetFloat(string key, float defaultValue)
        {
            Debug.LogError("Not supported saves");
            return defaultValue;
        }

        public void SetBool(string key, bool value)
        {
            Debug.LogError("Not supported saves");
        }

        public bool GetBool(string key, bool defaultValue)
        {
            Debug.LogError("Not supported saves");
            return defaultValue;
        }
    }
}