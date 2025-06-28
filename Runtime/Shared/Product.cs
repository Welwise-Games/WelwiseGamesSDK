using Newtonsoft.Json;

namespace WelwiseGamesSDK.Shared
{
    public struct Product
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("imageURI")]
        public string ImageUri { get; set; }
        
        [JsonProperty("price")]
        public string Price { get; set; }
        
        [JsonProperty("priceCurrencyCode")]
        public string PriceCurrencyCode { get; set; }
        
        [JsonProperty("priceValue")]
        public string PriceValue { get; set; }
        
        public string CurrencyImageUrl => 
            !string.IsNullOrEmpty(PriceCurrencyCode) ? 
                $"//yastatic.net/s3/games-static/static-data/images/payments/sdk/currency-icon-s@2x.png?currency={PriceCurrencyCode}" : 
                string.Empty;
    }
}