using System;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Shared.Modules
{
    /// <summary>
    /// Provides information about the runtime environment.
    /// Implement this interface to access device-specific information.
    /// </summary>
    public interface IEnvironment : IModule
    {
        /// <summary>
        /// Unique identifier of the current player
        /// </summary>
        public Guid PlayerId { get; }
        
        /// <summary>
        /// Type of device running the game
        /// </summary>
        public DeviceType DeviceType { get; }
        
        /// <summary>
        /// Current language
        /// </summary>
        public string Language { get; }
        
        /// <summary>
        /// Retrieves current server time in UTC milliseconds
        /// </summary>
        /// <param name="callback">Callback receiving server timestamp</param>
        public void RequestServerTime(Action<long> callback);
    }
}