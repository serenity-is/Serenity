using jQueryApi;
using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;

namespace Serenity
{
    // TODO: https://github.com/mleibman/SlickGrid/wiki/Handling-selection selection ile ilgili konuları import et
    // TODO: https://github.com/mleibman/SlickGrid/wiki/Providing-data-to-the-grid item meta data ilgili konular, 
    // row: cssClasses, focusable, selectable vs.
    // col: focusable, selectable, formatter, editor, colSpan
    // When looking up a property, the grid checks in the following order: 1Row-level item metadata. 2Column-level item metadata by column id. 3Column-level item metadata by column index.
    // 4 Column definition. 5 Grid options. 6 Grid defaults.

    [Imported, IncludeGenericArguments(false)]
    public class SlickEvent
    {
        public void Subscribe(Action<jQueryEvent, dynamic> handler)
        {
        }

        public void Subscribe(Func<jQueryEvent, dynamic, dynamic> handler)
        {
        }

        public void Unsubscribe(Action<jQueryEvent, dynamic> handler)
        {
        }
    }

    [Imported, Serializable]
    public class SlickColumnSort
    {
        public string ColumnId { get; set; }
        public bool SortAsc { get; set; }
    }

    [Imported, Serializable]
    public class SlickRowCell
    {
        public int Row { get; set; }
        public int Cell { get; set; }
    }

    [Imported, Serializable]
    public class SlickPositionInfo
    {
        public int Bottom { get; set; }
        public int Height { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public bool Visible { get; set; }
        public int Width { get; set; }
    }

    [Imported, Serializable]
    public class SlickRangeInfo
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int LeftPx { get; set; }
        public int RightPx { get; set; }
    }


    /// <summary>
    /// Options for the Slick columns
    /// </summary>
    [Imported, ScriptNamespace("Slick"), ScriptName("AutoTooltips")]
    public class SlickAutoTooltips
    {
        public SlickAutoTooltips(SlickAutoTooltipsOptions options)
        {
        }
    }

    [Imported, Serializable]
    public class SlickAutoTooltipsOptions
    {
        

        public bool EnableForHeaderCells { get; set; }
        public bool EnableForCells { get; set; }
        public int MaxToolTipLength { get; set; }
    }

    /// <summary>
    /// Options for the Slick columns
    /// </summary>
    [Imported, ScriptNamespace("Slick"), ScriptName("Grid")]
    public class SlickGrid
    {
        /// <summary>
        /// Creates a new Slick.Grid
        /// </summary>
        /// <param name="container">Container node to create the grid in. This can be a DOM Element, a jQuery node, or a jQuery selector.</param>
        /// <param name="data">Databinding source. This can either be a regular JavaScript array or a custom object exposing getItem(index) and getLength() functions.</param>
        /// <param name="columns">An array of column definition objects. See Column Options for a list of options that can be included on each column definition object.</param>
        /// <param name="options">Additional options. See Grid Options for a list of options that can be included.</param>
        public SlickGrid(jQueryObject container, List<dynamic> data,
            List<SlickColumn> columns, SlickGridOptions options)
        {
        }

        /// <summary>
        /// Initializes the grid. Called after plugins are registered. Normally, this is called by the constructor, so you don't need to call it. 
        /// However, in certain cases you may need to delay the initialization until some other process has finished. In that case, set 
        /// the explicitInitialization option to true and call the grid.init() manually.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Destroys the grid.
        /// </summary>
        public void Destroy()
        {
        }

        /// <summary>
        /// Returns an array of every data object, unless you're using DataView in which case it returns a DataView object.
        /// </summary>
        public List<dynamic> GetData()
        {
            return null;
        }

        /// <summary>
        /// Returns the databinding item at a given position.
        /// </summary>
        /// <param name="index">index</param>
        public dynamic GetDataItem(int index)
        {
            return null;
        }

        /// <summary>
        /// Sets a new source for databinding and removes all rendered rows. Note that this doesn't render the new rows - you can follow it with a call to render() to do that.
        /// </summary>
        /// <param name="data">New databinding source. This can either be a regular JavaScript array or a custom object exposing getItem(index) and getLength() functions.</param>
        /// <param name="scrollToTop">If true, the grid will reset the vertical scroll position to the top of the grid.</param>
        public void SetData(List<dynamic> data, bool scrollToTop)
        {
        }

        /// <summary>
        /// Returns the size of the databinding source.
        /// </summary>
        public int GetDataLength()
        {
            return 0;
        }

