using System.IO;
using UnityEditor;
using UnityEngine;
using WelwiseGamesSDK.Internal;
using WelwiseGamesSDK.Shared;

namespace WelwiseGames.Editor
{
    public class SDKSettingsEditor : EditorWindow
    {
        // ReSharper disable once InconsistentNaming
        public const string PACKAGE_VERSION = "0.0.2";
        
        private SDKSettings _settings;
        private SerializedObject _serializedSettings;
        private SerializedProperty _supportedSDKType;
        private SerializedProperty _playerId;
        private SerializedProperty _muteAudioOnPause;
        private SerializedProperty _debugDeviceType;
        private SerializedProperty _debugLanguageCode;
        private SerializedProperty _autoConstructAndInitializeSingleton;
        private SerializedProperty _debugInitializeTime;
        private SupportedSDKType _lastSDKType;

        [MenuItem("Tools/WelwiseGamesSDK/SDK Settings")]
        public static void ShowWindow()
        {
            GetWindow<SDKSettingsEditor>("SDK Settings");
        }

        void OnEnable()
        {
            _settings = SDKSettings.LoadOrCreateSettings();
            _serializedSettings = new SerializedObject(_settings);

            _supportedSDKType = _serializedSettings.FindProperty("_supportedSDKType");
            _playerId = _serializedSettings.FindProperty("_playerId");
            _muteAudioOnPause = _serializedSettings.FindProperty("_muteAudioOnPause");
            _debugDeviceType = _serializedSettings.FindProperty("_debugDeviceType");
            _debugLanguageCode = _serializedSettings.FindProperty("_debugLanguageCode");
            _autoConstructAndInitializeSingleton = _serializedSettings.FindProperty("_autoConstructAndInitializeSingleton");
            _debugInitializeTime = _serializedSettings.FindProperty("_debugInitializeTime");
            
            if (_settings.InstalledPackageVersion != PACKAGE_VERSION)
            {
                Debug.Log($"Detect new package version ({PACKAGE_VERSION}). Update files...");
                UpdateFilesForCurrentSDK();
                _settings.InstalledPackageVersion = PACKAGE_VERSION;
                SaveChanges();
            }
            
            ValidateRequiredPackages();
            _lastSDKType = _settings.SupportedSDKType;
        }
        
        private void ValidateRequiredPackages()
        {
            switch (_settings.SupportedSDKType)
            {
                case SupportedSDKType.WelwiseGames:
                    if (!File.Exists(Path.Combine(Application.dataPath, "Plugins/WebGL/welwise-sdk.jslib")))
                    {
                        Debug.Log("Welwise Games SDK package missing. Importing...");
                        ImportPackage("welwise-games-template");
                        UpdateWebGLTemplate();
                    }
                    break;
                    
                case SupportedSDKType.YandexGames:
                    if (!File.Exists(Path.Combine(Application.dataPath, "Plugins/WebGL/yandex-games.jslib")))
                    {
                        Debug.Log("Yandex Games SDK package missing. Importing...");
                        ImportPackage("yandex-games-template");
                        UpdateWebGLTemplate();
                    }
                    break;
            }
        }

        void OnGUI()
        {
            _serializedSettings.Update();
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_supportedSDKType, new GUIContent("SDK Type"));
            EditorGUILayout.PropertyField(_muteAudioOnPause, new GUIContent("Mute Audio On Pause"));
            EditorGUILayout.PropertyField(_autoConstructAndInitializeSingleton, new GUIContent("Auto Singleton"));
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(_playerId, new GUIContent("Player ID"));
        
                if (GUILayout.Button("Generate", GUILayout.Width(80)))
                {
                    _playerId.stringValue = System.Guid.NewGuid().ToString();
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_debugDeviceType, new GUIContent("Device Type"));
            EditorGUILayout.PropertyField(_debugLanguageCode, new GUIContent("Language Code"));
            EditorGUILayout.PropertyField(_debugInitializeTime, new GUIContent("Initialization Time"));
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"Package Version: {PACKAGE_VERSION}", new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            });
            
            _serializedSettings.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                SaveChanges();
            }
            
            if (_settings.SupportedSDKType != _lastSDKType)
            {
                HandleSDKTypeChange(_lastSDKType, _settings.SupportedSDKType);
                _lastSDKType = _settings.SupportedSDKType;
                SaveChanges();
            }
        }
        
        public static void HandleSDKTypeChange(SupportedSDKType oldType, SupportedSDKType newType)
        {
            try
            {
                bool success = true;
                
                DeleteDirectory("WebGL Templates/Welwise SDK");
                
                switch (newType)
                {
                    case SupportedSDKType.WelwiseGames:
                        success &= DeleteFile("Plugins/WebGL/yandex-games.jslib");
                        ImportPackage("welwise-games-template");
                        break;
                        
                    case SupportedSDKType.YandexGames:
                        success &= DeleteFile("Plugins/WebGL/welwise-sdk.jslib");
                        ImportPackage("yandex-games-template");
                        break;
                }

                if (success)
                {
                    UpdateWebGLTemplate();
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing SDK change: {e.Message}");
            }
        }
        
        private void UpdateFilesForCurrentSDK()
        {
            Debug.Log($"Updating files for SDK: {_settings.SupportedSDKType}");
            HandleSDKTypeChange(_settings.SupportedSDKType, _settings.SupportedSDKType);
        }

        private static bool DeleteFile(string relativePath)
        {
            string fullPath = Path.Combine(Application.dataPath, relativePath);
            string assetPath = Path.Combine("Assets", relativePath);

            if (File.Exists(fullPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
                Debug.Log($"Deleted file: {assetPath}");
                return true;
            }
            return false;
        }

        private static void DeleteDirectory(string relativePath)
        {
            string assetPath = Path.Combine("Assets", relativePath);
            
            if (AssetDatabase.IsValidFolder(assetPath))
            {
                FileUtil.DeleteFileOrDirectory(assetPath);
                FileUtil.DeleteFileOrDirectory(assetPath + ".meta");
                Debug.Log($"Deleted directory: {assetPath}");
            }
        }

        private static void ImportPackage(string packageName)
        {
            var guids = AssetDatabase.FindAssets(packageName);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) == $"{packageName}.unitypackage")
                {
                    AssetDatabase.ImportPackage(path, false);
                    Debug.Log($"Imported {packageName} package");
                    return;
                }
            }
            Debug.LogError($"Package {packageName}.unitypackage not found!");
        }

        private static void UpdateWebGLTemplate()
        {
            PlayerSettings.WebGL.template = "PROJECT:Welwise SDK";
            Debug.Log("WebGL template updated to 'Welwise SDK'");
        }

        private new void SaveChanges()
        {
            if (_serializedSettings != null && _settings != null)
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }
    }
}