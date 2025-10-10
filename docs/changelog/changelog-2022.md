# CHANGELOG 2022

This changelog documents all Serenity versions published in the year 2022 (versions 5.1.2 through 6.4.2).

## 6.4.2 (2022-12-09)

> Release Notes: https://serenity.is/docs/release-notes/6.4.2

Bugfixes:
  - fix script null reference issue with MultipleFileUploadEditors
  - include source maps for some pro nuget packages in static web assets which were missing them (StartSharp)
  - the key sent to IFilenameFormatSanitizer contained "|" chars

## 6.4.1 (2022-12-09)

Features:
  - added AutoValidateAntiforgeryIgnoreBearerAttribute which can be used instead of AutoValidateAntiforgeryTokenAttribute to skip AntiForgery validation when cookie header is not present and Authorization header is Bearer (e.g. JWT etc). use it in place of AutoValidateAntiforgeryTokenAttribute in Startup.cs to try.
  - implemented OpenIddict integration for OpenID / JWT authentication schemes in Serenity.Pro.OpenIddict (StartSharp)
  - added LocalTextDataScript to access local text as a JSON object from mobile apps etc. by specifying package name with "pack" and language ID with "lang" query string parameters via DynJS.axd/RemoteData.LocalText or DynamicData/RemoteData.LocalText
  - introduced IFilenameFormatSanitizer interface which can be used in place of default sanitizing logic in upload filename formatting by implementing it in a custom upload editor attribute by deriving from default ones or
  injecting the implementation via DI.
  - added StringHelper.SanitizeFilePath and made StringHelper.SanitizeFilename handle more invalid filename chars returned from Path.GetInvalidFilenameChars.
  - serenity demo is moved to https://demo.serenity.is, old https://serenity.is/demo urls are redirected so it is not expected to cause a problem
  - StartSharp repository is moved to https://github.com/serenity-premium/startsharp

Bugfixes:
  - AllowNonImage in UploadOptions should be true, as it is mainly used for temporary uploads

## 6.4.0 (2022-12-05)

Features:
  - switch to pnpm for Serenity and feature projects (Serene / StartSharp still use npm)
  - also use state store for script type registry
  - allow mocking Q.debounce function in corelib for testing purposes
  - use jquery event bindings in SleekGrid where possible for better compatibility with legacy plugins
  - make sure interface checks via Q.isAssignableFrom etc. succeed by checking via __typeName even if the same interface is bundled into multiple scripts
  - abstracted SixLabors.ImageSharp dependency via new IImageProcessor interface, that might allow replacing that library with another in the future
  - ThumbnailGenerator class is now obsolete, use it via DI by IImageProcessor interface
  - abstracted upload processor via IUploadProcessor interface
  - UploadProcessor is now obsolete, inject and use IUploadProcessor interface via DI
  - abstracted upload file and image validation with new IUploadValidator interface.
  - introduced IUploadOptions interface and UploadOptions class for keeping default upload options and using in FilePage
  - reorganized upload editor attributes through new abstract class BaseUploadEditorAttribute
  - image validation is now only performed for the extensions specified in ImageExtensions which is ".gif;.jpg;.jpeg;.png;" by default
  - default extension black list can now be modified via ExtensionBlackList property of upload editor attributes
  - added UploadIntent property to upload editor attributes, which is a string that is passed to the upload editors and as a query string back to FilePage during temporary upload. This can be used to customize default thumbnail size, validations etc. in upload method based on intent.
  - **`[Breaking Change]`** IUploadStorage.CopyFrom and IUploadStorage.WriteFile methods accept OverwriteOption enum instead of bool? autoRename flag which was confusing
  - introduced full set of undelete handler related types and behaviors
  - completed XML documentation for Serenity.Services and Serenity.Web
  - switched to typescript modules also in Serene
  - update bootstrap, bootstrap-icons and jquery. 
  - updated sleekgrid to 1.4.4
  - jquery is now at Serenity.Assets/jquery/jquery.js instead of Serenity.Assets/Scripts/jquery-3.5.1.js, you should update your includes in appsettings.bundles.json
  - updated movie tutorial for typescript modules
  - updated serenity docs like introduction, di, configuration, authentication, localization, caching for latest serenity version
  - updated serenity docs api reference to include descriptions for all serenity libs

