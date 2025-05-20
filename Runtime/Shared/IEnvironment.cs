using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IEnvironment
    {
        public Guid PlayerId { get; }
        public DeviceType DeviceType { get; }
        public string LanguageCode { get; }
        public void RequestServerTime(Action<long> callback);
    }
}