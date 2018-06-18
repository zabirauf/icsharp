define(function () {
    var kernelSpecs = requirejs.s.contexts._.config.paths.kernelspecs;
    var staticFolder = kernelSpecs + "/csharp/static/";
    var resourcesFolder = staticFolder + "resources/";
    
    var link = document.createElement("link");
    link.type = "text/css";
    link.rel = "stylesheet";
    link.href = staticFolder + "custom/csharp.css";
    document.getElementsByTagName("head")[0].appendChild(link);

    var changedSinceTypecheck = true;
    var changedRecently = false;
    var kernelIdledSinceTypeCheck = false;
    var kernelIdledRecently = false

    var onload = function () {

        var md = IPython.notebook.metadata;
        if (md.language) {
            console.log('language already defined and is :', md.language);
        }
        else {
            md.language = 'csharp';
            console.log('add metadata hint that language is csharp...');
        }

        IPython.CodeCell.options_default.cm_config.mode = 'csharp';

        // callback called by the end-user
        function updateMarkers(data) {
            // applies intellisense hooks onto all cells
            var cells = getCodeCells();

            // clear old error marks
            cells.cells.forEach(function (cell) {
                cell.code_mirror.doc.getAllMarks().forEach(function (m) {
                    if (m.className === 'br-errormarker') {
                        m.clear();
                    }
                });
            });

            // apply new error marks
            data.forEach(function (err) {
                var editor = cells.cells[err.CellNumber].code_mirror;
                var from = { line: err.StartLine, ch: err.StartColumn };
                var to = { line: err.EndLine, ch: err.EndColumn };
                editor.doc.markText(from, to, { title: err.Message, className: 'br-errormarker' });
            });
        }

        function getCodeCells() {
            var results = { codes: [], cells: [], string_cells: [], selectedCell: null, selectedIndex: 0 };
            IPython.notebook.get_cells()
                .forEach(function (c) {
                    if (c.cell_type === 'code') {
                        c.completer = null; // KILLS THE DEFAULT COMPLETER (EXTRA PRECAUTION AGAINST FAILURES)
                        if (c.selected === true) {
                            results.selectedCell = c;
                            results.selectedIndex = results.cells.length;
                        }
                        results.cells.push(c);
                        results.string_cells.push(JSON.stringify(c.code_mirror.getValue()));
                        results.codes.push(c.code_mirror.getValue());
                       // console.log('Code: ' + c.code_mirror.getValue()); // CONSOLE LOG
                    }
                });

            return results;
        }

        function getLine(cell, line){
            var lines = cell.split("\n");
            return lines[line];
        }

        function getTrueCurPos(cell, line, ch){
            var lines = cell.split("\n");
            console.log(lines.length);
            var calibrated = 0;
            if (line <= 0) return ch;

            for (var i = 0; i < line; i++) {
                calibrated = calibrated + lines[i].length + 1;
            }
            return calibrated + ch;
        }


        function intellisenseRequestDeclaration(item) {
            var cells = getCodeCells();
            var editor = cells.selectedCell != null ? cells.selectedCell.code_mirror : null
            var cursor = editor != null ? editor.doc.getCursor() : { ch: 0, line: 0 }
            var callbacks = { shell: {}, iopub: {} };

            console.log("call backs: ");
            console.log(callbacks);
            callbacks.shell.reply = function (msg) {
                console.log('callback!');
                //console.log(msg);

                if (editor != null && item.keyCode !== 0) {
                    console.log('callback!');
                    console.log(msg);

                    editor.intellisense.setDeclarations(msg.content.matches);
                    editor.intellisense.setStartColumnIndex(msg.content.cursor_start);
                }
                
            };

            callbacks.iopub.output = function (msg) {
                updateMarkers(msg.content.data.errors);
            };

            var content = {
                code: JSON.stringify(cells.codes),
                code_cells: cells.string_cells,
                cursor_pos: cursor.ch,
                code_pos: getTrueCurPos(cells.codes[cells.selectedIndex], cursor.line, cursor.ch),
                line: JSON.stringify(getLine(cells.codes[cells.selectedIndex], cursor.line)),
                cursor_line: cursor.line,
                selected_cell: cells.selectedCell,
                selected_cell_index: cells.selectedIndex
            };

            console.log('intellisenseRequest!');
            console.log(content);

            IPython.notebook.kernel.send_shell_message("intellisense_request", content, callbacks, null, null);
        }

        function intellisenseRequestMethod(item) {
            var cells = getCodeCells();
            var editor = cells.selectedCell != null ? cells.selectedCell.code_mirror : null
            var cursor = editor != null ? editor.doc.getCursor() : { ch: 0, line: 0 }
            var callbacks = { shell: {}, iopub: {} };

                if (item.keyCode === 8)//|| item.keyCode === 48)
                {
                    editor.intellisense.getMeths().setVisible(false);
                }
                else
                {
                  
                

            console.log("call backs: method");
            console.log(callbacks);
            callbacks.shell.reply = function (msg) {
                console.log('callback!');
                //console.log(msg);

                if (editor != null && item.keyCode !== 0) {
                    console.log('callback method!');
                    console.log(msg);

             //   editor.intellisense.setMethods(['CompareTo(int)', 'CompareTo(Object)']);

               //     editor.intellisense.setMethods(msg.content.matches);

                }
             }

            callbacks.iopub.output = function (msg) {
                updateMarkers(msg.content.data.errors);
            };

            var content = {
                code: JSON.stringify(cells.codes),
                code_cells: cells.string_cells,
                cursor_pos: cursor.ch,
                line: JSON.stringify(getLine(cells.codes[cells.selectedIndex], cursor.line)),
                cursor_line: cursor.line,
                selected_cell: cells.selectedCell,
                selected_cell_index: cells.selectedIndex
            };

            console.log('intellisenseRequest!');
            console.log(content);

            IPython.notebook.kernel.send_shell_message("intellisense_request", content, callbacks, null, null);
        }
    }

        //There are dependencies in the lazy loading 
        require(['codemirror/addon/mode/loadmode'], function () {
            require([staticFolder + 'custom/csharp.js'], function () {
                require([staticFolder + 'custom/webintellisense.js', staticFolder + 'custom/webintellisense-codemirror.js'], function () {
                    // applies intellisense hooks onto a cell
                    function applyIntellisense(cell) {
                        if (cell.cell_type !== 'code') { return; }

                        var editor = cell.code_mirror;
                        if (editor.intellisense == null) {
                            var intellisense = new CodeMirrorIntellisense(editor);
                            editor.setOption('theme', 'neat');
                            editor.intellisense = intellisense;

                            editor.on('changes', function (cm, changes) {
                                changedSinceTypecheck = true;
                                changedRecently = true;
                            });

                            //intellisense.addMethodsTrigger({ keyCode: 57, shiftKey: true }); // `(`
                            //intellisense.addMethodsTrigger({ keyCode: 48, shiftKey: true });// `)`
                         //    intellisense.addMethodsTrigger({ keyCode: 8 }); // `backspace`
                          //   intellisense.addMethodsTrigger({ keyCode: 190 });
                            intellisense.addDeclarationTrigger({ keyCode: 190,}); // `.`
                            intellisense.addDeclarationTrigger({ keyCode: 32, ctrlKey: true, preventDefault: true, type: 'down' }); // `ctrl+space`
                            // intellisense.addDeclarationTrigger({ keyCode: 191 }); // `/`
                            // intellisense.addDeclarationTrigger({ keyCode: 220 }); // `\`
                            // intellisense.addDeclarationTrigger({ keyCode: 222 }); // `"`
                            // intellisense.addDeclarationTrigger({ keyCode: 222, shiftKey: true }); // `"`
                            intellisense.onMethod(intellisenseRequestMethod); 
                            intellisense.onDeclaration(intellisenseRequestDeclaration);
                        }
                    }

                    // applies intellisense hooks onto all cells
                    IPython.notebook.get_cells().forEach(function (cell) {
                        applyIntellisense(cell);
                    });

                    // applies intellisense hooks onto cells that are selected
                    $([IPython.events]).on('create.Cell', function (event, data) {
                        applyIntellisense(data.cell);
                    });

                    $([IPython.events]).on('delete.Cell', function (event, data) {
                        data.cell.code_mirror.intellisense.setDeclarations([])
                    });

                    $([IPython.events]).on('kernel_idle.Kernel', function (event, data) {
                        kernelIdledSinceTypeCheck = true;
                        kernelIdledRecently = true;
                    });



                    window.setInterval(function () {
                        if (!changedSinceTypecheck && !kernelIdledSinceTypeCheck)
                            return;

                        if (changedRecently || kernelIdledRecently) {
                            changedRecently = false;
                            kernelIdledRecently = false;
                            return;
                        }

                        changedSinceTypecheck = false;
                        changedRecently = false;
                        kernelIdledSinceTypeCheck = false;
                        kernelIdledRecently = false;
                        intellisenseRequestDeclaration({ keyCode: 0 }) //FIX THIS LATER
                    }, 1000);

                });
            });
        });
    }

    return { onload: onload }
})
    