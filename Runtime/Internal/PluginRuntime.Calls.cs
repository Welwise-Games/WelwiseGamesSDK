using System;
using System.Runtime.InteropServices;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Internal
{
    internal sealed partial class PluginRuntime
    {
        public void HandleGamePause(string pauseState) {
            
            bool shouldPause = pauseState.ToLower() == "true";
    
            // Приостановка/возобновление времени игры
            Time.timeScale = shouldPause ? 0f : 1f;
    
            // Приостановка/возобновление аудио
            AudioListener.pause = shouldPause;
    
            Debug.Log($"[PluginRuntime] Game {(shouldPause ? "paused" : "resumed")}");
        }
        #region Module Support Check

        [DllImport("__Internal")]
        private static extern IntPtr JSGetModules();

        public static string GetAvailableModules()
        {
            var ptr = JSGetModules();
            if (ptr == IntPtr.Zero) return string.Empty;
        
            var json = Marshal.PtrToStringAuto(ptr);
            FreeMemory(ptr);
        
            return json;
        }

        [DllImport("__Internal")]
        private static extern void FreeMemory(IntPtr ptr);

        #endregion
        
        #region Initialization
        [DllImport("__Internal")]
        private static extern void JSInit();

        public static void Init(Action onSuccess, Action<string> onError)
        {
            OnInitSuccess += SuccessHandler;
            OnInitError += ErrorHandler;

            JSInit();
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
        private static extern void JSGetServerTime();

        public static void GetServerTime(Action<string> onSuccess, Action<string> onError)
        {
            OnGetTimeSuccess += SuccessHandler;
            OnGetTimeError += ErrorHandler;

            JSGetServerTime();
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
        private static extern void JSGoToGame(string gameId);

        public static void GoToGame(string gameId, Action onSuccess, Action<string> onError)
        {
            OnNavigateSuccess += SuccessHandler;
            OnNavigateError += ErrorHandler;

            JSGoToGame(gameId);
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
        private static extern void JSIsMetaverseSupported();

        public static void IsMetaverseSupported(Action<bool> onSuccess, Action<string> onError)
        {
            OnIsMetaverseSupportedSuccess += SuccessHandler;
            OnIsMetaverseSupportedError += ErrorHandler;
            
            JSIsMetaverseSupported();
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
        private static extern void JSGetPlayerId();
        
        public static void GetPlayerId(Action<string> onSuccess, Action<string> onError)
        {
            OnGetPlayerIdSuccess += SuccessHandler;
            OnGetPlayerIdError += ErrorHandler;
            
            JSGetPlayerId();
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
        private static extern void JSGetDeviceType();
        
        public static void GetDeviceType(Action<string> onSuccess, Action<string> onError)
        {
            OnGetDeviceTypeSuccess += SuccessHandler;
            OnGetDeviceTypeError += ErrorHandler;
            
            JSGetDeviceType();
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
        private static extern void JSGetLanguageCode();
        
        public static void GetLanguageCode(Action<string> onSuccess, Action<string> onError)
        {
            OnGetLanguageCodeSuccess += SuccessHandler;
            OnGetLanguageCodeError += ErrorHandler;
            
            JSGetLanguageCode();
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
        private static extern void JSGameReady();
    
        public static void GameReady(Action onSuccess, Action<string> onError)
        {
            OnGameReadySuccess += SuccessHandler;
            OnGameReadyError += ErrorHandler;
    
            JSGameReady();
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
        private static extern void JSGameplayStart();
    
        public static void GameplayStart(Action onSuccess, Action<string> onError)
        {
            OnGameplayStartSuccess += SuccessHandler;
            OnGameplayStartError += ErrorHandler;
    
            JSGameplayStart();
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
        private static extern void JSGameplayStop();
    
        public static void GameplayStop(Action onSuccess, Action<string> onError)
        {
            OnGameplayStopSuccess += SuccessHandler;
            OnGameplayStopError += ErrorHandler;
    
            JSGameplayStop();
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
        private static extern void JSShowInterstitial();
        
        [DllImport("__Internal")]
        private static extern void JSShowRewarded();
        
        public static void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            OnInterstitialOpen += OpenHandler;
            OnInterstitialClose += CloseHandler;
            OnInterstitialError += ErrorHandler;
        
            JSShowInterstitial();
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
        
            JSShowRewarded();
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

        #region Payments
        [DllImport("__Internal")]
        private static extern void JSPaymentsInit();
        
        public static void PaymentsInit(Action onSuccess, Action<string> onError)
        {
            OnPaymentsInitSuccess += SuccessHandler;
            OnPaymentsInitError += ErrorHandler;
        
            JSPaymentsInit();
            return;
        
            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnPaymentsInitSuccess -= SuccessHandler;
                OnPaymentsInitError -= ErrorHandler;
            }
        
            void SuccessHandler()
            {
                onSuccess?.Invoke();
                OnPaymentsInitSuccess -= SuccessHandler;
                OnPaymentsInitError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JSPaymentsGetCatalog();
        
        public static void PaymentsGetCatalog(Action<string> onSuccess, Action<string> onError)
        {
            OnPaymentsGetCatalogSuccess += SuccessHandler;
            OnPaymentsGetCatalogError += ErrorHandler;
        
            JSPaymentsGetCatalog();
            return;
        
            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnPaymentsGetCatalogSuccess -= SuccessHandler;
                OnPaymentsGetCatalogError -= ErrorHandler;
            }
        
            void SuccessHandler(string catalog)
            {
                onSuccess?.Invoke(catalog);
                OnPaymentsGetCatalogSuccess -= SuccessHandler;
                OnPaymentsGetCatalogError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JSPaymentsGetPurchases();
        
        public static void PaymentsGetPurchases(Action<string> onSuccess, Action<string> onError)
        {
            OnPaymentsGetPurchasesSuccess += SuccessHandler;
            OnPaymentsGetPurchasesError += ErrorHandler;
        
            JSPaymentsGetPurchases();
            return;
        
            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnPaymentsGetPurchasesSuccess -= SuccessHandler;
                OnPaymentsGetPurchasesError -= ErrorHandler;
            }
        
            void SuccessHandler(string purchases)
            {
                onSuccess?.Invoke(purchases);
                OnPaymentsGetPurchasesSuccess -= SuccessHandler;
                OnPaymentsGetPurchasesError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JSPaymentsPurchase(string productId, string developerPayload);
        
        public static void PaymentsPurchase(string productId, string developerPayload, 
            Action<string> onSuccess, Action<string> onError)
        {
            OnPaymentsPurchaseSuccess += SuccessHandler;
            OnPaymentsPurchaseError += ErrorHandler;
        
            JSPaymentsPurchase(productId, developerPayload);
            return;
        
            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnPaymentsPurchaseSuccess -= SuccessHandler;
                OnPaymentsPurchaseError -= ErrorHandler;
            }
        
            void SuccessHandler(string purchase)
            {
                onSuccess?.Invoke(purchase);
                OnPaymentsPurchaseSuccess -= SuccessHandler;
                OnPaymentsPurchaseError -= ErrorHandler;
            }
        }
        
        [DllImport("__Internal")]
        private static extern void JSPaymentsConsume(string purchaseToken);
        
        public static void PaymentsConsume(string purchaseToken, 
            Action<string> onSuccess, Action<string> onError)
        {
            OnPaymentsConsumeSuccess += SuccessHandler;
            OnPaymentsConsumeError += ErrorHandler;
        
            JSPaymentsConsume(purchaseToken);
            return;
        
            void ErrorHandler(string error)
            {
                onError?.Invoke(error);
                OnPaymentsConsumeSuccess -= SuccessHandler;
                OnPaymentsConsumeError -= ErrorHandler;
            }
        
            void SuccessHandler(string token)
            {
                onSuccess?.Invoke(token);
                OnPaymentsConsumeSuccess -= SuccessHandler;
                OnPaymentsConsumeError -= ErrorHandler;
            }
        }
        
        #endregion
    }
}