        /// <summary>
        /// Returns an object containing all of the Grid options set on the grid. See a list of Grid Options here.
        /// </summary>
        public SlickGridOptions GetOptions()
        {
            return null;
        }

        /// <summary>
        /// Returns an array of row indices corresponding to the currently selected rows.
        /// </summary>
        /// <returns></returns>
        public int[] GetSelectedRows()
        {
            return null;
        }

        /// <summary>
        /// Returns the current SelectionModel. See here for more information about SelectionModels.
        /// </summary>
        public dynamic GetSelectionModel()
        {
            return null;
        }

        /// <summary>
        /// Proportionately resizes all columns to fill available horizontal space. This does not take the cell contents into consideration.
        /// </summary>
        public void AutoSizeColumns()
        {
        }

        /// <summary>
        /// Returns the index of a column with a given id. Since columns can be reordered by the user, this can be used to get the column definition independent of the order:
        /// </summary>
        /// <param name="id">column id</param>
        public int GetColumnIndex(string id)
        {
            return 0;
        }

        /// <summary>
        /// Returns an array of column definitions, containing the option settings for each individual column.
        /// </summary>
        public List<SlickColumn> GetColumns()
        {
            return null;
        }

        /// <summary>
        /// Sets grid columns. Column headers will be recreated and all rendered rows will be removed. To rerender the grid (if necessary), call render().
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumns(List<SlickColumn> columns)
        {
        }

        /// <summary>
        /// Accepts a columnId string and an ascending boolean. Applies a sort glyph in either ascending or descending form to the header of the column. 
        /// Note that this does not actually sort the column. It only adds the sort glyph to the header.
        /// </summary>
        /// <param name="columnId">column id</param>
        /// <param name="ascending">true: ascending, false: descending</param>
        public void SetSortColumn(string columnId, bool ascending)
        {
        }

        /// <summary>
        /// Accepts an array of objects in the form [ { columnId: [string], sortAsc: [boolean] }, ... ]. When called, this will apply a sort glyph in either ascending or descending form to the header of each column specified in the array. 
        /// Note that this does not actually sort the column. It only adds the sort glyph to the header
        /// </summary>
        public void SetSortColumns(List<SlickColumnSort> cols)
        {
        }

        /// <summary>
        /// Updates an existing column definition and a corresponding header DOM element with the new title and tooltip.
        /// </summary>
        /// <param name="columnId">column id</param>
        /// <param name="title">title</param>
        /// <param name="toolTip">toolTip</param>
        public void UpdateColumnHeader(string columnId, string title, string toolTip)
        {
        }

        /// <summary>
        /// Adds an "overlay" of CSS classes to cell DOM elements. SlickGrid can have many such overlays associated with different keys and they are frequently used by plugins. 
        /// For example, SlickGrid uses this method internally to decorate selected cells with selectedCellCssClass (see options).
        /// </summary>
        /// <param name="key">A unique key you can use in calls to setCellCssStyles and removeCellCssStyles. If a hash with that key has already been set, an exception will be thrown.</param>
        /// <param name="hash">A hash of additional cell CSS classes keyed by row number and then by column id. Multiple CSS classes can be specified and separated by space.
        /// example: { 0: { "number_column": "cell-bold", "title_column": "cell-title cell-highlighted" }, 4: { "percent_column": "cell-highlighted" }}</param>
        public void AddCellCssStyles(string key, dynamic hash)
        {
        }

        /// <summary>
        /// Returns true if you can click on a given cell and make it the active focus.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="col">A column index.</param>
        /// <returns></returns>
        public bool CanCellBeActive(int row, int col)
        {
            return false;
        }

        /// <summary>
        /// Returns true if selecting the row causes this particular cell to have the selectedCellCssClass applied to it. A cell can be selected if it exists and if 
        /// it isn't on an empty / "Add New" row and if it is not marked as "unselectable" in the column definition.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="col">A column index.</param>
        /// <returns></returns>
        public bool CanCellBeSelected(int row, int col)
        {
            return false;
        }

        /// <summary>
        /// Attempts to switch the active cell into edit mode. Will throw an error if the cell is set to be not editable. Uses the specified editor, otherwise defaults to any default editor for that given cell.
        /// </summary>
        /// <param name="editor">A SlickGrid editor (see examples in slick.editors.js).</param>
        public void EditActiveCell(Type editor)
        {
        }

