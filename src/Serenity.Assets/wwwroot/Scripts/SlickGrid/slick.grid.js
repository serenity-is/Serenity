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

  // src/grid/index.ts
  var grid_exports = {};
  __export(grid_exports, {
    BasicLayout: () => BasicLayout,
    Grid: () => Grid,
    gridDefaults: () => gridDefaults
  });

  // global-externals:_
  var { addClass, applyFormatterResultToCellNode, columnDefaults, convertCompatFormatter, ensureUniqueColumnIds, escape, defaultColumnFormat, disableSelection, EventEmitter, EventData, GlobalEditorLock, initializeColumns, H, keyCode, NonDataRow, parsePx, preClickClassName, Range, removeClass, RowCell, spacerDiv, titleize } = Slick;

  // src/grid/gridoptions.ts
  var gridDefaults = {
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
    defaultFormat: defaultColumnFormat,
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
    footerRowHeight: 30,
    forceFitColumns: false,
    forceSyncScrolling: false,
    forceSyncScrollInterval: 250,
    formatterFactory: null,
    fullWidthRows: false,
    groupingPanel: false,
    groupingPanelHeight: 30,
    headerRowHeight: 30,
    leaveSpaceForNewRows: false,
    minBuffer: 3,
    multiColumnSort: false,
    multiSelect: true,
    renderAllCells: false,
    rowHeight: 30,
    selectedCellCssClass: "selected",
    showCellSelection: true,
    showColumnHeader: true,
    showFooterRow: false,
    showGroupingPanel: true,
    showHeaderRow: false,
    showTopPanel: false,
    suppressActiveCellChangeOnEdit: false,
    topPanelHeight: 30,
    useLegacyUI: true,
    useCssVars: false
  };

  // src/grid/basiclayout.ts
  var BasicLayout = function() {
    var host;
    var canvasWidth;
    var headersWidth;
    var canvas;
    var headerCols;
    var headerRowCols;
    var headerRowSpacer;
    var footerRowCols;
    var footerRowSpacer;
    var topPanel;
    var viewport;
    function init(hostGrid) {
      host = hostGrid;
      const spacerW = calcCanvasWidth() + host.getScrollDims().width + "px";
      const options = host.getOptions();
      const uisd = options.useLegacyUI ? " ui-state-default" : "";
      headerCols = H("div", { class: "slick-header-columns", style: (options.rtl ? "right" : "left") + ":-1000px" });
      var headerColsS = H("div", { class: "slick-header" + uisd, style: !options.showColumnHeader && "display: none" }, headerCols);
      updateHeadersWidth();
      headerRowCols = H("div", { class: "slick-headerrow-columns" });
      headerRowSpacer = spacerDiv(spacerW);
      var headerRow = H("div", { class: "slick-headerrow" + uisd, style: !options.showHeaderRow && "display: none" }, headerRowCols, headerRowSpacer);
      topPanel = H("div", { class: "slick-top-panel", style: "width: 10000px" });
      var topPanelS = H("div", { class: "slick-top-panel-scroller" + uisd, style: !options.showTopPanel && "display: none" }, topPanel);
      canvas = H("div", { class: "grid-canvas", tabIndex: "0", hideFocus: "" });
      viewport = H("div", { class: "slick-viewport", tabIndex: "0", hideFocus: "" }, canvas);
      footerRowCols = H("div", { class: "slick-footerrow-columns" });
      footerRowSpacer = spacerDiv(spacerW);
      var footerRow = H("div", { class: "slick-footerrow" + uisd, style: !options.showFooterRow && "display: none" }, footerRowCols, footerRowSpacer);
      host.getContainerNode().append(headerColsS, headerRow, topPanelS, viewport, footerRow);
    }
    function appendCachedRow(_, rowNode) {
      rowNode && canvas.appendChild(rowNode);
    }
    function applyColumnWidths() {
      var x = 0, w, rule, cols = host.getColumns(), opts = host.getOptions(), rtl = opts.rtl;
      if (opts.useCssVars) {
        var styles = host.getContainerNode().style;
        for (var i = 0; i < cols.length; i++) {
          w = cols[i].width;
          var prop = "--l" + i;
          var oldVal = styles.getPropertyValue(prop);
          var newVal = x + "px";
          if (oldVal !== newVal)
            styles.setProperty(prop, newVal);
          prop = "--r" + i;
          oldVal = styles.getPropertyValue(prop);
          newVal = canvasWidth - x - w + "px";
          if (oldVal !== newVal)
            styles.setProperty(prop, newVal);
          x += w;
        }
      } else {
        for (var i = 0; i < cols.length; i++) {
          w = cols[i].width;
          rule = host.getColumnCssRules(i);
          rule[rtl ? "right" : "left"].style[rtl ? "right" : "left"] = x + "px";
          rule[rtl ? "left" : "right"].style[rtl ? "left" : "right"] = canvasWidth - x - w + "px";
          x += w;
        }
      }
    }
    function bindAncestorScrollEvents() {
      var elem = canvas;
      while ((elem = elem.parentNode) != document.body && elem != null) {
        if (elem == viewport || elem.scrollWidth != elem.clientWidth || elem.scrollHeight != elem.clientHeight) {
          host.bindAncestorScroll(elem);
        }
      }
    }
    function calcCanvasWidth() {
      var cols = host.getColumns(), i = cols.length;
      var rowWidth = 0;
      while (i--) {
        rowWidth += cols[i].width;
      }
      return host.getOptions().fullWidthRows ? Math.max(
        rowWidth,
        host.getAvailableWidth()
      ) : rowWidth;
    }
    function updateHeadersWidth() {
      headersWidth = 0;
      var scrollWidth = host.getScrollDims().width;
      var cols = host.getColumns();
      for (var i = 0, ii = cols.length; i < ii; i++) {
        headersWidth += cols[i].width;
      }
      headersWidth += scrollWidth;
      headersWidth = Math.max(headersWidth, host.getViewportInfo().width) + 1e3;
      headerCols.style.width = headersWidth + "px";
    }
    const destroy = () => {
      host = null;
    };
    function getCanvasNodeFor() {
      return canvas;
    }
    function getCanvasNodes() {
      return [canvas];
    }
    function getCanvasWidth() {
      return canvasWidth;
    }
    function getHeaderCols() {
      return [headerCols];
    }
    function getHeaderColumn(cell) {
      return headerCols.children.item(cell);
    }
    function getHeaderRowCols() {
      return [headerRowCols];
    }
    function getHeaderRowColumn(cell) {
      return headerRowCols.childNodes.item(cell);
    }
    function getHeaderRowColsFor() {
      return headerRowCols;
    }
    function getFooterRowColumn(cell) {
      return footerRowCols.childNodes.item(cell);
    }
    function getFooterRowColsFor() {
      return footerRowCols;
    }
    function getHeaderColsFor() {
      return headerCols;
    }
    function getFooterRowCols() {
      return [footerRowCols];
    }
    function getRowFromCellNode(cellNode) {
      return host.getRowFromNode(cellNode.parentElement);
    }
    function getTopPanelFor() {
      return topPanel;
    }
    function getTopPanelNodes() {
      return [topPanel];
    }
    function getViewportNodeFor() {
      return viewport;
    }
    function getViewportNodes() {
      return [viewport];
    }
    function handleScrollH() {
      headerCols.parentElement.scrollLeft = host.getScrollLeft();
      topPanel.parentElement.scrollLeft = host.getScrollLeft();
      headerRowCols.parentElement.scrollLeft = host.getScrollLeft();
      footerRowCols.parentElement.scrollLeft = host.getScrollLeft();
    }
    function noop() {
    }
    function realScrollHeightChange() {
      canvas.style.height = host.getViewportInfo().realScrollHeight + "px";
    }
    function reorderViewColumns(viewCols) {
      return viewCols;
    }
    function returnFalse() {
      return false;
    }
    function setOverflow() {
      var alwaysVS = host.getOptions().alwaysShowVerticalScroll;
      viewport.style.overflowX = "auto";
      viewport.style.overflowY = alwaysVS ? "scroll" : host.getOptions().autoHeight ? "hidden" : "auto";
    }
    function updateCanvasWidth() {
      var oldCanvasWidth = canvasWidth;
      canvasWidth = calcCanvasWidth();
      var scrollWidth = host.getScrollDims().width;
      const vpi = host.getViewportInfo();
      var canvasWidthPx = canvasWidth + "px";
      canvas.style.width = canvasWidthPx;
      headerRowCols.style.width = canvasWidthPx;
      footerRowCols.style.width = canvasWidthPx;
      updateHeadersWidth();
      vpi.hasHScroll = canvasWidth > host.getViewportInfo().width - scrollWidth;
      var spacerWidthPx = canvasWidth + (vpi.hasVScroll ? scrollWidth : 0) + "px";
      headerRowSpacer.style.width = spacerWidthPx;
      footerRowSpacer.style.width = spacerWidthPx;
      return canvasWidth != oldCanvasWidth;
    }
    const resizeCanvas = () => {
      var vs = host.getViewportInfo();
      var _paneTopH = vs.height + vs.topPanelHeight + vs.headerRowHeight + vs.footerRowHeight;
      const options = host.getOptions();
      if (options.autoHeight) {
        host.getContainerNode().style.height = _paneTopH + vs.groupingPanelHeight + parsePx(getComputedStyle(headerCols.parentElement).height) + "px";
        viewport.style.height = "";
      } else
        viewport.style.height = vs.height + "px";
    };
    function returnZero() {
      return 0;
    }
    var intf = {
      afterHeaderColumnDrag: noop,
      afterRenderRows: noop,
      afterSetOptions: noop,
      appendCachedRow,
      applyColumnWidths,
      beforeCleanupAndRenderCells: noop,
      bindAncestorScrollEvents,
      calcCanvasWidth,
      updateHeadersWidth,
      isFrozenRow: returnFalse,
      destroy,
      getCanvasNodeFor,
      getCanvasNodes,
      getCanvasWidth,
      getFooterRowCols,
      getFooterRowColsFor,
      getFooterRowColumn,
      getHeaderCols,
      getHeaderColsFor,
      getHeaderColumn,
      getHeaderRowCols,
      getHeaderRowColsFor,
      getHeaderRowColumn,
      getRowFromCellNode,
      getFrozenCols: returnZero,
      getFrozenRowOffset: returnZero,
      getFrozenRows: returnZero,
      getScrollCanvasY: getCanvasNodeFor,
      getScrollContainerX: getViewportNodeFor,
      getScrollContainerY: getViewportNodeFor,
      getTopPanelFor,
      getTopPanelNodes,
      getViewportNodeFor,
      getViewportNodes,
      handleScrollH,
      handleScrollV: noop,
      init,
      layoutName: "basic",
      realScrollHeightChange,
      reorderViewColumns,
      resizeCanvas,
      setOverflow,
      setPaneVisibility: noop,
      setScroller: noop,
      updateCanvasWidth
    };
    return intf;
  };

  // src/grid/cellnavigator.ts
  var CellNavigator = class {
    constructor(h) {
      this.host = h;
    }
    findFirstFocusableCell(row) {
      var cell = 0;
      var cols = this.host.getColumnCount();
      while (cell < cols) {
        if (this.host.canCellBeActive(row, cell)) {
          return cell;
        }
        cell += this.host.getColspan(row, cell);
      }
      return null;
    }
    findLastFocusableCell(row) {
      var cell = 0;
      var lastFocusableCell = null;
      var cols = this.host.getColumnCount();
      while (cell < cols) {
        if (this.host.canCellBeActive(row, cell)) {
          lastFocusableCell = cell;
        }
        cell += this.host.getColspan(row, cell);
      }
      return lastFocusableCell;
    }
    gotoRight(row, cell) {
      var cols = this.host.getColumnCount();
      if (cell >= cols) {
        return null;
      }
      do {
        cell += this.host.getColspan(row, cell);
      } while (cell < cols && !this.host.canCellBeActive(row, cell));
      if (cell < cols) {
        return {
          row,
          cell,
          posX: cell
        };
      }
      return null;
    }
    gotoLeft(row, cell) {
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
        pos = this.gotoRight(prev.row, prev.cell);
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
      var rowCount = this.host.getRowCount();
      while (true) {
        if (++row >= rowCount) {
          return null;
        }
        prevCell = cell = 0;
        while (cell <= posX) {
          prevCell = cell;
          cell += this.host.getColspan(row, cell);
        }
        if (this.host.canCellBeActive(row, prevCell)) {
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
          cell += this.host.getColspan(row, cell);
        }
        if (this.host.canCellBeActive(row, prevCell)) {
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
        if (this.host.canCellBeActive(row, cell)) {
          return {
            row,
            cell,
            posX: cell
          };
        }
      }
      var pos = this.gotoRight(row, cell);
      if (pos) {
        return pos;
      }
      var firstFocusableCell = null;
      var dataLengthIncludingAddNew = this.host.getRowCount();
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
      var cols = this.host.getColumnCount();
      if (row == null && cell == null) {
        row = this.host.getRowCount() - 1;
        cell = posX = cols - 1;
        if (this.host.canCellBeActive(row, cell)) {
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
        pos = this.gotoLeft(row, cell);
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
    /**
     * @param {string} dir Navigation direction.
     * @return {boolean} Whether navigation resulted in a change of active cell.
     */
    navigate(dir, activeRow, activeCell, activePosX) {
      var tabbingDirections = {
        up: -1,
        down: 1,
        prev: -1,
        next: 1,
        home: -1,
        end: 1
      };
      const rtl = this.host.isRTL();
      tabbingDirections[rtl ? "right" : "left"] = -1;
      tabbingDirections[rtl ? "left" : "right"] = 1;
      this.host.setTabbingDirection(tabbingDirections[dir]);
      var stepFunctions = {
        up: this.gotoUp,
        down: this.gotoDown,
        prev: this.gotoPrev,
        next: this.gotoNext,
        home: this.gotoRowStart,
        end: this.gotoRowEnd
      };
      stepFunctions[rtl ? "right" : "left"] = this.gotoLeft;
      stepFunctions[rtl ? "left" : "right"] = this.gotoRight;
      var stepFn = stepFunctions[dir].bind(this);
      return stepFn(activeRow, activeCell, activePosX);
    }
  };

  // src/grid/internal.ts
  var maxSupportedCssHeight;
  var scrollbarDimensions;
  function absBox(elem) {
    var box = {
      top: elem.offsetTop,
      left: elem.offsetLeft,
      bottom: 0,
      right: 0,
      width: elem.offsetWidth,
      height: elem.offsetHeight,
      visible: true
    };
    box.bottom = box.top + box.height;
    box.right = box.left + box.width;
    var offsetParent = elem.offsetParent;
    while ((elem = elem.parentNode) != document.body && elem != null) {
      if (box.visible && elem.scrollHeight != elem.offsetHeight && getComputedStyle(elem).overflowY !== "visible") {
        box.visible = box.bottom > elem.scrollTop && box.top < elem.scrollTop + elem.clientHeight;
      }
      if (box.visible && elem.scrollWidth != elem.offsetWidth && getComputedStyle(elem).overflowX != "visible") {
        box.visible = box.right > elem.scrollLeft && box.left < elem.scrollLeft + elem.clientWidth;
      }
      box.left -= elem.scrollLeft;
      box.top -= elem.scrollTop;
      if (elem === offsetParent) {
        box.left += elem.offsetLeft;
        box.top += elem.offsetTop;
        offsetParent = elem.offsetParent;
      }
      box.bottom = box.top + box.height;
      box.right = box.left + box.width;
    }
    return box;
  }
  function autosizeColumns(cols, availWidth, absoluteColMinWidth) {
    var i, c, widths = [], shrinkLeeway = 0, total = 0, prevTotal;
    for (i = 0; i < cols.length; i++) {
      c = cols[i];
      widths.push(c.width);
      total += c.width;
      if (c.resizable) {
        shrinkLeeway += c.width - Math.max(c.minWidth, absoluteColMinWidth);
      }
    }
    prevTotal = total;
    while (total > availWidth && shrinkLeeway) {
      var shrinkProportion = (total - availWidth) / shrinkLeeway;
      for (i = 0; i < cols.length && total > availWidth; i++) {
        c = cols[i];
        var width = widths[i];
        if (!c.resizable || width <= c.minWidth || width <= absoluteColMinWidth) {
          continue;
        }
        var absMinWidth = Math.max(c.minWidth, absoluteColMinWidth);
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
    return reRender;
  }
  function getMaxSupportedCssHeight() {
    return maxSupportedCssHeight != null ? maxSupportedCssHeight : navigator.userAgent.toLowerCase().match(/gecko\//) ? 4e6 : 32e6;
  }
  function getScrollBarDimensions(recalc) {
    if (!scrollbarDimensions || recalc) {
      var c = document.body.appendChild(H("div", {
        style: "position:absolute;top:-10000px;left:-10000px;width:100px;height:100px;overflow: scroll;border:0"
      }));
      scrollbarDimensions = {
        width: Math.round(c.offsetWidth - c.clientWidth),
        height: Math.round(c.offsetWidth - c.clientHeight)
      };
      c.remove();
    }
    return scrollbarDimensions;
  }
  function simpleArrayEquals(arr1, arr2) {
    if (!Array.isArray(arr1) || !Array.isArray(arr2) || arr1.length !== arr2.length)
      return false;
    arr1 = arr1.slice().sort();
    arr2 = arr2.slice().sort();
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
  function calcMinMaxPageXOnDragStart(cols, colIdx, pageX, forceFit, absoluteColMinWidth) {
    var shrinkLeewayOnRight = null, stretchLeewayOnRight = null, j, c;
    if (forceFit) {
      shrinkLeewayOnRight = 0;
      stretchLeewayOnRight = 0;
      for (j = colIdx + 1; j < cols.length; j++) {
        c = cols[j];
        if (c.resizable) {
          if (stretchLeewayOnRight != null) {
            if (c.maxWidth) {
              stretchLeewayOnRight += c.maxWidth - c.previousWidth;
            } else {
              stretchLeewayOnRight = null;
            }
          }
          shrinkLeewayOnRight += c.previousWidth - Math.max(c.minWidth || 0, absoluteColMinWidth);
        }
      }
    }
    var shrinkLeewayOnLeft = 0, stretchLeewayOnLeft = 0;
    for (j = 0; j <= colIdx; j++) {
      c = cols[j];
      if (c.resizable) {
        if (stretchLeewayOnLeft != null) {
          if (c.maxWidth) {
            stretchLeewayOnLeft += c.maxWidth - c.previousWidth;
          } else {
            stretchLeewayOnLeft = null;
          }
        }
        shrinkLeewayOnLeft += c.previousWidth - Math.max(c.minWidth || 0, absoluteColMinWidth);
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
    return {
      maxPageX: pageX + Math.min(shrinkLeewayOnRight, stretchLeewayOnLeft),
      minPageX: pageX - Math.min(shrinkLeewayOnLeft, stretchLeewayOnRight)
    };
  }
  function shrinkOrStretchColumn(cols, colIdx, d, forceFit, absoluteColMinWidth) {
    var c, j, x, actualMinWidth;
    if (d < 0) {
      x = d;
      for (j = colIdx; j >= 0; j--) {
        c = cols[j];
        if (c.resizable) {
          actualMinWidth = Math.max(c.minWidth || 0, absoluteColMinWidth);
          if (x && c.previousWidth + x < actualMinWidth) {
            x += c.previousWidth - actualMinWidth;
            c.width = actualMinWidth;
          } else {
            c.width = c.previousWidth + x;
            x = 0;
          }
        }
      }
      if (forceFit) {
        x = -d;
        for (j = colIdx + 1; j < cols.length; j++) {
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
      }
    } else {
      x = d;
      for (j = colIdx; j >= 0; j--) {
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
      if (forceFit) {
        x = -d;
        for (j = colIdx + 1; j < cols.length; j++) {
          c = cols[j];
          if (c.resizable) {
            actualMinWidth = Math.max(c.minWidth || 0, absoluteColMinWidth);
            if (x && c.previousWidth + x < actualMinWidth) {
              x += c.previousWidth - actualMinWidth;
              c.width = actualMinWidth;
            } else {
              c.width = c.previousWidth + x;
              x = 0;
            }
          }
        }
      }
    }
  }
  function addUiStateHover() {
    this == null ? void 0 : this.classList.add("ui-state-hover");
  }
  function removeUiStateHover() {
    this == null ? void 0 : this.classList.remove("ui-state-hover");
  }
  function getVBoxDelta(el) {
    if (!el)
      return 0;
    var style = getComputedStyle(el);
    if (style.boxSizing === "border-box")
      return 0;
    var p = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];
    var delta = 0;
    for (var val of p)
      delta += parsePx(style.getPropertyValue(val)) || 0;
    return delta;
  }
  function getInnerWidth(el) {
    var _a;
    var style = getComputedStyle(el);
    var width = (_a = parsePx(style.width)) != null ? _a : 0;
    if (style.boxSizing != "border-box")
      return Math.max(0, width);
    var p = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
    for (var val of p)
      width -= parsePx(style.getPropertyValue(val)) || 0;
    return Math.max(width, 0);
  }

  // src/grid/grid.ts
  var Grid = class {
    constructor(container, data, columns, options) {
      this._activeCellNode = null;
      this._cellCssClasses = {};
      this._cellHeightDiff = 0;
      this._cellWidthDiff = 0;
      this._colLeft = [];
      this._colRight = [];
      this._currentEditor = null;
      this._headerColumnWidthDiff = 0;
      this._hEditorLoader = null;
      this._hPostRender = null;
      this._hPostRenderCleanup = null;
      this._hRender = null;
      this._ignoreScrollUntil = 0;
      this._initialized = false;
      this._page = 0;
      this._pageOffset = 0;
      this._pagingActive = false;
      this._pagingIsLastPage = false;
      this._plugins = [];
      this._postProcessCleanupQueue = [];
      this._postProcessedRows = {};
      this._postProcessFromRow = null;
      this._postProcessGroupId = 0;
      this._postProcessToRow = null;
      this._rowsCache = {};
      this._scrollLeft = 0;
      this._scrollLeftPrev = 0;
      this._scrollLeftRendered = 0;
      this._scrollTop = 0;
      this._scrollTopPrev = 0;
      this._scrollTopRendered = 0;
      this._selectedRows = [];
      this._sortColumns = [];
      this._tabbingDirection = 1;
      this._uid = "sleekgrid_" + Math.round(1e6 * Math.random());
      this._viewportInfo = {};
      this._vScrollDir = 1;
      this._boundAncestorScroll = [];
      this.onActiveCellChanged = new EventEmitter();
      this.onActiveCellPositionChanged = new EventEmitter();
      this.onAddNewRow = new EventEmitter();
      this.onBeforeCellEditorDestroy = new EventEmitter();
      this.onBeforeDestroy = new EventEmitter();
      this.onBeforeEditCell = new EventEmitter();
      this.onBeforeFooterRowCellDestroy = new EventEmitter();
      this.onBeforeHeaderCellDestroy = new EventEmitter();
      this.onBeforeHeaderRowCellDestroy = new EventEmitter();
      this.onCellChange = new EventEmitter();
      this.onCellCssStylesChanged = new EventEmitter();
      this.onClick = new EventEmitter();
      this.onColumnsReordered = new EventEmitter();
      this.onColumnsResized = new EventEmitter();
      this.onCompositeEditorChange = new EventEmitter();
      this.onContextMenu = new EventEmitter();
      this.onDblClick = new EventEmitter();
      this.onDrag = new EventEmitter();
      this.onDragEnd = new EventEmitter();
      this.onDragInit = new EventEmitter();
      this.onDragStart = new EventEmitter();
      this.onFooterRowCellRendered = new EventEmitter();
      this.onHeaderCellRendered = new EventEmitter();
      this.onHeaderClick = new EventEmitter();
      this.onHeaderContextMenu = new EventEmitter();
      this.onHeaderMouseEnter = new EventEmitter();
      this.onHeaderMouseLeave = new EventEmitter();
      this.onHeaderRowCellRendered = new EventEmitter();
      this.onKeyDown = new EventEmitter();
      this.onMouseEnter = new EventEmitter();
      this.onMouseLeave = new EventEmitter();
      this.onScroll = new EventEmitter();
      this.onSelectedRowsChanged = new EventEmitter();
      this.onSort = new EventEmitter();
      this.onValidationError = new EventEmitter();
      this.onViewportChanged = new EventEmitter();
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
        hash = {}, cols = this._cols;
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
        this.updateGrandTotals();
      };
      this.viewOnDataChanged = () => {
        this.invalidate();
        this.render();
      };
      this.resizeCanvas = () => {
        if (!this._initialized) {
          return;
        }
        this.calcViewportSize();
        this._layout.resizeCanvas();
        if (!this._scrollDims || !this._scrollDims.width) {
          this._scrollDims = getScrollBarDimensions(true);
        }
        if (this._options.forceFitColumns) {
          this.autosizeColumns();
        }
        this.updateRowCount();
        this.handleScroll();
        this._scrollLeftRendered = -1;
        this.render();
      };
      this.render = () => {
        if (!this._initialized) {
          return;
        }
        var visible = this.getVisibleRange();
        var rendered = this.getRenderedRange();
        this.cleanupRows(rendered);
        if (this._scrollLeftRendered != this._scrollLeft) {
          this._layout.beforeCleanupAndRenderCells(rendered);
          this.cleanUpAndRenderCells(rendered);
        }
        this.renderRows(rendered);
        this._layout.afterRenderRows(rendered);
        this._postProcessFromRow = visible.top;
        this._postProcessToRow = Math.min(this.getDataLengthIncludingAddNew() - 1, visible.bottom);
        this.startPostProcessing();
        this._scrollTopRendered = this._scrollTop;
        this._scrollLeftRendered = this._scrollLeft;
        this._lastRenderTime = new Date().getTime();
        this._hRender = null;
      };
      this.handleHeaderRowScroll = (e) => {
        if (this._ignoreScrollUntil >= new Date().getTime())
          return;
        var scrollLeft = e.target.scrollLeft;
        if (scrollLeft != this._layout.getScrollContainerX().scrollLeft) {
          this._layout.getScrollContainerX().scrollLeft = scrollLeft;
        }
      };
      this.handleFooterRowScroll = (e) => {
        if (this._ignoreScrollUntil >= new Date().getTime())
          return;
        var scrollLeft = e.target.scrollLeft;
        if (scrollLeft != this._layout.getScrollContainerX().scrollLeft) {
          this._layout.getScrollContainerX().scrollLeft = scrollLeft;
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
      var _a, _b;
      this._data = data;
      this._colDefaults = Object.assign({}, columnDefaults);
      this._options = options = Object.assign({}, gridDefaults, options);
      options.jQuery = this._jQuery = options.jQuery === void 0 ? typeof jQuery !== "undefined" ? jQuery : void 0 : options.jQuery;
      if (this._jQuery && container instanceof this._jQuery)
        this._container = container[0];
      else if (container instanceof Element)
        this._container = container;
      else if (typeof container === "string")
        this._container = document.querySelector(container);
      if (this._container == null) {
        throw new Error("SleekGrid requires a valid container, " + container + " does not exist in the DOM.");
      }
      this._container.classList.add("slick-container");
      if (options == null ? void 0 : options.createPreHeaderPanel) {
        if (options.groupingPanel == null)
          options.groupingPanel = true;
        if (options.groupingPanelHeight == null && options.preHeaderPanelHeight != null)
          options.groupingPanelHeight = options.preHeaderPanelHeight;
        if (options.showGroupingPanel == null && options.showPreHeaderPanel != null)
          options.showGroupingPanel = options.showPreHeaderPanel;
      }
      this._options.rtl = (_a = this._options.rtl) != null ? _a : document.body.classList.contains("rtl") || typeof getComputedStyle != "undefined" && getComputedStyle(this._container).direction == "rtl";
      if (this._options.rtl)
        this._container.classList.add("rtl");
      else
        this._container.classList.add("ltr");
      this.validateAndEnforceOptions();
      this._colDefaults.width = options.defaultColumnWidth;
      this._editController = {
        "commitCurrentEdit": this.commitCurrentEdit.bind(this),
        "cancelCurrentEdit": this.cancelCurrentEdit.bind(this)
      };
      if (this._jQuery)
        this._jQuery(this._container).empty();
      else
        this._container.innerHTML = "";
      this._container.style.overflow = "hidden";
      this._container.style.outline = "0";
      this._container.classList.add(this._uid);
      if (this._options.useLegacyUI)
        this._container.classList.add("ui-widget");
      if (!/relative|absolute|fixed/.test(getComputedStyle(this._container).position)) {
        this._container.style.position = "relative";
      }
      this._container.appendChild(this._focusSink1 = H("div", {
        class: "slick-focus-sink",
        hideFocus: "",
        style: "position:fixed;width:0!important;height:0!important;top:0;left:0;outline:0!important;",
        tabIndex: "0"
      }));
      this._layout = (_b = options.layoutEngine) != null ? _b : new BasicLayout();
      this.setInitialCols(columns);
      this._scrollDims = getScrollBarDimensions();
      if (options.groupingPanel) {
        this.createGroupingPanel();
      }
      this._layout.init({
        cleanUpAndRenderCells: this.cleanUpAndRenderCells.bind(this),
        bindAncestorScroll: this.bindAncestorScroll.bind(this),
        getAvailableWidth: this.getAvailableWidth.bind(this),
        getCellFromPoint: this.getCellFromPoint.bind(this),
        getColumnCssRules: this.getColumnCssRules.bind(this),
        getColumns: this.getColumns.bind(this),
        getContainerNode: this.getContainerNode.bind(this),
        getDataLength: this.getDataLength.bind(this),
        getOptions: this.getOptions.bind(this),
        getRowFromNode: this.getRowFromNode.bind(this),
        getScrollDims: this.getScrollBarDimensions.bind(this),
        getScrollLeft: () => this._scrollLeft,
        getScrollTop: () => this._scrollTop,
        getViewportInfo: () => this._viewportInfo,
        renderRows: this.renderRows.bind(this)
      });
      this._container.append(this._focusSink2 = this._focusSink1.cloneNode());
      if (options.viewportClass)
        this.getViewports().forEach((vp) => addClass(vp, options.viewportClass));
      if (!options.explicitInitialization) {
        this.init();
      }
      this.bindToData();
    }
    createGroupingPanel() {
      if (this._groupingPanel || !this._focusSink1)
        return;
      this._focusSink1.insertAdjacentElement("afterend", this._groupingPanel = H("div", {
        class: "slick-grouping-panel",
        style: "overflow:hidden; position:relative;" + (!this._options.showGroupingPanel ? " display: none" : "")
      }));
      if (this._options.createPreHeaderPanel) {
        this._groupingPanel.appendChild(H("div", { class: "slick-preheader-panel" }));
      }
    }
    bindAncestorScroll(elem) {
      if (this._jQuery)
        this._jQuery(elem).on("scroll", this.handleActiveCellPositionChange);
      else
        elem.addEventListener("scroll", this.handleActiveCellPositionChange);
      this._boundAncestorScroll.push(elem);
    }
    init() {
      if (this._initialized)
        return;
      this._initialized = true;
      this.calcViewportSize();
      this.measureCellPaddingAndBorder();
      var viewports = this.getViewports();
      if (this._jQuery && !this._options.enableTextSelectionOnCells) {
        this._jQuery(viewports).on("selectstart.ui", () => {
          return this._jQuery(this).is("input,textarea");
        });
      }
      this._layout.setPaneVisibility();
      this._layout.setScroller();
      this.setOverflow();
      this.updateViewColLeftRight();
      this.createColumnHeaders();
      this.createColumnFooters();
      this.setupColumnSort();
      this.createCssRules();
      this.resizeCanvas();
      this._layout.bindAncestorScrollEvents();
      const onEvent = (el, type, listener) => {
        if (this._jQuery)
          this._jQuery(el).on(type, listener);
        else
          el.addEventListener(type, listener);
      };
      onEvent(this._container, "resize", this.resizeCanvas);
      viewports.forEach((vp) => {
        var scrollTicking = false;
        onEvent(vp, "scroll", (e) => {
          if (!scrollTicking) {
            scrollTicking = true;
            window.requestAnimationFrame(() => {
              this.handleScroll();
              scrollTicking = false;
            });
          }
        });
      });
      if (this._jQuery && this._jQuery.fn.mousewheel && (this.hasFrozenColumns() || this.hasFrozenRows())) {
        this._jQuery(viewports).on("mousewheel", this.handleMouseWheel.bind(this));
      }
      this._layout.getHeaderCols().forEach((hs) => {
        disableSelection(hs);
        onEvent(hs, "contextmenu", this.handleHeaderContextMenu.bind(this));
        onEvent(hs, "click", this.handleHeaderClick.bind(this));
        if (this._jQuery) {
          this._jQuery(hs).on("mouseenter", ".slick-header-column", this.handleHeaderMouseEnter.bind(this)).on("mouseleave", ".slick-header-column", this.handleHeaderMouseLeave.bind(this));
        } else {
          hs.addEventListener("mouseenter", (e) => e.target.closest(".slick-header-column") && this.handleHeaderMouseEnter(e));
          hs.addEventListener("mouseleave", (e) => e.target.closest(".slick-header-column") && this.handleHeaderMouseLeave(e));
        }
      });
      this._layout.getHeaderRowCols().forEach((el) => {
        onEvent(el.parentElement, "scroll", this.handleHeaderRowScroll);
      });
      this._layout.getFooterRowCols().forEach((el) => {
        onEvent(el.parentElement, "scroll", this.handleFooterRowScroll);
      });
      [this._focusSink1, this._focusSink2].forEach((fs) => onEvent(fs, "keydown", this.handleKeyDown.bind(this)));
      var canvases = Array.from(this.getCanvases());
      canvases.forEach((canvas) => {
        onEvent(canvas, "keydown", this.handleKeyDown.bind(this));
        onEvent(canvas, "click", this.handleClick.bind(this));
        onEvent(canvas, "dblclick", this.handleDblClick.bind(this));
        onEvent(canvas, "contextmenu", this.handleContextMenu.bind(this));
      });
      if (this._jQuery && this._jQuery.fn.drag) {
        this._jQuery(canvases).on("draginit", this.handleDragInit.bind(this)).on("dragstart", { distance: 3 }, this.handleDragStart.bind(this)).on("drag", this.handleDrag.bind(this)).on("dragend", this.handleDragEnd.bind(this));
      }
      canvases.forEach((canvas) => {
        if (this._jQuery) {
          this._jQuery(canvas).on("mouseenter", ".slick-cell", this.handleMouseEnter.bind(this)).on("mouseleave", ".slick-cell", this.handleMouseLeave.bind(this));
        } else {
          canvas.addEventListener("mouseenter", (e) => e.target.closest(".slick-cell") && this.handleMouseEnter(e));
          canvas.addEventListener("mouseleave", (e) => e.target.closest(".slick-cell") && this.handleMouseLeave(e));
        }
      });
      if (navigator.userAgent.toLowerCase().match(/webkit/) && navigator.userAgent.toLowerCase().match(/macintosh/) && this._jQuery) {
        this._jQuery(canvases).on("mousewheel", this.handleMouseWheel.bind(this));
      }
    }
    hasFrozenColumns() {
      return this._layout.getFrozenCols() > 0;
    }
    hasFrozenRows() {
      return this._layout.getFrozenRows() > 0;
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
      this.unregisterSelectionModel();
      this._selectionModel = model;
      if (this._selectionModel) {
        this._selectionModel.init(this);
        this._selectionModel.onSelectedRangesChanged.subscribe(this.handleSelectedRangesChanged);
      }
    }
    unregisterSelectionModel() {
      var _a, _b;
      if (!this._selectionModel)
        return;
      this._selectionModel.onSelectedRangesChanged.unsubscribe(this.handleSelectedRangesChanged);
      (_b = (_a = this._selectionModel).destroy) == null ? void 0 : _b.call(_a);
    }
    getScrollBarDimensions() {
      return this._scrollDims;
    }
    getDisplayedScrollbarDimensions() {
      return {
        width: this._viewportInfo.hasVScroll ? this._scrollDims.width : 0,
        height: this._viewportInfo.hasHScroll ? this._scrollDims.height : 0
      };
    }
    getAbsoluteColumnMinWidth() {
      return this._absoluteColMinWidth;
    }
    getSelectionModel() {
      return this._selectionModel;
    }
    colIdOrIdxToCell(columnIdOrIdx) {
      if (columnIdOrIdx == null)
        return null;
      if (typeof columnIdOrIdx !== "number")
        return this.getColumnIndex(columnIdOrIdx);
      return columnIdOrIdx;
    }
    getCanvasNode(columnIdOrIdx, row) {
      return this._layout.getCanvasNodeFor(this.colIdOrIdxToCell(columnIdOrIdx || 0), row || 0);
    }
    getCanvases() {
      var canvases = this._layout.getCanvasNodes();
      return this._jQuery ? this._jQuery(canvases) : canvases;
    }
    getActiveCanvasNode(e) {
      if (e) {
        this._activeCanvasNode = e.target.closest(".grid-canvas");
      }
      return this._activeCanvasNode;
    }
    getViewportNode(columnIdOrIdx, row) {
      return this._layout.getViewportNodeFor(this.colIdOrIdxToCell(columnIdOrIdx || 0), row || 0);
    }
    getViewports() {
      return this._layout.getViewportNodes();
    }
    getActiveViewportNode(e) {
      if (e) {
        this._activeViewportNode = e.target.closest(".slick-viewport");
      }
      return this._activeViewportNode;
    }
    getAvailableWidth() {
      return this._viewportInfo.hasVScroll ? this._viewportInfo.width - this._scrollDims.width : this._viewportInfo.width;
    }
    updateCanvasWidth(forceColumnWidthsUpdate) {
      const widthChanged = this._layout.updateCanvasWidth();
      if (widthChanged || forceColumnWidthsUpdate) {
        this._layout.applyColumnWidths();
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
      var header = this._layout.getHeaderColumn(idx);
      if (!header)
        return;
      if (title !== void 0) {
        columnDef.name = title;
      }
      if (toolTip !== void 0) {
        columnDef.toolTip = toolTip;
      }
      this.trigger(this.onBeforeHeaderCellDestroy, {
        node: header,
        column: columnDef
      });
      if (toolTip !== void 0)
        header.title = toolTip || "";
      if (title !== void 0) {
        var child = header.firstElementChild;
        if (columnDef.nameIsHtml)
          child && (child.innerHTML = title != null ? title : "");
        else
          child && (child.textContent = title != null ? title : "");
      }
      this.trigger(this.onHeaderCellRendered, {
        node: header,
        column: columnDef
      });
    }
    getHeader() {
      return this._layout.getHeaderCols()[0];
    }
    getHeaderColumn(columnIdOrIdx) {
      var cell = this.colIdOrIdxToCell(columnIdOrIdx);
      if (cell == null)
        return null;
      return this._layout.getHeaderColumn(cell);
    }
    getGroupingPanel() {
      return this._groupingPanel;
    }
    getPreHeaderPanel() {
      var _a;
      return (_a = this._groupingPanel) == null ? void 0 : _a.querySelector(".slick-preheader-panel");
    }
    getHeaderRow() {
      return this._layout.getHeaderRowCols()[0];
    }
    getHeaderRowColumn(columnIdOrIdx) {
      var cell = this.colIdOrIdxToCell(columnIdOrIdx);
      if (cell == null)
        return;
      return this._layout.getHeaderRowColumn(cell);
    }
    getFooterRow() {
      return this._layout.getFooterRowCols()[0];
    }
    getFooterRowColumn(columnIdOrIdx) {
      var cell = this.colIdOrIdxToCell(columnIdOrIdx);
      if (cell == null)
        return null;
      return this._layout.getFooterRowColumn(cell);
    }
    createColumnFooters() {
      var footerRowCols = this._layout.getFooterRowCols();
      footerRowCols.forEach((frc) => {
        frc.querySelectorAll(".slick-footerrow-column").forEach((el) => {
          var columnDef = this.getColumnFromNode(el);
          if (columnDef) {
            this.trigger(this.onBeforeFooterRowCellDestroy, {
              node: el,
              column: columnDef
            });
          }
        });
        if (this._jQuery) {
          this._jQuery(frc).empty();
        } else
          frc.innerHTML = "";
      });
      var cols = this._cols;
      for (var i = 0; i < cols.length; i++) {
        var m = cols[i];
        var footerRowCell = H("div", { class: "slick-footerrow-column l" + i + " r" + i + (this._options.useLegacyUI ? " ui-state-default" : "") });
        footerRowCell.dataset.c = i.toString();
        this._jQuery && this._jQuery(footerRowCell).data("column", m);
        if (m.footerCssClass)
          addClass(footerRowCell, m.footerCssClass);
        else if (m.cssClass)
          addClass(footerRowCell, m.cssClass);
        this._layout.getFooterRowColsFor(i).appendChild(footerRowCell);
        this.trigger(this.onFooterRowCellRendered, {
          node: footerRowCell,
          column: m
        });
      }
    }
    createColumnHeaders() {
      var _a, _b;
      const headerCols = this._layout.getHeaderCols();
      headerCols.forEach((hc) => {
        hc.querySelectorAll(".slick-header-column").forEach((el) => {
          var columnDef = this.getColumnFromNode(el);
          if (columnDef) {
            this.trigger(this.onBeforeHeaderCellDestroy, {
              node: el,
              column: columnDef
            });
          }
        });
        if (this._jQuery) {
          this._jQuery(hc).empty();
        } else {
          hc.innerHTML = "";
        }
      });
      this._layout.updateHeadersWidth();
      const headerRowCols = this._layout.getHeaderRowCols();
      headerRowCols.forEach((hrc) => {
        hrc.querySelectorAll(".slick-headerrow-column").forEach((el) => {
          var columnDef = this.getColumnFromNode(el);
          if (columnDef) {
            this.trigger(this.onBeforeHeaderRowCellDestroy, {
              node: el,
              column: columnDef,
              grid: this
            });
          }
        });
        if (this._jQuery) {
          this._jQuery(hrc).empty();
        } else {
          hrc.innerHTML = "";
        }
      });
      var cols = this._cols, frozenCols = this._layout.getFrozenCols();
      for (var i = 0; i < cols.length; i++) {
        var m = cols[i];
        var headerTarget = this._layout.getHeaderColsFor(i);
        var name = document.createElement("span");
        name.className = "slick-column-name";
        if (m.nameIsHtml)
          name.innerHTML = (_a = m.name) != null ? _a : "";
        else
          name.textContent = (_b = m.name) != null ? _b : "";
        var header = H("div", {
          class: "slick-header-column" + (this._options.useLegacyUI ? " ui-state-default " : ""),
          id: "" + this._uid + m.id,
          title: m.toolTip || "",
          style: "width: " + (m.width - this._headerColumnWidthDiff) + "px"
        }, name);
        header.dataset.c = i.toString();
        this._jQuery && this._jQuery(header).data("column", m);
        m.headerCssClass && addClass(header, m.headerCssClass);
        i < frozenCols && header.classList.add("frozen");
        headerTarget.appendChild(header);
        if ((this._options.enableColumnReorder || m.sortable) && this._options.useLegacyUI) {
          if (this._jQuery) {
            this._jQuery(header).on("mouseenter", addUiStateHover);
            this._jQuery(header).on("mouseleave", removeUiStateHover);
          } else {
            header.addEventListener("mouseenter", addUiStateHover);
            header.addEventListener("mouseleave", removeUiStateHover);
          }
        }
        if (m.sortable) {
          header.classList.add("slick-header-sortable");
          header.appendChild(H("span", { class: "slick-sort-indicator" }));
        }
        this.trigger(this.onHeaderCellRendered, {
          node: header,
          column: m
        });
        if (this._options.showHeaderRow) {
          var headerRowTarget = this._layout.getHeaderRowColsFor(i);
          var headerRowCell = H("div", { class: "slick-headerrow-column l" + i + " r" + i + (this._options.useLegacyUI ? " ui-state-default" : "") });
          headerRowCell.dataset.c = i.toString();
          this._jQuery && this._jQuery(headerRowCell).data("column", m);
          headerRowTarget.appendChild(headerRowCell);
          this.trigger(this.onHeaderRowCellRendered, {
            node: headerRowCell,
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
      const handler = (e) => {
        var tgt = e.target;
        if (tgt.classList.contains("slick-resizable-handle")) {
          return;
        }
        var colNode = tgt.closest(".slick-header-column");
        if (!colNode) {
          return;
        }
        var column = this.getColumnFromNode(colNode);
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
      };
      this._layout.getHeaderCols().forEach((el) => {
        if (this._jQuery)
          this._jQuery(el).on("click", handler);
        else
          el.addEventListener("click", handler);
      });
    }
    setupColumnReorder() {
      var _a;
      const jQuerySortable = this._jQuery && ((_a = this._jQuery.fn) == null ? void 0 : _a.sortable);
      if (jQuerySortable)
        this._jQuery(this._layout.getHeaderCols()).filter(":ui-sortable").sortable("destroy");
      var columnScrollTimer = null;
      var scrollColumnsRight = () => {
        this._layout.getScrollContainerX().scrollLeft = this._layout.getScrollContainerX().scrollLeft + 10;
      };
      var scrollColumnsLeft = () => {
        this._layout.getScrollContainerX().scrollLeft = this._layout.getScrollContainerX().scrollLeft - 10;
      };
      var canDragScroll;
      var hasGrouping = this._options.groupingPanel;
      jQuerySortable && this._jQuery([this._layout.getHeaderCols()]).sortable({
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
          ui.placeholder.outerHeight(ui.helper.outerHeight());
          ui.placeholder.outerWidth(ui.helper.outerWidth());
          canDragScroll = !this.hasFrozenColumns() || ui.placeholder.offset()[this._options.rtl ? "right" : "left"] + Math.round(ui.placeholder.width()) > this._jQuery(this._layout.getScrollContainerX()).offset()[this._options.rtl ? "right" : "left"];
          this._jQuery(ui.helper).addClass("slick-header-column-active");
        },
        beforeStop: (_, ui) => {
          this._jQuery(ui.helper).removeClass("slick-header-column-active");
          if (hasGrouping) {
            var headerDraggableGroupBy = this._jQuery(this.getGroupingPanel());
            var hasDroppedColumn = headerDraggableGroupBy.find(".slick-dropped-grouping").length;
            if (hasDroppedColumn > 0) {
              headerDraggableGroupBy.find(".slick-dropped-placeholder").hide();
              headerDraggableGroupBy.find(".slick-dropped-grouping").show();
            }
          }
        },
        sort: (e) => {
          if (canDragScroll && e.originalEvent.pageX > this._container.clientWidth) {
            if (!columnScrollTimer) {
              columnScrollTimer = setInterval(
                scrollColumnsRight,
                100
              );
            }
          } else if (canDragScroll && e.originalEvent.pageX < this._jQuery(this._layout.getScrollContainerX()).offset().left) {
            if (!columnScrollTimer) {
              columnScrollTimer = setInterval(
                scrollColumnsLeft,
                100
              );
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
            this._jQuery(e.target).sortable("cancel");
            return;
          }
          var reorderedCols;
          this._layout.getHeaderCols().forEach((el) => reorderedCols = sortToDesiredOrderAndKeepRest(
            this._initCols,
            this._jQuery(el).sortable("toArray").map((x) => x.substring(this._uid.length))
          ));
          this.setColumns(reorderedCols);
          this.trigger(this.onColumnsReordered, {});
          e.stopPropagation();
          this.setupColumnResize();
        }
      });
    }
    setupColumnResize() {
      var minPageX, pageX, maxPageX, cols = this._cols;
      var columnElements = [];
      this._layout.getHeaderCols().forEach((el) => {
        columnElements = columnElements.concat(Array.from(el.children));
      });
      var j, c, pageX, minPageX, maxPageX, firstResizable, lastResizable, cols = this._cols;
      var firstResizable, lastResizable;
      columnElements.forEach((el, i) => {
        var _a;
        (_a = el.querySelector(".slick-resizable-handle")) == null ? void 0 : _a.remove();
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
      const noJQueryDrag = !this._jQuery || !this._jQuery.fn || !this._jQuery.fn.drag;
      columnElements.forEach((el, colIdx) => {
        if (colIdx < firstResizable || this._options.forceFitColumns && colIdx >= lastResizable) {
          return;
        }
        const handle = el.appendChild(document.createElement("div"));
        handle.classList.add("slick-resizable-handle");
        handle.draggable = true;
        var docDragOver = null;
        const dragStart = (e) => {
          var _a;
          if (!this.getEditorLock().commitCurrentEdit()) {
            !noJQueryDrag && e.preventDefault();
            return;
          }
          if (noJQueryDrag) {
            docDragOver = (z) => z.preventDefault();
            document.addEventListener("dragover", docDragOver);
          }
          pageX = e.pageX;
          (_a = e.target.parentElement) == null ? void 0 : _a.classList.add("slick-header-column-active");
          columnElements.forEach((e2, z) => {
            cols[z].previousWidth = e2.offsetWidth;
          });
          const minMax = calcMinMaxPageXOnDragStart(cols, colIdx, pageX, this._options.forceFitColumns, this._absoluteColMinWidth);
          maxPageX = minMax.maxPageX;
          minPageX = minMax.minPageX;
          noJQueryDrag && (e.dataTransfer.effectAllowed = "move");
        };
        const drag = (e) => {
          if (noJQueryDrag) {
            if (!e.pageX && !e.clientX && !e.pageY && !e.clientY)
              return;
            e.dataTransfer.effectAllowed = "none";
            e.preventDefault();
          }
          shrinkOrStretchColumn(cols, colIdx, Math.min(maxPageX, Math.max(minPageX, e.pageX)) - pageX, this._options.forceFitColumns, this._absoluteColMinWidth);
          this._layout.afterHeaderColumnDrag();
          this.applyColumnHeaderWidths();
          if (this._options.syncColumnCellResize) {
            this._layout.applyColumnWidths();
          }
        };
        const dragEnd = (e) => {
          var _a;
          if (docDragOver) {
            document.removeEventListener("dragover", docDragOver);
            docDragOver = null;
          }
          (_a = e.target.parentElement) == null ? void 0 : _a.classList.remove("slick-header-column-active");
          for (j = 0; j < columnElements.length; j++) {
            c = cols[j];
            var newWidth = columnElements[j].offsetWidth;
            if (c.previousWidth !== newWidth && c.rerenderOnResize) {
              this.invalidateAllRows();
            }
          }
          this.updateCanvasWidth(true);
          this.render();
          this.trigger(this.onColumnsResized);
        };
        if (noJQueryDrag) {
          handle.addEventListener("dragstart", dragStart);
          handle.addEventListener("drag", drag);
          handle.addEventListener("dragend", dragEnd);
          handle.addEventListener("dragover", (e) => {
            e.preventDefault();
            e.dataTransfer.effectAllowed = "move";
          });
        } else {
          this._jQuery(handle).on("dragstart", dragStart).on("drag", drag).on("dragend", dragEnd);
        }
      });
    }
    setOverflow() {
      this._layout.setOverflow();
      if (this._options.viewportClass)
        this.getViewports().forEach((vp) => addClass(vp, this._options.viewportClass));
    }
    measureCellPaddingAndBorder() {
      const h = ["border-left-width", "border-right-width", "padding-left", "padding-right"];
      const v = ["border-top-width", "border-bottom-width", "padding-top", "padding-bottom"];
      var el = this._layout.getHeaderColsFor(0).appendChild(H("div", { class: "slick-header-column" + (this._options.useLegacyUI ? " ui-state-default" : ""), style: "visibility:hidden" }));
      this._headerColumnWidthDiff = 0;
      var cs = getComputedStyle(el);
      if (cs.boxSizing != "border-box")
        h.forEach((val) => this._headerColumnWidthDiff += parsePx(cs.getPropertyValue(val)) || 0);
      el.remove();
      var r = this._layout.getCanvasNodeFor(0, 0).appendChild(H(
        "div",
        { class: "slick-row" },
        el = H("div", { class: "slick-cell", id: "", style: "visibility: hidden" })
      ));
      el.innerHTML = "-";
      this._cellWidthDiff = this._cellHeightDiff = 0;
      cs = getComputedStyle(el);
      if (cs.boxSizing != "border-box") {
        h.forEach((val) => this._cellWidthDiff += parsePx(cs.getPropertyValue(val)) || 0);
        v.forEach((val) => this._cellHeightDiff += parsePx(cs.getPropertyValue(val)) || 0);
      }
      r.remove();
      this._absoluteColMinWidth = Math.max(this._headerColumnWidthDiff, this._cellWidthDiff);
    }
    createCssRules() {
      var cellHeight = this._options.rowHeight - this._cellHeightDiff;
      if (this._options.useCssVars && this.getColumns().length > 50)
        this._options.useCssVars = false;
      this._container.classList.toggle("sleek-vars", !!this._options.useCssVars);
      if (this._options.useCssVars) {
        var style = this._container.style;
        style.setProperty("--sleek-row-height", this._options.rowHeight + "px");
        style.setProperty("--sleek-cell-height", cellHeight + "px");
        style.setProperty("--sleek-top-panel-height", this._options.topPanelHeight + "px");
        style.setProperty("--sleek-grouping-panel-height", this._options.groupingPanelHeight + "px");
        style.setProperty("--sleek-headerrow-height", this._options.headerRowHeight + "px");
        style.setProperty("--sleek-footerrow-height", this._options.footerRowHeight + "px");
        return;
      }
      var el = this._styleNode = document.createElement("style");
      el.dataset.uid = this._uid;
      var rules = [
        "." + this._uid + " { --slick-cell-height: " + this._options.rowHeight + "px; }",
        "." + this._uid + " .slick-group-header-column { " + (this._options.rtl ? "right" : "left") + ": 1000px; }",
        "." + this._uid + " .slick-header-column { " + (this._options.rtl ? "right" : "left") + ": 1000px; }",
        "." + this._uid + " .slick-top-panel { height:" + this._options.topPanelHeight + "px; }",
        "." + this._uid + " .slick-grouping-panel { height:" + this._options.groupingPanelHeight + "px; }",
        "." + this._uid + " .slick-headerrow-columns { height:" + this._options.headerRowHeight + "px; }",
        "." + this._uid + " .slick-cell { height:" + cellHeight + "px; }",
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
      if (this._options.useCssVars)
        return null;
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
      return this._options.rtl ? {
        "right": this._columnCssRulesL[idx],
        "left": this._columnCssRulesR[idx]
      } : {
        "left": this._columnCssRulesL[idx],
        "right": this._columnCssRulesR[idx]
      };
    }
    removeCssRules() {
      var _a;
      (_a = this._styleNode) == null ? void 0 : _a.remove();
      this._styleNode = null;
      this._stylesheet = null;
    }
    destroy() {
      var _a;
      this.getEditorLock().cancelCurrentEdit();
      this.trigger(this.onBeforeDestroy);
      var i = this._plugins.length;
      while (i--) {
        this.unregisterPlugin(this._plugins[i]);
      }
      if (this._options.enableColumnReorder && this._jQuery && this._jQuery.fn.sortable) {
        this._jQuery(this._layout.getHeaderCols()).filter(":ui-sortable").sortable("destroy");
      }
      this.unbindAncestorScrollEvents();
      this.unbindFromData();
      this.unregisterSelectionModel();
      (_a = this._jQuery) == null ? void 0 : _a.call(this, this._container).off(".slickgrid");
      this.removeCssRules();
      var canvasNodes = this._layout.getCanvasNodes();
      if (this._jQuery)
        this._jQuery(canvasNodes).off("draginit dragstart dragend drag");
      else
        canvasNodes.forEach((el) => el.remove());
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
    //////////////////////////////////////////////////////////////////////////////////////////////
    // General
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
      var vpi = this._viewportInfo, availWidth = vpi.hasVScroll ? vpi.width - this._scrollDims.width : vpi.width;
      var reRender = autosizeColumns(this._cols, availWidth, this._absoluteColMinWidth);
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
      var h;
      for (var i = 0, cols = this._cols, colCount = cols.length, diff = this._headerColumnWidthDiff; i < colCount; i++) {
        h = this._layout.getHeaderColumn(i);
        if (h) {
          var target = cols[i].width - diff;
          if (h.offsetWidth !== target) {
            h.style.width = target + "px";
          }
        }
      }
      this.updateViewColLeftRight();
    }
    setSortColumn(columnId, ascending) {
      this.setSortColumns([{ columnId, sortAsc: ascending }]);
    }
    setSortColumns(cols) {
      this._sortColumns = cols || [];
      var headerColumnEls = [];
      this._layout.getHeaderCols().forEach((el) => headerColumnEls = headerColumnEls.concat(Array.from(el.children)));
      headerColumnEls.forEach((hel) => {
        hel.classList.remove("slick-header-column-sorted");
        var si = hel.querySelector(".slick-sort-indicator");
        si && si.classList.remove("slick-sort-indicator-asc", "slick-sort-indicator-desc");
      });
      this._sortColumns.forEach((col) => {
        if (col.sortAsc == null) {
          col.sortAsc = true;
        }
        var columnIndex = this.getColumnIndex(col.columnId);
        if (columnIndex != null) {
          var header = headerColumnEls[columnIndex];
          if (header) {
            header.classList.add("slick-header-column-sorted");
            var si = header.querySelector(".slick-sort-indicator");
            si && si.classList.add(col.sortAsc ? "slick-sort-indicator-asc" : "slick-sort-indicator-desc");
          }
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
      var x = 0, r, cols = this._cols, i, l = cols.length, frozenCols = this._layout.getFrozenCols();
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
      initializeColumns(initCols, this._colDefaults);
      var initColById = {};
      var viewCols = [];
      var viewColById = {};
      var i, m;
      for (i = 0; i < initCols.length; i++) {
        m = initCols[i];
        initColById[m.id] = i;
        if (m.visible !== false)
          viewCols.push(m);
      }
      viewCols = this._layout.reorderViewColumns(viewCols, this._options);
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
      var _a, _b;
      this.setInitialCols(columns);
      this.updateViewColLeftRight();
      if (this._initialized) {
        this._layout.setPaneVisibility();
        this.setOverflow();
        this.invalidateAllRows();
        this.createColumnHeaders();
        this.createColumnFooters();
        this.updateGrandTotals();
        this.removeCssRules();
        this.createCssRules();
        this.resizeCanvas();
        this.updateCanvasWidth();
        this._layout.applyColumnWidths();
        this.handleScroll();
        (_b = (_a = this.getSelectionModel()) == null ? void 0 : _a.refreshSelections) == null ? void 0 : _b.call(_a);
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
      if (args.groupingPanel && !this._options.groupingPanel)
        this.createGroupingPanel();
      else if (args.groupingPanel != void 0 && !args.groupingPanel && this._groupingPanel)
        this._groupingPanel.remove();
      if (args.showColumnHeader !== void 0) {
        this.setColumnHeaderVisibility(args.showColumnHeader);
      }
      if (this._options.enableAddRow !== args.enableAddRow) {
        this.invalidateRow(this.getDataLength());
      }
      this._options = Object.assign(this._options, args);
      this.validateAndEnforceOptions();
      this._layout.afterSetOptions(args);
      if (args.columns && !suppressColumnSet) {
        this.setColumns((_a = args.columns) != null ? _a : this._initCols);
      }
      if (!suppressSetOverflow) {
        this.setOverflow();
      }
      this._layout.setScroller();
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
      return this._layout.getTopPanelFor(0);
    }
    setTopPanelVisibility(visible) {
      if (this._options.showTopPanel != visible) {
        this._options.showTopPanel = !!visible;
        this._layout.getTopPanelNodes().forEach((el) => {
          if (this._jQuery)
            this._jQuery(el)[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas);
          else {
            el.style.display = visible ? "" : "none";
            this.resizeCanvas();
          }
        });
      }
    }
    setColumnHeaderVisibility(visible, animate) {
      if (this._options.showColumnHeader != visible) {
        this._options.showColumnHeader = visible;
        this._layout.getHeaderCols().forEach((n) => {
          const el = n.parentElement;
          if (animate && this._jQuery)
            this._jQuery(el)[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas);
          else {
            el.style.display = visible ? "" : "none";
            this.resizeCanvas();
          }
        });
      }
    }
    setFooterRowVisibility(visible) {
      if (this._options.showFooterRow != visible) {
        this._options.showFooterRow = !!visible;
        this._layout.getFooterRowCols().forEach((n) => {
          const el = n.parentElement;
          if (this._jQuery)
            this._jQuery(el)[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas);
          else {
            el.style.display = visible ? "" : "none";
            this.resizeCanvas();
          }
        });
      }
    }
    setGroupingPanelVisibility(visible) {
      if (this._options.showGroupingPanel != visible) {
        this._options.showGroupingPanel = visible;
        if (!this._options.groupingPanel)
          return;
        const el = this._groupingPanel;
        if (this._jQuery)
          this._jQuery(el)[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas);
        else {
          el.style.display = visible ? "" : "none";
          this.resizeCanvas();
        }
      }
    }
    setPreHeaderPanelVisibility(visible) {
      this.setGroupingPanelVisibility(visible);
    }
    setHeaderRowVisibility(visible) {
      if (this._options.showHeaderRow != visible) {
        this._options.showHeaderRow = visible;
        this._layout.getHeaderRowCols().forEach((n) => {
          const el = n.parentElement;
          if (this._jQuery)
            this._jQuery(el)[visible ? "slideDown" : "slideUp"]("fast", this.resizeCanvas);
          else {
            el.style.display = visible ? "" : "none";
            this.resizeCanvas();
          }
        });
      }
    }
    getContainerNode() {
      return this._container;
    }
    getUID() {
      return this._uid;
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    // Rendering / Scrolling
    getRowTop(row) {
      return this._options.rowHeight * row - this._pageOffset;
    }
    getRowFromPosition(y) {
      return Math.floor((y + this._pageOffset) / this._options.rowHeight);
    }
    scrollTo(y) {
      const vpi = this._viewportInfo;
      y = Math.max(y, 0);
      y = Math.min(y, vpi.virtualHeight - Math.round(this._jQuery(this._layout.getScrollContainerY()).height()) + (vpi.hasHScroll || this.hasFrozenColumns() ? this._scrollDims.height : 0));
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
        this._layout.handleScrollV();
        this._layout.getScrollContainerY().scrollTop = newScrollTop;
        this.trigger(this.onViewportChanged);
      }
    }
    getFormatter(row, column) {
      var data = this._data;
      if (data.getItemMetadata) {
        const itemMetadata = data.getItemMetadata(row);
        if (itemMetadata) {
          const colsMetadata = itemMetadata.columns;
          if (colsMetadata) {
            var columnMetadata = colsMetadata[column.id] || colsMetadata[this.getColumnIndex(column.id)];
            if (columnMetadata) {
              if (columnMetadata.format)
                return columnMetadata.format;
              if (columnMetadata.formatter)
                return convertCompatFormatter(columnMetadata.formatter);
            }
          }
          if (itemMetadata.format)
            return itemMetadata.format;
          if (itemMetadata.formatter)
            return convertCompatFormatter(itemMetadata.formatter);
        }
      }
      if (column.format)
        return column.format;
      if (column.formatter)
        return convertCompatFormatter(column.formatter);
      var opt = this._options;
      var factory = opt.formatterFactory;
      if (factory) {
        if (factory.getFormat) {
          var format = factory.getFormat(column);
          if (format)
            return format;
        } else if (factory.getFormatter) {
          var compat = factory.getFormatter(column);
          if (compat)
            return convertCompatFormatter(compat);
        }
      }
      if (opt.defaultFormat)
        return opt.defaultFormat;
      if (opt.defaultFormatter)
        return convertCompatFormatter(opt.defaultFormatter);
      return defaultColumnFormat;
    }
    getFormatterContext(row, cell) {
      var column = this._cols[cell];
      var item = this.getDataItem(row);
      const ctx = {
        cell,
        column,
        grid: this,
        escape,
        item,
        row
      };
      if (item)
        ctx.value = this.getDataItemValueForColumn(item, column);
      return ctx;
    }
    getEditor(row, cell) {
      var column = this._cols[cell];
      var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      var colsMetadata = itemMetadata && itemMetadata.columns;
      if (colsMetadata && colsMetadata[column.id] && colsMetadata[column.id].editor !== void 0) {
        return colsMetadata[column.id].editor;
      }
      if (colsMetadata && colsMetadata[cell] && colsMetadata[cell].editor !== void 0) {
        return colsMetadata[cell].editor;
      }
      return column.editor || this._options.editorFactory && this._options.editorFactory.getEditor(column, row);
    }
    getDataItemValueForColumn(item, columnDef) {
      if (this._options.dataItemColumnValueExtractor)
        return this._options.dataItemColumnValueExtractor(item, columnDef);
      return item[columnDef.field];
    }
    appendRowHtml(stringArrayL, stringArrayR, row, range, dataLength) {
      var _a, _b;
      var d = this.getDataItem(row);
      var dataLoading = row < dataLength && !d;
      var rowCss = "slick-row" + (this._layout.isFrozenRow(row) ? " frozen" : "") + (dataLoading ? " loading" : "") + (row === this._activeRow ? " active" : "") + (row % 2 == 1 ? " odd" : " even");
      if (!d) {
        rowCss += " " + this._options.addNewRowCssClass;
      }
      var itemMetadata = (_b = (_a = this._data).getItemMetadata) == null ? void 0 : _b.call(_a, row);
      if (itemMetadata && itemMetadata.cssClasses) {
        rowCss += " " + itemMetadata.cssClasses;
      }
      var rowOffset = this._layout.getFrozenRowOffset(row);
      var rowHtml = "<div class='" + (this._options.useLegacyUI ? "ui-widget-content " : "") + rowCss + "' style='top:" + (this.getRowTop(row) - rowOffset) + "px'>";
      stringArrayL.push(rowHtml);
      const frozenCols = this._layout.getFrozenCols();
      if (frozenCols) {
        stringArrayR.push(rowHtml);
      }
      var colspan, m, cols = this._cols;
      for (var i = 0, ii = cols.length; i < ii; i++) {
        var columnData = null;
        m = cols[i];
        colspan = 1;
        if (itemMetadata && itemMetadata.columns) {
          columnData = itemMetadata.columns[m.id] || itemMetadata.columns[i];
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
      if (frozenCols) {
        stringArrayR.push("</div>");
      }
    }
    appendCellHtml(sb, row, cell, colspan, item, metadata) {
      var _a, _b, _c, _d, _e;
      var cols = this._cols, frozenCols = this._layout.getFrozenCols(), column = cols[cell];
      var klass = "slick-cell l" + cell + " r" + Math.min(cols.length - 1, cell + colspan - 1) + (column.cssClass ? " " + column.cssClass : "");
      if (cell < frozenCols)
        klass += " frozen";
      if (row === this._activeRow && cell === this._activeCell)
        klass += " active";
      if (metadata && metadata.cssClasses) {
        klass += " " + metadata.cssClasses;
      }
      for (var key in this._cellCssClasses) {
        if (this._cellCssClasses[key][row] && this._cellCssClasses[key][row][column.id]) {
          klass += " " + this._cellCssClasses[key][row][column.id];
        }
      }
      var html;
      const ctx = {
        cell,
        column,
        escape,
        grid: this,
        item,
        row
      };
      if (item) {
        ctx.value = this.getDataItemValueForColumn(item, column);
        html = this.getFormatter(row, column)(ctx);
      }
      klass = escape(klass);
      if (((_a = ctx.addClass) == null ? void 0 : _a.length) || ((_b = ctx.addAttrs) == null ? void 0 : _b.length) || ((_c = ctx.tooltip) == null ? void 0 : _c.length)) {
        if ((_d = ctx.addClass) == null ? void 0 : _d.length)
          klass += " " + escape(ctx.addClass);
        sb.push('<div class="' + klass + '"');
        if ((_e = ctx.addClass) == null ? void 0 : _e.length)
          sb.push(' data-fmtcls="' + escape(ctx.addClass) + '"');
        var attrs = ctx.addAttrs;
        if (attrs != null) {
          var ks = [];
          for (var k in attrs) {
            sb.push(k + '="' + escape(attrs[k]) + '"');
            ks.push(k);
          }
          sb.push(' data-fmtatt="' + escape(ks.join(",")) + '"');
        }
        var toolTip = ctx.tooltip;
        if (toolTip != null && toolTip.length)
          sb.push('tooltip="' + escape(toolTip) + '"');
        if (html != null)
          sb.push(">" + html + "</div>");
        else
          sb.push("></div>");
      } else if (html != null)
        sb.push('<div class="' + klass + '">' + html + "</div>");
      else
        sb.push('<div class="' + klass + '"></div>');
      this._rowsCache[row].cellRenderQueue.push(cell);
      this._rowsCache[row].cellColSpans[cell] = colspan;
    }
    cleanupRows(rangeToKeep) {
      var i;
      for (var x in this._rowsCache) {
        i = parseInt(x, 10);
        if (i !== this._activeRow && (i < rangeToKeep.top || i > rangeToKeep.bottom) && !this._layout.isFrozenRow(i))
          this.removeRowFromCache(i);
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
    }
    invalidate() {
      this.updateRowCount();
      this.invalidateAllRows();
      this.render();
      this.updateGrandTotals();
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
      var _a, _b;
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
        rowNodeL: cacheEntry.rowNodeL,
        rowNodeR: cacheEntry.rowNodeR
      });
      (_a = cacheEntry.rowNodeL) == null ? void 0 : _a.remove();
      (_b = cacheEntry.rowNodeR) == null ? void 0 : _b.remove();
    }
    queuePostProcessedCellForCleanup(cellnode, columnIdx, rowIdx) {
      this._postProcessCleanupQueue.push({
        groupId: this._postProcessGroupId,
        cellNode: cellnode,
        columnIdx,
        rowIdx
      });
      this._jQuery(cellnode).detach();
    }
    removeRowFromCache(row) {
      var _a, _b, _c, _d;
      var cacheEntry = this._rowsCache[row];
      if (!cacheEntry) {
        return;
      }
      if (this._options.enableAsyncPostRenderCleanup && this._postProcessedRows[row]) {
        this.queuePostProcessedRowForCleanup(cacheEntry, this._postProcessedRows[row], row);
      } else {
        (_b = (_a = cacheEntry.rowNodeL) == null ? void 0 : _a.parentElement) == null ? void 0 : _b.removeChild(cacheEntry.rowNodeL);
        (_d = (_c = cacheEntry.rowNodeR) == null ? void 0 : _c.parentElement) == null ? void 0 : _d.removeChild(cacheEntry.rowNodeR);
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
    updateCell(row, cell) {
      var cellNode = this.getCellNode(row, cell);
      if (!cellNode)
        return;
      if (this._currentEditor && this._activeRow === row && this._activeCell === cell) {
        this._currentEditor.loadValue(this.getDataItem(row));
      } else {
        this.updateCellWithFormatter(cellNode, row, cell);
        this.invalidatePostProcessingResults(row);
      }
    }
    updateCellWithFormatter(cellNode, row, cell) {
      var html;
      const ctx = this.getFormatterContext(row, cell);
      if (ctx.item)
        html = this.getFormatter(row, ctx.column)(ctx);
      applyFormatterResultToCellNode(ctx, html, cellNode);
    }
    updateRow(row) {
      var cacheEntry = this._rowsCache[row];
      if (!cacheEntry) {
        return;
      }
      this.ensureCellNodesInRowsCache(row);
      var d = this.getDataItem(row);
      for (var x in cacheEntry.cellNodesByColumnIdx) {
        if (!cacheEntry.cellNodesByColumnIdx.hasOwnProperty(x)) {
          continue;
        }
        var cell = parseInt(x, 10);
        if (row === this._activeRow && cell === this._activeCell && this._currentEditor) {
          this._currentEditor.loadValue(d);
        } else {
          this.updateCellWithFormatter(cacheEntry.cellNodesByColumnIdx[cell], row, cell);
        }
      }
      this.invalidatePostProcessingResults(row);
    }
    calcViewportSize() {
      const layout = this._layout;
      const vs = this._viewportInfo;
      vs.width = getInnerWidth(this._container);
      vs.groupingPanelHeight = this._options.groupingPanel && this._options.showGroupingPanel ? this._options.groupingPanelHeight + getVBoxDelta(this._groupingPanel) : 0;
      vs.topPanelHeight = this._options.showTopPanel ? this._options.topPanelHeight + getVBoxDelta(layout.getTopPanelFor(0).parentElement) : 0;
      vs.headerRowHeight = this._options.showHeaderRow ? this._options.headerRowHeight + getVBoxDelta(layout.getHeaderRowColsFor(0).parentElement) : 0;
      vs.footerRowHeight = this._options.showFooterRow ? this._options.footerRowHeight + getVBoxDelta(layout.getFooterRowColsFor(0).parentElement) : 0;
      vs.headerHeight = this._options.showColumnHeader ? parsePx(getComputedStyle(layout.getHeaderColsFor(0).parentElement).height) + getVBoxDelta(layout.getHeaderColsFor(0).parentElement) : 0;
      if (this._options.autoHeight) {
        vs.height = this._options.rowHeight * this.getDataLengthIncludingAddNew();
        if (this._layout.calcCanvasWidth() > vs.width)
          vs.height += this._scrollDims.height;
      } else {
        var style = getComputedStyle(this._container);
        vs.height = parsePx(style.height) - parsePx(style.paddingTop) - parsePx(style.paddingBottom) - vs.headerHeight - vs.topPanelHeight - vs.headerRowHeight - vs.footerRowHeight - vs.groupingPanelHeight;
      }
      vs.numVisibleRows = Math.ceil(vs.height / this._options.rowHeight);
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
      var scrollCanvas = this._layout.getScrollCanvasY();
      var oldH = Math.round(parsePx(getComputedStyle(scrollCanvas).height));
      var numberOfRows;
      const frozenRows = this._layout.getFrozenRows();
      if (frozenRows) {
        numberOfRows = this.getDataLength() - frozenRows;
      } else {
        numberOfRows = dataLengthIncludingAddNew + (this._options.leaveSpaceForNewRows ? this._viewportInfo.numVisibleRows - 1 : 0);
      }
      var tempViewportH = Math.round(parsePx(getComputedStyle(this._layout.getScrollContainerY()).height));
      const vpi = this._viewportInfo;
      var oldViewportHasVScroll = vpi.hasVScroll;
      vpi.hasVScroll = !this._options.autoHeight && numberOfRows * this._options.rowHeight > tempViewportH;
      this.makeActiveCellNormal();
      var l = dataLengthIncludingAddNew - 1;
      for (var x in this._rowsCache) {
        var i = parseInt(x, 10);
        if (i >= l) {
          this.removeRowFromCache(i);
        }
      }
      this._options.enableAsyncPostRenderCleanup && this.startPostProcessingCleanup();
      vpi.virtualHeight = Math.max(this._options.rowHeight * numberOfRows, tempViewportH - this._scrollDims.height);
      if (this._activeCellNode && this._activeRow > l) {
        this.resetActiveCell();
      }
      if (vpi.virtualHeight < getMaxSupportedCssHeight()) {
        vpi.realScrollHeight = this._pageHeight = vpi.virtualHeight;
        this._numberOfPages = 1;
        this._jumpinessCoefficient = 0;
      } else {
        vpi.realScrollHeight = getMaxSupportedCssHeight();
        this._pageHeight = vpi.realScrollHeight / 100;
        this._numberOfPages = Math.floor(vpi.virtualHeight / this._pageHeight);
        this._jumpinessCoefficient = (vpi.virtualHeight - vpi.realScrollHeight) / (this._numberOfPages - 1);
      }
      if (vpi.realScrollHeight !== oldH) {
        this._layout.realScrollHeightChange();
        this._scrollTop = this._layout.getScrollContainerY().scrollTop;
      }
      var oldScrollTopInRange = this._scrollTop + this._pageOffset <= vpi.virtualHeight - tempViewportH;
      if (vpi.virtualHeight == 0 || this._scrollTop == 0) {
        this._page = this._pageOffset = 0;
      } else if (oldScrollTopInRange) {
        this.scrollTo(this._scrollTop + this._pageOffset);
      } else {
        this.scrollTo(vpi.virtualHeight - tempViewportH);
      }
      if (vpi.realScrollHeight != oldH && this._options.autoHeight) {
        this.resizeCanvas();
      }
      if (this._options.forceFitColumns && oldViewportHasVScroll != vpi.hasVScroll) {
        this.autosizeColumns();
      }
      this.updateCanvasWidth(false);
    }
    /**
     * @param viewportTop optional viewport top
     * @param viewportLeft optional viewport left
     * @returns viewport range
     */
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
      if (this._options.rtl) {
        viewportLeft = Math.abs(viewportLeft);
      }
      return {
        top: this.getRowFromPosition(viewportTop),
        bottom: this.getRowFromPosition(viewportTop + this._viewportInfo.height) + 1,
        leftPx: viewportLeft,
        rightPx: viewportLeft + this._viewportInfo.width
      };
    }
    getRenderedRange(viewportTop, viewportLeft) {
      var range = this.getVisibleRange(viewportTop, viewportLeft);
      var buffer = Math.round(this._viewportInfo.height / this._options.rowHeight);
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
        range.rightPx = this._layout.getCanvasWidth();
      } else {
        range.leftPx -= this._viewportInfo.width;
        range.rightPx += this._viewportInfo.width;
        range.leftPx = Math.max(0, range.leftPx);
        range.rightPx = Math.min(this._layout.getCanvasWidth(), range.rightPx);
      }
      return range;
    }
    ensureCellNodesInRowsCache(row) {
      var _a, _b, _c, _d;
      var cacheEntry = this._rowsCache[row];
      if (cacheEntry) {
        if (cacheEntry.cellRenderQueue.length) {
          var lastChild = (_c = (_a = cacheEntry.rowNodeR) == null ? void 0 : _a.lastElementChild) != null ? _c : (_b = cacheEntry.rowNodeL) == null ? void 0 : _b.lastElementChild;
          while (lastChild && cacheEntry.cellRenderQueue.length) {
            var columnIdx = cacheEntry.cellRenderQueue.pop();
            cacheEntry.cellNodesByColumnIdx[columnIdx] = lastChild;
            lastChild = lastChild.previousElementSibling;
            if (lastChild == null)
              lastChild = (_d = cacheEntry.rowNodeL) == null ? void 0 : _d.lastElementChild;
          }
        }
      }
    }
    cleanUpCells(range, row) {
      if (this._layout.isFrozenRow(row))
        return;
      var cacheEntry = this._rowsCache[row];
      var cellsToRemove = [], frozenCols = this._layout.getFrozenCols();
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
      }
    }
    cleanUpAndRenderCells(range) {
      var cacheEntry;
      var stringArray = [];
      var processedRows = [];
      var cellsAdded;
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
        var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
        var colsMetadata = itemMetadata && itemMetadata.columns;
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
          if (colsMetadata) {
            columnData = colsMetadata[cols[i].id] || colsMetadata[i];
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
          processedRows.push(row);
        }
      }
      if (!stringArray.length) {
        return;
      }
      var x = document.createElement("div");
      x.innerHTML = stringArray.join("");
      var processedRow;
      var node, frozenCols = this._layout.getFrozenCols();
      while ((processedRow = processedRows.pop()) != null) {
        cacheEntry = this._rowsCache[processedRow];
        var columnIdx;
        while ((columnIdx = cacheEntry.cellRenderQueue.pop()) != null) {
          node = x.lastElementChild;
          if (frozenCols > 0 && columnIdx >= frozenCols) {
            cacheEntry.rowNodeR.appendChild(node);
          } else {
            cacheEntry.rowNodeL.appendChild(node);
          }
          cacheEntry.cellNodesByColumnIdx[columnIdx] = node;
        }
      }
    }
    renderRows(range) {
      var stringArrayL = [], stringArrayR = [], rows = [], needToReselectCell = false, dataLength = this.getDataLength();
      for (var i = range.top, ii = range.bottom; i <= ii; i++) {
        if (this._rowsCache[i] || this.hasFrozenRows() && this._options.frozenBottom && i == dataLength) {
          continue;
        }
        rows.push(i);
        this._rowsCache[i] = {
          rowNodeL: null,
          rowNodeR: null,
          // ColSpans of rendered cells (by column idx).
          // Can also be used for checking whether a cell has been rendered.
          cellColSpans: [],
          // Cell nodes (by column idx).  Lazy-populated by ensureCellNodesInRowsCache().
          cellNodesByColumnIdx: [],
          // Column indices of cell nodes that have been rendered, but not yet indexed in
          // cellNodesByColumnIdx.  These are in the same order as cell nodes added at the
          // end of the row.
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
      var l = document.createElement("div"), r = document.createElement("div");
      l.innerHTML = stringArrayL.join("");
      r.innerHTML = stringArrayR.join("");
      const layout = this._layout;
      for (var i = 0, ii = rows.length; i < ii; i++) {
        var row = rows[i];
        var item = this._rowsCache[row];
        item.rowNodeL = l.firstElementChild;
        item.rowNodeR = r.firstElementChild;
        layout.appendCachedRow(row, item.rowNodeL, item.rowNodeR);
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
        var c = this._rowsCache[row];
        var p = this.getRowTop(parseInt(row, 10)) + "px";
        c.rowNodeL && (c.rowNodeL.style.top = p);
        c.rowNodeR && (c.rowNodeR.style.top = p);
      }
    }
    updateGrandTotals() {
      var _a, _b;
      if (!this._options.showFooterRow || !this._initialized)
        return;
      var totals;
      if (this._data && this._data.getGrandTotals)
        totals = this._data.getGrandTotals();
      totals = totals != null ? totals : {};
      var cols = this._cols;
      for (var m of cols) {
        if (m.id != void 0) {
          var formatter = (_a = m.groupTotalsFormatter) != null ? _a : this._options.groupTotalsFormatter;
          if (!formatter)
            continue;
          var content = (_b = formatter(totals, m, this)) != null ? _b : "";
          this.getFooterRowColumn(m.id).innerHTML = content;
        }
      }
    }
    // for usage as fallback by the groupmetadataitemprovider
    groupTotalsFormatter(p1, p2, grid) {
      return this._options.groupTotalsFormatter ? this._options.groupTotalsFormatter(p1, p2, grid != null ? grid : this) : "";
    }
    handleMouseWheel(e, delta, deltaX, deltaY) {
      deltaX = (typeof deltaX == "undefined" ? e.originalEvent.deltaX : deltaX) || 0;
      deltaY = (typeof deltaY == "undefined" ? e.originalEvent.deltaY : deltaY) || 0;
      this._scrollTop = Math.max(0, this._layout.getScrollContainerY().scrollTop - deltaY * this._options.rowHeight);
      this._scrollLeft = this._layout.getScrollContainerX().scrollLeft + deltaX * 10;
      this.handleScroll(true);
    }
    handleScroll(isMouseWheel) {
      this._scrollTop = this._layout.getScrollContainerY().scrollTop;
      this._scrollLeft = this._layout.getScrollContainerX().scrollLeft;
      var vScrollDist = Math.abs(this._scrollTop - this._scrollTopPrev);
      var hScrollDist = Math.abs(this._scrollLeft - this._scrollLeftPrev);
      if (hScrollDist || vScrollDist)
        this._ignoreScrollUntil = new Date().getTime() + 100;
      if (hScrollDist) {
        this._scrollLeftPrev = this._scrollLeft;
        this._layout.getScrollContainerX().scrollLeft = this._scrollLeft;
        this._layout.handleScrollH();
      }
      const vpi = this._viewportInfo;
      if (vScrollDist) {
        this._vScrollDir = this._scrollTopPrev < this._scrollTop ? 1 : -1;
        this._scrollTopPrev = this._scrollTop;
        if (isMouseWheel === true) {
          this._layout.getScrollContainerY().scrollTop = this._scrollTop;
        }
        this._layout.handleScrollV();
        if (vScrollDist < this._viewportInfo.height) {
          this.scrollTo(this._scrollTop + this._pageOffset);
        } else {
          var oldOffset = this._pageOffset;
          if (vpi.realScrollHeight == vpi.height) {
            this._page = 0;
          } else {
            this._page = Math.min(this._numberOfPages - 1, Math.floor(this._scrollTop * ((vpi.virtualHeight - this._viewportInfo.height) / (vpi.realScrollHeight - this._viewportInfo.height)) * (1 / this._pageHeight)));
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
        if (Math.abs(this._scrollTopRendered - this._scrollTop) > 20 || Math.abs(this._scrollLeftRendered - this._scrollLeft) > 20) {
          if (this._options.forceSyncScrolling || this._options.forceSyncScrollInterval && this._lastRenderTime < new Date().getTime() - this._options.forceSyncScrollInterval) {
            this.render();
          } else {
            this._hRender = setTimeout(this.render, 50);
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
      var _a;
      var cols = this._cols;
      while (((_a = this._postProcessCleanupQueue) == null ? void 0 : _a.length) > 0) {
        var groupId = this._postProcessCleanupQueue[0].groupId;
        while (this._postProcessCleanupQueue.length > 0 && this._postProcessCleanupQueue[0].groupId == groupId) {
          var entry = this._postProcessCleanupQueue.shift();
          entry.rowNodeL && entry.rowNodeL.remove();
          entry.rowNodeR && entry.rowNodeR.remove();
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
                const r = removedRowHash[columnId];
                removeClass(node, r);
              }
            }
          }
        }
        if (addedRowHash) {
          for (columnId in addedRowHash) {
            if (!removedRowHash || removedRowHash[columnId] != addedRowHash[columnId]) {
              node = this.getCellNode(parseInt(row, 10), this.getColumnIndex(columnId));
              if (node) {
                const a = addedRowHash[columnId];
                addClass(node, a);
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
        var cellEl = this._jQuery(this.getCellNode(row, cell));
        toggleCellClass(4);
      }
      var klass = this._options.cellFlashingCssClass;
      function toggleCellClass(times) {
        if (!times) {
          return;
        }
        setTimeout(function() {
          cellEl.queue(function() {
            cellEl.toggleClass(klass).dequeue();
            toggleCellClass(times - 1);
          });
        }, speed);
      }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////
    // Interactivity
    handleDragInit(e, dd) {
      var cell = this.getCellFromEvent(e);
      if (!cell || !this.cellExists(cell.row, cell.cell)) {
        return false;
      }
      var retval = this.trigger(this.onDragInit, dd, e);
      if (e.isImmediatePropagationStopped && e.isImmediatePropagationStopped()) {
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
      if (e.isImmediatePropagationStopped && e.isImmediatePropagationStopped()) {
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
      var handled = e.isImmediatePropagationStopped && e.isImmediatePropagationStopped();
      if (!handled) {
        if (!e.shiftKey && !e.altKey) {
          if (this._options.editable && this._currentEditor && this._currentEditor.keyCaptureList) {
            if (this._currentEditor.keyCaptureList.indexOf(e.which) >= 0) {
              return;
            }
          }
          if (e.key === "Home") {
            if (e.ctrlKey) {
              this.navigateTop();
              handled = true;
            } else
              handled = this.navigateRowStart();
          } else if (e.key === "End") {
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
          if (e.key === "Esc" || e.key === "Escape") {
            if (!this.getEditorLock().isActive()) {
              return;
            }
            this.cancelEditAndSetFocus();
          } else if (e.key === "PageDown") {
            this.navigatePageDown();
            handled = true;
          } else if (e.key === "PageUp") {
            this.navigatePageUp();
            handled = true;
          } else if (e.key === "Left" || e.key === "ArrowLeft") {
            handled = this.navigateLeft();
          } else if (e.key === "Right" || e.key === "ArrowRight") {
            handled = this.navigateRight();
          } else if (e.key === "Up" || e.key === "ArrowUp") {
            handled = this.navigateUp();
          } else if (e.key === "Down" || e.key === "ArrowDown") {
            handled = this.navigateDown();
          } else if (e.key === "Tab") {
            if (this._options.enableTabKeyNavigation)
              handled = this.navigateNext();
          } else if (e.key === "Enter") {
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
        } else if (e.key === "Tab" && e.shiftKey && !e.ctrlKey && !e.altKey) {
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
        if (e.target != document.activeElement || this._jQuery(e.target).hasClass("slick-cell")) {
          this.setFocus();
        }
      }
      var cell = this.getCellFromEvent(e);
      if (!cell || this._currentEditor != null && this._activeRow == cell.row && this._activeCell == cell.cell) {
        return;
      }
      this.trigger(this.onClick, { row: cell.row, cell: cell.cell }, e);
      if (e.isImmediatePropagationStopped && e.isImmediatePropagationStopped()) {
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
      var cellEl = e.target.closest(".slick-cell");
      if (!cellEl) {
        return;
      }
      if (this._activeCellNode === cellEl && this._currentEditor != null) {
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
      if (e.isImmediatePropagationStopped && e.isImmediatePropagationStopped()) {
        return;
      }
      if (this._options.editable) {
        this.gotoCell(cell.row, cell.cell, true);
      }
    }
    handleHeaderMouseEnter(e) {
      const column = this.getColumnFromNode(e.target);
      column && this.trigger(this.onHeaderMouseEnter, { column }, e);
    }
    handleHeaderMouseLeave(e) {
      const column = this.getColumnFromNode(e.target);
      column && this.trigger(this.onHeaderMouseLeave, { column }, e);
    }
    handleHeaderContextMenu(e) {
      var header = e.target.closest(".slick-header-column");
      var column = this.getColumnFromNode(header);
      column && this.trigger(this.onHeaderContextMenu, { column }, e);
    }
    handleHeaderClick(e) {
      var header = e.target.closest(".slick-header-column");
      var column = this.getColumnFromNode(header);
      column && this.trigger(this.onHeaderClick, { column }, e);
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
      if (cellNode == null)
        return null;
      var c = cellNode.dataset.c;
      if (c != null)
        return parseInt(c, 10);
      var cls = /\sl(\d+)\s/.exec(" " + cellNode.className + " ");
      if (!cls) {
        return null;
      }
      return parseInt(cls[1], 10);
    }
    getColumnFromNode(cellNode) {
      if (cellNode == null)
        return null;
      var cell = this.getCellFromNode(cellNode);
      if (cell === null && this._jQuery)
        return this._jQuery(cell).data("column");
      return this._cols[cell];
    }
    getRowFromNode(rowNode) {
      if (rowNode != null) {
        for (var row in this._rowsCache) {
          var c = this._rowsCache[row];
          if (c.rowNodeL === rowNode || c.rowNodeR === rowNode)
            return parseInt(row, 10);
        }
      }
      return null;
    }
    getCellFromEvent(e) {
      var row, cell;
      var cellEl = e.target.closest(".slick-cell");
      if (!cellEl) {
        return null;
      }
      row = this._layout.getRowFromCellNode(cellEl, e.clientX, e.clientY);
      cell = this.getCellFromNode(cellEl);
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
      var rowOffset = this._layout.getFrozenRowOffset(row);
      var cols = this._cols, frozenCols = this._layout.getFrozenCols();
      var y1 = this.getRowTop(row) - rowOffset;
      var y2 = y1 + this._options.rowHeight - 1;
      var x1 = 0;
      for (var i = 0; i < cell; i++) {
        x1 += cols[i].width;
        if (i == frozenCols - 1) {
          x1 = 0;
        }
      }
      var x2 = x1 + cols[cell].width;
      return this._options.rtl ? {
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
    //////////////////////////////////////////////////////////////////////////////////////////////
    // Cell switching
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
      if (cell < this._layout.getFrozenCols())
        return;
      var colspan = this.getColspan(row, cell);
      this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell + (colspan > 1 ? colspan - 1 : 0)]);
    }
    scrollColumnIntoView(cell) {
      this.internalScrollColumnIntoView(this._colLeft[cell], this._colRight[cell]);
    }
    internalScrollColumnIntoView(left, right) {
      var scrollRight = this._scrollLeft + parsePx(getComputedStyle(this._layout.getScrollContainerX()).width) - (this._viewportInfo.hasVScroll ? this._scrollDims.width : 0);
      var target;
      if (left < this._scrollLeft)
        target = left;
      else if (right > scrollRight)
        target = Math.min(left, right - this._layout.getScrollContainerX().clientWidth);
      else
        return;
      this._layout.getScrollContainerX().scrollLeft = target;
      this.handleScroll();
      this.render();
    }
    setActiveCellInternal(newCell, opt_editMode, preClickModeOn, suppressActiveCellChangedEvent, e) {
      var _a, _b;
      if (this._activeCellNode != null) {
        this.makeActiveCellNormal();
        this._activeCellNode.classList.remove("active");
        var c = this._rowsCache[this._activeRow];
        if (c) {
          c.rowNodeL && c.rowNodeL.classList.remove("active");
          c.rowNodeR && c.rowNodeR.classList.remove("active");
        }
      }
      this._activeCellNode = newCell;
      if (this._activeCellNode != null) {
        var bcl = this._activeCellNode.getBoundingClientRect();
        var rowOffset = Math.floor((_b = (_a = this._activeCellNode.closest(".grid-canvas")) == null ? void 0 : _a.getBoundingClientRect().top) != null ? _b : 0 + document.body.scrollTop);
        var isBottom = this._activeCellNode.closest(".grid-canvas-bottom") != null;
        if (this.hasFrozenRows() && isBottom) {
          rowOffset -= this._options.frozenBottom ? Math.round(parsePx(getComputedStyle(this._layout.getCanvasNodeFor(0, 0)).height)) : this._layout.getFrozenRows() * this._options.rowHeight;
        }
        var cell = this.getCellFromPoint(bcl[this._options.rtl ? "right" : "left"] + document.body.scrollLeft, Math.ceil(bcl.top + document.body.scrollTop) - rowOffset);
        this._activeRow = cell.row;
        this._activeCell = this._activePosX = this.getCellFromNode(this._activeCellNode);
        if (this._options.showCellSelection) {
          this._activeCellNode.classList.add("active");
          var c = this._rowsCache[this._activeRow];
          if (c) {
            c.rowNodeL && c.rowNodeL.classList.add("active");
            c.rowNodeR && c.rowNodeR.classList.add("active");
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
        this._activeCellNode.classList.remove("editable", "invalid");
        this.updateCellWithFormatter(this._activeCellNode, this._activeRow, this._activeCell);
        this.invalidatePostProcessingResults(this._activeRow);
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
      this._activeCellNode.classList.add("editable");
      var useEditor = editor || this.getEditor(this._activeRow, this._activeCell);
      if (!editor && !useEditor.suppressClearOnEdit) {
        this._activeCellNode.innerHTML = "";
      }
      var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(this._activeRow);
      var colsMetadata = itemMetadata && itemMetadata.columns;
      var columnMetadata = colsMetadata && (colsMetadata[columnDef.id] || colsMetadata[this._activeCell]);
      this._currentEditor = new useEditor({
        grid: this,
        gridPosition: absBox(this._container),
        position: absBox(this._activeCellNode),
        container: this._activeCellNode,
        column: columnDef,
        columnMetaData: columnMetadata,
        item: item || {},
        event: e,
        editorCellNavOnLRKeys: this._options.editorCellNavOnLRKeys,
        commitChanges: this.commitEditAndSetFocus.bind(this),
        cancelChanges: this.cancelEditAndSetFocus.bind(this)
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
    getActiveCellPosition() {
      return absBox(this._activeCellNode);
    }
    getGridPosition() {
      return absBox(this._container);
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
      if (!this._layout.isFrozenRow(row)) {
        var viewportScrollH = Math.round(parsePx(getComputedStyle(this._layout.getScrollContainerY()).height));
        var rowNumber = this.hasFrozenRows() && !this._options.frozenBottom ? row - this._layout.getFrozenRows() + 1 : row;
        var rowAtTop = rowNumber * this._options.rowHeight;
        var rowAtBottom = (rowNumber + 1) * this._options.rowHeight - viewportScrollH + (this._viewportInfo.hasHScroll ? this._scrollDims.height : 0);
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
      var deltaRows = dir * this._viewportInfo.numVisibleRows;
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
    /**
     * @param {string} dir Navigation direction.
     * @return {boolean} Whether navigation resulted in a change of active cell.
     */
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
      if (!this._cellNavigator) {
        this._cellNavigator = new CellNavigator({
          getColumnCount: () => this._cols.length,
          getRowCount: () => this.getDataLengthIncludingAddNew(),
          getColspan: this.getColspan.bind(this),
          canCellBeActive: this.canCellBeActive.bind(this),
          setTabbingDirection: (dir2) => this._tabbingDirection = dir2,
          isRTL: () => this._options.rtl
        });
      }
      var pos = this._cellNavigator.navigate(dir, this._activeRow, this._activeCell, this._activePosX);
      if (pos) {
        if (this.hasFrozenRows() && this._options.frozenBottom && pos.row == this.getDataLength()) {
          return;
        }
        var isAddNewRow = pos.row == this.getDataLength();
        if (!this._layout.isFrozenRow(pos.row)) {
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
    setActiveRow(row, cell, suppressScrollIntoView) {
      if (!this._initialized)
        return;
      if (row > this.getDataLength() || row < 0 || cell >= this._cols.length || cell < 0)
        return;
      this._activeRow = row;
      if (!suppressScrollIntoView)
        this.scrollCellIntoView(row, cell || 0, false);
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
      var colsMetadata = rowMetadata && rowMetadata.columns;
      if (colsMetadata && cols[cell] && colsMetadata[cols[cell].id] && typeof colsMetadata[cols[cell].id].focusable === "boolean") {
        return colsMetadata[cols[cell].id].focusable;
      }
      if (colsMetadata && colsMetadata[cell] && typeof colsMetadata[cell].focusable === "boolean") {
        return colsMetadata[cell].focusable;
      }
      return cols[cell].focusable;
    }
    canCellBeSelected(row, cell) {
      var cols = this._cols;
      if (row >= this.getDataLength() || row < 0 || cell >= cols.length || cell < 0) {
        return false;
      }
      var itemMetadata = this._data.getItemMetadata && this._data.getItemMetadata(row);
      if (itemMetadata && typeof itemMetadata.selectable === "boolean") {
        return itemMetadata.selectable;
      }
      var columnMetadata = itemMetadata && itemMetadata.columns && (itemMetadata.columns[cols[cell].id] || itemMetadata.columns[cell]);
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
    //////////////////////////////////////////////////////////////////////////////////////////////
    // IEditor implementation for the editor lock
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
            this._activeCellNode.classList.remove("invalid");
            this._activeCellNode.offsetWidth;
            this._activeCellNode.classList.add("invalid");
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
  return __toCommonJS(grid_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;
