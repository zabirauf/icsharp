// For intellisense/auto complete support
// Using CodeMirror for Syntax Completion

// Per page refresh execution
$([IPython.events]).on("app_initialized.NotebookApp", function () {
    $('head').append('<link rel="stylesheet" type="text/css" href="custom.css">');


    IPython.CodeCell.options_default['cm_config']['mode'] = 'csharp';

    CodeMirror.requireMode('csharp', function () {
        IPython.OutputArea.prototype._should_scroll = function () { return false }
        cells = IPython.notebook.get_cells();
        for (var i in cells) {
            c = cells[i];
            if (c.cell_type === 'code') {
                c.auto_highlight()
            }
        }
    });


});
