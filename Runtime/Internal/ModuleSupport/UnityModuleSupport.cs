using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.ModuleSupport
{
    internal class UnityModuleSupport : IModuleSupport
    {
        private readonly SDKSettings _settings;

        public UnityModuleSupport(SDKSettings settings)
        {
            _settings = settings;
        }
        
        public bool CheckModule(string moduleName)
        {
            return moduleName switch
            {
                SupportedModuleKeys.AdvertisementModuleKey 
                    => _settings.EditorAdvertisementModule,
                SupportedModuleKeys.PaymentsModuleKey 
                    => _settings.EditorPaymentsModule,
                SupportedModuleKeys.AnalyticsModuleKey 
                    => _settings.EditorAnalyticsModule,
                SupportedModuleKeys.EnvironmentModuleKey 
                    => _settings.EditorEnvironmentModule,
                SupportedModuleKeys.PlatformNavigationModuleKey 
                    => _settings.EditorPlatformNavigationModule,
                SupportedModuleKeys.PlayerDataModuleKey 
                    => _settings.EditorPlayerDataModule,
                SupportedModuleKeys.GameDataModuleKey 
                    => _settings.EditorGameDataModule,
                SupportedModuleKeys.MetaverseDataModuleKey 
                    => _settings.EditorMetaverseDataModule,
                _ => false
            };
        }
    }
}