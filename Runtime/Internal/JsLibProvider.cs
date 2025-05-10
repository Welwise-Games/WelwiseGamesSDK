#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

namespace WelwiseGamesSDK.Internal
{
    internal static class JsLibProvider
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void JsVoidCallback(int callbackId);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void JsStringCallback(int callbackId, string data);

        #region Init
        [DllImport("__Internal")]
        private static extern void JsInit(
            JsVoidCallback onSuccess,
            JsStringCallback onError
        );

        public static void Init(Action onSuccess, Action<string> onError)
        {
            var successId = JsLibCallbackHandler.RegisterCallback(onSuccess);
            var errorId = JsLibCallbackHandler.RegisterCallback(onError);

            JsInit(
                JsLibCallbackHandler.VoidCallbackHandler,
                JsLibCallbackHandler.StringCallbackHandler
            );
        }
        #endregion

        #region PlayerData
        [DllImport("__Internal")]
        private static extern void JSGetPlayerData(
            JsStringCallback onSuccess,
            JsStringCallback onError
        );

        public static void GetPlayerData(Action<string> onSuccess, Action<string> onError)
        {
            var successId = JsLibCallbackHandler.RegisterCallback(onSuccess);
            var errorId = JsLibCallbackHandler.RegisterCallback(onError);

            JSGetPlayerData(
                JsLibCallbackHandler.StringCallbackHandler,
                JsLibCallbackHandler.StringCallbackHandler
            );
        }

        [DllImport("__Internal")]
        private static extern void JSSetPlayerData(
            string jsonData,
            JsVoidCallback onSuccess,
            JsStringCallback onError
        );

        public static void SetPlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            var successId = JsLibCallbackHandler.RegisterCallback(onSuccess);
            var errorId = JsLibCallbackHandler.RegisterCallback(onError);

            JSSetPlayerData(
                jsonData,
                JsLibCallbackHandler.VoidCallbackHandler,
                JsLibCallbackHandler.StringCallbackHandler
            );
        }
        #endregion

        #region ServerTime
        [DllImport("__Internal")]
        private static extern void JsGetServerTime(
            JsStringCallback onSuccess,
            JsStringCallback onError
        );

        public static void GetServerTime(Action<string> onSuccess, Action<string> onError)
        {
            var successId = JsLibCallbackHandler.RegisterCallback(onSuccess);
            var errorId = JsLibCallbackHandler.RegisterCallback(onError);

            JsGetServerTime(
                JsLibCallbackHandler.StringCallbackHandler,
                JsLibCallbackHandler.StringCallbackHandler
            );
        }
        #endregion

        #region Navigation
        [DllImport("__Internal")]
        private static extern void JsGoToGame(
            int gameId,
            JsVoidCallback onSuccess,
            JsStringCallback onError
        );

        public static void GoToGame(int gameId, Action onSuccess, Action<string> onError)
        {
            var successId = JsLibCallbackHandler.RegisterCallback(onSuccess);
            var errorId = JsLibCallbackHandler.RegisterCallback(onError);

            JsGoToGame(
                gameId,
                JsLibCallbackHandler.VoidCallbackHandler,
                JsLibCallbackHandler.StringCallbackHandler
            );
        }
        #endregion
    }
}
#endif