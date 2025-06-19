using System;
using System.Security.Cryptography;
using System.Text;
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
        public string Language { get; private set; }

        private int _loadedCount;
        private bool _hasErrors;
        private const int TotalProperties = 3;
        
        public void Load()
        {
            ResetState();
            RequestPlayerId();
            RequestDeviceType();
            RequestLanguageCode();
        }

        private void ResetState()
        {
            _loadedCount = 0;
            _hasErrors = false;
            PlayerId = Guid.Empty;
            Language = string.Empty;
        }

        private void RequestPlayerId()
        {
            PluginRuntime.GetPlayerId(
                playerId =>
                {
                    Debug.Log("[WebEnvironment] Get player id");
                    if (Guid.TryParse(playerId, out var guid))
                    {
                        PlayerId = guid;
                    }
                    else
                    {
                        try
                        {
                            byte[] data;
                            try
                            {
                                data = Convert.FromBase64String(playerId);
                            }
                            catch
                            {
                                data = Encoding.UTF8.GetBytes(playerId);
                            }
                            
                            using (var md5 = MD5.Create())
                            {
                                byte[] hash = md5.ComputeHash(data);
                                guid = new Guid(hash);
                            }
                            PlayerId = guid;
                        }
                        catch (Exception ex)
                        {
                            HandleError($"Failed to convert PlayerId to Guid: {ex.Message}");
                            return;
                        }
                    }
                    CheckCompletion();
                },
                HandleError);
        }

        private void RequestDeviceType()
        {
            PluginRuntime.GetDeviceType(
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
            PluginRuntime.GetLanguageCode(
                language =>
                {
                    if (string.IsNullOrEmpty(language))
                    {
                        HandleError("Empty language code");
                        return;
                    }
                    Language = language;
                    CheckCompletion();
                },
                HandleError);
        }

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
            PluginRuntime.GetServerTime(
                s =>
                {
                    Debug.Log("[WebEnvironment] Get server time");
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
        }
    }
}