mergeInto(LibraryManager.library, {
     ChangeGame: function (id) {
        var idStr = UTF8ToString(id);
        console.log('[SIM] CHANGE GAME ' + idStr);
    }
});