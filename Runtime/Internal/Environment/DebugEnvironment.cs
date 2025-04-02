using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class DebugEnvironment : IEnvironment
    {
        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string LanguageCode { get; }

        public DebugEnvironment(string playerId, DeviceType deviceType, string languageCode)
        {
            PlayerId = !string.IsNullOrEmpty(playerId) ? Guid.Parse(playerId) : Guid.NewGuid();
            DeviceType = deviceType;
            LanguageCode = languageCode;
        }
    }
}