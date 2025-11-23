## 9.2.0 (2025-11-24)

### Features
- This will be the last version using .NET 8, only minor 9.2.x versions from `net8` branch may be released to fix any critical issues.
- Instead of using sleek-vars.rtl sleek-vars.ltr etc. change the values of --l, --r variables based on rtl. Increase number of supported columns to 100 from 50 which should be enough for most cases.
- Add styleNonce option to SleekGrid constructor support more strict CSP directives. If not passed, it will be looked up from a meta element with csp-nonce name, or style/script with nonce value in document head.
- Replace inline style attributes to potentially support stricter CSP directives. Currently only CKEditor 4 is an issue as it requires unsafe-inline.
- Configured `HtmlNoteContentEditor` and `HtmlReportContentEditor` to use `Tiptap` instead of `CKEditor` (StartSharp). See tiptap-init.ts file in latest template. Tiptap usage is currently opt-in. CKEditor support will be removed in a future version as it is out-of-date and is not compatible with strict CSP.
- Add helpers to add / get CSP directives, both from the layout and per specific pages (like emailclient page which uses CKEditor 4 that requires unsafe-inline)
- New `useUpdatableComputed` helper in `domwise` to create a factory for computed signals that can be manually updated as a batch (by internally incrementing a counter signal). This may be useful for computed signals that depend not only on other signals, but externally changing state.

### Bugfixes
- Fix potential issue with SlickFormatting.treeToggle when formatter returns a string, and enableHtmlRendering is false. Update the default comment in FormatterContext.enableHtmlRendering

## 9.1.7 (2025-11-19)

### Bugfixes
- Null flags issue when restoring persisted settings

## 9.1.6 (2025-11-18)

### Features
- Improve column picker dialog toggle all handling so that only one populate necessary when multiple columns are shown
- Stored the used persistence flags in persisted grid settings.
- Improve HTML content editor validation handling
- Add getFilteredItems method to IRemoteView interface, remove comments from RemoteView methods that already has comments in IRemoteView
- Bump glob to 12.0.0, vitest to 4.0.10, jsdom to 27.2.0
- @serenity-is/tsbuild 9.1.6 with updated glob dependency

### Bugfixes
- Fix gridradioselectionmixin using html string

## 9.1.5 (2025-11-17)

### Features
- Enhance type system for attributes and interfaces. Add CustomAttribute base class for extensible attribute support and attribute typing. Introduce AttributeSpecifier union type for flexible attribute specification (classes, instances, factories). Update registerClass, registerEditor, registerFormatter to handle AttributeSpecifier. No need to call Attributes.factoryFunction() as it will be called by registration helper automatically (e.g. `[Attributes.someAttr]` is enough). Similarly, new `SomeAttributeClass()` is not necessary when passing attributes.

### Bugfixes
- Potential this issue in setTimeout for asyncPostProcess
- Fix wrong isImmediatePropagationStopped check in onDragInit causing inputs to not get focus in the grid

## 9.1.4 (2025-11-16)

### Features
- New Column Picker Dialog which only has one set of columns (e.g. not split by visible / hidden) and has checkboxes to toggle visibility. It also applies changes as soon as a checkbox is clicked or columns dragged. The new one can also work with raw SleekGrid, or any other component with columns as long as necessary callbacks are provided.
- Use the IRemoteView interface in RemoteView events
- Provide more info from sleekgrid.getLayoutInfo method about column pinning / frozen rows ability

### Bugfixes
- Fix actions dropdown visibility issue due to slick-cell having a z-index value in base.css
- Fix some actions in header menu failing due to losing this context when calling preventDefault

## 9.1.3 (2025-11-14)

### Bugfixes
- Fix an issue with System.Data.Common reference in sergen g

## 9.1.2 (2025-11-14)

### Features
- Redesigned SleekGrid event system as the previous one caused some issues when it is used in a site that has jQuery loaded. Only a subset of Args properties (grid, column, row, cell etc) are passed through the event argument, while the rest can be accessed via second args parameter, or e.args.
- Changed view namespace for .cshtml files to `MyProject.Views` (for /Modules) and `MyProject.Views.Shared` (for /Views) by default to avoid potential namespace problems

### Bugfixes
- Converted formatters in Serene Translation page to JSX syntax as HTML strings no longer supported by default
- Add coalesce to quickFilterParams as Object.assign fails when it is null