        /// <summary>
        /// Flashes the cell twice by toggling the CSS class 4 times.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell"> A column index.</param>
        /// <param name="speed">(optional) - The milliseconds delay between the toggling calls. Defaults to 100 ms.</param>
        public void FlashCell(int row, int cell, int speed)
        {
        }

        /// <summary>
        /// Returns an object representing the coordinates of the currently active cell:
        /// </summary>
        public SlickRowCell GetActiveCell()
        {
            return null;
        }

        /// <summary>
        /// Returns the DOM element containing the currently active cell. If no cell is active, null is returned.
        /// </summary>
        public System.Html.Element GetActiveCellNode()
        {
            return null;
        }

        /// <summary>
        /// Returns an object representing information about the active cell's position. All coordinates are absolute and take into consideration the visibility and scrolling position of all ancestors. 
        /// </summary>
        /// <returns></returns>
        public SlickPositionInfo GetActiveCellPosition()
        {
            return null;
        }

        /// <summary>
        /// Accepts a key name, returns the group of CSS styles defined under that name. See setCellCssStyles for more info.
        /// </summary>
        /// <param name="key">A string</param>
        public dynamic GetCellCssStyles(string key)
        {
            return null;
        }

        /// <summary>
        /// Returns the active cell editor. If there is no actively edited cell, null is returned.
        /// </summary>
        public object GetCellEditor()
        {
            return null;
        }

        /// <summary>
        /// Returns a hash containing row and cell indexes from a standard W3C/jQuery event.
        /// </summary>
        /// <param name="e">A standard W3C/jQuery event.</param>
        public SlickRowCell GetCellFromEvent(jQueryEvent e)
        {
            return null;
        }

        /// <summary>
        /// Returns a hash containing row and cell indexes. Coordinates are relative to the top left corner of the grid beginning with the first row (not including the column headers).
        /// </summary>
        /// <param name="x">An x coordinate.</param>
        /// <param name="y">A y coordinate.</param>
        /// <returns></returns>
        public SlickRowCell GetCellFromPoint(int x, int y)
        {
            return null;
        }

        /// <summary>
        /// Returns a DOM element containing a cell at a given row and cell.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell">A column index.</param>
        /// <returns></returns>
        public System.Html.Element GetCellNode(int row, int cell)
        {
            return null;
        }

        /// <summary>
        /// Returns an object representing information about a cell's position. All coordinates are absolute and take into consideration the visibility and scrolling position of all ancestors. 
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell">A column index.</param>
        public SlickPositionInfo GetCellNodeBox(int row, int cell)
        {
            return null;
        }

        /// <summary>
        /// Accepts a row integer and a cell integer, scrolling the view to the row where row is its row index, and cell is its cell index. Optionally accepts a forceEdit boolean which, if true, will attempt to initiate the edit dialogue for the field in the specified cell.
        /// Unlike setActiveCell, this scrolls the row into the viewport and sets the keyboard focus.
        /// </summary>
        /// <param name="row">A row index</param>
        /// <param name="cell">A column index</param>
        /// <param name="forceEdit">force edit</param>
        public void GoToCell(int row, int cell, bool forceEdit)
        {
        }

        /// <summary>
        /// Switches the active cell one row down skipping unselectable cells. Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigateDown()
        {
        }

        /// <summary>
        /// Switches the active cell one cell left skipping unselectable cells. Unline navigatePrev, navigateLeft stops at the first cell of the row. Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigateLeft()
        {
        }

        /// <summary>
        /// Tabs over active cell to the next selectable cell. Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigateNext()
        {
        }

        /// <summary>
        /// Tabs over active cell to the previous selectable cell. Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigatePrev()
        {
        }

        /// <summary>
        /// Switches the active cell one cell right skipping unselectable cells. Unline navigateNext, navigateRight stops at the last cell of the row. 
        /// Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigateRight()
        {
        }

        /// <summary>
        /// Switches the active cell one row up skipping unselectable cells. Returns a boolean saying whether it was able to complete or not.
        /// </summary>
        public void NavigateUp()
        {
        }

        /// <summary>
        /// Removes an "overlay" of CSS classes from cell DOM elements. See setCellCssStyles for more.
        /// </summary>
        public void RemoveCellCssStyles(string key)
        {
        }

        /// <summary>
        /// Resets active cell.
        /// </summary>
        public void ResetActiveCell()
        {
        }

