using System;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Advertisement
{
    internal sealed class UnityAdvertisement : MonoBehaviour, IAdvertisement
    {
        private static UnityAdvertisement _instance;
        
        private Action<InterstitialState> _interstitialCallback;
        private Action<RewardedState> _rewardedCallback;
        private bool _showDebugWindow;
        private readonly Rect _windowRect = new Rect(20, 20, 300, 200);
        private string[] _interstitialStates;
        private string[] _rewardedStates;
        private int _selectedInterstitialState;
        private int _selectedRewardedState;
        private readonly float _simulatedDelay = 2f;
        private Coroutine _currentCoroutine;

        internal static UnityAdvertisement Create()
        {
            var gameObject = new GameObject("DebugAdvertisement");
            var comp = gameObject.AddComponent<UnityAdvertisement>();
            if (_instance != null)
            {
                Destroy(gameObject);
                return _instance;
            }

            _instance = comp;
            DontDestroyOnLoad(gameObject);
            
            comp._interstitialStates = Enum.GetNames(typeof(InterstitialState));
            comp._rewardedStates = Enum.GetNames(typeof(RewardedState));
            return comp;
        }

        public void ShowInterstitial(Action<InterstitialState> callbackState)
        {
            if (_currentCoroutine != null) return;
            
            _interstitialCallback = callbackState;
            _showDebugWindow = true;
            _currentCoroutine = StartCoroutine(AdProcess(() => {
                var state = (InterstitialState)_selectedInterstitialState;
                _interstitialCallback?.Invoke(state);
            }));
        }

        public void ShowRewarded(Action<RewardedState> callbackState)
        {
            if (_currentCoroutine != null) return;
            
            _rewardedCallback = callbackState;
            _showDebugWindow = true;
            _currentCoroutine = StartCoroutine(AdProcess(() => {
                var state = (RewardedState)_selectedRewardedState;
                _rewardedCallback?.Invoke(state);
            }));
        }

        private IEnumerator AdProcess(Action onComplete)
        {
            yield return new WaitForSeconds(_simulatedDelay);
            onComplete?.Invoke();
            _currentCoroutine = null;
            _showDebugWindow = false;
        }

        private void OnGUI()
        {
            if (!_showDebugWindow) return;

            GUI.Window(0, _windowRect, DrawDebugWindow, "Advertisement Debugger");
        }

        private void DrawDebugWindow(int windowID)
        {
            GUILayout.BeginVertical();
            
            if (_interstitialCallback != null)
            {
                GUILayout.Label("Interstitial Result:");
                _selectedInterstitialState = GUILayout.SelectionGrid(
                    _selectedInterstitialState,
                    _interstitialStates,
                    2);
            }
            else if (_rewardedCallback != null)
            {
                GUILayout.Label("Rewarded Result:");
                _selectedRewardedState = GUILayout.SelectionGrid(
                    _selectedRewardedState,
                    _rewardedStates,
                    2);
            }

            if (GUILayout.Button("Simulate Ad Close"))
            {
                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                    _currentCoroutine = null;
                }
                _showDebugWindow = false;
            }

            GUILayout.EndVertical();
            
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}