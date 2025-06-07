
using WelwiseGamesSDK.Internal;

namespace WelwiseGamesSDK.Shared
{
    public sealed class SDKBuilder
    {
        private readonly SDKSettings _settings;
        
        internal SDKBuilder(SDKSettings settings)
        {
            _settings = settings;
        }

        public void AsSingle()
        {
            if (WelwiseSDK.Instance != null) return;
            WelwiseSDK.SetSDK(Build());
        }

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