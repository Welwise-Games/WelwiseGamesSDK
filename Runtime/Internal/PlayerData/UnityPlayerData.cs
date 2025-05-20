using System.Collections.Generic;
using UnityEngine;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal sealed class UnityPlayerData : PlayerData
    {
        private const string PlayerNameKey = "WS_PLAYER_NAME__";
        private const string GamePrefix = "WS_SDK_GAME__";
        private const string MetaversePrefix = "WS_SDK_METAVERSE__";

        public override void Load()
        {
            _playerName = PlayerPrefs.GetString(PlayerNameKey, "Ghost");
            LoadContainerData(_gameDataContainer, GamePrefix);
            LoadContainerData(_metaverseDataContainer, MetaversePrefix);
        }

        public override void Save()
        {
            SaveContainerData(_gameDataContainer, GamePrefix);
            SaveContainerData(_metaverseDataContainer, MetaversePrefix);
            PlayerPrefs.SetString(PlayerNameKey, _playerName);
            PlayerPrefs.Save();
        }
        
        private static void LoadContainerData(DataContainer container, string prefix)
        {
            container.Clear();

            var keysStr = PlayerPrefs.GetString(prefix + "__keys", "");
            if (string.IsNullOrEmpty(keysStr)) 
                return;

            var keys = keysStr.Split(',');
            foreach (var key in keys)
            {
                var prefixedKey = prefix + key;
                if (!PlayerPrefs.HasKey(prefixedKey)) continue;
                var value = PlayerPrefs.GetString(prefixedKey);
                container.DeserializeToContainer(key, value);
            }
        }
        
        private void SaveContainerData(DataContainer container, string prefix)
        {
            if (!container.Changed) return;
            
            var keys = new HashSet<string>();
    
            foreach (var kvp in container.Strings)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataMarshallingUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Ints)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataMarshallingUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Floats)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataMarshallingUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Booleans)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataMarshallingUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
    
            PlayerPrefs.SetString(prefix + "__keys", string.Join(",", keys));
            container.Changed = false;
        }
    }
}