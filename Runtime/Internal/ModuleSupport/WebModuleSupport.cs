using System.Collections.Generic;
using Newtonsoft.Json;

namespace WelwiseGamesSDK.Internal.ModuleSupport
{
    internal sealed class WebModuleSupport : IModuleSupport
    {
        private readonly HashSet<string> _modules = new(
            JsonConvert.DeserializeObject<string[]>(PluginRuntime.GetAvailableModules())
        );

        public bool CheckModule(string moduleName) => _modules.Contains(moduleName);
    }
}