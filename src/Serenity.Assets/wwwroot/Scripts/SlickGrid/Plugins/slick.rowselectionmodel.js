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

  // src/plugins/rowselectionmodel.ts
  var rowselectionmodel_exports = {};
  __export(rowselectionmodel_exports, {
    RowSelectionModel: () => RowSelectionModel
  });

  // global-externals:_
  var { EventEmitter, EventSubscriber, Range } = Slick;

  // src/plugins/rowselectionmodel.ts
  function getRowsRange(from, to) {
    let i, rows = [];
    for (i = from; i <= to; i++) {
      rows.push(i);
    }
    for (i = to; i < from; i++) {
      rows.push(i);
    }
    return rows;
  }
  function rangesToRows(ranges) {
    let rows = [];
    for (let i = 0; i < ranges.length; i++) {
      for (let j = ranges[i].fromRow; j <= ranges[i].toRow; j++) {
        rows.push(j);
      }
    }
    return rows;
  }
  var _RowSelectionModel = class _RowSelectionModel {
    constructor(options) {
      this.handler = new EventSubscriber();
      this.onSelectedRangesChanged = new EventEmitter();
      this.options = Object.assign({}, _RowSelectionModel.defaults, options);
    }
    init(grid) {
      this.grid = grid;
      this.handler.subscribe(grid.onActiveCellChanged, this.wrapHandler(this.handleActiveCellChange));
      this.handler.subscribe(grid.onKeyDown, this.wrapHandler(this.handleKeyDown));
      this.handler.subscribe(grid.onClick, this.wrapHandler(this.handleClick));
    }
    destroy() {
      var _a;
      (_a = this.handler) == null ? void 0 : _a.unsubscribeAll();
    }
    wrapHandler(handler) {
      return function() {
        if (!this.inHandler) {
          this.inHandler = true;
          handler.apply(this, arguments);
          this.inHandler = false;
        }
      }.bind(this);
    }
    rowsToRanges(rows) {
      let ranges = [];
      let lastCell = this.grid.getColumns().length - 1;
      for (let i = 0; i < rows.length; i++) {
        ranges.push(new Range(rows[i], 0, rows[i], lastCell));
      }
      return ranges;
    }
    getSelectedRows() {
      return rangesToRows(this.ranges);
    }
    setSelectedRows(rows) {
      this.setSelectedRanges(this.rowsToRanges(rows));
    }
    setSelectedRanges(ranges) {
      if ((!this.ranges || this.ranges.length === 0) && (!ranges || ranges.length === 0))
        return;
      this.ranges = ranges;
      this.onSelectedRangesChanged.notify(this.ranges);
    }
    getSelectedRanges() {
      return this.ranges;
    }
    handleActiveCellChange(_, data) {
      if (this.options.selectActiveRow && data.row != null) {
        this.setSelectedRanges([new Range(data.row, 0, data.row, this.grid.getColumns().length - 1)]);
      }
    }
    handleKeyDown(e) {
      let activeRow = this.grid.getActiveCell();
      if (!(activeRow && e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey && (e.which == 38 || e.which == 40)))
        return;
      let selectedRows = this.getSelectedRows();
      selectedRows.sort(function(x, y) {
        return x - y;
      });
      if (!selectedRows.length) {
        selectedRows = [activeRow.row];
      }
      let top = selectedRows[0];
      let bottom = selectedRows[selectedRows.length - 1];
      let active;
      if (e.which == 40) {
        active = activeRow.row < bottom || top == bottom ? ++bottom : ++top;
      } else {
        active = activeRow.row < bottom ? --bottom : --top;
      }
      if (active >= 0 && active < this.grid.getDataLength()) {
        this.grid.scrollRowIntoView(active);
        this.ranges = this.rowsToRanges(getRowsRange(top, bottom));
        this.setSelectedRanges(this.ranges);
      }
      e.preventDefault();
      e.stopPropagation();
    }
    handleClick(e) {
      let cell = this.grid.getCellFromEvent(e);
      if (!cell || !this.grid.canCellBeActive(cell.row, cell.cell)) {
        return false;
      }
      if (!this.grid.getOptions().multiSelect || !e.ctrlKey && !e.shiftKey && !e.metaKey) {
        return false;
      }
      let selection = rangesToRows(this.ranges);
      let idx = selection.indexOf(cell.row);
      if (idx === -1 && (e.ctrlKey || e.metaKey)) {
        selection.push(cell.row);
        this.grid.setActiveCell(cell.row, cell.cell);
      } else if (idx !== -1 && (e.ctrlKey || e.metaKey)) {
        selection = selection.filter((o) => {
          return o !== cell.row;
        });
        this.grid.setActiveCell(cell.row, cell.cell);
      } else if (selection.length && e.shiftKey) {
        let last = selection.pop();
        let from = Math.min(cell.row, last);
        let to = Math.max(cell.row, last);
        selection = [];
        for (let i = from; i <= to; i++) {
          if (i !== last) {
            selection.push(i);
          }
        }
        selection.push(last);
        this.grid.setActiveCell(cell.row, cell.cell);
      }
      this.ranges = this.rowsToRanges(selection);
      this.setSelectedRanges(this.ranges);
      e.stopImmediatePropagation();
      return true;
    }
  };
  _RowSelectionModel.defaults = {
    selectActiveRow: true
  };
  var RowSelectionModel = _RowSelectionModel;
  return __toCommonJS(rowselectionmodel_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;
