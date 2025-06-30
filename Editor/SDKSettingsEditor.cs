using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditorInternal;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Types;
using DeviceType = WelwiseGamesSDK.Shared.Types.DeviceType;

namespace WelwiseGames.Editor
{
    public class SDKSettingsEditor : EditorWindow
    {
        public const string TemplateVersion = "0.0.11";
        private const string LogoFileName = "__ws-logo";
        
        private enum TabType
        {
            General,
            Build,
            Advertisement,
            Environment,
            Payments,
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
        
        private bool _settingsDirty;
        private double _lastChangeTime;
        private const double SaveDelay = 1.0;

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
            
            _settingsDirty = false;
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
            SaveIfDirtyImmediate();
        }

        private void OnLostFocus()
        {
            SaveIfDirtyImmediate();
        }

        private void InitializeLists()
        {
            _productsList = new ReorderableList(_settings.MockProducts, typeof(Product), true, true, true, true)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Products"),
                    drawElementCallback = (rect, index, _, _) => DrawProductElement(rect, index),
                    elementHeight = EditorGUIUtility.singleLineHeight * 7,
                    onRemoveCallback = list => 
                    {
                        _settings.MockProducts.RemoveAt(list.index);
                        MarkSettingsDirty();
                    }
                };

            _purchasesList = new ReorderableList(_settings.MockPurchases, typeof(Purchase), true, true, true, true)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Purchases"),
                    drawElementCallback = (rect, index, _, _) => DrawPurchaseElement(rect, index),
                    elementHeight = EditorGUIUtility.singleLineHeight * 4,
                    onRemoveCallback = list => 
                    {
                        _settings.MockPurchases.RemoveAt(list.index);
                        MarkSettingsDirty();
                    }
                };
        }
        
        private void MarkSettingsDirty()
        {
            _settingsDirty = true;
            _lastChangeTime = EditorApplication.timeSinceStartup;
            EditorApplication.delayCall -= DelayedSave;
            EditorApplication.delayCall += DelayedSave;
        }

        private void DelayedSave()
        {
            if (_settingsDirty && EditorApplication.timeSinceStartup - _lastChangeTime >= SaveDelay)
            {
                SaveIfDirtyImmediate();
            }
        }

        private void SaveIfDirtyImmediate()
        {
            if (_settingsDirty)
            {
                SaveSettingsImmediate();
                _settingsDirty = false;
            }
        }

        private void SaveSettingsImmediate()
        {
            if (_settings == null) return;
            _settings.Save();
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

        private void DrawPaymentsSettings()
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Payments Settings", EditorStyles.boldLabel);
            
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
                UpdateTemplateForCurrentSDK();
                _settings.InstalledTemplateVersion = TemplateVersion;
                SaveSettingsImmediate();
            }
            else
            {
                EnsureTemplateFilesExist();
            }
        }

        private void EnsureTemplateFilesExist()
        {
            string templatePath = "Assets/WebGLTemplates/Welwise SDK";
            string adapterPath = $"{templatePath}/sdk-adapter.js";
            
            if (!AssetDatabase.IsValidFolder(templatePath) || 
                !File.Exists(Path.Combine(Application.dataPath, "WebGLTemplates/Welwise SDK/sdk-adapter.js")))
            {
                Debug.Log("Template files missing. Reimporting...");
                UpdateTemplateForCurrentSDK();
            }
        }

        private void UpdateTemplateForCurrentSDK()
        {
            WebGLTemplateUpdater.UpdateTemplate(_settings.SDKType);
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
                    DrawGeneralSettings();
                    break;
                    
                case TabType.Build:
                    DrawBuildSettings();
                    break;
                    
                case TabType.Advertisement:
                    DrawAdvertisementSettings();
                    break;
                    
                case TabType.Environment:
                    DrawEnvironmentSettings();
                    break;
                    
                case TabType.Payments:
                    DrawPaymentsSettings();
                    break;
                    
                case TabType.About:
                    DrawAboutTab();
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                MarkSettingsDirty();
            }

            if (_settings.SDKType != _lastSDKType)
            {
                HandleSDKTypeChange(_lastSDKType, _settings.SDKType);
                _lastSDKType = _settings.SDKType;
                SaveSettingsImmediate();
            }
        }
        
        private void DrawHeader()
        {
            const int headerHeight = 32;
    
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
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
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

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    var tabs = Enum.GetNames(typeof(TabType));
                    float tabWidth = (EditorGUIUtility.currentViewWidth - 180) / tabs.Length;
            
                    for (int i = 0; i < tabs.Length; i++)
                    {
                        var style = new GUIStyle(tabStyle);
                        if ((int)_currentTab == i)
                        {
                            style.normal.background = Texture2D.grayTexture;
                        }
                
                        if (GUILayout.Button(tabs[i], style, GUILayout.Width(tabWidth)))
                        {
                            _currentTab = (TabType)i;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawGeneralSettings()
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
            
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);
    
            _settings.DebugInitializeTime = EditorGUILayout.Slider(
                "Initialization Time", 
                _settings.DebugInitializeTime, 0f, 10f);

            EditorGUILayout.LabelField("Module Availability:");
    
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                _settings.EditorAdvertisementModule = EditorGUILayout.ToggleLeft(
                    "Advertisement Module", 
                    _settings.EditorAdvertisementModule);
        
                _settings.EditorPaymentsModule = EditorGUILayout.ToggleLeft(
                    "Payments Module", 
                    _settings.EditorPaymentsModule);
        
                _settings.EditorAnalyticsModule = EditorGUILayout.ToggleLeft(
                    "Analytics Module", 
                    _settings.EditorAnalyticsModule);
        
                _settings.EditorEnvironmentModule = EditorGUILayout.ToggleLeft(
                    "Environment Module", 
                    _settings.EditorEnvironmentModule);
        
                _settings.EditorPlatformNavigationModule = EditorGUILayout.ToggleLeft(
                    "Platform Navigation Module", 
                    _settings.EditorPlatformNavigationModule);
        
                _settings.EditorPlayerDataModule = EditorGUILayout.ToggleLeft(
                    "Player Data Module", 
                    _settings.EditorPlayerDataModule);
        
                _settings.EditorGameDataModule = EditorGUILayout.ToggleLeft(
                    "Game Data Module", 
                    _settings.EditorGameDataModule);
        
                _settings.EditorMetaverseDataModule = EditorGUILayout.ToggleLeft(
                    "Metaverse Data Module", 
                    _settings.EditorMetaverseDataModule);
            }
            EditorGUILayout.EndVertical();
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
            MarkSettingsDirty();
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
        
        private void DrawAdvertisementSettings()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Advertisement Settings", EditorStyles.boldLabel);
            
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
        
        private void DrawEnvironmentSettings()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Environment Settings", EditorStyles.boldLabel);
            
            DrawPlayerIdField();
            
            _settings.DebugDeviceType = (DeviceType)EditorGUILayout.EnumPopup(
                "Device Type", 
                _settings.DebugDeviceType);
            
            _settings.DebugLanguageCode = EditorGUILayout.TextField(
                "Language", 
                _settings.DebugLanguageCode);
        }
        
        private void DrawPlayerIdField()
        {
            EditorGUILayout.BeginHorizontal();
            {
                _settings.DebugPlayerId = EditorGUILayout.TextField("Player ID", _settings.DebugPlayerId);
        
                if (GUILayout.Button("Generate", GUILayout.Width(80)))
                {
                    _settings.DebugPlayerId = Guid.NewGuid().ToString();
                    MarkSettingsDirty();
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
                WebGLTemplateUpdater.UpdateTemplate(newType);
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing SDK change: {e.Message}");
            }
        }
    }
}