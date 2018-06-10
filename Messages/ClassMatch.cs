using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace iCSharp.Messages
{
    public class ClassMatch
    {
		[JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("documentation")]
        public string Documentation { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
        
        [JsonProperty("classmethods")]
		public List<MethodMatch> ClassMethods { get; set; }

		[JsonProperty("classaccess")]
		public string ClassAccess { get; set; }

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