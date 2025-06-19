using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    internal sealed partial class PluginRuntime : MonoBehaviour
    {
        private static SDKSettings _settings;
        private static PluginRuntime _instance;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            _settings = SDKSettings.LoadOrCreateSettings();
            
            var go = new GameObject(nameof(PluginRuntime));
            _instance = go.AddComponent<PluginRuntime>();
            DontDestroyOnLoad(go);
            
            if (!_settings.AutoConstructAndInitializeSingleton) return;
            
            new SDKBuilder(_settings).AsSingle();
            WelwiseSDK.Instance.Initialize();
        }
        
        private void OnApplicationFocus(bool hasFocus) => HandleGamePause(!hasFocus);

        private void OnApplicationPause(bool pauseStatus) => HandleGamePause(pauseStatus);

        private static void HandleGamePause(bool isPaused)
        {
            if (_settings.MuteAudioOnPause)
            {
                AudioListener.volume = isPaused ? 0 : 1; 
            }
        }

        public new static Coroutine StartCoroutine(IEnumerator routine) => ((MonoBehaviour)_instance).StartCoroutine(routine);
        public new static void StopCoroutine(Coroutine routine) => ((MonoBehaviour)_instance).StopCoroutine(routine);
    }
}