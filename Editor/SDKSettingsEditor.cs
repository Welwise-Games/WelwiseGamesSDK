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
using System.Collections.Generic;

namespace WelwiseGames.Editor
{
    public class SDKSettingsEditor : EditorWindow
    {
        private const string PlayerNameKey = "WS_PLAYER_NAME__";
        private const string GamePrefix = "WS_SDK_GAME__";
        private const string MetaversePrefix = "WS_SDK_METAVERSE__";
        
        public const string TemplateVersion = "0.0.19";
        private const string LogoFileName = "__ws-logo";
        
        private enum TabType
        {
            General,
            Build,
            Advertisement,
            Environment,
            Payments,
            PlayerData,
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
        private bool _threeJsChanged = false;

        // Fields for new data entry
        private string _newKeyName = "";
        private string _newKeyValue = "";
        private int _newKeyTypeIndex = 3; // 0:int, 1:float, 2:bool, 3:string
        private int _newKeyContainerIndex = 0; // 0:Game, 1:Metaverse

        // PlayerData scroll position
        private Vector2 _playerDataScrollPosition;

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
    
            bool needsUpdate = false;
    
            // Проверка базовых файлов
            if (!AssetDatabase.IsValidFolder(templatePath) || 
                !File.Exists(Path.Combine(Application.dataPath, "WebGLTemplates/Welwise SDK/sdk-adapter.js")))
            {
                Debug.Log("Template files missing. Reimporting...");
                needsUpdate = true;
            }
    
            // Проверка ThreeJS файлов
            string threeCanvasPath = $"{templatePath}/threeCanvas.js";
            bool threeJsExists = File.Exists(threeCanvasPath);
    
            if (_settings.UseThreeJsLoader && !threeJsExists)
            {
                Debug.Log("ThreeJS files missing but required. Reimporting...");
                needsUpdate = true;
            }
            else if (!_settings.UseThreeJsLoader && threeJsExists)
            {
                Debug.Log("ThreeJS files present but not required. Removing...");
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                UpdateTemplateForCurrentSDK();
            }
        }

        private void UpdateTemplateForCurrentSDK()
        {
            WebGLTemplateUpdater.UpdateTemplate(_settings.SDKType, _settings.UseThreeJsLoader);
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
                    
                case TabType.PlayerData:
                    DrawPlayerDataTab();
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
            
            if (_threeJsChanged)
            {
                UpdateTemplateForCurrentSDK();
                MarkSettingsDirty();
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
                var p = _settings.UseThreeJsLoader;
                _settings.UseThreeJsLoader = EditorGUILayout.Toggle(
                    "Use ThreeJS Loader", 
                    _settings.UseThreeJsLoader);
                _threeJsChanged = p != _settings.UseThreeJsLoader;
        
                _settings.AspectRatio = (SDKSettings.AspectRatioMode)EditorGUILayout.EnumPopup(
                    "Aspect Ratio Mode", 
                    _settings.AspectRatio);
        
                if (_settings.AspectRatio != SDKSettings.AspectRatioMode.Default)
                {
                    DrawBackgroundImageField();
                    DrawBackgroundPreview();
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
        
        private void HandleSDKTypeChange(SupportedSDKType oldType, SupportedSDKType newType)
        {
            try
            {
                WebGLTemplateUpdater.UpdateTemplate(newType, _settings.UseThreeJsLoader);
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing SDK change: {e.Message}");
            }
        }
        
        private void DrawPlayerDataTab()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Player Data Settings", EditorStyles.boldLabel);

            // Перенесена настройка из General
            _settings.LoadSaveOnInitialize = EditorGUILayout.Toggle(
                "Load Save On Initialize", 
                _settings.LoadSaveOnInitialize);

            EditorGUILayout.Space(15);
            
            // Кнопка удаления всех данных
            if (GUILayout.Button("Delete All Player Data", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Delete All Player Data", 
                    "Are you sure you want to delete ALL SDK player data? This cannot be undone.", 
                    "Delete", "Cancel"))
                {
                    DeleteAllPlayerData();
                }
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Saved Data", EditorStyles.boldLabel);

            // Отображение сохраненных данных с прокруткой
            _playerDataScrollPosition = EditorGUILayout.BeginScrollView(_playerDataScrollPosition, GUILayout.ExpandHeight(true));
            DrawSavedPlayerData();
            EditorGUILayout.EndScrollView();

            // Добавление новых полей
            if (!Application.isPlaying)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField("Add New Field", EditorStyles.boldLabel);
                DrawNewFieldUI();
            }
        }

        private void DeleteAllPlayerData()
        {
            // Удаление всех ключей SDK
            PlayerPrefs.DeleteKey(PlayerNameKey);
            
            DeleteKeysForPrefix(GamePrefix);
            DeleteKeysForPrefix(MetaversePrefix);
            
            PlayerPrefs.Save();
            Debug.Log("All SDK player data deleted");
        }

        private void DeleteKeysForPrefix(string prefix)
        {
            string keysKey = prefix + "__keys";
            string keys = PlayerPrefs.GetString(keysKey, "");
            
            if (!string.IsNullOrEmpty(keys))
            {
                foreach (string key in keys.Split(','))
                {
                    PlayerPrefs.DeleteKey(prefix + key);
                }
            }
            
            PlayerPrefs.DeleteKey(keysKey);
        }

        private void DrawSavedPlayerData()
        {
            // Player Name
            DrawPlayerNameSection();
            
            // Game Data
            DrawDataContainer(GamePrefix, "Game Data");
            
            // Metaverse Data
            DrawDataContainer(MetaversePrefix, "Metaverse Data");
        }

        private void DrawPlayerNameSection()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Player Name", EditorStyles.miniBoldLabel);
            
            string currentName = PlayerPrefs.GetString(
                PlayerNameKey, 
                "Ghost"
            );
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (Application.isPlaying)
                {
                    EditorGUILayout.LabelField(currentName);
                }
                else
                {
                    string newName = EditorGUILayout.TextField(currentName);
                    if (newName != currentName)
                    {
                        PlayerPrefs.SetString(
                            PlayerNameKey, 
                            newName
                        );
                        PlayerPrefs.Save();
                    }
                }
                
                if (!Application.isPlaying && GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    PlayerPrefs.DeleteKey(PlayerNameKey);
                    PlayerPrefs.Save();
                }
            }
        }

