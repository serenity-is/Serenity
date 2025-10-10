# CHANGELOG 2020

This changelog documents all Serenity versions published in the year 2020 (versions 3.10.0 through 3.14.5).

## 3.14.5 (2020-11-24)

### Features
  - Added support for the ICustomQueryParameter interface of Dapper, which allows control over parameter types such as varchar (ANSI), fixed length, etc.

### Bugfixes
  - Implemented an alternative logic similar to the original one for changeSelect2.

## 3.14.4 (2020-11-23)

### Bugfixes
  - Resolved an issue where changeSelect2 was triggered even when the value was set through the `.value` property, due to a check using `hasClass` instead of `data`.

## 3.14.3 (2020-10-21)

### Bugfixes
  - Prefixed the `__rownum__` alias with `x` as Oracle does not accept it otherwise.

## 3.14.2 (2020-10-19)

### Features
  - Added an option to disable the layout timer in the datagrid.

### Bugfixes
  - Resolved an issue where row numbers over `ORDER BY` did not guarantee order, potentially causing paging issues in rare cases on SQL 2005/2008.
  - Added missing localization for "Set to Now" in the DateTime editor.
  - Ensured the time part of the DateTime editor is readonly when the editor itself is readonly.
  - Corrected improper usage of `keyprefix` for `get` in RedisDistributedCache.
  - Fixed infinite resizing in some cases in the datagrid due to the layout timer.
  - Resolved an issue where directly reading `pathLink.href` resulted in the full address (e.g., `localhost:6543/`) instead of just `/`. Used `getAttribute` to fix `Q.Config.AppPath`.

## 3.14.1 (2020-09-09)

### Bugfixes
  - Fixed an issue where setting a date editor's date to today in UTC minus time zones resulted in the previous day being displayed.
  - Ensured quick search text and field persist after user changes if the `quickSearch` persistence flag is true.

## 3.14.0 (2020-08-06)

### Features
  - Enabled local sorting on remote views, useful for in-memory grid editors. Override `getViewOptions` to enable and set `localSort` to true.
  - Restored the `flexify` decorator for backward compatibility (not recommended for use).

