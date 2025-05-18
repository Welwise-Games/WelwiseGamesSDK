using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Analytics
{
    internal sealed class WebAnalytics : IAnalytics
    {
        public void SendEvent(string eventName)
        {
            Debug.Log($"[{nameof(WebAnalytics)}.SendEvent] Not Native Implemented: {eventName}");
        }

        public void SendEvent(string eventName, string data)
        {
            Debug.Log($"[{nameof(WebAnalytics)}.SendEvent] Not Native Implemented: {eventName} data: {data} ");
        }
        
        public void GameIsReady()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GameReady(() => {}, Debug.LogError);
#endif
        }

        public void GameplayStart()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GameplayStart(() => {}, Debug.LogError);
#endif
        }

        public void GameplayEnd()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.GameplayStop(() => {}, Debug.LogError);
#endif
        }
    }
}