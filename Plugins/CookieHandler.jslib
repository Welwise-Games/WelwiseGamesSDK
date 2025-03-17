mergeInto(LibraryManager.library, {
    SetCookie: function (key, value, expireDays) {
        var keyStr = UTF8ToString(key);
        var valueStr = UTF8ToString(value);
        
        var date = new Date();
        date.setTime(date.getTime() + (expireDays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + date.toUTCString();

        document.cookie = 
            encodeURIComponent(keyStr) + "=" + encodeURIComponent(valueStr) 
            + "; " + expires + "; path=/";
    },
    GetCookie: function (key) {
        var keyStr = UTF8ToString(key);
        var encodedKey = encodeURIComponent(keyStr);
        var cookies = document.cookie.split(';');
        for (var i = 0; i < cookies.length; i++) {
            var cookie = cookies[i].trim();
            if (cookie.startsWith(encodedKey + '=')) {
                var value = decodeURIComponent(
                    cookie.substring(encodedKey.length + 1)
                );
                return Pointer_stringify(value);
            }
        }
        return "";
    }
});