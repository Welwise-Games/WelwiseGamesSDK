using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class WebPlatformNavigation : IPlatformNavigation
    {
        public bool IsAvailable { get; }
        private readonly PlayerData.PlayerData _playerData;
        private bool _isNavigating;
        private string _currentId;
        private Action<string> _currentCallback;

        public WebPlatformNavigation(PlayerData.PlayerData playerData, bool isAvailable)
        {
            _playerData = playerData;
            IsAvailable = isAvailable;
            _playerData.Saved += PlayerDataOnSaved;
        }

        private void PlayerDataOnSaved()
        {
            if (!_isNavigating) return;
            Debug.Log($"GAME ID={_currentId}");
            PluginRuntime.GoToGame(_currentId, () => { }, _currentCallback);
        }

        public void GoToGame(string id, Action<string> onError)
        {
            _currentId = id;
            _currentCallback = onError;
            
            if (_isNavigating) return;
            _isNavigating = true;
            _playerData.Save();
        }
    }
}