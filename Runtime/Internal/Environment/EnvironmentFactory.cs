using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal static class EnvironmentFactory
    {
        public static IEnvironment Create(SDKSettings sdkSettings)
        {
#if UNITY_EDITOR
            DeviceInfo.InitializeFallback(sdkSettings.DebugDeviceType switch
                {
                    DeviceType.Desktop => 0,
                    DeviceType.Mobile => 1,
                    DeviceType.Tablet => 2,
                    _ => throw new ArgumentOutOfRangeException()
                }, 
                sdkSettings.DebugLanguageCode
                );
#endif
            
            return sdkSettings.Mode switch
            {
                SDKMode.Development or SDKMode.Production => new WebEnvironment(),
                SDKMode.Debug => new DebugEnvironment(sdkSettings.DebugPlayerId, sdkSettings.DebugDeviceType,
                    sdkSettings.DebugLanguageCode),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}