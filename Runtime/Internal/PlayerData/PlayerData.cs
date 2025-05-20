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
        
        public string GetPlayerName() => _playerName;

        public void SetPlayerName(string name) => _playerName = name;

        public abstract void Load();
        public abstract void Save();
    }
}