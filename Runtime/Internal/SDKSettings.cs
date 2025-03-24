using UnityEngine;
using WelwiseGamesSDK.Shared;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WelwiseGamesSDK.Internal
{
    public sealed class SDKSettings : ScriptableObject, ISDKConfig
    {
        public SDKMode Mode => _mode;
        
        public string DebugPlayerId => _playerId;
        public string GameId => _gameId;
        public float SyncDelay => _syncDelay;
        public string ApiAuthKey => _apiAuthKey;
        public string MetaverseId => _metaverseId;
        public bool UseMetaverse => _useMetaverse;
        internal SupportedSDKType SupportedSDKType => _supportedSDKType;
        
        [SerializeField] private SDKMode _mode;
        [SerializeField] private SupportedSDKType _supportedSDKType;
        [SerializeField] private string _gameId;
        [SerializeField] private string _playerId;
        [SerializeField] private float _syncDelay = 35;
        [SerializeField] private string _apiAuthKey;
        [SerializeField] private bool _useMetaverse;
        [SerializeField] private string _metaverseId;
        
        
        public static SDKSettings LoadOrCreateSettings()
        {
            var allSettings = Resources.LoadAll<SDKSettings>("");
            if (allSettings.Length > 0)
            {
                var settings = allSettings[0];
        
                if (allSettings.Length > 1)
                {
                    Debug.LogWarning($"Found {allSettings.Length} SDKSettings assets! Using first one: {settings.name}");
                }
        
                if (settings.name != nameof(SDKSettings))
                {
                    Debug.LogWarning($"SDK Settings loaded from asset '{settings.name}', " +
                                     "but recommended name is 'SDKSettings'");
                }
                return settings;
            }


#if UNITY_EDITOR
            return CreateSettingsAsset();
#else
            Debug.LogError("SDKSettings asset is missing! Please create it in Resources folder.");
#endif
        }

#if UNITY_EDITOR
        private static SDKSettings CreateSettingsAsset()
        {
            var settings = ScriptableObject.CreateInstance<SDKSettings>();
            
            var resourcePath = "Assets/Resources";
            EnsureResourcesFolderExists(resourcePath);
            
            var fullPath = $"{resourcePath}/{nameof(SDKSettings)}.asset";
            
            AssetDatabase.CreateAsset(settings, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.LogWarning($"SDKSettings was auto-created at {fullPath}. Please configure it!");
            return settings;
        }

        private static void EnsureResourcesFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
        }
#endif
    }
}