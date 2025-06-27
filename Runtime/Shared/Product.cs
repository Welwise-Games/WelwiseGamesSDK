namespace WelwiseGamesSDK.Shared
{
    public struct Product
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUri { get; set; }
        public string Price { get; set; }
        public string PriceCurrencyCode { get; set; }
        public decimal PriceValue { get; set; }
        public string CurrencyImageUrl { get; set; }
        public bool IsConsumable { get; set; }
    }
}