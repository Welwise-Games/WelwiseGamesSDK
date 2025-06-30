using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IInitializable
    {
        public void Initialize();
        public bool IsInitialized { get; }
        public event Action Initialized; 
    }
}