Bugfixes:
  - fix row selection mixin select all button state not updated for some cases
  - corelib/slick had an embedded corelib/q copy because of esbuild configuration issue
  - initially generated modular ServiceTyping by sergen contained invalid method URLs with Administration/User
  - sleekgrid: auto column hints plugin were not working properly because of jquery event binding difference
  - sleekgrid: fix rtl mode scroll column rendering
  - sleekgrid: old column filter row headers are not deleted before creating new ones

## 6.3.6 (2022-11-15)

Bugfixes:
  - null for ImportClause in TSTypeLister

## 6.3.5 (2022-11-15)

Bugfixes:
  - better detection for formatters type in clienttypes transformation

## 6.3.4 (2022-11-15)

Features:
  - add option to disable module re-exports (or indexes) for servertypings transform in sergen.json under ServerTypings/ModuleReExports

## 6.3.3 (2022-11-14)

Features:
  - change TOptions generic argument for Widget to default to any

## 6.3.2 (2022-11-14)

Bugfixes:
  - fix group totals is not displayed

## 6.3.1 (2022-11-08)

Bugfixes:
  - possible null reference exception in sergen when combination types used as interfaces

## 6.3.0 (2022-11-06)

> Release Notes: https://serenity.is/docs/release-notes/6.3.0

Features:
  - ported all common features projects to ES modules, including Serenity.Extensions, Serenity.Demo.BasicSamples, Serenity.Demo.Northwind
  - ported all pro features projects to ES modules, including Serenity.Pro.Extensions, Serenity.Demo.AdvancedSamples, Serenity.Pro.Meeting etc. `(StartSharp)`
  - moved CSHTML views for feature packages to `/Areas/ProjectName` and code files to `/Modules` instead of `/ProjectName/`
  - added simplified IFileSystem base interface which removes dependency to System.IO.Abstractions, but it can still be used in tests
  - upload behavior refinements, remove hard coded dependency to editor attributes and use interfaces instead
  - allow upload attributes other than ImageUploadEditor / MultipleImageUploadEditor to be also handled by upload behaviors by implementing some interfaces
  - renamed MultipleImageUploadEditor to MultipleFileUploadEditor, ImageUploadBehavior to FileUploadBehavior
  - testable disk upload storage via IDiskUploadFileSystem abstraction. it can also be used to create a custom disk upload storage.
  - ported several tests including local texts, globalfilter written in .NET framework for Serenity.Core
  - use LookupEditorBaseAttribute / ServiceLookupEditorBaseAttribute for editors defined in modules as well
  - replace const enums with normal enums to comply with isolatedModules option
  - export Formatter interface from @serenity-is/corelib in additional to @serenity-is/corelib/slick
  - enable tsbuild clear plugin by default only when splitting is true. it is still possible to enable it by specifying clean: true
  - delete only .js / .js.map files when clean plugin is enabled for tsbuild
  - don't use const enum for es modules service method exports, but use const object instead
  - better handling for enum types in es modules 
  - better handling for Razor SDK projects that use ProjectName dir instead of Modules folder
  - improve imports for external module types in code / source generators
  - moved ui overrides to serenity corelib from pro.extensions
  - create new buttons-inner divs when a toolbar has separator, instead of creating a separator div
  - specify full names for more classes including QuickSearchInput, TemplatedDialog etc.
  - Areas/ProjectName is also scanned by MVC generator for views / strip view parts
  - set buttontext as icon font only for < jquery ui 1.12.1 as 1.13x does not allow html in button text
  - reuse editor type finding for external libs as long as their namespace match with project name. this improves type discovery for modules, as typescript does not preserve decorators in .d.ts files. make sure your namespaces match the project name, e.g. an editor with key 'MyLib.MySome.MyEditor' should be exported from a 'MyLib.MySome' project (mylib.mysome npm package) with 'MyEditor' classname.
  - extends support for tsconfig.json in source / code generator
  - better determination of module name in TSModuleResolver for node_modules packages
  - also restore dist/index.js for project references to node_modules in addition to dist/index.ts
  - auto fake import enums in form.ts / columns.ts if possible, to avoid errors when such types are in a different module
  - add ResolveWithHash to HtmlScriptExtensions so it can be used to avoid caching issues while importing module page scripts under esm/..
  - instead of removing a property from the form.ts when the editor type can't be discovered, assume it as a widget so it can be understood something is wrong and "as any" etc can be used to reference the form field in such cases
  - adapt email client css for BS5 theme `(StartSharp)`

