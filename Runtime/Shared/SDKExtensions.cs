using System;
using System.Threading.Tasks;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Provides extension methods for SDK-related asynchronous operations.
    /// </summary>
    public static class SDKExtensions
    {
        
        /// <summary>
        /// Initializes the SDK asynchronously.
        /// </summary>
        /// <param name="sdk">SDK instance to initialize</param>
        /// <returns>Task representing initialization process</returns>
        public static Task InitializeAsync(this ISDK sdk)
        {
            if (sdk.IsInitialized)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            Action handler = null;

            handler = () =>
            {
                sdk.Initialized -= handler;
                tcs.TrySetResult(true);
            };

            sdk.Initialized += handler;
            sdk.Initialize();

            return tcs.Task;
        }
        
        /// <summary>
        /// Loads player data asynchronously.
        /// </summary>
        /// <param name="playerData">Player data instance to load</param>
        /// <returns>Task representing loading process</returns>
        public static Task LoadAsync(this IPlayerData playerData)
        {
            if (playerData.IsLoaded)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            Action handler = null;

            handler = () =>
            {
                playerData.Loaded -= handler;
                tcs.TrySetResult(true);
            };

            playerData.Loaded += handler;
            playerData.Load();

            return tcs.Task;
        }
        
        /// <summary>
        /// Checks if environment represents a desktop device.
        /// </summary>
        public static bool IsDesktop(this IEnvironment environment) => environment.DeviceType == DeviceType.Desktop;
        /// <summary>
        /// Checks if environment represents a mobile device (phone or tablet).
        /// </summary>
        public static bool IsMobile(this IEnvironment environment) => environment.DeviceType is DeviceType.Mobile or DeviceType.Tablet;
    }
}