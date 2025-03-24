using System;

namespace WelwiseGamesSDK.Shared
{
    public interface INeedInitializeService
    {
        public void Initialize();
        public event Action Initialized;
    }
}