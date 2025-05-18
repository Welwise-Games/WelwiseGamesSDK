namespace WelwiseGamesSDK.Shared
{
    public interface IPlayerData
    {
        public string GetPlayerName();
        public void SetPlayerName(string name);
        
        public IData GameData { get; }
        public IData MetaverseData { get; }
        
        public void Save();
    }
}