Bugfixes:
  - output directory for Razor SDK projects should use ProjectName dir instead of Modules
  - fix RowSelectionModel plugin export
  - fix executeOnceWhenVisible alias in corelib typings
  - fix GlobFilter regex based matching for `Modules/**/*` style patterns
  - fix TSFileLister does not work exactly like tsconfig for patterns, as TypeScript considers all patterns to start at root unlike gitignore patterns
  - fix deepClone does not work properly with Date and several other types of objects (used https://github.com/angus-c/just)
  - also check for "None" in addition to "none" or other cases for module / namespace detection
  - fix email client script error `(StartSharp)`

## 6.2.9 (2022-10-22)

Features:
  - add tests for es module editor option generation
  - don't generate row type if it has ScriptSkip attribute, even if it is referenced by something else. 
  - don't generate row / other type members if it has scriptskip or ignore attribute
  - use export for nested permission key namespaces

## 6.2.8 (2022-10-22)

Bugfixes:
  - fix options for editors defined in es modules not generated in client types

## 6.2.7 (2022-10-22)

Features:
  - try to import formatter types used by columns to Columns.ts so that modular formatters can be registered from the grids that they are used from
  - add option to disable particular transform in source generator with sergen.json MVC/SourceGenerator and ClientTypes/SourceGenerator settings by using false.
  - go back to non-incremental source generator for clienttypes as it breaks syntax highlighting when referencing .ts files as AdditionalTexts, and there is no single output .Web.js for modules `(StartSharp)`
  - disable source generator when SourceGeneratorTransform is false in project file `(StartSharp)`
  - read MVC/StripViewPaths from sergen.json in ViewPathsSourceGenerator if available `(StartSharp)`

## 6.2.6 (2022-10-17)

Features:
  - published @serenity-is/tsbuild npm package which will be used from tsbuild.js
  - added NodeScriptRunner for running tsbuild in watch mode (imported from .NET SpaServices)
  - added startnodescripts settings in appsettings.Development to npm run tsbuild:watch in development at startup (StartSharp)
  - try to import dialog types used by inplace add editors to Form.ts so that modular dialogs can be registered from the dialogs that these editors are used from
  - include full name of class in registerClass calls for code generated by sergen 
  - removed maximizable dialog from sergen scriban templates as it is not a part of core and not used anymore
  - send an empty source map with 202 status, instead of 403, to avoid seeing warnings when SourceMapSecurityMiddleware is enabled
  - handling for root namespace in client types source generator (StartSharp)
  - made add button functional in dashboard task list (StartSharp)
  - reverted back to Microsoft.Data.SqlClient to 3.1.1 from 4.1.0 in sergen, as Encrypt=true became the default in 4.x+ (https://techcommunity.microsoft.com/t5/sql-server-blog/released-general-availability-of-microsoft-data-sqlclient-4-0/ba-p/2983346) which is a **`[Breaking Change]`** causing connections to fail. As Serenity.Data used 2.1.0 of Microsoft.Data.SqlClient, only sergen was affected by this issue, not StartSharp or Serene apps. Please see the linked doc, and either install a trusted certificate in your SQL server, or set TrustServerCertificate=true or Encrypt=false in your connection string, as we'll be updating Microsoft.Data.SqlClient to newer versions in the future, that may bring back this error.

Bugfixes:
  - fix invalid relative module paths generated in source generator
  - fix cache is reset for previously reset groups after update, even if they are not reset in current transaction

## 6.2.5 (2022-10-15)

Features:
  - publish tsbuild as npm package (@serenity-is/tsbuild)
  
Bugfixes:
  - don't sort desc column first, if multiple SortOrder attributes are defined (#6559)

## 6.2.4 (2022-10-13)

  - ignore DataGrid.defaultRowHeight and DataGrid.defaultHeaderHeight in getSlickOptions() as they cause sleekgrid to fail rendering in Serene etc. where they are not set.

## 6.2.3 (2022-10-13)

Features:
  - move restoretypings target to Serenity.Net.Web.targets, this allows restoring typings\lib\index.d.ts files without having to run dotnet sergen restore

## 6.2.2 (2022-10-13)

Bugfixes:
  - fix referenced packages node_modules index.d.ts restoring for modular coding in Serenity.Pro.Coder.targets `(StartSharp)`
  - recreate package-lock.json on template build `(StartSharp)` so node_modules populated correctly. delete your package-lock.json and node_modules than run npm i if you have such an issue.

## 6.2.1 (2022-10-11)

Bugfixes:
  - handle case where logger factory is null in DefaultSqlConnections causing sergen generate command to fail

## 6.2.0 (2022-10-10)

Features:
  - implement logging support for SqlHelper via WrappedConnection and DefaultSqlConnections
  - improve logging for SqlHelper, include ms and command hash code
  - added brotli support to dynamic scripts and script bundles
  - converted serenity json texts to static web assets. AddAllTexts() is obsolete, use services.AddBaseTexts(env.WebRootFileProvider).AddJsonTexts(env.WebRootFileProvider, "Scripts/site/texts").AddJsonTexts(env.ContentRootFileProvider, "App_Data/texts")
  - convert module texts to static web assets in common and pro packages
  - converted all StartSharp modules to modular TypeScript `(StartSharp)`
  - renamed ScriptInitialization.ts to ScriptInit.ts
  - removed namespace typings `(StartSharp)`
  - removed StartSharp.Web.js `(StartSharp)`
  - enable logging for sql only in development, added a appsettings.Development.json `(StartSharp)`
  - pass keepNames option as true to esbuild so widgets can keep their css class names like s-RolePermissionDialog etc
  - no need for PreserveCompliationContext as it is set by Razor sdk
  - moved SourceMapSecurityMiddlewareExtensions to usual namespace (Serenity.Extensions.DependencyInjection)
  
Bugfixes:
  - fix interfaces and enums are declared multiple times in Q / Slick / Serenity namespaces in Serenity.CoreLib.d.ts
  - update sleekgrid to fix script error with selection models
  - fix clean plugin for tsbuild
  

## 6.1.9 (2022-10-02)

Features:
  - enabled source generator (Serenity.Pro.Coder) in StartSharp template
  - **`[Breaking Change]`** Q.Config.responsiveDialogs is now assumed to be true by default (this should not affect anyone unless using a very old version like 2.x etc)
  - move idPrefix up to widget, introduce renderContents method which can be overridden to manually to initialize widget content via another method instead of an html template
  - remove unused jsRender method

## 6.1.8 (2022-09-29)

Bugfixes:
  - fix source mapping for corelib.js
  - **`[Breaking Change]`** please override getPropertyItemsData instead of getPropertyItems (bug since 6.1.0) when there is not a form key / columns key

## 6.1.7 (2022-09-29)

Bugfixes:
  - don't include package.json / tsconfig.json files inside Serenity.Scripts nuget package as content files 

## 6.1.6 (2022-09-29)

Bugfixes:
  - resolve issue with msbuild locator causing sergen restore to fail

## 6.1.5 (2022-09-21)

Bugfixes:
  - fix sergen servertypings not removing suffixes like .Entities

## 6.1.4 (2022-09-20)
  - fix slickgrid headers trimmed in smaller screen when in a sub dialog

## 6.1.3 (2022-09-05)

Bugfixes:
  - wrap grid creation in jquery ready for modular page in sergen
  - fix possible null ref exception when Cecil returns null for typeDef.Resolve()

## 6.1.2 (2022-09-05)

Bugfixes:
  - fix output dir for razor sdk projects modular servertypes
  - fix for modular import from self file

## 6.1.1 (2022-09-04)

Bugfixes:
  - must set nameIsHtml in GridRowSelectionMixin.createSelectColumn

## 6.1.0 (2022-09-04)

Features:
  - rewrote slickgrid (sleekgrid) in typescript (https://github.com/serenity-is/sleekgrid)
  - formatters can add classes, attributes and tooltip to the target cell directly via ctx properties
  - allow setting end of line character to lf/crlf manually in sergen.json via "EndOfLine" setting.
  - published @serenity-is/corelib as an npm package (modular version with embedded source)
  - useAsync option for datagrid, entitygrid, propertydialog and entitydialog
  - ability to retrieve data as json from dynamic scripts e.g. lookups etc via /DynamicData instead of /DynJS.axd
  - code generator will generate code with only usings that are actually used implemented via helper methods in Scriban templates
  - **`[Breaking Change]`** Columns and Form dynamic scripts returns an object (PropertyItemsData) instead of array. It should not be a breaking change if you receive those scripts via Q.getColumns, Q.getForm etc. methods (legacy), but make sure you update both Serenity.Scripts and Serenity.Web to 6.1.0 at least.
  - Q.ScriptData methods won't raise a client side error if a script that does not seem to be registered (registeredscripts) is tried to be loaded, it will try, then fail if server does not return it
  - preparation for es6 modular typescript code generation / transform / source generators (StartSharp)
  - new .NET Source Generators for clienttypes, servertypings and mvc commands in addition to a row fields source generator via RowTemplate class (StartSharp, Serenity.Pro.Coder nuget package)
  - **`[Breaking Change]`** global Promise interface augmentation is removed, please update your target to ES6 in tsconfig.json if you use Promise.resolve etc. anywhere

## 6.0.8 (2022-07-12)

Features:
  - add option to set background color when using padwithfill

## 6.0.7 (2022-07-11)

Bugfixes:
  - fix utf 8 bom not written to generated files in last two versions

## 6.0.6 (2022-07-11)

Features:
  - remove app specific texts from Serenity

## 6.0.5 (2022-07-11)

Features:
  - add key for EmailEditor (please prefer EmailAddressEditor)

## 6.0.4 (2022-07-10)

Features:
  - remove System.IO.Abstractions dependency from Serenity.CodeGenerator as preparation for source generators
  - add key constant to all editor, foramtter, filtering attributes
  - use pnpm if installed
  - use libman instead of npm for typings where possible

Bugfixes:
  - fix cast error occuring in TSTypeLister in rare cases
  - fix generated texts not accessible
  - slick grid getCellNode should return an element not jQuery object
  

## 6.0.3 (2022-06-24)

Features:
  - remove ss type related code from import generator
  - allow and handle IEnumerable, IList, ISet, IDictionary types in response types in addition to List, Dictionary etc. in response types for server typings generator
  - don't generate proxy method for service actions implementing IActionResult
  - add handleRoute method to entity grid allowing to override default route handling behavior
  - better handling for multi part routes when restoring location hash in case the first part causes a service call

Bugfixes:
  - raise an error in FormatFileName when OriginalName passed is null
  - pass original name to generated .jpg file when temporary file is scaled and a .jpg file is generated on upload

## 6.0.2 (2022-06-16)

Features:
  - add autoRename = null option to upload storage which means overwrite **`[Breaking Change]`**
  - also apply flag to the existing generated field for notmapped field types
  - added JoinNonEmpty methods to StringHelper (like StringHelper.Join but for arrays similar to string.Join)
  - add ability to directly set a value without checking expiration status in TwoLevelCache
  - use C# 10 global usings feature in Serenity source

Bugfixes:
  - fix syntax error with tabindex property in slickgrid.js
  - fix project refs check in sergen
  - load validation messages after page is ready (after local texts are ready). (#6389)

## 6.0.1 (2022-04-21)

Features:
  - rewrote typescript parser in c# using a modified / optimized version of https://github.com/ToCSharp/TypeScriptAST
  - increased performance of sergen client types command using new typescript parser
  - added ServiceLookupPermissionAttribute which will simplify permission checks for service lookups
  - sergen generates AuthorizeList attributes on top of the List and ListExcel methods in endpoints (#6364)
  - sergen generates TextualField field type as lowercase string keyword (#6341)
  - made AutoParam consistent after cloning SqlQuery (#6330)
  - added ISqlDialectMapper interface
  
Bugfixes:
  - upload editor progress fix
  - setting Old property to the null to prevent a few issues while re-using a save request handler. (#6356)
  - null check added where to sergen connections checked. (#6360)

## 6.0.0 (2022-03-26)

Features:
  - moving to .NET 6 and ASP.NET Core as .NET 5 will soon be out of support
  - will only support Visual Studio 2022 with latest updates as Visual Studio 2019 does not support .NET 6. please don't update to Serenity 6 if you still have to use VS2019.
  - **`[Breaking Change]`** replaced System.Drawing.Common with ImageSharp, as System.Drawing.Common is only officially supported in Windows now.
  - updated Dapper, FluentMigrator and many other packages to their latest versions

## 5.2.4 (2022-03-09)
 
Features:
  - a better console ui for sergen with multiple selection support
  - added on demand service resolver interface - IServiceResolve
  - use System.Text.Json for sergen transform
  - allow passing project refs optionally to sergen
  - don't register keys with logical operators in NestedPermissionKeyRegistration

Bugfixes:
  - resolve more errors with corelib when jquery not loaded, $ is not jquery, or document not initialized. fix bootstrap 5+ check (bootstrap 4 was also returning 5+)  
  - SqlUpdate.SetTo xml comment correction (#6225)
  

## 5.2.3 (2022-01-20)

Bugfixes:
  - bootstrap modal dialog stays under jQuery UI dialogs (StartSharp)
  - error in console while closing a bootstrap modal when a jquery ui dialog is open
  - inline action font style should be normal
  - remove schema names from worklog tables

## 5.2.2 (2022-01-18)

Features:
  - add bootstrap v5.1.3 RTL versions to Serenity.Assets
  - don't generate unused less file for apps created from recent templates
  - removed unused less commands from project file
  - add support for generating a separate .rtl bundle if any bundle parts contains {.rtl} placeholder
  - add RTL support to new pro theme (StartSharp)
  - added rtl rules for dashboard, pro-theme and pro-extensions
  - used rtlcss for x.rtl.css on build of pro packages
  - update npm version check in project creation wizard (npm 8+ / node 16+)
  - project wizard removes empty typings folders when features are deselected

Bugfixes:
  - fix padding / alignment of multiple select
  - fix classes and html dir generation in Layout.cshtml (StartSharp)
  - fix worklog module class name typo in WorkLogGrid

## 5.2.1 (2022-01-16)

Bugfixes:
  - Serenity.Scripts contains package.json etc as content files due to a change in NET6 SDK possibly

## 5.2.0 (2022-01-15)

  - bootstrap 5 support (5.1.3)
  - pro theme with simplified (1/3 size) bootstrap 5 compatible css (StartSharp)
  - new dark theme cosmos (StartSharp)
  - no longer use select2, slickgrid etc. css files (all in pro theme, StartSharp)
  - completely removed AdminLTE
  - redesigned dashboard with new charts, chat panel etc.
  - use chartjs instead of morris
  - use css variables for easy color scheme / theme development (StartSharp)
  - no longer use less for css
  - classic theme layout option via s-classic-layout on html element (header instead of a sidebar band) (StartSharp)
  - use nprogress instead of pace (StartSharp)
  - custom enum flags editor and sample (StartSharp)
  - use bs5 collapsible for sidebar (StartSharp)
  - separate templates for VS 2022 / VS 2019. required due to VS 2022 extension incompatibility / amd64 (StartSharp)
  - bring back module selection dialog on project creation (StartSharp)
  - abstract navigation model
  - use line awesome instead with shims instead of font awesome 4 (StartSharp)
  - added sample work log module (StartSharp)
  - fix all pro-theme issues (https://github.com/serenity-is/Serenity/milestone/1?closed=1)
  

## 5.1.3 (2022-01-15)

Bugfixes:
  - fix missing static web assets issue in Serenity.Scripts due to target framework bug

## 5.1.2 (2022-01-14)

Features:
  - this is a preparation release for new Bootstrap 5 based pro theme in StartSharp. Some changes might potentially break Bootstrap 3 apps. Revert to 5.1.1 if you have any issues.
  - added CustomData dictionary properties to ServiceRequest / ServiceResponse, avoid using them and define custom subclasses except in rare cases where that's not possible
  - don't set CSRF-TOKEN header for crossDomain requests as it is causing a CORS pre-flight validation
  - allow join attributes to be used based on dialects
  - add option to negate dialect match, throw an error if there are multiple matches for the same dialect
  - add ability for dialects to optionally change SqlQuery.ToString() by implementing ISqlQueryToString interface
  - made OuterApplyAttribute, LeftJoinAttribute, InnerJoinAttribute, and ExpressionAttribute accept ServerType to set their Dialect, added NegateDialect property
  - allow tablename to be changed based on dialect as well
  - allow foreign keys to be selected based on dialect
  - use NavigationGroup / NavigationSection attributes to reorganize existing navigation items to groups / sections (StartSharp pro theme)
  - add bs- prefix where needed for data attributes for bs5 compability
  - allow option to pass a multiplier and delta for column widths which is useful when switching to a larger font, as widths set server side may not match expected size  
  - add bootstrap 5.1.3, line awesome with font awesome shims, tabler icons, and preact 10.6.4
  - add nprogress (to be used instead of pace)
  - change dropdown type timeouts down to 200 ms from 500 ms for a more responsive ui, don't show search icon before actually calling the service
  - remove sourcemappingurls from bundled css as it is not supported at the moment in nuglify etc.
  - add latest open sans and poppins fonts
  - removing source maps for bootstrap as sass code is not useful while inspecting
  - removed legacy serenity less files, and use output serenity.css instead (now serenity.css is for legacy BS3 apps only)
  - removed less dependency
  - added INavigationModel, and INavigationModelFactory interfaces
  - removed todo from UniqueFieldSaveBehavior and UniqueConstraintSaveBehavior (#6176)
  - added Oracle12cDialect which prefers offset fetch instead of rownum
  - get rid of "Update" button and use "Save" both for "Create" and "Update" modes
  - move to latest stable version of node / npm (16.13.2 / 8.1.2)
  - better npm install implementation for Serenity.Scripts / Serenity.Assets
  
Bugfixes:
  - check for form content type before trying to read from form in JsonRequestAttribute
  - SaveRequestHandler should not modify the request entity but clone it as it can be modified in validate request phase, for example trimming etc.
  - clone may leave assignedFields in invalid state
  - fix bs5 modal showing
  - fix bs5 close button markup
  - fix group separator naming in ScriptCulture.cs
  - fix restore command in linux
  - grouping panel should not scroll with the grid columns, it has no relation to horizontal scroll of the grid
  - fix pane top when there is frozen columns and grouping panel is visible (#6213)
