using Newtonsoft.Json;

namespace WelwiseGamesSDK.Shared
{
    public struct Purchase
    {
        [JsonProperty("productID")]
        public string ProductId { get; set; }
        
        [JsonProperty("purchaseToken")]
        public string PurchaseToken { get; set; }
        
        [JsonProperty("developerPayload")]
        public string DeveloperPayload { get; set; }
        
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}