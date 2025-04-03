mergeInto(LibraryManager.library, {
    GetDeviceType: function () {
        var userAgent = navigator.userAgent.toLowerCase();
        // 0 - Desktop, 1 - Mobile, 2 - Tablet
        if (/mobile|android|iphone|ipod|blackberry|windows phone/.test(userAgent)) return 1;
        if (/tablet|ipad|playbook|silk|android(?!.*mobile)/.test(userAgent)) return 2;
        return 0;
    },
    GetLanguage: function () {
        return (navigator.language || navigator.userLanguage).split('-')[0];
    }
});