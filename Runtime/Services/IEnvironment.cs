using System;

namespace WelwiseGamesSDK
{
    public interface IEnvironment
    {
        public Guid PlayerId { get; }
        public bool UseMetaverse { get; }
    }
}