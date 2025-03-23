using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IEnvironment
    {
        public Guid PlayerId { get; }
        public bool UseMetaverse { get; }
    }
}