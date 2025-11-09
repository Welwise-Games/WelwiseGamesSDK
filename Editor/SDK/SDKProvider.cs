using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace WelwiseGames.Editor.SDK
{
    public static class SDKProvider
    {
        private const string SDKsFolder = "SDKs";
        private static Dictionary<string, SDKDefinition> _sdkDefinitionsCache = new Dictionary<string, SDKDefinition>();
        private static Dictionary<string, string> _adapterCodeCache = new Dictionary<string, string>();
        private static Dictionary<string, string> _injectionCodeCache = new Dictionary<string, string>();
        private static bool _isCacheInitialized = false;

        [UnityEditor.InitializeOnLoadMethod]
        private static void InitializeCache()
        {
            if (_isCacheInitialized) return;

            try
            {
                // Pre-load all SDK definitions at editor startup
                var definitionFiles = Resources.LoadAll<TextAsset>(SDKsFolder)
                    .Where(textAsset => textAsset.name.EndsWith("_defenition"))
                    .ToArray();

                foreach (var definitionFile in definitionFiles)
                {
                    try
                    {
                        var definition = JsonConvert.DeserializeObject<SDKDefinition>(definitionFile.text);
                        if (definition != null && !string.IsNullOrEmpty(definition.Name))
                        {
                            _sdkDefinitionsCache[definition.Name] = definition;
                            
                            // Pre-load adapter code for this SDK
                            var adapterResourcePath = $"{SDKsFolder}/{definition.Name}_adapter";
                            var adapterAsset = Resources.Load<TextAsset>(adapterResourcePath);
                            if (adapterAsset != null)
                            {
                                _adapterCodeCache[definition.Name] = adapterAsset.text;
                            }

                            // Pre-load injection scripts for this SDK
                            foreach (var script in definition.PostBuildScripts)
                            {
                                if (!string.IsNullOrEmpty(script.File))
                                {
                                    var injectionFileName = Path.GetFileNameWithoutExtension(script.File);
                                    var injectionResourcePath = $"{SDKsFolder}/{injectionFileName}";
                                    var injectionAsset = Resources.Load<TextAsset>(injectionResourcePath);
                                    if (injectionAsset != null)
                                    {
                                        var cacheKey = $"{definition.Name}_{script.InjectPoint}";
                                        _injectionCodeCache[cacheKey] = injectionAsset.text;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to cache SDK definition from {definitionFile.name}: {ex.Message}");
                    }
                }

                _isCacheInitialized = true;
                Debug.Log($"SDKProvider cache initialized with {_sdkDefinitionsCache.Count} SDKs");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error initializing SDKProvider cache: {ex.Message}");
            }
        }

        public static string[] GetSDKNames()
        {
            EnsureCacheInitialized();
            return _sdkDefinitionsCache.Keys.ToArray();
        }

        public static SDKDefinition GetSDKDefinition(string sdkName)
        {
            if (string.IsNullOrEmpty(sdkName))
            {
                Debug.LogError("SDK name cannot be null or empty");
                return null;
            }

            EnsureCacheInitialized();

            if (_sdkDefinitionsCache.TryGetValue(sdkName, out var definition))
            {
                return definition;
            }

            Debug.LogError($"SDK definition not found in cache: {sdkName}");
            return null;
        }

        public static string GetAdapterCode(string sdkName)
        {
            if (string.IsNullOrEmpty(sdkName))
            {
                Debug.LogError("SDK name cannot be null or empty");
                return string.Empty;
            }

            EnsureCacheInitialized();

            if (_adapterCodeCache.TryGetValue(sdkName, out var adapterCode))
            {
                return adapterCode;
            }

            Debug.LogError($"Adapter code not found in cache for SDK: {sdkName}");
            return string.Empty;
        }

        public static string GetInjectionCode(PostBuildScript postBuildScript)
        {
            if (postBuildScript == null)
            {
                Debug.LogError("PostBuildScript cannot be null");
                return string.Empty;
            }

            if (string.IsNullOrEmpty(postBuildScript.File))
            {
                Debug.LogError("PostBuildScript File property cannot be null or empty");
                return string.Empty;
            }

            EnsureCacheInitialized();

            // Extract SDK name from filename (e.g., "Y8_injection-head" -> "Y8")
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(postBuildScript.File);
            var parts = fileNameWithoutExtension.Split(new[] { "_injection-" }, StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 2)
            {
                Debug.LogError($"Invalid injection file name format: {postBuildScript.File}");
                return string.Empty;
            }

            var sdkName = parts[0];
            var cacheKey = $"{sdkName}_{postBuildScript.InjectPoint}";

            if (_injectionCodeCache.TryGetValue(cacheKey, out var injectionCode))
            {
                return injectionCode;
            }

            Debug.LogError($"Injection code not found in cache for: {cacheKey}");
            return string.Empty;
        }

        private static void EnsureCacheInitialized()
        {
            if (!_isCacheInitialized)
            {
                InitializeCache();
            }
        }

        // Public methods for cache management
        public static void ReloadCache()
        {
            ClearCache();
            InitializeCache();
        }

        public static void ClearCache()
        {
            _sdkDefinitionsCache.Clear();
            _adapterCodeCache.Clear();
            _injectionCodeCache.Clear();
            _isCacheInitialized = false;
        }

        public static void PreloadSpecificSDK(string sdkName)
        {
            if (string.IsNullOrEmpty(sdkName)) return;

            try
            {
                // Load definition
                var definitionResourcePath = $"{SDKsFolder}/{sdkName}_defenition";
                var definitionAsset = Resources.Load<TextAsset>(definitionResourcePath);
                if (definitionAsset != null)
                {
                    var definition = JsonConvert.DeserializeObject<SDKDefinition>(definitionAsset.text);
                    if (definition != null)
                    {
                        _sdkDefinitionsCache[sdkName] = definition;

                        // Load adapter
                        var adapterResourcePath = $"{SDKsFolder}/{sdkName}_adapter";
                        var adapterAsset = Resources.Load<TextAsset>(adapterResourcePath);
                        if (adapterAsset != null)
                        {
                            _adapterCodeCache[sdkName] = adapterAsset.text;
                        }

                        // Load injections
                        foreach (var script in definition.PostBuildScripts)
                        {
                            if (!string.IsNullOrEmpty(script.File))
                            {
                                var injectionFileName = Path.GetFileNameWithoutExtension(script.File);
                                var injectionResourcePath = $"{SDKsFolder}/{injectionFileName}";
                                var injectionAsset = Resources.Load<TextAsset>(injectionResourcePath);
                                if (injectionAsset != null)
                                {
                                    var cacheKey = $"{sdkName}_{script.InjectPoint}";
                                    _injectionCodeCache[cacheKey] = injectionAsset.text;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error preloading SDK {sdkName}: {ex.Message}");
            }
        }
    }
}