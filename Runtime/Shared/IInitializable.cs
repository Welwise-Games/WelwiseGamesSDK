using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IInitializable : IModule
    {
        public void Initialize();
        public bool IsInitialized { get; }
        public event Action Initialized; 
    }
}