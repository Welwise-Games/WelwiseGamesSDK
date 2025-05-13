#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;

namespace WelwiseGamesSDK.Internal
{
    public class JsBridge : MonoBehaviour
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
        public static event System.Action<string> OnGetDeviceTypeSuccess;
        public static event System.Action<string> OnGetDeviceTypeError;
        public static event System.Action<string> OnGetLanguageCodeSuccess;
        public static event System.Action<string> OnGetLanguageCodeError;


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
        public void HandleGetDeviceTypeSuccess(string deviceType) => OnGetDeviceTypeSuccess?.Invoke(deviceType);
        public void HandleGetDeviceTypeError(string error) => OnGetDeviceTypeError?.Invoke(error);
        public void HandleGetLanguageCodeSuccess(string languageCode) => OnGetLanguageCodeSuccess?.Invoke(languageCode);
        public void HandleGetLanguageCodeError(string error) => OnGetLanguageCodeError?.Invoke(error);


        private void OnDestroy()
        {
            OnInitSuccess = null;
            OnInitError = null;
            OnGetDataSuccess = null;
            OnGetDataError = null;
            OnSetDataSuccess = null;
            OnSetDataError = null;
            OnGetTimeSuccess = null;
            OnGetTimeError = null;
            OnNavigateSuccess = null;
            OnNavigateError = null;
            OnGetPlayerIdSuccess = null;
            OnGetPlayerIdError = null;
            OnGetDeviceTypeSuccess = null;
            OnGetDeviceTypeError = null;
            OnGetLanguageCodeSuccess = null;
            OnGetLanguageCodeError = null;
        }
    }
}
#endif