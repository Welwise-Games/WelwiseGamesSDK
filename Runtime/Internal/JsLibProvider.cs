using System;
using System.Runtime.InteropServices;

namespace WelwiseGamesSDK.Internal
{
    internal static class JsLibProvider
    {
#if UNITY_WEBGL
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