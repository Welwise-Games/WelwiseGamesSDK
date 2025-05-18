using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal sealed class WebPlayerData : PlayerData
    {
        #pragma warning disable
        public event Action Ready;
        #pragma warning restore
        
        private readonly IEnvironment _environment;
        private readonly SupportedSDKType _supportedSDK;
        
        private bool _isMetaverseSupported;
        private bool _isLoaded;
        private bool _isSaving;
        
        public WebPlayerData(SDKSettings settings,IEnvironment environment)
        {
            _environment = environment;
            _supportedSDK = settings.SupportedSDKType;
        }

        public override void Load()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.IsMetaverseSupported(supported =>
            {
                _isMetaverseSupported = supported;
                if (supported) LoadCombinedData();
                else LoadPlayerData();
            }, error =>
            {
                _isLoaded = true;
                Debug.LogError(error);
                Ready?.Invoke();
            });
            
#endif
            if (string.IsNullOrEmpty(_playerName) || _playerName.ToLower() == "Гость")
            {
                _playerName = CreateFallbackName();
            }
        }

        private void LoadPlayerData()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GetPlayerData(json =>
            {
                try
                {
                    _gameDataContainer.Clear();
                    switch (_supportedSDK)
                    {
                        case SupportedSDKType.WelwiseGames:
                            var playerData = JsonConvert.DeserializeObject<PlayerData>(json);
                            _playerName = playerData.PlayerName;
                            foreach (var entry in playerData.PlayerGameData)
                            {
                                _gameDataContainer.DeserializeToContainer(entry.Identifier, entry.Value);
                            }

                            break;
                        case SupportedSDKType.YandexGames:
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                            foreach (var kvp in dict)
                            {
                                if (kvp.Key.Equals("playerName", StringComparison.OrdinalIgnoreCase))
                                {
                                    _playerName = kvp.Value?.ToString();
                                    continue;
                                }

                                if (kvp.Value is string valueStr)
                                {
                                    _gameDataContainer.DeserializeToContainer(kvp.Key, valueStr);
                                }
                                else
                                {
                                    Debug.LogWarning($"[{nameof(WebPlayerData)}] Value for key {kvp.Key} is not a string: {kvp.Value}");
                                }
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                Ready?.Invoke();
                _isLoaded = true;
            }, error =>
            {
                _isLoaded = true;
                Debug.LogError(error);
                Ready?.Invoke();
            });
#endif
        }

        private void LoadCombinedData()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GetCombinedPlayerData(json =>
            {
                try
                {
                    var playerData = JsonConvert.DeserializeObject<MetaverseGameData>(json);
                    _playerName = playerData.PlayerName;
                    foreach (var entry in playerData.PlayerGameData)
                    {
                        _gameDataContainer.DeserializeToContainer(entry.Identifier, entry.Value);
                    }
                    foreach (var entry in playerData.PlayerMetaverseData)
                    {
                        _metaverseDataContainer.DeserializeToContainer(entry.Identifier, entry.Value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                Ready?.Invoke();
                _isLoaded = true;
            }, error =>
            {
                Debug.LogError(error);
                Ready?.Invoke();
            });
#endif
        }

        public override void Save()
        {
            if (!_isLoaded) return;
            if (_isSaving) return;

            if (_isMetaverseSupported)
            {
                if (_gameDataContainer.Changed && _metaverseDataContainer.Changed)
                {
                    _gameDataContainer.Changed = false;
                    _metaverseDataContainer.Changed = false;
                    _isSaving = true;
                    var combinedData = new MetaverseGameData
                    {
                        PlayerName = _playerName
                    };

                    foreach (var kvp in _gameDataContainer.Booleans)
                    {
                        combinedData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Ints)
                    {
                        combinedData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Floats)
                    {
                        combinedData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Strings)
                    {
                        combinedData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    // ------
                    foreach (var kvp in _metaverseDataContainer.Booleans)
                    {
                        combinedData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _metaverseDataContainer.Ints)
                    {
                        combinedData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _metaverseDataContainer.Floats)
                    {
                        combinedData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _metaverseDataContainer.Strings)
                    {
                        combinedData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
#if UNITY_WEBGL && !UNITY_EDITOR
                    JsLibProvider.SetCombinedPlayerData(JsonConvert.SerializeObject(combinedData), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                    },
                    error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
#endif
                }
                else if (_gameDataContainer.Changed && !_metaverseDataContainer.Changed)
                {
                    _gameDataContainer.Changed = false;
                    _isSaving = true;
                    var playerData = new PlayerData()
                    {
                        PlayerName = _playerName,
                    };
                    
                    foreach (var kvp in _gameDataContainer.Booleans)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Ints)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Floats)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Strings)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
#if UNITY_WEBGL && !UNITY_EDITOR
                    JsLibProvider.SetPlayerData(JsonConvert.SerializeObject(playerData), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                    },
                    error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
#endif
                }
                else if (!_gameDataContainer.Changed && _metaverseDataContainer.Changed)
                {
                    _metaverseDataContainer.Changed = false;
                    _isSaving = true;

                    var metaverseData = new MetaversePlayerData()
                    {
                        PlayerName = _playerName
                    };
                    
                    foreach (var kvp in _gameDataContainer.Booleans)
                    {
                        metaverseData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Ints)
                    {
                        metaverseData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Floats)
                    {
                        metaverseData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Strings)
                    {
                        metaverseData.PlayerMetaverseData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
#if UNITY_WEBGL && !UNITY_EDITOR
                    JsLibProvider.SetMetaversePlayerData(JsonConvert.SerializeObject(metaverseData), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                    }, error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
#endif
                }
            }
            else
            {
                if (_gameDataContainer.Changed)
                {
                    _gameDataContainer.Changed = false;
                    _isSaving = true;
                    var playerData = new PlayerData()
                    {
                        PlayerName = _playerName,
                    };
                    
                    foreach (var kvp in _gameDataContainer.Booleans)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Ints)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Floats)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
                    
                    foreach (var kvp in _gameDataContainer.Strings)
                    {
                        playerData.PlayerGameData.Add(new DataEntry()
                        {
                            Identifier = kvp.Key, 
                            Value = DataMarshallingUtil.Serialize(kvp.Value)
                        });
                    }
#if UNITY_WEBGL && !UNITY_EDITOR
                    JsLibProvider.SetPlayerData(JsonConvert.SerializeObject(playerData), () =>
                        {
                            _isSaving = false;
                            if (_gameDataContainer.Changed) Save();
                        },
                        error =>
                        {
                            Debug.LogError(error);
                            _isSaving = false;
                        });
#endif
                }
            }
        }
        
        private string CreateFallbackName()
        {
            var bytes = _environment.PlayerId.ToByteArray();
            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;

            var hash = offsetBasis;
            foreach (var b in bytes)
            {
                hash ^= b;
                hash *= prime;
            }
            var id = (int)(hash % 1000);
            return _environment.LanguageCode switch
            {
                "ru" => $"Призрак_{id}",
                _ => $"Ghost_{id}"
            };
        }
        
        
        public class DataEntry
        {
            [JsonProperty("identifier", NullValueHandling = NullValueHandling.Ignore)]
            public string Identifier { get; set; }

            [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
            public string Value { get; set; }

            [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Values { get; set; }
        }

        public class PlayerData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerGameData", NullValueHandling = NullValueHandling.Ignore)]
            public List<DataEntry> PlayerGameData { get; set; }
        }

        public class MetaversePlayerData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerMetaverseData", NullValueHandling = NullValueHandling.Ignore)]
            public List<DataEntry> PlayerMetaverseData { get; set; }
        }

        public class MetaverseGameData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerMetaverseData", NullValueHandling = NullValueHandling.Ignore)]
            public List<DataEntry> PlayerMetaverseData { get; set; }

            [JsonProperty("playerGameData", NullValueHandling = NullValueHandling.Ignore)]
            public List<DataEntry> PlayerGameData { get; set; }
        }
    }
    
    
    
}