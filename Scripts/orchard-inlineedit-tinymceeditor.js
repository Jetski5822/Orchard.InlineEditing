(function ($, window, inlineedit, tinymce) {
    "use strict";

    var $inlineedit = $(inlineedit);

    $inlineedit.bind(inlineedit.events.retrieveContent, function (event, scope, shapeEditor) {
        console.log('Processing Content: TinyMce Editor');
        
        if (tinymce != undefined) {
            console.log('Processing Content: TinyMce Editor triggering save');
            var activatedEditorId = $('#' + shapeEditor.attr("id") + ' div.tinymce').attr("id");
            var activatedEditor = tinymce.editors[activatedEditorId];

            if (activatedEditor.isDirty()) {
                var afterContent = activatedEditor.save();
                $('#' + activatedEditorId).parent('form').children('textarea').val(afterContent);
            }

            console.log('Processing Content: TinyMce Editor triggering saved');
        }

        console.log('Processed Content: TinyMce Editor');
    });

    $inlineedit.bind(inlineedit.events.editorPrepared, function (event, shape, shapeEditor) {
        console.log('Finalizing Editor');
        
        // TODO: http://blog.mirthlab.com/2008/11/13/dynamically-adding-and-removing-tinymce-instances-to-a-page/

        //var themeButtons1 = "search,replace,|,cut,copy,paste,|,undo,redo,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen,";
        //var themeButtons2 = "";

        //if ($(shape).innerWidth() < 300) {
        //    themeButtons1 = "search,replace,|,cut,copy,paste,|,undo,redo,|,link,unlink,charmap,emoticon,codeblock";
        //    themeButtons2 = "bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen,";
        //}

        var mediaPlugins = ',|';

        if (shapeEditor.MetadataType == 'Parts_Common_Body') {
            var selector = '#' + shape.id + ' div.tinymce';

            var editorShapeId = $(selector).attr('id');
            var newEditorShapeId = shape.id + '_' + editorShapeId;
            $(selector).attr('id', newEditorShapeId);

            tinymce.init({
                selector: '#' + newEditorShapeId,
                theme: 'modern',
                schema: 'html5',
                plugins: 'fullscreen,autoresize,searchreplace,link,charmap,code' + mediaPlugins.substr(2),
                toolbar: 'searchreplace,|,cut,copy,paste,|,undo,redo' + mediaPlugins + ',|,link,unlink,charmap,|,bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen,',
                convert_urls: false,
                valid_elements: '*[*]',
                // shouldn't be needed due to the valid_elements setting, but TinyMCE would strip script.src without it.
                extended_valid_elements: 'script[type|defer|src|language]',
                menubar: false,
                statusbar: false,
                skin: 'orchardlightgray',
                inline: true
            });
        }

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
})(jQuery, window, window.orchard.inlineedit.ui, window.tinymce);