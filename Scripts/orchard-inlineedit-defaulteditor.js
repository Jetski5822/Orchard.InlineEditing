(function ($, window, inlineedit) {
    "use strict";

    var $inlineedit = $(inlineedit);

    $inlineedit.bind(inlineedit.events.retrieveContent, function (event, scope, shapeEditor) {
        console.log('Processing Content: Default Editor');

        console.log('Processed Content: Default Editor');
    });

    $inlineedit.bind(inlineedit.events.editorPrepared, function(event, shape, shapeEditor) {
        console.log('Finalizing Editor');

        var tagName = $('.editable-content', $(shape).parent()).children().first().prop('tagName');
        

        console.log('Finalized Editor');
    });

    var editor = {
        
    };

    if (!window.orchard) {
        window.orchard = {};
    }

    if (!window.orchard.inlineedit) {
        window.orchard.inlineedit = {};
    }

    if (!window.orchard.inlineedit) {
        window.orchard.inlineedit.ui = {};
    }

    window.orchard.inlineedit.defaulteditor = editor;
})(jQuery, window, window.orchard.inlineedit.ui);