using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class WebEnvironment : IEnvironment
    {
        public event Action Ready;
        
        public Guid PlayerId { get; private set; }
        public DeviceType DeviceType { get; private set; }
        public string LanguageCode { get; private set; }

        private int _loadedCount;
        private bool _hasErrors;
        private const int TotalProperties = 3;

        public WebEnvironment() { }

        public void Load()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ResetState();
            RequestPlayerId();
            RequestDeviceType();
            RequestLanguageCode();
#else
            Debug.LogWarning("[WebEnvironment] WebGL environment is only available in builds");
#endif
        }

        private void ResetState()
        {
            _loadedCount = 0;
            _hasErrors = false;
            PlayerId = Guid.Empty;
            LanguageCode = string.Empty;
        }
#if UNITY_WEBGL && !UNITY_EDITOR
        private void RequestPlayerId()
        {
            
            JsLibProvider.GetPlayerId(
                playerId =>
                {
                    if (!Guid.TryParse(playerId, out var guid))
                    {
                        HandleError($"Invalid PlayerId format: {playerId}");
                        return;
                    }
                    PlayerId = guid;
                    CheckCompletion();
                },
                HandleError);
        }

        private void RequestDeviceType()
        {
            JsLibProvider.GetDeviceType(
                deviceTypeStr =>
                {
                    if (!Enum.TryParse(deviceTypeStr, true, out DeviceType deviceType))
                    {
                        HandleError($"Unknown device type: {deviceTypeStr}");
                        return;
                    }
                    DeviceType = deviceType;
                    CheckCompletion();
                },
                HandleError);
        }

        private void RequestLanguageCode()
        {
            JsLibProvider.GetLanguageCode(
                language =>
                {
                    if (string.IsNullOrEmpty(language))
                    {
                        HandleError("Empty language code");
                        return;
                    }
                    LanguageCode = language;
                    CheckCompletion();
                },
                HandleError);
        }
#endif

        private void CheckCompletion()
        {
            if (_hasErrors) return;

            _loadedCount++;
            if (_loadedCount != TotalProperties) return;
            Debug.Log("[WebEnvironment] All environment data loaded");
            Ready?.Invoke();
        }

        private void HandleError(string error)
        {
            if (_hasErrors) return;
            
            _hasErrors = true;
            Debug.LogError($"[WebEnvironment] Initialization failed: {error}");
            ResetState();
        }

        public void RequestServerTime(Action<long> callback)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GetServerTime(
                s =>
                {
                    if (!long.TryParse(s, out var time))
                    {
                        Debug.LogError($"[WebEnvironment] Failed to parse server time: {s}");
                        callback?.Invoke(DateTime.Now.Ticks);
                        return;
                    }
                    callback?.Invoke(time);
                },
                error =>
                {
                    Debug.LogError($"[WebEnvironment] Server time error: {error}");
                    callback?.Invoke(DateTime.Now.Ticks);
                });
#else
            callback?.Invoke(DateTime.Now.Ticks);
#endif
        }
    }
}