using System;
using Newtonsoft.Json;

namespace WelwiseGamesSDK.Shared.Types
{
    [Serializable]
    public class Purchase
    {
        [JsonProperty("productID")] public string ProductId;
        [JsonProperty("purchaseToken")] public string PurchaseToken;
        [JsonProperty("developerPayload")] public string DeveloperPayload;
        [JsonProperty("signature")] public string Signature;
    }
}