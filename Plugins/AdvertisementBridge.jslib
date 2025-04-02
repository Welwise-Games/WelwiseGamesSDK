mergeInto(LibraryManager.library, {
    JsShowInterstitial: function () {
        window.dispatchEvent(new CustomEvent('unityShowInterstitial', {
            detail: {
                callback: function(state) {
                    SendMessage('Advertisement', 'OnInterstitialState', state);
                }
            }
        }));
    },

    JsShowRewarded: function () {
        window.dispatchEvent(new CustomEvent('unityShowRewarded', {
            detail: {
                callback: function(state) {
                    SendMessage('WebAdvertisement', 'OnRewardedState', state);
                }
            }
        }));
    }
});