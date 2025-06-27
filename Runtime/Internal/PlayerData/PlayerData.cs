using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal abstract class PlayerData : IPlayerData
    {
        public event Action Saved;
        public IData GameData => _gameDataContainer;
        public IData MetaverseData => _metaverseDataContainer;
        
        protected readonly DataContainer _gameDataContainer;
        protected readonly DataContainer _metaverseDataContainer;

        protected PlayerData()
        {
            _gameDataContainer = new DataContainer(ValidateGameData);
            _metaverseDataContainer = new DataContainer(ValidateMetaverseData);
        }
        
        protected string _playerName;
        protected string _previousPlayerName;

        public event Action Loaded;
        public string GetPlayerName() => _playerName;

        public void SetPlayerName(string name) => _playerName = name;
        public bool IsLoaded { get; private set; }

        public abstract void Load();
        public abstract void Save();

        protected void OnLoaded()
        {
            IsLoaded = true;
            Loaded?.Invoke();
        }

        private bool ValidateGameData(string key) =>
            !_metaverseDataContainer.HasKey(key);
        
        private bool ValidateMetaverseData(string key) =>
            !_gameDataContainer.HasKey(key);
        
        protected void OnSaved() => Saved?.Invoke();
    }
}