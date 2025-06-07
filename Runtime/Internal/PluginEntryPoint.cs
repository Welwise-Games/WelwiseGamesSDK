using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    public static class PluginEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Entry()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JsLibProvider.Initialize();
#endif
            var settings = SDKSettings.LoadOrCreateSettings();
            if (settings.AutoConstructAndInitializeSingleton)
            {
                WelwiseSDK.Construct().AsSingle();
                WelwiseSDK.Instance.Initialize();
            }
        }
    }
}