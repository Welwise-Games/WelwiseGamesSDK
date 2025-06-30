using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using WelwiseGamesSDK.Shared;
using WelwiseGamesSDK.Shared.Modules;
using WelwiseGamesSDK.Shared.Types;

namespace WelwiseGamesSDK.Internal.Payments
{
    public class WebPayments : IPayments
    {        
        public event Action Initialized;
        public event Action<Purchase> PurchaseSuccess;
        public event Action<string, string> PurchaseFailed;
        public event Action<string> ConsumeSuccess;
        public event Action<string, string> ConsumeFailed;
        public event Action CatalogUpdated;
        public event Action PurchasesUpdated;
        public event Action<string> CatalogLoadFailed;
        public event Action<string> PurchasesLoadFailed;
        
        private List<Product> _products = new List<Product>();
        private List<Purchase> _purchases = new List<Purchase>();
        public bool IsAvailable { get; }
        public bool IsInitialized => _isInitialized;
        
        private bool _isInitialized;

        public IReadOnlyList<Product> Products => _products.AsReadOnly();
        public IReadOnlyList<Purchase> Purchases => _purchases.AsReadOnly();
        
        public WebPayments(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

        public void Initialize()
        {
            try
            {
                PluginRuntime.PaymentsInit(
                    onSuccess: () => 
                    {
                        _isInitialized = true;
                        Initialized?.Invoke();
                    },
                    onError: (error) => 
                    {
                        Debug.LogError($"Payments initialization failed: {error}");
                    }
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Payments initialization exception: {e.Message}");
            }
        }

        public void LoadCatalog()
        {
            PluginRuntime.PaymentsGetCatalog(
                onSuccess: (json) => 
                {
                    try
                    {
                        var products = JsonConvert.DeserializeObject<Product[]>(json);
                        _products = products.ToList();
                        CatalogUpdated?.Invoke();
                    }
                    catch (Exception e)
                    {
                        CatalogLoadFailed?.Invoke($"Catalog deserialization failed: {e.Message}");
                    }
                },
                onError: (error) => 
                {
                    CatalogLoadFailed?.Invoke(error);
                }
            );
        }

        public void LoadPurchases()
        {
            PluginRuntime.PaymentsGetPurchases(
                onSuccess: (json) => 
                {
                    try
                    {
                        var purchases = JsonConvert.DeserializeObject<Purchase[]>(json);
                        _purchases = purchases.ToList();
                        PurchasesUpdated?.Invoke();
                    }
                    catch (Exception e)
                    {
                        PurchasesLoadFailed?.Invoke($"Purchases deserialization failed: {e.Message}");
                    }
                },
                onError: (error) => 
                {
                    PurchasesLoadFailed?.Invoke(error);
                }
            );
        }

        public void Purchase(string productId, string developerPayload = null)
        {
            try
            {
                PluginRuntime.PaymentsPurchase(productId, developerPayload,
                    onSuccess: (purchaseJson) => 
                    {
                        try
                        {
                            var purchase = JsonConvert.DeserializeObject<Purchase>(purchaseJson);
                            PurchaseSuccess?.Invoke(purchase);
                            LoadPurchases(); // Обновляем список покупок
                        }
                        catch (Exception e)
                        {
                            PurchaseFailed?.Invoke(productId, $"Purchase deserialization failed: {e.Message}");
                        }
                    },
                    onError: (error) => 
                    {
                        PurchaseFailed?.Invoke(productId, error);
                    }
                );
            }
            catch (Exception e)
            {
                PurchaseFailed?.Invoke(productId, $"Purchase failed: {e.Message}");
            }
        }

        public void Consume(string purchaseToken)
        {
            try
            {
                PluginRuntime.PaymentsConsume(
                    purchaseToken,
                    onSuccess: (token) => 
                    {
                        ConsumeSuccess?.Invoke(token);
                        LoadPurchases();
                    },
                    onError: (error) => 
                    {
                        ConsumeFailed?.Invoke(purchaseToken, error);
                    }
                );
            }
            catch (Exception e)
            {
                ConsumeFailed?.Invoke(purchaseToken, $"Consume failed: {e.Message}");
            }
        }
    }
}