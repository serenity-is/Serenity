# CHANGELOG 2017

This changelog documents all Serenity versions published in the year 2017 (versions 2.8.0 through 3.3.14).

## 3.3.14 (2017-12-27)

Features:
  - prevent reloading grid while loading persisted quick search
  - validate permission on ReportRegistry.GetReport method to secure execution
  - message dialog style changes for modern look
  - made inplace add functionality compatible with multiple lookup editor. when multiple editing items works by double clicking on labels.
  - give a hint about field name when cast exception occurs on a field asobject method (thanks @dfaruque)
  - serins npm package (serin for startsharp)
  
Bugfixes:
  - took care of unnecessary loops on distinct lookup registration
  - should put quotes around guid values in sql debug text
  - fix search for previously loaded CKEditor script
  - preprocessor directive to determine site.less folder in new sergen was wrong, so site.module.less was created in wrong directory
  - RadioButtonEditor missing options in constructor in Editors.ts
  - renamed getEntityIsActiveField (older name) in d.ts to getIsActiveProperty (current name), thanks @	tky753
  - set property item as localizable only if Localizable attribute value is true
  - fix sqlite connection string database location invalid due to a change in Microsoft.Data.Sqlite
  - fix hint and title options are mixed in ExcelExportHelper.ts `(Serene)`
  - show pdf in new window not working on some browsers `(Serene)`
  - fix exception while adding implicit permissions `(Serene)`
  
## 3.3.13 (2017-11-23)

Features:
  - added DateOnly option to DateTimeField, which means field has date value only (it only affects json serialization / deserialization not get/set value), DateTimeKind.Unspecified means a DateTime field with Time portion that shouldn't apply any timezone conversions (this might be breaking change if you relied on DateTimeKind.Unspecified means date only), DateTimeKind.Local means server should apply it to its local time zone (as before), DateTimeKind.Utc means store and retrieve as utc (as before). Added tests, property grid changes, and a useUtc option to DateTimeEditor that should be true when DateTimeKind.Local or DateTimeKind.Utc is used.
  - create log file with FileShare.Read flag in FileLogger


## 3.3.12 (2017-11-21)

Features:
  - introduce FormWidthAttribute which works similar to FormCssClass attribute before, but targeted only at bootstrap grid classes. FormCssClassAttribute is still usable but intended for other cases, like offset, and custom field css classes
  - ability to add break point, e.g. wrap to next row before this field for bootstrap grid fields, using one of line-break-xs, line-break-sm, line-break-md, line-break-lg attributes
  - added Q.attrEncode which should be used instead of Q.htmlEncode for attribute values, if you are using Q.htmlEncode in any formatter to generate attribute value, please replace it with attrEncode as quotes can be a problem
  
Bugfixes:
  - fix utc datetime deserialization issue due to DateTimeOffset
  - added missing date time picker sprites image

## 3.3.11 (2017-11-15)

Features:
  - added getdisplaytextfor and getcriteriafor to filter store
  - autocomplete lookup editor should add item on init selection
  - make quick filter loadState, saveState, displayText methods available in TypeScript
  - improve fallback template logic

## 3.3.10 (2017-11-11)

Features:
  - added wizard dialog widget and related order wizard sample `(StartSharp)`
  - added UntilNext option to LabelWidthAttribute and FormCssClassAttribute (e.g. HalfWidth etc) so that it applies to all following properties until another one of same kind is used
  - port BooleanEditor, MaskedEditor, StringEditor, TextAreaEditor, TimeEditor classes to TypeScript from Saltaralle
  - ability to save quick filter display text
  - ability to save quick search text and field
  - accept unicode letters as valid sql identifiers
  - use Pascalize instead of Capitalize while generating module name from connection key
  - added some new attribute derived from FormCssClassAttribute for bootstrap grid system form (thanks @estrusco)
  - ability to use a fallback template if can't find expected default one in templated widget

Bugfixes:
  - fix slick grid column header has border on hover inside dialog
  - replace ".Entities." with "." in ScriptDtoGenerator (for saltaralle)
  - fix RadioButtonEditor not able to clear checks on setting value
  
