namespace WelwiseGamesSDK.Internal
{
    internal sealed partial class PluginRuntime
    {
        public static event System.Action OnInitSuccess;
        public static event System.Action<string> OnInitError;

        public static event System.Action<string> OnGetDataSuccess;
        public static event System.Action<string> OnGetDataError;
        public static event System.Action OnSetDataSuccess;
        public static event System.Action<string> OnSetDataError;

        public static event System.Action<string> OnGetTimeSuccess;
        public static event System.Action<string> OnGetTimeError;

        public static event System.Action OnNavigateSuccess;
        public static event System.Action<string> OnNavigateError;

        public static event System.Action<string> OnGetPlayerIdSuccess;
        public static event System.Action<string> OnGetPlayerIdError;
        public static event System.Action<string> OnIsMetaverseSupportedSuccess;
        public static event System.Action<string> OnIsMetaverseSupportedError;
        public static event System.Action<string> OnGetDeviceTypeSuccess;
        public static event System.Action<string> OnGetDeviceTypeError;
        public static event System.Action<string> OnGetLanguageCodeSuccess;
        public static event System.Action<string> OnGetLanguageCodeError;

        public static event System.Action<string> OnGetMetaverseDataSuccess;
        public static event System.Action<string> OnGetMetaverseDataError;
        public static event System.Action OnSetMetaverseDataSuccess;
        public static event System.Action<string> OnSetMetaverseDataError;
        
        public static event System.Action<string> OnGetCombinedDataSuccess;
        public static event System.Action<string> OnGetCombinedDataError;
        public static event System.Action OnSetCombinedDataSuccess;
        public static event System.Action<string> OnSetCombinedDataError;

        public static event System.Action OnGameReadySuccess;
        public static event System.Action<string> OnGameReadyError;
        public static event System.Action OnGameplayStartSuccess;
        public static event System.Action<string> OnGameplayStartError;
        public static event System.Action OnGameplayStopSuccess;
        public static event System.Action<string> OnGameplayStopError;

        public static event System.Action OnInterstitialOpen;
        public static event System.Action OnInterstitialClose;
        public static event System.Action<string> OnInterstitialError;
        
        public static event System.Action OnRewardedOpen;
        public static event System.Action OnRewardedRewarded;
        public static event System.Action OnRewardedClose;
        public static event System.Action<string> OnRewardedError;
        public static event System.Action OnPaymentsInitSuccess;
        public static event System.Action<string> OnPaymentsInitError;
        public static event System.Action<string> OnPaymentsGetCatalogSuccess;
        public static event System.Action<string> OnPaymentsGetCatalogError;
        public static event System.Action<string> OnPaymentsGetPurchasesSuccess;
        public static event System.Action<string> OnPaymentsGetPurchasesError;
        public static event System.Action<string> OnPaymentsPurchaseSuccess;
        public static event System.Action<string> OnPaymentsPurchaseError;
        public static event System.Action<string> OnPaymentsConsumeSuccess;
        public static event System.Action<string> OnPaymentsConsumeError;

        public void HandlePaymentsInitSuccess(string _) => OnPaymentsInitSuccess?.Invoke();
        public void HandlePaymentsInitError(string error) => OnPaymentsInitError?.Invoke(error);
        public void HandlePaymentsGetCatalogSuccess(string json) => OnPaymentsGetCatalogSuccess?.Invoke(json);
        public void HandlePaymentsGetCatalogError(string error) => OnPaymentsGetCatalogError?.Invoke(error);
        public void HandlePaymentsGetPurchasesSuccess(string json) => OnPaymentsGetPurchasesSuccess?.Invoke(json);
        public void HandlePaymentsGetPurchasesError(string error) => OnPaymentsGetPurchasesError?.Invoke(error);
        public void HandlePaymentsPurchaseSuccess(string json) => OnPaymentsPurchaseSuccess?.Invoke(json);
        public void HandlePaymentsPurchaseError(string error) => OnPaymentsPurchaseError?.Invoke(error);
        public void HandlePaymentsConsumeSuccess(string token) => OnPaymentsConsumeSuccess?.Invoke(token);
        public void HandlePaymentsConsumeError(string error) => OnPaymentsConsumeError?.Invoke(error);


