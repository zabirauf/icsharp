using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class CompleteRequest
    {
         [JsonProperty("code_cells")]
         public List<string> CodeCells { get; set; }

         [JsonProperty("cursor_pos")]
         public int CursorPosition { get; set; }

        [JsonProperty("line")]
        public string Line { get; set; }

        [JsonProperty("cursor_line")]
         public int CursorLine { get; set; }

        [JsonProperty("code_pos")]
        public int CodePosition { get; set; }

        [JsonProperty("selected_cell_index")]
         public int Selected_Cell_Index { get; set; }

        /*
                code: JSON.stringify(cells.codes),
                code_cells: cells.string_cells,
                cursor_pos: cursor.ch,
                line: getLine(cells.codes[cells.selectedIndex], cursor.line),
                cursor_line: cursor.line,
                selected_cell: cells.selectedCell,
                selected_cell_index: cells.selectedIndex
         */

    }
}
