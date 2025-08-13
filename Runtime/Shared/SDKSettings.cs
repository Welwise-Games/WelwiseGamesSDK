using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using WelwiseGamesSDK.Shared.Types;
using DeviceType = WelwiseGamesSDK.Shared.Types.DeviceType;

namespace WelwiseGamesSDK.Shared
{
    [Serializable]
    public sealed class SDKSettings
    {
        public enum AspectRatioMode
        {
            Default,
            Aspect16_9,
            Aspect9_16
        }
        
        public string DebugPlayerId;
        public SupportedSDKType SDKType;
        public DeviceType DebugDeviceType;
        public string DebugLanguageCode;
        public bool MuteAudioOnPause;
        public bool AutoConstructAndInitializeSingleton;
        public float DebugInitializeTime;
        public bool LoadSaveOnInitialize;
        public string InstalledTemplateVersion;
        public AspectRatioMode AspectRatio;
        public string BackgroundImagePath; // Путь относительно папки Resources
        public float AdSimulationDuration;
        public InterstitialState InterstitialAdReturnState;
        public RewardedState RewardedAdReturnState;
        public List<Product> MockProducts = new List<Product>();
        public List<Purchase> MockPurchases = new List<Purchase>();
        public float PurchaseSimulationDuration = 2f;
        public bool EditorAdvertisementModule = true;
        public bool EditorPaymentsModule = true;
        public bool EditorAnalyticsModule = true;
        public bool EditorEnvironmentModule = true;
        public bool EditorPlatformNavigationModule = true;
        public bool EditorPlayerDataModule = true;
        public bool EditorGameDataModule = true;
        public bool EditorMetaverseDataModule = true;
        public bool UseThreeJsLoader = true;
        public bool ShowAdOnStart = true;
        
        public static SDKSettings LoadOrCreateSettings()
        {
            try
            {
                var settingsPath = GetSettingsPath();
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    return JsonConvert.DeserializeObject<SDKSettings>(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load SDK settings: {e.Message}");
            }

            return new SDKSettings
            {
                DebugPlayerId = Guid.NewGuid().ToString(),
                SDKType = SupportedSDKType.WelwiseGames,
                DebugDeviceType = DeviceType.Desktop,
                DebugLanguageCode = "en",
                MuteAudioOnPause = true,
                AutoConstructAndInitializeSingleton = true,
                DebugInitializeTime = 2f,
                LoadSaveOnInitialize = true,
                InstalledTemplateVersion = "",
                AspectRatio = AspectRatioMode.Default,
                BackgroundImagePath = "",
                AdSimulationDuration = 5f,
                InterstitialAdReturnState = InterstitialState.Closed,
                RewardedAdReturnState = RewardedState.Closed,
                MockProducts = new List<Product>(),
                MockPurchases = new List<Purchase>(),
                PurchaseSimulationDuration = 2f
            };
        }
        
        public void Save()
        {
            try
            {
                var path = GetSettingsPath();
                var directory = Path.GetDirectoryName(path);
                
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, json);
                
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save SDK settings: {e.Message}");
            }
        }
        
        private static string GetSettingsPath()
        {
            var resourcesPath = Path.Combine(Application.dataPath, "Resources");
            return Path.Combine(resourcesPath, "sdk_settings.json");
        }
    }
}