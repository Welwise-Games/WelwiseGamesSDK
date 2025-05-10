#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
#endif

namespace WelwiseGamesSDK.Internal
{
    internal static class JsLibProvider
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] 
        public static extern void JSGetPlayerData(Action<string> onSuccess, Action<string> onError);
        [DllImport("__Internal")]
        public static extern void JSSetPlayerData(string jsonData, Action onSuccess, Action<string> onError);
        [DllImport("__Internal")]
        public static extern void JsGetServerTime(Action<string> onSuccess, Action<string> onError);
        [DllImport("__Internal")]
        public static extern void JsGoToGame(int id, Action onSuccess, Action<string> onError);
        [DllImport("__Internal")]
        public static extern void JsInit(Action onSuccess, Action<string> onError);
#endif
    }
}