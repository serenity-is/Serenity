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

  // src/plugins/rowmovemanager.ts
  var rowmovemanager_exports = {};
  __export(rowmovemanager_exports, {
    RowMoveManager: () => RowMoveManager
  });

  // global-externals:_
  var { EventEmitter, EventSubscriber, H } = Slick;

  // src/plugins/rowmovemanager.ts
  var _RowMoveManager = class _RowMoveManager {
    constructor(options) {
      this.handler = new EventSubscriber();
      this.onBeforeMoveRows = new EventEmitter();
      this.onMoveRows = new EventEmitter();
      this.options = Object.assign({}, _RowMoveManager.defaults, options);
    }
    init(grid) {
      this.grid = grid;
      this.handler.subscribe(grid.onDragInit, this.handleDragInit.bind(this)).subscribe(grid.onDragStart, this.handleDragStart.bind(this)).subscribe(grid.onDrag, this.handleDrag.bind(this)).subscribe(grid.onDragEnd, this.handleDragEnd.bind(this));
    }
    destroy() {
      var _a;
      (_a = this.handler) == null ? void 0 : _a.unsubscribeAll();
    }
    handleDragInit(e) {
      e.stopImmediatePropagation();
    }
    handleDragStart(e, dd) {
      let cell = this.grid.getCellFromEvent(e);
      if (this.options.cancelEditOnDrag && this.grid.getEditorLock().isActive()) {
        this.grid.getEditorLock().cancelCurrentEdit();
      }
      if (this.grid.getEditorLock().isActive() || !/move|selectAndMove/.test(this.grid.getColumns()[cell.cell].behavior)) {
        return false;
      }
      this.dragging = true;
      e.stopImmediatePropagation();
      let selectedRows = this.grid.getSelectedRows();
      if (selectedRows.length == 0 || selectedRows.indexOf(cell.row) == -1) {
        selectedRows = [cell.row];
        this.grid.setSelectedRows(selectedRows);
      }
      let rowHeight = this.grid.getOptions().rowHeight;
      dd.selectedRows = selectedRows;
      let canvas = this.grid.getCanvasNode();
      dd.selectionProxy = canvas.appendChild(H("div", {
        "class": "slick-reorder-proxy",
        "style": `position: absolute; z-index: 9999; width: ${canvas.clientWidth}px; height: ${rowHeight * selectedRows.length}px`
      }));
      dd.guide = canvas.appendChild(H("div", {
        "class": "slick-reorder-guide",
        "style": `position: absolute; z-index: 99998; width: ${canvas.clientWidth}px; top: -1000`
      }));
      dd.insertBefore = -1;
    }
    handleDrag(e, dd) {
      if (!this.dragging)
        return;
      e.stopImmediatePropagation();
      let canvas = this.grid.getCanvasNode();
      let box = canvas.getBoundingClientRect();
      let docElem = document.documentElement;
      let canvasTop = box.top + window.scrollY - docElem.clientTop;
      let top = e.pageY - canvasTop;
      dd.selectionProxy.style.top = top - 5 + "px";
      let insertBefore = Math.max(0, Math.min(Math.round(top / this.grid.getOptions().rowHeight), this.grid.getDataLength()));
      if (insertBefore !== dd.insertBefore) {
        let eventData = {
          rows: dd.selectedRows,
          insertBefore
        };
        if (this.onBeforeMoveRows.notify(eventData) === false) {
          dd.guide.style.top = "-1000";
          dd.canMove = false;
        } else {
          dd.guide.style.top = insertBefore * this.grid.getOptions().rowHeight + "px";
          dd.canMove = true;
        }
        dd.insertBefore = insertBefore;
      }
    }
    handleDragEnd(e, dd) {
      if (!this.dragging)
        return;
      this.dragging = false;
      e.stopImmediatePropagation();
      dd.guide.remove();
      dd.selectionProxy.remove();
      if (dd.canMove) {
        let eventData = {
          rows: dd.selectedRows,
          insertBefore: dd.insertBefore
        };
        this.onMoveRows.notify(eventData);
      }
    }
  };
  _RowMoveManager.defaults = {
    cancelEditOnDrag: false
  };
  var RowMoveManager = _RowMoveManager;
  return __toCommonJS(rowmovemanager_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;