        private void DrawDataContainer(string prefix, string label)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);
            
            string keysKey = prefix + "__keys";
            string keysStr = PlayerPrefs.GetString(keysKey, "");
            
            if (string.IsNullOrEmpty(keysStr))
            {
                EditorGUILayout.LabelField("No data");
                return;
            }
            
            foreach (string key in keysStr.Split(','))
            {
                string fullKey = prefix + key;
                if (!PlayerPrefs.HasKey(fullKey)) continue;
                
                string valueStr = PlayerPrefs.GetString(fullKey);
                char valueType = valueStr.Length > 0 ? valueStr[0] : ' ';
                string valueContent = valueStr.Length > 1 ? valueStr.Substring(1) : "";
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Отображение ключа
                    EditorGUILayout.LabelField(key, GUILayout.Width(150));
                    
                    // Отображение и редактирование значения
                    if (Application.isPlaying)
                    {
                        EditorGUILayout.LabelField(valueContent);
                    }
                    else
                    {
                        string newValue = DrawValueField(valueType, valueContent);
                        
                        if (newValue != valueContent)
                        {
                            PlayerPrefs.SetString(
                                fullKey, 
                                valueType + newValue
                            );
                            PlayerPrefs.Save();
                        }
                    }
                    
                    // Кнопка удаления
                    if (!Application.isPlaying && 
                        GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        PlayerPrefs.DeleteKey(fullKey);
                        
                        // Обновление списка ключей
                        List<string> keys = new List<string>(keysStr.Split(','));
                        keys.Remove(key);
                        PlayerPrefs.SetString(
                            keysKey, 
                            string.Join(",", keys)
                        );
                        
                        PlayerPrefs.Save();
                        return; // Прервать для обновления интерфейса
                    }
                }
            }
        }

        private string DrawValueField(char valueType, string currentValue)
        {
            switch (valueType)
            {
                case 'i': // int
                    int intVal;
                    int.TryParse(currentValue, out intVal);
                    return EditorGUILayout.IntField(intVal).ToString();
                
                case 'f': // float
                    float floatVal;
                    float.TryParse(
                        currentValue, 
                        System.Globalization.NumberStyles.Float, 
                        System.Globalization.CultureInfo.InvariantCulture, 
                        out floatVal
                    );
                    return EditorGUILayout.FloatField(floatVal)
                        .ToString(System.Globalization.CultureInfo.InvariantCulture);
                
                case 'b': // bool
                    bool boolVal;
                    bool.TryParse(currentValue, out boolVal);
                    return EditorGUILayout.Toggle(boolVal).ToString();
                
                default: // string
                    return EditorGUILayout.TextField(currentValue);
            }
        }

        private void DrawNewFieldUI()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                // Выбор контейнера
                _newKeyContainerIndex = EditorGUILayout.Popup(
                    "Container",
                    _newKeyContainerIndex,
                    new[] { "Game Data", "Metaverse Data" }
                );
                
                // Имя ключа
                _newKeyName = EditorGUILayout.TextField("Key Name", _newKeyName);
                
                // Тип значения
                _newKeyTypeIndex = EditorGUILayout.Popup(
                    "Value Type",
                    _newKeyTypeIndex,
                    new[] { "Integer", "Float", "Boolean", "String" }
                );
                
                // Значение
                _newKeyValue = EditorGUILayout.TextField("Value", _newKeyValue);
                
                // Кнопка добавления
                if (GUILayout.Button("Add Field") && !string.IsNullOrEmpty(_newKeyName))
                {
                    AddNewField();
                }
            }
        }

        private void AddNewField()
        {
            string prefix = _newKeyContainerIndex == 0 ? 
                GamePrefix : 
                MetaversePrefix;
            
            string fullKey = prefix + _newKeyName;
            
            // Проверка существования ключа
            if (PlayerPrefs.HasKey(fullKey))
            {
                Debug.LogWarning($"Key '{_newKeyName}' already exists!");
                return;
            }
            
            // Форматирование значения
            char typePrefix = _newKeyTypeIndex switch
            {
                0 => 'i', // int
                1 => 'f', // float
                2 => 'b', // bool
                _ => 's'  // string
            };
            
            // Сохранение значения
            PlayerPrefs.SetString(fullKey, typePrefix + _newKeyValue);
            
            // Обновление списка ключей
            string keysKey = prefix + "__keys";
            string existingKeys = PlayerPrefs.GetString(keysKey, "");
            
            List<string> keys = new List<string>();
            if (!string.IsNullOrEmpty(existingKeys))
            {
                keys.AddRange(existingKeys.Split(','));
            }
            
            if (!keys.Contains(_newKeyName))
            {
                keys.Add(_newKeyName);
                PlayerPrefs.SetString(keysKey, string.Join(",", keys));
            }
            
            PlayerPrefs.Save();
            
            // Сброс полей
            _newKeyName = "";
            _newKeyValue = "";
        }
    }
}