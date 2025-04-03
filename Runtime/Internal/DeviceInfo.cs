using System.Runtime.InteropServices;

namespace WelwiseGamesSDK.Internal
{
    internal static class DeviceInfo
    {
            private static string _fallbackDevice;
            private static string _fallbackLanguage;
            
            public static void InitializeFallback(string fallbackDevice, string fallbackLanguage)
            { 
                    _fallbackDevice = fallbackDevice;
                    _fallbackLanguage = fallbackLanguage;
            }
        public static string DetectDeviceType()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetDeviceType();
#else
                return _fallbackDevice;
#endif
        }

        public static string DetectLanguage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetLanguage();
#else
            return _fallbackLanguage;
#endif
        }
        

        [DllImport("__Internal")]
        private static extern string GetDeviceType();

        [DllImport("__Internal")]
        private static extern string GetLanguage();
    }
}