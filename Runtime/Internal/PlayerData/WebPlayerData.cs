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
                    var playerData = JsonConvert.DeserializeObject<UniversalPlayerData>(json);
                    _playerName = playerData.PlayerName;
                    
                    foreach (var entry in playerData.PlayerGameData)
                    {
                        _gameDataContainer.DeserializeToContainer(entry.Identifier, entry.Value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                
                // Fallback для имени игрока
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
                    var combinedData = JsonConvert.DeserializeObject<UniversalCombinedData>(json);
                    _playerName = combinedData.PlayerName;
                    
                    foreach (var entry in combinedData.PlayerGameData)
                    {
                        _gameDataContainer.DeserializeToContainer(entry.Identifier, entry.Value);
                    }
                    
                    foreach (var entry in combinedData.PlayerMetaverseData)
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

        private string SerializePlayerData()
        {
            _gameDataContainer.Changed = false;
            _isSaving = true;
            
            var playerData = new UniversalPlayerData
            {
                PlayerName = _playerName,
                PlayerGameData = new List<UniversalDataEntry>()
            };
            
            AddContainerDataToEntries(_gameDataContainer, playerData.PlayerGameData);
            
            return JsonConvert.SerializeObject(playerData);
        }

        private string SerializeMetaverseData()
        {
            _metaverseDataContainer.Changed = false;
            _isSaving = true;

            var metaverseData = new UniversalMetaverseData
            {
                PlayerName = _playerName,
                PlayerMetaverseData = new List<UniversalDataEntry>()
            };
            
            AddContainerDataToEntries(_metaverseDataContainer, metaverseData.PlayerMetaverseData);
            
            return JsonConvert.SerializeObject(metaverseData);
        }

        private string SerializeCombinedData()
        {
            _gameDataContainer.Changed = false;
            _metaverseDataContainer.Changed = false;
            _isSaving = true;
            
            var combinedData = new UniversalCombinedData
            {
                PlayerName = _playerName,
                PlayerGameData = new List<UniversalDataEntry>(),
                PlayerMetaverseData = new List<UniversalDataEntry>()
            };
            
            AddContainerDataToEntries(_gameDataContainer, combinedData.PlayerGameData);
            AddContainerDataToEntries(_metaverseDataContainer, combinedData.PlayerMetaverseData);
            
            return JsonConvert.SerializeObject(combinedData);
        }
        
        private void AddContainerDataToEntries(DataContainer container, List<UniversalDataEntry> entries)
        {
            foreach (var kvp in container.Booleans)
            {
                entries.Add(new UniversalDataEntry
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
            
            foreach (var kvp in container.Ints)
            {
                entries.Add(new UniversalDataEntry
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
            
            foreach (var kvp in container.Floats)
            {
                entries.Add(new UniversalDataEntry
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
            
            foreach (var kvp in container.Strings)
            {
                entries.Add(new UniversalDataEntry
                {
                    Identifier = kvp.Key, 
                    Value = DataConvertUtil.Serialize(kvp.Value)
                });
            }
        }

        public override void Save()
        {
            if (!IsInitialized) return;
            if (_isSaving) return;

            if (_isMetaverseSupported)
            {
                if (_gameDataContainer.Changed && _metaverseDataContainer.Changed || _previousPlayerName != _playerName)
                {
                    PluginRuntime.SetCombinedPlayerData(SerializeCombinedData(), () =>
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
                    PluginRuntime.SetPlayerData(SerializePlayerData(), () =>
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
                    PluginRuntime.SetMetaversePlayerData(SerializeMetaverseData(), () =>
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
                    PluginRuntime.SetPlayerData(SerializePlayerData(), () =>
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
        
        // Универсальные классы для данных
        public class UniversalDataEntry
        {
            [JsonProperty("identifier", NullValueHandling = NullValueHandling.Ignore)]
            public string Identifier { get; set; }

            [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
            public string Value { get; set; }

            [JsonProperty("values", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Values { get; set; }
        }

        public class UniversalPlayerData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerGameData", NullValueHandling = NullValueHandling.Ignore)]
            public List<UniversalDataEntry> PlayerGameData { get; set; }
        }

        public class UniversalMetaverseData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerMetaverseData", NullValueHandling = NullValueHandling.Ignore)]
            public List<UniversalDataEntry> PlayerMetaverseData { get; set; }
        }

        public class UniversalCombinedData
        {
            [JsonProperty("playerName", NullValueHandling = NullValueHandling.Ignore)]
            public string PlayerName { get; set; }

            [JsonProperty("playerMetaverseData", NullValueHandling = NullValueHandling.Ignore)]
            public List<UniversalDataEntry> PlayerMetaverseData { get; set; }

            [JsonProperty("playerGameData", NullValueHandling = NullValueHandling.Ignore)]
            public List<UniversalDataEntry> PlayerGameData { get; set; }
        }
    }
}