## 9.1.1 (2025-11-11)

### Features
- Add submitOnEnter and closeOnEscape options to PromptDialog (defaults to true, can be modified by setting PromptDialog.defaultOptions). 
- Add PromptDialog.getEditor method to access the editor. 
- Pass known options from editor options to the autonumeric instance from integer/decimal editors.
- Improve login page 2FA code entering UI experience (StartSharp).

## Bugfixes
- Add `[hidden] *` to validation ignore default list, just like `.hidden *`.

## 9.1.0 (2025-11-10)

### Features

- `@serenity-is/domwise` is our new library to create DOM elements with full TypeScript support. It is primarily based on [jsx-dom](https://github.com/alex-kinokon/jsx-dom) with integrated signal support via [@preact/signals-core](https://github.com/preactjs/signals) and some extra types / ideas from [ryansolid/dom-expressions (SolidJS)](https://github.com/ryansolid/dom-expressions) and [lusito/tsx-dom](https://github.com/Lusito/tsx-dom). `@serenity-is/domwise` also exports signal functions like `signal`, `computed`, `effect` etc. from `@preact/signals-core` and supports signals as attribute values and element children, and reactively updates the attributes and dom nodes when signal values change. Please remove `jsx-dom` from your `package.json`, and set the `jsxImportSource` to `@serenity-is/domwise`. There are a few differences between `jsx-dom` and `domwise`. The HTML element attributes are lowercased to match the actual HTML attribute casing, e.g., `readonly` instead of `readOnly`, `for` instead of `htmlFor`. We aliased some like `tabIndex`, `htmlFor`, `maxLength` etc. for easier transition. This should make it easier to copy/paste actual HTML code to our `.tsx` files, e.g., from a Bootstrap sample. For compatibility, event handlers are left as camelCase, e.g., onClick.
- The layout system for SleekGrid is completely rewritten, and we now introduce `EnhancedLayout` (StartSharp), which is an improved and feature-complete version of previously available (but unused) `FrozenLayout`. It supports pinning columns to the left (start) or right (end) and frozen columns at the top or bottom. `EnhancedLayout` is enabled by default for all grids in StartSharp, though pinning / frozen rows are opt-in via column menu / options. If you were using `FrozenLayout` in some of your grids (by overriding `getSlickOptions`), it is recommended to remove it.
- Columns now have a functional dropdown menu with actions to hide columns, show column picker, reset columns, edit column filter, change sort order, move the column to the left or right, pin columns (EnhancedLayout), auto-size the column (AutoColumnWidthMixin), group by the column (DraggableGroupingMixin), and set the Summary/Aggregate types (CustomSummaryMixin).
- Automatic Registration for Grid Mixins/Plugins (StartSharp): All the available grid plugins / mixins like `HeaderMenuPlugin`, `AutoColumnWidthMixin`,  `CustomSummaryMixin`, `DraggableGroupingMixin` and `HeaderFiltersMixin` now provide an option to automatically register with all grids in the application. It is also possible to limit the registration to a subset of the grids via a predicate. Auto-registration is configured in StartSharp via `Modules/Common/script-init/autoregister-init.ts` file where you may customize the auto-registration process and override options for the auto-registered instance.
- HeaderFiltersMixin provides an option (`showButtonWhenNotFiltered`) to hide the filter button when the column is not filtered. This is useful to save space for the column header itself.
- DraggableGroupingMixin has an option named `showPanelWhenNoGroups` that, if set to false, hides the grouping bar (one with text `Drag a column here...`) when there are no grouped columns.
- CustomSummaryMixin offers an option named `showFooterWhenNoSummaries` that, if set to false, hides the grid footer when there are currently no columns with summaries. It also shows indicator symbols for the current aggregate type.
- Renamed `[ScriptSkip]` attribute to `[TransformIgnore]` attribute to better match the intent, e.g., ignoring the class or property during Sergen/Pro.Coder transforms from C# to TypeScript. `ScriptSkip` still exists but is deprecated. The old name felt a bit like 'skip this on the client/script side'.
- Renamed `[Ignore]` attribute to `IgnoreUIField` to better match the intent, e.g., ignoring the generation of column / form fields for that property. `Ignore` attribute still exists but is deprecated. The old name was not clear enough.
- Renamed `IgnoreNameAttribute` to `SkipNameCheckAttribute` to better match the intent. `IgnoreName` attribute still exists but is deprecated.
- [OneWay] attribute is also obsolete; use the [SkipOnSave] attribute. There is now a [SkipOnLoad] attribute which does for loading what [OneWay]/[SkipOnSave] does for saving. We also now have an [Unbound] attribute, which when placed on a column property, will cause the column to have no field, and act like a mix of [SkipOnSave, SkipOnLoad, SkipNameCheck].
- Renamed `Grid` (@serenity-is/sleekgrid) to `SleekGrid` to avoid mix-ups with `DataGrid`, `EntityGrid` and other grid types.
- New `Attributes` namespace which works similarly to the `@Decorators` namespace for [Symbol.typeInfo] style type registration, e.g., [Attributes.panel()] instead of [new PanelAttribute()]
- The `FilterableAttribute` which used to control advanced filtering on grids via a bottom filter bar and an advanced filter dialog is renamed to `AdvancedFilteringAttribute` which you may use via `Attributes.advancedFiltering()`.
- When there is no active advanced filter, the filter bar will no longer be shown to save space. To make it possible to edit an advanced filter when the filter bar is hidden, we added a tool button next to the column picker. The filter bar is also moved to the top of the grid. This will make it easier to identify when a grid has an advanced filter.
- The getColumns method is deprecated (DataGrid/EntityGrid). We had a `protected getColumns()` method in `DataGrid` subclasses that is used to create the initial column set for a grid, but because its name matched the `SleekGrid`'s `getColumns()` method that returned the current set of visible columns in the grid, sometimes users tried to call this method to get the current columns, instead of `dataGrid.sleekGrid.getColumns()`. Please override `protected createColumns()` method instead which better matches the intent. To access currently visible columns for a data/entity grid, you may use `.columns` property, or `.sleekGrid.getColumns()`. To access all the columns (including invisible ones, not necessarily matching the visible order) use `.allColumns` property or `.sleekGrid.getAllColumns()`.
- For compatibility with jQuery, we previously chose to set `style.display` to `none` when an element is hidden via `Fluent.toggle` methods. We now decided to use the `hidden` attribute which applies to all element types, and actually hides the element even if the `style.display` property is overridden by a Bootstrap class like `d-block`. Fluent tries to handle this transition gracefully, but it is recommended to update the logic in your own files.
- SleekGrid now has a better column dragging system that also works on touch devices.
- Rewrote the layout system of the SleekGrid from the ground up. It now uses a `CSS Grid` layout for the main panels / viewport.
- SleekGrid is now able to handle dynamic changes of pinned columns and frozen rows. It also handles them responsively so that when the viewport gets small (e.g., on mobile devices) or when there are not enough data rows, the number of pinned columns and frozen rows is temporarily reduced to make the scrollable area visible.
- Note that SleekGrid that was hosted as a submodule inside Serenity repository, is now embedded as a sub-tree. If you are using Serenity as a submodule, you may need to rename / delete `Serenity/packages/sleekgrid` folder before pulling latest changes.
- To avoid direct dependency on SleekGrid and RemoteView classes, we now have interface versions of these types, ISleekGrid and IRemoteView. This may affect you if you referenced one of these types directly.
- SleekGrid now has an improved and more secure HTML sanitizer, based on DOMParser. Even though we enable DOMPurify in our application templates instead of this sanitizer, the built-in sanitizer should be sufficient for most cases. The old regex-based basic sanitizer could fail in some complex cases. `@serenity-is/corelib` now also has a `sanitizeHtml` function that uses SleekGrid's configured one if available, DOMPurify or its own DOMParser-based sanitizer. We recommend using this function where you need HTML sanitization for user-provided content (unless you directly reference DOMPurify).
- Strings Returned From Formatters Are Assumed To Be Text (`**[Breaking Change]**`) as `gridDefaults.enableHtmlRendering` is now false by default. This means strings returned from formatters are assumed to be text, not raw HTML. You should convert those formatters to return HTML elements (via `domwise` etc). Note that when this flag is false (default now), the ctx.escape() function will return strings as is and will not HTML-encode them, as it would otherwise result in double HTML escaping.
If you want to revert to the previous behavior (not recommended), set gridDefaults.enableHtmlRendering = true in ScriptInit.ts etc.
- Added a `MVC:AsNamespace` option to sergen.json which causes a `.MVC` namespace suffix to be generated instead of an `MVC` class. When enabled, the `ESM` helper class will also be generated under this namespace, e.g., `RootNamespace.MVC`. This should reduce confusion when a namespace like `Serenity.Extensions` is added to global usings as it also has its own `MVC` helper. The default is false for now, as it would be a problem for `ESM.` references, as they would need to be written as `MVC.ESM.` or an alias would need to be assigned in the project file like `<Using Include="$(MSBuildProjectName).MVC.ESM" Alias="ESM" />`. New projects have this alias defined by default. The new `defaults@9.0.0` in `sergen.json` can be used instead of `defaults@6.6.0` to enable this flag by default (new projects will also use `defaults@9.0.0`).
- Added VSIX support for Visual Studio 2026
- Added comments to HeaderFiltersPlugin options. Added a getFilterText option to override the displayed value. Pass 'header-filter' as purpose to formatters. Allow passing more options through the mixin to the plugin. Added more options to checkbox/boolean formatter and special handling for 'header-filter' purpose
- Added tests for broken source links to Serenity demo pages
- The source links for sample pages in Demo now point to the commit used at the time of publishing instead of the master branch.
- New SleekGrid formatterContext helper that should be used instead of directly creating a formatter context as this helper will set sanitizer, escape and other arguments correctly based on global / grid options.
- Deprecated groupTotalsFormatter and use groupTotalsFormat that accepts a FormatterContext instead, similar to column.format
- Added a sample to show country flags in Customer / Supplier pages.
- New `bindThis(this).someFunction` helper method in `@serenity-is/sleekdom` to auto-bind class functions instead of `this.someFunction.bind(this)` which creates a new function every time, and makes unsubscribing from events difficult.
- `**[Breaking Change]**` Instead of a `.hidden` class we now use HTML standard "hidden" attribute/property.
- Added a debounceTimes option to LayoutTimer to avoid momentarily changing dimensions to trigger layout events.
- Purpose parameter passed to formatters to identify for what purpose the formatter is called like 'header-filter', 'group-total', etc.
- Removed legacy Exception, ArgumentException classes.
- Converted RemoteView into a proper class with private and public members
- Removed unused inlineFilters option from RemoteView.
- Removed legacy fastSort function from RemoteView.
- Converted aggregators to proper classes, removed pre-compiled aggregators
- Moved JS reserved keyword list to CodeWriter, commented JS reserved words while generating texts.ts.
- Added asKey() and asTry() methods to generated text namespaces which will be handled by proxyTexts.
- Converted type registries to proper classes instead of a helper function
- Added isSafeReturnUrl function, and used it in getReturnUrl instead of the simple check
- Simplified Unicode email validation regex
- Used AuthorizeRetrieve attribute in generated endpoint Retrieve method
- New AggregatorTypeRegistry that registers Aggregator types that contain summaryType, aggregateType, and displayName static properties
- Merged ReflectionUtils and ReflectionOptionsSetter
- Updated handler templates to use [GenerateFields] and use the `.RequestHandlers` namespace suffix if EnableGenerateFields is true. Got rid of MySaveRequest etc. aliases in handler templates.
- Parse global usings from project and allow passing them via -globalusings to Sergen
- Updated many NPM/NuGet packages, including System/ASP.NET Core ones to 9.0.10, esbuild to 0.27.0, Microsoft.Data.SqlClient to 6.1.2, Microsoft.TypeScript.MSBuild to 5.9.3.

### Bugfixes
- When any cell in the last row of a full size grid with a `Select2` dropdown is clicked, as `Select2` tries to position its dropdown momentarily, a page scrollbar is shown. This triggered a layout event in the grid, causing it to hide the editor due to a full re-render. Another similar issue was present in the `Product` page with legacy editor inputs and the Android touch keyboard. These issues are now fixed, and `SleekGrid` should perform better with mobile devices.
- Fixed some potential compatibility issues with .NET 10 like Enumerable.Reverse/Array.Reverse(span) overloads.
- Fixed SMS two factor method raising an exception (StartSharp) when user does not have a mobile number
- Dump SQL server parameter types properly if parameter is a SqlClient.SqlParameter

## 9.0.0 (2025-09-15)

### Features

- **Type registration via decorators** (e.g., `Decorators.registerClass`, `Decorators.registerEditor`, etc.) is now deprecated (it still works but is not recommended):

    ```ts
    import { Decorators, Widget } from "@serenity-is/corelib";

    @Decorators.registerClass("MyProject")
    export class MyType extends Widget<any> {
    }
    ```

    Instead, use a static `[Symbol.typeInfo]` property declaration:

    ```ts
    import { Widget } from "@serenity-is/corelib";

    export class MyType extends Widget<any> {
        static [Symbol.typeInfo] = 
            this.registerClass("MyProject.MyModule.MyType");
    }
    ```

    This new approach offers several advantages over decorators. Most importantly, decorator information is not emitted by TypeScript in declaration files (e.g., `.d.ts`), making it difficult or impossible to identify registration names for referenced project/npm package types.

    Another reason for this change is a longstanding bug in esbuild ([see issue](https://github.com/evanw/esbuild/issues/4087)) with decorators and property initializers, which currently only has a workaround by using `experimentalDecorators: true` in `tsconfig.json`.

    The new registration methods also support passing interfaces or attributes as an array in the second argument. For example, to set `@Decorators.panel(true)` on a class, you can do:

    ```ts
    import { Widget, PanelAttribute } from "@serenity-is/corelib";

    export class MyType extends Widget<any> {
        static [Symbol.typeInfo] = 
            this.registerClass("MyProject.MyModule.MyType", 
                [new PanelAttribute(true)]);
    }
    ```

    Note: When using this new registration method, TypeScript will not allow any decorators on the containing type, as the `this` expression is referenced in a static field initializer. If you need to use third-party decorators, you can use an alternative registration style:

    ```ts
    import { Widget, classTypeInfo, registerType } from "@serenity-is/corelib";

    @someCustomDecorator("test")
    export class MyType extends Widget<any> {
        static [Symbol.typeInfo] = 
            classTypeInfo("MyProject.MyModule.MyType"); 
        static { registerType(this); }
    }
    ```

    This is equivalent to `this.registerType`, as the `registerType` static method in `Widget` calls `classTypeInfo` and `registerType` internally.

    This style is also useful for formatter registration, as formatters typically do not have a base type and cannot simply call `this.registerType` like widget subclasses:

    ```ts
    export class MyFormatter implements Formatter {
        static [Symbol.typeInfo] = 
            formatterTypeInfo("MyProject.MyModule.MyFormatter"); 
        static { registerType(this); }
    }
    ```

    For consistency, you may also choose to extend the new `FormatterBase` class:

    ```ts
    export class MyFormatter extends FormatterBase implements Formatter {
        static [Symbol.typeInfo] = 
            this.registerFormatter("MyProject.MyModule.MyFormatter");
    }
    ```

    For all registration styles, you can now pass only the namespace ending with a dot (the enclosing type name will be auto-appended):

    ```ts
    import { Widget } from "@serenity-is/corelib";

    export class MyType extends Widget<any> {
        static [Symbol.typeInfo] = this.registerClass("MyProject.MyModule.");
    }
    ```

    Sergen / Serenity.Pro.Coder now also generates namespace constants under the `Modules/ServerTypes/Namespaces.ts` file, so you can use them to avoid typos:

    ```ts
    import { Widget } from "@serenity-is/corelib";
    import { nsMyModule } from "../../ServerTypes/Namespaces";

    export class MyType extends Widget<any> {
        static [Symbol.typeInfo] = this.registerClass(nsMyModule);
    }
    ```

    We hope this will reduce mistakes when renaming classes or using a namespace with mismatched casing.

- Added an `EditLink` function that can be used instead of `SlickHelpers.itemLink` with JSX elements. The `DataGrid` also has an `EditLink` arrow function for use in formatters.
- The row type itself is now used as the base row type if a row type is passed to the `PropertyItemProvider`.
- Imports generated by `ServerTypingsGenerator` are now ordered and organized.
- You can now pass `formatterType` by reference in `PropertyItem`.
- Added a `value` argument to the `Validator.optional` method, allowing validation methods to be called without an element—just a value—when desired.
- Adjusted the `Validator`'s `onfocusout` method: if a required input is cleared and loses focus, it is still validated if it has a "required" rule. Otherwise, errors were not shown until the form is saved.
- Introduced a new Inline Editing sample in the Grid Editor (StartSharp).
- Added new options and improved features in `GridEditController` (StartSharp):
    - New `getPropertyItem` option to optionally override the column item.
    - The `bulkSaveHandler` option now works.
    - The `saveHandler` callback receives an `args` argument with a `defaultHandler` property, allowing you to call the default save handler when needed.
- Splitted WizardDialog's renderContents method into renderButtons, renderCancelButton, renderBackButton, renderNextButton etc. to make it easier to modify/inject content (StartSharp)

## 8.8.9 (2025-09-03)

## Features
- Extract commit ID from assembly / nuget package generated by SourceLink and use that instead of master branch to link to source files for demo
- Containg folder name of several projects are changed (for example Serenity\src\web\Serenity.Net.Web.csproj instead of Serenity\src\Serenity.Net.Web\Serenity.Net.Web.csproj). This may only affect you if you are using Serenity via submodules.
- Updated OpenIddict to 7.0.0

### Bugfixes
- Use Fluent.addClass as opt.cssClass passed to quick filter element might contain spaces

## 8.8.8 (2025-08-23)

### Features
- Added Html.CspNonce() helper function and made it possible to optionally enable CSP by uncommenting and adjusting Content-Security-Policy meta element in _LayoutHead.cshtml
- Also register addon option values as local text keys if they match the candidate regex
- Run TSBuild target before ResolveProjectStaticWebAssets abd clean content items to resolve issues with static web assets issues in NET9 SDK.
- Add npmCopy helper function to tsbuild that copies files under node_modules/ to wwwroot/npm/ folder. Used it to copy flatpickr, moment, chartjs and other scripts used in DashboardIndex to wwwroot/npm for use in appsetting.bundles.json. The entries in appsettings.bundles.json that start with ~/npm/ will be automatically copied from node_modules/ to wwwroot/npm/ by default.
- Add Modules/**/*.mts to default entry points
- Add ability to localize input add-ons text/hint if the value is a local text key
- Converted UI samples to use .tsx instead of .cshtml files
- Exception Log page is now using `Administration:ExceptionLog` permission instead of `Administration:Security` permission.
- Updated .NET libraries including:
    - ASP.NET Core framework packages to 9.0.8
    - ClosedXML to 0.105.0
    - FirebirdSql.Data.FirebirdClient to 10.3.3
    - FluentMigrator to 7.1.0
    - Microsoft.Data.SqlClient to 6.1.1
    - Microsoft.TypeScript.MSBuild to 5.9.2
    - NUglify to 1.21.17
    - Npgsql to 9.0.3
    - SixLabors.ImageSharp to 2.1.11
- Updated NPM packages including:
    - chart.js to 4.5.0
    - dompurify to 3.2.6
    - esbuild to 0.25.9
    - glob to 11.0.3
    - preact to 10.27.1
    - typescript to 5.9.2
    - vitest to 3.2.4
- `**[Breaking Change]**` Split IInsertLogRow into IInsertDateRow and IInsertUserRow subinterfaces. Split IUpdateLogRow similarly. Replace IInsertLogRow.InsertUserIdField with IInsertUserIdRow.InsertUserIdField, IUpdateLogRow.UpdateUserIdField with IUpdateUserIdRow.UpdateUserIdField, IInsertLogRow.InsertDateField with IInsertDateRow.InsertDateField, and IUpdateLogRow.UpdateDateField with IUpdateDateRow.UpdateDateField if you implemented these interfaces in your rows
- **`[Breaking Change]`** Removed following scripts from Serenity.Assets. If you still need these legacy scripts (e.g. in appsettings.bundles.json) you may install them via npm / libman.json or use them via CDN:
    - jquery.colorbox (prefer glightbox from npm, we use it in StartSharp now)
    - jquery.cookie
    - jquery.event.drag
    - jquery.maskedinput
    - jquery-ui
    - jquery-ui-i18n
    - jspdf (if not available, PDFExportHelper will try to load it from CDNJS by default)
    - jspdf.autotable (if not available, PDFExportHelper will try to load it from CDNJS by default)

- **`[Breaking Change]`** Removing ckeditor from Serenity.Assets. HtmlContentEditor will load it (v4.22.1) from CDNJS by default. Please remove line `HtmlContentEditor.CKEditorBasePath = "~/Serenity.Assets/Scripts/ckeditor/"` from ScriptInit.ts.

### Bugfixes
- Resolve issue with multiple file drag drop on uploader due to event.dataTransfer.items array getting lost during async call
- Disable each individual radio button for RadioButtonEditor when switching read only, closes #7372
- Fix extending entry points with "+" does not work in tsbuild

## 8.8.6 (2025-05-31)

### Features
- A new Interface source generator that may automatically generate relevant service handler interfaces for handler types with `[GenerateInterface]` attribute.
- Update esbuild to 0.25.5. Warning! Please set `"experimentalDecorators": true` in your tsconfig.json as esbuild has an unresolved bug with decorators and initializers (https://github.com/evanw/esbuild/pull/4092). You may have script errors otherwise!
- Introduced experimental DateOnlyField type. This required changing target framework of Serenity.Net.Services to net8 (was netstandard2.1).
- Add Oracle-specific SQL type mappings to SqlTypeToFieldTypeMap (#7363)
- Add support for partial properties for row fields source generator (requires NET9+)

### Bugfixes
- Also invalidate UserRole / UserPermission cache group when saving user as LinkingSetRelation might be updating roles 

## 8.8.5 (2025-03-09)

### Features

- Update SixLabors.ImageSharp to 2.1.10

### Bugfixes

- Fix quoting of UPPER field expression for case sensitive like dialects

## 8.8.4 (2025-03-05)

### Features

- Add ability to intercept some SQL operations done through SqlHelper and EntityConnectionExtensions by implementing ISqlOperationInterceptor and/or IRowOperationInterceptor in the mock connection class.
- Switched to vitest from jest for javascript tests
- Try to avoid issue when StringField reads value via Newtonsoft.Json and a DateTime token is received
- Updated nuget/npm packages
- New writeIfChanged option in tsbuild to accelerate builds in VS
- Also allow DefaultHandler(false) to skip a handler when multiple handlers for a type is found
- AuthorizeRetrieveAttribute that may be used for endpoint Retrieve methods
- Go back to StackExchange.Exceptional from our temporary fork (Serenity.Exceptional.AspNetCore) as in 3.0.1 version they also switched to Microsoft.Data.SqlClient


### Bugfixes
- Fix PasswordStrengthValidator not correctly validating MembershipSettings.RequireDigit rule (#7306)


## 2024

All Serenity versions published in the year 2024 (versions 8.2.1 through 8.8.3) can be found in [changelog-2024.md](docs/changelog/changelog-2024.md).

## 2023

All Serenity versions published in the year 2023 (versions 6.4.3 through 8.1.5) can be found in [changelog-2023.md](docs/changelog/changelog-2023.md).

## 2022

All Serenity versions published in the year 2022 (versions 5.1.2 through 6.4.2) can be found in [changelog-2022.md](docs/changelog/changelog-2022.md).

## 2021

All Serenity versions published in the year 2021 (versions 5.0.20 through 5.1.1) can be found in [changelog-2021.md](docs/changelog/changelog-2021.md).

## 2020

All Serenity versions published in the year 2020 (versions 3.10.0 through 3.14.5) can be found in [changelog-2020.md](docs/changelog/changelog-2020.md).

## 2019

All Serenity versions published in the year 2019 (versions 3.8.4 through 3.9.14) can be found in [changelog-2019.md](docs/changelog/changelog-2019.md).

## 2018

All Serenity versions published in the year 2018 (versions 3.3.15 through 3.8.3) can be found in [changelog-2018.md](docs/changelog/changelog-2018.md).

## 2017

All Serenity versions published in the year 2017 (versions 2.8.0 through 3.3.14) can be found in [changelog-2017.md](docs/changelog/changelog-2017.md).

## 2016

All Serenity versions published in the year 2016 (versions 1.8.19 through 2.7.2) can be found in [changelog-2016.md](docs/changelog/changelog-2016.md).

## 2015

All Serenity versions published in the year 2015 (versions 1.4.7 through 1.8.18) can be found in [changelog-2015.md](docs/changelog/changelog-2015.md).

## 2014

All Serenity versions published in the year 2014 (versions 1.3.0 through 1.4.6) can be found in [changelog-2014.md](docs/changelog/changelog-2014.md).