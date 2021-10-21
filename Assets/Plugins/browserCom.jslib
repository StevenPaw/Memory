mergeInto(LibraryManager.library, {

  SendMessageToBrowser: function (str) {
    window.alert(Pointer_stringify(str));
  },

});