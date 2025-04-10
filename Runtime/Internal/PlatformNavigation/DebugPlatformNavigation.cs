using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class DebugPlatformNavigation : IPlatformNavigation
    {
        public void GoToGame(string id) => Debug.Log($"[{nameof(DebugPlatformNavigation)}] Call go to game {id}");
    }
}