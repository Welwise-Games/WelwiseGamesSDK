using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace WelwiseGamesSDK.Internal
{
    internal static class DeviceInfo
    {
            private static int _fallbackDevice;
            private static string _fallbackLanguage;
            
            public static void InitializeFallback(int fallbackDevice, string fallbackLanguage)
            { 
                    _fallbackDevice = fallbackDevice;
                    _fallbackLanguage = fallbackLanguage;
            }
        public static int DetectDeviceType()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
                var deviceType = GetDeviceType();
                Debug.Log($"[C#] Detected device type: '{deviceType}'");
                return deviceType;
#else
                return _fallbackDevice;
#endif
        }

        public static string DetectLanguage()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var ptr = GetLanguage();
            var lang = Marshal.PtrToStringUTF8(ptr);
            FreeLanguagePtr(ptr);
            return lang?.ToLower();
#else
            return _fallbackLanguage;
#endif
        }
        

        [DllImport("__Internal")]
        private static extern int GetDeviceType();

        [DllImport("__Internal")]
        private static extern IntPtr GetLanguage();
        
        [DllImport("__Internal")]
        private static extern void FreeLanguagePtr(IntPtr ptr);
    }
}