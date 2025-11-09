using System;
using Newtonsoft.Json;

namespace WelwiseGames.Editor
{
    [Serializable]
    public sealed class SDKConfigField
    {
        [JsonProperty("name")]
        public string Name;
        
        [JsonProperty("type")]
        public string Type; // "string", "bool", "float", "int"
        
        [JsonProperty("displayName")]
        public string DisplayName;
        
        [JsonProperty("defaultValue")]
        public string DefaultValue;
        
        [JsonProperty("tooltip")]
        public string Tooltip;
    }
}