## 3.3.9 (2017-11-04)

Bugfixes:
  - fix constructor of DistinctValuesEditorAttribute

## 3.3.8 (2017-11-04)

Features:
  - added DistinctValuesScript, DistinctValuesEditorAttribute and related logic to automatically define distinct lookup scripts on fields
  - extract UploadedFile into public class to be able to use in other methods
  - move up getDialogTitle method from PropertyDialog to TemplatedDialog
  - cleanup serenity.jqueryui.less duplicates
  
Bugfixes:
  - fix service typing generation for legacy result<t> type endpoints
  - fix included deleted toggle background position
  - report dialog broken after panel feature
  - remove selectmenu and bring back shake effect as quicksearch is still using it

## 3.3.7 (2017-11-01)

Features:
  - give more descriptive error, and show error notification in addition to exception when dialog type is not found
  - also show a notification when enum type is not found as users generally don't check console for errors
  - more descriptive error and notification when widget can't be found on an element
  - add missing methods like tryFirst, single etc. in Q.cs

Bugfixes:
  - CKEditor fail when initialized /destroyed in an invisible element / dialog / panel.
  
## 3.3.6 (2017-10-30)

Features:
  - use font awesome icons for dialog titlebar buttons, merge aristo.css into serenity.css, change titlebar styling
  - cleanup dialogExtendQ
  - dont swallow exceptions in ScriptBundleManager

## 3.3.5 (2017-10-25)

Features:
  - introduce explicity included bundles , e.g. ones with "/" in their names and has to be included explicity, using @Html.ScriptBundle or @Html.StyleBundle, which allows using different sets of bundles for different parts of sites.
  - set customer in new order dialog to selected customer in quick filter of order grid `(Serene)`
  
Bugfixes:
  - fix RouteDialog call in initEntityDialog method should use itemType passed in parameters

## 3.3.4 (2017-10-14)

Features:
  - easy to configure cdn support (experimental, in use at demo.serenity.is)
  - allow cdns to cache dynamic scripts for anonymous access
  - check bundled script rights also when checkRights() on concatenated script is called
  - increase scriban template loop limits as some users has tables with many columns (300+)
  - added LocalTextScript method to HtmlScriptExtensions that works well with CDNs
  - fix typo in image upload validation error and show file size in KBs/MBs
  - localize select all button in check tree editor
 
Bugfixes:
  - custom template path option is not used in .net desktop serene

## 3.3.3 (2017-10-23)

Features:
  - dont auto focus first element on mobile device, as it shows keyboard
  - prevent jquery ui dialog from focusing an input in mobile devices
  - removing embedded touch punch from cropzoom.js as it was causing dialog back button in android browsers to not work. better to exclude cropzoom.js from layouthead.cshtml completely in your project unless you use it specially.
  - resolve possible memory leak in jquery ui remove event / cleanData method due to HtmlElementCollection enumeration
  - remove layout events when full height grid div is removed
  - removed fastclick.js as it is no longer useful in mobile devices `(Serene)`
  - don't spin logo in mobile devices `(Serene)`
  - hide some quick filters in order / customer screens for xs devices using hidden-xs bootstrap class `(Serene)`
  - specify TypeScriptToolsVersion in web.csproj as VS2017 asks for it on template project creation anyway `(Serene)`

Bugfixes:
  - fix HostingEnvironment.MapPath in ASP.NET Core / IIS when app is hosted under a virtual subdirectory, fixes script bundling
  - add back spacing line between foreign row fields in generated code
  - call select2 destroy on base select2 editor, not lookupeditorbase
  - check view is not null before calling populate in initialPopulate
  - destroy mouse trap instance on toolbar destroy to prevent memory leaks

## 3.3.2 (2017-10-22)

Bugfixes:

  - select2 sprites broken in high res devices

## 3.3.1 (2017-10-21)

Features:

  - release new assets package (3.3.1)

