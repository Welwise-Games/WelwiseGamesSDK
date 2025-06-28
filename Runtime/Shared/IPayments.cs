using System;
using System.Collections.Generic;

namespace WelwiseGamesSDK.Shared
{
    public interface IPayments : IModule, IInitializable
    {
        // Основные методы
        public void Purchase(string productId, string developerPayload = null);
        public void Consume(string purchaseToken);
        
        // Новые методы для загрузки данных
        public void LoadCatalog();
        public void LoadPurchases();
        
        // Свойства
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<Purchase> Purchases { get; }
        
        // События
        public event Action<Purchase> PurchaseSuccess;
        public event Action<string, string> PurchaseFailed;
        public event Action<string> ConsumeSuccess;
        public event Action<string, string> ConsumeFailed;
        public event Action CatalogUpdated;
        public event Action PurchasesUpdated;
        
        // События для ошибок загрузки
        public event Action<string> CatalogLoadFailed;
        public event Action<string> PurchasesLoadFailed;
    }
}