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

        public static void Initialize()
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

        #region Environment Properties
        [DllImport("__Internal")]
        private static extern void JsIsMetaverseSupported();

        public static void IsMetaverseSupported(Action<bool> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
            
            successWrapper = (supported) => 
            {
                onSuccess?.Invoke(supported.ToLower() == "true");
                JsBridge.OnIsMetaverseSupportedSuccess -= successWrapper;
                JsBridge.OnIsMetaverseSupportedError -= errorWrapper;
            };
        
            errorWrapper = (error) => 
            {
                onError?.Invoke(error);
                JsBridge.OnIsMetaverseSupportedSuccess -= successWrapper;
                JsBridge.OnIsMetaverseSupportedError -= errorWrapper;
            };
        
            JsBridge.OnIsMetaverseSupportedSuccess += successWrapper;
            JsBridge.OnIsMetaverseSupportedError += errorWrapper;
            
            JsIsMetaverseSupported();
        }
        
        
        [DllImport("__Internal")]
        private static extern void JsGetPlayerId();
        
        public static void GetPlayerId(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
        
            successWrapper = (playerId) => 
            {
                onSuccess?.Invoke(playerId);
                JsBridge.OnGetPlayerIdSuccess -= successWrapper;
                JsBridge.OnGetPlayerIdError -= errorWrapper;
            };
        
            errorWrapper = (error) => 
            {
                onError?.Invoke(error);
                JsBridge.OnGetPlayerIdSuccess -= successWrapper;
                JsBridge.OnGetPlayerIdError -= errorWrapper;
            };
        
            JsBridge.OnGetPlayerIdSuccess += successWrapper;
            JsBridge.OnGetPlayerIdError += errorWrapper;
            
            JsGetPlayerId();
        }
        
        [DllImport("__Internal")]
        private static extern void JsGetDeviceType();
        
        public static void GetDeviceType(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
        
            successWrapper = (deviceType) => 
            {
                onSuccess?.Invoke(deviceType);
                JsBridge.OnGetDeviceTypeSuccess -= successWrapper;
                JsBridge.OnGetDeviceTypeError -= errorWrapper;
            };
        
            errorWrapper = (error) => 
            {
                onError?.Invoke(error);
                JsBridge.OnGetDeviceTypeSuccess -= successWrapper;
                JsBridge.OnGetDeviceTypeError -= errorWrapper;
            };
        
            JsBridge.OnGetDeviceTypeSuccess += successWrapper;
            JsBridge.OnGetDeviceTypeError += errorWrapper;
            
            JsGetDeviceType();
        }
        
        [DllImport("__Internal")]
        private static extern void JsGetLanguageCode();
        
        public static void GetLanguageCode(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
        
            successWrapper = (languageCode) => 
            {
                onSuccess?.Invoke(languageCode);
                JsBridge.OnGetLanguageCodeSuccess -= successWrapper;
                JsBridge.OnGetLanguageCodeError -= errorWrapper;
            };
        
            errorWrapper = (error) => 
            {
                onError?.Invoke(error);
                JsBridge.OnGetLanguageCodeSuccess -= successWrapper;
                JsBridge.OnGetLanguageCodeError -= errorWrapper;
            };
        
            JsBridge.OnGetLanguageCodeSuccess += successWrapper;
            JsBridge.OnGetLanguageCodeError += errorWrapper;
            
            JsGetLanguageCode();
        }
        #endregion

        #region Metaverse Player Data
    
        [DllImport("__Internal")]
        private static extern void JSGetMetaversePlayerData();
    
        public static void GetMetaversePlayerData(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = (data) =>
            {
                onSuccess?.Invoke(data);
                JsBridge.OnGetMetaverseDataSuccess -= successWrapper;
                JsBridge.OnGetMetaverseDataError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGetMetaverseDataSuccess -= successWrapper;
                JsBridge.OnGetMetaverseDataError -= errorWrapper;
            };
    
            JsBridge.OnGetMetaverseDataSuccess += successWrapper;
            JsBridge.OnGetMetaverseDataError += errorWrapper;
    
            JSGetMetaversePlayerData();
        }
    
        [DllImport("__Internal")]
        private static extern void JSSetMetaversePlayerData(string jsonData);
    
        public static void SetMetaversePlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnSetMetaverseDataSuccess -= successWrapper;
                JsBridge.OnSetMetaverseDataError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnSetMetaverseDataSuccess -= successWrapper;
                JsBridge.OnSetMetaverseDataError -= errorWrapper;
            };
    
            JsBridge.OnSetMetaverseDataSuccess += successWrapper;
            JsBridge.OnSetMetaverseDataError += errorWrapper;
    
            JSSetMetaversePlayerData(jsonData);
        }
        #endregion

        #region Combined Player Data
        [DllImport("__Internal")]
        private static extern void JSGetCombinedPlayerData();
    
        public static void GetCombinedPlayerData(Action<string> onSuccess, Action<string> onError)
        {
            Action<string> successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = (data) =>
            {
                onSuccess?.Invoke(data);
                JsBridge.OnGetCombinedDataSuccess -= successWrapper;
                JsBridge.OnGetCombinedDataError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGetCombinedDataSuccess -= successWrapper;
                JsBridge.OnGetCombinedDataError -= errorWrapper;
            };
    
            JsBridge.OnGetCombinedDataSuccess += successWrapper;
            JsBridge.OnGetCombinedDataError += errorWrapper;
    
            JSGetCombinedPlayerData();
        }
    
        [DllImport("__Internal")]
        private static extern void JSSetCombinedPlayerData(string jsonData);
    
        public static void SetCombinedPlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnSetCombinedDataSuccess -= successWrapper;
                JsBridge.OnSetCombinedDataError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnSetCombinedDataSuccess -= successWrapper;
                JsBridge.OnSetCombinedDataError -= errorWrapper;
            };
    
            JsBridge.OnSetCombinedDataSuccess += successWrapper;
            JsBridge.OnSetCombinedDataError += errorWrapper;
    
            JSSetCombinedPlayerData(jsonData);
        }
        #endregion
        
        #region Game Ready and Gameplay
        [DllImport("__Internal")]
        private static extern void JsGameReady();
    
        public static void GameReady(Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnGameReadySuccess -= successWrapper;
                JsBridge.OnGameReadyError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGameReadySuccess -= successWrapper;
                JsBridge.OnGameReadyError -= errorWrapper;
            };
    
            JsBridge.OnGameReadySuccess += successWrapper;
            JsBridge.OnGameReadyError += errorWrapper;
    
            JsGameReady();
        }
    
        [DllImport("__Internal")]
        private static extern void JsGameplayStart();
    
        public static void GameplayStart(Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnGameplayStartSuccess -= successWrapper;
                JsBridge.OnGameplayStartError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGameplayStartSuccess -= successWrapper;
                JsBridge.OnGameplayStartError -= errorWrapper;
            };
    
            JsBridge.OnGameplayStartSuccess += successWrapper;
            JsBridge.OnGameplayStartError += errorWrapper;
    
            JsGameplayStart();
        }
    
        [DllImport("__Internal")]
        private static extern void JsGameplayStop();
    
        public static void GameplayStop(Action onSuccess, Action<string> onError)
        {
            Action successWrapper = null;
            Action<string> errorWrapper = null;
    
            successWrapper = () =>
            {
                onSuccess?.Invoke();
                JsBridge.OnGameplayStopSuccess -= successWrapper;
                JsBridge.OnGameplayStopError -= errorWrapper;
            };
    
            errorWrapper = (error) =>
            {
                onError?.Invoke(error);
                JsBridge.OnGameplayStopSuccess -= successWrapper;
                JsBridge.OnGameplayStopError -= errorWrapper;
            };
    
            JsBridge.OnGameplayStopSuccess += successWrapper;
            JsBridge.OnGameplayStopError += errorWrapper;
    
            JsGameplayStop();
        }
        #endregion
    }
}
#endif