﻿using System;
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

            if (settings.InstalledPackageVersion == SDKSettingsEditor.PACKAGE_VERSION) return;
            Debug.Log($"Detect new package version ({SDKSettingsEditor.PACKAGE_VERSION}). Update files...");
            SDKSettingsEditor.HandleSDKTypeChange(settings.SupportedSDKType, settings.SupportedSDKType);
            settings.InstalledPackageVersion = SDKSettingsEditor.PACKAGE_VERSION;
            AssetDatabase.SaveAssets();
        } 

        private static void CheckForUpdates()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(CheckVersionCoroutine());
        }
    
        private static IEnumerator CheckVersionCoroutine()
        {
            string localVersion = null;
            var listRequest = Client.List();
            while (!listRequest.IsCompleted)
                yield return null;

            if (listRequest.Status == StatusCode.Success)
            {
                var package = listRequest.Result.FirstOrDefault(p => p.name == PackageID);
                if (package != null)
                {
                    localVersion = package.version;
                }
            }

            if (string.IsNullOrEmpty(localVersion))
            {
                Debug.LogWarning($"[{DisplayName}] Failed to get local package version");
                yield break;
            }

            var remoteUrl = string.Format(RemoteVersionURL, GithubUser, GithubRepo);

            using UnityWebRequest webRequest = UnityWebRequest.Get(remoteUrl);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var remoteVersion = ParseVersionFromJson( webRequest.downloadHandler.text);
                    
                if (!string.IsNullOrEmpty(remoteVersion))
                {
                    CompareVersions(localVersion, remoteVersion);
                }
                else
                {
                    Debug.LogWarning($"[{DisplayName}] Failed to parse remote version");
                }
            }
            else
            {
                Debug.LogWarning($"[{DisplayName}] Version check error: {webRequest.error}");
            }
        }
    
        private static string ParseVersionFromJson(string json)
        {
            var versionRegex = new Regex("\"version\":\\s*\"([0-9]+\\.[0-9]+\\.[0-9]+)\"");
            var match = versionRegex.Match(json);

            return match.Success ?
                match.Groups[1].Value :
                string.Empty;
        }
        
        private static void CompareVersions(string currentVersion, string remoteVersion)
        {
            if (currentVersion == remoteVersion) return;

            var notificationData = LoadNotificationData();
            var showNotification = false;

            if (notificationData == null)
            {
                showNotification = true;
            }
            else
            {
                var isNewVersion = notificationData.Version != remoteVersion;
                var isTimeExpired = false;

                if (DateTime.TryParse(notificationData.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out var lastDate))
                {
                    isTimeExpired = (DateTime.Now - lastDate).TotalDays >= 7;
                }

                showNotification = isNewVersion || isTimeExpired;
            }

            if (!showNotification) return;
            ShowUpdateNotification(currentVersion, remoteVersion);
            SaveNotificationData(remoteVersion);
        }

        private static NotificationData LoadNotificationData()
        {
            var jsonData = PlayerPrefs.GetString(NotificationDataKey, "");
            return !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<NotificationData>(jsonData) : null;
        }

        private static void SaveNotificationData(string version)
        {
            var data = new NotificationData
            {
                Version = version,
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };

            PlayerPrefs.SetString(NotificationDataKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        private static void ShowUpdateNotification(string currentVersion, string remoteVersion)
        {
            Debug.LogWarning($"[{DisplayName}] Update available!\nCurrent: {currentVersion}, New: {remoteVersion}\nPlease update via Package Manager");

            var openPackageManager = EditorUtility.DisplayDialog(
                $"{DisplayName} Update Available",
                $"A new version is available!\nCurrent: {currentVersion}\nNew: {remoteVersion}",
                "Open Package Manager",
                "Later"
            );

            if (openPackageManager)
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