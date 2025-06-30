using UnityEngine;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal sealed class UnityAnalytics : IAnalytics
    {
        public bool IsAvailable { get; }

        public UnityAnalytics(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

        public void SendEvent(string eventName) => Debug.Log($"[{nameof(UnityAnalytics)}] Sending event: {eventName}");
        public void SendEvent(string eventName, string data) => Debug.Log($"[{nameof(UnityAnalytics)}] Sending event: {eventName} with data: {data}");
        public void GameIsReady() => Debug.Log($"[{nameof(UnityAnalytics)}] Game is ready");
        public void GameplayStart() => Debug.Log($"[{nameof(UnityAnalytics)}] Gameplay start");
        public void GameplayEnd() => Debug.Log($"[{nameof(UnityAnalytics)}] Gameplay end");
    }
}