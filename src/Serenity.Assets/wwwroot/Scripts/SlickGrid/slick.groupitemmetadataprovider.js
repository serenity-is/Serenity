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

  // src/data/groupitemmetadataprovider.ts
  var groupitemmetadataprovider_exports = {};
  __export(groupitemmetadataprovider_exports, {
    GroupItemMetadataProvider: () => GroupItemMetadataProvider
  });

  // global-externals:_
  var { convertCompatFormatter, Group } = Slick;

  // src/data/groupitemmetadataprovider.ts
  var _GroupItemMetadataProvider = class {
    constructor(opt) {
      this.pluginName = "GroupItemMetadataProvider";
      this.handleGridClick = (e, args) => {
        var _a, _b, _c, _d, _e, _f, _g;
        let grid = (_a = args == null ? void 0 : args.grid) != null ? _a : this.grid;
        if (!grid)
          return;
        var item = grid.getDataItem(args.row);
        if (!item || !(item instanceof Group) || !this.options.toggleCssClass || !e.target.classList.contains(this.options.toggleCssClass))
          return;
        e.stopImmediatePropagation();
        e.preventDefault();
        var range = grid.getRenderedRange();
        (_c = (_b = grid.getData()).setRefreshHints) == null ? void 0 : _c.call(_b, {
          ignoreDiffsBefore: range.top,
          ignoreDiffsAfter: range.bottom + 1
        });
        if (item.collapsed)
          (_e = (_d = grid.getData()).expandGroup) == null ? void 0 : _e.call(_d, item.groupingKey);
        else
          (_g = (_f = grid.getData()).collapseGroup) == null ? void 0 : _g.call(_f, item.groupingKey);
      };
      this.handleGridKeyDown = (e, args) => {
        var _a, _b, _c, _d, _e, _f, _g;
        if (!this.options.enableExpandCollapse || e.key !== " " && e.key !== "-" && e.key !== "+")
          return;
        let grid = (_a = args == null ? void 0 : args.grid) != null ? _a : this.grid;
        if (!grid)
          return;
        var activeCell = grid.getActiveCell();
        if (!activeCell)
          return;
        var item = grid.getDataItem(activeCell.row);
        if (!item || !(item instanceof Group))
          return;
        e.stopImmediatePropagation();
        e.preventDefault();
        if (e.key == "+" && !item.collapsed || e.key == "-" && item.collapsed)
          return;
        var range = grid.getRenderedRange();
        (_c = (_b = grid.getData()).setRefreshHints) == null ? void 0 : _c.call(_b, {
          ignoreDiffsBefore: range.top,
          ignoreDiffsAfter: range.bottom + 1
        });
        if (item.collapsed)
          (_e = (_d = grid.getData()).expandGroup) == null ? void 0 : _e.call(_d, item.groupingKey);
        else
          (_g = (_f = grid.getData()).collapseGroup) == null ? void 0 : _g.call(_f, item.groupingKey);
      };
      this.groupCellPosition = () => {
        var _a, _b, _c, _d;
        const result = {
          cell: 0,
          colspan: "*"
        };
        if (!this.options.groupRowTotals || !this.grid) {
          return result;
        }
        var cols = this.grid.getColumns();
        var col1;
        for (var idx = 0; idx < cols.length; idx++) {
          col1 = cols[idx];
          if (!((_b = (_a = this.options).hasSummaryType) == null ? void 0 : _b.call(_a, cols[idx]))) {
            result.cell = idx;
            break;
          }
        }
        result.colspan = 0;
        for (var idx = result.cell + 1; idx < cols.length; idx++) {
          var col2 = cols[idx];
          if (!((_d = (_c = this.options).hasSummaryType) == null ? void 0 : _d.call(_c, col2)) && !!(col1 == null ? void 0 : col1.frozen) === !!(col2 == null ? void 0 : col2.frozen)) {
            result.colspan++;
          } else
            break;
        }
        result.colspan = Math.max(1, result.colspan);
        return result;
      };
      this.getGroupRowMetadata = (item) => {
        const opt = this.options;
        const gcp = this.groupCellPosition();
        const result = {
          selectable: false,
          focusable: opt.groupFocusable,
          cssClasses: opt.groupCssClass + " " + opt.groupLevelPrefix + (item == null ? void 0 : item.level),
          columns: {
            [gcp.cell]: {
              colspan: gcp.colspan,
              cssClasses: opt.groupCellCssClass,
              format: opt.groupFormat,
              editor: null
            }
          }
        };
        if (opt.groupRowTotals)
          result.format = opt.totalsFormat;
        return result;
      };
      this.getTotalsRowMetadata = (item) => {
        var _a;
        const opt = this.options;
        return {
          selectable: false,
          focusable: opt.totalsFocusable,
          cssClasses: opt.totalsCssClass + " " + opt.groupLevelPrefix + ((_a = item == null ? void 0 : item.group) == null ? void 0 : _a.level),
          format: opt.totalsFormat,
          editor: null
        };
      };
      var _a, _b, _c, _d;
      this.options = Object.assign({}, _GroupItemMetadataProvider.defaults, opt);
      (_b = (_a = this.options).groupFormat) != null ? _b : _a.groupFormat = (opt == null ? void 0 : opt.groupFormatter) ? convertCompatFormatter(opt.groupFormatter) : (ctx) => _GroupItemMetadataProvider.defaultGroupFormat(ctx, this.options);
      (_d = (_c = this.options).totalsFormat) != null ? _d : _c.totalsFormat = (opt == null ? void 0 : opt.totalsFormatter) ? convertCompatFormatter(opt.totalsFormatter) : (ctx) => _GroupItemMetadataProvider.defaultTotalsFormat(ctx, this.grid);
    }
    static defaultGroupFormat(ctx, opt) {
      opt != null ? opt : opt = _GroupItemMetadataProvider.defaults;
      let item = ctx.item;
      if (!opt.enableExpandCollapse)
        return item == null ? void 0 : item.title;
      let indentation = item.level * opt.groupIndentation;
      return `<span class="${ctx.escape(opt.toggleCssClass + " " + (item.collapsed ? opt.toggleCollapsedCssClass : opt.toggleExpandedCssClass))}" style="margin-left: ${indentation}px">
<span class="${ctx.escape(opt.groupTitleCssClass)}" level="${ctx.escape(item.level)}">${item.title}</span>`;
    }
    static defaultTotalsFormat(ctx, grid) {
      var _a, _b, _c, _d, _e, _f;
      var item = ctx.item;
      if (!item.__groupTotals && item.totals)
        item = item.totals;
      return (_f = (_e = (_b = (_a = ctx.column) == null ? void 0 : _a.groupTotalsFormatter) == null ? void 0 : _b.call(_a, item, ctx.column)) != null ? _e : (_d = (_c = grid != null ? grid : ctx.grid) == null ? void 0 : _c.groupTotalsFormatter) == null ? void 0 : _d.call(_c, item, ctx.column)) != null ? _f : "";
    }
    init(grid) {
      this.grid = grid;
      grid.onClick.subscribe(this.handleGridClick);
      grid.onKeyDown.subscribe(this.handleGridKeyDown);
    }
    destroy() {
      var _a, _b;
      if (this.grid) {
        (_a = this.grid.onClick) == null ? void 0 : _a.unsubscribe(this.handleGridClick);
        (_b = this.grid.onKeyDown) == null ? void 0 : _b.unsubscribe(this.handleGridKeyDown);
      }
    }
    getOptions() {
      return this.options;
    }
    setOptions(value) {
      Object.assign(this.options, value);
    }
  };
  var GroupItemMetadataProvider = _GroupItemMetadataProvider;
  GroupItemMetadataProvider.defaults = {
    enableExpandCollapse: true,
    groupCellCssClass: "slick-group-cell",
    groupCssClass: "slick-group",
    groupFocusable: true,
    groupIndentation: 15,
    groupLevelPrefix: "slick-group-level-",
    groupTitleCssClass: "slick-group-title",
    hasSummaryType: (col) => col.summaryType && col.summaryType != -1,
    totalsCssClass: "slick-group-totals",
    toggleCssClass: "slick-group-toggle",
    toggleCollapsedCssClass: "collapsed",
    toggleExpandedCssClass: "expanded",
    totalsFocusable: false
  };
  return __toCommonJS(groupitemmetadataprovider_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._; Slick.Data = Slick.Data || {}; Slick.Data.GroupItemMetadataProvider = Slick.GroupItemMetadataProvider;
