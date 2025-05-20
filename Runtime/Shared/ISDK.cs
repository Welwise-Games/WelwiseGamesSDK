using System;

namespace WelwiseGamesSDK.Shared
{
    public interface ISDK
    {
        public IPlayerData PlayerData { get; }
        public IEnvironment Environment { get; }
        public IAdvertisement Advertisement { get; }
        public IAnalytics Analytics { get; }
        public IPlatformNavigation PlatformNavigation { get; }
        
        public void Initialize();
        public event Action Initialized;
        public  bool IsInitialized { get; }
    }
}