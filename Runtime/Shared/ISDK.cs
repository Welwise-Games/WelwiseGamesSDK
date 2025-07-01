using System;
using WelwiseGamesSDK.Shared.Modules;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Main entry point for SDK functionality.
    /// Provides access to all core SDK services.
    /// </summary>
    public interface ISDK : IInitializable
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
        /// In-app purchases
        /// </summary>
        public IPayments Payments { get; }
    }
}