using UnityEditor;
using UnityEngine;
using System.IO;

namespace WelwiseGames.Editor
{
    [InitializeOnLoad]
    public static class PluginMigrations
    {
        private const string MigrationKey = "WelwiseGamesSDK_Migration_v229";
        private const string TargetVersion = "2.2.9";

        static PluginMigrations()
        {
            if (!EditorPrefs.GetBool(MigrationKey, false))
            {
                EditorApplication.delayCall += RunMigrations;
            }
        }

        private static void RunMigrations()
        {
            if (EditorPrefs.GetBool(MigrationKey, false))
                return;

            // Проверяем версию пакета
            string packageVersion = GetPackageVersion();
            if (IsVersionNewerOrEqual(packageVersion, TargetVersion))
            {
                DeleteLegacyFiles();
                EditorPrefs.SetBool(MigrationKey, true);
                Debug.Log("WelwiseGamesSDK: Legacy files migration completed");
            }
        }

        private static string GetPackageVersion()
        {
            try
            {
                var listRequest = UnityEditor.PackageManager.Client.List();
                while (!listRequest.IsCompleted)
                    System.Threading.Thread.Sleep(100);

                if (listRequest.Status == UnityEditor.PackageManager.StatusCode.Success)
                {
                    foreach (var package in listRequest.Result)
                    {
                        if (package.name == "com.welwise.sdk")
                        {
                            return package.version;
                        }
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки
            }
            return "0.0.0";
        }

        private static bool IsVersionNewerOrEqual(string current, string target)
        {
            try
            {
                var currentVersion = new System.Version(current);
                var targetVersion = new System.Version(target);
                return currentVersion.CompareTo(targetVersion) >= 0;
            }
            catch
            {
                return false;
            }
        }

        private static void DeleteLegacyFiles()
        {
            // Удаляем старый ScriptableObject
            string[] oldSettingsPaths = 
            {
                "Assets/Resources/SDKSettings.asset",
                "Assets/Resources/SDKSettings.asset.meta"
            };

            // Удаляем старые jslib файлы
            string[] oldJslibPaths = 
            {
                "Assets/Plugins/WebGL/welwise-sdk.jslib",
                "Assets/Plugins/WebGL/welwise-sdk.jslib.meta",
                "Assets/Plugins/WebGL/yandex-sdk.jslib",
                "Assets/Plugins/WebGL/yandex-sdk.jslib.meta"
            };

            DeleteFiles(oldSettingsPaths);
            DeleteFiles(oldJslibPaths);
        }

        private static void DeleteFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    Debug.Log($"Deleting legacy file: {path}");
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Debug.Log($"Deleting legacy directory: {path}");
                    Directory.Delete(path, true);
                }
            }
        }
    }
}