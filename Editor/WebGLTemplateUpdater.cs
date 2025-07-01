using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGames.Editor
{
    public static class WebGLTemplateUpdater
    {
        private const string TemplateFolder = "WebGLTemplates/Welwise SDK";
        private const string AdapterFileName = "sdk-adapter.js";

        public static void UpdateTemplate(SupportedSDKType sdkType)
        {
            try
            {
                CleanTemplateFolder();
                ImportTemplatePackage();
                CopySDKAdapter(sdkType);
                UpdatePlayerSettingsTemplate();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating template: {e.Message}");
            }
        }

        private static void CleanTemplateFolder()
        {
            if (AssetDatabase.IsValidFolder($"Assets/{TemplateFolder}"))
            {
                AssetDatabase.DeleteAsset($"Assets/{TemplateFolder}");
            }
        }

        private static void ImportTemplatePackage()
        {
            var packageGuid = AssetDatabase.FindAssets("unified-template");
            if (packageGuid.Length == 0)
            {
                Debug.LogError("Template package not found in project!");
                return;
            }

            var packagePath = AssetDatabase.GUIDToAssetPath(packageGuid[0]);
            if (!packagePath.EndsWith(".unitypackage"))
            {
                Debug.LogError("Invalid template package format!");
                return;
            }

            AssetDatabase.ImportPackage(packagePath, false);
        }

        public static void CopySDKAdapter(SupportedSDKType sdkType)
        {
            var resourceName = GetAdapterResourceName(sdkType);
            var adapterAsset = Resources.Load<TextAsset>(resourceName);
            
            if (adapterAsset == null)
            {
                Debug.LogError($"Failed to load SDK adapter: {resourceName}");
                return;
            }

            CreateTemplateDirectory();
            SaveAdapterFile(adapterAsset.text);
        }

        private static string GetAdapterResourceName(SupportedSDKType sdkType)
        {
            return sdkType switch
            {
                SupportedSDKType.WelwiseGames => "welwise-sdk-adapter",
                SupportedSDKType.YandexGames => "yandex-sdk-adapter",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static void CreateTemplateDirectory()
        {
            var templatePath = $"Assets/{TemplateFolder}";
            if (!Directory.Exists(templatePath))
            {
                Directory.CreateDirectory(templatePath);
            }
        }

        private static void SaveAdapterFile(string content)
        {
            AssetDatabase.DeleteAsset($"Assets/{TemplateFolder}/{AdapterFileName}");
            var filePath = $"Assets/{TemplateFolder}/{AdapterFileName}";
            File.WriteAllText(filePath, content);
        }

        private static void UpdatePlayerSettingsTemplate()
        {
            PlayerSettings.WebGL.template = "PROJECT:Welwise SDK";
            Debug.Log("WebGL template updated");
        }
    }
}