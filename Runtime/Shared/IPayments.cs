using System;
using System.Collections.Generic;

namespace WelwiseGamesSDK.Shared
{
    public interface IPayments : IModule, IInitializable
    {
        public void Purchase(string productId, string developerPayload = null);
        public void Consume(string purchaseToken);
        
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<Purchase> Purchases { get; }
        
        public event Action<Purchase> PurchaseSuccess;
        public event Action<string, string> PurchaseFailed;
        public event Action<string> ConsumeSuccess;
        public event Action<string, string> ConsumeFailed;
        public event Action CatalogUpdated;
        public event Action PurchasesUpdated;
    }
}