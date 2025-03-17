using System;

namespace WelwiseGamesSDK.Internal
{
    public interface IWaitService
    {
        public event Action Ready;
    }
}