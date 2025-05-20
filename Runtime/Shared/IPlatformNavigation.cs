using System;

namespace WelwiseGamesSDK.Shared
{
    public interface IPlatformNavigation
    {
        public void GoToGame(int id, Action<string> onError);
    }
}