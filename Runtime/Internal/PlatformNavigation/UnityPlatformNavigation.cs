using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class UnityPlatformNavigation : IPlatformNavigation
    {
        public bool IsAvailable { get; }

        public UnityPlatformNavigation(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
        
        public void GoToGame(int id, Action<string> onError)
        {
            Debug.Log($"[{nameof(UnityPlatformNavigation)}] Go to game ID: {id}");
            Debug.Break();
        }
    }
}