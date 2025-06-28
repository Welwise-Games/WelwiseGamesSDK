using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Internal;

namespace WelwiseGamesSDK.Internal.Payments
{
    public class UnityPayments : IPayments
    {
        private SDKSettings _settings;
        private List<Product> _products = new List<Product>();
        private List<Purchase> _purchases = new List<Purchase>();
        
        public bool IsAvailable => true;
        public bool IsInitialized { get; private set; }
        public IReadOnlyList<Product> Products => _products.AsReadOnly();
        public IReadOnlyList<Purchase> Purchases => _purchases.AsReadOnly();
        
        public event Action Initialized;
        public event Action<Purchase> PurchaseSuccess;
        public event Action<string, string> PurchaseFailed;
        public event Action<string> ConsumeSuccess;
        public event Action<string, string> ConsumeFailed;
        public event Action CatalogUpdated;
        public event Action PurchasesUpdated;
        public event Action<string> CatalogLoadFailed;
        public event Action<string> PurchasesLoadFailed;

        public void Initialize()
        {
            if (IsInitialized) return;
            
            _settings = SDKSettings.LoadOrCreateSettings();
            _products = new List<Product>(_settings.MockProducts);
            _purchases = new List<Purchase>(_settings.MockPurchases);
            
            PluginRuntime.StartCoroutine(SimulateInitialization());
        }

        private IEnumerator SimulateInitialization()
        {
            yield return new WaitForSeconds(_settings.DebugInitializeTime);
            IsInitialized = true;
            Initialized?.Invoke();
        }

        public void Purchase(string productId, string developerPayload = null)
        {
            if (!IsInitialized)
            {
                PurchaseFailed?.Invoke(productId, "Payments not initialized");
                return;
            }

            PluginRuntime.StartCoroutine(SimulatePurchase(productId, developerPayload));
        }

        private IEnumerator SimulatePurchase(string productId, string developerPayload)
        {
            int productIndex = _products.FindIndex(p => p.Id == productId);
            if (productIndex < 0)
            {
                PurchaseFailed?.Invoke(productId, "Product not found");
                yield break;
            }
            yield return new WaitForSeconds(_settings.PurchaseSimulationDuration);
            var purchase = new Purchase
            {
                ProductId = productId,
                PurchaseToken = Guid.NewGuid().ToString(),
                DeveloperPayload = developerPayload,
                Signature = "fake_signature_" + Guid.NewGuid()
            };

            _purchases.Add(purchase);
            PurchaseSuccess?.Invoke(purchase);
            PurchasesUpdated?.Invoke();
        }

        public void Consume(string purchaseToken)
        {
            if (!IsInitialized)
            {
                ConsumeFailed?.Invoke(purchaseToken, "Payments not initialized");
                return;
            }

            PluginRuntime.StartCoroutine(SimulateConsume(purchaseToken));
        }

        private IEnumerator SimulateConsume(string purchaseToken)
        {
            var index = _purchases.FindIndex(p => p.PurchaseToken == purchaseToken);
            if (index < 0)
            {
                ConsumeFailed?.Invoke(purchaseToken, "Purchase not found");
                yield break;
            }
            yield return new WaitForSeconds(_settings.PurchaseSimulationDuration);
            _purchases.RemoveAt(index);
            ConsumeSuccess?.Invoke(purchaseToken);
            PurchasesUpdated?.Invoke();
        }

        public void LoadCatalog()
        {
            if (!IsInitialized)
            {
                CatalogLoadFailed?.Invoke("Payments not initialized");
                return;
            }

            PluginRuntime.StartCoroutine(SimulateCatalogLoad());
        }

        private IEnumerator SimulateCatalogLoad()
        {
            yield return new WaitForSeconds(_settings.DebugInitializeTime);
            
            _settings = SDKSettings.LoadOrCreateSettings();
            _products = new List<Product>(_settings.MockProducts);
            CatalogUpdated?.Invoke();
        }

        public void LoadPurchases()
        {
            if (!IsInitialized)
            {
                PurchasesLoadFailed?.Invoke("Payments not initialized");
                return;
            }

            PluginRuntime.StartCoroutine(SimulatePurchasesLoad());
        }

        private IEnumerator SimulatePurchasesLoad()
        {
            yield return new WaitForSeconds(_settings.DebugInitializeTime);
            _settings = SDKSettings.LoadOrCreateSettings();
            _purchases = new List<Purchase>(_settings.MockPurchases);
            PurchasesUpdated?.Invoke();
        }
    }
}