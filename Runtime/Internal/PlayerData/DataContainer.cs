using System;
using System.Collections.Generic;
using UnityEngine;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal sealed class DataContainer : IData
    {
        public bool IsAvailable { get; }
        public bool Changed;
        
        public readonly Dictionary<string, float> Floats = new();
        public readonly Dictionary<string, string> Strings = new();
        public readonly Dictionary<string, int> Ints = new();
        public readonly Dictionary<string, bool> Booleans = new();
        private readonly Func<string, bool> _setValidator;

        public DataContainer(Func<string, bool> setValidator, bool isAvailable)
        {
            _setValidator = setValidator;
            IsAvailable = isAvailable;
        }

        public void Clear()
        {
            Floats.Clear();
            Strings.Clear();
            Ints.Clear();
            Booleans.Clear();
        }

        public void SetString(string key, string value) =>
            SetData(Strings, key, value);

        public void SetInt(string key, int value) =>
            SetData(Ints, key, value);

        public void SetFloat(string key, float value) =>
            SetData(Floats, key, value);

        public void SetBool(string key, bool value) => 
            SetData(Booleans, key, value);
        
        public string GetString(string key, string defaultValue) => 
            Strings.GetValueOrDefault(key, defaultValue);
        
        public int GetInt(string key, int defaultValue) => 
            Ints.GetValueOrDefault(key, defaultValue);
        
        public float GetFloat(string key, float defaultValue) => 
            Floats.GetValueOrDefault(key, defaultValue);
        
        public bool GetBool(string key, bool defaultValue) => 
            Booleans.GetValueOrDefault(key, defaultValue);
        
        public bool HasKey(string key) => 
            Strings.ContainsKey(key) || Ints.ContainsKey(key) || Booleans.ContainsKey(key) || Floats.ContainsKey(key);

        private void SetData<T>(Dictionary<string, T> dictionary, string key, T value)
        {
            if (!_setValidator.Invoke(key))
            {
                Debug.LogWarning($"Validation set data error: key={key}, value={value} not set");
                return;
            }
            
            var previous = dictionary.GetValueOrDefault(key);
            dictionary[key] = value;
            Changed = !EqualityComparer<T>.Default.Equals(previous, value);
        }
    }
}