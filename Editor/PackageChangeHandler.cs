using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using WelwiseGamesSDK.Shared;

namespace WelwiseGames.Editor
{
    [InitializeOnLoad]
    public class PackageChangeHandler
    {
        private const string DisplayName = "Welwise Games SDK";
        private const string PackageID = "com.welwise.sdk";
        private const string GithubUser = "Welwise-Games";
        private const string GithubRepo = "WelwiseGamesSDK";
        private const string RemoteVersionURL = "https://raw.githubusercontent.com/{0}/{1}/main/package.json";
        private const string NotificationDataKey = "WelwiseGamesSDK_UpdateNotificationData";
        
        static PackageChangeHandler()
        {
            Events.registeredPackages += OnPackagesChanged;
            EditorApplication.delayCall += CheckForUpdates;
        }

        private static void OnPackagesChanged(PackageRegistrationEventArgs args)
        {
            var settings = SDKSettings.LoadOrCreateSettings();
            if (settings.InstalledTemplateVersion == SDKSettingsEditor.TemplateVersion) 
                return;
            
            Debug.Log($"Detected new package version ({SDKSettingsEditor.TemplateVersion}). Updating files...");
            SDKSettingsEditor.HandleSDKTypeChange(settings.SDKType, settings.SDKType);
            settings.InstalledTemplateVersion = SDKSettingsEditor.TemplateVersion;
            settings.Save();
        } 

        private static void CheckForUpdates()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(CheckVersionCoroutine());
        }
    
        private static IEnumerator CheckVersionCoroutine()
        {
            string localVersion = GetLocalPackageVersion();
            if (string.IsNullOrEmpty(localVersion)) 
                yield break;

            var remoteUrl = string.Format(RemoteVersionURL, GithubUser, GithubRepo);
            using var webRequest = UnityWebRequest.Get(remoteUrl);
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[{DisplayName}] Version check error: {webRequest.error}");
                yield break;
            }

            var remoteVersion = ParseVersionFromJson(webRequest.downloadHandler.text);
            if (string.IsNullOrEmpty(remoteVersion)) 
                yield break;

            CompareVersions(localVersion, remoteVersion);
        }
        
        private static string GetLocalPackageVersion()
        {
            var listRequest = Client.List();
            while (!listRequest.IsCompleted)
                System.Threading.Thread.Sleep(100);

            return listRequest.Status == StatusCode.Success ? 
                listRequest.Result.FirstOrDefault(p => p.name == PackageID)?.version : 
                null;
        }
    
        private static string ParseVersionFromJson(string json)
        {
            var match = new Regex("\"version\":\\s*\"([\\d.]+)\"").Match(json);
            return match.Success ? match.Groups[1].Value : null;
        }
        
        private static void CompareVersions(string currentVersion, string remoteVersion)
        {
            if (currentVersion == remoteVersion) return;

            var notification = LoadNotificationData();
            var shouldNotify = notification == null || 
                              IsNewVersion(notification, remoteVersion) || 
                              IsNotificationExpired(notification);

            if (!shouldNotify) return;
            
            ShowUpdateNotification(currentVersion, remoteVersion);
            SaveNotificationData(remoteVersion);
        }
        
        private static bool IsNewVersion(NotificationData notification, string remoteVersion)
        {
            return notification.Version != remoteVersion;
        }
        
        private static bool IsNotificationExpired(NotificationData notification)
        {
            return DateTime.TryParse(notification.Date, CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out var lastDate) && 
                (DateTime.Now - lastDate).TotalDays >= 7;
        }

        private static NotificationData LoadNotificationData()
        {
            var json = PlayerPrefs.GetString(NotificationDataKey);
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<NotificationData>(json);
        }

        private static void SaveNotificationData(string version)
        {
            PlayerPrefs.SetString(NotificationDataKey, JsonConvert.SerializeObject(new NotificationData
            {
                Version = version,
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            }));
            PlayerPrefs.Save();
        }

        private static void ShowUpdateNotification(string currentVersion, string remoteVersion)
        {
            Debug.LogWarning($"[{DisplayName}] Update available! Current: {currentVersion}, New: {remoteVersion}");

            if (EditorUtility.DisplayDialog(
                $"{DisplayName} Update Available",
                $"New version {remoteVersion} is available!\nCurrent version: {currentVersion}",
                "Open Package Manager",
                "Later"))
            {
                EditorApplication.ExecuteMenuItem("Window/Package Manager");
            }
        }
        
        [Serializable]
        private class NotificationData
        {
            public string Version;
            public string Date;
        }
    }
}