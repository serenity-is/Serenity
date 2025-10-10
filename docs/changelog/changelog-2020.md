# CHANGELOG 2020

This changelog documents all Serenity versions published in the year 2020 (versions 3.10.0 through 3.14.5).

## 3.14.5 (2020-11-24)

Features:
  - support ICustomQueryParameter interface of Dapper, used to control parameter type like varchar (ansi), fixed length etc.

Bugfixes:
  - try another logic similar to original one implemented for changeSelect2

## 3.14.4 (2020-11-23)

Bugfixes:
  - fix changeSelect2 is raised even when value is set through .value property due to check using hasClass instead of data

## 3.14.3 (2020-10-21)

Bugfixes:
  - prefix `__rownum__` alias with x as Oracle does not seem to like it

## 3.14.2 (2020-10-19)

Features:
  - option to disable layout timer in datagrid

Bugfixes:
  - row number over order by does not gurantee order and might mess paging in some rare cases on SQL 2005/2008
  - missing localization for "set to now" in date time editor
  - datetime editor time part is not readonly when editor itself is readonly
  - keyprefix not used properly for get in RedisDistributedCache
  - fix infinite resizing on some cases in datagrid due to layout timer
  - reading pathLink.href directly results in full address like localhost:6543/ instead of just /, so use getAttribute to fix Q.Config.AppPath

## 3.14.1 (2020-09-09)

Bugfixes:
  - fix setting a date editor date to today in UTC minus time zone resulting in one day before
  - make sure quick search text and field is persisted after user change if quickSearch persistance flag is true

## 3.14.0 (2020-08-06)

