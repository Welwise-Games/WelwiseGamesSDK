
using System.Collections.Generic;

namespace WelwiseGamesSDK.Shared
{
    public interface ISDK
    {
        public ISaves GameSaves { get; }
        public ISaves MetaverseSaves { get; }
        public ISDKConfig Config { get; }
        public IEnvironment Environment { get; }
        public IGameSessionTracker GameSessionTracker { get; }
        public IAdvertisement Advertisement { get; }
        public List<INeedInitializeService> NeedInitializeServices { get; }
    }
}