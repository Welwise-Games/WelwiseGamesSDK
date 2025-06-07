using System;
using System.Threading.Tasks;

namespace WelwiseGamesSDK.Shared
{
    public static class SDKExtensions
    {
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
        
        public static bool IsDesktop(this IEnvironment environment) => environment.DeviceType == DeviceType.Desktop;
        public static bool IsMobile(this IEnvironment environment) => environment.DeviceType == DeviceType.Mobile || environment.DeviceType == DeviceType.Tablet;
    }
}