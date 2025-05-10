#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WelwiseGamesSDK.Internal
{
    internal static class JsLibProvider
    {
        private static JsBridge _bridge;
        private static GameObject _bridgeObject;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            _bridgeObject = new GameObject("JsBridge");
            _bridge = _bridgeObject.AddComponent<JsBridge>();
            Object.DontDestroyOnLoad(_bridgeObject);
        }

        #region Initialization
        [DllImport("__Internal")]
        private static extern void JsInit();

        public static void Init(Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;

            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnInitSuccess -= successWrapper;
                JsBridge.OnInitError -= errorWrapper;
            };

            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnInitSuccess -= successWrapper;
                JsBridge.OnInitError -= errorWrapper;
            };

            JsBridge.OnInitSuccess += successWrapper;
            JsBridge.OnInitError += errorWrapper;

            JsInit();
        }
        #endregion

        #region Player Data
        [DllImport("__Internal")]
        private static extern void JSGetPlayerData();

        public static void GetPlayerData(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;

            successWrapper = (data) =>
            {
                onSuccess?.Invoke(data);
                JsBridge.OnGetDataSuccess -= successWrapper;
                JsBridge.OnGetDataError -= errorWrapper;
            };

            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGetDataSuccess -= successWrapper;
                JsBridge.OnGetDataError -= errorWrapper;
            };

            JsBridge.OnGetDataSuccess += successWrapper;
            JsBridge.OnGetDataError += errorWrapper;

            JSGetPlayerData();
        }

        [DllImport("__Internal")]
        private static extern void JSSetPlayerData(string jsonData);

        public static void SetPlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;

            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnSetDataSuccess -= successWrapper;
                JsBridge.OnSetDataError -= errorWrapper;
            };

            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnSetDataSuccess -= successWrapper;
                JsBridge.OnSetDataError -= errorWrapper;
            };

            JsBridge.OnSetDataSuccess += successWrapper;
            JsBridge.OnSetDataError += errorWrapper;

            JSSetPlayerData(jsonData);
        }
        #endregion

        #region Server Time
        [DllImport("__Internal")]
        private static extern void JsGetServerTime();

        public static void GetServerTime(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;

            successWrapper = (timestamp) =>
            {
                onSuccess?.Invoke(timestamp);
                JsBridge.OnGetTimeSuccess -= successWrapper;
                JsBridge.OnGetTimeError -= errorWrapper;
            };

            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGetTimeSuccess -= successWrapper;
                JsBridge.OnGetTimeError -= errorWrapper;
            };

            JsBridge.OnGetTimeSuccess += successWrapper;
            JsBridge.OnGetTimeError += errorWrapper;

            JsGetServerTime();
        }
        #endregion

        #region Navigation
        [DllImport("__Internal")]
        private static extern void JsGoToGame(int gameId);

        public static void GoToGame(int gameId, Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;

            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnNavigateSuccess -= successWrapper;
                JsBridge.OnNavigateError -= errorWrapper;
            };

            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnNavigateSuccess -= successWrapper;
                JsBridge.OnNavigateError -= errorWrapper;
            };

            JsBridge.OnNavigateSuccess += successWrapper;
            JsBridge.OnNavigateError += errorWrapper;

            JsGoToGame(gameId);
        }
        #endregion
    }
}
#endif