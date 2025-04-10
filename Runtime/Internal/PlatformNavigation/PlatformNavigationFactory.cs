using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal static class PlatformNavigationFactory
    {
        public static IPlatformNavigation Create(SDKSettings settings)
        {
            return settings.Mode switch
            {
                SDKMode.Development or SDKMode.Production => new WebPlatformNavigation(),
                SDKMode.Debug => new DebugPlatformNavigation(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}