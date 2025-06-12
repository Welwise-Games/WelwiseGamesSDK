using System;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Manages player-specific data storage.
    /// Implement this interface to handle player profiles and game state.
    /// </summary>
    public interface IPlayerData
    {
        /// <summary>
        /// Event triggered when player data finishes loading
        /// </summary>
        public event Action Loaded;
        
        /// <summary>
        /// Retrieves the player's display name
        /// </summary>
        public string GetPlayerName();
        
        /// <summary>
        /// Sets the player's display name
        /// </summary>
        /// <param name="name">New display name</param>
        public void SetPlayerName(string name);
        
        /// <summary>
        /// Indicates if player data has been loaded
        /// </summary>
        public bool IsLoaded { get; }
        
        /// <summary>
        /// Initiates asynchronous loading of player data
        /// </summary>
        public void Load();
        
        /// <summary>
        /// Provides access to game-specific storage
        /// </summary>
        public IData GameData { get; }
        
        /// <summary>
        /// Provides access to cross-game metaverse storage
        /// </summary>
        public IData MetaverseData { get; }
        
        /// <summary>
        /// Persists current player data to storage
        /// </summary>
        public void Save();
    }
}