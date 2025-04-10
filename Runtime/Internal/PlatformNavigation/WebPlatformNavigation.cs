using System.Runtime.InteropServices;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal class WebPlatformNavigation : IPlatformNavigation
    {
        [DllImport("__Internal")]
        private static extern void ChangeGame(string id);


        public void GoToGame(string id) => ChangeGame(id);
    }
}