using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class WebEnvironment : IEnvironment
    {
        private const string IdKey = "WS_PLAYER_ID";

        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string LanguageCode { get; }

        public WebEnvironment()
        {
            var id = CookieHandler.LoadData(IdKey);
            if (string.IsNullOrEmpty(id))
            {
                PlayerId = Guid.NewGuid();
                Debug.LogWarning("[Environment] Player ID was not found in cookie, create new one.");
                CookieHandler.SaveData(IdKey, PlayerId.ToString());
            }
            else
            {
                PlayerId = Guid.Parse(id);
            }
            DeviceType = DeviceInfo.DetectDeviceType() switch
            {
                "Desktop" => DeviceType.Desktop,
                "Mobile" => DeviceType.Desktop,
                "Tablet" => DeviceType.Tablet,
                _ => throw new ArgumentOutOfRangeException()
            };
            LanguageCode = DeviceInfo.DetectLanguage();
        }
    }
}