## 3.3.0 (2017-10-21)

Features:
  - we now have an embedded css minifier / bundler similar to script bundler. CSS bundler can override URLs in bundled CSS files to absolute paths. it is now possible to include other dynamic scripts in script bundles using dynamic://ScriptName syntax, e.g. dynamic://Lookup.Northwind.Product.
  - use new css bundling and dynamic script bundling features to improve page load times dramatically and decrease number of loaded resources like scripts, css etc when bundling is enabled `(Serene)`
  - login now has a slim layout page, embedded css and javascript so that Serene.Web.js and site.css, and some other scripts that are not required in login page are not loaded `(Serene)`
  - use font awesome instead of simple line icons in navigation to remove dependency on simple line icons `(Serene)`
  - allow overriding separator in ConcatenatedScript
  - allow returning css from dynjs handler / middleware, separate legacy BundleCssHandler to its own file
  - use font awesome icons in column picker instead of simple line icons
  - slimmer version of jquery ui is default now. it doesn't contain extra effects, tooltips, spinner, accordion, jQuery 1.7 support
  - font open sans data uri version that only contains Normal version, which will be used by default in Serene
  - use browser Promise if available, fallback to jQuery.Deferred or RSVP. RSVP is no longer required.
  - convert most used toolbar buttons, pager buttons, back buttons, dialog close button, quick search icon, select2, date picker and filter panel images to data uris image to data uris
  - improve submodule.targets file to auto include serenity projects and remove DLLs if projects exists
  
Bugfixes:
  - fix dynamic script middleware gzip support check
   
## 3.2.2 (2017-10-18)

Features:
  - added openDialogsAsPanel option (which is null by default, e.g. auto) to data grids that allows overriding panel / dialog opening mode per grid.
  - added openDialogAsPanel option to lookup editor (null = auto by default)

Bugfixes:
  - bring back default options populated by sergen on first run (.net framework version)

## 3.2.1 (2017-10-18)

