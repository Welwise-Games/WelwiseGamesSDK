using System.Runtime.InteropServices;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal class WebPlatformNavigation : IPlatformNavigation
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void ChangeGame(string id);
#endif


        public void GoToGame(string id)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            ChangeGame(id);
#else
            Debug.Log("[WebPlatformNavigation] Going to " + id);
#endif
        }
    }
}