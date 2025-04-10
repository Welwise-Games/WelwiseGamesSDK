using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal sealed class DebugAnalytics : IAnalytics
    {
        public void SendEvent(string eventName) => Debug.Log($"[{nameof(DebugAnalytics)}] Sending event: {eventName}");
        public void SendEvent(string eventName, string data) => Debug.Log($"[{nameof(DebugAnalytics)}] Sending event: {eventName} with data: {data}");
        public void SendGameIsReady() => Debug.Log($"[{nameof(DebugAnalytics)}] Sending game is ready");
    }
}