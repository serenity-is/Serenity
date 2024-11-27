﻿/** 
 * ## Serenity Core Library
 * 
 * This is the package containing core TypeScript classes and functions used in Serenity applications. 
 * 
 * It should be installed by default in your projects created from `Serene` or `StartSharp` template:
 * 
 * ```json
 * {
 *   "dependencies": {
 *     // ...
 *     "@serenity-is/corelib": "latest"
 *   }
 * }
 * ```
 * 
 * The version number for this package should be equal or as close as possible to Serenity NuGet package versions in your project file.
 * 
 * > When using classic namespaces instead of the ESM modules, the types and functions in this module are directly available from the global `Serenity` and `Q` namespaces.
 * > e.g. `Serenity.EntityGrid`
 * @packageDocumentation
 */

export * from "./base";
export * from "./q";
export * from "./slick";
export * from "./interfaces";
export * from "./types";
export * from "./patch";
export * from "./ui/datagrid/irowdefinition";
export * from "./ui/datagrid/islickformatter";
export * from "./ui/helpers/lazyloadhelper";
export * from "./ui/widgets/prefixedcontext";
export * from "./ui/widgets/widgetutils";
export * from "./ui/widgets/widget";
export * from "./ui/widgets/toolbar";
export * from "./ui/dialogs/basedialog";
export * from "./ui/widgets/basepanel";
export * from "./ui/editors/editorwidget";
export * from "./ui/editors/cascadedwidgetlink";
export * from "./ui/helpers/tabsextensions";
export * from "./ui/widgets/reflectionoptionssetter";
export * from "./ui/widgets/propertygrid";
export * from "./ui/widgets/propertypanel";
export * from "./ui/helpers/subdialoghelper";
export * from "./ui/dialogs/dialogextensions";
export * from "./ui/dialogs/propertydialog";
export * from "./ui/editors/editorutils";
export * from "./ui/editors/stringeditor";
export * from "./ui/editors/passwordeditor";
export * from "./ui/editors/textareaeditor";
export * from "./ui/editors/booleaneditor";
export * from "./ui/editors/autonumeric";
export * from "./ui/editors/decimaleditor";
export * from "./ui/editors/integereditor";
export * from "./ui/editors/dateeditor";
export * from "./ui/editors/datetimeeditor";
export * from "./ui/editors/timeeditor";
export * from "./ui/editors/emaileditor";
export * from "./ui/editors/emailaddresseditor";
export * from "./ui/editors/urleditor";
export * from "./ui/editors/radiobuttoneditor";
export * from "./ui/editors/combobox"
export * from "./ui/editors/comboboxeditor";
export * from "./ui/editors/selecteditor";
export * from "./ui/editors/select2";
export * from "./ui/editors/dateyeareditor";
export * from "./ui/editors/enumeditor";
export * from "./ui/editors/lookupeditor";
export * from "./ui/editors/servicelookupeditor";
export * from "./ui/editors/htmlcontenteditor";
export * from "./ui/editors/maskededitor";
export * from "./ui/editors/recaptcha";
export * from "./ui/helpers/uploadhelper";
export * from "./ui/editors/uploadeditors";
export * from "./ui/datagrid/quickfilter";
export * from "./ui/datagrid/quickfilterbar";
export * from "./ui/datagrid/quicksearchinput";
export * from "./ui/filtering/filteroperator";
export * from "./ui/filtering/filterline";
export * from "./ui/filtering/filterstore";
export * from "./ui/filtering/filtering";
export * from "./ui/filtering/filterwidgetbase";
export * from "./ui/filtering/filterpanel";
export * from "./ui/filtering/filterdialog";
export * from "./ui/filtering/filterdisplaybar";
export * from "./ui/datagrid/slickpager";
export * from "./ui/datagrid/idatagrid";
export * from "./ui/helpers/slickhelpers";
export * from "./ui/formatters/formatters";
export * from "./ui/datagrid/datagrid";
export * from "./ui/datagrid/columnpickerdialog";
export * from "./ui/datagrid/treegridmixin";
export * from "./ui/editors/checktreeeditor";
export * from "./ui/datagrid/entitygrid";
export * from "./ui/dialogs/entitytoolbuttons";
export * from "./ui/dialogs/entitylocalizer";
export * from "./ui/dialogs/entitydialog";

export type Constructor<T> = new (...args: any[]) => T;