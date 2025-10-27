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
        public string GameDistributionId;
        public string Y8AppId;
        public string Y8HostId;
        public string Y8AdsenseId;
        public string Y8ChannelId;
        public string Y8AdFrequency;
        public bool Y8TestAdsOn;
        public bool Y8ActivateAFP;
        
        public static SDKSettings LoadOrCreateSettings()
        {
            var settingsAsset = Resources.Load<TextAsset>("sdk_settings");
            if (settingsAsset != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<SDKSettings>(settingsAsset.text);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to deserialize SDK settings: {e.Message}");
                }
            }

            return CreateDefaultSettings();
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
        
        private static SDKSettings CreateDefaultSettings()
        {
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
                PurchaseSimulationDuration = 2f,
                Y8AppId = "YOUR_Y8_APP_ID",
                Y8HostId = "ca-host-pub-6129580795478709",
                Y8AdsenseId = "ca-pub-6129580795478709",
                Y8ChannelId = "123456",
                Y8AdFrequency = "180s",
                Y8TestAdsOn = true,
                Y8ActivateAFP = true
            };
        }
    }
}