        /// <summary>
        /// Registers a plugin
        /// </summary>
        /// <param name="plugin">An instance of plugin object to register.</param>
        public void RegisterPlugin(object plugin)
        {
        }

        /// <summary>
        /// Sets an active cell.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell">A column index.</param>
        public void SetActiveCell(int row, int cell)
        {
        }

        /// <summary>
        /// Sets CSS classes to specific grid cells by calling removeCellCssStyles(key) followed by addCellCssStyles(key, hash). key is name for this set of styles so you can reference it later - to modify it or remove it, for example. hash is a per-row-index, per-column-name nested hash of CSS classes to apply.
        /// Suppose you have a grid with columns:
        /// ["login", "name", "birthday", "age", "likes_icecream", "favorite_cake"]
        /// ...and you'd like to highlight the "birthday" and "age" columns for people whose birthday is today, in this case, rows at index 0 and 9. (The first and tenth row in the grid).
        ///  .highlight{ background: yellow } 
        ///  
        /// grid.setCellCssStyles("birthday_highlight", {
        ///     0: {
        ///         birthday: "highlight", 
        ///         age: "highlight" 
        ///     },
        ///     9: {
        ///         birthday: "highlight",
        ///         age: "highlight"
        ///     }
        /// })
        /// </summary>
        /// <param name="key">A string key. Will overwrite any data already associated with this key.</param>
        /// <param name="hash">A hash of additional cell CSS classes keyed by row number and then by column id. Multiple CSS classes can be specified and separated by space.</param>
        public void SetCellCssStyles(string key, dynamic hash)
        {
        }

        /// <summary>
        /// Returns the DIV element matching class grid-canvas, which contains every data row currently being rendered in the DOM.
        /// </summary>
        /// <returns></returns>
        public System.Html.Element GetCanvasNode()
        {
            return null;
        }

        /// <summary>
        /// Returns an object representing information about the grid's position on the page. 
        /// </summary>
        public SlickPositionInfo GetGridPosition()
        {
            return null;
        }

        /// <summary>
        /// If passed no arguments, returns an object that tells you the range of rows (by row number) currently being rendered, as well as 
        /// the left/right range of pixels currently rendered. { top: [rowIndex], bottom: [rowIndex], leftPx: [numPixels], rightPx: [numPixels] }
        /// The options viewportTop and viewportLeft are optional, and tell what what would be rendered at a certain scroll top/left offset. 
        /// For example, grid.getRenderedRange(1000) would essentially be asking: "if I were to scroll 1000 pixels down, what rows would be rendered?"
        /// </summary>
        /// <param name="viewportTop">(optional) - The number of pixels offset from the top of the grid.</param>
        /// <param name="viewportLeft">(optional) - The number of pixels offset from the left of the grid.</param>
        /// <returns></returns>
        public SlickRangeInfo GetRenderedRange(int viewportTop, int viewportLeft)
        {
            return null;
        }

        /// <summary>
        /// Returns an object telling you which rows are currently being displayed on the screen, and also the pixel offsets for left/right scrolling. 
        /// { top: [rowIndex], bottom: [rowIndex], leftPx: [numPixels], rightPx: [numPixels] }
        /// Also accepts viewportTop and viewportLeft offsets to tell you what would be shown to the user if you were to scroll to that point.
        /// </summary>
        /// <param name="viewportTop">(optional) - The number of pixels offset from the top of the grid.</param>
        /// <param name="viewportLeft">(optional) - The number of pixels offset from the left of the grid.</param>
        /// <returns></returns>
        public SlickRangeInfo GetViewport(int viewportTop, int viewportLeft)
        {
            return null;
        }

        [AlternateSignature]
        public SlickRangeInfo GetViewport()
        {
            return null;
        }

        /// <summary>
        /// Redraws the grid. Invalidates all rows and calls render().
        /// </summary>
        public void Invalidate()
        {
        }

        /// <summary>
        /// Tells the grid that all rows in the table are invalid. (If render() is called after this, it will redraw the entire grid.)
        /// </summary>
        public void InvalidateAllRows()
        {
        }

        /// <summary>
        /// Tells the grid that the row specified by row is invalid. (If render() is called after this, it will redraw the contents of that row.)
        /// </summary>
        /// <param name="row">A row index.</param>
        public void InvalidateRow(int row)
        {
        }

