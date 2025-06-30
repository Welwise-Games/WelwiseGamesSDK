using System;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal abstract class PlayerData : IPlayerData
    {
        public event Action Saved;
        public event Action Initialized;
        public bool IsAvailable { get; }
        public bool IsInitialized { get; private set; }

        public IData GameData => _gameDataContainer;
        public IData MetaverseData => _metaverseDataContainer;
        
        protected readonly DataContainer _gameDataContainer;
        protected readonly DataContainer _metaverseDataContainer;

        protected PlayerData(bool isAvailableSelf, bool isGameDataAvailable, bool isMetaverseDataAvailable)
        {
            IsAvailable = isAvailableSelf;
            _gameDataContainer = new DataContainer(ValidateGameData, isGameDataAvailable);
            _metaverseDataContainer = new DataContainer(ValidateMetaverseData, isMetaverseDataAvailable);
        }
        
        protected string _playerName;
        protected string _previousPlayerName;

        public string GetPlayerName() => _playerName;

        public void SetPlayerName(string name) => _playerName = name;

        public abstract void Initialize();
        public abstract void Save();

        protected void OnLoaded()
        {
            IsInitialized = true;
            Initialized?.Invoke();
        }

        private bool ValidateGameData(string key) =>
            !_metaverseDataContainer.HasKey(key);
        
        private bool ValidateMetaverseData(string key) =>
            !_gameDataContainer.HasKey(key);
        
        protected void OnSaved() => Saved?.Invoke();
    }
}