using System;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Main entry point for SDK functionality.
    /// Provides access to all core SDK services.
    /// </summary>
    public interface ISDK
    {
        /// <summary>
        /// Player profile and data management
        /// </summary>
        public IPlayerData PlayerData { get; }
        
        /// <summary>
        /// Runtime environment information
        /// </summary>
        public IEnvironment Environment { get; }
        
        /// <summary>
        /// Advertisement display capabilities
        /// </summary>
        public IAdvertisement Advertisement { get; }
        
        /// <summary>
        /// Analytics tracking services
        /// </summary>
        public IAnalytics Analytics { get; }
        
        /// <summary>
        /// Cross-game navigation features
        /// </summary>
        public IPlatformNavigation PlatformNavigation { get; }
        
        /// <summary>
        /// Initializes the SDK services
        /// </summary>
        public void Initialize();
        
        /// <summary>
        /// Event triggered when SDK initialization completes
        /// </summary>
        public event Action Initialized;
        
        /// <summary>
        /// Indicates if SDK has been initialized
        /// </summary>
        public bool IsInitialized { get; }
    }
}