        /// <summary>
        /// Accepts an array of row indices, and tells the grid that those rows are invalid. (If render() is called after this, it will redraw the contents of those rows.)
        /// </summary>
        /// <param name="rows">An array of row indices.</param>
        public void InvalidateRows(int[] rows)
        {
        }

        /// <summary>
        /// Rerenders rows in the DOM.
        /// </summary>
        public void Render()
        {
        }

        /// <summary>
        /// Resizes the canvas to fit the current DIV container. (For example, to resize the grid, you would first change the size of the div, then call resizeCanvas().)
        /// </summary>
        public void ResizeCanvas()
        {
        }

        /// <summary>
        /// Scrolls the indicated cell into view.
        /// Note that this does nothing unless the indicated column is already not in view. For example, 
        /// if the grid is scrolled to the far left and you were looking at row 0, calling scrollCellIntoView(100,0) 
        /// would not simply scroll you to row 100. But if column 8 were out of view and you called scrollCellIntoView(100,8), 
        /// then it would scroll down and to the right.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell">A column index.</param>
        public void ScrollCellIntoView(int row, int cell)
        {
        }

        /// <summary>
        /// Scrolls the view to the indicated row, placing the row at the top of the view.
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="doPaging">A boolean. If false, the grid will scroll so the indicated row is at the top of the view. 
        /// If true, the grid will scroll so the indicated row is at the bottom of the view. Defaults to false.</param>
        public void ScrollRowIntoView(int row, bool doPaging)
        {
        }

        /// <summary>
        /// Scrolls the view to the indicated row, placing the row at the top of the view.
        /// </summary>
        /// <param name="row">A row index.</param>
        public void ScrollRowToTop(int row)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="row">A row index.</param>
        /// <param name="cell">A cell index.</param>
        public void UpdateCell(int row, int cell)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="row">A row index.</param>
        public void UpdateRow(int row)
        {
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void UpdateRowCount()
        {
        }

        /// <summary>
        /// Returns the element of a DIV row beneath the actual column headers. For an example of how you might use this, 
        /// see the header row quick filter example, which grabs the element, appends inputs, and delegates events to the inputs.
        /// </summary>
        public Element GetHeaderRow()
        {
            return null;
        }

        /// <summary>
        /// If a header row is implemented and has one child for each column, as seen in the header row quick filter example, 
        /// you may use this function to pass a columnId and get the individual cell from that header row. Returns a DIV element.
        /// </summary>
        /// <param name="columnId">The id string of a column.</param>
        public Element GetHeaderRowColumn(string columnId)
        {
            return null;
        }

        /// <summary>
        /// Returns an array of objects representing columns that have a sort glyph in the header:
        /// </summary>
        public SlickColumnSort[] GetSortColumns()
        {
            return null;
        }

        /// <summary>
        /// Returns the DIV element of the top panel. The panel is hidden by default, but you can show it by 
        /// initializing the grid with showTopPanel set to true, or by calling grid.setTopPanelVisibility(true).
        /// </summary>
        public Element GetTopPanel()
        {
            return null;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="visible">visible</param>
        public void SetHeaderRowVisibility(bool visible)
        {
        }

        [IntrinsicProperty]
        public SlickEvent OnScroll { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnSort { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnHeaderContextMenu { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnHeaderClick { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnMouseEnter { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnMouseLeave { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnClick { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnDblClick { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnContextMenu { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnKeyDown { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnAddNewRow { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnValidationError { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnViewportChanged { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnColumnsReordered { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnColumnsResized { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnCellChange { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnBeforeEditCell { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnBeforeCellEditorDestroy { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnHeaderCellRendered { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnBeforeHeaderCellDestroy { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnBeforeDestroy { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnActiveCellChanged { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnActiveCellPositionChanged { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnDragInit { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnDragStart { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnDrag { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnDragEnd { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnSelectedRowsChanged { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnCellCssStylesChanged { get; private set; }
    }

    [Imported, ScriptNamespace("Slick"), ScriptName("RowMoveManager")]
    public class SlickRowMoveManager
    {
        public SlickRowMoveManager(SlickRowMoveManagerOptions options)
        {
        }

        [IntrinsicProperty]
        public SlickEvent OnBeforeMoveRows { get; private set; }
        [IntrinsicProperty]
        public SlickEvent OnMoveRows { get; private set; }
    }

    [Imported, Serializable]
    public class SlickRowMoveManagerOptions
    {
        public Boolean CancelEditOnDrag { get; set; }
    }


}