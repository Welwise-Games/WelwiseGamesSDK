namespace WelwiseGamesSDK.Shared
{
    public interface ISDKConfig
    {
        public SDKMode Mode { get; }
        public string DebugPlayerId { get; }
        public string GameId { get; }
        public float SyncDelay { get; }
        public string ApiAuthKey { get; }
        public string MetaverseId { get; }
        public bool IsMetaverseConnected { get; }
    }
}