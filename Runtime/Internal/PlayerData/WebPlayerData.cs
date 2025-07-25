﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal sealed class WebPlayerData : PlayerData
    {
        private readonly IEnvironment _environment;
        private readonly SupportedSDKType _supportedSDK;
        
        private bool _isMetaverseSupported;
        private bool _isSaving;
        
        public WebPlayerData(SDKSettings settings, IEnvironment environment, 
            bool isAvailableSelf, bool isGameDataAvailable, bool isMetaverseDataAvailable) 
            : base(isAvailableSelf, isGameDataAvailable, isMetaverseDataAvailable)
        {
            _environment = environment;
            _supportedSDK = settings.SDKType;
        }

        public override void Initialize()
        {
            PluginRuntime.IsMetaverseSupported(supported =>
            {
                Debug.Log("[WebPlayerData] Get Supported Saves");
                _isMetaverseSupported = supported;
                if (supported) LoadCombinedData();
                else LoadPlayerData();
            }, error =>
            {
                OnLoaded();
                Debug.LogError(error);
            });
        }

        private void LoadPlayerData()
        {
            PluginRuntime.GetPlayerData(json =>
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
                if (string.IsNullOrEmpty(_playerName) || _playerName.ToLower() == "гость")
                {
                    _playerName = CreateFallbackName();
                    _previousPlayerName = _playerName;
                }
                OnLoaded();
            }, error =>
            {
                OnLoaded();
                Debug.LogError(error);
            });
        }

        private void LoadCombinedData()
        {
            PluginRuntime.GetCombinedPlayerData(json =>
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
                if (string.IsNullOrEmpty(_playerName) || _playerName.ToLower() == "гость")
                {
                    _playerName = CreateFallbackName();
                    _previousPlayerName = _playerName;
                }
                OnLoaded();
            }, error =>
            {
                OnLoaded();
                Debug.LogError(error);
            });
        }

        private string SaveCombinedData()
        {
             _gameDataContainer.Changed = false;
             _metaverseDataContainer.Changed = false;
             _isSaving = true;
             var combinedData = new MetaverseGameData
             {
                 PlayerName = _playerName,
                 PlayerGameData = new List<DataEntry>(),
                 PlayerMetaverseData = new List<DataEntry>(),
             };
             foreach (var kvp in _gameDataContainer.Booleans)
             {
                 combinedData.PlayerGameData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _gameDataContainer.Ints)
             {
                 combinedData.PlayerGameData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _gameDataContainer.Floats)
             {
                 combinedData.PlayerGameData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _gameDataContainer.Strings)
             {
                 combinedData.PlayerGameData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             // ------
             foreach (var kvp in _metaverseDataContainer.Booleans)
             {
                 combinedData.PlayerMetaverseData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _metaverseDataContainer.Ints)
             {
                 combinedData.PlayerMetaverseData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _metaverseDataContainer.Floats)
             {
                 combinedData.PlayerMetaverseData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             
             foreach (var kvp in _metaverseDataContainer.Strings)
             {
                 combinedData.PlayerMetaverseData.Add(new DataEntry()
                 {
                     Identifier = kvp.Key, 
                     Value = DataConvertUtil.Serialize(kvp.Value)
                 });
             }
             return JsonConvert.SerializeObject(combinedData);
        }

        private string SavePlayerDataYandex()
        {
            _gameDataContainer.Changed = false;
            _isSaving = true;
            
            Dictionary<string, string> playerData = new();
            
            foreach (var kvp in _gameDataContainer.Booleans)
            {
                playerData[kvp.Key] = DataConvertUtil.Serialize(kvp.Value);
            }
            
            foreach (var kvp in _gameDataContainer.Strings)
            {
                playerData[kvp.Key] = DataConvertUtil.Serialize(kvp.Value);
            }
            
            foreach (var kvp in _gameDataContainer.Ints)
            {
                playerData[kvp.Key] = DataConvertUtil.Serialize(kvp.Value);
            }
            
            foreach (var kvp in _gameDataContainer.Floats)
            {
                playerData[kvp.Key] = DataConvertUtil.Serialize(kvp.Value);
            }
            return JsonConvert.SerializeObject(playerData);
        }

        private string SavePlayerDataWelwise()
        { 
            _gameDataContainer.Changed = false;
            _isSaving = true;
            var playerData = new PlayerData()
            {
                PlayerName = _playerName,
                PlayerGameData = new List<DataEntry>(),
            };
                    
            foreach (var kvp in _gameDataContainer.Booleans)
            {
                playerData.PlayerGameData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _gameDataContainer.Ints)
            {
                playerData.PlayerGameData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _gameDataContainer.Floats)
            {
                playerData.PlayerGameData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _gameDataContainer.Strings)
            {
                playerData.PlayerGameData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
            
            return JsonConvert.SerializeObject(playerData);
        }

        private string SaveMetaverseData()
        {
            _metaverseDataContainer.Changed = false;
            _isSaving = true;

            var metaverseData = new MetaversePlayerData()
            {
                PlayerName = _playerName,
                PlayerMetaverseData = new List<DataEntry>(),
            };
                    
            foreach (var kvp in _metaverseDataContainer.Booleans)
            {
                metaverseData.PlayerMetaverseData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _metaverseDataContainer.Ints)
            {
                metaverseData.PlayerMetaverseData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _metaverseDataContainer.Floats)
            {
                metaverseData.PlayerMetaverseData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
                    
            foreach (var kvp in _metaverseDataContainer.Strings)
            {
                metaverseData.PlayerMetaverseData.Add(new DataEntry()
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
            return JsonConvert.SerializeObject(metaverseData);
        }

        public override void Save()
        {
            if (!IsInitialized) return;
            if (_isSaving) return;

            if (_isMetaverseSupported)
            {
                if (_gameDataContainer.Changed && _metaverseDataContainer.Changed || _previousPlayerName != _playerName)
                {
                    PluginRuntime.SetCombinedPlayerData(SaveCombinedData(), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                        else OnSaved();
                        _previousPlayerName = _playerName;
                    },
                    error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
                }
                else if (_gameDataContainer.Changed && !_metaverseDataContainer.Changed || _previousPlayerName != _playerName)
                {
                    PluginRuntime.SetPlayerData(SavePlayerDataWelwise(), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                        else OnSaved();
                    },
                    error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
                }
                else if (!_gameDataContainer.Changed && _metaverseDataContainer.Changed || _previousPlayerName != _playerName)
                {
                    PluginRuntime.SetMetaversePlayerData(SaveMetaverseData(), () =>
                    {
                        _isSaving = false;
                        if (_gameDataContainer.Changed || _metaverseDataContainer.Changed) Save();
                        else OnSaved();
                    }, error =>
                    {
                        Debug.LogError(error);
                        _isSaving = false;
                    });
                }
                else OnSaved();
            }
            else
            {
                if (_gameDataContainer.Changed || _previousPlayerName != _playerName)
                {
                    var data = _supportedSDK switch
                    {
                        SupportedSDKType.WelwiseGames => SavePlayerDataWelwise(),
                        SupportedSDKType.YandexGames => SavePlayerDataYandex(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    PluginRuntime.SetPlayerData(data, () =>
                        {
                            _isSaving = false;
                            if (_gameDataContainer.Changed) Save();
                            else OnSaved();
                        },
                        error =>
                        {
                            Debug.LogError(error);
                            _isSaving = false;
                        });
                }
                else OnSaved();
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
            return _environment.Language switch
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