# CHANGELOG 2021

This changelog documents all Serenity versions published in the year 2021 (versions 5.0.20 through 5.1.1).

## 5.1.1 (2021-10-29)

Bugfixes:
  - fix issue with tslib, corelib and rollup causing Serenity.CoreLib.js to fail

## 5.1.0 (2021-10-26)

Features:
  - better handling for custom types like dynamic, list etc. for code generation in endpoints
  - generate c# keywords like string, int, long instead of system type names like String, Int32, Int64 for property types, closes #6026  
  - remove unnecessary space from title for fields that start with underscore, closes #6041
  - add ColumnsType to MasterDetailRelationAttribute that will be used instead of IncludeColumns, and IncludeColumnNames which is a string array instead of comma separated string, closes #6048

Bugfixes:
  - fix summaries not shown under groups at first when grouping changed by drag drop
  - add NotNull attribute to generated row primary keys that are not nullable
  - fix possible issue in rare cases with dapper query overloads that accept dynamic type params
  
## 5.0.51 (2021-09-18)

Features:
  - cleanup code analysis warnings
  - include base class properties while searching for id / name properties in server typings row generation
  - temporary workaround for database caret references, which is a legacy feature
  
Bugfixes:
  - fix extension comparison for meta files in DiskUploadStorage
  - add export to generated permission keys namespaces, thanks @marcobisio
  - swallow exception in TemporaryFileStorage as this may cause startup errors and result in application pool crash while trying to create temporary folder, better to ignore than crash

## 5.0.50 (2021-09-07)

Features:
  - fix dateeditor validation for min max dates using jquery format instead of Q.format

## 5.0.49 (2021-09-02)

Features:
  - embed debug info inside dlls

## 5.0.48 (2021-09-02)

Features:
  - another attempt to make sourcelink debugging work again

## 5.0.47 (2021-09-02)

Features:
  - attempt to make sourcelink debugging work again, hopefully

Bugfixes:
  - handle css bundle url rewrite case when bundle key contains slashes

## 5.0.46 (2021-08-31)

Features:
  - add option to include an ErrorId in service errors
  - make dependency injection for service handlers easier by implementing auto register, and optional generic classes, and get rid of repositories all together, see next commit in common-features repository CustomerEndpoint and sample handler codes
  - update sergen to remove repository class and add request handlers instead, and update generated endpoint classes to use new DI friendly structure
  - get rid of ".Entities" namespace suffix for generated rows, the less namespaces the better
  
Bugfixes:
  - server typings generator should ignore arguments with `[FromServices]` attribute while generating code.

## 5.0.45 (2021-07-20)

Features:
  - sergen generates and uses columnKey on first code generation
  
Bugfixes:
  - fix typos in api docs 
  - fix FieldReadPermission ApplyToLookups option used incorrectly
  - postgres issue with code generation when a schema other than public is used
  - jquery validate options classList add/remove raises errors if the class is an empty string

## 5.0.44 (2021-07-12)

Features:
  - allow getItemMetadata to return css class for slick cells
  
Bugfixes:
  - Fix min value issue in decimal editor

## 5.0.43 (2021-05-29)

Features:
  - updated jspdf to latest version
  - speed up server typings command via caching and improving assembly resolving

## 5.0.42 (2021-05-22)

Features:
  - better error handling for file uploads if server returns an error message or HTTP error

## 5.0.41 (2021-05-10)

Features:
  - ability to disable toastr dialog positioning by not including position-toast in positionClass option
  - better atribute base class for service lookup editor derived client type

## 5.0.40 (2021-05-10)

Features:
  - handle stylesheet issue when running slickgrid tests in jsdom environment
  - implement similar logic to service lookup editor like lookup editor for client type base attribute type determination

## 5.0.39 (2021-04-30)

Features:
  - add extra plugin interfaces for configure services / background jobs
  - handle bootstrap 4 nav tab markup
  - use dispose instead of destroy for bootstrap 4
  - added full namespaced class name for widgets

