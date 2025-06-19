using System;
using System.Runtime.InteropServices;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    internal sealed partial class PluginRuntime
    {
        #region Initialization
        [DllImport("__Internal")]
        private static extern void JsInit();

        public static void Init(Action onSuccess, Action<string> onError)
        {
            OnInitSuccess += SuccessHandler;
            OnInitError += ErrorHandler;

            JsInit();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnInitSuccess -= SuccessHandler;
                OnInitError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnInitSuccess -= SuccessHandler;
                OnInitError -= ErrorHandler;
            }
        }
        #endregion

        #region Player Data
        [DllImport("__Internal")]
        private static extern void JSGetPlayerData();

        public static void GetPlayerData(Action<string> onSuccess, Action<string> onError)
        {
            OnGetDataSuccess += SuccessHandler;
            OnGetDataError += ErrorHandler;

            JSGetPlayerData();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetDataSuccess -= SuccessHandler;
                OnGetDataError -= ErrorHandler;
            }

            void SuccessHandler(string data)
            {
                onSuccess?.Invoke(data);
                OnGetDataSuccess -= SuccessHandler;
                OnGetDataError -= ErrorHandler;
            }
        }

        [DllImport("__Internal")]
        private static extern void JSSetPlayerData(string jsonData);

        public static void SetPlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            OnSetDataSuccess += SuccessHandler;
            OnSetDataError += ErrorHandler;

            JSSetPlayerData(jsonData);
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnSetDataSuccess -= SuccessHandler;
                OnSetDataError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnSetDataSuccess -= SuccessHandler;
                OnSetDataError -= ErrorHandler;
            }
        }
        #endregion

        #region Server Time
        [DllImport("__Internal")]
        private static extern void JsGetServerTime();

        public static void GetServerTime(Action<string> onSuccess, Action<string> onError)
        {
            OnGetTimeSuccess += SuccessHandler;
            OnGetTimeError += ErrorHandler;

            JsGetServerTime();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetTimeSuccess -= SuccessHandler;
                OnGetTimeError -= ErrorHandler;
            }

            void SuccessHandler(string timestamp)
            {
                onSuccess?.Invoke(timestamp);
                OnGetTimeSuccess -= SuccessHandler;
                OnGetTimeError -= ErrorHandler;
            }
        }
        #endregion

        #region Navigation
        [DllImport("__Internal")]
        private static extern void JsGoToGame(int gameId);

        public static void GoToGame(int gameId, Action onSuccess, Action<string> onError)
        {
            OnNavigateSuccess += SuccessHandler;
            OnNavigateError += ErrorHandler;

            JsGoToGame(gameId);
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnNavigateSuccess -= SuccessHandler;
                OnNavigateError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnNavigateSuccess -= SuccessHandler;
                OnNavigateError -= ErrorHandler;
            }
        }
        #endregion

        #region Environment Properties
        [DllImport("__Internal")]
        private static extern void JsIsMetaverseSupported();

        public static void IsMetaverseSupported(Action<bool> onSuccess, Action<string> onError)
        {
            OnIsMetaverseSupportedSuccess += SuccessHandler;
            OnIsMetaverseSupportedError += ErrorHandler;
            
            JsIsMetaverseSupported();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnIsMetaverseSupportedSuccess -= SuccessHandler;
                OnIsMetaverseSupportedError -= ErrorHandler;
            }

            void SuccessHandler(string supported)
            {
                onSuccess?.Invoke(supported.ToLower() == "true");
                OnIsMetaverseSupportedSuccess -= SuccessHandler;
                OnIsMetaverseSupportedError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JsGetPlayerId();
        
        public static void GetPlayerId(Action<string> onSuccess, Action<string> onError)
        {
            OnGetPlayerIdSuccess += SuccessHandler;
            OnGetPlayerIdError += ErrorHandler;
            
            JsGetPlayerId();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetPlayerIdSuccess -= SuccessHandler;
                OnGetPlayerIdError -= ErrorHandler;
            }

            void SuccessHandler(string playerId)
            {
                onSuccess?.Invoke(playerId);
                OnGetPlayerIdSuccess -= SuccessHandler;
                OnGetPlayerIdError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JsGetDeviceType();
        
        public static void GetDeviceType(Action<string> onSuccess, Action<string> onError)
        {
            OnGetDeviceTypeSuccess += SuccessHandler;
            OnGetDeviceTypeError += ErrorHandler;
            
            JsGetDeviceType();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetDeviceTypeSuccess -= SuccessHandler;
                OnGetDeviceTypeError -= ErrorHandler;
            }

            void SuccessHandler(string deviceType)
            {
                onSuccess?.Invoke(deviceType);
                OnGetDeviceTypeSuccess -= SuccessHandler;
                OnGetDeviceTypeError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JsGetLanguageCode();
        
        public static void GetLanguageCode(Action<string> onSuccess, Action<string> onError)
        {
            OnGetLanguageCodeSuccess += SuccessHandler;
            OnGetLanguageCodeError += ErrorHandler;
            
            JsGetLanguageCode();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetLanguageCodeSuccess -= SuccessHandler;
                OnGetLanguageCodeError -= ErrorHandler;
            }

            void SuccessHandler(string languageCode)
            {
                onSuccess?.Invoke(languageCode);
                OnGetLanguageCodeSuccess -= SuccessHandler;
                OnGetLanguageCodeError -= ErrorHandler;
            }
        }
        #endregion

        #region Metaverse Player Data
        [DllImport("__Internal")]
        private static extern void JSGetMetaversePlayerData();
    
        public static void GetMetaversePlayerData(Action<string> onSuccess, Action<string> onError)
        {
            OnGetMetaverseDataSuccess += SuccessHandler;
            OnGetMetaverseDataError += ErrorHandler;
    
            JSGetMetaversePlayerData();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetMetaverseDataSuccess -= SuccessHandler;
                OnGetMetaverseDataError -= ErrorHandler;
            }

            void SuccessHandler(string data)
            {
                onSuccess?.Invoke(data);
                OnGetMetaverseDataSuccess -= SuccessHandler;
                OnGetMetaverseDataError -= ErrorHandler;
            }
        }
    
        [DllImport("__Internal")]
        private static extern void JSSetMetaversePlayerData(string jsonData);
    
        public static void SetMetaversePlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            OnSetMetaverseDataSuccess += SuccessHandler;
            OnSetMetaverseDataError += ErrorHandler;
    
            JSSetMetaversePlayerData(jsonData);
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnSetMetaverseDataSuccess -= SuccessHandler;
                OnSetMetaverseDataError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnSetMetaverseDataSuccess -= SuccessHandler;
                OnSetMetaverseDataError -= ErrorHandler;
            }
        }
        #endregion

        #region Combined Player Data
        [DllImport("__Internal")]
        private static extern void JSGetCombinedPlayerData();
    
        public static void GetCombinedPlayerData(Action<string> onSuccess, Action<string> onError)
        {
            OnGetCombinedDataSuccess += SuccessHandler;
            OnGetCombinedDataError += ErrorHandler;
    
            JSGetCombinedPlayerData();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGetCombinedDataSuccess -= SuccessHandler;
                OnGetCombinedDataError -= ErrorHandler;
            }

            void SuccessHandler(string data)
            {
                onSuccess?.Invoke(data);
                OnGetCombinedDataSuccess -= SuccessHandler;
                OnGetCombinedDataError -= ErrorHandler;
            }
        }
    
        [DllImport("__Internal")]
        private static extern void JSSetCombinedPlayerData(string jsonData);
    
        public static void SetCombinedPlayerData(string jsonData, Action onSuccess, Action<string> onError)
        {
            OnSetCombinedDataSuccess += SuccessHandler;
            OnSetCombinedDataError += ErrorHandler;
    
            JSSetCombinedPlayerData(jsonData);
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnSetCombinedDataSuccess -= SuccessHandler;
                OnSetCombinedDataError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnSetCombinedDataSuccess -= SuccessHandler;
                OnSetCombinedDataError -= ErrorHandler;
            }
        }
        #endregion
        
        #region Game Ready and Gameplay
        [DllImport("__Internal")]
        private static extern void JsGameReady();
    
        public static void GameReady(Action onSuccess, Action<string> onError)
        {
            OnGameReadySuccess += SuccessHandler;
            OnGameReadyError += ErrorHandler;
    
            JsGameReady();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGameReadySuccess -= SuccessHandler;
                OnGameReadyError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnGameReadySuccess -= SuccessHandler;
                OnGameReadyError -= ErrorHandler;
            }
        }
    
        [DllImport("__Internal")]
        private static extern void JsGameplayStart();
    
        public static void GameplayStart(Action onSuccess, Action<string> onError)
        {
            OnGameplayStartSuccess += SuccessHandler;
            OnGameplayStartError += ErrorHandler;
    
            JsGameplayStart();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGameplayStartSuccess -= SuccessHandler;
                OnGameplayStartError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnGameplayStartSuccess -= SuccessHandler;
                OnGameplayStartError -= ErrorHandler;
            }
        }
    
        [DllImport("__Internal")]
        private static extern void JsGameplayStop();
    
        public static void GameplayStop(Action onSuccess, Action<string> onError)
        {
            OnGameplayStopSuccess += SuccessHandler;
            OnGameplayStopError += ErrorHandler;
    
            JsGameplayStop();
            return;

            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnGameplayStopSuccess -= SuccessHandler;
                OnGameplayStopError -= ErrorHandler;
            }

            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnGameplayStopSuccess -= SuccessHandler;
                OnGameplayStopError -= ErrorHandler;
            }
        }
        #endregion

        #region Advertisement
        [DllImport("__Internal")]
        private static extern void JsShowInterstitial();
        
        [DllImport("__Internal")]
        private static extern void JsShowRewarded();
        
        public static void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            OnInterstitialOpen += OpenHandler;
            OnInterstitialClose += CloseHandler;
            OnInterstitialError += ErrorHandler;
        
            JsShowInterstitial();
            return;

            void Unsubscribe()
            {
                OnInterstitialOpen -= OpenHandler;
                OnInterstitialClose -= CloseHandler;
                OnInterstitialError -= ErrorHandler;
            }

            void ErrorHandler(string error)
            {
                callbackState?.Invoke(InterstitialState.Error);
                Unsubscribe();
            }

            void CloseHandler()
            {
                callbackState?.Invoke(InterstitialState.Closed);
                Unsubscribe();
            }

            void OpenHandler()
            {
                callbackState?.Invoke(InterstitialState.Opened);
                OnInterstitialOpen -= OpenHandler;
            }
        }
        
        public static void ShowRewarded(Action<RewardedState> callbackState)
        {
            OnRewardedOpen += OpenHandler;
            OnRewardedRewarded += RewardedHandler;
            OnRewardedClose += CloseHandler;
            OnRewardedError += ErrorHandler;
        
            JsShowRewarded();
            return;

            void Unsubscribe()
            {
                OnRewardedOpen -= OpenHandler;
                OnRewardedRewarded -= RewardedHandler;
                OnRewardedClose -= CloseHandler;
                OnRewardedError -= ErrorHandler;
            }

            void ErrorHandler(string error)
            {
                callbackState?.Invoke(RewardedState.Error);
                Unsubscribe();
            }

            void CloseHandler()
            {
                callbackState?.Invoke(RewardedState.Closed);
                Unsubscribe();
            }

            void RewardedHandler() => callbackState?.Invoke(RewardedState.Rewarded);

            void OpenHandler() => callbackState?.Invoke(RewardedState.Opened);
        }
        #endregion
    }
}