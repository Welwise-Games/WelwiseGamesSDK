using System;
using WelwiseGamesSDK.Shared;
using DeviceType = WelwiseGamesSDK.Shared.DeviceType;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class UnityEnvironment : IEnvironment
    {
        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string LanguageCode { get; }

        public UnityEnvironment(string playerId, DeviceType deviceType, string languageCode)
        {
            if (string.IsNullOrEmpty(playerId) || !Guid.TryParse(playerId, out var id))
            {
                PlayerId = Guid.NewGuid();
            }
            else
            {
                PlayerId = id;
            }
            
            DeviceType = deviceType;
            LanguageCode = languageCode;
        }
        
        public void RequestServerTime(Action<long> callback)
        {
            callback?.Invoke(DateTime.Now.Ticks);
        }
        
    }
}