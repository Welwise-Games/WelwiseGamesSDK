﻿using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

namespace WelwiseGames.Editor
{
    public class SDKSettingsEditor : EditorWindow
    {
        public const string TemplateVersion = "0.0.11";
        private const string LogoFileName = "Logo";
        
        private enum TabType
        {
            General,
            MockPayments,
            About
        }
        
        private TabType _currentTab = TabType.General;
        private SDKSettings _settings;
        private SupportedSDKType _lastSDKType;
        private Texture2D _backgroundTexture;
        private Texture2D _logoTexture;
        private bool _needsTextureReload;
        private ReorderableList _productsList;
        private ReorderableList _purchasesList;
        private string _manifestVersion = "Unknown";
        private ListRequest _listRequest;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/WelwiseGamesSDK/SDK Settings")]
        public static void ShowWindow()
        {
            GetWindow<SDKSettingsEditor>("SDK Settings");
        }

        private void OnEnable()
        {
            LoadSettings();
            InitializeLists();
            ValidateRequiredPackages();
            LoadLogoTexture();
            StartManifestVersionFetch();
        }

        private void LoadLogoTexture()
        {
            _logoTexture = Resources.Load<Texture2D>(LogoFileName);
        }

        private void StartManifestVersionFetch()
        {
            _listRequest = Client.List();
            EditorApplication.update += ProgressPackageVersionFetch;
        }

        private void ProgressPackageVersionFetch()
        {
            if (_listRequest == null || !_listRequest.IsCompleted) 
                return;

            EditorApplication.update -= ProgressPackageVersionFetch;
            
            if (_listRequest.Status == StatusCode.Success)
            {
                foreach (var package in _listRequest.Result)
                {
                    if (package.name == "com.welwise.sdk")
                    {
                        _manifestVersion = package.version;
                        break;
                    }
                }
            }
            else
            {
                _manifestVersion = "Error";
            }
            
            Repaint();
        }

        private void OnDisable()
        {
            EditorApplication.update -= ProgressPackageVersionFetch;
        }

        private void InitializeLists()
        {
            _productsList = new ReorderableList(_settings.MockProducts, typeof(Product), true, true, true, true)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Mock Products"),
                    drawElementCallback = (rect, index, _, _) => DrawProductElement(rect, index),
                    elementHeight = EditorGUIUtility.singleLineHeight * 7,
                    onRemoveCallback = list => 
                    {
                        _settings.MockProducts.RemoveAt(list.index);
                        SaveSettings();
                    }
                };

