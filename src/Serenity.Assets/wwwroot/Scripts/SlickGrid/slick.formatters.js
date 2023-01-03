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

  // src/formatters/index.ts
  var formatters_exports = {};
  __export(formatters_exports, {
    CheckboxFormatter: () => CheckboxFormatter,
    CheckmarkFormatter: () => CheckmarkFormatter,
    Formatters: () => Formatters,
    PercentCompleteBarFormatter: () => PercentCompleteBarFormatter,
    PercentCompleteFormatter: () => PercentCompleteFormatter,
    YesNoFormatter: () => YesNoFormatter
  });

  // global-externals:_
  var { escape } = Slick;

  // src/formatters/formatters.ts
  function PercentCompleteFormatter(ctx) {
    if (ctx.value == null || ctx.value === "")
      return "-";
    if (ctx.value < 50)
      return "<span style='color:red; font-weight:bold;'>" + ctx.escape() + "%</span>";
    return "<span style='color:green'>" + escape(ctx.value) + "%</span>";
  }
  function PercentCompleteBarFormatter(ctx) {
    if (ctx.value == null || ctx.value === "")
      return "";
    var color;
    if (ctx.value < 30)
      color = "red";
    else if (ctx.value < 70)
      color = "silver";
    else
      color = "green";
    return "<span class='percent-complete-bar slick-percentcomplete-bar' style='background:" + color + ";width:" + escape(ctx.value) + "%' title='" + escape(ctx.value) + "%'></span>";
  }
  function YesNoFormatter(ctx) {
    return ctx.value ? "Yes" : "No";
  }
  function CheckboxFormatter(ctx) {
    return `<i class="slick-checkbox slick-edit-preclick${ctx.value ? " checked" : ""}"></i>`;
  }
  function CheckmarkFormatter(ctx) {
    return ctx.value ? '<i class="slick-checkmark"></i>' : "";
  }

  // src/formatters/index.ts
  var Formatters;
  ((Formatters2) => {
    function PercentComplete(_row, _cell, value) {
      return PercentCompleteFormatter({ escape, value });
    }
    Formatters2.PercentComplete = PercentComplete;
    function PercentCompleteBar(_row, _cell, value) {
      return PercentCompleteBarFormatter({ escape, value });
    }
    Formatters2.PercentCompleteBar = PercentCompleteBar;
    function YesNo(_row, _cell, value) {
      return YesNoFormatter({ escape, value });
    }
    Formatters2.YesNo = YesNo;
    function Checkbox(_row, _cell, value) {
      return CheckboxFormatter({ escape, value });
    }
    Formatters2.Checkbox = Checkbox;
    function Checkmark(_row, _cell, value) {
      return CheckmarkFormatter({ escape, value });
    }
    Formatters2.Checkmark = Checkmark;
  })(Formatters || (Formatters = {}));
  return __toCommonJS(formatters_exports);
})();
["Data", "Editors", "Formatters", "Plugins"].forEach(ns => Slick._[ns] && (Slick[ns] = Object.assign(Slick[ns] || {}, Slick._[ns])) && delete Slick._[ns]); Object.assign(Slick, Slick._); delete Slick._;
