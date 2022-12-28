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

  // src/core/index.ts
  var core_exports = {};
  __export(core_exports, {
    EditorLock: () => EditorLock,
    EventData: () => EventData,
    EventEmitter: () => EventEmitter,
    EventSubscriber: () => EventSubscriber,
    GlobalEditorLock: () => GlobalEditorLock,
    Group: () => Group,
    GroupTotals: () => GroupTotals,
    H: () => H,
    NonDataRow: () => NonDataRow,
    Range: () => Range,
    addClass: () => addClass,
    applyFormatterResultToCellNode: () => applyFormatterResultToCellNode,
    columnDefaults: () => columnDefaults,
    convertCompatFormatter: () => convertCompatFormatter,
    defaultColumnFormat: () => defaultColumnFormat,
    disableSelection: () => disableSelection,
    escape: () => escape,
    initializeColumns: () => initializeColumns,
    keyCode: () => keyCode,
    parsePx: () => parsePx,
    patchEvent: () => patchEvent,
    preClickClassName: () => preClickClassName,
    removeClass: () => removeClass,
    spacerDiv: () => spacerDiv,
    titleize: () => titleize
  });

  // src/core/base.ts
  var NonDataRow = class {
    constructor() {
      this.__nonDataRow = true;
    }
  };
  var preClickClassName = "slick-edit-preclick";
  typeof window !== "undefined" && window.Slick && (window.Slick.Map = Map);

  // src/core/column.ts
  var columnDefaults = {
    nameIsHtml: false,
    resizable: true,
    sortable: false,
    minWidth: 30,
    rerenderOnResize: false,
    defaultSortAsc: true,
    focusable: true,
    selectable: true
  };
  function initializeColumns(columns, defaults) {
    var _a, _b;
    var usedIds = {};
    for (var i = 0; i < columns.length; i++) {
      var m = columns[i];
      if (defaults != null) {
        for (var k in defaults) {
          if (m[k] === void 0)
            m[k] = defaults[k];
        }
      }
      if (m.minWidth && m.width < m.minWidth)
        m.width = m.minWidth;
      if (m.maxWidth && m.width > m.maxWidth)
        m.width = m.maxWidth;
      if (m.id == null || usedIds[m.id]) {
        const prefix = m.id != null && m.id.length ? m.id : m.field != null ? m.field : "col";
        var x = 0;
        while (usedIds[m.id = prefix + (x == 0 ? "" : "_" + x.toString())])
          x++;
      }
      usedIds[m.id] = true;
      if (m.name === void 0) {
        m.name = titleize((_b = (_a = m.field) != null ? _a : m.id) != null ? _b : "");
        delete m.nameIsHtml;
      }
    }
  }
  function underscore(str) {
    return (str != null ? str : "").replace(/([A-Z]+)([A-Z][a-z])/, "$1_$2").replace(/([a-z\d])([A-Z])/, "$1_$2").replace(/[-\s]/, "_").toLowerCase();
  }
  function titleize(str) {
    if (!str)
      return str;
    return underscore(str).replace(/\s/, "_").split("_").filter((x) => x.length).map((x) => x.charAt(0).toUpperCase() + x.substring(1).toLowerCase()).join(" ");
  }

  // src/core/event.ts
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
  var EventEmitter = class {
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
  var EventSubscriber = class {
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

  // src/core/editing.ts
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

  // src/core/util.ts
  function addClass(el, cls) {
    if (cls == null || !cls.length)
      return;
    if (cls.indexOf(" ") >= 0) {
      var arr = cls.split(" ").map((x) => x.trim()).filter((x) => x.length);
      for (var a of arr)
        el.classList.add(a);
    } else
      el.classList.add(cls);
  }
  var esc = {
    "<": "&lt;",
    ">": "&gt;",
    '"': "&quot;",
    "'": "&apos;",
    "&": "&amp;"
  };
  function escFunc(a) {
    return esc[a];
  }
  function escape(s) {
    if (!arguments.length)
      s = this.value;
    if (s == null)
      return "";
    if (typeof s !== "string")
      s = "" + s;
    return s.replace(/[<>"'&]/g, escFunc);
  }
  function disableSelection(target) {
    if (target) {
      target.setAttribute("unselectable", "on");
      target.style.userSelect = "none";
      target.addEventListener("selectstart", () => false);
    }
  }
  function removeClass(el, cls) {
    if (cls == null || !cls.length)
      return;
    if (cls.indexOf(" ") >= 0) {
      var arr = cls.split(" ").map((x) => x.trim()).filter((x) => x.length);
      for (var a of arr)
        el.classList.remove(a);
    } else
      el.classList.remove(cls);
  }
  function H(tag, attr, ...children) {
    var el = document.createElement(tag);
    var k, v, c;
    if (attr) {
      for (k in attr) {
        v = attr[k];
        if (v != null && v !== false) {
          if (k === "ref" && typeof v === "function") {
            v(el);
            continue;
          }
          var key = k === "cssClass" ? "class" : k;
          el.setAttribute(key, v === true ? "" : v);
        }
      }
    }
    if (children && children.length)
      el.append(...children);
    return el;
  }
  function spacerDiv(width) {
    return H("div", { style: "display:block;height:1px;position:absolute;top:0;left:0;", width });
  }
  function parsePx(str) {
    var value = parseFloat(str);
    if (isNaN(value))
      return 0;
    return value;
  }

  // src/core/formatting.ts
  function defaultColumnFormat(ctx) {
    return escape(ctx.value);
  }
  function convertCompatFormatter(compatFormatter) {
    if (compatFormatter == null)
      return null;
    return function(ctx) {
      var fmtResult = compatFormatter(ctx.row, ctx.cell, ctx.value, ctx.column, ctx.item, ctx.grid);
      if (fmtResult != null && typeof fmtResult !== "string" && Object.prototype.toString.call(fmtResult) === "[object Object]") {
        ctx.addClass = fmtResult.addClasses;
        ctx.tooltip = fmtResult.toolTip;
        return fmtResult.text;
      }
      return fmtResult;
    };
  }
  function applyFormatterResultToCellNode(ctx, html, node) {
    var _a, _b, _c, _d;
    var oldFmtAtt = (_a = node.dataset) == null ? void 0 : _a.fmtatt;
    if ((oldFmtAtt == null ? void 0 : oldFmtAtt.length) > 0) {
      for (var k of oldFmtAtt.split(","))
        node.removeAttribute(k);
      delete node.dataset.fmtatt;
    }
    var oldFmtCls = (_b = node.dataset) == null ? void 0 : _b.fmtcls;
    if ((oldFmtCls == null ? void 0 : oldFmtCls.length) && ctx.addClass != oldFmtCls) {
      removeClass(node, oldFmtCls);
      if (!((_c = ctx.addClass) == null ? void 0 : _c.length))
        delete node.dataset.fmtcls;
    }
    var oldTooltip = node.getAttribute("tooltip");
    if (oldTooltip != null && ctx.tooltip != oldTooltip)
      node.removeAttribute("tooltip");
    if (ctx.tooltip !== void 0 && oldTooltip != ctx.tooltip)
      node.setAttribute("tooltip", ctx.tooltip);
    if (html == void 0)
      node.innerHTML = "";
    else
      node.innerHTML = "" + html;
    if (ctx.addAttrs != null) {
      var keys = Object.keys(ctx.addAttrs);
      if (keys.length) {
        for (var k of keys) {
          node.setAttribute(k, ctx.addAttrs[k]);
        }
        node.dataset.fmtatt = keys.join(",");
      }
    }
    if ((_d = ctx.addClass) == null ? void 0 : _d.length) {
      addClass(node, ctx.addClass);
      node.dataset.fmtcls = ctx.addClass;
    }
  }

  // src/core/group.ts
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

  // src/core/range.ts
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
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._; Slick.Event = Slick.EventEmitter; Slick.EventHandler = Slick.EventSubscriber;
