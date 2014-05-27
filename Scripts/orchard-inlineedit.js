(function ($, window, ui) {
    "use strict";

    var $ui = $(ui);

    $ui.bind(ui.events.notifyEditorChanged, function() {
        console.log("editor changed");
    });

})(jQuery, window, window.orchard.inlineedit.ui);