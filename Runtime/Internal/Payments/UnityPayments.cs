using System;
using System.Collections.Generic;
using WelwiseGamesSDK.Shared;

namespace WelwiseGamesSDK.Internal.Payments
{
    public class UnityPayments : IPayments
    {
        public bool IsAvailable { get; }
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public bool IsInitialized { get; }
        public event Action Initialized;
        public void Purchase(string productId, string developerPayload = null)
        {
            throw new NotImplementedException();
        }

        public void Consume(string purchaseToken)
        {
            throw new NotImplementedException();
        }

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