Features:
  - restore hash in login page returnURL (e.g. returnUrl=Northwind/Customer/Edit#12) `(Serene)`

Bugfixes:
  - panel is not closed when browser back button pressed
  - build error in Serene due to TypeScript.MsBuild package on build servers

## 3.2.0 (2017-10-17)

Features:
  - using dialogs as full page panel support, which also works fine with grids, applied it in Northwind Customer and Order dialogs
  - better responsive handling in x-small devices
  - **`[Breaking Change]`** panels are hidden by default, so you need to .removeClass('hidden') after appending panel element to a container. see EntityDialogAsPanel sample Index.cshtml

## 3.1.1 (2017-10-16)

Bugfixes:
  - fix null reference exception on t4 transform
  - Skip Take With Order By clause used correct syntax for OracleDialect
## 3.1.0 (2017-10-16)

Features:
  - support CSRF (cross site request forgery) protection in Serenity pages / services. it is highly recommended to apply changes in latest Serene commit to your existing project to enable CSRF protection!
  - .net core sergen no longer has dependency to serenity.web, thus asp.net core, so it is now much slimmer and 10 times faster to load
  - improved .net core build / dotnet sergen transform time significantly (20x) if input files (.ts) didn't change by employing caching in TSTypeLister.
  - t4 transform files in .net framework version is also much faster (caching + change control), see latest commit
  - t4 transform doesn't modify files if only line endings changed, solves a problem with git due to line ending transformation (files was looking like changed after T4 transform)
  - servertypings t4 transform doesn't invoke tsc if none of files changed
  - typescript compiler is only invoked when an input file changes which improves build time significantly, see changes in Serene.csproj
  - using Microsoft.TypeScript.MsBuild package instead of Node based TSC in Serene AspNetCore (usually 2x faster)
  - less compiler is only invoked when an input file changes which improves build time significantly, see changes in Serene.csproj
  - use scriban template engine for sergen
  - it is now possible to override templates sergen uses by setting "CustomTemplates" option to a directory with .scriban templates. if there is a template with same name (e.g. Row.scriban) in that directory sergen uses that, otherwise uses default embedded one.
  - sergen provides option to generate custom files by setting CustomGenerate option in configuration file. this is a dictionary of template file => output file format ({0} class name, {1} Module, {2} class path, {3} typings path)
  - it is possible to show / editor custom settings (boolean/string) by adding them to CustomSettings dictionary in sergen configuration file. these settings are available as {{CustomSettings.SettingKey}} in scriban templates.
  - added "mvct" command to sergen that does mvc / clienttypes at once. used this option for pre build script instead of two calls to dotnet-sergen.
  - removed rowwithregion, lookup generate options etc as they are now possible with custom generate option and custom settings
  - sergen no longer generates code for Saltaralle. script project option is removed. use an older sergen version if you still generate code for saltaralle.
  - put each field on separate line for easier merge in ServerTypings generated files (for easier merge in git)
  - support quick filter persist / restore  (thanks @marcobisio, pull request #1455)
  - add FullTextContains quick search type (simple version of pull request #2200 that only support quick search with contains for performance reasons, thanks @marcobisio)


## 3.0.8 (2017-10-11)

Features:
  - made it possible to use bootstrap col-md-6, col-sm-4 etc. to layout form fields. 
  - added attribute FormCssClassAttribute for applying custom sizes.
  - use HalfWidthAttribute, OneThirdWidthAttribute, QuarterWidth etc. for quick field sizing 
  - added ImplicitPermissionAttribute and related auto permission granting system which is a much better alternative to LogicOperatorPermissionService. e.g. when a user / role has Northwind:Modify permission, it should also automatically have Northwind:View and Northwind:General permissions
  - in addition to resizing canvas, also invalidate grid on resize, fixes some display issues on windows 7 pcs
  - added missing class declaration for IStringValue to .d.ts
  - more descriptive error message for "Can't find {0} enum type!"
  - more descriptive error message for "{0} field on {1} is read before assigned a value!"

## 3.0.7 (2017-10-07)

Features:
  - add Tab attribute to forms that works similar to Category attribute but works with bootstrap tabs. can be combined with categories.

## 3.0.6 (2017-10-02)

Features:
  - added autoComplete option to LookupEditor, which allows creating new items by user typed text
  - sampled autoComplete with Northwind Customer's country / city editing `(Serene)`

## 3.0.5 (2017-09-06)

Features:
  - allow overriding dialect for SqlInsert and SqlUpdate like SqlQuery does

Bugfixes:
  - fix culture identifiers in .net core version UserCultureProvider and don't crash if a culture is not found in the system `(Serene)`
  - fix upload problem in linux due to case sensitivity
  - set readonly flag of ckeditor instance after instance is ready

## 3.0.4 (2017-09-05)

Bugfixes:
  - try to resolve problem with dotnet sergen restore in linux due to case sensitivity and nuget lowercasing package ids

## 3.0.3 (2017-09-04)

Features:
  - dotnet sergen servertypings can try to find output DLL itself so assemblies option in sergen.json can be removed for normal workflows
  - dotnet sergen servertypings uses release output dll if its date is newer than debug output dll

## 3.0.2 (2017-09-04)

Features:
  - rowregistry can support multiple rows per ConnectionKey / TableName pair

## 3.0.1 (2017-09-04)

Bugfixes:
  - fix dotnet sergen failing with assembly load error

## 3.0.0 (2017-09-03)

Features:
  - upgraded to .NET Core 2.0 / AspNetCore 2.0
  - ability to use full .net framework with Serene AspNetCore version (wait for guide)
  - typescript 2.4 is now recommended version
  - make sure dotnet-sergen can load assembly and list types properly before trying to generate server typings
  - added tablename as attribute on Row for RowWithRegion (thanks @dfaruque)
  - raise forbidresult from PageAuthorizeAttribute when user doesn't have permission but logged in
  - make display order and IDisplayOrderRow work with rows of non-integer ID columns
  - handle service exceptions in asp.net core service endpoints similar to mvc ones
  
Bugfixes:
  - fix unused select overload (thanks @TomaszKustra)  

## 2.10.3 (2017-08-17)

Features:
  - experimental functionality to use a Serenity editor as a Vue component using special <editor /> element
  - date/datetime editor year range option default to -100:+50 instead of -10:+10 (better for birth date etc.)
  - router use history.back() only when dialog is closed by x button or escape key

## 2.10.2 (2017-08-12)

Features:
  - option to use original file name as uploaded file name with a {4} format parameter in FileNameFormat. it auto adds file (1).docx etc if there is already a file with same name on disk

Bugfixes:
  - resolve problem if router can't use history.back(), e.g. history is empty somehow

## 2.10.1 (2017-08-10)

Features:
  - added option to ignore enum members (thanks @Estrusco)

Bugfixes:
  - resolve problem with Firefox / hash based router while editing items in detail editors. You may add Q.Router.enabled = false in ScriptInitialization.ts if still having issues or wan't to disable this "feature".
  - findElementWithRelativeId missing # (thanks @Estrusco)

## 2.10.0 (2017-07-23)

Features:
  - updated all packages, including Newtonsoft.Json, Nuglify, AspNetCore, StackExchange.Redis

## 2.9.32 (2017-07-23)

Features:
  - better handling of return url when forms auth login url is absolute
  - try to restore previous hash if non-routed dialog opened

Bugfixes:
  - fix issues with dotnet-sergen and non-matching assembly versions

## 2.9.31 (2017-07-18)

Bugfixes:
  - router didn't attach to hashchange at start

## 2.9.30 (2017-07-18)

Features:
  - use shorter hash fragments for not properly routed dialogs
  - added option to disable router (add "Q.Router.enabled = false" in ScriptInitialization.ts)
  - when time dropdown changes trigger a change event for date input in DateTimeEditor

Bugfixes:
  - set file size correctly when uploaded file is not an image
  - skip assemblies that doesn't like to list types

## 2.9.29 (2017-07-17)

Bugfixes:
  - resolve routing problem when a dialog is closed and another opened right away
  
## 2.9.28 (2017-07-16)

Features:
  - basic hash based router for handling back button in mobile / desktop (primarily for modal dialogs). it can also handle forward button / bookmarking if implemented properly by routing targets.

## 2.9.27 (2017-07-13)

Features:
  - pass item class from attribute in navigation items

## 2.9.26 (2017-07-13)

Features:
  - ability to add css class to navigation items
  - wrapped headers in grid sample `(Serene)`

## 2.9.25 (2017-07-06)

Features:
  - made back button in mobile mode a bit bigger to make it easier to click in small touch devices, closes #1800
  - updated spanish translations (thanks @gustavo)
  - triggerDataChange overload that accepts a jQuery object is renamed to triggerDataChanged
  - exit gracefully when assembly DLL file is not found for sergen transform
Bugfixes:
  - inplace add lowercases typed text in new item dialog
  - jquery ui button noconflict .d.ts typing change warning
  - resolve possible null reference exception in GlobFilter
  
## 2.9.24 (2017-05-14)

Bugfixes:
  - possible filtering problem with DateTimeOffset deserialization in JSON.NET and date/time fields

## 2.9.23 (2017-05-09)

Features:
  - added NestedPermissionKeys attribute that works kinda similar to NestedLocalTexts attribute for permission key registration

## 2.9.22 (2017-05-07)

Features:
  - allow specifying an external lookup type on rows in LookupScriptAttribute constructor, or a generic type through LookupType property, simplifies multi tenancy
  - added .gitignore compatible high performance GlobFilter class
  - introduce mail settings and pickup directory functionality in .net core version
  
Bugfixes:
  - code generator for .net core should look for sergen.json instead of project.json
  - when an appsetting key is not available, null reference exception occurs
  - template helper can't find views in .net core version
  - fix typo in signup (thanks @Febriantos) `(Serene)`

## 2.9.21 (2017-03-26)

Features:
  - run tsc directly, not through npm, to avoid weird error log with npm
  - ScriptInitialization.ts reference to LanguageList
  - toastr 2.1.33 has getContainer method, so removing one in Serenity, update toastr typings version in your packages.json to ^2.1.33
  - don't show "undefined" when a XHR connection error occurs. try to be more descriptive but it will be limited as there is no possibility to get exact error.
  - return to avoid kestrel header errors in .net core when browser requests .map files which doesn't exist from DynamicScriptMiddleware

## 2.9.20 (2017-03-19)

Features:
  - allow setting CKEditor readonly option after widget initialization

Bugfixes:
  - script bundling settings are not read in .net core version
  - don't crash on t4 transform when a class has no namespace

## 2.9.19 (2017-03-16)

Bugfixes:
  - fix .net core version file name casing issues in linux

## 2.9.18 (2017-03-16)

Bugfixes:
  - backport prefix determination algorithm to sergen net45 version

## 2.9.17 (2017-03-13)

Features:
  - Visual Studio 2017 support (.NET Core version no longer works on Visual Studio 2015 sorry because of project.json to csproj change. you need to use VS Code or upgrade to VS2017)
  - obsoleted jsrender (will later replace with Vue)
  - added language cookie to cookies sent to pdf renderer `(Serene)`

## 2.9.9 (2017-03-02)

Features:
  - use TypeScript compiler from npm. Visual Studio still uses its own extension for intellisense and compile on save, so make sure your version matches.

Bugfixes:
  - try fix nuget sergen path issue by moving back it to tools folder from tools/net45

## 2.9.8 (2017-02-17)

Bugfixes:
  - fix issue with stored proc (e.g. sqlite pragma foreign_key_list) returning empty resultset with 0 fields, dapper is raising multi map error. backporting fix from Dapper itself.

## 2.9.7 (2017-02-17)

Bugfixes:
  - resolve bug caused by TypeScript __extends helper copying all static members including __metadata which shouldn't be as Saltaralle type system depends on it being a unique array. this is revealed by panel decorator getting applied to base class as well.

## 2.9.6 (2017-02-15)

Features:
  - obsoleted linq.js (e.g. Saltaralle.Linq). if you still have saltaralle code that depends on linq.js, add it to your layouthead.cshtml manually, e.g. @Html.Script("~/Scripts/Saltaralle/linq.js")

## 2.9.5 (2017-02-14)

Features:
  - sergen .net core / node version tolerates whitespace in paths and other arguments
  - russian texts update (thanks @Сергей Соболев)

Bugfixes:
  - handle issue with servertypings.tt and datetimeoffset/bytearray field types

## 2.9.4 (2017-01-04)

Features:
  - allow spaces in dotnet-sergen arguments (requires updating npm sergen too, e.g. npm update -g sergen)

## 2.9.3 (2017-01-31)

Features:
  - allow using .ts.html suffix in addition to .Template.html to let Visual Studio group a template under its related component, e.g. SomeDialog.ts => SomeDialog.ts.html
  - add module template prefix to template key automatically (might be a breaking change if you relied on exact file name)

Bugfixes:
  - fix group declaration on group import for slickgrid.d.ts

## 2.9.2 (2017-01-31)

Features:
  - backport schema providers in sergen .net core version to .net framework version
  - use Serene.Web for .NET core project template, instead of longer Serene.AspNetCore name
  - fill in (CascadeField, CascadeValue) and (FilterField, FilterValue) in InitNewEntity method of LookupEditorBase, e.g. InplaceAdd
  - sergen generates code for mvc, clienttypes before build, and servertypings post build in asp.net core version (project.json)

## 2.9.1 (2017-01-29)

Features:
  - new `[Origin]` attribute to auto set Expression, DisplayName, Size and Scale attributes from the originating row for view fields.
  - allow specifying a `[ForeignKey]` by using a row type that has a `[TableName]` attribute. ID field can also be automatically determined if row has a property with `[Identity]` attribute, or a single property with `[PrimaryKey]` attribute. Implementing IIdField doesn't help ID field detection.
  - for script bundling, replace MsieJsEngine + UglifyJS with Nuglify which is based on MS Ajax Minifier, faster and effective resource wise.
  - put semicolon between ConcatenatedScript parts to avoid javascript errors with certain minified files
  - allowClear param in enum editor (thanks @Estrusco)

## 2.9.0 (2017-01-25)

Features:
  - Sergen for .NET Core now works with Firebird, MySql, Postgres, Sqlite in addition to SqlServer. 
  - New schema provider system in Sergen to query database metadata
  - Made FirebirdDialect quoting compatible with with FluentMigrator one
  
Bugfixes:
  - use this.uniqueName while binding to layout, to resolve script errors after widget is destroyed, and use it to unbind on destroy
  - fix report dialog not showing parameters (thanks @Scott)
  
## 2.8.11 (2017-01-21)

Features:
  - Serin uses latest VSIX template from VSGallery instead of embedding a template in itself `(Serene)`
  - Serin replaces connection strings to use Sqlite in OSX / Linux `(Serene)`
  - Serene ASP.NET Core version now works with Sqlite (no code generation support yet) `(Serene)`
  - Serene ASP.NET Core comes with Sqlite factory preconfigured
  - Serene uses MsSqlLocalDB instance in Windows by default `(Serene)`
  - increased speed of Sqlite migrations for Northwind dramatically

Bugfixes:
  - resolved problem with .NET Core and Sqlite with reader.GetBytes() as Microsoft.Data.Sqlite doesn't support it
  - fixed Serenity.FluentMigrator and Sqlite compability problem about case sensitivity
  
## 2.8.10 (2017-01-20)

Bugfixes:
  - resolve delete error on localization behavior

## 2.8.9 (2017-01-20)

  - simplified data localization with a behavior
  - localizations are integrated into saverequest and retrieveresponse for easier implementation
  - add language / culture switching support to ASP.NET Core version `(Serene)`

## 2.8.8 (2017-01-18)

Features:
  - resolve regression bug with script bundling

## 2.8.7 (2017-01-17)

Features:
  - develop npm sergen package to workaround dotnet cli tool bug about readline/readkey

## 2.8.6 (2017-01-15)

Bugfixes:
  - implement console readline hint without using console width or setcursorposition
  
## 2.8.5 (2017-01-15)

Bugfixes:
  - remove test check in sergen cli
  
## 2.8.4 (2017-01-15)

Bugfixes:
  - can't use auto completion in sergen thanks to cli bug

## 2.8.3 (2017-01-14)

Bugfixes:
  - resolve problem with dotnet-sergen self assemblies

## 2.8.2 (2017-01-14)

Features:
  - lazy load connection string provider factory

Bugfixes:
  - resolve problem with dotnet-sergen can't load FluentMigrator assembly

## 2.8.1 (2017-01-13)

Features:
  - Serene now runs on ASP.NET Core / .NET Core (Beta)
  - allow specifying expressions for a field based on connection dialect type, for example `[Expression("CONCAT(A, B)"), Expression("A || B", Dialect = "Sqlite")]`, match with longest dialect name wins
  - `[DisplayName]` and `[InstanceName]` used Title instead of Tablename in generated row.cs (thanks @dfaruque)

## 2.8.0 (2017-01-10)

Features:

  - Serenity NuGet packages now contains netstandard1.6 targets (e.g. .NET Core support)
  - new command line code generator (dotnet-sergen) for .NET cli
  - criteria.exists overload that accepts a sqlquery
  - ability to add a quickfilter separator line on columns or manually
  - option to add a css class to a quick filter, e.g. something like visible-lg to make it visible only on large devices
  - boolean quick filter takes display texts from filtering options
  - moving /modules/common/imports to /imports and /modules/common/migrations to /migrations `(Serene)` by default
  - switched to NPM for TypeScript typings (.d.ts) files, see upgrade guide in GitHub

Bugfixes:
  - don't use prefix if a field name is equal to prefix length in sergen