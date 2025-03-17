using System;

namespace WelwiseGamesSDK.Internal.Environment
{
    internal sealed class DebugEnvironment : IEnvironment
    {
        public Guid PlayerId { get; }
        public bool UseMetaverse { get; }

        public DebugEnvironment(string playerId, bool useMetaverse)
        {
            PlayerId = !string.IsNullOrEmpty(playerId) ? Guid.Parse(playerId) : Guid.NewGuid();
            UseMetaverse = useMetaverse;
        }
    }
}