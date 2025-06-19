using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal sealed class WebAnalytics : IAnalytics
    {
        public void SendEvent(string eventName) =>
            Debug.Log($"[{nameof(WebAnalytics)}.SendEvent] Not Native Implemented: {eventName}");

        public void SendEvent(string eventName, string data) =>
            Debug.Log($"[{nameof(WebAnalytics)}.SendEvent] Not Native Implemented: {eventName} data: {data} ");
        
        public void GameIsReady() => 
            PluginRuntime.GameReady(() => {}, Debug.LogError);
        
        public void GameplayStart() => 
            PluginRuntime.GameplayStart(() => {}, Debug.LogError);
        
        public void GameplayEnd() => 
            PluginRuntime.GameplayStop(() => {}, Debug.LogError);
    }
}