Bugfixes:
  - SqlQuery union should not emit parens for first query as it breaks in Sqlite
  - fixed operator for max value validation in date editor
  - quick filter date time range handle utc properly

## 5.0.38 (2021-03-25)

Features:
  - rename JsonFilterAttribute back to JsonFilter. Please prefer `[JsonRequest]` attribute as JsonFilter is now obsolete

## 5.0.37 (2021-03-25)

Features:
  - Added tests for connection ById and TryById methods using mocks
  - Added SqlQuery tests
  - Added tests for MasterDetailRelationBehavior
  - 

Bugfixes:
  - Fix TwoLevelCache.Get returns null when local cache does not have an item but distributed cache has
  - Rename JsonFilter to JsonFilterAttribute, made AllowXYZ properties settable as attribute argument

## 5.0.36 (2021-01-26)

Bugfixes:
  - Fix null reference error while setting FileUploadEditor value to null directly
  
## 5.0.35 (2021-01-25)

Features:
  - Added documentation for Serenity.Entity

## 5.0.34 (2021-01-18)

Features:
  - Added IIsSensitiveMessage interface that controls if the exception message can be revealed to the end user in all environments, implement it for ValidationError
  - Added ISaveExceptionBehavior and IDeleteExceptionBehavior which can be used to preview exception and throw different exceptions instead of database ones etc. This replaces ISqlExceptionHumanizer.

Bugfixes:
  - Sergen transform might fail if typeRoots in tsconfig.json is null
  - Fix ui progress bar styling

## 5.0.33 (2021-01-16)

Features:
  - Also scan for annotation types in referenced assemblies during Cecil import generation in Sergen

## 5.0.32 (2021-01-16)

Features:
  - Improve dotnet sergen restore performance
  - Don't copy files under wwwroot\Scripts\serenity for submodules, can now get them through static web assets

## 5.0.31 (2021-01-16)

Features:
  - Improve base type detection in client types generator
  - Implement getFallbackTemplate for Entity/Property dialogs so templates under Views\Templates are no longer necessary

## 5.0.30 (2021-01-15)

Bugfixes:
  - Register nested permission display names in user permissions UI

## 5.0.29 (2021-01-15)

Features:
  - Output sergen.exe directly to bin directory, exclude Debug/Release and TargetFramework. You may need to update reference if you are using Serenity as submodule.
  - Generate XYZColumns.columnsKey just like XYZForm.formKey
  - Use TypingsToPackage item type to determine typings that will be packaged, also use msbuild to determine package references in sergen

## 5.0.28 (2021-01-14)

Bugfixes:
  - Bring back exception logging in service endpoints
  - Resolve issue while restoring typings in sergen

## 5.0.27 (2021-01-13)

