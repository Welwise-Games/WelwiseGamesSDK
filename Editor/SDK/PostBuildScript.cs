using System;
using Newtonsoft.Json;

namespace WelwiseGames.Editor.SDK
{
    
    [Serializable]
    public sealed class PostBuildScript
    {
        [JsonProperty("injectPoint")]
        public string InjectPoint; // "head", "before_body_end", "after_body_start"
        
        [JsonProperty("file")]
        public string File;
    }
}