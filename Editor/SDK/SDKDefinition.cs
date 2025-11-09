using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WelwiseGames.Editor.SDK
{
    [Serializable]
    public sealed class SDKDefinition
    {
        [JsonProperty("name")]
        public string Name;
        
        [JsonProperty("adapterFile")]
        public string AdapterFile;
        
        [JsonProperty("configFields")]
        public List<SDKConfigField> ConfigFields = new ();
        
        [JsonProperty("postBuildScripts")]
        public List<PostBuildScript> PostBuildScripts = new ();
    }
}