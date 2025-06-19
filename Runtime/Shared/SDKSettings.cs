using UnityEngine;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WelwiseGamesSDK.Shared
{
    public sealed class SDKSettings : ScriptableObject
    {
        public enum AspectRatioMode
        {
            Default,
            // ReSharper disable once InconsistentNaming
            Aspect16_9,
            // ReSharper disable once InconsistentNaming
            Aspect9_16
        }
        
        internal string DebugPlayerId => _playerId;
        public SupportedSDKType SupportedSDKType => _supportedSDKType;
        internal DeviceType DebugDeviceType => _debugDeviceType;
        internal string DebugLanguageCode => _debugLanguageCode;
        internal bool MuteAudioOnPause => _muteAudioOnPause;
        internal bool AutoConstructAndInitializeSingleton => _autoConstructAndInitializeSingleton;
        internal float DebugInitializeTime => _debugInitializeTime;
        internal bool LoadSaveOnInitialize => _loadSaveOnInitialize;
        public string InstalledPackageVersion
        {
            get => _installedPackageVersion;
            set => _installedPackageVersion = value;
        }
        
        public AspectRatioMode AspectRatio => _aspectRatio;
        public Texture2D BackgroundImage => _backgroundImage;
        public float AdSimulationDuration => _adSimulationDuration;
        public InterstitialState InterstitialAdReturnState => _interstitialAdReturnState;
        public RewardedState RewardedAdReturnState => _rewardedAdReturnState;
        
        [SerializeField] private SupportedSDKType _supportedSDKType;
        [SerializeField] private bool _muteAudioOnPause;
        [SerializeField] private string _playerId;
        [SerializeField] private DeviceType _debugDeviceType;
        [SerializeField] private string _debugLanguageCode;
        [SerializeField] private bool _autoConstructAndInitializeSingleton;
        [SerializeField] [Range(0f, 10f)] private float _debugInitializeTime;
        [SerializeField] private string _installedPackageVersion = "";
        [SerializeField] private AspectRatioMode _aspectRatio;
        [SerializeField] private Texture2D _backgroundImage;
        [SerializeField] private bool _loadSaveOnInitialize = true;
        [SerializeField] [Range(0f, 10f)] private float _adSimulationDuration;
        [SerializeField] private InterstitialState _interstitialAdReturnState;
        [SerializeField] private RewardedState _rewardedAdReturnState;
        
        
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
            return null;
#endif
        }
#if UNITY_EDITOR
        [MenuItem("Tools/WelwiseGamesSDK/Create Settings", priority = 0)]
        private static void CreateSettingsMenuItem()
        {
            var settings = LoadOrCreateSettings();
            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }

        [MenuItem("Tools/WelwiseGamesSDK/Create Settings", validate = true)]
        private static bool ValidateCreateSettingsMenuItem()
        {
            return Resources.Load<SDKSettings>(nameof(SDKSettings)) == null;
        }
#endif

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