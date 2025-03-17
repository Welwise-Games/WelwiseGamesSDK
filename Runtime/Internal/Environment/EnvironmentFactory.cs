namespace WelwiseGamesSDK.Internal.Environment
{
    internal static class EnvironmentFactory
    {
        public static IEnvironment Create(bool useDebugId, string playerId, bool useMetaverse)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!useDebugId) return new WebEnvironment(useMetaverse);
            else return new DebugEnvironment(playerId, useMetaverse);
#else
            return new DebugEnvironment(playerId, useMetaverse);
#endif
        }
    }
}