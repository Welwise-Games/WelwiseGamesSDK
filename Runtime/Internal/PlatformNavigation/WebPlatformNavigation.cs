using System;
using UnityEngine;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlatformNavigation
{
    internal sealed class WebPlatformNavigation : IPlatformNavigation
    {
        private readonly PlayerData.PlayerData _playerData;
        private bool _isNavigating;
        private int _currentId;
        private Action<string> _currentCallback;

        public WebPlatformNavigation(PlayerData.PlayerData playerData)
        {
            _playerData = playerData;
            _playerData.Saved += PlayerDataOnSaved;
        }

        private void PlayerDataOnSaved()
        {
            if (!_isNavigating) return;
            Debug.Log($"GAME ID={_currentId}");
            PluginRuntime.GoToGame(_currentId, () => { }, _currentCallback);
        }

        public void GoToGame(int id, Action<string> onError)
        {
            _currentId = id;
            _currentCallback = onError;
            
            if (_isNavigating) return;
            _isNavigating = true;
            _playerData.Save();
        }
    }
}