using WelwiseGamesSDK.Internal;

namespace WelwiseGamesSDK.Shared
{
    /// <summary>
    /// Constructs SDK instances with platform-specific implementations.
    /// Use this class to create singleton or transient SDK instances.
    /// </summary>
    public sealed class SDKBuilder
    {
        private readonly SDKSettings _settings;
        
        internal SDKBuilder(SDKSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Registers the SDK as a global singleton instance.
        /// Safe to call multiple times (only first call has effect).
        /// </summary>
        public void AsSingle()
        {
            if (WelwiseSDK.Instance != null) return;
            WelwiseSDK.SetSDK(Build());
        }

        /// <summary>
        /// Creates a transient SDK instance without singleton registration.
        /// </summary>
        /// <returns>New SDK instance</returns>
        public ISDK AsTransient() => Build();

        private ISDK Build()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return new WebSDK(_settings);
#else
            return new UnitySDK(_settings);
#endif
        }
    }
}