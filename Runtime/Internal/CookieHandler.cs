using System.Runtime.InteropServices;

namespace WelwiseGamesSDK.Internal
{
    internal static class CookieHandler
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SetCookie(string key, string value, int expireDays);
        [DllImport("__Internal")]
        private static extern string GetCookie(string key);
#endif

        public static void SaveData(string key, string value, int expireDays = 30)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetCookie(key, value, expireDays);
#endif
        }

        public static string LoadData(string key)
        {                
#if UNITY_WEBGL && !UNITY_EDITOR
            return GetCookie(key);
#endif
            return "";
        }
    }
}