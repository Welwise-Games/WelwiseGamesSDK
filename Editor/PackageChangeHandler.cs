using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGames.Editor
{
    [InitializeOnLoad]
    public class PackageChangeHandler
    {

        static PackageChangeHandler()
        {
            Events.registeredPackages += OnPackagesChanged;
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
    }
}