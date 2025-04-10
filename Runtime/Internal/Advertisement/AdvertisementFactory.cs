using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal static class AdvertisementFactory
    {
        public static IAdvertisement Create(SDKSettings settings)
        {
            return settings.Mode switch
            {
                SDKMode.Development or SDKMode.Production => WebAdvertisement.Create(),
                SDKMode.Debug => DebugAdvertisement.Create(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}