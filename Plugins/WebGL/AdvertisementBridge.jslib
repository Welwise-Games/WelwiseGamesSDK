mergeInto(LibraryManager.library, {
    JsShowInterstitial: function (onClose, onError) {
        var onCloseStr = UTF8ToString(onClose);
        var onErrorStr = UTF8ToString(onError);

        console.log('[SIM] Starting interstitial simulation...');
        
        // Симуляция с задержкой 2 секунды
        setTimeout(function() {
            const success = Math.random() > 0.3;
            const wasShown = success && Math.random() > 0.5;

            if(success) {
                console.log('[SIM] Interstitial closed successfully');
                unityInstance.SendMessage('WebAdvertisement', onCloseStr, wasShown ? "true" : "false");
            } else {
                const error = {
                    code: 500,
                    message: 'Simulated interstitial error'
                };
                console.log('[SIM] Interstitial error:', error);
                unityInstance.SendMessage('WebAdvertisement', onErrorStr, JSON.stringify(error));
            }
        }, 2000);
    },

    JsShowRewarded: function (onOpen, onRewarded, onClose, onError) {
        var onOpenStr = UTF8ToString(onOpen);
        var onRewardedStr = UTF8ToString(onRewarded);
        var onCloseStr = UTF8ToString(onClose);
        var onErrorStr = UTF8ToString(onError);

        console.log('[SIM] Starting rewarded simulation...');
        
        // Симуляция с задержкой и случайными событиями
        setTimeout(function() {
            try {
                // Имитация открытия
                console.log('[SIM] Reward ad opened');
                unityInstance.SendMessage('WebAdvertisement', onOpenStr);
                
                // 70% шанс успешного показа
                if(Math.random() > 0.3) {
                    setTimeout(function() {
                        // Имитация награды
                        console.log('[SIM] User earned reward');
                        unityInstance.SendMessage('WebAdvertisement', onRewardedStr);
                        
                        setTimeout(function() {
                            console.log('[SIM] Reward ad closed');
                            unityInstance.SendMessage('WebAdvertisement', onCloseStr);
                        }, 1000);
                    }, 2000);
                } else {
                    const error = {
                        code: 400,
                        message: 'Simulated video error'
                    };
                    console.log('[SIM] Reward error:', error);
                    unityInstance.SendMessage('WebAdvertisement', onErrorStr, JSON.stringify(error));
                }
            } catch(e) {
                console.log('[SIM] Reward error:', e);
                unityInstance.SendMessage('WebAdvertisement', onErrorStr, JSON.stringify(e));
            }
        }, 1000);
    }
});