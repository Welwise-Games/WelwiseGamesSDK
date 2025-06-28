mergeInto(LibraryManager.library, {
    JSInit: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.Init === 'function') {
            window.__sdk_adapter.Init();
        } else {
            console.error("SDK Adapter: Init not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleInitError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetPlayerData: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetPlayerData === 'function') {
            window.__sdk_adapter.GetPlayerData();
        } else {
            console.error("SDK Adapter: GetPlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSSetPlayerData: function(jsonDataPtr) {
        var jsonString = UTF8ToString(jsonDataPtr);
        if (window.__sdk_adapter && typeof window.__sdk_adapter.SetPlayerData === 'function') {
            window.__sdk_adapter.SetPlayerData(jsonString);
        } else {
            console.error("SDK Adapter: SetPlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleSetDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetServerTime: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetServerTime === 'function') {
            window.__sdk_adapter.GetServerTime();
        } else {
            console.error("SDK Adapter: GetServerTime not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetTimeError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGoToGame: function(gameId) {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GoToGame === 'function') {
            window.__sdk_adapter.GoToGame(gameId);
        } else {
            console.error("SDK Adapter: JsGoToGame not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleNavigateError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetPlayerId: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetPlayerId === 'function') {
            window.__sdk_adapter.GetPlayerId();
        } else {
            console.error("SDK Adapter: GetPlayerId not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetPlayerIdError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetDeviceType: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetDeviceType === 'function') {
            window.__sdk_adapter.GetDeviceType();
        } else {
            console.error("SDK Adapter: GetDeviceType not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetDeviceTypeError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetLanguageCode: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetLanguageCode === 'function') {
            window.__sdk_adapter.GetLanguageCode();
        } else {
            console.error("SDK Adapter: GetLanguageCode not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetLanguageCodeError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetMetaversePlayerData: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetMetaversePlayerData === 'function') {
            window.__sdk_adapter.GetMetaversePlayerData();
        } else {
            console.error("SDK Adapter: GetMetaversePlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetMetaverseDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSSetMetaversePlayerData: function(jsonDataPtr) {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.SetMetaversePlayerData === 'function') {
            window.__sdk_adapter.SetMetaversePlayerData(jsonDataPtr);
        } else {
            console.error("SDK Adapter: SetMetaversePlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleSetMetaverseDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSIsMetaverseSupported: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.IsMetaverseSupported === 'function') {
            window.__sdk_adapter.IsMetaverseSupported();
        } else {
            console.error("SDK Adapter: IsMetaverseSupported not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleIsMetaverseSupportedError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGetCombinedPlayerData: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GetCombinedPlayerData === 'function') {
            window.__sdk_adapter.GetCombinedPlayerData();
        } else {
            console.error("SDK Adapter: GetCombinedPlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleGetCombinedDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSSetCombinedPlayerData: function(jsonDataPtr) {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.SetCombinedPlayerData === 'function') {
            window.__sdk_adapter.SetCombinedPlayerData(jsonDataPtr);
        } else {
            console.error("SDK Adapter: SetCombinedPlayerData not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleSetCombinedDataError',
                'SDK adapter not loaded'
            );
        }
    },

    JSGameReady: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GameReady === 'function') {
            window.__sdk_adapter.GameReady();
        } else {
            console.error("SDK Adapter: GameReady not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandleGameReadyError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSGameplayStart: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GameplayStart === 'function') {
            window.__sdk_adapter.GameplayStart();
        } else {
            console.error("SDK Adapter: GameplayStart not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandleGameplayStartError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSGameplayStop: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.GameplayStop === 'function') {
            window.__sdk_adapter.GameplayStop(); // Исправлено: было JsGameplayStop
        } else {
            console.error("SDK Adapter: GameplayStop not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandleGameplayStopError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSShowInterstitial: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.ShowInterstitial === 'function') {
            window.__sdk_adapter.ShowInterstitial();
        } else {
            console.error("SDK Adapter: ShowInterstitial not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleInterstitialError',
                'SDK adapter not loaded'
            );
        }
    },

    JSShowRewarded: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.ShowRewarded === 'function') {
            window.__sdk_adapter.ShowRewarded();
        } else {
            console.error("SDK Adapter: ShowRewarded not implemented");
            unityInstance.SendMessage(
                'PluginRuntime',
                'HandleRewardedError',
                'SDK adapter not loaded'
            );
        }
    },
    JSPaymentsInit: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.PaymentsInit === 'function') {
            window.__sdk_adapter.PaymentsInit();
        } else {
            console.error("SDK Adapter: PaymentsInit not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandlePaymentsInitError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSPaymentsGetCatalog: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.PaymentsGetCatalog === 'function') {
            window.__sdk_adapter.PaymentsGetCatalog();
        } else {
            console.error("SDK Adapter: PaymentsGetCatalog not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandlePaymentsGetCatalogError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSPaymentsGetPurchases: function() {
        if (window.__sdk_adapter && typeof window.__sdk_adapter.PaymentsGetPurchases === 'function') {
            window.__sdk_adapter.PaymentsGetPurchases();
        } else {
            console.error("SDK Adapter: PaymentsGetPurchases not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandlePaymentsGetPurchasesError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSPaymentsPurchase: function(productIdPtr, developerPayloadPtr) {
        const productId = UTF8ToString(productIdPtr);
        const developerPayload = UTF8ToString(developerPayloadPtr);

        if (window.__sdk_adapter && typeof window.__sdk_adapter.PaymentsPurchase === 'function') {
            window.__sdk_adapter.PaymentsPurchase(productId, developerPayload);
        } else {
            console.error("SDK Adapter: PaymentsPurchase not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandlePaymentsPurchaseError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    },

    JSPaymentsConsume: function(purchaseTokenPtr) {
        const purchaseToken = UTF8ToString(purchaseTokenPtr);

        if (window.__sdk_adapter && typeof window.__sdk_adapter.PaymentsConsume === 'function') {
            window.__sdk_adapter.PaymentsConsume(purchaseToken);
        } else {
            console.error("SDK Adapter: PaymentsConsume not implemented");
            try {
                unityInstance.SendMessage('PluginRuntime', 'HandlePaymentsConsumeError', 'SDK adapter not loaded');
            } catch (e) {
                console.error("Error sending error message:", e);
            }
        }
    }
});