namespace WelwiseGamesSDK.Shared
{
    public struct Purchase
    {
        public string ProductId { get; set; }
        public string PurchaseToken { get; set; }
        public string DeveloperPayload { get; set; }
        public string Signature { get; set; }
        public bool IsConsumed { get; set; }
    }
}