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
    var _a;
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
        m.name = titleize((_a = m.field) != null ? _a : m.id);
        delete m.nameIsHtml;
      }
    }
  }
  function titleize(str) {
    if (!str)
      return str;
    str = ("" + str).replace(/([A-Z]+)([A-Z][a-z])/, "$1_$2").replace(/([a-z\d])([A-Z])/, "$1_$2").replace(/[-\s]/, "_").toLowerCase();
    return str.replace(/\s/, "_").split("_").filter((x) => x.length).map((x) => x.charAt(0).toUpperCase() + x.substring(1).toLowerCase()).join(" ");
  }

  // src/core/event.ts
  var EventData = class {
    constructor() {
      this._isPropagationStopped = false;
      this._isImmediatePropagationStopped = false;
    }
    /***
     * Stops event from propagating up the DOM tree.
     * @method stopPropagation
     */
    stopPropagation() {
      this._isPropagationStopped = true;
    }
    /***
     * Returns whether stopPropagation was called on this event object.
     */
    isPropagationStopped() {
      return this._isPropagationStopped;
    }
    /***
     * Prevents the rest of the handlers from being executed.
     */
    stopImmediatePropagation() {
      this._isImmediatePropagationStopped = true;
    }
    /***
     * Returns whether stopImmediatePropagation was called on this event object.\
     */
    isImmediatePropagationStopped() {
      return this._isImmediatePropagationStopped;
    }
  };
  var EventEmitter = class {
    constructor() {
      this._handlers = [];
    }
    /***
     * Adds an event handler to be called when the event is fired.
     * <p>Event handler will receive two arguments - an <code>EventData</code> and the <code>data</code>
     * object the event was fired with.<p>
     * @method subscribe
     * @param fn {Function} Event handler.
     */
    subscribe(fn) {
      this._handlers.push(fn);
    }
    /***
     * Removes an event handler added with <code>subscribe(fn)</code>.
     * @method unsubscribe
     * @param fn {Function} Event handler to be removed.
     */
    unsubscribe(fn) {
      for (var i = this._handlers.length - 1; i >= 0; i--) {
        if (this._handlers[i] === fn) {
          this._handlers.splice(i, 1);
        }
      }
    }
    /***
     * Fires an event notifying all subscribers.
     * @param args {Object} Additional data object to be passed to all handlers.
     * @param e {EventData}
     *      Optional.
     *      An <code>EventData</code> object to be passed to all handlers.
     *      For DOM events, an existing W3C/jQuery event object can be passed in.
     * @param scope {Object}
     *      Optional.
     *      The scope ("this") within which the handler will be executed.
     *      If not specified, the scope will be set to the <code>Event</code> instance.
     */
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
          return this;
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
    /***
     * Returns true if a specified edit controller is active (has the edit lock).
     * If the parameter is not specified, returns true if any edit controller is active.
     * @method isActive
     * @param editController {EditController}
     * @return {Boolean}
     */
    isActive(editController) {
      return editController ? this.activeEditController === editController : this.activeEditController != null;
    }
    /***
     * Sets the specified edit controller as the active edit controller (acquire edit lock).
     * If another edit controller is already active, and exception will be thrown.
     * @method activate
     * @param editController {EditController} edit controller acquiring the lock
     */
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
    /***
     * Unsets the specified edit controller as the active edit controller (release edit lock).
     * If the specified edit controller is not the active one, an exception will be thrown.
     * @method deactivate
     * @param editController {EditController} edit controller releasing the lock
     */
    deactivate(editController) {
      if (this.activeEditController !== editController) {
        throw "SleekGrid.EditorLock.deactivate: specified editController is not the currently active one";
      }
      this.activeEditController = null;
    }
    /***
     * Attempts to commit the current edit by calling "commitCurrentEdit" method on the active edit
     * controller and returns whether the commit attempt was successful (commit may fail due to validation
     * errors, etc.).  Edit controller's "commitCurrentEdit" must return true if the commit has succeeded
     * and false otherwise.  If no edit controller is active, returns true.
     * @method commitCurrentEdit
     * @return {Boolean}
     */
    commitCurrentEdit() {
      return this.activeEditController ? this.activeEditController.commitCurrentEdit() : true;
    }
    /***
     * Attempts to cancel the current edit by calling "cancelCurrentEdit" method on the active edit
     * controller and returns whether the edit was successfully cancelled.  If no edit controller is
     * active, returns true.
     * @method cancelCurrentEdit
     * @return {Boolean}
     */
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
          var key = k === "className" ? "class" : k;
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
    var _a, _b;
    var oldFmtAtt = node.dataset.fmtatt;
    if ((oldFmtAtt == null ? void 0 : oldFmtAtt.length) > 0) {
      for (var k of oldFmtAtt.split(","))
        node.removeAttribute(k);
      delete node.dataset.fmtatt;
    }
    var oldFmtCls = node.dataset.fmtcls;
    if ((oldFmtCls == null ? void 0 : oldFmtCls.length) && ctx.addClass != oldFmtCls) {
      removeClass(node, oldFmtCls);
      if (!((_a = ctx.addClass) == null ? void 0 : _a.length))
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
    if ((_b = ctx.addClass) == null ? void 0 : _b.length) {
      addClass(node, ctx.addClass);
      node.dataset.fmtcls = ctx.addClass;
    }
  }

  // src/core/group.ts
  var Group = class extends NonDataRow {
    constructor() {
      super(...arguments);
      this.__group = true;
      /**
       * Grouping level, starting with 0.
       * @property level
       * @type {Number}
       */
      this.level = 0;
      /***
       * Number of rows in the group.
       * @property count
       * @type {Number}
       */
      this.count = 0;
      /***
       * Whether a group is collapsed.
       * @property collapsed
       * @type {Boolean}
       */
      this.collapsed = false;
      /**
       * Rows that are part of the group.
       * @property rows
       * @type {Array}
       */
      this.rows = [];
    }
    /***
     * Compares two Group instances.
     * @method equals
     * @return {Boolean}
     * @param group {Group} Group instance to compare to.
     */
    equals(group) {
      return this.value === group.value && this.count === group.count && this.collapsed === group.collapsed && this.title === group.title;
    }
  };
  var GroupTotals = class extends NonDataRow {
    constructor() {
      super(...arguments);
      this.__groupTotals = true;
      /***
       * Whether the totals have been fully initialized / calculated.
       * Will be set to false for lazy-calculated group totals.
       * @param initialized
       * @type {Boolean}
       */
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
    /***
     * Returns whether a range represents a single row.
     */
    isSingleRow() {
      return this.fromRow == this.toRow;
    }
    /***
     * Returns whether a range represents a single cell.
     */
    isSingleCell() {
      return this.fromRow == this.toRow && this.fromCell == this.toCell;
    }
    /***
     * Returns whether a range contains a given cell.
     */
    contains(row, cell) {
      return row >= this.fromRow && row <= this.toRow && cell >= this.fromCell && cell <= this.toCell;
    }
    /***
     * Returns a readable representation of a range.
     */
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
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._; Slick.Event = Slick.EventEmitter; Slick.EventHandler = Slick.EventSubscriber; typeof Map !== 'undefined' && (Slick.Map = Map);
