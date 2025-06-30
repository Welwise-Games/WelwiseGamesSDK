using System;
using Newtonsoft.Json;

namespace WelwiseGamesSDK.Shared.Types
{
    [Serializable]
    public class Product
    {
        [JsonProperty("id")] public string Id;
        [JsonProperty("title")] public string Title;
        [JsonProperty("description")] public string Description;
        [JsonProperty("imageURI")] public string ImageUri;
        [JsonProperty("price")] public string Price;
        [JsonProperty("priceCurrencyCode")] public string PriceCurrencyCode;
        [JsonProperty("priceValue")] public string PriceValue;
    }
}