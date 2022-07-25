var Slick = Slick || {};
Slick._ = (() => {
  var __defProp = Object.defineProperty;
  var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __hasOwnProp = Object.prototype.hasOwnProperty;
  var __export = (target, all) => {
    for (var name in all)
      __defProp(target, name, { get: all[name], enumerable: true });
  };
  var __copyProps = (to, from, except, desc) => {
    if (from && typeof from === "object" || typeof from === "function") {
      for (let key of __getOwnPropNames(from))
        if (!__hasOwnProp.call(to, key) && key !== except)
          __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
    }
    return to;
  };
  var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

  // SlickGrid/slick.grid.ts
  var slick_grid_exports = {};
  __export(slick_grid_exports, {
    Grid: () => Grid
  });

  // SlickGrid/base.ts
  var NonDataRow = class {
    constructor() {
      this.__nonDataRow = true;
    }
  };
  var preClickClassName = "slick-edit-preclick";
  typeof window !== "undefined" && window.Slick && (window.Slick.Map = Map);

  // SlickGrid/editing.ts
  var EditorLock = class {
    isActive(editController) {
      return editController ? this.activeEditController === editController : this.activeEditController != null;
    }
    activate(editController) {
      if (editController === this.activeEditController) {
        return;
      }
      if (this.activeEditController != null) {
        throw "SleekGrid.EditorLock.activate: an editController is still active, can't activate another editController";
      }
      if (!editController.commitCurrentEdit) {
        throw "SleekGrid.EditorLock.activate: editController must implement .commitCurrentEdit()";
      }
      if (!editController.cancelCurrentEdit) {
        throw "SleekGrid.EditorLock.activate: editController must implement .cancelCurrentEdit()";
      }
      this.activeEditController = editController;
    }
    deactivate(editController) {
      if (this.activeEditController !== editController) {
        throw "SleekGrid.EditorLock.deactivate: specified editController is not the currently active one";
      }
      this.activeEditController = null;
    }
    commitCurrentEdit() {
      return this.activeEditController ? this.activeEditController.commitCurrentEdit() : true;
    }
    cancelCurrentEdit() {
      return this.activeEditController ? this.activeEditController.cancelCurrentEdit() : true;
    }
  };
  var GlobalEditorLock = new EditorLock();

  // SlickGrid/event.ts
  var EventData = class {
    constructor() {
      this._isPropagationStopped = false;
      this._isImmediatePropagationStopped = false;
    }
    stopPropagation() {
      this._isPropagationStopped = true;
    }
    isPropagationStopped() {
      return this._isPropagationStopped;
    }
    stopImmediatePropagation() {
      this._isImmediatePropagationStopped = true;
    }
    isImmediatePropagationStopped() {
      return this._isImmediatePropagationStopped;
    }
  };
  var Event = class {
    constructor() {
      this._handlers = [];
    }
    subscribe(fn) {
      this._handlers.push(fn);
    }
    unsubscribe(fn) {
      for (var i = this._handlers.length - 1; i >= 0; i--) {
        if (this._handlers[i] === fn) {
          this._handlers.splice(i, 1);
        }
      }
    }
    notify(args, e, scope) {
      e = e || new EventData();
      scope = scope || this;
      var returnValue;
      for (var i = 0; i < this._handlers.length && !(e.isPropagationStopped() || e.isImmediatePropagationStopped()); i++) {
        returnValue = this._handlers[i].call(scope, e, args);
      }
      return returnValue;
    }
    clear() {
      this._handlers = [];
    }
  };
  var keyCode = {
    BACKSPACE: 8,
    DELETE: 46,
    DOWN: 40,
    END: 35,
    ENTER: 13,
    ESCAPE: 27,
    HOME: 36,
    INSERT: 45,
    LEFT: 37,
    PAGEDOWN: 34,
    PAGEUP: 33,
    RIGHT: 39,
    TAB: 9,
    UP: 38
  };

  // SlickGrid/internal.ts
  var maxSupportedCssHeight;
  var scrollbarDimensions;
  function adjustFrozenColumnCompat(columns, options) {
    if ((options == null ? void 0 : options.frozenColumn) == null) {
      delete options.frozenColumn;
      return;
    }
    var toFreeze = options.frozenColumn + 1;
    options.frozenColumn = -1;
    var i = 0;
    while (i < columns.length) {
      var col = columns[i++];
      if (toFreeze > 0 && col.visible !== false) {
        col.frozen = true;
        options.frozenColumn++;
        toFreeze--;
      } else if (col.frozen !== void 0)
        delete col.frozen;
    }
  }
  function attrEncode(s) {
    if (s == null)
      return "";
    return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
  }
  function defaultFormatter(row, cell, value) {
    return htmlEncode(value);
  }
  function disableSelection($target) {
    if ($target && $target.jquery) {
      $target.attr("unselectable", "on").css("MozUserSelect", "none").on("selectstart.ui", function() {
        return false;
      });
    }
  }
  function getMaxSupportedCssHeight() {
    return maxSupportedCssHeight != null ? maxSupportedCssHeight : navigator.userAgent.toLowerCase().match(/gecko\//) ? 4e6 : 32e6;
  }
  function getScrollBarDimensions(recalc) {
    if (!scrollbarDimensions || recalc) {
      var $c = $("<div style='position:absolute; top:-10000px; left:-10000px; width:100px; height:100px; overflow:scroll;'></div>").appendTo("body");
      scrollbarDimensions = {
        width: Math.round($c.width() - $c[0].clientWidth),
        height: Math.round($c.height() - $c[0].clientHeight)
      };
      $c.remove();
    }
    return scrollbarDimensions;
  }
  function H(tag, attr, ...children) {
    var el = document.createElement(tag);
    var k, v, c;
    if (attr) {
      for (k in attr) {
        v = attr[k];
        if (v != null && v !== false)
          el.setAttribute(k, v === true ? "" : v);
      }
    }
    if (children) {
      for (c of children)
        el.appendChild(c);
    }
    return el;
  }
  function htmlEncode(s) {
    return (s + "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  }
  function simpleArrayEquals(arr1, arr2) {
    if (!Array.isArray(arr1) || !Array.isArray(arr2) || arr1.length !== arr2.length)
      return false;
    arr1.sort();
    arr2.sort();
    for (var i = 0; i < arr1.length; i++) {
      if (arr1[i] !== arr2[i])
        return false;
    }
    return true;
  }
  function sortToDesiredOrderAndKeepRest(columns, idOrder) {
    if (idOrder.length == 0)
      return columns;
    var orderById = {}, colIdxById = {}, result = [];
    for (var i = 0; i < idOrder.length; i++)
      orderById[idOrder[i]] = i;
    for (i = 0; i < columns.length; i++)
      colIdxById[columns[i].id] = i;
    function takeFrom(i2) {
      for (var j = i2; j < columns.length; j++) {
        var c2 = columns[j];
        if (i2 != j && orderById[c2.id] != null)
          break;
        result.push(c2);
        colIdxById[c2.id] = null;
      }
    }
    if (orderById[columns[0].id] == null)
      takeFrom(0);
    for (var id of idOrder) {
      i = colIdxById[id];
      if (i != null)
        takeFrom(i);
    }
    for (i = 0; i < columns.length; i++) {
      var c = columns[i];
      if (colIdxById[c.id] != null) {
        result.push(c);
        colIdxById[c.id] = null;
      }
    }
    return result;
  }
  function addUiStateHover() {
    $(this).addClass("ui-state-hover");
  }
  function removeUiStateHover() {
    $(this).removeClass("ui-state-hover");
  }

  // SlickGrid/range.ts
  var Range = class {
    constructor(fromRow, fromCell, toRow, toCell) {
      if (toRow === void 0 && toCell === void 0) {
        toRow = fromRow;
        toCell = fromCell;
      }
      this.fromRow = Math.min(fromRow, toRow);
      this.fromCell = Math.min(fromCell, toCell);
      this.toRow = Math.max(fromRow, toRow);
      this.toCell = Math.max(fromCell, toCell);
    }
    isSingleRow() {
      return this.fromRow == this.toRow;
    }
    isSingleCell() {
      return this.fromRow == this.toRow && this.fromCell == this.toCell;
    }
    contains(row, cell) {
      return row >= this.fromRow && row <= this.toRow && cell >= this.fromCell && cell <= this.toCell;
    }
    toString() {
      if (this.isSingleCell()) {
        return "(" + this.fromRow + ":" + this.fromCell + ")";
      } else {
        return "(" + this.fromRow + ":" + this.fromCell + " - " + this.toRow + ":" + this.toCell + ")";
      }
    }
  };

  // SlickGrid/grid.ts
  var Grid = class {
    constructor(container, data, columns, options) {
      this._activeCellNode = null;
      this._actualFrozenRow = -1;
      this._cellCssClasses = {};
      this._cellHeightDiff = 0;
      this._cellWidthDiff = 0;
      this._colLeft = [];
      this._colRight = [];
      this._currentEditor = null;
      this._footerRowH = 0;
      this._frozenRowsHeight = 0;
      this._groupingPanelH = 0;
      this._hasFrozenRows = false;
      this._headerColumnWidthDiff = 0;
      this._headerRowH = 0;
      this._hEditorLoader = null;
      this._hPostRender = null;
      this._hPostRenderCleanup = null;
      this._hRender = null;
      this._ignoreScrollUntil = 0;
      this._initialized = false;
      this._jQueryNewWidthBehaviour = false;
      this._numVisibleRows = 0;
      this._page = 0;
      this._pageOffset = 0;
      this._pagingActive = false;
      this._pagingIsLastPage = false;
      this._paneBottomH = 0;
      this._paneTopH = 0;
      this._plugins = [];
      this._postProcessCleanupQueue = [];
      this._postProcessedRows = {};
      this._postProcessFromRow = null;
      this._postProcessGroupId = 0;
      this._postProcessToRow = null;
      this._rowsCache = {};
      this._rtl = false;
      this._rtlE = "right";
      this._rtlS = "left";
      this._scrollLeft = 0;
      this._scrollLeftPrev = 0;
      this._scrolLLeftRendered = 0;
      this._scrollTop = 0;
      this._scrollTopPrev = 0;
      this._scrollTopRendered = 0;
      this._selectedRows = [];
      this._sortColumns = [];
      this._tabbingDirection = 1;
      this._topPanelH = 0;
      this._uid = "slickgrid_" + Math.round(1e6 * Math.random());
      this._viewportTopH = 0;
      this._vScrollDir = 1;
      this._boundAncestorScroll = [];
      this.onActiveCellChanged = new Event();
      this.onActiveCellPositionChanged = new Event();
      this.onAddNewRow = new Event();
      this.onBeforeCellEditorDestroy = new Event();
      this.onBeforeDestroy = new Event();
      this.onBeforeEditCell = new Event();
      this.onBeforeFooterRowCellDestroy = new Event();
      this.onBeforeHeaderCellDestroy = new Event();
      this.onBeforeHeaderRowCellDestroy = new Event();
      this.onCellChange = new Event();
      this.onCellCssStylesChanged = new Event();
      this.onClick = new Event();
      this.onColumnsReordered = new Event();
      this.onColumnsResized = new Event();
      this.onContextMenu = new Event();
      this.onDblClick = new Event();
      this.onDrag = new Event();
      this.onDragEnd = new Event();
      this.onDragInit = new Event();
      this.onDragStart = new Event();
      this.onFooterRowCellRendered = new Event();
      this.onHeaderCellRendered = new Event();
      this.onHeaderClick = new Event();
      this.onHeaderContextMenu = new Event();
      this.onHeaderMouseEnter = new Event();
      this.onHeaderMouseLeave = new Event();
      this.onHeaderRowCellRendered = new Event();
      this.onKeyDown = new Event();
      this.onMouseEnter = new Event();
      this.onMouseLeave = new Event();
      this.onScroll = new Event();
      this.onSelectedRowsChanged = new Event();
      this.onSort = new Event();
      this.onValidationError = new Event();
      this.onViewportChanged = new Event();
      this.handleSelectedRangesChanged = (e, ranges) => {
        var previousSelectedRows = this._selectedRows.slice(0);
        this._selectedRows = [];
        var hash = {}, cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
          for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
            if (!hash[j]) {
              this._selectedRows.push(j);
              hash[j] = {};
            }
            for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
              if (this.canCellBeSelected(j, k)) {
                hash[j][cols[k].id] = this._options.selectedCellCssClass;
              }
            }
          }
        }
        this.setCellCssStyles(this._options.selectedCellCssClass, hash);
        if (!simpleArrayEquals(previousSelectedRows, this._selectedRows)) {
          var caller = e && e.detail && e.detail.caller || "click";
          var newSelectedAdditions = this._selectedRows.filter((i2) => previousSelectedRows.indexOf(i2) < 0);
          var newSelectedDeletions = previousSelectedRows.filter((i2) => this._selectedRows.indexOf(i2) < 0);
          this.trigger(this.onSelectedRowsChanged, {
            rows: this.getSelectedRows(),
            previousSelectedRows,
            caller,
            changedSelectedRows: newSelectedAdditions,
            changedUnselectedRows: newSelectedDeletions
          }, e);
        }
        this._selectedRows = [];
        var hash = {}, cols = this._cols;
        for (var i = 0; i < ranges.length; i++) {
          for (var j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
            if (!hash[j]) {
              this._selectedRows.push(j);
              hash[j] = {};
            }
            for (var k = ranges[i].fromCell; k <= ranges[i].toCell; k++) {
              if (this.canCellBeSelected(j, k)) {
                hash[j][cols[k].id] = this._options.selectedCellCssClass;
              }
            }
          }
        }
      };
      this.viewOnRowCountChanged = () => {
        this.updateRowCount();
        this.render();
      };
      this.viewOnRowsChanged = (_, args) => {
        this.invalidateRows(args.rows);
        this.render();
        this.updateFooterTotals();
      };
      this.viewOnDataChanged = () => {
        this.invalidate();
        this.render();
      };
      this.handleHeaderRowScroll = () => {
        if (this._ignoreScrollUntil >= new Date().getTime())
          return;
        var scrollLeft = this.hasFrozenColumns ? this._headerRowColsR.parentElement.scrollLeft : this._headerRowColsL.parentElement.scrollLeft;
        if (scrollLeft != this._scrollContainerX.scrollLeft) {
          this._scrollContainerX.scrollLeft = scrollLeft;
        }
      };
      this.handleFooterRowScroll = () => {
        if (this._ignoreScrollUntil >= new Date().getTime())
          return;
        var scrollLeft = this.hasFrozenColumns ? this._footerRowR.parentElement.scrollLeft : this._footerRowColsL.parentElement.scrollLeft;
        if (scrollLeft != this._scrollContainerX.scrollLeft) {
          this._scrollContainerX.scrollLeft = scrollLeft;
        }
      };
      this.handleActiveCellPositionChange = () => {
        if (!this._activeCellNode) {
          return;
        }
        this.trigger(this.onActiveCellPositionChanged, {});
        if (this._currentEditor) {
          var cellBox = this.getActiveCellPosition();
          if (this._currentEditor.show && this._currentEditor.hide) {
            if (!cellBox.visible) {
              this._currentEditor.hide();
            } else {
              this._currentEditor.show();
            }
          }
          if (this._currentEditor.position) {
            this._currentEditor.position(cellBox);
          }
        }
      };
      if (typeof jQuery === "undefined") {
        throw "SlickGrid requires jquery module to be loaded";
      }
      if (!jQuery.fn.drag) {
        throw "SlickGrid requires jquery.event.drag module to be loaded";
      }
      this._data = data;
      var defaults = {
        addNewRowCssClass: "new-row",
        alwaysAllowHorizontalScroll: false,
        alwaysShowVerticalScroll: false,
        asyncEditorLoadDelay: 100,
        asyncEditorLoading: false,
        asyncPostCleanupDelay: 40,
        asyncPostRenderDelay: 50,
        autoEdit: true,
        autoHeight: false,
        cellFlashingCssClass: "flashing",
        dataItemColumnValueExtractor: null,
        defaultColumnWidth: 80,
        defaultFormatter,
        editable: false,
        editorFactory: null,
        editorLock: GlobalEditorLock,
        enableAddRow: false,
        enableAsyncPostRender: false,
        enableAsyncPostRenderCleanup: false,
        enableCellNavigation: true,
        enableColumnReorder: true,
        enableTabKeyNavigation: true,
        enableTextSelectionOnCells: false,
        explicitInitialization: false,
        footerRowHeight: 25,
        forceFitColumns: false,
        forceSyncScrolling: false,
        formatterFactory: null,
        frozenBottom: false,
        frozenRow: -1,
        fullWidthRows: false,
        groupingPanel: false,
        groupingPanelHeight: 34,
        headerRowHeight: 25,
        leaveSpaceForNewRows: false,
        useLegacyUI: true,
        minBuffer: 3,
        multiColumnSort: false,
        multiSelect: true,
        renderAllCells: false,
        rowHeight: 25,
        selectedCellCssClass: "selected",
        showCellSelection: true,
        showColumnHeader: true,
        showFooterRow: false,
        showGroupingPanel: true,
        showHeaderRow: false,
        showTopPanel: false,
        suppressActiveCellChangeOnEdit: false,
        topPanelHeight: 25
      };
      this._colDefaults = {
        name: "",
        resizable: true,
        sortable: false,
        minWidth: 30,
        rerenderOnResize: false,
        headerCssClass: null,
        footerCssClass: null,
        defaultSortAsc: true,
        focusable: true,
        selectable: true
      };
      if (container instanceof jQuery)
        this._container = container[0];
      else if (container instanceof Element)
        this._container = container;
      else if (typeof container === "string")
        this._container = document.querySelector(container);
      if (this._container == null) {
        throw new Error("SlickGrid requires a valid container, " + container + " does not exist in the DOM.");
      }
      this._rtl = $(document.body).hasClass("rtl") || $(this._container).css("direction") == "rtl";
      if (this._rtl) {
        this._rtlS = "right";
        this._rtlE = "left";
      }
      options = $.extend({}, defaults, options);
      this._options = options;
      this.validateAndEnforceOptions();
      this._colDefaults.width = options.defaultColumnWidth;
      adjustFrozenColumnCompat(columns, this._options);
      this.setInitialCols(columns);
      if (options.enableColumnReorder && !$.fn.sortable) {
        throw new Error("SlickGrid's 'enableColumnReorder = true' option requires jquery-ui.sortable module to be loaded");
      }
      this._editController = {
        "commitCurrentEdit": this.commitCurrentEdit.bind(this),
        "cancelCurrentEdit": this.cancelCurrentEdit.bind(this)
      };
      $(this._container).empty().css("overflow", "hidden").css("outline", 0).addClass(this._uid);
      if (this._options.useLegacyUI)
        this._container.classList.add("ui-widget");
      if (!/relative|absolute|fixed/.test($(this._container).css("position"))) {
        this._container.style.position = "relative";
      }
      this._container.appendChild(this._focusSink1 = H("div", {
        class: "slick-focus-sink",
        hideFocus: "",
        style: "position:fixed;width:0!important;height:0!important;top:0;left:0;outline:0!important;",
        tabIndex: "0"
      }));
      if (options.groupingPanel) {
        this._container.appendChild(this._groupingPanel = H("div", {
          class: "slick-grouping-panel",
          style: "overflow:hidden; position:relative;" + (!options.showGroupingPanel ? " display: none" : "")
        }));
      }
      const uisd = this._options.useLegacyUI ? " ui-state-default" : "";
      var spacerW = this.getCanvasWidth() + (this._scrollDims = getScrollBarDimensions()).width + "px";
      this._paneHeaderL = H("div", { class: "slick-pane slick-pane-header slick-pane-left", tabIndex: "0" }, H("div", { class: "slick-header slick-header-left" + uisd, style: !options.showColumnHeader && "display: none" }, this._headerColsL = H("div", { class: "slick-header-columns slick-header-columns-left", style: this._rtlS + ":-1000px" })));
      this._paneHeaderR = H("div", { class: "slick-pane slick-pane-header slick-pane-right", tabIndex: "0" }, H("div", { class: "slick-header slick-header-right" + uisd, style: !options.showColumnHeader && "display: none" }, this._headerColsR = H("div", { class: "slick-header-columns slick-header-columns-right", style: this._rtlS + ":-1000px" })));
      var headerRowL = H("div", { class: "slick-headerrow" + uisd, style: !options.showHeaderRow && "display: none" }, this._headerRowColsL = H("div", { class: "slick-headerrow-columns slick-headerrow-columns-left" }), this._headerRowSpacerL = H("div", { style: "display:block;height:1px;position:absolute;top:0;left:0;", width: spacerW }));
      var topPanelLS = H("div", { class: "slick-top-panel-scroller" + uisd, style: !options.showTopPanel && "display: none" }, this._topPanelL = H("div", { class: "slick-top-panel", style: "width: 10000px" }));
      this._viewportTopL = H("div", { class: "slick-viewport slick-viewport-top slick-viewport-left", tabIndex: "0", hideFocus: "" }, this._canvasTopL = H("div", { class: "grid-canvas grid-canvas-top grid-canvas-left", tabIndex: "0", hideFocus: "" }));
      var footerRowL = H("div", { class: "slick-footerrow" + uisd, style: !options.showFooterRow && "display: none" }, this._footerRowColsL = H("div", { class: "slick-footerrow-columns slick-footerrow-columns-left" }), this._footerRowSpacerL = H("div", { style: "display:block;height:1px;position:absolute;top:0;left:0;", width: spacerW }));
      this._paneTopL = H("div", { class: "slick-pane slick-pane-top slick-pane-left", tabIndex: "0" }, headerRowL, topPanelLS, this._viewportTopL, footerRowL);
      var headerRowR = H("div", { class: "slick-headerrow" + uisd, style: !options.showHeaderRow && "display: none" }, this._headerRowColsR = H("div", { class: "slick-headerrow-columns slick-headerrow-columns-right" }), this._headerRowSpacerR = H("div", { style: "display:block;height:1px;position:absolute;top:0;left:0;", width: spacerW }));
      var topPanelRS = H("div", { class: "slick-top-panel-scroller" + uisd, style: !options.showTopPanel && "display: none" }, this._topPanelR = H("div", { class: "slick-top-panel", style: "width: 10000px" }));
      this._viewportTopR = H("div", { class: "slick-viewport slick-viewport-top slick-viewport-right", tabIndex: "0", hideFocus: "" }, this._canvasTopR = H("div", { class: "grid-canvas grid-canvas-top grid-canvas-right", tabIndex: "0", hideFocus: "" }));
      var footerRowR = H("div", { class: "slick-footer-row" + uisd, style: !options.showFooterRow && "display: none" }, this._footerRowR = H("div", { class: "slick-footerrow-columns slick-footerrow-columns-right" }), this._footerRowSpacerR = H("div", { style: "display:block;height:1px;position:absolute;top:0;left:0;", width: spacerW }));
      this._paneTopR = H("div", { class: "slick-pane slick-pane-top slick-pane-right", tabIndex: "0" }, headerRowR, topPanelRS, this._viewportTopR, footerRowR);
      this._paneBottomL = H("div", { class: "slick-pane slick-pane-bottom slick-pane-left", tabIndex: "0" }, this._viewportBottomL = H("div", { class: "slick-viewport slick-viewport-bottom slick-viewport-left", tabIndex: "0", hideFocus: "" }, this._canvasBottomL = H("div", { class: "grid-canvas grid-canvas-bottom grid-canvas-left", tabIndex: "0", hideFocus: "" })));
      this._paneBottomR = H("div", { class: "slick-pane slick-pane-bottom slick-pane-right", tabIndex: "0" }, this._viewportBottomR = H("div", { class: "slick-viewport slick-viewport-bottom slick-viewport-right", tabIndex: "0", hideFocus: "" }), this._canvasBottomR = H("div", { class: "grid-canvas grid-canvas-bottom grid-canvas-right", tabIndex: "0", hideFocus: "" }));
      this._container.append(this._paneHeaderL, this._paneHeaderR, this._paneTopL, this._paneTopR, this._paneBottomL, this._focusSink2 = this._focusSink1.cloneNode());
      if (options.viewportClass)
        this.getViewports().toggleClass(options.viewportClass, true);
      if (!options.explicitInitialization) {
        this.init();
      }
      this.bindToData();
    }
    init() {
      if (this._initialized)
        return;
      this._initialized = true;
      this.getViewportWidth();
      this.getViewportHeight();
      this.measureCellPaddingAndBorder();
      disableSelection($([this._headerColsL, this._headerColsR]));
      var viewports = this.getViewports();
      if (!this._options.enableTextSelectionOnCells) {
        viewports.on("selectstart.ui", function() {
          return $(this).is("input,textarea");
        });
      }
      this.adjustFrozenRowOption();
      this.setPaneVisibility();
      this.setScroller();
      this.setOverflow();
      this.updateViewColLeftRight();
      this.createColumnHeaders();
      this.createColumnFooters();
      this.setupColumnSort();
      this.createCssRules();
      this.resizeCanvas();
      this.bindAncestorScrollEvents();
      $(this._container).on("resize.slickgrid", this.resizeCanvas.bind(this));
      viewports.on("scroll", this.handleScroll.bind(this));
      if (jQuery.fn.mousewheel && (this.hasFrozenColumns() || this._hasFrozenRows)) {
        viewports.on("mousewheel", this.handleMouseWheel.bind(this));
      }
      $([this._headerColsL.parentElement, this._headerColsR.parentElement]).on("contextmenu", this.handleHeaderContextMenu.bind(this)).on("click", this.handleHeaderClick.bind(this)).delegate(".slick-header-column", "mouseenter", this.handleHeaderMouseEnter.bind(this)).delegate(".slick-header-column", "mouseleave", this.handleHeaderMouseLeave.bind(this));
      this._headerRowColsL.parentElement.addEventListener("scroll", this.handleHeaderRowScroll);
      this._headerRowColsR.parentElement.addEventListener("scroll", this.handleHeaderRowScroll);
      this._footerRowColsL.parentElement.addEventListener("scroll", this.handleFooterRowScroll);
      this._footerRowR.parentElement.addEventListener("scroll", this.handleFooterRowScroll);
      $([this._focusSink1, this._focusSink2]).on("keydown", this.handleKeyDown.bind(this));
      var canvases = this.getCanvases();
      canvases.on("keydown", this.handleKeyDown.bind(this)).on("click", this.handleClick.bind(this)).on("dblclick", this.handleDblClick.bind(this)).on("contextmenu", this.handleContextMenu.bind(this)).on("draginit", this.handleDragInit.bind(this)).on("dragstart", { distance: 3 }, this.handleDragStart.bind(this)).on("drag", this.handleDrag.bind(this)).on("dragend", this.handleDragEnd.bind(this)).delegate(".slick-cell", "mouseenter", this.handleMouseEnter.bind(this)).delegate(".slick-cell", "mouseleave", this.handleMouseLeave.bind(this));
      if (navigator.userAgent.toLowerCase().match(/webkit/) && navigator.userAgent.toLowerCase().match(/macintosh/)) {
        canvases.on("mousewheel", this.handleMouseWheel.bind(this));
      }
    }
    hasFrozenColumns() {
      return this._frozenCols > 0;
    }
    registerPlugin(plugin) {
      this._plugins.unshift(plugin);
      plugin.init(this);
    }
    unregisterPlugin(plugin) {
      for (var i = this._plugins.length; i >= 0; i--) {
        if (this._plugins[i] === plugin) {
          if (this._plugins[i].destroy) {
            this._plugins[i].destroy();
          }
          this._plugins.splice(i, 1);
          break;
        }
      }
    }
    getPluginByName(name) {
      for (var i = this._plugins.length - 1; i >= 0; i--) {
        if (this._plugins[i].pluginName === name)
          return this._plugins[i];
      }
    }
    setSelectionModel(model) {
      if (this._selectionModel) {
        this._selectionModel.onSelectedRangesChanged.unsubscribe(this.handleSelectedRangesChanged);
        if (this._selectionModel.destroy) {
          this._selectionModel.destroy();
        }
      }
      this._selectionModel = model;
      if (this._selectionModel) {
        this._selectionModel.init(this);
        this._selectionModel.onSelectedRangesChanged.subscribe(this.handleSelectedRangesChanged);
      }
    }
    getScrollBarDimensions() {
      return this._scrollDims;
    }
    getDisplayedScrollbarDimensions() {
      return {
        width: this._viewportHasVScroll ? this._scrollDims.width : 0,
        height: this._viewportHasHScroll ? this._scrollDims.height : 0
      };
    }
    getAbsoluteColumnMinWidth() {
      return this._absoluteColMinWidth;
    }
    getSelectionModel() {
      return this._selectionModel;
    }
    getCanvasNode() {
      return this._canvasTopL;
    }
    getCanvases() {
      return $([this._canvasTopL, this._canvasTopR, this._canvasBottomL, this._canvasBottomR]);
    }
    getActiveCanvasNode(e) {
      this.setActiveCanvasNode(e);
      return this._activeCanvasNode;
    }
    setActiveCanvasNode(e) {
      if (e) {
        this._activeCanvasNode = $(e.target).closest(".grid-canvas")[0];
      }
    }
    getViewportNode() {
      return this._viewportTopL;
    }
    getViewports() {
      return $([this._viewportTopL, this._viewportTopR, this._viewportBottomL, this._viewportBottomR]);
    }
    getActiveViewportNode(e) {
      this.setActiveViewportNode(e);
      return this._activeViewportNode;
    }
    setActiveViewportNode(e) {
      if (e) {
        this._activeViewportNode = $(e.target).closest(".slick-viewport")[0];
      }
    }
    calcHeaderWidths() {
      this._headersWidthL = this._headersWidthR = 0;
      var scrollWidth = this._scrollDims.width;
      var cols = this._cols, frozenCols = this._frozenCols;
      for (var i = 0, ii = cols.length; i < ii; i++) {
        var width = cols[i].width;
        if (frozenCols > 0 && i >= frozenCols) {
          this._headersWidthR += width;
        } else {
          this._headersWidthL += width;
        }
      }
      if (frozenCols > 0) {
        this._headersWidthL = this._headersWidthL + 1e3;
        this._headersWidthR = Math.max(this._headersWidthR, this._viewportW) + this._headersWidthL;
        this._headersWidthR += scrollWidth;
      } else {
        this._headersWidthL += scrollWidth;
        this._headersWidthL = Math.max(this._headersWidthL, this._viewportW) + 1e3;
      }
    }
    getCanvasWidth() {
      var availableWidth = this._viewportHasVScroll ? this._viewportW - this._scrollDims.width : this._viewportW;
      var cols = this._cols, i = cols.length, frozenCols = this._frozenCols;
      this._canvasWidthL = this._canvasWidthR = 0;
      while (i--) {
        if (frozenCols > 0 && i >= frozenCols) {
          this._canvasWidthR += cols[i].width;
        } else {
          this._canvasWidthL += cols[i].width;
        }
      }
      var totalRowWidth = this._canvasWidthL + this._canvasWidthR;
      return this._options.fullWidthRows ? Math.max(totalRowWidth, availableWidth) : totalRowWidth;
    }
    updateCanvasWidth(forceColumnWidthsUpdate) {
      var oldCanvasWidth = this._canvasWidth;
      var oldCanvasWidthL = this._canvasWidthL;
      var oldCanvasWidthR = this._canvasWidthR;
      var widthChanged;
      this._canvasWidth = this.getCanvasWidth();
      var scrollWidth = this._scrollDims.width;
      widthChanged = this._canvasWidth !== oldCanvasWidth || this._canvasWidthL !== oldCanvasWidthL || this._canvasWidthR !== oldCanvasWidthR;
      if (widthChanged || this.hasFrozenColumns() || this._hasFrozenRows) {
        var canvasWidthL = this._canvasWidthL + "px";
        var canvasWidthR = this._canvasWidthR + "px";
        this._canvasTopL.style.width = canvasWidthL;
        this.calcHeaderWidths();
        this._headerColsL.style.width = this._headersWidthL + "px";
        this._headerColsR.style.width = this._headersWidthR + "px";
        if (this._groupingPanel) {
          this._groupingPanel.style.width = this._canvasWidth + "px";
        }
        if (this.hasFrozenColumns()) {
          var viewportMinus = this._viewportW - this._canvasWidthL + "px";
          this._canvasTopR.style.width = canvasWidthR;
          this._paneHeaderL.style.width = canvasWidthL;
          this._paneHeaderR.style[this._rtlS] = canvasWidthL;
          this._paneHeaderR.style.width = viewportMinus;
          this._paneTopL.style.width = canvasWidthL;
          this._paneTopR.style[this._rtlS] = canvasWidthL;
          this._paneTopR.style.width = viewportMinus;
          this._headerRowColsL.style.width = canvasWidthL;
          this._headerRowColsL.parentElement.style.width = canvasWidthL;
          this._headerRowColsR.style.width = canvasWidthR;
          this._headerRowColsR.parentElement.style.width = viewportMinus;
          this._footerRowColsL.style.width = canvasWidthL;
          this._footerRowColsL.parentElement.style.width = canvasWidthL;
          this._footerRowR.style.width = canvasWidthR;
          this._footerRowR.parentElement.style.width = viewportMinus;
          this._viewportTopL.style.width = canvasWidthL;
          this._viewportTopR.style.width = viewportMinus;
          if (this._hasFrozenRows) {
            this._paneBottomL.style.width = canvasWidthL;
            this._paneBottomR.style[this._rtlS] = canvasWidthL;
            this._viewportBottomL.style.width = canvasWidthL;
            this._viewportBottomR.style.width = viewportMinus;
            this._canvasBottomL.style.width = canvasWidthL;
            this._canvasBottomR.style.width = canvasWidthR;
          }
        } else {
          this._paneHeaderL.style.width = "100%";
          this._paneTopL.style.width = "100%";
          this._headerRowColsL.parentElement.style.width = "100%";
          this._headerRowColsL.style.width = this._canvasWidth + "px";
          this._footerRowColsL.parentElement.style.width = "100%";
          this._footerRowColsL.style.width = this._canvasWidth + "px";
          this._viewportTopL.style.width = "100%";
          if (this._hasFrozenRows) {
            this._viewportBottomL.style.width = "100%";
            this._canvasBottomL.style.width = canvasWidthL;
          }
        }
        this._viewportHasHScroll = this._canvasWidth > this._viewportW - scrollWidth;
      }
      var w = this._canvasWidth + (this._viewportHasVScroll ? scrollWidth : 0) + "px";
      this._headerRowSpacerL.style.width = w;
      this._headerRowSpacerR.style.width = w;
      this._footerRowSpacerL.style.width = w;
      this._footerRowSpacerR.style.width = w;
      if (widthChanged || forceColumnWidthsUpdate) {
        this.applyColumnWidths();
      }
    }
    bindAncestorScrollEvents() {
      var elem = this._hasFrozenRows && !this._options.frozenBottom ? this._canvasBottomL : this._canvasTopL;
      while ((elem = elem.parentNode) != document.body && elem != null) {
        if (elem == this._viewportTopL || elem.scrollWidth != elem.clientWidth || elem.scrollHeight != elem.clientHeight) {
          elem.addEventListener("scroll", this.handleActiveCellPositionChange);
          this._boundAncestorScroll.push(elem);
        }
      }
    }
    unbindAncestorScrollEvents() {
      if (this._boundAncestorScroll) {
        for (var x of this._boundAncestorScroll)
          x.removeEventListener("scroll", this.handleActiveCellPositionChange);
      }
      this._boundAncestorScroll = [];
    }
    updateColumnHeader(columnId, title, toolTip) {
      if (!this._initialized) {
        return;
      }
      var idx = this.getColumnIndex(columnId);
      if (idx == null) {
        return;
      }
      var columnDef = this._cols[idx];
      var $header = $([this._headerColsL, this._headerColsR]).children().eq(idx);
      if (!$header)
        return;
      if (title !== void 0) {
        columnDef.name = title;
      }
      if (toolTip !== void 0) {
        columnDef.toolTip = toolTip;
      }
      this.trigger(this.onBeforeHeaderCellDestroy, {
        node: $header[0],
        column: columnDef
      });
      $header.attr("title", toolTip || "").children().eq(0).html(title);
      this.trigger(this.onHeaderCellRendered, {
        node: $header[0],
        column: columnDef
      });
    }
    getHeader() {
      return this._headerColsL;
    }
    getHeaderColumn(columnIdOrIdx) {
      var idx = typeof columnIdOrIdx === "number" ? columnIdOrIdx : this.getColumnIndex(columnIdOrIdx);
      if (idx == null)
        return null;
      var $rtn = $([this._headerColsL, this._headerColsR]).children().eq(idx);
      return $rtn && $rtn[0];
    }
    getGroupingPanel() {
      return this._groupingPanel;
    }
    getHeaderRow() {
      return this._headerRowColsL;
    }
    getHeaderRowColumn(columnId) {
      var idx = this.getColumnIndex(columnId);
      if (idx == null)
        return;
      var headerRowTarget, frozenCols = this._frozenCols;
      if (frozenCols <= 0 || idx < frozenCols) {
        headerRowTarget = this._headerRowColsL;
      } else {
        headerRowTarget = this._headerRowColsR;
        idx -= frozenCols;
      }
      return headerRowTarget.childNodes.item(idx);
    }
    getFooterRow() {
      return this._footerRowColsL;
    }
    getFooterRowColumn(columnId) {
      var idx = this.getColumnIndex(columnId);
      if (idx == null)
        return null;
      var footerRowTarget, frozenCols = this._frozenCols;
      if (frozenCols <= 0 || idx < frozenCols) {
        footerRowTarget = this._footerRowColsL;
      } else {
        footerRowTarget = this._footerRowR;
        idx -= frozenCols;
      }
      return footerRowTarget.childNodes.item(idx);
    }
    createColumnFooters() {
      var self = this;
      $([this._footerRowColsL, this._footerRowR]).find(".slick-footerrow-column").each(function() {
        var columnDef = $(self).data("column");
        if (columnDef) {
          self.trigger(self.onBeforeFooterRowCellDestroy, {
            node: this,
            column: columnDef
          });
        }
      });
      $(this._footerRowColsL).empty();
      $(this._footerRowR).empty();
      var cols = this._cols, frozenCols = this._frozenCols;
      for (var i = 0; i < cols.length; i++) {
        var m = cols[i];
        var footerRowCell = $("<div class='" + (this._options.useLegacyUI ? "ui-state-default " : "") + "slick-footerrow-column l" + i + " r" + i + "'></div>").data("column", m).addClass(m.footerCssClass || m.cssClass || "").addClass(i < frozenCols ? "frozen" : "").appendTo(frozenCols > 0 && i >= frozenCols ? this._footerRowR : this._footerRowColsL);
        this.trigger(this.onFooterRowCellRendered, {
          node: footerRowCell[0],
          column: m
        });
      }
    }
    formatGroupTotal(total, columnDef) {
      if (columnDef.formatter != null) {
        var item = new NonDataRow();
        item[columnDef.field] = total;
        try {
          return columnDef.formatter(-1, -1, total, columnDef, item);
        } catch (e) {
        }
      }
      if (typeof total == "number" && typeof Q !== "undefined" && Q.formatNumber) {
        if (columnDef.sourceItem && columnDef.sourceItem.displayFormat) {
          return Q.formatNumber(total, columnDef.sourceItem.displayFormat);
        } else
          return Q.formatNumber(total, "#,##0.##");
      } else
        return htmlEncode(total == null ? void 0 : total.toString());
    }
    groupTotalText(totals, columnDef, key) {
      var ltKey = key.substring(0, 1).toUpperCase() + key.substring(1);
      var text = typeof Q !== "undefined" && Q.tryGetText && Q.tryGetText(ltKey) || ltKey;
      var total = totals[key][columnDef.field];
      total = this.formatGroupTotal(total, columnDef);
      return "<span class='aggregate agg-" + key + "'  title='" + text + "'>" + total + "</span>";
    }
    groupTotalsFormatter(totals, columnDef) {
      if (!totals || !columnDef)
        return "";
      var text = null;
      var self = this;
      ["sum", "avg", "min", "max", "cnt"].forEach(function(key) {
        if (text == null && totals[key] && totals[key][columnDef.field] != null) {
          text = self.groupTotalText(totals, columnDef, key);
          return false;
        }
      });
      return text || "";
    }
    createColumnHeaders() {
      var self = this;
      $([this._headerColsL, this._headerColsR]).find(".slick-header-column").each(function() {
        var columnDef = $(self).data("column");
        if (columnDef) {
          self.trigger(self.onBeforeHeaderCellDestroy, {
            node: this,
            column: columnDef
          });
        }
      });
      $(this._headerColsL).empty();
      $(this._headerColsR).empty();
      this.calcHeaderWidths();
      this._headerColsL.style.width = this._headersWidthL + "px";
      this._headerColsR.style.width = this._headersWidthR + "px";
      $([this._headerRowColsL, this._headerRowColsR]).find(".slick-headerrow-column").each(function() {
        var columnDef = $(this).data("column");
        if (columnDef) {
          self.trigger(self.onBeforeHeaderRowCellDestroy, {
            node: this,
            column: columnDef
          });
        }
      });
      $(this._headerRowColsL).empty();
      $(this._headerRowColsR).empty();
      var cols = this._cols, frozenCols = this._frozenCols;
      for (var i = 0; i < cols.length; i++) {
        var m = cols[i];
        var $headerTarget = frozenCols > 0 && i >= frozenCols ? this._headerColsR : this._headerColsL;
        var header = $("<div class='" + (this._options.useLegacyUI ? "ui-state-default " : "") + "slick-header-column' />").html("<span class='slick-column-name'>" + m.name + "</span>").width(m.width - this._headerColumnWidthDiff).attr("id", "" + this._uid + m.id).attr("title", m.toolTip || "").data("column", m).addClass(m.headerCssClass || "").addClass(i < frozenCols ? "frozen" : "").appendTo($headerTarget);
        if ((this._options.enableColumnReorder || m.sortable) && this._options.useLegacyUI) {
          header.on("mouseenter", addUiStateHover).on("mouseleave", removeUiStateHover);
        }
        if (m.sortable) {
          header.addClass("slick-header-sortable");
          header.append("<span class='slick-sort-indicator' />");
        }
        this.trigger(this.onHeaderCellRendered, {
          node: header[0],
          column: m
        });
        if (this._options.showHeaderRow) {
          var headerRowTarget = frozenCols > 0 && i >= frozenCols ? this._headerRowColsR : this._headerRowColsL;
          var headerRowCell = $("<div class='" + (this._options.useLegacyUI ? "ui-state-default " : "") + "slick-headerrow-column l" + i + " r" + i + "'></div>").data("column", m).appendTo(headerRowTarget);
          this.trigger(this.onHeaderRowCellRendered, {
            node: headerRowCell[0],
            column: m
          });
        }
      }
      this.setSortColumns(this._sortColumns);
      this.setupColumnResize();
      if (this._options.enableColumnReorder)
        this.setupColumnReorder();
    }
    setupColumnSort() {
      $([this._headerColsL, this._headerColsR]).click((e) => {
        e.metaKey = e.metaKey || e.ctrlKey;
        if ($(e.target).hasClass("slick-resizable-handle")) {
          return;
        }
        var $col = $(e.target).closest(".slick-header-column");
        if (!$col.length) {
          return;
        }
        var column = $col.data("column");
        if (column.sortable) {
          if (!this.getEditorLock().commitCurrentEdit()) {
            return;
          }
          var sortOpts = null;
          var i = 0;
          for (; i < this._sortColumns.length; i++) {
            if (this._sortColumns[i].columnId == column.id) {
              sortOpts = this._sortColumns[i];
              sortOpts.sortAsc = !sortOpts.sortAsc;
              break;
            }
          }
          if (e.metaKey && this._options.multiColumnSort) {
            if (sortOpts) {
              this._sortColumns.splice(i, 1);
            }
          } else {
            if (!e.shiftKey && !e.metaKey || !this._options.multiColumnSort) {
              this._sortColumns = [];
            }
            if (!sortOpts) {
              sortOpts = { columnId: column.id, sortAsc: column.defaultSortAsc };
              this._sortColumns.push(sortOpts);
            } else if (this._sortColumns.length == 0) {
              this._sortColumns.push(sortOpts);
            }
          }
          this.setSortColumns(this._sortColumns);
          if (!this._options.multiColumnSort) {
            this.trigger(this.onSort, {
              multiColumnSort: false,
              sortCol: column,
              sortAsc: sortOpts.sortAsc
            }, e);
          } else {
            var cols = this._initCols;
            this.trigger(this.onSort, {
              multiColumnSort: true,
              sortCols: this._sortColumns.map((col) => ({ sortCol: cols[this.getInitialColumnIndex(col.columnId)], sortAsc: col.sortAsc }))
            }, e);
          }
        }
      });
    }
    setupColumnReorder() {
      $([this._headerColsL, this._headerColsR]).filter(":ui-sortable").sortable("destroy");
      var columnScrollTimer = null;
      var scrollColumnsRight = () => {
        this._scrollContainerX.scrollLeft = this._scrollContainerX.scrollLeft + 10;
      };
      var scrollColumnsLeft = () => {
        this._scrollContainerX.scrollLeft = this._scrollContainerX.scrollLeft - 10;
      };
      var canDragScroll;
      var hasGrouping = this._options.groupingPanel;
      $([this._headerColsL, this._headerColsR]).sortable({
        containment: hasGrouping ? void 0 : "parent",
        distance: 3,
        axis: hasGrouping ? void 0 : "x",
        cursor: "default",
        tolerance: "intersection",
        helper: "clone",
        placeholder: "slick-sortable-placeholder" + (this._options.useLegacyUI ? " ui-state-default" : "") + " slick-header-column",
        forcePlaceholderSize: hasGrouping ? true : void 0,
        appendTo: hasGrouping ? "body" : void 0,
        start: (_, ui) => {
          ui.placeholder.width(ui.helper.outerWidth() - this._headerColumnWidthDiff);
          canDragScroll = !this.hasFrozenColumns() || ui.placeholder.offset()[this._rtlS] + Math.round(ui.placeholder.width()) > $(this._scrollContainerX).offset()[this._rtlS];
          $(ui.helper).addClass("slick-header-column-active");
        },
        beforeStop: (_, ui) => {
          $(ui.helper).removeClass("slick-header-column-active");
          if (hasGrouping) {
            var $headerDraggableGroupBy = $(this.getGroupingPanel());
            var hasDroppedColumn = $headerDraggableGroupBy.find(".slick-dropped-grouping").length;
            if (hasDroppedColumn > 0) {
              $headerDraggableGroupBy.find(".slick-dropped-placeholder").hide();
              $headerDraggableGroupBy.find(".slick-dropped-grouping").show();
            }
          }
        },
        sort: (e) => {
          if (canDragScroll && e.originalEvent.pageX > this._container.clientWidth) {
            if (!columnScrollTimer) {
              columnScrollTimer = setInterval(scrollColumnsRight, 100);
            }
          } else if (canDragScroll && e.originalEvent.pageX < $(this._scrollContainerX).offset().left) {
            if (!columnScrollTimer) {
              columnScrollTimer = setInterval(scrollColumnsLeft, 100);
            }
          } else {
            clearInterval(columnScrollTimer);
            columnScrollTimer = null;
          }
        },
        stop: (e) => {
          var cancel = false;
          clearInterval(columnScrollTimer);
          columnScrollTimer = null;
          if (cancel || !this.getEditorLock().commitCurrentEdit()) {
            $(e.target).sortable("cancel");
            return;
          }
          ;
          var reorderedCols = sortToDesiredOrderAndKeepRest(this._initCols, this._headerColsL.sortable("toArray").map((x) => x.substring(this._uid.length)));
          reorderedCols = sortToDesiredOrderAndKeepRest(reorderedCols, this._headerColsR.sortable("toArray").map((x) => x.substring(this._uid.length)));
          this.setColumns(reorderedCols);
          this.trigger(this.onColumnsReordered, {});
          e.stopPropagation();
          this.setupColumnResize();
        }
      });
    }
    setupColumnResize() {
      var $col, j, k, c, pageX, columnElements, minPageX, maxPageX, firstResizable, lastResizable, cols = this._cols;
      columnElements = $([this._headerColsL, this._headerColsR]).children();
      columnElements.find(".slick-resizable-handle").remove();
      columnElements.each((i) => {
        if (cols[i].resizable) {
          if (firstResizable === void 0) {
            firstResizable = i;
          }
          lastResizable = i;
        }
      });
      if (firstResizable === void 0) {
        return;
      }
      columnElements.each((i, e) => {
        if (i < firstResizable || this._options.forceFitColumns && i >= lastResizable) {
          return;
        }
        $col = $(e);
        $("<div class='slick-resizable-handle' />").appendTo(e).on("dragstart", (e2, dd) => {
          if (!this.getEditorLock().commitCurrentEdit()) {
            return false;
          }
          pageX = e2.pageX;
          $(e2.target).parent().addClass("slick-header-column-active");
          var shrinkLeewayOnRight = null, stretchLeewayOnRight = null;
          columnElements.each((i2, e3) => {
            cols[i2].previousWidth = $(e3).outerWidth();
          });
          if (this._options.forceFitColumns) {
            shrinkLeewayOnRight = 0;
            stretchLeewayOnRight = 0;
            for (j = i + 1; j < columnElements.length; j++) {
              c = cols[j];
              if (c.resizable) {
                if (stretchLeewayOnRight != null) {
                  if (c.maxWidth) {
                    stretchLeewayOnRight += c.maxWidth - c.previousWidth;
                  } else {
                    stretchLeewayOnRight = null;
                  }
                }
                shrinkLeewayOnRight += c.previousWidth - Math.max(c.minWidth || 0, this._absoluteColMinWidth);
              }
            }
          }
          var shrinkLeewayOnLeft = 0, stretchLeewayOnLeft = 0;
          for (j = 0; j <= i; j++) {
            c = cols[j];
            if (c.resizable) {
              if (stretchLeewayOnLeft != null) {
                if (c.maxWidth) {
                  stretchLeewayOnLeft += c.maxWidth - c.previousWidth;
                } else {
                  stretchLeewayOnLeft = null;
                }
              }
              shrinkLeewayOnLeft += c.previousWidth - Math.max(c.minWidth || 0, this._absoluteColMinWidth);
            }
          }
          if (shrinkLeewayOnRight === null) {
            shrinkLeewayOnRight = 1e5;
          }
          if (shrinkLeewayOnLeft === null) {
            shrinkLeewayOnLeft = 1e5;
          }
          if (stretchLeewayOnRight === null) {
            stretchLeewayOnRight = 1e5;
          }
          if (stretchLeewayOnLeft === null) {
            stretchLeewayOnLeft = 1e5;
          }
          maxPageX = pageX + Math.min(shrinkLeewayOnRight, stretchLeewayOnLeft);
          minPageX = pageX - Math.min(shrinkLeewayOnLeft, stretchLeewayOnRight);
        }).on("drag", (e2, dd) => {
          var actualMinWidth, d = Math.min(maxPageX, Math.max(minPageX, e2.pageX)) - pageX, x;
          if (d < 0) {
            x = d;
            var newCanvasWidthL = 0, newCanvasWidthR = 0;
            for (j = i; j >= 0; j--) {
              c = cols[j];
              if (c.resizable) {
                actualMinWidth = Math.max(c.minWidth || 0, this._absoluteColMinWidth);
                if (x && c.previousWidth + x < actualMinWidth) {
                  x += c.previousWidth - actualMinWidth;
                  c.width = actualMinWidth;
                } else {
                  c.width = c.previousWidth + x;
                  x = 0;
                }
              }
            }
            var frozenCols = this._frozenCols;
            for (k = 0; k <= i; k++) {
              c = cols[k];
              if (frozenCols > 0 && k >= frozenCols) {
                newCanvasWidthR += c.width;
              } else {
                newCanvasWidthL += c.width;
              }
            }
            if (this._options.forceFitColumns) {
              x = -d;
              for (j = i + 1; j < columnElements.length; j++) {
                c = cols[j];
                if (c.resizable) {
                  if (x && c.maxWidth && c.maxWidth - c.previousWidth < x) {
                    x -= c.maxWidth - c.previousWidth;
                    c.width = c.maxWidth;
                  } else {
                    c.width = c.previousWidth + x;
                    x = 0;
                  }
                  if (frozenCols > 0 && j >= frozenCols) {
                    newCanvasWidthR += c.width;
                  } else {
                    newCanvasWidthL += c.width;
                  }
                }
              }
            } else {
              for (j = i + 1; j < columnElements.length; j++) {
                c = cols[j];
                if (frozenCols >= 0 && j >= frozenCols) {
                  newCanvasWidthR += c.width;
                } else {
                  newCanvasWidthL += c.width;
                }
              }
            }
          } else {
            x = d;
            var newCanvasWidthL = 0, newCanvasWidthR = 0;
            for (j = i; j >= 0; j--) {
              c = cols[j];
              if (c.resizable) {
                if (x && c.maxWidth && c.maxWidth - c.previousWidth < x) {
                  x -= c.maxWidth - c.previousWidth;
                  c.width = c.maxWidth;
                } else {
                  c.width = c.previousWidth + x;
                  x = 0;
                }
              }
            }
            for (k = 0; k <= i; k++) {
              c = cols[k];
              if (frozenCols > 0 && k >= frozenCols) {
                newCanvasWidthR += c.width;
              } else {
                newCanvasWidthL += c.width;
              }
            }
            if (this._options.forceFitColumns) {
              x = -d;
              for (j = i + 1; j < columnElements.length; j++) {
                c = cols[j];
                if (c.resizable) {
                  actualMinWidth = Math.max(c.minWidth || 0, this._absoluteColMinWidth);
                  if (x && c.previousWidth + x < actualMinWidth) {
                    x += c.previousWidth - actualMinWidth;
                    c.width = actualMinWidth;
                  } else {
                    c.width = c.previousWidth + x;
                    x = 0;
                  }
                  if (frozenCols && j >= frozenCols) {
                    newCanvasWidthR += c.width;
                  } else {
                    newCanvasWidthL += c.width;
                  }
                }
              }
            } else {
              for (j = i + 1; j < columnElements.length; j++) {
                c = cols[j];
                if (frozenCols > 0 && j >= frozenCols) {
                  newCanvasWidthR += c.width;
                } else {
                  newCanvasWidthL += c.width;
                }
              }
            }
          }
          if (this.hasFrozenColumns() && newCanvasWidthL != this._canvasWidthL) {
            this._headerColsL.style.width = newCanvasWidthL + 1e3 + "px";
            this._paneHeaderR.style[this._rtlS] = newCanvasWidthL + "px";
          }
          this.applyColumnHeaderWidths();
          if (this._options.syncColumnCellResize) {
            this.applyColumnWidths();
          }
        }).on("dragend", (e2) => {
          var newWidth;
          $(e2.target).parent().removeClass("slick-header-column-active");
          for (j = 0; j < columnElements.length; j++) {
            c = cols[j];
            newWidth = $(columnElements[j]).outerWidth();
            if (c.previousWidth !== newWidth && c.rerenderOnResize) {
              this.invalidateAllRows();
            }
          }
          this.updateCanvasWidth(true);
          this.render();
          this.trigger(this.onColumnsResized);
        });
      });
    }
    getVBoxDelta($el) {
      if ($el.css("box-sizing") == "border-box")
        return 0;
      var p = ["borderTopWidth", "borderBottomWidth", "paddingTop", "paddingBottom"];
      var delta = 0;
      $.each(p, function(n, val) {
        delta += parseFloat($el.css(val)) || 0;
      });
      return delta;
    }
    adjustFrozenRowOption() {
      this._options.frozenRow = this._options.frozenRow >= 0 && this._options.frozenRow < this._numVisibleRows ? this._options.frozenRow : -1;
      if (this._options.frozenRow > -1) {
        this._hasFrozenRows = true;
        this._frozenRowsHeight = this._options.frozenRow * this._options.rowHeight;
        var dataLength = this.getDataLength() || this._data.length;
        this._actualFrozenRow = this._options.frozenBottom ? dataLength - this._options.frozenRow : this._options.frozenRow;
      } else {
        this._hasFrozenRows = false;
      }
    }
    setPaneVisibility() {
      this._paneHeaderR.style.display = this._paneTopR.style.display = this._hasFrozenRows ? "" : "none";
      this._paneBottomL.style.display = this._hasFrozenRows ? "" : "none";
      this._paneBottomR.style.display = this._hasFrozenRows && this.hasFrozenColumns() ? "" : "none";
    }
    setOverflow() {
      var frozenRows = this._hasFrozenRows;
      var frozenCols = this.hasFrozenColumns();
      var alwaysHS = this._options.alwaysAllowHorizontalScroll;
      var alwaysVS = this._options.alwaysShowVerticalScroll;
      this._viewportTopL.style.overflowX = this._viewportTopR.style.overflowX = frozenRows && !alwaysHS ? "hidden" : frozenCols ? "scroll" : "auto";
      this._viewportTopL.style.overflowY = this._viewportBottomL.style.overflowY = !frozenCols && alwaysVS ? "scroll" : frozenCols ? "hidden" : frozenRows ? "scroll" : "auto";
      this._viewportTopR.style.overflowY = alwaysVS || frozenRows ? "scroll" : "auto";
      this._viewportBottomL.style.overflowX = this._viewportBottomR.style.overflowX = frozenCols && !alwaysHS ? "scroll" : "auto";
      this._viewportBottomR.style.overflowY = alwaysVS ? "scroll" : "auto";
      if (this._options.viewportClass)
        this.getViewports().toggleClass(this._options.viewportClass, true);
    }
    setScroller() {
      if (this.hasFrozenColumns()) {
        if (this._hasFrozenRows) {
          if (this._options.frozenBottom) {
            this._scrollContainerX = this._viewportBottomR;
            this._scrollContainerY = this._viewportTopR;
          } else {
            this._scrollContainerX = this._scrollContainerY = this._viewportBottomR;
          }
        } else {
          this._scrollContainerX = this._scrollContainerY = this._viewportTopR;
        }
      } else {
        if (this._hasFrozenRows) {
          if (this._options.frozenBottom) {
            this._scrollContainerX = this._viewportBottomL;
            this._scrollContainerY = this._viewportTopL;
          } else {
            this._scrollContainerX = this._scrollContainerY = this._viewportBottomL;
          }
        } else {
          this._scrollContainerX = this._scrollContainerY = this._viewportTopL;
        }
      }
    }
    measureCellPaddingAndBorder() {
      var el;
      var h = ["borderLeftWidth", "borderRightWidth", "paddingLeft", "paddingRight"];
      var v = ["borderTopWidth", "borderBottomWidth", "paddingTop", "paddingBottom"];
      var verArray = $.fn.jquery.split(".");
      this._jQueryNewWidthBehaviour = verArray[0] == "1" && verArray[1] >= 8 || verArray[0] >= 2;
      el = $("<div class='" + (this._options.useLegacyUI ? "ui-state-default " : "") + "slick-header-column' style='visibility:hidden'>-</div>").appendTo(this._headerColsL);
      this._headerColumnWidthDiff = 0;
      if (el.css("box-sizing") != "border-box" && el.css("-moz-box-sizing") != "border-box" && el.css("-webkit-box-sizing") != "border-box") {
        $.each(h, (_, val) => {
          this._headerColumnWidthDiff += parseFloat(el.css(val)) || 0;
        });
      }
      el.remove();
      var r = $("<div class='slick-row' />").appendTo(this._canvasTopL);
      el = $("<div class='slick-cell' id='' style='visibility:hidden'>-</div>").appendTo(r);
      this._cellWidthDiff = this._cellHeightDiff = 0;
      if (el.css("box-sizing") != "border-box" && el.css("-moz-box-sizing") != "border-box" && el.css("-webkit-box-sizing") != "border-box") {
        $.each(h, (_, val) => {
          this._cellWidthDiff += parseFloat(el.css(val)) || 0;
        });
        $.each(v, (_, val) => {
          this._cellHeightDiff += parseFloat(el.css(val)) || 0;
        });
      }
      r.remove();
      this._absoluteColMinWidth = Math.max(this._headerColumnWidthDiff, this._cellWidthDiff);
    }
    createCssRules() {
      var el = this._styleNode = document.createElement("style");
      el.dataset.uid = this._uid;
      var rowHeight = this._options.rowHeight - this._cellHeightDiff;
      var rules = [
        "." + this._uid + " .slick-group-header-column { " + this._rtlS + ": 1000px; }",
        "." + this._uid + " .slick-header-column { " + this._rtlS + ": 1000px; }",
        "." + this._uid + " .slick-top-panel { height:" + this._options.topPanelHeight + "px; }",
        "." + this._uid + " .slick-grouping-panel { height:" + this._options.groupingPanelHeight + "px; }",
        "." + this._uid + " .slick-headerrow-columns { height:" + this._options.headerRowHeight + "px; }",
        "." + this._uid + " .slick-cell { height:" + rowHeight + "px; }",
        "." + this._uid + " .slick-row { height:" + this._options.rowHeight + "px; }",
        "." + this._uid + " .slick-footerrow-columns { height:" + this._options.footerRowHeight + "px; }"
      ];
      var cols = this._cols;
      for (var i = 0; i < cols.length; i++) {
        rules.push("." + this._uid + " .l" + i + " { }");
        rules.push("." + this._uid + " .r" + i + " { }");
      }
      el.appendChild(document.createTextNode(rules.join(" ")));
      document.head.appendChild(el);
    }
    getColumnCssRules(idx) {
      if (!this._stylesheet) {
        var stylesheetFromUid = document.querySelector("style[data-uid='" + this._uid + "']");
        if (stylesheetFromUid && stylesheetFromUid.sheet) {
          this._stylesheet = stylesheetFromUid.sheet;
        } else {
          var sheets = document.styleSheets;
          for (var i = 0; i < sheets.length; i++) {
            if ((sheets[i].ownerNode || sheets[i].owningElement) == this._styleNode) {
              this._stylesheet = sheets[i];
              break;
            }
          }
        }
        if (!this._stylesheet) {
          throw new Error("Cannot find stylesheet.");
        }
        this._columnCssRulesL = [];
        this._columnCssRulesR = [];
        var cssRules = this._stylesheet.cssRules || this._stylesheet.rules;
        var matches, columnIdx;
        for (var i = 0; i < cssRules.length; i++) {
          var selector = cssRules[i].selectorText;
          if (matches = /\.l\d+/.exec(selector)) {
            columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
            this._columnCssRulesL[columnIdx] = cssRules[i];
          } else if (matches = /\.r\d+/.exec(selector)) {
            columnIdx = parseInt(matches[0].substring(2, matches[0].length), 10);
            this._columnCssRulesR[columnIdx] = cssRules[i];
          }
        }
      }
      return this._rtl ? {
        "right": this._columnCssRulesL[idx],
        "left": this._columnCssRulesR[idx]
      } : {
        "left": this._columnCssRulesL[idx],
        "right": this._columnCssRulesR[idx]
      };
    }
    removeCssRules() {
      this._styleNode.remove();
      this._stylesheet = null;
    }
    destroy() {
      this.getEditorLock().cancelCurrentEdit();
      this.trigger(this.onBeforeDestroy);
      var i = this._plugins.length;
      while (i--) {
        this.unregisterPlugin(this._plugins[i]);
      }
      if (this._options.enableColumnReorder) {
        $([this._headerColsL, this._headerColsR]).filter(":ui-sortable").sortable("destroy");
      }
      this.unbindAncestorScrollEvents();
      $(this._container).off(".slickgrid");
      this.removeCssRules();
      this.getCanvases().off("draginit dragstart dragend drag");
      $(this._container).empty().removeClass(this._uid);
      for (var k in this) {
        if (!Object.prototype.hasOwnProperty.call(this, k))
          continue;
        if (k.startsWith("on")) {
          var ev = this[k];
          if ((ev == null ? void 0 : ev.clear) && (ev == null ? void 0 : ev.subscribe))
            ev == null ? void 0 : ev.clear();
        }
        delete this[k];
      }
    }
    trigger(evt, args, e) {
      e = e || new EventData();
      args = args || {};
      args.grid = this;
      return evt.notify(args, e, this);
    }
    getEditorLock() {
      return this._options.editorLock;
    }
    getEditController() {
      return this._editController;
    }
    getColumnIndex(id) {
      return this._colById[id];
    }
    getInitialColumnIndex(id) {
      return this._initColById[id];
    }
    autosizeColumns() {
      var i, c, widths = [], shrinkLeeway = 0, total = 0, prevTotal, availWidth = this._viewportHasVScroll ? this._viewportW - this._scrollDims.width : this._viewportW, cols = this._cols;
      for (i = 0; i < cols.length; i++) {
        c = cols[i];
        widths.push(c.width);
        total += c.width;
        if (c.resizable) {
          shrinkLeeway += c.width - Math.max(c.minWidth, this._absoluteColMinWidth);
        }
      }
      prevTotal = total;
      while (total > availWidth && shrinkLeeway) {
        var shrinkProportion = (total - availWidth) / shrinkLeeway;
        for (i = 0; i < cols.length && total > availWidth; i++) {
          c = cols[i];
          var width = widths[i];
          if (!c.resizable || width <= c.minWidth || width <= this._absoluteColMinWidth) {
            continue;
          }
          var absMinWidth = Math.max(c.minWidth, this._absoluteColMinWidth);
          var shrinkSize = Math.floor(shrinkProportion * (width - absMinWidth)) || 1;
          shrinkSize = Math.min(shrinkSize, width - absMinWidth);
          total -= shrinkSize;
          shrinkLeeway -= shrinkSize;
          widths[i] -= shrinkSize;
        }
        if (prevTotal <= total) {
          break;
        }
        prevTotal = total;
      }
      prevTotal = total;
      while (total < availWidth) {
        var growProportion = availWidth / total;
        for (i = 0; i < cols.length && total < availWidth; i++) {
          c = cols[i];
          var currentWidth = widths[i];
          var growSize;
          if (!c.resizable || c.maxWidth <= currentWidth) {
            growSize = 0;
          } else {
            growSize = Math.min(Math.floor(growProportion * currentWidth) - currentWidth, c.maxWidth - currentWidth || 1e6) || 1;
          }
          total += growSize;
          widths[i] += total <= availWidth ? growSize : 0;
        }
        if (prevTotal >= total) {
          break;
        }
        prevTotal = total;
      }
      var reRender = false;
      for (i = 0; i < cols.length; i++) {
        if (cols[i].rerenderOnResize && cols[i].width != widths[i]) {
          reRender = true;
        }
        cols[i].width = widths[i];
      }
      this.applyColumnHeaderWidths();
      this.updateCanvasWidth(true);
      if (reRender) {
        this.invalidateAllRows();
        this.render();
      }
    }
    applyColumnHeaderWidths() {
      if (!this._initialized) {
        return;
      }
      var h, cols = this._cols;
      for (var i = 0, headers = $([this._headerColsL, this._headerColsR]).children(), ii = headers.length; i < ii; i++) {
        h = $(headers[i]);
        if (this._jQueryNewWidthBehaviour) {
          if (h.outerWidth() !== cols[i].width) {
            h.outerWidth(cols[i].width);
          }
        } else {
          if (Math.round(h.width()) !== cols[i].width - this._headerColumnWidthDiff) {
            h.width(cols[i].width - this._headerColumnWidthDiff);
          }
        }
      }
      this.updateViewColLeftRight();
    }
    applyColumnWidths() {
      var x = 0, w, rule, cols = this._cols, frozenCols = this._frozenCols;
      for (var i = 0; i < cols.length; i++) {
        if (frozenCols == i)
          x = 0;
        w = cols[i].width;
        rule = this.getColumnCssRules(i);
        rule[this._rtlS].style[this._rtlS] = x + "px";
        rule[this._rtlE].style[this._rtlE] = (frozenCols > 0 && i >= frozenCols ? this._canvasWidthR : this._canvasWidthL) - x - w + "px";
        x += w;
      }
    }
    setSortColumn(columnId, ascending) {
      this.setSortColumns([{ columnId, sortAsc: ascending }]);
    }
    setSortColumns(cols) {
      this._sortColumns = cols || [];
      var headerColumnEls = $([this._headerColsL, this._headerColsR]).children();
      headerColumnEls.removeClass("slick-header-column-sorted").find(".slick-sort-indicator").removeClass("slick-sort-indicator-asc slick-sort-indicator-desc");
      $.each(this._sortColumns, (_, col) => {
        if (col.sortAsc == null) {
          col.sortAsc = true;
        }
        var columnIndex = this.getColumnIndex(col.columnId);
        if (columnIndex != null) {
          headerColumnEls.eq(columnIndex).addClass("slick-header-column-sorted").find(".slick-sort-indicator").addClass(col.sortAsc ? "slick-sort-indicator-asc" : "slick-sort-indicator-desc");
        }
      });
    }
    getSortColumns() {
      return this._sortColumns;
    }
    getColumns() {
      return this._cols;
    }
    getInitialColumns() {
      return this._initCols;
    }
    updateViewColLeftRight() {
      this._colLeft = [];
      this._colRight = [];
      var x = 0, r, cols = this._cols, i, l = cols.length, frozenCols = this._frozenCols;
      for (var i = 0; i < l; i++) {
        if (frozenCols === i)
          x = 0;
        r = x + cols[i].width;
        this._colLeft[i] = x;
        this._colRight[i] = r;
        x = r;
      }
    }
    setInitialCols(initCols) {
      var defs = this._colDefaults;
      var initColById = {};
      var frozenColumns = [];
      var viewCols = [];
      var viewColById = {};
      var i, m, k;
      for (i = 0; i < initCols.length; i++) {
        m = initCols[i];
        for (k in defs) {
          if (m[k] === void 0)
            m[k] = this._colDefaults[k];
        }
        if (m.minWidth && m.width < m.minWidth) {
          m.width = m.minWidth;
        }
        if (m.maxWidth && m.width > m.maxWidth) {
          m.width = m.maxWidth;
        }
        initColById[m.id] = i;
        if (m.visible !== false) {
          (m.frozen ? frozenColumns : viewCols).push(m);
        }
      }
      this._frozenCols = frozenColumns.length;
      if (frozenColumns.length > 0)
        viewCols = frozenColumns.concat(viewCols);
      for (i = 0; i < viewCols.length; i++) {
        m = viewCols[i];
        viewColById[m.id] = i;
      }
      this._initCols = initCols;
      this._initColById = initColById;
      this._cols = viewCols;
      this._colById = viewColById;
    }
    setColumns(columns) {
      var _a;
      this.setInitialCols(columns);
      this.updateViewColLeftRight();
      if (this._initialized) {
        this.setPaneVisibility();
        this.setOverflow();
        this.invalidateAllRows();
        this.createColumnHeaders();
        this.createColumnFooters();
        this.updateFooterTotals();
        this.removeCssRules();
        this.createCssRules();
        this.resizeCanvas();
        this.updateCanvasWidth();
        this.applyColumnWidths();
        this.handleScroll();
        (_a = this.getSelectionModel()) == null ? void 0 : _a.refreshSelections();
      }
    }
    getOptions() {
      return this._options;
    }
    setOptions(args, suppressRender, suppressColumnSet, suppressSetOverflow) {
      var _a;
      if (!this.getEditorLock().commitCurrentEdit()) {
        return;
      }
      this.makeActiveCellNormal();
      if (args.showColumnHeader !== void 0) {
        this.setColumnHeaderVisibility(args.showColumnHeader);
      }
      if (this._options.enableAddRow !== args.enableAddRow) {
        this.invalidateRow(this.getDataLength());
      }
      this._options = $.extend(this._options, args);
      this.validateAndEnforceOptions();
      this.adjustFrozenRowOption();
      this.getViewports().css("overflow-y", this._options.autoHeight ? "hidden" : "auto");
      if (!suppressSetOverflow) {
        this.setOverflow();
      }
      if (args.columns && !suppressColumnSet) {
        adjustFrozenColumnCompat(args.columns, this._options);
        this.setColumns((_a = args.columns) != null ? _a : this._initCols);
      } else if (args.frozenColumn != null) {
        adjustFrozenColumnCompat(this._initCols, this._options);
        this.setColumns(this._initCols);
      }
      this.setScroller();
      if (!suppressRender)
        this.render();
    }
    validateAndEnforceOptions() {
      if (this._options.autoHeight) {
        this._options.leaveSpaceForNewRows = false;
      }
    }
    bindToData() {
      if (this._data) {
        this._data.onRowCountChanged && this._data.onRowCountChanged.subscribe(this.viewOnRowCountChanged);
        this._data.onRowsChanged && this._data.onRowsChanged.subscribe(this.viewOnRowsChanged);
        this._data.onDataChanged && this._data.onDataChanged.subscribe(this.viewOnDataChanged);
      }
    }
    unbindFromData() {
      if (this._data) {
        this._data.onRowCountChanged && this._data.onRowCountChanged.unsubscribe(this.viewOnRowCountChanged);
        this._data.onRowsChanged && this._data.onRowsChanged.unsubscribe(this.viewOnRowsChanged);
        this._data.onDataChanged && this._data.onDataChanged.unsubscribe(this.viewOnDataChanged);
      }
    }
    setData(newData, scrollToTop) {
      this.unbindFromData();
      this._data = newData;
      this.bindToData();
      this.invalidateAllRows();
      this.updateRowCount();
      if (scrollToTop) {
        this.scrollTo(0);
      }
    }
    getData() {
      return this._data;
    }
    getDataLength() {
      if (this._data.getLength) {
        return this._data.getLength();
      } else {
        return this._data.length;
      }
    }
    getDataLengthIncludingAddNew() {
      return this.getDataLength() + (!this._options.enableAddRow ? 0 : !this._pagingActive || this._pagingIsLastPage ? 1 : 0);
    }
    getDataItem(i) {
      if (this._data.getItem) {
        return this._data.getItem(i);
      } else {
        return this._data[i];
      }
    }
    getTopPanel() {
      return this._topPanelL;
    }
    setTopPanelVisibility(visible) {
      if (this._options.showTopPanel != visible) {
        this._options.showTopPanel = !!visible;
        $([this._topPanelL.parentElement, this._topPanelR.parentElement])[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas.bind(this));
      }
    }
    setColumnHeaderVisibility(visible, animate) {
      if (this._options.showColumnHeader != visible) {
        this._options.showColumnHeader = visible;
        var headerScroller = $([this._headerColsL.parentElement, this._headerColsR.parentElement]);
        if (visible) {
          if (animate) {
            headerScroller.slideDown("fast", this.resizeCanvas.bind(this));
          } else {
            headerScroller.show();
            this.resizeCanvas();
          }
        } else {
          if (animate) {
            headerScroller.slideUp("fast", this.resizeCanvas.bind(this));
          } else {
            headerScroller.hide();
            this.resizeCanvas();
          }
        }
      }
    }
    setFooterRowVisibility(visible) {
      if (this._options.showFooterRow != visible) {
        this._options.showFooterRow = !!visible;
        $([this._footerRowColsL.parentElement, this._footerRowR.parentElement])[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas.bind(this));
      }
    }
    setGroupingPanelVisibility(visible) {
      if (this._options.showGroupingPanel != visible) {
        this._options.showGroupingPanel = visible;
        if (!this._options.groupingPanel)
          return;
        if (visible) {
          $(this._groupingPanel).slideDown("fast", this.resizeCanvas.bind(this));
        } else {
          $(this._groupingPanel).slideUp("fast", this.resizeCanvas.bind(this));
        }
      }
    }
    setHeaderRowVisibility(visible) {
      if (this._options.showHeaderRow != visible) {
        this._options.showHeaderRow = visible;
        $([this._headerRowColsL.parentElement, this._headerRowColsR.parentElement])[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas.bind(this));
      }
    }
    getContainerNode() {
      return this._container;
    }
    getUID() {
      return this._uid;
    }
    getRowTop(row) {
      return this._options.rowHeight * row - this._pageOffset;
    }
    getRowFromPosition(y) {
      return Math.floor((y + this._pageOffset) / this._options.rowHeight);
    }
    scrollTo(y) {
      y = Math.max(y, 0);
      y = Math.min(y, this._virtualHeight - Math.round($(this._scrollContainerY).height()) + (this._viewportHasHScroll || this.hasFrozenColumns() ? this._scrollDims.height : 0));
      var oldOffset = this._pageOffset;
      this._page = Math.min(this._numberOfPages - 1, Math.floor(y / this._pageHeight));
      this._pageOffset = Math.round(this._page * this._jumpinessCoefficient);
      var newScrollTop = y - this._pageOffset;
      if (this._pageOffset != oldOffset) {
        var range = this.getVisibleRange(newScrollTop);
        this.cleanupRows(range);
        this.updateRowPositions();
      }
      if (this._scrollTopPrev != newScrollTop) {
        this._vScrollDir = this._scrollTopPrev + oldOffset < newScrollTop + this._pageOffset ? 1 : -1;
        this._scrollTopRendered = this._scrollTop = this._scrollTopPrev = newScrollTop;
        if (this.hasFrozenColumns()) {
          this._viewportTopL.scrollTop = newScrollTop;
        }
        if (this._hasFrozenRows) {
          this._viewportBottomL.scrollTop = this._viewportBottomR.scrollTop = newScrollTop;
        }
        this._scrollContainerY.scrollTop = newScrollTop;
        this.trigger(this.onViewportChanged);
      }
    }
    getFormatter(row, column) {
      var data = this._data;
      var rowMetadata = data.getItemMetadata && data.getItemMetadata(row);
      var colsMetadata = rowMetadata && rowMetadata.columns;
      var colOverrides = colsMetadata && (colsMetadata[column.id] || colsMetadata[this.getInitialColumnIndex(column.id)]);
      return colOverrides && colOverrides.formatter || rowMetadata && rowMetadata.formatter || column.formatter || this._options.formatterFactory && this._options.formatterFactory.getFormatter(column) || this._options.defaultFormatter;
    }
    getEditor(row, cell) {
      var column = this._cols[cell];
      var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      var columnMetadata = rowMetadata && rowMetadata.columns;
      if (columnMetadata && columnMetadata[column.id] && columnMetadata[column.id].editor !== void 0) {
        return columnMetadata[column.id].editor;
      }
      if (columnMetadata && columnMetadata[cell] && columnMetadata[cell].editor !== void 0) {
        return columnMetadata[cell].editor;
      }
      return column.editor || this._options.editorFactory && this._options.editorFactory.getEditor(column);
    }
    getDataItemValueForColumn(item, columnDef) {
      if (this._options.dataItemColumnValueExtractor) {
        return this._options.dataItemColumnValueExtractor(item, columnDef);
      }
      return item[columnDef.field];
    }
    appendRowHtml(stringArrayL, stringArrayR, row, range, dataLength) {
      var d = this.getDataItem(row);
      var dataLoading = row < dataLength && !d;
      var rowCss = "slick-row" + (this._hasFrozenRows && row <= this._options.frozenRow ? " frozen" : "") + (dataLoading ? " loading" : "") + (row === this._activeRow ? " active" : "") + (row % 2 == 1 ? " odd" : " even");
      if (!d) {
        rowCss += " " + this._options.addNewRowCssClass;
      }
      var metadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      if (metadata && metadata.cssClasses) {
        rowCss += " " + metadata.cssClasses;
      }
      var frozenRowOffset = this.getFrozenRowOffset(row);
      var rowHtml = "<div class='" + (this._options.useLegacyUI ? "ui-widget-content " : "") + rowCss + "' style='top:" + (this.getRowTop(row) - frozenRowOffset) + "px'>";
      stringArrayL.push(rowHtml);
      if (this.hasFrozenColumns()) {
        stringArrayR.push(rowHtml);
      }
      var colspan, m, cols = this._cols, frozenCols = this._frozenCols;
      for (var i = 0, ii = cols.length; i < ii; i++) {
        var columnData = null;
        m = cols[i];
        colspan = 1;
        if (metadata && metadata.columns) {
          columnData = metadata.columns[m.id] || metadata.columns[i];
          colspan = columnData && columnData.colspan || 1;
          if (colspan === "*") {
            colspan = ii - i;
          }
        }
        if (this._colRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
          if (this._colLeft[i] > range.rightPx) {
            break;
          }
          this.appendCellHtml(frozenCols > 0 && i >= frozenCols ? stringArrayR : stringArrayL, row, i, colspan, d, columnData);
        }
        if (colspan > 1) {
          i += colspan - 1;
        }
      }
      stringArrayL.push("</div>");
      if (this.hasFrozenColumns()) {
        stringArrayR.push("</div>");
      }
    }
    appendCellHtml(sb, row, cell, colspan, item, metadata) {
      var _a, _b, _c;
      var cols = this._cols, frozenCols = this._frozenCols, m = cols[cell];
      var klass = "slick-cell l" + cell + " r" + Math.min(cols.length - 1, cell + colspan - 1) + (m.cssClass ? " " + m.cssClass : "");
      if (cell < frozenCols)
        klass += " frozen";
      if (row === this._activeRow && cell === this._activeCell)
        klass += " active";
      if (metadata && metadata.cssClasses) {
        klass += " " + metadata.cssClasses;
      }
      for (var key in this._cellCssClasses) {
        if (this._cellCssClasses[key][row] && this._cellCssClasses[key][row][m.id]) {
          klass += " " + this._cellCssClasses[key][row][m.id];
        }
      }
      var fmtResult;
      if (item) {
        var value = this.getDataItemValueForColumn(item, m);
        fmtResult = this.getFormatter(row, m)(row, cell, value, m, item, this);
      }
      if (fmtResult == null)
        sb.push('<div class="' + attrEncode(klass) + '"></div>');
      else if (typeof fmtResult === "string")
        sb.push('<div class="' + attrEncode(klass) + '">' + fmtResult + "</div>");
      else {
        if ((_a = fmtResult.addClass) == null ? void 0 : _a.length)
          klass += " " + fmtResult.addClass;
        sb.push('<div class="' + attrEncode(klass) + '"');
        if ((_b = fmtResult.addClass) == null ? void 0 : _b.length)
          sb.push(' data-fmtcls="' + attrEncode(fmtResult.addClass) + '"');
        var attrs = fmtResult.addAttrs;
        if (attrs != null) {
          var ks = [];
          for (var k in attrs) {
            sb.push(k + '="' + attrEncode(attrs[k]) + '"');
            ks.push(k);
          }
          sb.push(' data-fmtatt="' + attrEncode(ks.join(",")) + '"');
        }
        var toolTip = fmtResult.toolTip;
        if (toolTip != null && toolTip.length)
          sb.push('tooltip="' + attrEncode(toolTip) + '"');
        if ((_c = fmtResult.text) == null ? void 0 : _c.length)
          sb.push(">" + fmtResult.text + "</div>");
        else
          sb.push("></div>");
      }
      this._rowsCache[row].cellRenderQueue.push(cell);
      this._rowsCache[row].cellColSpans[cell] = colspan;
    }
    cleanupRows(rangeToKeep) {
      var i;
      for (var x in this._rowsCache) {
        var removeFrozenRow = true;
        i = parseInt(x, 10);
        if (this._hasFrozenRows && (this._options.frozenBottom && i >= this._actualFrozenRow || !this._options.frozenBottom && i <= this._actualFrozenRow)) {
          removeFrozenRow = false;
        }
        if (i !== this._activeRow && (i < rangeToKeep.top || i > rangeToKeep.bottom) && removeFrozenRow) {
          this.removeRowFromCache(i);
        }
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
    }
    invalidate() {
      this.updateRowCount();
      this.invalidateAllRows();
      this.render();
      this.updateFooterTotals();
    }
    invalidateAllRows() {
      if (this._currentEditor) {
        this.makeActiveCellNormal();
      }
      for (var row in this._rowsCache) {
        this.removeRowFromCache(parseInt(row, 10));
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
    }
    queuePostProcessedRowForCleanup(cacheEntry, postProcessedRow, rowIdx) {
      this._postProcessGroupId++;
      for (var x in postProcessedRow) {
        if (postProcessedRow.hasOwnProperty(x)) {
          var columnIdx = parseInt(x, 10);
          this._postProcessCleanupQueue.push({
            groupId: this._postProcessGroupId,
            cellNode: cacheEntry.cellNodesByColumnIdx[columnIdx | 0],
            columnIdx: columnIdx | 0,
            rowIdx
          });
        }
      }
      this._postProcessCleanupQueue.push({
        groupId: this._postProcessGroupId,
        rowNode: cacheEntry.rowNode
      });
      $(cacheEntry.rowNode).detach();
    }
    queuePostProcessedCellForCleanup(cellnode, columnIdx, rowIdx) {
      this._postProcessCleanupQueue.push({
        groupId: this._postProcessGroupId,
        cellNode: cellnode,
        columnIdx,
        rowIdx
      });
      $(cellnode).detach();
    }
    removeRowFromCache(row) {
      var cacheEntry = this._rowsCache[row];
      if (!cacheEntry) {
        return;
      }
      if (this._options.enableAsyncPostRenderCleanup && this._postProcessedRows[row]) {
        this.queuePostProcessedRowForCleanup(cacheEntry, this._postProcessedRows[row], row);
      } else {
        cacheEntry.rowNode.each(function() {
          if (this.parentElement)
            this.parentElement.removeChild(this);
        });
      }
      delete this._rowsCache[row];
      delete this._postProcessedRows[row];
    }
    invalidateRows(rows) {
      var i, rl;
      if (!rows || !rows.length) {
        return;
      }
      this._vScrollDir = 0;
      for (i = 0, rl = rows.length; i < rl; i++) {
        if (this._currentEditor && this._activeRow === rows[i]) {
          this.makeActiveCellNormal();
        }
        if (this._rowsCache[rows[i]]) {
          this.removeRowFromCache(rows[i]);
        }
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
    }
    invalidateRow(row) {
      this.invalidateRows([row]);
    }
    applyFormatResultToCellNode(fmtResult, cellNode) {
      var _a, _b, _c, _d;
      var oldFmtCls = (_a = cellNode.dataset) == null ? void 0 : _a.fmtcls;
      if (oldFmtCls != null && oldFmtCls.length > 0) {
        cellNode.classList.remove(...oldFmtCls.split(" "));
        delete cellNode.dataset.fmtcls;
      }
      var oldFmtAtt = (_b = cellNode.dataset) == null ? void 0 : _b.fmtatt;
      if (oldFmtAtt != null && oldFmtAtt.length > 0) {
        for (var k of oldFmtAtt.split(","))
          cellNode.removeAttribute(k);
        delete cellNode.dataset.fmtatt;
      }
      cellNode.removeAttribute("tooltip");
      if (fmtResult == null) {
        cellNode.innerHTML = "";
        return;
      }
      if (typeof fmtResult === "string") {
        cellNode.innerHTML = fmtResult;
        return;
      }
      cellNode.innerHTML = fmtResult.text;
      if ((_c = fmtResult.addClass) == null ? void 0 : _c.length) {
        cellNode.classList.add(...fmtResult.addClass.split(" "));
        cellNode.dataset.fmtcls = fmtResult.addClass;
      }
      if (fmtResult.addAttrs != null) {
        var keys = Object.keys(fmtResult.addAttrs);
        if (keys.length) {
          for (var k of keys) {
            cellNode.setAttribute(k, fmtResult.addAttrs[k]);
          }
          cellNode.dataset.fmtatt = keys.join(",");
        }
      }
      if ((_d = fmtResult.toolTip) == null ? void 0 : _d.length)
        cellNode.setAttribute("tooltip", fmtResult.toolTip);
    }
    updateCell(row, cell) {
      var cellNode = this.getCellNode(row, cell);
      if (!cellNode) {
        return;
      }
      var m = this._cols[cell], d = this.getDataItem(row);
      if (this._currentEditor && this._activeRow === row && this._activeCell === cell) {
        this._currentEditor.loadValue(d);
      } else {
        var fmtResult = d ? this.getFormatter(row, m)(row, cell, this.getDataItemValueForColumn(d, m), m, d) : "";
        this.applyFormatResultToCellNode(fmtResult, cellNode);
        this.invalidatePostProcessingResults(row);
      }
    }
    updateRow(row) {
      var cacheEntry = this._rowsCache[row];
      if (!cacheEntry) {
        return;
      }
      this.ensureCellNodesInRowsCache(row);
      var d = this.getDataItem(row);
      var fmtResult;
      for (var x in cacheEntry.cellNodesByColumnIdx) {
        if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
          continue;
        }
        var columnIdx = parseInt(x, 10);
        var m = this._cols[columnIdx], node = cacheEntry.cellNodesByColumnIdx[columnIdx];
        if (row === this._activeRow && columnIdx === this._activeCell && this._currentEditor) {
          this._currentEditor.loadValue(d);
        } else {
          fmtResult = d ? this.getFormatter(row, m)(row, columnIdx, this.getDataItemValueForColumn(d, m), m, d) : "";
          this.applyFormatResultToCellNode(fmtResult, node);
        }
      }
      this.invalidatePostProcessingResults(row);
    }
    getViewportHeight() {
      if (!this._options.autoHeight || this.hasFrozenColumns()) {
        this._topPanelH = this._options.showTopPanel ? this._options.topPanelHeight + this.getVBoxDelta($(this._topPanelL.parentElement)) : 0;
        this._headerRowH = this._options.showHeaderRow ? this._options.headerRowHeight + this.getVBoxDelta($(this._headerRowColsL.parentElement)) : 0;
        this._footerRowH = this._options.showFooterRow ? this._options.footerRowHeight + this.getVBoxDelta($(this._footerRowColsL.parentElement)) : 0;
      }
      if (this._options.autoHeight) {
        this._viewportH = this._options.rowHeight * this.getDataLengthIncludingAddNew();
        if (!this.hasFrozenColumns()) {
          this._viewportH += $(this._paneHeaderL).outerHeight();
          this._viewportH += this._options.showHeaderRow ? this._options.headerRowHeight + this.getVBoxDelta($(this._headerRowColsL.parentElement)) : 0;
          this._viewportH += this._options.showFooterRow ? this._options.footerRowHeight + this.getVBoxDelta($(this._footerRowColsL.parentElement)) : 0;
          this._viewportH += this.getCanvasWidth() > this._viewportW ? this._scrollDims.height : 0;
        }
      } else {
        var columnNamesH = this._options.showColumnHeader ? parseFloat($.css(this._headerColsL.parentElement, "height")) + this.getVBoxDelta($(this._headerColsL.parentElement)) : 0;
        this._groupingPanelH = this._options.groupingPanel && this._options.showGroupingPanel ? this._options.groupingPanelHeight + this.getVBoxDelta($(this._groupingPanel)) : 0;
        this._viewportH = parseFloat($.css(this._container, "height", true)) - parseFloat($.css(this._container, "paddingTop", true)) - parseFloat($.css(this._container, "paddingBottom", true)) - columnNamesH - this._topPanelH - this._headerRowH - this._footerRowH - this._groupingPanelH;
      }
      this._numVisibleRows = Math.ceil(this._viewportH / this._options.rowHeight);
      return this._viewportH;
    }
    getViewportWidth() {
      this._viewportW = parseFloat($.css(this._container, "width", true));
    }
    resizeCanvas() {
      if (!this._initialized) {
        return;
      }
      this._paneTopH = 0;
      this._paneBottomH = 0;
      this._viewportTopH = 0;
      this.getViewportWidth();
      this.getViewportHeight();
      if (this._hasFrozenRows) {
        if (this._options.frozenBottom) {
          this._paneTopH = this._viewportH - this._frozenRowsHeight - this._scrollDims.height;
          this._paneBottomH = this._frozenRowsHeight + this._scrollDims.height;
        } else {
          this._paneTopH = this._frozenRowsHeight;
          this._paneBottomH = this._viewportH - this._frozenRowsHeight;
        }
      } else {
        this._paneTopH = this._viewportH;
      }
      this._paneTopH += this._topPanelH + this._headerRowH + this._footerRowH;
      if (this.hasFrozenColumns() && this._options.autoHeight) {
        this._paneTopH += this._scrollDims.height;
      }
      this._viewportTopH = this._paneTopH - this._topPanelH - this._headerRowH - this._footerRowH;
      if (this._options.autoHeight) {
        if (this.hasFrozenColumns()) {
          this._container.style.height = this._paneTopH + parseFloat($.css(this._headerColsL.parentElement, "height")) + "px";
        }
        this._paneTopL.style.position = "relative";
      } else
        this._paneTopL.style.position = "";
      this._paneTopL.style.top = ($(this._paneHeaderL).height() || (this._options.showHeaderRow ? this._options.headerRowHeight : 0) + this._groupingPanelH) + "px";
      this._paneTopL.style.height = this._paneTopH + "px";
      var paneBottomTop = $(this._paneTopL).position().top + this._paneTopH;
      if (!this._options.autoHeight) {
        this._viewportTopL.style.height = this._viewportTopH + "px";
      }
      if (this.hasFrozenColumns()) {
        this._paneTopR.style.top = this._paneTopL.style.top;
        this._paneTopR.style.height = this._paneTopH + "px";
        this._viewportTopR.style.height = this._viewportTopH + "px";
        if (this._hasFrozenRows) {
          this._paneBottomL.style.top = paneBottomTop + "px";
          this._paneBottomL.style.height = this._paneBottomH + "px";
          this._paneBottomR.style.top = paneBottomTop + "px";
          this._paneBottomR.style.height = this._paneBottomH + "px";
          this._viewportBottomR.style.height = this._paneBottomH + "px";
        }
      } else {
        if (this._hasFrozenRows) {
          this._paneBottomL.style.width = "100%";
          this._paneBottomL.style.height = this._paneBottomH + "px";
          this._paneBottomL.style.top = paneBottomTop + "px";
        }
      }
      if (this._hasFrozenRows) {
        this._viewportBottomL.style.height = this._paneBottomH + "px";
        if (this._options.frozenBottom) {
          this._canvasBottomL.style.height = this._frozenRowsHeight + "px";
          if (this.hasFrozenColumns()) {
            this._canvasBottomR.style.height = this._frozenRowsHeight + "px";
          }
        } else {
          this._canvasTopL.style.height = this._frozenRowsHeight + "px";
          if (this.hasFrozenColumns()) {
            this._canvasTopR.style.height = this._frozenRowsHeight + "px";
          }
        }
      } else {
        this._viewportTopR.style.height = this._viewportTopH + "px";
      }
      if (!this._scrollDims || !this._scrollDims.width) {
        this._scrollDims = getScrollBarDimensions(true);
      }
      if (this._options.forceFitColumns) {
        this.autosizeColumns();
      }
      this.updateRowCount();
      this.handleScroll();
      this._scrolLLeftRendered = -1;
      this.render();
    }
    updatePagingStatusFromView(pagingInfo) {
      this._pagingActive = pagingInfo.pageSize !== 0;
      this._pagingIsLastPage = pagingInfo.pageNum == pagingInfo.totalPages - 1;
    }
    updateRowCount() {
      if (!this._initialized) {
        return;
      }
      var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
      var numberOfRows = 0;
      var oldH = this._hasFrozenRows && !this._options.frozenBottom ? Math.round($(this._canvasBottomL).height()) : Math.round($(this._canvasTopL).height());
      if (this._hasFrozenRows) {
        var numberOfRows = this.getDataLength() - this._options.frozenRow;
      } else {
        var numberOfRows = dataLengthIncludingAddNew + (this._options.leaveSpaceForNewRows ? this._numVisibleRows - 1 : 0);
      }
      var tempViewportH = Math.round($(this._scrollContainerY).height());
      var oldViewportHasVScroll = this._viewportHasVScroll;
      this._viewportHasVScroll = !this._options.autoHeight && numberOfRows * this._options.rowHeight > tempViewportH;
      this.makeActiveCellNormal();
      var l = dataLengthIncludingAddNew - 1;
      for (var x in this._rowsCache) {
        var i = parseInt(x, 10);
        if (i >= l) {
          this.removeRowFromCache(i);
        }
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
      this._virtualHeight = Math.max(this._options.rowHeight * numberOfRows, tempViewportH - this._scrollDims.height);
      if (this._activeCellNode && this._activeRow > l) {
        this.resetActiveCell();
      }
      if (this._virtualHeight < getMaxSupportedCssHeight()) {
        this._realScrollHeight = this._pageHeight = this._virtualHeight;
        this._numberOfPages = 1;
        this._jumpinessCoefficient = 0;
      } else {
        this._realScrollHeight = getMaxSupportedCssHeight();
        this._pageHeight = this._realScrollHeight / 100;
        this._numberOfPages = Math.floor(this._virtualHeight / this._pageHeight);
        this._jumpinessCoefficient = (this._virtualHeight - this._realScrollHeight) / (this._numberOfPages - 1);
      }
      if (this._realScrollHeight !== oldH) {
        if (this._hasFrozenRows && !this._options.frozenBottom) {
          this._canvasBottomL.style.height = this._realScrollHeight + "px";
          if (this.hasFrozenColumns()) {
            this._canvasBottomR.style.height = this._realScrollHeight + "px";
          }
        } else {
          this._canvasTopL.style.height = this._realScrollHeight + "px";
          this._canvasTopR.style.height = this._realScrollHeight + "px";
        }
        this._scrollTop = this._scrollContainerY.scrollTop;
      }
      var oldScrollTopInRange = this._scrollTop + this._pageOffset <= this._virtualHeight - tempViewportH;
      if (this._virtualHeight == 0 || this._scrollTop == 0) {
        this._page = this._pageOffset = 0;
      } else if (oldScrollTopInRange) {
        this.scrollTo(this._scrollTop + this._pageOffset);
      } else {
        this.scrollTo(this._virtualHeight - tempViewportH);
      }
      if (this._realScrollHeight != oldH && this._options.autoHeight) {
        this.resizeCanvas();
      }
      if (this._options.forceFitColumns && oldViewportHasVScroll != this._viewportHasVScroll) {
        this.autosizeColumns();
      }
      this.updateCanvasWidth(false);
    }
    getViewport(viewportTop, viewportLeft) {
      return this.getVisibleRange(viewportTop, viewportLeft);
    }
    getVisibleRange(viewportTop, viewportLeft) {
      if (viewportTop == null) {
        viewportTop = this._scrollTop;
      }
      if (viewportLeft == null) {
        viewportLeft = this._scrollLeft;
      }
      return {
        top: this.getRowFromPosition(viewportTop),
        bottom: this.getRowFromPosition(viewportTop + this._viewportH) + 1,
        leftPx: viewportLeft,
        rightPx: viewportLeft + this._viewportW
      };
    }
    getRenderedRange(viewportTop, viewportLeft) {
      var range = this.getVisibleRange(viewportTop, viewportLeft);
      var buffer = Math.round(this._viewportH / this._options.rowHeight);
      var minBuffer = this._options.minBuffer || 3;
      if (this._vScrollDir == -1) {
        range.top -= buffer;
        range.bottom += minBuffer;
      } else if (this._vScrollDir == 1) {
        range.top -= minBuffer;
        range.bottom += buffer;
      } else {
        range.top -= minBuffer;
        range.bottom += minBuffer;
      }
      range.top = Math.max(0, range.top);
      range.bottom = Math.min(this.getDataLengthIncludingAddNew() - 1, range.bottom);
      if (this._options.renderAllCells) {
        range.leftPx = 0;
        range.rightPx = this._canvasWidth;
      } else {
        range.leftPx -= this._viewportW;
        range.rightPx += this._viewportW;
        range.leftPx = Math.max(0, range.leftPx);
        range.rightPx = Math.min(this._canvasWidth, range.rightPx);
      }
      return range;
    }
    ensureCellNodesInRowsCache(row) {
      var cacheEntry = this._rowsCache[row];
      if (cacheEntry) {
        if (cacheEntry.cellRenderQueue.length) {
          var lastChild = cacheEntry.rowNode.children().last()[0];
          while (cacheEntry.cellRenderQueue.length) {
            var columnIdx = cacheEntry.cellRenderQueue.pop();
            cacheEntry.cellNodesByColumnIdx[columnIdx] = lastChild;
            lastChild = lastChild.previousSibling;
            if (lastChild == null)
              lastChild = $(cacheEntry.rowNode[0]).children().last()[0];
          }
        }
      }
    }
    cleanUpCells(range, row) {
      if (this._hasFrozenRows && (this._options.frozenBottom && row > this._actualFrozenRow || row <= this._actualFrozenRow)) {
        return;
      }
      var totalCellsRemoved = 0;
      var cacheEntry = this._rowsCache[row];
      var cellsToRemove = [], frozenCols = this._frozenCols;
      for (var x in cacheEntry.cellNodesByColumnIdx) {
        if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
          continue;
        }
        var i = parseInt(x, 10);
        if (i < frozenCols) {
          continue;
        }
        var colspan = cacheEntry.cellColSpans[i], cols = this._cols;
        if (this._colLeft[i] > range.rightPx || this._colRight[Math.min(cols.length - 1, i + colspan - 1)] < range.leftPx) {
          if (!(row == this._activeRow && i === this._activeCell)) {
            cellsToRemove.push(i);
          }
        }
      }
      var cellToRemove, node;
      this._postProcessGroupId++;
      while ((cellToRemove = cellsToRemove.pop()) != null) {
        node = cacheEntry.cellNodesByColumnIdx[cellToRemove];
        if (this._options.enableAsyncPostRenderCleanup && this._postProcessedRows[row] && this._postProcessedRows[row][cellToRemove]) {
          this.queuePostProcessedCellForCleanup(node, cellToRemove, row);
        } else {
          node.parentElement.removeChild(node);
        }
        delete cacheEntry.cellColSpans[cellToRemove];
        delete cacheEntry.cellNodesByColumnIdx[cellToRemove];
        if (this._postProcessedRows[row]) {
          delete this._postProcessedRows[row][cellToRemove];
        }
        totalCellsRemoved++;
      }
    }
    cleanUpAndRenderCells(range) {
      var cacheEntry;
      var stringArray = [];
      var processedRows = [];
      var cellsAdded;
      var totalCellsAdded = 0;
      var colspan;
      var cols = this._cols;
      for (var row = range.top, btm = range.bottom; row <= btm; row++) {
        cacheEntry = this._rowsCache[row];
        if (!cacheEntry) {
          continue;
        }
        this.ensureCellNodesInRowsCache(row);
        this.cleanUpCells(range, row);
        cellsAdded = 0;
        var metadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
        metadata = metadata && metadata.columns;
        var d = this.getDataItem(row);
        for (var i = 0, ii = cols.length; i < ii; i++) {
          if (this._colLeft[i] > range.rightPx) {
            break;
          }
          if ((colspan = cacheEntry.cellColSpans[i]) != null) {
            i += colspan > 1 ? colspan - 1 : 0;
            continue;
          }
          var columnData = null;
          colspan = 1;
          if (metadata) {
            columnData = metadata[cols[i].id] || metadata[i];
            colspan = columnData && columnData.colspan || 1;
            if (colspan === "*") {
              colspan = ii - i;
            }
          }
          if (this._colRight[Math.min(ii - 1, i + colspan - 1)] > range.leftPx) {
            this.appendCellHtml(stringArray, row, i, colspan, d, columnData);
            cellsAdded++;
          }
          i += colspan > 1 ? colspan - 1 : 0;
        }
        if (cellsAdded) {
          totalCellsAdded += cellsAdded;
          processedRows.push(row);
        }
      }
      if (!stringArray.length) {
        return;
      }
      var x = document.createElement("div");
      x.innerHTML = stringArray.join("");
      var processedRow;
      var node, frozenCols = this._frozenCols;
      while ((processedRow = processedRows.pop()) != null) {
        cacheEntry = this._rowsCache[processedRow];
        var columnIdx;
        while ((columnIdx = cacheEntry.cellRenderQueue.pop()) != null) {
          node = x.lastChild;
          if (frozenCols > 0 && columnIdx >= frozenCols) {
            cacheEntry.rowNode[1].appendChild(node);
          } else {
            cacheEntry.rowNode[0].appendChild(node);
          }
          cacheEntry.cellNodesByColumnIdx[columnIdx] = node;
        }
      }
    }
    renderRows(range) {
      var stringArrayL = [], stringArrayR = [], rows = [], needToReselectCell = false, dataLength = this.getDataLength();
      for (var i = range.top, ii = range.bottom; i <= ii; i++) {
        if (this._rowsCache[i] || this._hasFrozenRows && this._options.frozenBottom && i == dataLength) {
          continue;
        }
        rows.push(i);
        this._rowsCache[i] = {
          rowNode: null,
          cellColSpans: [],
          cellNodesByColumnIdx: [],
          cellRenderQueue: []
        };
        this.appendRowHtml(stringArrayL, stringArrayR, i, range, dataLength);
        if (this._activeCellNode && this._activeRow === i) {
          needToReselectCell = true;
        }
      }
      if (!rows.length) {
        return;
      }
      var x = document.createElement("div"), right = document.createElement("div");
      x.innerHTML = stringArrayL.join("");
      right.innerHTML = stringArrayR.join("");
      for (var i = 0, ii = rows.length; i < ii; i++) {
        if (this._hasFrozenRows && rows[i] >= this._actualFrozenRow) {
          if (this.hasFrozenColumns()) {
            this._rowsCache[rows[i]].rowNode = $().add($(x.firstChild).appendTo(this._canvasBottomL)).add($(right.firstChild).appendTo(this._canvasBottomR));
          } else {
            this._rowsCache[rows[i]].rowNode = $().add($(x.firstChild).appendTo(this._canvasBottomL));
          }
        } else if (this.hasFrozenColumns()) {
          this._rowsCache[rows[i]].rowNode = $().add($(x.firstChild).appendTo(this._canvasTopL)).add($(right.firstChild).appendTo(this._canvasTopR));
        } else {
          this._rowsCache[rows[i]].rowNode = $().add($(x.firstChild).appendTo(this._canvasTopL));
        }
      }
      if (needToReselectCell) {
        this._activeCellNode = this.getCellNode(this._activeRow, this._activeCell);
      }
    }
    startPostProcessing() {
      if (!this._options.enableAsyncPostRender) {
        return;
      }
      clearTimeout(this._hPostRender);
      if (this._options.asyncPostRenderDelay < 0) {
        this.asyncPostProcessRows();
      } else {
        this._hPostRender = setTimeout(this.asyncPostProcessRows.bind(this), this._options.asyncPostRenderDelay);
      }
    }
    startPostProcessingCleanup() {
      if (!this._options.enableAsyncPostRenderCleanup) {
        return;
      }
      clearTimeout(this._hPostRenderCleanup);
      if (this._options.asyncPostCleanupDelay < 0) {
        this.asyncPostProcessCleanupRows();
      } else {
        this._hPostRenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
      }
    }
    invalidatePostProcessingResults(row) {
      if (this._options.enableAsyncPostRenderCleanup) {
        for (var columnIdx in this._postProcessedRows[row]) {
          if (this._postProcessedRows[row].hasOwnProperty(columnIdx)) {
            this._postProcessedRows[row][columnIdx] = "C";
          }
        }
      } else {
        delete this._postProcessedRows[row];
      }
      this._postProcessFromRow = Math.min(this._postProcessFromRow, row);
      this._postProcessToRow = Math.max(this._postProcessToRow, row);
      this.startPostProcessing();
    }
    updateRowPositions() {
      for (var row in this._rowsCache) {
        this._rowsCache[row].rowNode.css("top", this.getRowTop(parseInt(row, 10)) + "px");
      }
    }
    updateFooterTotals() {
      if (!this._options.showFooterRow || !this._initialized)
        return;
      var totals = null;
      if (this._data.getGrandTotals) {
        totals = this._data.getGrandTotals();
      }
      var cols = this._cols;
      for (var i = 0; i < cols.length; i++) {
        var m = cols[i];
        var content;
        if (m.field && totals) {
          content = m.groupTotalsFormatter && m.groupTotalsFormatter(totals, m, this) || this.groupTotalsFormatter && this.groupTotalsFormatter(totals, m) || "";
        }
        $(this.getFooterRowColumn(m.id)).html(content);
      }
    }
    render() {
      if (!this._initialized) {
        return;
      }
      var visible = this.getVisibleRange();
      var rendered = this.getRenderedRange();
      this.cleanupRows(rendered);
      if (this._scrolLLeftRendered != this._scrollLeft) {
        if (this._hasFrozenRows) {
          var renderedFrozenRows = jQuery.extend(true, {}, rendered);
          if (this._options.frozenBottom) {
            renderedFrozenRows.top = this._actualFrozenRow;
            renderedFrozenRows.bottom = this.getDataLength();
          } else {
            renderedFrozenRows.top = 0;
            renderedFrozenRows.bottom = this._options.frozenRow;
          }
          this.cleanUpAndRenderCells(renderedFrozenRows);
        }
        this.cleanUpAndRenderCells(rendered);
      }
      this.renderRows(rendered);
      if (this._hasFrozenRows) {
        if (this._options.frozenBottom) {
          this.renderRows({
            top: this._actualFrozenRow,
            bottom: this.getDataLength() - 1,
            leftPx: rendered.leftPx,
            rightPx: rendered.rightPx
          });
        } else {
          this.renderRows({
            top: 0,
            bottom: this._options.frozenRow - 1,
            leftPx: rendered.leftPx,
            rightPx: rendered.rightPx
          });
        }
      }
      this._postProcessFromRow = visible.top;
      this._postProcessToRow = Math.min(this.getDataLengthIncludingAddNew() - 1, visible.bottom);
      this.startPostProcessing();
      this._scrollTopRendered = this._scrollTop;
      this._scrolLLeftRendered = this._scrollLeft;
      this._hRender = null;
    }
    handleMouseWheel(e, delta, deltaX, deltaY) {
      deltaX = (typeof deltaX == "undefined" ? e.originalEvent.deltaX : deltaX) || 0;
      deltaY = (typeof deltaY == "undefined" ? e.originalEvent.deltaY : deltaY) || 0;
      this._scrollTop = Math.max(0, this._scrollContainerY.scrollTop - deltaY * this._options.rowHeight);
      this._scrollLeft = this._scrollContainerX.scrollLeft + deltaX * 10;
      var handled = this._handleScroll(true);
      if (handled)
        e.preventDefault();
    }
    handleScroll() {
      this._scrollTop = this._scrollContainerY.scrollTop;
      this._scrollLeft = this._scrollContainerX.scrollLeft;
      return this._handleScroll(false);
    }
    _handleScroll(isMouseWheel) {
      var maxScrollDistanceY = this._scrollContainerY.scrollHeight - this._scrollContainerY.clientHeight;
      var maxScrollDistanceX = this._scrollContainerY.scrollWidth - this._scrollContainerY.clientWidth;
      maxScrollDistanceY = Math.max(0, maxScrollDistanceY);
      maxScrollDistanceX = Math.max(0, maxScrollDistanceX);
      if (this._scrollTop > maxScrollDistanceY) {
        this._scrollTop = maxScrollDistanceY;
      }
      if (this._scrollLeft > maxScrollDistanceX) {
        this._scrollLeft = maxScrollDistanceX;
      }
      var vScrollDist = Math.abs(this._scrollTop - this._scrollTopPrev);
      var hScrollDist = Math.abs(this._scrollLeft - this._scrollLeftPrev);
      if (hScrollDist || vScrollDist)
        this._ignoreScrollUntil = new Date().getTime() + 100;
      if (hScrollDist) {
        this._scrollLeftPrev = this._scrollLeft;
        this._scrollContainerX.scrollLeft = this._scrollLeft;
        if (this.hasFrozenColumns()) {
          this._headerColsR.parentElement.scrollLeft = this._scrollLeft;
          this._topPanelR.parentElement.scrollLeft = this._scrollLeft;
          this._headerRowColsR.parentElement.scrollLeft = this._scrollLeft;
          this._footerRowR.parentElement.scrollLeft = this._scrollLeft;
          if (this._hasFrozenRows) {
            this._viewportTopR.scrollLeft = this._scrollLeft;
          }
        } else {
          this._headerColsL.parentElement.scrollLeft = this._scrollLeft;
          this._topPanelL.parentElement.scrollLeft = this._scrollLeft;
          this._headerRowColsL.parentElement.scrollLeft = this._scrollLeft;
          this._footerRowColsL.parentElement.scrollLeft = this._scrollLeft;
          if (this._hasFrozenRows) {
            this._viewportTopL.scrollLeft = this._scrollLeft;
          }
        }
      }
      if (vScrollDist) {
        this._vScrollDir = this._scrollTopPrev < this._scrollTop ? 1 : -1;
        this._scrollTopPrev = this._scrollTop;
        if (isMouseWheel) {
          this._scrollContainerY.scrollTop = this._scrollTop;
        }
        if (this.hasFrozenColumns()) {
          if (this._hasFrozenRows && !this._options.frozenBottom) {
            this._viewportBottomL.scrollTop = this._scrollTop;
          } else {
            this._viewportTopL.scrollTop = this._scrollTop;
          }
        }
        if (vScrollDist < this._viewportH) {
          this.scrollTo(this._scrollTop + this._pageOffset);
        } else {
          var oldOffset = this._pageOffset;
          if (this._realScrollHeight == this._viewportH) {
            this._page = 0;
          } else {
            this._page = Math.min(this._numberOfPages - 1, Math.floor(this._scrollTop * ((this._virtualHeight - this._viewportH) / (this._realScrollHeight - this._viewportH)) * (1 / this._pageHeight)));
          }
          this._pageOffset = Math.round(this._page * this._jumpinessCoefficient);
          if (oldOffset != this._pageOffset) {
            this.invalidateAllRows();
          }
        }
      }
      if (hScrollDist || vScrollDist) {
        if (this._hRender) {
          clearTimeout(this._hRender);
        }
        if (Math.abs(this._scrollTopRendered - this._scrollTop) > 20 || Math.abs(this._scrolLLeftRendered - this._scrollLeft) > 20) {
          if (this._options.forceSyncScrolling || Math.abs(this._scrollTopRendered - this._scrollTop) < this._viewportH && Math.abs(this._scrolLLeftRendered - this._scrollLeft) < this._viewportW) {
            this.render();
          } else {
            this._hRender = setTimeout(this.render.bind(this), 50);
          }
          this.trigger(this.onViewportChanged);
        }
      }
      this.trigger(this.onScroll, { scrollLeft: this._scrollLeft, scrollTop: this._scrollTop });
      return !!(hScrollDist || vScrollDist);
    }
    asyncPostProcessRows() {
      var dataLength = this.getDataLength();
      var cols = this._cols;
      while (this._postProcessFromRow <= this._postProcessToRow) {
        var row = this._vScrollDir >= 0 ? this._postProcessFromRow++ : this._postProcessToRow--;
        var cacheEntry = this._rowsCache[row];
        if (!cacheEntry || row >= dataLength) {
          continue;
        }
        if (!this._postProcessedRows[row]) {
          this._postProcessedRows[row] = {};
        }
        this.ensureCellNodesInRowsCache(row);
        for (var x in cacheEntry.cellNodesByColumnIdx) {
          if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
            continue;
          }
          var columnIdx = parseInt(x, 10);
          var m = cols[columnIdx];
          var processedStatus = this._postProcessedRows[row][columnIdx];
          if (processedStatus !== "R") {
            if (m.asyncPostRender || m.asyncPostRenderCleanup) {
              var node = cacheEntry.cellNodesByColumnIdx[columnIdx];
              if (node) {
                m.asyncPostRender && m.asyncPostRender(node, row, this.getDataItem(row), m, processedStatus === "C");
              }
            }
            this._postProcessedRows[row][columnIdx] = "R";
          }
        }
        if (this._options.asyncPostRenderDelay >= 0) {
          this._hPostRender = setTimeout(this.asyncPostProcessRows.bind(this), this._options.asyncPostRenderDelay);
          return;
        }
      }
    }
    asyncPostProcessCleanupRows() {
      var cols = this._cols;
      while (this._postProcessCleanupQueue.length > 0) {
        var groupId = this._postProcessCleanupQueue[0].groupId;
        while (this._postProcessCleanupQueue.length > 0 && this._postProcessCleanupQueue[0].groupId == groupId) {
          var entry = this._postProcessCleanupQueue.shift();
          if (entry.rowNode != null) {
            $(entry.rowNode).remove();
          }
          if (entry.cellNode != null) {
            var column = cols[entry.columnIdx];
            if (column && column.asyncPostRenderCleanup) {
              column.asyncPostRenderCleanup(entry.cellNode, entry.rowIdx, column);
              entry.cellNode.remove();
            }
          }
        }
        if (this._options.asyncPostRenderDelay >= 0) {
          this._hPostRenderCleanup = setTimeout(this.asyncPostProcessCleanupRows.bind(this), this._options.asyncPostCleanupDelay);
          return;
        }
      }
    }
    updateCellCssStylesOnRenderedRows(addedHash, removedHash) {
      var node, columnId, addedRowHash, removedRowHash;
      for (var row in this._rowsCache) {
        removedRowHash = removedHash && removedHash[row];
        addedRowHash = addedHash && addedHash[row];
        if (removedRowHash) {
          for (columnId in removedRowHash) {
            if (!addedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
              node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
              if (node) {
                $(node).removeClass(removedRowHash[columnId]);
              }
            }
          }
        }
        if (addedRowHash) {
          for (columnId in addedRowHash) {
            if (!removedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
              node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
              if (node) {
                $(node).addClass(addedRowHash[columnId]);
              }
            }
          }
        }
      }
    }
    addCellCssStyles(key, hash) {
      if (this._cellCssClasses[key]) {
        throw "addCellCssStyles: cell CSS hash with key '" + key + "' already exists.";
      }
      this._cellCssClasses[key] = hash;
      this.updateCellCssStylesOnRenderedRows(hash, null);
      this.trigger(this.onCellCssStylesChanged, { key, hash });
    }
    removeCellCssStyles(key) {
      if (!this._cellCssClasses[key]) {
        return;
      }
      this.updateCellCssStylesOnRenderedRows(null, this._cellCssClasses[key]);
      delete this._cellCssClasses[key];
      this.trigger(this.onCellCssStylesChanged, { key, hash: null });
    }
    setCellCssStyles(key, hash) {
      var prevHash = this._cellCssClasses[key];
      this._cellCssClasses[key] = hash;
      this.updateCellCssStylesOnRenderedRows(hash, prevHash);
      this.trigger(this.onCellCssStylesChanged, { key, hash });
    }
    getCellCssStyles(key) {
      return this._cellCssClasses[key];
    }
    flashCell(row, cell, speed) {
      speed = speed || 100;
      if (this._rowsCache[row]) {
        var $cell = $(this.getCellNode(row, cell));
        toggleCellClass(4);
      }
      var klass = this._options.cellFlashingCssClass;
      function toggleCellClass(times) {
        if (!times) {
          return;
        }
        setTimeout(function() {
          $cell.queue(function() {
            $cell.toggleClass(klass).dequeue();
            toggleCellClass(times - 1);
          });
        }, speed);
      }
    }
    handleDragInit(e, dd) {
      var cell = this.getCellFromEvent(e);
      if (!cell || !this.cellExists(cell.row, cell.cell)) {
        return false;
      }
      var retval = this.trigger(this.onDragInit, dd, e);
      if (e.isImmediatePropagationStopped()) {
        return retval;
      }
      return false;
    }
    handleDragStart(e, dd) {
      var cell = this.getCellFromEvent(e);
      if (!cell || !this.cellExists(cell.row, cell.cell)) {
        return false;
      }
      var retval = this.trigger(this.onDragStart, dd, e);
      if (e.isImmediatePropagationStopped()) {
        return retval;
      }
      return false;
    }
    handleDrag(e, dd) {
      return this.trigger(this.onDrag, dd, e);
    }
    handleDragEnd(e, dd) {
      this.trigger(this.onDragEnd, dd, e);
    }
    handleKeyDown(e) {
      this.trigger(this.onKeyDown, { row: this._activeRow, cell: this._activeCell }, e);
      var handled = e.isImmediatePropagationStopped();
      if (!handled) {
        if (!e.shiftKey && !e.altKey) {
          if (this._options.editable && this._currentEditor && this._currentEditor.keyCaptureList) {
            if (this._currentEditor.keyCaptureList.indexOf(e.which) >= 0) {
              return;
            }
          }
          if (e.which == keyCode.HOME) {
            if (e.ctrlKey) {
              this.navigateTop();
              handled = true;
            } else
              handled = this.navigateRowStart();
          } else if (e.which == keyCode.END) {
            if (e.ctrlKey) {
              this.navigateBottom();
              handled = true;
            } else
              handled = this.navigateRowEnd();
          }
        }
      }
      if (!handled) {
        if (!e.shiftKey && !e.altKey && !e.ctrlKey) {
          if (this._options.editable && this._currentEditor && this._currentEditor.keyCaptureList) {
            if (this._currentEditor.keyCaptureList.indexOf(e.which) >= 0) {
              return;
            }
          }
          if (e.which == keyCode.ESCAPE) {
            if (!this.getEditorLock().isActive()) {
              return;
            }
            this.cancelEditAndSetFocus();
          } else if (e.which == keyCode.PAGEDOWN) {
            this.navigatePageDown();
            handled = true;
          } else if (e.which == keyCode.PAGEUP) {
            this.navigatePageUp();
            handled = true;
          } else if (e.which == keyCode.LEFT) {
            handled = this.navigateLeft();
          } else if (e.which == keyCode.RIGHT) {
            handled = this.navigateRight();
          } else if (e.which == keyCode.UP) {
            handled = this.navigateUp();
          } else if (e.which == keyCode.DOWN) {
            handled = this.navigateDown();
          } else if (e.which == keyCode.TAB) {
            if (this._options.enableTabKeyNavigation)
              handled = this.navigateNext();
          } else if (e.which == keyCode.ENTER) {
            if (this._options.editable) {
              if (this._currentEditor) {
                if (this._activeRow === this.getDataLength()) {
                  this.navigateDown();
                } else {
                  this.commitEditAndSetFocus();
                }
              } else {
                if (this.getEditorLock().commitCurrentEdit()) {
                  this.makeActiveCellEditable();
                }
              }
            }
            handled = true;
          }
        } else if (e.which == keyCode.TAB && e.shiftKey && !e.ctrlKey && !e.altKey) {
          handled = this.navigatePrev();
        }
      }
      if (handled) {
        e.stopPropagation();
        e.preventDefault();
        try {
          e.originalEvent.keyCode = 0;
        } catch (error) {
        }
      }
    }
    handleClick(e) {
      if (!this._currentEditor) {
        if (e.target != document.activeElement || $(e.target).hasClass("slick-cell")) {
          this.setFocus();
        }
      }
      var cell = this.getCellFromEvent(e);
      if (!cell || this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell) {
        return;
      }
      this.trigger(this.onClick, { row: cell.row, cell: cell.cell }, e);
      if (e.isImmediatePropagationStopped()) {
        return;
      }
      if (this.canCellBeActive(cell.row, cell.cell)) {
        if (!this.getEditorLock().isActive() || this.getEditorLock().commitCurrentEdit()) {
          var preClickModeOn = e.target && e.target.classList.contains(preClickClassName);
          var column = this._cols[cell.cell];
          var suppressActiveCellChangedEvent = !!(this._options.editable && column && column.editor && this._options.suppressActiveCellChangeOnEdit);
          this.setActiveCellInternal(this.getCellNode(cell.row, cell.cell), null, preClickModeOn, suppressActiveCellChangedEvent, e);
        }
      }
    }
    handleContextMenu(e) {
      var $cell = $(e.target).closest(".slick-cell", this._container);
      if ($cell.length === 0) {
        return;
      }
      if (this._activeCellNode === $cell[0] && this._currentEditor != null) {
        return;
      }
      this.trigger(this.onContextMenu, {}, e);
    }
    handleDblClick(e) {
      var cell = this.getCellFromEvent(e);
      if (!cell || this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell) {
        return;
      }
      this.trigger(this.onDblClick, { row: cell.row, cell: cell.cell }, e);
      if (e.isImmediatePropagationStopped()) {
        return;
      }
      if (this._options.editable) {
        this.gotoCell(cell.row, cell.cell, true);
      }
    }
    handleHeaderMouseEnter(e) {
      this.trigger(this.onHeaderMouseEnter, {
        column: $(e.target).data("column")
      }, e);
    }
    handleHeaderMouseLeave(e) {
      this.trigger(this.onHeaderMouseLeave, {
        column: $(e.target).data("column")
      }, e);
    }
    handleHeaderContextMenu(e) {
      var $header = $(e.target).closest(".slick-header-column", ".slick-header-columns");
      var column = $header && $header.data("column");
      this.trigger(this.onHeaderContextMenu, { column }, e);
    }
    handleHeaderClick(e) {
      var $header = $(e.target).closest(".slick-header-column", ".slick-header-columns");
      var column = $header && $header.data("column");
      if (column) {
        this.trigger(this.onHeaderClick, { column }, e);
      }
    }
    handleMouseEnter(e) {
      this.trigger(this.onMouseEnter, {}, e);
    }
    handleMouseLeave(e) {
      this.trigger(this.onMouseLeave, {}, e);
    }
    cellExists(row, cell) {
      return !(row < 0 || row >= this.getDataLength() || cell < 0 || cell >= this._cols.length);
    }
    getCellFromPoint(x, y) {
      var row = this.getRowFromPosition(y);
      var cell = 0;
      var cols = this._cols;
      var w = 0;
      for (var i = 0; i < cols.length && w < x; i++) {
        w += cols[i].width;
        cell++;
      }
      if (cell < 0) {
        cell = 0;
      }
      return { row, cell: cell - 1 };
    }
    getCellFromNode(cellNode) {
      var cls = /l(\d+)/.exec(cellNode.className);
      if (!cls) {
        throw "getCellFromNode: cannot get cell - " + cellNode.className;
      }
      return parseInt(cls[1], 10);
    }
    getRowFromNode(rowNode) {
      for (var row in this._rowsCache)
        for (var i in this._rowsCache[row].rowNode)
          if (this._rowsCache[row].rowNode[i] === rowNode)
            return parseInt(row, 10);
      return null;
    }
    getFrozenRowOffset(row) {
      var offset = this._hasFrozenRows ? this._options.frozenBottom ? row >= this._actualFrozenRow ? this._realScrollHeight < this._viewportTopH ? this._actualFrozenRow * this._options.rowHeight : this._realScrollHeight : 0 : row >= this._actualFrozenRow ? this._frozenRowsHeight : 0 : 0;
      return offset;
    }
    getCellFromEvent(e) {
      var row, cell;
      var $cell = $(e.target).closest(".slick-cell", this._container);
      if (!$cell.length) {
        return null;
      }
      row = this.getRowFromNode($cell[0].parentNode);
      if (this._hasFrozenRows) {
        var c = $cell.parents(".grid-canvas").offset();
        var rowOffset = 0;
        var isBottom = $cell.parents(".grid-canvas-bottom").length;
        if (isBottom) {
          rowOffset = this._options.frozenBottom ? Math.round($(this._canvasTopL).height()) : this._frozenRowsHeight;
        }
        row = this.getCellFromPoint(e.clientX - c[this._rtlS], e.clientY - c.top + rowOffset + $(document).scrollTop()).row;
      }
      cell = this.getCellFromNode($cell[0]);
      if (row == null || cell == null) {
        return null;
      } else {
        return {
          "row": row,
          "cell": cell
        };
      }
    }
    getCellNodeBox(row, cell) {
      if (!this.cellExists(row, cell)) {
        return null;
      }
      var frozenRowOffset = this.getFrozenRowOffset(row);
      var cols = this._cols, frozenCols = this._frozenCols;
      var y1 = this.getRowTop(row) - frozenRowOffset;
      var y2 = y1 + this._options.rowHeight - 1;
      var x1 = 0;
      for (var i = 0; i < cell; i++) {
        x1 += cols[i].width;
        if (i == frozenCols - 1) {
          x1 = 0;
        }
      }
      var x2 = x1 + cols[cell].width;
      return this._rtl ? {
        top: y1,
        right: x1,
        bottom: y2,
        left: x2
      } : {
        top: y1,
        left: x1,
        bottom: y2,
        right: x2
      };
    }
    resetActiveCell() {
      this.setActiveCellInternal(null, false);
    }
    focus() {
      this.setFocus();
    }
    setFocus() {
      if (this._tabbingDirection == -1) {
        this._focusSink1.focus();
      } else {
        this._focusSink2.focus();
      }
    }
    scrollCellIntoView(row, cell, doPaging) {
      this.scrollRowIntoView(row, doPaging);
      if (cell < this._frozenCols)
        return;
      var colspan = this.getColspan(row, cell);
      this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell + (colspan > 1 ? colspan - 1 : 0)]);
    }
    scrollColumnIntoView(cell) {
      this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell]);
    }
    internalScrollColumnIntoView(left, right) {
      var scrollRight = this._scrollLeft + $(this._scrollContainerX).width() - (this._viewportHasVScroll ? this._scrollDims.width : 0);
      var target;
      if (left < this._scrollLeft)
        target = left;
      else if (right > scrollRight)
        target = Math.min(left, right - this._scrollContainerX.clientWidth);
      else
        return;
      this._scrollContainerX.scrollLeft = target;
      this.handleScroll();
      this.render();
    }
    setActiveCellInternal(newCell, opt_editMode, preClickModeOn, suppressActiveCellChangedEvent, e) {
      if (this._activeCellNode != null) {
        this.makeActiveCellNormal();
        $(this._activeCellNode).removeClass("active");
        if (this._rowsCache[this._activeRow]) {
          $(this._rowsCache[this._activeRow].rowNode).removeClass("active");
        }
      }
      var activeCellChanged = this._activeCellNode !== newCell;
      this._activeCellNode = newCell;
      if (this._activeCellNode != null) {
        var $activeCellNode = $(this._activeCellNode);
        var $activeCellOffset = $activeCellNode.offset();
        var rowOffset = Math.floor($activeCellNode.parents(".grid-canvas").offset().top);
        var isBottom = $activeCellNode.parents(".grid-canvas-bottom").length;
        if (this._hasFrozenRows && isBottom) {
          rowOffset -= this._options.frozenBottom ? Math.round($(this._canvasTopL).height()) : this._frozenRowsHeight;
        }
        var cell = this.getCellFromPoint($activeCellOffset[this._rtlS], Math.ceil($activeCellOffset.top) - rowOffset);
        this._activeRow = cell.row;
        this._activeCell = this._activePosX = this.getCellFromNode(this._activeCellNode);
        if (this._options.showCellSelection) {
          $activeCellNode.addClass("active");
          if (this._rowsCache[this._activeRow]) {
            $(this._rowsCache[this._activeRow].rowNode).addClass("active");
          }
        }
        if (opt_editMode == null) {
          opt_editMode = this._activeRow == this.getDataLength() || this._options.autoEdit;
        }
        if (this._options.editable && opt_editMode && this.isCellPotentiallyEditable(this._activeRow, this._activeCell)) {
          clearTimeout(this._hEditorLoader);
          if (this._options.asyncEditorLoading) {
            this._hEditorLoader = setTimeout(() => {
              this.makeActiveCellEditable(void 0, preClickModeOn, e);
            }, this._options.asyncEditorLoadDelay);
          } else {
            this.makeActiveCellEditable(void 0, preClickModeOn, e);
          }
        }
      } else {
        this._activeRow = this._activeCell = null;
      }
      if (!suppressActiveCellChangedEvent) {
        this.trigger(this.onActiveCellChanged, this.getActiveCell());
      }
    }
    clearTextSelection() {
      if (document.selection && document.selection.empty) {
        try {
          document.selection.empty();
        } catch (e) {
        }
      } else if (window.getSelection) {
        var sel = window.getSelection();
        if (sel && sel.removeAllRanges) {
          sel.removeAllRanges();
        }
      }
    }
    isCellPotentiallyEditable(row, cell) {
      var dataLength = this.getDataLength();
      if (row < dataLength && !this.getDataItem(row)) {
        return false;
      }
      if (this._cols[cell].cannotTriggerInsert && row >= dataLength) {
        return false;
      }
      if (!this.getEditor(row, cell)) {
        return false;
      }
      return true;
    }
    makeActiveCellNormal() {
      if (!this._currentEditor) {
        return;
      }
      this.trigger(this.onBeforeCellEditorDestroy, { editor: this._currentEditor });
      this._currentEditor.destroy();
      this._currentEditor = null;
      if (this._activeCellNode) {
        var d = this.getDataItem(this._activeRow);
        $(this._activeCellNode).removeClass("editable invalid");
        if (d) {
          var column = this._cols[this._activeCell];
          var fmtResult = d ? this.getFormatter(this._activeRow, column)(this._activeRow, this._activeCell, this.getDataItemValueForColumn(d, column), column, d) : "";
          this.applyFormatResultToCellNode(fmtResult, this._activeCellNode);
          this.invalidatePostProcessingResults(this._activeRow);
        }
      }
      if (navigator.userAgent.toLowerCase().match(/msie/)) {
        this.clearTextSelection();
      }
      this.getEditorLock().deactivate(this._editController);
    }
    editActiveCell(editor) {
      this.makeActiveCellEditable(editor);
    }
    makeActiveCellEditable(editor, preClickModeOn, e) {
      if (!this._activeCellNode) {
        return;
      }
      if (!this._options.editable) {
        throw "Grid : makeActiveCellEditable : should never get called when options.editable is false";
      }
      clearTimeout(this._hEditorLoader);
      if (!this.isCellPotentiallyEditable(this._activeRow, this._activeCell)) {
        return;
      }
      var columnDef = this._cols[this._activeCell];
      var item = this.getDataItem(this._activeRow);
      if (this.trigger(this.onBeforeEditCell, { row: this._activeRow, cell: this._activeCell, item, column: columnDef }) === false) {
        this.setFocus();
        return;
      }
      this.getEditorLock().activate(this._editController);
      $(this._activeCellNode).addClass("editable");
      var useEditor = editor || this.getEditor(this._activeRow, this._activeCell);
      if (!editor && !useEditor.suppressClearOnEdit) {
        this._activeCellNode.innerHTML = "";
      }
      var metadata = this._data.getItemMetadata && this._data.getItemMetadata(this._activeRow);
      metadata = metadata && metadata.columns;
      var columnMetaData = metadata && (metadata[columnDef.id] || metadata[this._activeCell]);
      this._currentEditor = new useEditor({
        grid: this,
        gridPosition: this.absBox(this._container),
        position: this.absBox(this._activeCellNode),
        container: this._activeCellNode,
        column: columnDef,
        item: item || {},
        event: e,
        commitChanges: this.commitEditAndSetFocus,
        cancelChanges: this.cancelEditAndSetFocus
      });
      if (item) {
        this._currentEditor.loadValue(item);
        if (preClickModeOn && this._currentEditor.preClick) {
          this._currentEditor.preClick();
        }
      }
      this._serializedEditorValue = this._currentEditor.serializeValue();
      if (this._currentEditor.position) {
        this.handleActiveCellPositionChange();
      }
    }
    commitEditAndSetFocus() {
      if (this.getEditorLock().commitCurrentEdit()) {
        this.setFocus();
        if (this._options.autoEdit) {
          this.navigateDown();
        }
      }
    }
    cancelEditAndSetFocus() {
      if (this.getEditorLock().cancelCurrentEdit()) {
        this.setFocus();
      }
    }
    absBox(elem) {
      var box = {
        top: elem.offsetTop,
        bottom: 0,
        width: $(elem).outerWidth(),
        height: $(elem).outerHeight(),
        visible: true
      };
      box[this._rtlS] = elem.offsetLeft;
      box[this._rtlE] = 0;
      box.bottom = box.top + box.height;
      box[this._rtlE] = box[this._rtlS] + box.width;
      var offsetParent = elem.offsetParent;
      while ((elem = elem.parentNode) != document.body) {
        if (box.visible && elem.scrollHeight != elem.offsetHeight && $(elem).css("overflowY") != "visible") {
          box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
        }
        if (box.visible && elem.scrollWidth != elem.offsetWidth && $(elem).css("overflowX") != "visible") {
          box.visible = box[this._rtlE] > elem.scrollLeft && box[this._rtlS] < elem.scrollLeft + elem.clientWidth;
        }
        box[this._rtlS] -= elem.scrollLeft;
        box.top -= elem.scrollTop;
        if (elem === offsetParent) {
          box.right += elem.offsetLeft;
          box.top += elem.offsetTop;
          offsetParent = elem.offsetParent;
        }
        box.bottom = box.top + box.height;
        box[this._rtlE] = box[this._rtlS] + box.width;
      }
      return box;
    }
    getActiveCellPosition() {
      return this.absBox(this._activeCellNode);
    }
    getGridPosition() {
      return this.absBox(this._container);
    }
    getCellEditor() {
      return this._currentEditor;
    }
    getActiveCell() {
      if (!this._activeCellNode) {
        return null;
      } else {
        return { row: this._activeRow, cell: this._activeCell };
      }
    }
    getActiveCellNode() {
      return this._activeCellNode;
    }
    scrollActiveCellIntoView() {
      if (this._activeRow != null && this._activeCell != null) {
        this.scrollCellIntoView(this._activeRow, this._activeCell);
      }
    }
    scrollRowIntoView(row, doPaging) {
      if (!this._hasFrozenRows || !this._options.frozenBottom && row > this._actualFrozenRow - 1 || this._options.frozenBottom && row < this._actualFrozenRow - 1) {
        var viewportScrollH = Math.round($(this._scrollContainerY).height());
        var rowNumber = this._hasFrozenRows && !this._options.frozenBottom ? row - this._options.frozenRow : row;
        var rowAtTop = rowNumber * this._options.rowHeight;
        var rowAtBottom = (rowNumber + 1) * this._options.rowHeight - viewportScrollH + (this._viewportHasHScroll ? this._scrollDims.height : 0);
        if ((rowNumber + 1) * this._options.rowHeight > this._scrollTop + viewportScrollH + this._pageOffset) {
          this.scrollTo(doPaging ? rowAtTop : rowAtBottom);
          this.render();
        } else if (rowNumber * this._options.rowHeight < this._scrollTop + this._pageOffset) {
          this.scrollTo(doPaging ? rowAtBottom : rowAtTop);
          this.render();
        }
      }
    }
    scrollRowToTop(row) {
      this.scrollTo(row * this._options.rowHeight);
      this.render();
    }
    scrollPage(dir) {
      var deltaRows = dir * this._numVisibleRows;
      this.scrollTo((this.getRowFromPosition(this._scrollTop) + deltaRows) * this._options.rowHeight);
      this.render();
      if (this._options.enableCellNavigation && this._activeRow != null) {
        var row = this._activeRow + deltaRows;
        var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
        if (row >= dataLengthIncludingAddNew) {
          row = dataLengthIncludingAddNew - 1;
        }
        if (row < 0) {
          row = 0;
        }
        var cell = 0, prevCell = null;
        var prevActivePosX = this._activePosX;
        while (cell <= this._activePosX) {
          if (this.canCellBeActive(row, cell)) {
            prevCell = cell;
          }
          cell += this.getColspan(row, cell);
        }
        if (prevCell != null) {
          this.setActiveCellInternal(this.getCellNode(row, prevCell));
          this._activePosX = prevActivePosX;
        } else {
          this.resetActiveCell();
        }
      }
    }
    navigatePageDown() {
      this.scrollPage(1);
    }
    navigatePageUp() {
      this.scrollPage(-1);
    }
    navigateTop() {
      this.navigateToRow(0);
    }
    navigateBottom() {
      this.navigateToRow(this.getDataLength() - 1);
    }
    navigateToRow(row) {
      var dataLength = this.getDataLength();
      if (!dataLength) {
        return true;
      }
      if (row < 0)
        row = 0;
      else if (row >= dataLength)
        row = dataLength - 1;
      this.scrollCellIntoView(row, 0, true);
      if (this._options.enableCellNavigation && this._activeRow != null) {
        var cell = 0, prevCell = null;
        var prevActivePosX = this._activePosX;
        while (cell <= this._activePosX) {
          if (this.canCellBeActive(row, cell))
            prevCell = cell;
          cell += this.getColspan(row, cell);
        }
        if (prevCell != null) {
          this.setActiveCellInternal(this.getCellNode(row, prevCell));
          this._activePosX = prevActivePosX;
        } else
          this.resetActiveCell();
      }
      return true;
    }
    getColspan(row, cell) {
      var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      if (!itemMetadata || !itemMetadata.columns) {
        return 1;
      }
      var cols = this._cols;
      var columnData = cols[cell] && (itemMetadata.columns[cols[cell].id] || itemMetadata.columns[cell]);
      var colspan = columnData && columnData.colspan;
      if (colspan === "*") {
        colspan = cols.length - cell;
      } else {
        colspan = colspan || 1;
      }
      return colspan;
    }
    findFirstFocusableCell(row) {
      var cell = 0;
      var cols = this._cols;
      while (cell < cols.length) {
        if (this.canCellBeActive(row, cell)) {
          return cell;
        }
        cell += this.getColspan(row, cell);
      }
      return null;
    }
    findLastFocusableCell(row) {
      var cell = 0;
      var lastFocusableCell = null;
      var cols = this._cols;
      while (cell < cols.length) {
        if (this.canCellBeActive(row, cell)) {
          lastFocusableCell = cell;
        }
        cell += this.getColspan(row, cell);
      }
      return lastFocusableCell;
    }
    gotoRight(row, cell, posX) {
      var cols = this._cols;
      if (cell >= cols.length) {
        return null;
      }
      do {
        cell += this.getColspan(row, cell);
      } while (cell < cols.length && !this.canCellBeActive(row, cell));
      if (cell < cols.length) {
        return {
          row,
          cell,
          posX: cell
        };
      }
      return null;
    }
    gotoLeft(row, cell, posX) {
      if (cell <= 0) {
        return null;
      }
      var firstFocusableCell = this.findFirstFocusableCell(row);
      if (firstFocusableCell === null || firstFocusableCell >= cell) {
        return null;
      }
      var prev = {
        row,
        cell: firstFocusableCell,
        posX: firstFocusableCell
      };
      var pos;
      while (true) {
        pos = this.gotoRight(prev.row, prev.cell, prev.posX);
        if (!pos) {
          return null;
        }
        if (pos.cell >= cell) {
          return prev;
        }
        prev = pos;
      }
    }
    gotoDown(row, cell, posX) {
      var prevCell;
      var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
      while (true) {
        if (++row >= dataLengthIncludingAddNew) {
          return null;
        }
        prevCell = cell = 0;
        while (cell <= posX) {
          prevCell = cell;
          cell += this.getColspan(row, cell);
        }
        if (this.canCellBeActive(row, prevCell)) {
          return {
            row,
            cell: prevCell,
            posX
          };
        }
      }
    }
    gotoUp(row, cell, posX) {
      var prevCell;
      while (true) {
        if (--row < 0) {
          return null;
        }
        prevCell = cell = 0;
        while (cell <= posX) {
          prevCell = cell;
          cell += this.getColspan(row, cell);
        }
        if (this.canCellBeActive(row, prevCell)) {
          return {
            row,
            cell: prevCell,
            posX
          };
        }
      }
    }
    gotoNext(row, cell, posX) {
      if (row == null && cell == null) {
        row = cell = posX = 0;
        if (this.canCellBeActive(row, cell)) {
          return {
            row,
            cell,
            posX: cell
          };
        }
      }
      var pos = this.gotoRight(row, cell, posX);
      if (pos) {
        return pos;
      }
      var firstFocusableCell = null;
      var dataLengthIncludingAddNew = this.getDataLengthIncludingAddNew();
      while (++row < dataLengthIncludingAddNew) {
        firstFocusableCell = this.findFirstFocusableCell(row);
        if (firstFocusableCell != null) {
          return {
            row,
            cell: firstFocusableCell,
            posX: firstFocusableCell
          };
        }
      }
      return null;
    }
    gotoPrev(row, cell, posX) {
      var cols = this._cols;
      if (row == null && cell == null) {
        row = this.getDataLengthIncludingAddNew() - 1;
        cell = posX = cols.length - 1;
        if (this.canCellBeActive(row, cell)) {
          return {
            row,
            cell,
            posX: cell
          };
        }
      }
      var pos;
      var lastSelectableCell;
      while (!pos) {
        pos = this.gotoLeft(row, cell, posX);
        if (pos) {
          break;
        }
        if (--row < 0) {
          return null;
        }
        cell = 0;
        lastSelectableCell = this.findLastFocusableCell(row);
        if (lastSelectableCell != null) {
          pos = {
            row,
            cell: lastSelectableCell,
            posX: lastSelectableCell
          };
        }
      }
      return pos;
    }
    gotoRowStart(row) {
      var newCell = this.findFirstFocusableCell(row);
      if (newCell === null)
        return null;
      return {
        row,
        cell: newCell,
        posX: newCell
      };
    }
    gotoRowEnd(row) {
      var newCell = this.findLastFocusableCell(row);
      if (newCell === null)
        return null;
      return {
        row,
        cell: newCell,
        posX: newCell
      };
    }
    navigateRight() {
      return this.navigate("right");
    }
    navigateLeft() {
      return this.navigate("left");
    }
    navigateDown() {
      return this.navigate("down");
    }
    navigateUp() {
      return this.navigate("up");
    }
    navigateNext() {
      return this.navigate("next");
    }
    navigatePrev() {
      return this.navigate("prev");
    }
    navigateRowStart() {
      return this.navigate("home");
    }
    navigateRowEnd() {
      return this.navigate("end");
    }
    navigate(dir) {
      if (!this._options.enableCellNavigation) {
        return false;
      }
      if (!this._activeCellNode && dir != "prev" && dir != "next") {
        return false;
      }
      if (!this.getEditorLock().commitCurrentEdit()) {
        return true;
      }
      this.setFocus();
      var tabbingDirections = {
        up: -1,
        down: 1,
        prev: -1,
        next: 1,
        home: -1,
        end: 1
      };
      tabbingDirections[this._rtlS] = -1;
      tabbingDirections[this._rtlE] = 1;
      this._tabbingDirection = tabbingDirections[dir];
      var stepFunctions = {
        up: this.gotoUp,
        down: this.gotoDown,
        prev: this.gotoPrev,
        next: this.gotoNext,
        home: this.gotoRowStart,
        end: this.gotoRowEnd
      };
      stepFunctions[this._rtlS] = this.gotoLeft;
      stepFunctions[this._rtlE] = this.gotoRight;
      var stepFn = stepFunctions[dir].bind(this);
      var pos = stepFn(this._activeRow, this._activeCell, this._activePosX);
      if (pos) {
        if (this._hasFrozenRows && this._options.frozenBottom && pos.row == this.getDataLength()) {
          return;
        }
        var isAddNewRow = pos.row == this.getDataLength();
        if (!this._options.frozenBottom && pos.row >= this._actualFrozenRow || this._options.frozenBottom && pos.row < this._actualFrozenRow) {
          this.scrollCellIntoView(pos.row, pos.cell, !isAddNewRow);
        }
        this.setActiveCellInternal(this.getCellNode(pos.row, pos.cell));
        this._activePosX = pos.posX;
        return true;
      } else {
        this.setActiveCellInternal(this.getCellNode(this._activeRow, this._activeCell));
        return false;
      }
    }
    getCellNode(row, cell) {
      if (this._rowsCache[row]) {
        this.ensureCellNodesInRowsCache(row);
        return this._rowsCache[row].cellNodesByColumnIdx[cell];
      }
      return null;
    }
    setActiveCell(row, cell) {
      if (!this._initialized) {
        return;
      }
      var cols = this._cols;
      if (row > this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
        return;
      }
      if (!this._options.enableCellNavigation) {
        return;
      }
      this.scrollCellIntoView(row, cell, false);
      this.setActiveCellInternal(this.getCellNode(row, cell), false);
    }
    canCellBeActive(row, cell) {
      var cols = this._cols;
      if (!this._options.enableCellNavigation || row >= this.getDataLengthIncludingAddNew() || row < 0 || cell >= cols.length || cell < 0) {
        return false;
      }
      var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      if (rowMetadata && typeof rowMetadata.focusable === "boolean") {
        return rowMetadata.focusable;
      }
      var columnMetadata = rowMetadata && rowMetadata.columns;
      if (columnMetadata && cols[cell] && columnMetadata[cols[cell].id] && typeof columnMetadata[cols[cell].id].focusable === "boolean") {
        return columnMetadata[cols[cell].id].focusable;
      }
      if (columnMetadata && columnMetadata[cell] && typeof columnMetadata[cell].focusable === "boolean") {
        return columnMetadata[cell].focusable;
      }
      return cols[cell].focusable;
    }
    canCellBeSelected(row, cell) {
      var cols = this._cols;
      if (row >= this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
        return false;
      }
      var rowMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      if (rowMetadata && typeof rowMetadata.selectable === "boolean") {
        return rowMetadata.selectable;
      }
      var columnMetadata = rowMetadata && rowMetadata.columns && (rowMetadata.columns[cols[cell].id] || rowMetadata.columns[cell]);
      if (columnMetadata && typeof columnMetadata.selectable === "boolean") {
        return columnMetadata.selectable;
      }
      return cols[cell].selectable;
    }
    gotoCell(row, cell, forceEdit) {
      if (!this._initialized) {
        return;
      }
      if (!this.canCellBeActive(row, cell)) {
        return;
      }
      if (!this.getEditorLock().commitCurrentEdit()) {
        return;
      }
      this.scrollCellIntoView(row, cell, false);
      var newCell = this.getCellNode(row, cell);
      this.setActiveCellInternal(newCell, forceEdit || row === this.getDataLength() || this._options.autoEdit);
      if (!this._currentEditor) {
        this.setFocus();
      }
    }
    commitCurrentEdit() {
      var item = this.getDataItem(this._activeRow);
      var column = this._cols[this._activeCell];
      var self = this;
      if (this._currentEditor) {
        if (this._currentEditor.isValueChanged()) {
          var validationResults = this._currentEditor.validate();
          if (validationResults.valid) {
            if (this._activeRow < this.getDataLength()) {
              var editCommand = {
                row: this._activeRow,
                cell: self._activeCell,
                editor: this._currentEditor,
                serializedValue: this._currentEditor.serializeValue(),
                prevSerializedValue: this._serializedEditorValue,
                execute: function() {
                  this.editor.applyValue(item, this.serializedValue);
                  self.updateRow(this.row);
                  self.trigger(self.onCellChange, {
                    row: this.activeRow,
                    cell: self._activeCell,
                    item
                  });
                },
                undo: function() {
                  this.editor.applyValue(item, this.prevSerializedValue);
                  self.updateRow(this.row);
                  self.trigger(self.onCellChange, {
                    row: this.activeRow,
                    cell: self._activeCell,
                    item
                  });
                }
              };
              if (this._options.editCommandHandler) {
                this.makeActiveCellNormal();
                this._options.editCommandHandler(item, column, editCommand);
              } else {
                editCommand.execute();
                this.makeActiveCellNormal();
              }
            } else {
              var newItem = {};
              this._currentEditor.applyValue(newItem, this._currentEditor.serializeValue());
              this.makeActiveCellNormal();
              this.trigger(this.onAddNewRow, { item: newItem, column });
            }
            return !this.getEditorLock().isActive();
          } else {
            $(this._activeCellNode).removeClass("invalid");
            $(this._activeCellNode).width();
            $(this._activeCellNode).addClass("invalid");
            this.trigger(this.onValidationError, {
              editor: this._currentEditor,
              cellNode: this._activeCellNode,
              validationResults,
              row: this._activeRow,
              cell: this._activeCell,
              column
            });
            this._currentEditor.focus();
            return false;
          }
        }
        this.makeActiveCellNormal();
      }
      return true;
    }
    cancelCurrentEdit() {
      this.makeActiveCellNormal();
      return true;
    }
    rowsToRanges(rows) {
      var ranges = [];
      var lastCell = this._cols.length - 1;
      for (var i = 0; i < rows.length; i++) {
        ranges.push(new Range(rows[i], 0, rows[i], lastCell));
      }
      return ranges;
    }
    getSelectedRows() {
      if (!this._selectionModel) {
        throw "Selection model is not set";
      }
      return this._selectedRows;
    }
    setSelectedRows(rows) {
      if (!this._selectionModel) {
        throw "Selection model is not set";
      }
      this._selectionModel.setSelectedRanges(this.rowsToRanges(rows));
    }
  };
  return __toCommonJS(slick_grid_exports);
})();
/**
 * @license
 * (c) 2017-2022 Serenity.is, Volkan Ceylan, Furkan Evran, Victor Tomaili, and other Serenity contributors
 * https://github.com/serenity-is/serenity
 * Ported to TypeScript and heavily refactored while keeping compatibility with original SlickGrid where possible
 * 
 * (c) 2009-2016 Michael Leibman 
 * michael{dot}leibman{at}gmail{dot}com
 * http://github.com/mleibman/slickgrid
 *
 * (c) 2009-2019 Ben McIntyre
 * http://github.com/6pac/slickgrid
 * 
 * Distributed under MIT license.
 * All rights reserved.
 *
 * Based on SlickGrid v2.4 and 6pack/slickgrid fixes
 *
 * NOTES:
 *     Cell/row DOM manipulations are done directly bypassing jQuery's DOM manipulation methods.
 *     This increases the speed dramatically, but can only be done safely because there are no event handlers
 *     or data associated with any cell/row DOM nodes.  Cell editors must make sure they implement .destroy()
 *     and do proper cleanup.
 */
(function(){for(var _ in Slick._)Slick[_]=Slick._[_];delete Slick._})();