        public void HandleInitSuccess(string _) => OnInitSuccess?.Invoke();
        public void HandleInitError(string error) => OnInitError?.Invoke(error);

        public void HandleGetDataSuccess(string data) => OnGetDataSuccess?.Invoke(data);
        public void HandleGetDataError(string error) => OnGetDataError?.Invoke(error);

        public void HandleSetDataSuccess(string _) => OnSetDataSuccess?.Invoke();
        public void HandleSetDataError(string error) => OnSetDataError?.Invoke(error);

        public void HandleGetTimeSuccess(string timestamp) => OnGetTimeSuccess?.Invoke(timestamp);
        public void HandleGetTimeError(string error) => OnGetTimeError?.Invoke(error);

        public void HandleNavigateSuccess(string _) => OnNavigateSuccess?.Invoke();
        public void HandleNavigateError(string error) => OnNavigateError?.Invoke(error);

        public void HandleGetPlayerIdSuccess(string playerId) => OnGetPlayerIdSuccess?.Invoke(playerId);
        public void HandleGetPlayerIdError(string error) => OnGetPlayerIdError?.Invoke(error);
        public void HandleIsMetaverseSupportedSuccess(string supported) => OnIsMetaverseSupportedSuccess?.Invoke(supported);
        public void HandleIsMetaverseSupportedError(string error) => OnIsMetaverseSupportedError?.Invoke(error);
        public void HandleGetDeviceTypeSuccess(string deviceType) => OnGetDeviceTypeSuccess?.Invoke(deviceType);
        public void HandleGetDeviceTypeError(string error) => OnGetDeviceTypeError?.Invoke(error);
        public void HandleGetLanguageCodeSuccess(string languageCode) => OnGetLanguageCodeSuccess?.Invoke(languageCode);
        public void HandleGetLanguageCodeError(string error) => OnGetLanguageCodeError?.Invoke(error);

        public void HandleGetMetaverseDataSuccess(string data) => OnGetMetaverseDataSuccess?.Invoke(data);
        public void HandleGetMetaverseDataError(string error) => OnGetMetaverseDataError?.Invoke(error);
        public void HandleSetMetaverseDataSuccess(string _) => OnSetMetaverseDataSuccess?.Invoke();
        public void HandleSetMetaverseDataError(string error) => OnSetMetaverseDataError?.Invoke(error);
        
        public void HandleGetCombinedDataSuccess(string data) => OnGetCombinedDataSuccess?.Invoke(data);
        public void HandleGetCombinedDataError(string error) => OnGetCombinedDataError?.Invoke(error);
        public void HandleSetCombinedDataSuccess(string _) => OnSetCombinedDataSuccess?.Invoke();
        public void HandleSetCombinedDataError(string error) => OnSetCombinedDataError?.Invoke(error);

        public void HandleGameReadySuccess(string _) => OnGameReadySuccess?.Invoke();
        public void HandleGameReadyError(string error) => OnGameReadyError?.Invoke(error);
        public void HandleGameplayStartSuccess(string _) => OnGameplayStartSuccess?.Invoke();
        public void HandleGameplayStartError(string error) => OnGameplayStartError?.Invoke(error);
        public void HandleGameplayStopSuccess(string _) => OnGameplayStopSuccess?.Invoke();
        public void HandleGameplayStopError(string error) => OnGameplayStopError?.Invoke(error);

        public void HandleInterstitialOpen(string _) => OnInterstitialOpen?.Invoke();
        public void HandleInterstitialClose(string _) => OnInterstitialClose?.Invoke();
        public void HandleInterstitialError(string error) => OnInterstitialError?.Invoke(error);
        
        public void HandleRewardedOpen(string _) => OnRewardedOpen?.Invoke();
        public void HandleRewardedRewarded(string _) => OnRewardedRewarded?.Invoke();
        public void HandleRewardedClose(string _) => OnRewardedClose?.Invoke();
        public void HandleRewardedError(string error) => OnRewardedError?.Invoke(error);
    }
}