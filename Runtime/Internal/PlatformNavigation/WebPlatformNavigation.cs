using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class WebPlatformNavigation : IPlatformNavigation
    {
        public void GoToGame(int id, Action<string> onError) =>
            PluginRuntime.GoToGame(id, ()=>{},onError);
    }
}