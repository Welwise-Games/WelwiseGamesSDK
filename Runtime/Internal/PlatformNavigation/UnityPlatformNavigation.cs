using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class UnityPlatformNavigation : IPlatformNavigation
    {
        public void GoToGame(int id, Action<string> onError)
        {
            Debug.Log($"[{nameof(UnityPlatformNavigation)}] Go to game ID: {id}");
            Debug.Break();
        }
    }
}