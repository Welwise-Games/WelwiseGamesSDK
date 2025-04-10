using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal static class AnalyticsFactory
    {
        public static IAnalytics Create(SDKSettings settings)
        {
            return settings.Mode switch
            {
                SDKMode.Development or SDKMode.Production => new WelwiseAnalytics(),
                SDKMode.Debug => new DebugAnalytics(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}