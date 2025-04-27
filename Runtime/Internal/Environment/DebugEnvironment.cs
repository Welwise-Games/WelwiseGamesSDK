using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class DebugEnvironment : IEnvironment
    {
        private const string IdKey = "WS_PLAYER_ID";

        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string LanguageCode { get; }

        public DebugEnvironment(string playerId, DeviceType deviceType, string languageCode)
        {
            #if UNITY_EDITOR
            PlayerId = !string.IsNullOrEmpty(playerId) ? Guid.Parse(playerId) : Guid.NewGuid();
            DeviceType = deviceType;
            LanguageCode = languageCode;
            #endif
            
            #if UNITY_WEBGL && !UNITY_EDITOR
            var id = CookieHandler.LoadData(IdKey);
            if (string.IsNullOrEmpty(id))
            {
                PlayerId = Guid.NewGuid();
                Debug.LogWarning("[Environment] Player ID was not found in cookie, create new one.");
                CookieHandler.SaveData(IdKey, PlayerId.ToString());
            }
            else
            {
                if (!Guid.TryParse(id, out var parsedId))
                {
                    parsedId = Guid.NewGuid();
                    Debug.LogWarning("[Environment] Invalid GUID in cookie, generated new one.");
                }
                PlayerId = parsedId;
            }

            var deviceTypeInt = DeviceInfo.DetectDeviceType();
            switch (deviceTypeInt)
            {
                case 0:
                    DeviceType = DeviceType.Desktop;
                    break;
                case 1:
                    DeviceType = DeviceType.Mobile;
                    break;
                case 2:
                    DeviceType = DeviceType.Tablet;
                    break;
                default:
                    Debug.LogError($"[WebEnvironment] Unknown device type: {deviceTypeInt}, setting to default values ( DeviceType.Desktop).");
                    DeviceType = DeviceType.Desktop;
                    return;
            }

            LanguageCode = DeviceInfo.DetectLanguage();
            #endif
        }
    }
}