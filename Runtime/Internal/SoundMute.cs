using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal
{
    public class SoundMute : MonoBehaviour
    {
        public static void CreateIfNeeded(SDKSettings settings)
        {
            if (!settings.MuteAudioOnPause) return;
            
            var go = new GameObject("SoundMute");
            go.AddComponent<SoundMute>();
            DontDestroyOnLoad(go);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            HandleGamePause(!hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            HandleGamePause(pauseStatus);
        }

        private void HandleGamePause(bool isPaused)
        {
            AudioListener.volume = isPaused ? 0 : 1; 
        }
    }
}