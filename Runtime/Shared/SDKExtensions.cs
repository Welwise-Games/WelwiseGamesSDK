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
        public static Task InitializeAsync(this ISDK sdk)
        {
            if (sdk.IsInitialized)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();

            sdk.Initialized += Handler;
            sdk.Initialize();

            return tcs.Task;

            void Handler()
            {
                sdk.Initialized -= Handler;
                tcs.TrySetResult(true);
            }
        }
        
        /// <summary>
        /// Loads player data asynchronously.
        /// </summary>
        public static Task LoadAsync(this IPlayerData playerData)
        {
            if (playerData.IsLoaded)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();

            playerData.Loaded += Handler;
            playerData.Load();

            return tcs.Task;

            void Handler()
            {
                playerData.Loaded -= Handler;
                tcs.TrySetResult(true);
            }
        }
        
        /// <summary>
        /// Checks if environment represents a desktop device.
        /// </summary>
        public static bool IsDesktop(this IEnvironment environment) => 
            environment.DeviceType == DeviceType.Desktop;
            
        /// <summary>
        /// Checks if environment represents a mobile device (phone or tablet).
        /// </summary>
        public static bool IsMobile(this IEnvironment environment) => 
            environment.DeviceType is DeviceType.Mobile or DeviceType.Tablet;
        
        /// <summary>
        /// Initializes the payments module asynchronously.
        /// </summary>
        public static Task InitializeAsync(this IPayments payments)
        {
            if (payments.IsInitialized)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();

            payments.Initialized += Handler;
            payments.Initialize();

            return tcs.Task;

            void Handler()
            {
                payments.Initialized -= Handler;
                tcs.TrySetResult(true);
            }
        }

        /// <summary>
        /// Loads product catalog asynchronously.
        /// </summary>
        public static Task LoadCatalogAsync(this IPayments payments)
        {
            var tcs = new TaskCompletionSource<bool>();

            payments.CatalogUpdated += SuccessHandler;
            payments.CatalogLoadFailed += ErrorHandler;
            payments.LoadCatalog();

            return tcs.Task;

            void SuccessHandler()
            {
                payments.CatalogUpdated -= SuccessHandler;
                payments.CatalogLoadFailed -= ErrorHandler;
                tcs.TrySetResult(true);
            }

            void ErrorHandler(string error)
            {
                payments.CatalogUpdated -= SuccessHandler;
                payments.CatalogLoadFailed -= ErrorHandler;
                tcs.TrySetException(new Exception($"Catalog load failed: {error}"));
            }
        }

        /// <summary>
        /// Loads purchases list asynchronously.
        /// </summary>
        public static Task LoadPurchasesAsync(this IPayments payments)
        {
            var tcs = new TaskCompletionSource<bool>();

            payments.PurchasesUpdated += SuccessHandler;
            payments.PurchasesLoadFailed += ErrorHandler;
            payments.LoadPurchases();

            return tcs.Task;

            void ErrorHandler(string error)
            {
                payments.PurchasesUpdated -= SuccessHandler;
                payments.PurchasesLoadFailed -= ErrorHandler;
                tcs.TrySetException(new Exception($"Purchases load failed: {error}"));
            }

            void SuccessHandler()
            {
                payments.PurchasesUpdated -= SuccessHandler;
                payments.PurchasesLoadFailed -= ErrorHandler;
                tcs.TrySetResult(true);
            }
        }
    }
}