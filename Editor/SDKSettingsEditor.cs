using System;
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
        public const string PACKAGE_VERSION = "0.0.10";
        
        private SDKSettings _settings;
        private SerializedObject _serializedSettings;
        private SerializedProperty _supportedSDKType;
        private SerializedProperty _playerId;
        private SerializedProperty _muteAudioOnPause;
        private SerializedProperty _debugDeviceType;
        private SerializedProperty _debugLanguageCode;
        private SerializedProperty _autoConstructAndInitializeSingleton;
        private SerializedProperty _debugInitializeTime;
        private SerializedProperty _aspectRatio;
        private SerializedProperty _backgroundImage;
        private SerializedProperty _loadSaveOnInitialize;
        private SerializedProperty _adSimulationDuration;
        private SerializedProperty _interstitialAdReturnState;
        private SerializedProperty _rewardedAdReturnState;
        private SupportedSDKType _lastSDKType;

        [MenuItem("Tools/WelwiseGamesSDK/SDK Settings")]
        public static void ShowWindow()
        {
            GetWindow<SDKSettingsEditor>("SDK Settings");
        }

        private void OnEnable()
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
            _aspectRatio = _serializedSettings.FindProperty("_aspectRatio");
            _backgroundImage = _serializedSettings.FindProperty("_backgroundImage");
            _loadSaveOnInitialize = _serializedSettings.FindProperty("_loadSaveOnInitialize");
            _adSimulationDuration = _serializedSettings.FindProperty("_adSimulationDuration");
            _interstitialAdReturnState  = _serializedSettings.FindProperty("_interstitialAdReturnState");
            _rewardedAdReturnState  = _serializedSettings.FindProperty("_rewardedAdReturnState");
            
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnGUI()
        {
            _serializedSettings.Update();
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_supportedSDKType, new GUIContent("SDK Type", "Selected sdk type, when changing the value, template files and jslib files are changed"));
            EditorGUILayout.PropertyField(_muteAudioOnPause, new GUIContent("Mute Audio On Pause", "Mute audio when changing focus or pausing the game"));
            EditorGUILayout.PropertyField(_autoConstructAndInitializeSingleton, new GUIContent("Auto Singleton", "Automatic execution of singleton creation and initialization"));
            EditorGUILayout.PropertyField(_loadSaveOnInitialize, new GUIContent("Load save on initialize", "Loading a save is done during the initialization phase"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Build Process Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(_aspectRatio, 
                    new GUIContent("Aspect Ratio Mode", "Game display mode on pc"));
                
                if (_aspectRatio.enumValueIndex != (int)SDKSettings.AspectRatioMode.Default)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    {
                        EditorGUILayout.PropertyField(_backgroundImage, 
                            new GUIContent("Background Image", "Background image for mode other than Default"));
                        
                        EditorGUILayout.HelpBox("Recommended size: 1920x1080 or larger. Will be scaled to fit screen.", MessageType.Info);
                        
                        if (_backgroundImage.objectReferenceValue != null)
                        {
                            EditorGUILayout.Space(5);
                            var preview = (Texture2D)_backgroundImage.objectReferenceValue;
                            GUILayout.Label("Preview:", EditorStyles.boldLabel);
                            var rect = GUILayoutUtility.GetRect(200, 120, GUILayout.ExpandWidth(false));
                            EditorGUI.DrawPreviewTexture(rect, preview);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PropertyField(_playerId, new GUIContent("Player ID", "The player ID that will be used when working in the Unity editor"));
        
                if (GUILayout.Button("Generate", GUILayout.Width(80)))
                {
                    _playerId.stringValue = System.Guid.NewGuid().ToString();
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(_debugDeviceType, new GUIContent("Device Type", "The type of device that will be used when working in the Unity editor"));
            EditorGUILayout.PropertyField(_debugLanguageCode, new GUIContent("Language", "The user language that will be used when working in the Unity editor"));
            EditorGUILayout.PropertyField(_debugInitializeTime, new GUIContent("Initialization Time", "Initialization time for initializing a plugin in the unity editor, it can be useful to set it longer for testing or shorter for quick development iterations."));
            EditorGUILayout.PropertyField(_adSimulationDuration, new GUIContent("Ad Duration", "Simulation time of ad display in the editor"));
            EditorGUILayout.PropertyField(_interstitialAdReturnState, new GUIContent("Interstitial Return", "The result of showing interstitial ads in the editor, can be changed during the game"));
            EditorGUILayout.PropertyField(_rewardedAdReturnState, new GUIContent("Rewarded Return", "The result of displaying Rewarded ads in the editor, can be changed during the game"));
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

            if (_settings.SupportedSDKType == _lastSDKType) return;
            HandleSDKTypeChange(_lastSDKType, _settings.SupportedSDKType);
            _lastSDKType = _settings.SupportedSDKType;
            SaveChanges();
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
            catch (Exception e)
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
            var assetPath = Path.Combine("Assets", relativePath);

            if (!AssetDatabase.IsValidFolder(assetPath)) return;
            FileUtil.DeleteFileOrDirectory(assetPath);
            FileUtil.DeleteFileOrDirectory(assetPath + ".meta");
            Debug.Log($"Deleted directory: {assetPath}");
        }

        private static void ImportPackage(string packageName)
        {
            var guids = AssetDatabase.FindAssets(packageName);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) != $"{packageName}.unitypackage") continue;
                AssetDatabase.ImportPackage(path, false);
                Debug.Log($"Imported {packageName} package");
                return;
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
            if (_serializedSettings == null || _settings == null) return;
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}