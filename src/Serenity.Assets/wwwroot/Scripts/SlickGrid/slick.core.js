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

  // node_modules/@serenity-is/sleekgrid/src/core/index.ts
  var core_exports = {};
  __export(core_exports, {
    EditorLock: () => EditorLock,
    Event: () => Event,
    EventData: () => EventData,
    EventHandler: () => EventHandler,
    GlobalEditorLock: () => GlobalEditorLock,
    Group: () => Group,
    GroupTotals: () => GroupTotals,
    NonDataRow: () => NonDataRow,
    Range: () => Range,
    attrEncode: () => attrEncode,
    htmlEncode: () => htmlEncode,
    keyCode: () => keyCode,
    patchEvent: () => patchEvent,
    preClickClassName: () => preClickClassName
  });

  // node_modules/@serenity-is/sleekgrid/src/core/base.ts
  var NonDataRow = class {
    constructor() {
      this.__nonDataRow = true;
    }
  };
  var preClickClassName = "slick-edit-preclick";
  typeof window !== "undefined" && window.Slick && (window.Slick.Map = Map);

  // node_modules/@serenity-is/sleekgrid/src/core/encode.ts
  function attrEncode(s) {
    if (s == null)
      return "";
    return (s + "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;");
  }
  function htmlEncode(s) {
    return (s + "").replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  }

  // node_modules/@serenity-is/sleekgrid/src/core/event.ts
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
      e = patchEvent(e) || new EventData();
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
  var EventHandler = class {
    constructor() {
      this._handlers = [];
    }
    subscribe(event, handler) {
      this._handlers.push({
        event,
        handler
      });
      event.subscribe(handler);
      return this;
    }
    unsubscribe(event, handler) {
      var i = this._handlers.length;
      while (i--) {
        if (this._handlers[i].event === event && this._handlers[i].handler === handler) {
          this._handlers.splice(i, 1);
          event.unsubscribe(handler);
          return;
        }
      }
      return this;
    }
    unsubscribeAll() {
      var i = this._handlers.length;
      while (i--) {
        this._handlers[i].event.unsubscribe(this._handlers[i].handler);
      }
      this._handlers = [];
      return this;
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
  function returnTrue() {
    return true;
  }
  function returnFalse() {
    return false;
  }
  function patchEvent(e) {
    if (e == null)
      return e;
    if (!e.isDefaultPrevented && e.preventDefault)
      e.isDefaultPrevented = function() {
        return this.defaultPrevented;
      };
    var org1, org2;
    if (!e.isImmediatePropagationStopped && (org1 = e.stopImmediatePropagation)) {
      e.isImmediatePropagationStopped = returnFalse;
      e.stopImmediatePropagation = function() {
        this.isImmediatePropagationStopped = returnTrue;
        org1.call(this);
      };
    }
    if (!e.isPropagationStopped && (org2 = e.stopPropagation)) {
      e.isPropagationStopped = returnFalse;
      e.stopPropagation = function() {
        this.isPropagationStopped = returnTrue;
        org2.call(this);
      };
    }
    return e;
  }

  // node_modules/@serenity-is/sleekgrid/src/core/editlock.ts
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

  // node_modules/@serenity-is/sleekgrid/src/core/group.ts
  var Group = class extends NonDataRow {
    constructor() {
      super(...arguments);
      this.__group = true;
      this.level = 0;
      this.count = 0;
      this.collapsed = false;
      this.rows = [];
    }
    equals(group) {
      return this.value === group.value && this.count === group.count && this.collapsed === group.collapsed && this.title === group.title;
    }
  };
  var GroupTotals = class extends NonDataRow {
    constructor() {
      super(...arguments);
      this.__groupTotals = true;
      this.initialized = false;
    }
  };

  // node_modules/@serenity-is/sleekgrid/src/core/range.ts
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
  return __toCommonJS(core_exports);
})();
["Plugins", "Formatters", "Editors"].forEach(ns => Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns] || {})); Object.assign(Slick, Slick._); delete Slick._;