Features:
  - ability to enable local sorting on remote view, might be useful for in memory grid editors, override getViewOptions to enable and set localSort to true
  - bring back flexify decorator for backward compatibility (please don't use it)
  
Bugfixes:
  - datepicker icon not displayed in Serene due to url encoding problem
  - set datepicker img width default to 16px by css as it is now a svg
  - checked null in GetName() and GetText() Enum extention methods (#4252)
  - restore Select2Item interface under Serenity namespace
  - fix issue with validation error highlighting of select2 editors
  - don't return last set value in date time picker, if the value was now or today. should resolve issues with validation on create.


## 3.13.7 (2020-07-21)

Bugfixes:
  - also include Serenity.CoreLib.d.ts under Scripts/typings/serenity/ as older projects still use that one in tsconfig.json (if you have it there, change path to Scripts/serenity/Serenity.CoreLib.d.ts

## 3.13.6 (2020-07-20)

Bugfixes:
  - include recaptcha and maskededitor in corelib again

## 3.13.5 (2020-07-09)

Features:
  - added FieldInsertPermissionAttribute, FieldUpdatePermissionAttribute, FieldModifyPermissionAttribute which works similar to FieldReadPermissionAttribute to set relevant permission on all fields, which can be overridden field level on particular fields
  - add ability to clear local text registry through a new IRemoveAll interface. required for runtime translation reloading in .NET Core as it is not possible to register a new ILocalTextRegistry unlike .NET framework

## 3.13.4 (2020-06-12)

Features:
  - added option to use bootstrap modal for dialogs instead of jQuery UI.
  - Q.alert, Q.confirm etc works without jQuery UI and it can use Bootstrap if available. it fallbacks to browser native dialogs if both is not loaded, though options will only be limited to the message.
  - Q.blockUI can work without blockUI plugin, but just for full screen blocking
  - serenity corelib itself can be loaded without loading external scripts like jQuery, jQuery UI, toastr etc. but functionality will be limited. useful for testing and frontend pages.
  - removed dependency on saltaralle mscorlib.js. no longer have to include it, unless you have some code that uses "ss." namespace in your project. most of them can be converted to "Q." alternatives. search for (ss as any) or (ss.) in your code before removing.
  - introduced splitted parts of Serenity.CoreLib.js so that only required libs can be loaded when needed. useful for frontend apps. the list of libs: serenity-core.js (contains Q and core types / functions), serenity-widget.js (contains base widget type, ui related typings, toolbar etc. depends on core), serenity-forms.js (contains validation, templated panel, property grid, property panel, depends on core and widget), serenity-editors.js (contains most editor types, depends on core and widget), serenity-slick.js (contains remoteview and other slickgrid related types, depends on core), serenity-dialogs.js (contains templated dialog, property dialog and entity dialog, depends on core, widget and forms), serenity-quickfilter.js (contains quick search and quick filter bar, depends on core, widget and editors), serenity-filterpanel.js (contains filter panel and dialog, depends on core, widget, editors, quickfilter, forms and dialogs), serenity-grids.js (contains data and entity grid, depends on core, slick, widget, forms, editors, dialogs, quickfilter and filterpanel).
  - optional flatpickr support for date/time editors. enabled with DateEditor.useFlatpickr = true or used automatically when flatpickr is loaded in page and jQuery UI datepicker itself not.
  - a new layout watcher that can call layout for elements when their width / height / or visibility change by using a timer. used to resolve slickgrid layout issues.
  - replace COREFX ifdefs with NET45 as .NET CORE and ASP.NET CORE is now the default. this will open way to use other IDEs like Visual Studio Code for .NET CORE projects. OmniSharp extension VSCODE does not work well with #IFs and project references.
  
Bugfixes:
  - make sure DateTimeField does not fail on read if the data reader returns a DateTimeOffset
  - if date/time kind is local, should convert the value to universal time before formatting, as .net format function does not do this conversion automatically
  
## 3.12.6 (2020-04-12)

Bugfixes:
  - fix css bundle manager removes quotes in url for data uris while adjusting relative paths in css bundle

## 3.12.5 (2020-04-12)

Bugfixes:
  - fix css bundle manager ignoring usemincss setting if the included file already has .min.css extension
  - jquery.validate has changed required logic to not trim input value by default, so whitespace was considered valid for string inputs
  
## 3.12.4 (2020-03-09)

Bugfixes:
  - shouldn't validate not null fields with default values on insert, if they are not assigned explicitly as the default value will be applied automatically. resolves create service issues with notnull fields with defaultvalue attributes, where these fields are not visible in form.

## 3.12.3 (2020-02-22)

Features:
  - if null value handling is set to include, row converter should also serialize null fields
  - define JsonSettings.StrictIncludeNulls and JsonSettings.TolerantIncludeNulls which serializes null values, and checks them on deserialization. JSON class methods also has an additional includeNulls (default = false) to be able to use these settings.
  - added NavigationPermissionAttribute which takes precedence over ReadPermissionAttribute on a row to determine permission for page and navigation item of the row.
  - add AsSqlValue method which calls AsObject by default to Field object. this will be useful for fields which has a different storage type then their value type, e.g. JsonField
  - added JsonField<TValue> type. it has no attributes that controls serialization options like nulls so such options must be set through Field.Settings property if required for now.
  - don't show an error if xhr status is abort
  - added async source support to Select2Editor
  - added async option to use getLookupAsync to lookup editor
  - added ServiceLookupEditor type
  - don't use type delay for initial load, hide select2 spinner while waiting for type delay
  - add async option to lookupeditor which makes it work like a service lookup editor
  - added FieldReadPermissionAttribute which determines the default permission required to read fields of a row, optionally excluding `[LookupInclude]` and Id/Name fields. This will make it easier to use a ServiceLookupEditor with a row, for example by setting `[ReadPermission("Northwind:Lookups")]`, `[FieldReadPermission("Northwind:General", ApplyToLookups = false)]`, `[NavigationPermission("Northwind:General")]`, so only users with Northwind.General can see the page and read all the fields through the list service / grid, while a user with Northwind.Lookups permission can call List service but can only read ID + Name + LookupInclude fields through it.
  - adding missing field HeaderHtmlUrl to IHtmlToPdfOptions
  - include pdb file for serenity.web for source link debug support
  - ensure connection is open for better compatibility with mvc version in dapper interface
  
Bugfixes:
  - fix Q.getLookupAsync not actually working async
  - avoid double initSelection by setting select2-change-triggered
  - fix layout timer registration cleanup
  - title text should be read from child with .title-text class
  - avoid exception while determining url when a controller has multiple action methods with same name
  
## 3.12.2 (2020-01-11)

Bugfixes:
  - DataGrid.updateInterface might fail if grid has no toolbar
  - make sure quick filters bar can still be created within a fake toolbar div, even when grid has no toolbar

## 3.12.1 (2020-01-11)

Bugfixes:
  - revert updates to System.Threading.Tasks.Extensions and System.Runtime.CompilerServices.Unsafe in Sergen (NET45)

## 3.12.0 (2020-01-08)

Bugfixes:
  - fix embedded resource issue with new sergen tool

## 3.11.0 (2020-01-08)

Features:
  - introduce sergen as a global/local dotnet tool as DotNetCliToolReference can't be used with .NET Core 3+ (remove DotNetCliToolReference from CSPROJ, run "dotnet new tool-manifest" in CSPROJ dir, followed by "dotnet tool install sergen", then "dotnet tool restore" and use "dotnet sergen" as before)


## 3.10.1 (2020-01-08)

Bugfixes:
  - compatibility issue with dotnet sergen


## 3.10.0 (2020-01-08)

Features:
  - support for .NET Core / ASP.NET Core 3.1 (you'll need Visual Studio 2019 as .NET Core 3 SDK is not supported in VS 2017 or older versions.)
  - selectable ability like checkbox selection for radio (#4777)

Bugfixes:
  - make sure required field validation server side also runs for non-string fields
  - only validate idField if it is actually assigned in saverequesthandler