            _purchasesList = new ReorderableList(_settings.MockPurchases, typeof(Purchase), true, true, true, true)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Mock Purchases"),
                    drawElementCallback = (rect, index, _, _) => DrawPurchaseElement(rect, index),
                    elementHeight = EditorGUIUtility.singleLineHeight * 4,
                    onRemoveCallback = list => 
                    {
                        _settings.MockPurchases.RemoveAt(list.index);
                        SaveSettings();
                    }
                };
        }
        
        private void DrawProductElement(Rect rect, int index)
        {
            var product = _settings.MockProducts[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            
            product.Id = EditorGUI.TextField(rect, "ID", product.Id);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.Title = EditorGUI.TextField(rect, "Title", product.Title);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.Description = EditorGUI.TextField(rect, "Description", product.Description);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.ImageUri = EditorGUI.TextField(rect, "Image URI", product.ImageUri);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.Price = EditorGUI.TextField(rect, "Price", product.Price);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.PriceCurrencyCode = EditorGUI.TextField(rect, "Currency", product.PriceCurrencyCode);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            product.PriceValue = EditorGUI.TextField(rect, "Price Value", product.PriceValue);
            
            _settings.MockProducts[index] = product;
        }

        private void DrawPurchaseElement(Rect rect, int index)
        {
            var purchase = _settings.MockPurchases[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            
            purchase.ProductId = EditorGUI.TextField(rect, "Product ID", purchase.ProductId);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            purchase.PurchaseToken = EditorGUI.TextField(rect, "Token", purchase.PurchaseToken);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            purchase.DeveloperPayload = EditorGUI.TextField(rect, "Payload", purchase.DeveloperPayload);
            rect.y += EditorGUIUtility.singleLineHeight;
            
            purchase.Signature = EditorGUI.TextField(rect, "Signature", purchase.Signature);
            
            _settings.MockPurchases[index] = purchase;
        }

        private void DrawMockPaymentsSettings()
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Mock Payments Settings", EditorStyles.boldLabel);
            
            _settings.PurchaseSimulationDuration = EditorGUILayout.Slider(
                "Simulation Duration",
                _settings.PurchaseSimulationDuration, 0.5f, 10f);
            
            EditorGUILayout.Space(10);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            _productsList.DoLayoutList();
            
            EditorGUILayout.Space(10);
            _purchasesList.DoLayoutList();
            EditorGUILayout.EndScrollView();
        }
        
        private void LoadSettings()
        {
            _settings = SDKSettings.LoadOrCreateSettings();
            _lastSDKType = _settings.SDKType;
            LoadBackgroundTexture();
        }
        
        private void LoadBackgroundTexture()
        {
            _backgroundTexture = !string.IsNullOrEmpty(_settings.BackgroundImagePath) ? 
                Resources.Load<Texture2D>(_settings.BackgroundImagePath) : 
                null;
        }

        private void ValidateRequiredPackages()
        {
            if (_settings.InstalledTemplateVersion != TemplateVersion)
            {
                Debug.Log($"Detected new template version ({TemplateVersion}). Updating files...");
                UpdateFilesForCurrentSDK();
                _settings.InstalledTemplateVersion = TemplateVersion;
                SaveSettings();
            }
            
            switch (_settings.SDKType)
            {
                case SupportedSDKType.WelwiseGames:
                    EnsureFileExists("Plugins/WebGL/welwise-sdk.jslib", "welwise-games-template");
                    break;
                    
                case SupportedSDKType.YandexGames:
                    EnsureFileExists("Plugins/WebGL/yandex-games.jslib", "yandex-games-template");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static void EnsureFileExists(string relativePath, string packageName)
        {
            var fullPath = Path.Combine(Application.dataPath, relativePath);
            if (File.Exists(fullPath)) return;
            Debug.Log($"{Path.GetFileName(relativePath)} missing. Importing template...");
            ImportPackage(packageName);
            UpdateWebGLTemplate();
        }

        private void OnGUI()
        {
            if (_needsTextureReload)
            {
                LoadBackgroundTexture();
                _needsTextureReload = false;
            }
            
            DrawHeader();
            
            EditorGUI.BeginChangeCheck();
            
            switch (_currentTab)
            {
                case TabType.General:
                    DrawRuntimeSettings();
                    DrawBuildSettings();
                    DrawEditorSettings();
                    break;
                    
                case TabType.MockPayments:
                    DrawMockPaymentsSettings();
                    break;
                    
                case TabType.About:
                    DrawAboutTab();
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
            }

            if (_settings.SDKType != _lastSDKType)
            {
                HandleSDKTypeChange(_lastSDKType, _settings.SDKType);
                _lastSDKType = _settings.SDKType;
                SaveSettings();
            }
        }
        
        private void DrawHeader()
        {
            const int headerHeight = 32;
            const int tabPadding = 0;
            
            var headerStyle = new GUIStyle(EditorStyles.toolbar)
            {
                fixedHeight = headerHeight
            };
        
            var tabStyle = new GUIStyle(EditorStyles.toolbarButton)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 0, 0),
                fixedHeight = headerHeight - 4
            };
        
            EditorGUILayout.BeginHorizontal(headerStyle);
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(180));
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (_logoTexture != null)
                        {
                            GUILayout.Label(_logoTexture, 
                                GUILayout.Height(32), 
                                GUILayout.Width(32));
                        }
                        GUILayout.Label("Welwise Games SDK", 
                            EditorStyles.boldLabel, 
                            GUILayout.Height(32));
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndVertical();
                
                GUILayout.FlexibleSpace();
                
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    var tabs = Enum.GetNames(typeof(TabType));
                    var tabWidth = (EditorGUIUtility.currentViewWidth - 350) / tabs.Length;
                    
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(headerHeight));
                    {
                        GUILayout.FlexibleSpace();
                        for (int i = 0; i < tabs.Length; i++)
                        {
                            if (i > 0) GUILayout.Space(tabPadding);
                            
                            var style = new GUIStyle(tabStyle);
                            if ((int)_currentTab == i)
                            {
                                style.normal.background = Texture2D.grayTexture;
                            }
                            
                            if (GUILayout.Button(tabs[i], style, 
                                GUILayout.Width(tabWidth - tabPadding)))
                            {
                                _currentTab = (TabType)i;
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawRuntimeSettings()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);

            _settings.SDKType = (SupportedSDKType)EditorGUILayout.EnumPopup(
                "SDK Type", 
                _settings.SDKType);
            
            _settings.MuteAudioOnPause = EditorGUILayout.Toggle(
                "Mute Audio On Pause", 
                _settings.MuteAudioOnPause);
            
            _settings.AutoConstructAndInitializeSingleton = EditorGUILayout.Toggle(
                "Auto Singleton", 
                _settings.AutoConstructAndInitializeSingleton);
            
            _settings.LoadSaveOnInitialize = EditorGUILayout.Toggle(
                "Load Save On Initialize", 
                _settings.LoadSaveOnInitialize);
        }
        
        private void DrawBuildSettings()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Build Process Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                _settings.AspectRatio = (SDKSettings.AspectRatioMode)EditorGUILayout.EnumPopup(
                    "Aspect Ratio Mode", 
                    _settings.AspectRatio);
                
                if (_settings.AspectRatio != SDKSettings.AspectRatioMode.Default)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.BeginVertical(EditorStyles.textArea);
                    {
                        DrawBackgroundImageField();
                        DrawBackgroundPreview();
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private void DrawBackgroundImageField()
        {
            var newTexture = (Texture2D)EditorGUILayout.ObjectField(
                "Background Image", 
                _backgroundTexture, 
                typeof(Texture2D), 
                false);
            
            if (newTexture == _backgroundTexture) return;
            
            _backgroundTexture = newTexture;
            _settings.BackgroundImagePath = _backgroundTexture != null ? 
                GetResourcePath(AssetDatabase.GetAssetPath(_backgroundTexture)) : 
                string.Empty;
            
            _needsTextureReload = true;
        }
        
        private void DrawBackgroundPreview()
        {
            if (_backgroundTexture == null) return;
    
            EditorGUILayout.HelpBox("Recommended size: 1920x1080 or larger", MessageType.Info);
    
            EditorGUILayout.Space(5);
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
    
            var rect = GUILayoutUtility.GetRect(200, 120, GUILayout.ExpandWidth(false));
            GUI.Box(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4), 
                GUIContent.none, 
                EditorStyles.helpBox);
    
            EditorGUI.DrawPreviewTexture(rect, _backgroundTexture);
        }
        
        private void DrawEditorSettings()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
            
            DrawPlayerIdField();
            
            _settings.DebugDeviceType = (DeviceType)EditorGUILayout.EnumPopup(
                "Device Type", 
                _settings.DebugDeviceType);
            
            _settings.DebugLanguageCode = EditorGUILayout.TextField(
                "Language", 
                _settings.DebugLanguageCode);
            
            _settings.DebugInitializeTime = EditorGUILayout.Slider(
                "Initialization Time", 
                _settings.DebugInitializeTime, 0f, 10f);
            
            _settings.AdSimulationDuration = EditorGUILayout.Slider(
                "Ad Duration", 
                _settings.AdSimulationDuration, 0f, 10f);
            
            _settings.InterstitialAdReturnState = (InterstitialState)EditorGUILayout.EnumPopup(
                "Interstitial Result", 
                _settings.InterstitialAdReturnState);
            
            _settings.RewardedAdReturnState = (RewardedState)EditorGUILayout.EnumPopup(
                "Rewarded Result", 
                _settings.RewardedAdReturnState);
        }
        
        private void DrawPlayerIdField()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _settings.DebugPlayerId = EditorGUILayout.TextField("Player ID", _settings.DebugPlayerId);
        
                if (GUILayout.Button("Generate", GUILayout.Width(80)))
                {
                    _settings.DebugPlayerId = Guid.NewGuid().ToString();
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawAboutTab()
        {
            EditorGUILayout.LabelField("Version Information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Template Version: {TemplateVersion}");
            EditorGUILayout.LabelField($"Manifest Version: {_manifestVersion}");
            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("https://be4dies-organization.gitbook.io/welwisegamessdk/");
            }
        }
        
        private string GetResourcePath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return string.Empty;
            
            const string resourcesPrefix = "Assets/Resources/";
            if (!assetPath.StartsWith(resourcesPrefix))
            {
                Debug.LogError("Background image must be in Resources folder!");
                return string.Empty;
            }
            
            var path = assetPath.Substring(resourcesPrefix.Length);
            return Path.ChangeExtension(path, null);
        }
        
        public static void HandleSDKTypeChange(SupportedSDKType oldType, SupportedSDKType newType)
        {
            try
            {
                DeleteDirectory("WebGLTemplates/Welwise SDK");
                
                switch (oldType)
                {
                    case SupportedSDKType.WelwiseGames:
                        DeleteFile("Plugins/WebGL/welwise-sdk.jslib");
                        break;
                    case SupportedSDKType.YandexGames:
                        DeleteFile("Plugins/WebGL/yandex-games.jslib");
                        break;
                }

                switch (newType)
                {
                    case SupportedSDKType.WelwiseGames:
                        ImportPackage("welwise-games-template");
                        break;
                    case SupportedSDKType.YandexGames:
                        ImportPackage("yandex-games-template");
                        break;
                }

                UpdateWebGLTemplate();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing SDK change: {e.Message}");
            }
        }
        
        private void UpdateFilesForCurrentSDK()
        {
            Debug.Log($"Updating files for SDK: {_settings.SDKType}");
            HandleSDKTypeChange(_settings.SDKType, _settings.SDKType);
        }

        private static void DeleteFile(string relativePath)
        {
            var assetPath = Path.Combine("Assets", relativePath);
            if (!AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath)) 
                return;
            
            AssetDatabase.DeleteAsset(assetPath);
            return;
        }

        private static void DeleteDirectory(string relativePath)
        {
            var path = $"Assets/{relativePath}";
            if (!AssetDatabase.IsValidFolder(path)) return;
            
            AssetDatabase.DeleteAsset(path);
        }

        private static void ImportPackage(string packageName)
        {
            var guids = AssetDatabase.FindAssets(packageName);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileName(path) != $"{packageName}.unitypackage") 
                    continue;
                
                AssetDatabase.ImportPackage(path, false);
                return;
            }
            
            Debug.LogError($"Package {packageName}.unitypackage not found!");
        }

        private static void UpdateWebGLTemplate()
        {
            PlayerSettings.WebGL.template = "PROJECT:Welwise SDK";
            Debug.Log("WebGL template updated to 'Welwise SDK'");
        }

        private void SaveSettings()
        {
            if (_settings == null) return;
            _settings.Save();
        }
    }
}