Features:
  - Obsolete ICustomizedFormScript, use ICustomizePropertyItems instead
  - Add ImplicitPermissionAttribute to Serenity.Net.Core (implementation only in StartSharp currently)  
  - Add ExportColumns to ListRequest which will be used for Excel export column list etc instead of IncludeColumns which has a different purpose and not guranteed to be persist order as it is a hashset
  - Add IDataReportRenderer abstraction for rendering a data only report to excel format
  - Move IExternalReport interface to Serenity.Services
  - Move DynamicDataReport to Serenity.Services but rename to TabularDataReport as the public interface has changed
  - Add an IExcelExporter interface to abstract exporting data from List services to Excel
  - Add ISqlExceptionHumanizer interface abstraction for producing user friendly exceptions from sql exceptions like FK, PK etc.
  - Start splitting more features into Razor class libraries, like Northwind, Basic Samples etc.
  - Improved Sergen to better work with razor class libraries
  - Add restore option to control file patterns to include/exclude in restore
  - Update sergen restore command to handle version variables, and Directory.Build.props
  - Reuse typings folder which will contain index.d.ts files matching @types npm structure, restore and prefer typings in recursive project references
  - Modify sergen TSTypeLister to get list of files directly from tsconfig.json instead of hardcoded files if possible
  - Also produce typings\serenity.corelib\index.d.ts which is compatible with typeRoots option in tsconfig
  - Use MsBuild.Evaluation library to resolve project references and packed typings
  - Allow FormScript / ColumnScript without specifiying key, but use type FullName in that case (Module attribute won't be used in any case)
  - Handle "." in folder names, allow project.name as a root dir by default for Sergen MVC command
  - Also provide static web assets with Serenity.Scripts (for now optional)

## 5.0.26 (2021-01-09)

Features:
  - Add optional exception logging to image / upload checker
  - Add localizations for image check results
  - Also include typing files under wwwroot/Scripts/serenity while scanning script types
  - Convert include exclude tables for Data Explorer to regex (StartSharp)
  - Add DataProtectionKeysFolder setting (StartSharp) 
  - Improve WkhtmlToPdf location finding, better error message (StartSharp)
  - Add exception logging to file uploads (StartSharp)
  
Bugfixes:
  - Add missing progress_bar.gif
  - Fix mail forward (StartSharp)
  - Fix password editor in mail client (StartSharp)
  - Fix component factory prop derived by inherited classes (StartSharp)
  - Fix data explorer schema provider assembly (StartSharp)
  - Hide note editor from Other Form One Tab sample (StartSharp)
  - Remove where from Northwind Sales By Category view

## 5.0.25 (2021-01-07)

Features:
  - Improve CSS relative URL rewrite handling in CssBundleManager, add tests for rewrite logic

## 5.0.24 (2021-01-06)

Bugfixes:
  - fix error with sergen during new module code generation

## 5.0.22 (2021-01-05)

Bugfixes:
  - HtmlContentEditor.CKEditorBasePath had to end with a slash

## 5.0.21 (2021-01-05)

Bugfixes:
  - TimeEditor left at Serenity.Serenity namespace due to typo

## 5.0.20 (2021-01-01)

Features:
  - Serenity.NET 5 which only supports .NET 5+
  - Embrace dependency injection which makes testing easier, and many integrated features in .NET / ASP.NET Core itself, like caching, options etc.
  - **`[Breaking Change]`** Due to dependency injection usage and obsoleting of Serenity specific Authentication, Dependency etc. classes there are many breaking changes, see Serenity docs and GitHub repo for upgrade notes.
  - Prepared Stargen upgrade tool for StartSharp users
  - Removed "Dependency" class which was a service locator abstraction, and used Dependency Injection (DI) instead (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0)
  - Removed Config class and used Options pattern where possible (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0)
  - There is almost no static classes / state in Serenity framework now
  - Replaced "ILocalCache" interface with .NET Core's "IMemoryCache"
  - Replaced Serenity specific "IDistributedCache" interface and their implementations Serenity.Caching.Redis / Serenity.Caching.Couchbase with .NET Core's "IDistributedCache"
  - Removed "IAuthenticationService" interface and "AuthenticationService" static class, introduced an injectable "IUserAccessor" abstraction
  - Removed Serenity specific "Log" class, and used .NET Core's own logging system
  - Replaced ExtensibilityHelper.SelfAssemblies with a ITypeSource abstraction
  - Replaced static SqlConnections with ISqlConnections abstraction, it is now theorically possible to use dynamic connection strings per request (multi tenancy++)
  - Use DI with lookup scripts, data scripts etc.
  - Introduced IRequestContext for service handlers
  - Row base class is replaced with IRow interface, and there is a generic Row< TFields > base class with access to its Fields type
  - Rows can theorically have different set of custom fields and attributes per request (multi tenancy++)
  - Service behaviors rewritten for DI and they can get constructor injection
  - Script/CSS bundling use options pattern, and bundles can be specified at runtime, also IFileProvider of .NET used so non-physical files can be included in bundles.
  - Default sql dialect can be set per async context
  - Redesigned upload system, opens way to use different storage providers like Azure, S3 etc.
  - Rewrote core script library with modular typescript
