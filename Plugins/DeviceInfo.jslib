mergeInto(LibraryManager.library, {
    GetDeviceType: function () {
        var userAgent = navigator.userAgent.toLowerCase();
        if (/mobile/.test(userAgent)) return "Mobile";
        if (/tablet|ipad|android.*?wv/.test(userAgent)) return "Tablet";
        return "Desktop";
    },
    GetLanguage: function () {
        return (navigator.language || navigator.userLanguage).split('-')[0];
    }
});