### Bugfixes
  - Resolved an issue where the datepicker icon was not displayed in Serene due to a URL encoding problem.
  - Set the default width of the datepicker image to 16px via CSS, as it is now an SVG.
  - Checked for null in `GetName()` and `GetText()` Enum extension methods (#4252).
  - Restored the `Select2Item` interface under the Serenity namespace.
  - Fixed validation error highlighting for Select2 editors.
  - Prevented the date-time picker from returning the last set value if it was "now" or "today," resolving validation issues on creation.

## 3.13.7 (2020-07-21)

### Bugfixes
  - Included `Serenity.CoreLib.d.ts` under `Scripts/typings/serenity/` for older projects still using it in `tsconfig.json`. If present, update the path to `Scripts/serenity/Serenity.CoreLib.d.ts`.

## 3.13.6 (2020-07-20)

### Bugfixes
  - Re-included `recaptcha` and `maskededitor` in CoreLib.

## 3.13.5 (2020-07-09)

### Features
  - Introduced `FieldInsertPermissionAttribute`, `FieldUpdatePermissionAttribute`, and `FieldModifyPermissionAttribute`, which work similarly to `FieldReadPermissionAttribute` to set relevant permissions on all fields. These can be overridden at the field level.
  - Added the ability to clear the local text registry through a new `IRemoveAll` interface, required for runtime translation reloading in .NET Core, as it is not possible to register a new `ILocalTextRegistry` unlike in the .NET Framework.

## 3.13.4 (2020-06-12)

### Features
  - Added an option to use Bootstrap modals for dialogs instead of jQuery UI.
  - `Q.alert`, `Q.confirm`, etc., now work without jQuery UI and can use Bootstrap if available. They fall back to browser-native dialogs if neither is loaded, though options will be limited to the message.
  - `Q.blockUI` can function without the BlockUI plugin, but only for full-screen blocking.
  - Serenity CoreLib can now be loaded without external scripts like jQuery, jQuery UI, toastr, etc., though functionality will be limited. This is useful for testing and frontend pages.
  - Removed the dependency on Saltaralle `mscorlib.js`. It is no longer required unless your project uses the `ss.` namespace. Most usages can be converted to `Q.` alternatives. Search for `(ss as any)` or `(ss.)` in your code before removing.
  - Introduced split parts of `Serenity.CoreLib.js` so that only required libraries can be loaded when needed. This is useful for frontend apps. The list of libraries includes `serenity-core.js` (contains Q and core types/functions), `serenity-widget.js` (contains the base widget type, UI-related typings, toolbar, etc., depends on core), `serenity-forms.js` (contains validation, templated panel, property grid, property panel, depends on core and widget), `serenity-editors.js` (contains most editor types, depends on core and widget), `serenity-slick.js` (contains RemoteView and other SlickGrid-related types, depends on core), `serenity-dialogs.js` (contains templated dialog, property dialog, and entity dialog, depends on core, widget, and forms), `serenity-quickfilter.js` (contains quick search and quick filter bar, depends on core, widget, and editors), `serenity-filterpanel.js` (contains filter panel and dialog, depends on core, widget, editors, quickfilter, forms, and dialogs), and `serenity-grids.js` (contains data and entity grid, depends on core, Slick, widget, forms, editors, dialogs, quickfilter, and filterpanel).
  - Added optional Flatpickr support for date/time editors. This can be enabled with `DateEditor.useFlatpickr = true` or used automatically when Flatpickr is loaded on the page and jQuery UI datepicker is not.
  - Introduced a new layout watcher that can call layout for elements when their width, height, or visibility changes using a timer. This resolves SlickGrid layout issues.
  - Replaced `COREFX` ifdefs with `NET45` as .NET Core and ASP.NET Core are now the default. This change improves compatibility with other IDEs like Visual Studio Code for .NET Core projects. The OmniSharp extension for VS Code does not work well with `#IFs` and project references.

### Bugfixes
  - Ensured `DateTimeField` does not fail on read if the data reader returns a `DateTimeOffset`.
  - Converted local date/time kinds to universal time before formatting, as the .NET format function does not perform this conversion automatically.

## 3.12.6 (2020-04-12)

### Bugfixes
  - Fixed an issue where the CSS bundle manager removed quotes in URLs for data URIs while adjusting relative paths in the CSS bundle.

## 3.12.5 (2020-04-12)

### Bugfixes
  - Fixed the CSS bundle manager ignoring the `usemincss` setting if the included file already had a `.min.css` extension.
  - Resolved an issue where `jquery.validate` changed the required logic to not trim input values by default, causing whitespace to be considered valid for string inputs.

## 3.12.4 (2020-03-09)

### Bugfixes
  - Ensured that non-null fields with default values are not validated on insert if they are not explicitly assigned, as the default value will be applied automatically. This resolves create service issues with non-null fields having `DefaultValue` attributes where these fields are not visible in the form.

## 3.12.3 (2020-02-22)

### Features
  - If null value handling is set to include, the row converter will also serialize null fields.
  - Defined `JsonSettings.StrictIncludeNulls` and `JsonSettings.TolerantIncludeNulls`, which serialize null values and check them on deserialization. JSON class methods now have an additional `includeNulls` parameter (default = false) to use these settings.
  - Added `NavigationPermissionAttribute`, which takes precedence over `ReadPermissionAttribute` on a row to determine permission for the page and navigation item of the row.
  - Introduced the `AsSqlValue` method, which calls `AsObject` by default for the `Field` object. This is useful for fields with a different storage type than their value type, e.g., `JsonField`.
  - Added the `JsonField<TValue>` type. It has no attributes controlling serialization options like nulls, so such options must be set through the `Field.Settings` property if required.
  - Suppressed errors if the XHR status is `abort`.
  - Added async source support to `Select2Editor`.
  - Added an async option to use `getLookupAsync` for the lookup editor.
  - Introduced the `ServiceLookupEditor` type.
  - Disabled type delay for the initial load and hid the Select2 spinner while waiting for the type delay.
  - Added an async option to `LookupEditor`, making it function like a service lookup editor.
  - Introduced `FieldReadPermissionAttribute`, which determines the default permission required to read fields of a row, optionally excluding `[LookupInclude]` and ID/Name fields. This simplifies using a `ServiceLookupEditor` with a row. For example, by setting `[ReadPermission("Northwind:Lookups")]`, `[FieldReadPermission("Northwind:General", ApplyToLookups = false)]`, and `[NavigationPermission("Northwind:General")]`, only users with `Northwind.General` can see the page and read all fields through the list service/grid, while users with `Northwind.Lookups` permission can call the list service but can only read ID + Name + LookupInclude fields through it.
  - Added the missing `HeaderHtmlUrl` field to `IHtmlToPdfOptions`.
  - Included the PDB file for `serenity.web` for source link debug support.
  - Ensured the connection is open for better compatibility with the MVC version in the Dapper interface.

### Bugfixes
  - Fixed `Q.getLookupAsync` not functioning asynchronously.
  - Avoided double `initSelection` by setting `select2-change-triggered`.
  - Resolved layout timer registration cleanup issues.
  - Ensured the title text is read from the child with the `.title-text` class.
  - Prevented exceptions when determining the URL if a controller has multiple action methods with the same name.

## 3.12.2 (2020-01-11)

### Bugfixes
  - Resolved an issue where `DataGrid.updateInterface` might fail if the grid has no toolbar.
  - Ensured the quick filters bar can still be created within a fake toolbar div, even when the grid has no toolbar.

## 3.12.1 (2020-01-11)

### Bugfixes
  - Reverted updates to `System.Threading.Tasks.Extensions` and `System.Runtime.CompilerServices.Unsafe` in Sergen (NET45).

## 3.12.0 (2020-01-08)

### Bugfixes
  - Fixed an embedded resource issue with the new Sergen tool.

## 3.11.0 (2020-01-08)

### Features
  - Introduced Sergen as a global/local .NET tool since `DotNetCliToolReference` cannot be used with .NET Core 3+. To use it, remove `DotNetCliToolReference` from the CSPROJ file, run `dotnet new tool-manifest` in the CSPROJ directory, followed by `dotnet tool install sergen`, then `dotnet tool restore`, and use `dotnet sergen` as before.

## 3.10.1 (2020-01-08)

### Bugfixes
  - Resolved compatibility issues with `dotnet sergen`.

## 3.10.0 (2020-01-08)

### Features
  - Added support for .NET Core / ASP.NET Core 3.1 (Visual Studio 2019 is required as .NET Core 3 SDK is not supported in VS 2017 or older versions).
  - Introduced selectable functionality similar to checkbox selection for radio buttons (#4777).

### Bugfixes
  - Ensured server-side validation for required fields also runs for non-string fields.
  - Validated `idField` only if it is actually assigned in `SaveRequestHandler`.