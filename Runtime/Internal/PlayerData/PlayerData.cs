using System;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.PlayerData
{
    internal abstract class PlayerData : IPlayerData
    {
        public IData GameData => _gameDataContainer;
        public IData MetaverseData => _metaverseDataContainer;
        
        protected readonly DataContainer _gameDataContainer = new ();
        protected readonly DataContainer _metaverseDataContainer = new ();
        
        protected string _playerName;

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
    }
}