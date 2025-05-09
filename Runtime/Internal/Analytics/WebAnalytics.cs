using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal sealed class WebAnalytics : IAnalytics
    {
        public void SendEvent(string eventName) => Debug.Log($"<! NOT IMPL !> [{nameof(WebAnalytics)}] Sending event: {eventName}");
        public void SendEvent(string eventName, string data) => Debug.Log($"<! NOT IMPL !> [{nameof(WebAnalytics)}] Sending event: {eventName} with data: {data}");
        public void SendGameIsReady() => Debug.Log($"<! NOT IMPL !> [{nameof(WelwiseGamesSDK)}] Sending game is ready");
    }
}