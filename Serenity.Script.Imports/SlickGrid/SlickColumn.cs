using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public delegate void SlickAsyncPostRender(object cellNode, int row, dynamic item, SlickColumn column);
    public delegate string SlickColumnFormatter(int row, int cell, object value, SlickColumn column, dynamic item);

    [Imported, Serializable]
    public class SlickFormatterContext
    {
        public int Row;
        public int Cell;
        public object Value;
        public SlickColumn Column;
        public dynamic Item;
    }

    public delegate string SlickFormatter(SlickFormatterContext ctx);

    /// <summary>
    /// Options for the Slick columns
    /// </summary>
    [Imported, Serializable]
    public class SlickColumn
    {
        /// <summary>
        /// This accepts a function of the form function(cellNode, row, dataContext, colDef) and is used to post-process the cell’s DOM node / nodes (default: null)
        /// </summary>
        public SlickAsyncPostRender AsyncPostRender;

        /// <summary>
        /// Used by the the slick.rowMoveManager.js plugin for moving rows. Has no effect without the plugin installed. (default: null)
        /// </summary>
        public object Behaviour { get; set; }

        /// <summary>
        /// In the “Add New” row, determines whether clicking cells in this column can trigger row addition. If true, clicking on the cell in this column in the “Add New” row will not trigger row addition. (default: null)
        /// </summary>
        public bool CannotTriggerInsert { get; set; }

        /// <summary>
        /// Accepts a string as a class name, applies that class to every row cell in the column. (default: "")
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// When set to true, the first user click on the header will do a descending sort. When set to false, the first user click on the header will do an ascending sort. (default: true)
        /// </summary>
        public bool DefaultSortAsc { get; set; }

        /// <summary>
        /// The editor for cell edits {TextEditor, IntegerEditor, DateEditor…} See slick.editors.js (default: null)
        /// </summary>
        public Type Editor { get; set; }

        /// <summary>
        /// The property name in the data object to pull content from. (This is assumed to be on the root of the data object.) (default: "")
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// When set to false, clicking on a cell in this column will not select the row for that cell. The cells in this column will also be skipped during tab navigation. (default: true)
        /// </summary>
        public bool Focusable { get; set; }

        /// <summary>
        /// This accepts a function of the form function(row, cell, value, columnDef, dataContext) and returns a formatted version of the data in each cell of this column. For example, setting formatter to function(r, c, v, cd, dc) { return “Hello!”; } would overwrite every value in the column with “Hello!” See defaultFormatter in slick.grid.js for an example formatter. (default: null)
        /// </summary>
        public SlickColumnFormatter Formatter { get; set; }

        /// <summary>
        /// My hack to allow formatters with single argument and intellisen
        /// </summary>
        public SlickFormatter Format { get; set; }

        /// <summary>
        /// Accepts a string as a class name, applies that class to the cell for the column header. (default: null)
        /// </summary>
        public string HeaderCssClass { get; set; }

        /// <summary>
        /// A unique identifier for the column within the grid. (default: "")
        /// </summary>
        [ScriptName("id")]
        public string Identifier { get; set; }

        /// <summary>
        /// Set the maximum allowable width of this column, in pixels. (default: null)
        /// </summary>
        public int MaxWidth { get; set; }

        /// <summary>
        /// Set the minimum allowable width of this column, in pixels. (default: 30)
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// The text to display on the column heading. (default: "")
        /// </summary>
        [ScriptName("name")]
        public string Title { get; set; }

        /// <summary>
        /// If set to true, whenever this column is resized, the entire table view will rerender. (default: false)
        /// </summary>
        public bool RerenderOnResize { get; set; }

        /// <summary>
        /// If false, column can no longer be resized. (default: true)
        /// </summary>
        public bool Resizable { get; set; }

        /// <summary>
        /// If false, when a row is selected, the CSS class for selected cells (“selected” by default) is not applied to the cell in this column. (default: true)
        /// </summary>
        public bool Selectable { get; set; }

        /// <summary>
        /// If true, the column will be sortable by clicking on the header. (default: false)
        /// </summary>
        public bool Sortable { get; set; }

        /// <summary>
        /// If set to a non-empty string, a tooltip will appear on hover containing the string. (default: "")
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Width of the column in pixels. (May often be overridden by things like minWidth, maxWidth, forceFitColumns, etc.) (default: null)
        /// </summary>
        public int Width { get; set; }
    }
}