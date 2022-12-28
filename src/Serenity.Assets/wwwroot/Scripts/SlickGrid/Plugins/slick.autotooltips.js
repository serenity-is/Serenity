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

  // src/plugins/autotooltips.ts
  var autotooltips_exports = {};
  __export(autotooltips_exports, {
    AutoTooltips: () => AutoTooltips
  });
  var _AutoTooltips = class {
    constructor(options) {
      this.handleMouseEnter = (e) => {
        var _a, _b;
        var cell = this.grid.getCellFromEvent(e);
        if (!cell)
          return;
        var node = this.grid.getCellNode(cell.row, cell.cell);
        if (!node)
          return;
        var text;
        if (!node.title || this.options.replaceExisting) {
          if (node.clientWidth < node.scrollWidth) {
            text = (_b = (_a = node.textContent) == null ? void 0 : _a.trim()) != null ? _b : "";
            if (this.options.maxToolTipLength && text.length > this.options.maxToolTipLength) {
              text = text.substring(0, this.options.maxToolTipLength - 3) + "...";
            }
          } else {
            text = "";
          }
          node.title = text;
        }
        node = null;
      };
      this.handleHeaderMouseEnter = (e, args) => {
        var column = args.column;
        if (column && !column.toolTip) {
          var node = e.target.closest(".slick-header-column");
          node && (node.title = node.clientWidth < node.scrollWidth ? column.name : "");
        }
      };
      this.pluginName = "AutoTooltips";
      this.options = Object.assign({}, _AutoTooltips.defaults, options);
    }
    init(grid) {
      this.grid = grid;
      if (this.options.enableForCells)
        this.grid.onMouseEnter.subscribe(this.handleMouseEnter);
      if (this.options.enableForHeaderCells)
        this.grid.onHeaderMouseEnter.subscribe(this.handleHeaderMouseEnter);
    }
    destroy() {
      if (this.options.enableForCells)
        this.grid.onMouseEnter.unsubscribe(this.handleMouseEnter);
      if (this.options.enableForHeaderCells)
        this.grid.onHeaderMouseEnter.unsubscribe(this.handleHeaderMouseEnter);
    }
  };
  var AutoTooltips = _AutoTooltips;
  AutoTooltips.defaults = {
    enableForCells: true,
    enableForHeaderCells: false,
    maxToolTipLength: null,
    replaceExisting: true
  };
  return __toCommonJS(autotooltips_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;
