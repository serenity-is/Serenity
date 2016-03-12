using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// Options for the SlickGrid constructor
    /// </summary>
    [Imported, Serializable]
    public class SlickGridOptions
    {
        /// <summary>
        /// Makes cell editors load asynchronously after a small delay. This greatly increases keyboard navigation speed (default:false).
        /// </summary>
        public bool AsyncEditorLoading { get; set; }

        /// <summary>
        /// Delay after which cell editor is loaded. Ignored unless asyncEditorLoading is true (default: 100)
        /// </summary>
        public int AsyncEditorLoadDelay { get; set; }

        /// <summary>
        /// (default: 50)
        /// </summary>
        public int AsyncPostRenderDelay { get; set; }

        /// <summary>
        /// Cell will not automatically go into edit mode when selected (default: true)
        /// </summary>
        public bool AutoEdit { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool AutoHeight { get; set; }

        /// <summary>
        /// A CSS class to apply to flashing cells via flashCell() (default: "flashing").
        /// </summary>
        public string CellFlashingCssClass { get; set; }

        /// <summary>
        /// A CSS class to apply to cells highlighted via setHighlightedCells() (default: "selected").
        /// </summary>
        public string CellHighlightCssClass { get; set; }

        /// <summary>
        /// DataItemColumnValueExtractor
        /// </summary>
        public Action DataItemColumnValueExtractor { get; set; }

        /// <summary>
        /// (default: 80)
        /// </summary>
        public int DefaultColumnWidth { get; set; }

        /// <summary>
        /// (default: defaultFormatter)
        /// </summary>
        public Action DefaultFormatter { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// Not listed as a default under options in slick.grid.js (default: queueAndExecuteCommand)
        /// </summary>
        public Action EditCommandHandler { get; set; }

        /// <summary>
        /// A factory object responsible to creating an editor for a given cell. Must implement getEditor(column). (default: null)
        /// </summary>
        public Action EditorFactory { get; set; }

        /// <summary>
        /// A Slick.EditorLock instance to use for controlling concurrent data edits (default:  Slick.GlobalEditorLock)
        /// </summary>
        public object EditorLock { get; set; }

        /// <summary>
        /// If true, a blank row will be displayed at the bottom - typing values in that row will add a new one. Must subscribe to onAddNewRow to save values. (default: false)
        /// </summary>
        public bool EnableAddRow { get; set; }

        /// <summary>
        /// If true, async post rendering will occur and asyncPostRender delegates on columns will be called (default: false)
        /// </summary>
        public bool EnableAsyncPostRender { get; set; }

        /// <summary>
        /// *WARNING*: Not contained in SlickGrid 2.1, may be deprecated (default: null)
        /// </summary>
        public bool EnableCellRangeSelection { get; set; }

        /// <summary>
        /// Appears to enable cell virtualisation for optimised speed with large datasets (default: true)
        /// </summary>
        public bool EnableCellNavigation { get; set; }

        /// <summary>
        /// (default: true)
        /// </summary>
        public bool EnableColumnReorder { get; set; }

        /// <summary>
        /// *WARNING*: Not contained in SlickGrid 2.1, may be deprecated (default: null)
        /// </summary>
        public bool EnableRowReordering { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool EnableTextSelectionOnCells { get; set; }

        /// <summary>
        /// See: Example: Explicit Initialization (default: false)
        /// </summary>
        public bool ExplicitInitialization { get; set; }

        /// <summary>
        /// Force column sizes to fit into the container (preventing horizontal scrolling). Effectively sets column width to be 1/Number of Columns which on small containers may not be desirable (default: false)
        /// </summary>
        public bool ForceFitColumns { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool ForceSyncScrolling { get; set; }

        /// <summary>
        /// A factory object responsible to creating a formatter for a given cell. Must implement getFormatter(column). (default: null)
        /// </summary>
        public Action FormatterFactory { get; set; }

        /// <summary>
        /// Will expand the table row divs to the full width of the container, table cell divs will remain aligned to the left (default: false)
        /// </summary>
        public bool FullWidthRows { get; set; }

        /// <summary>
        /// Frozen columns (experimental). Default -1.
        /// </summary>
        public int FrozenColumn { get; set; }

        /// <summary>
        /// Frozen rows (experimental). Default -1.
        /// </summary>
        public int FrozenRow { get; set; }

        /// <summary>
        /// Frozen rows at bottom (experimental)
        /// </summary>
        public bool FrozenBottom { get; set; }

        /// <summary>
        /// (default: 25)
        /// </summary>
        public int HeaderRowHeight { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool LeaveSpaceForNewRows { get; set; }

        /// <summary>
        /// See: Example: Multi-Column Sort, (default: false).
        /// </summary>
        public bool MultiColumnSort { get; set; }

        /// <summary>
        /// (default: true)
        /// </summary>
        public bool MultiSelect { get; set; }

        /// <summary>
        /// (default: 25)
        /// </summary>
        public int RowHeight { get; set; }

        /// <summary>
        /// (default: "selected")
        /// </summary>
        public string SelectedCellCssClass { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool ShowHeaderRow { get; set; }

        /// <summary>
        /// (default: false)
        /// </summary>
        public bool ShowFooterRow { get; set; }

        /// <summary>
        /// If true, the column being resized will change its width as the mouse is dragging the resize handle. If false, the column will resize after mouse drag ends. (default: false)
        /// </summary>
        public bool SyncColumnCellResize { get; set; }

        /// <summary>
        /// (default: 25)
        /// </summary>
        public int TopPanelHeight { get; set; }
    }
}