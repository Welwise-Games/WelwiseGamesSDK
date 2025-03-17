namespace WelwiseGamesSDK.Internal.Saves
{
    internal static class SavesFactory
    {
        public static ISaves CreateGameSaves(WebSender webSender)
        {
#if UNITY_EDITOR
            return new PlayerPrefsSaves(webSender, false);
#else
            return new GamePlatformSaves(webSender);
#endif
        }

        public static ISaves CreateMetaverseGameSaves(WebSender webSender)
        {
#if UNITY_EDITOR
            return new PlayerPrefsSaves(webSender, true);
#else
            return new MetaversePlatformSaves(webSender);
#endif
        }
    }
}