using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal sealed class UnityPlayerData : PlayerData
    {
        private const string PlayerNameKey = "WS_PLAYER_NAME__";
        private const string GamePrefix = "WS_SDK_GAME__";
        private const string MetaversePrefix = "WS_SDK_METAVERSE__";

        public UnityPlayerData(bool isAvailableSelf, bool isGameDataAvailable, bool isMetaverseDataAvailable) :
            base(isAvailableSelf, isGameDataAvailable, isMetaverseDataAvailable) {}

        public override void Initialize()
        {
            _playerName = PlayerPrefs.GetString(PlayerNameKey, "Ghost");
            LoadContainerData(_gameDataContainer, GamePrefix);
            LoadContainerData(_metaverseDataContainer, MetaversePrefix);
            OnLoaded();
        }

        public override void Save()
        {
            SaveContainerData(_gameDataContainer, GamePrefix);
            SaveContainerData(_metaverseDataContainer, MetaversePrefix);
            PlayerPrefs.SetString(PlayerNameKey, _playerName);
            PlayerPrefs.Save();
            OnSaved();
        }

        private static void LoadContainerData(DataContainer container, string prefix)
        {
            container.Clear();

            var keysJson = PlayerPrefs.GetString(prefix + "__keys", "");
            if (string.IsNullOrEmpty(keysJson))
                return;

            List<string> keys;
            try
            {
                keys = JsonConvert.DeserializeObject<List<string>>(keysJson);
            }
            catch
            {

                keys = new List<string>(keysJson.Split(','));
            }

            if (keys == null) return;

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
                PlayerPrefs.SetString(prefix + kvp.Key, DataConvertUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Ints)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataConvertUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Floats)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataConvertUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }
            foreach (var kvp in container.Booleans)
            {
                PlayerPrefs.SetString(prefix + kvp.Key, DataConvertUtil.Serialize(kvp.Value));
                keys.Add(kvp.Key);
            }

            PlayerPrefs.SetString(prefix + "__keys", JsonConvert.SerializeObject(new List<string>(keys)));
            container.Changed = false;
        }
    }
}