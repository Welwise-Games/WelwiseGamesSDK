namespace WelwiseGamesSDK.Shared.Types
{
    /// <summary>
    /// Specifies supported SDK platform implementations.
    /// Used in SDKSettings to configure runtime behavior.
    /// </summary>
    public enum SupportedSDKType
    {
        /// <summary>Native Welwise Games platform</summary>
        WelwiseGames,
        /// <summary>Yandex Games Web platform</summary>
        YandexGames,
        /// <summary>Game Distribution Web platform</summary>
        GameDistribution,
        /// <summary>Y8 Games Web platform</summary>
        Y8Games,
        /// <summary>PlayGamma Web platform</summary>
        PlayGamma,
    }
}