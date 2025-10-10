# CHANGELOG 2021

This changelog documents all Serenity versions published in the year 2021 (versions 5.0.20 through 5.1.1).

## 5.1.1 (2021-10-29)

### Bug Fixes
  - Fixed an issue with tslib, corelib, and rollup causing Serenity.CoreLib.js to fail.

## 5.1.0 (2021-10-26)

### Features
  - Improved handling for custom types like dynamic, list, etc., for code generation in endpoints.
  - Generated C# keywords like string, int, and long instead of system type names like String, Int32, and Int64 for property types. Closes #6026.  
  - Removed unnecessary space from titles for fields that start with an underscore. Closes #6041.
  - Added ColumnsType to MasterDetailRelationAttribute, which will be used instead of IncludeColumns. IncludeColumnNames is now a string array instead of a comma-separated string. Closes #6048.

### Bug Fixes
  - Fixed summaries not shown under groups initially when grouping was changed by drag and drop.
  - Added NotNull attribute to generated row primary keys that are not nullable.
  - Fixed a possible issue in rare cases with Dapper query overloads that accept dynamic type parameters.
  
## 5.0.51 (2021-09-18)

### Features
  - Cleaned up code analysis warnings.
  - Included base class properties while searching for ID/name properties in server typings row generation.
  - Added a temporary workaround for database caret references, which is a legacy feature.
  
### Bug Fixes
  - Fixed extension comparison for meta files in DiskUploadStorage.
  - Added export to generated permission keys namespaces. Thanks @marcobisio.
  - Suppressed exceptions in TemporaryFileStorage to prevent startup errors and application pool crashes while trying to create a temporary folder. It is better to ignore than crash.

## 5.0.50 (2021-09-07)

### Features
  - Fixed DateEditor validation for min/max dates using jQuery format instead of Q.format.

## 5.0.49 (2021-09-02)

### Features
  - Embedded debug info inside DLLs.

## 5.0.48 (2021-09-02)

### Features
  - Another attempt to make sourcelink debugging work again.

## 5.0.47 (2021-09-02)

### Features
  - Attempt to make sourcelink debugging work again, hopefully.

### Bug Fixes
  - Handled CSS bundle URL rewrite case when the bundle key contains slashes.

## 5.0.46 (2021-08-31)

### Features
  - Added an option to include an ErrorId in service errors.
  - Made dependency injection for service handlers easier by implementing auto-register, and optional generic classes, and got rid of repositories altogether. See the next commit in the common-features repository CustomerEndpoint and sample handler codes.
  - Updated sergen to remove the repository class and add request handlers instead, and updated generated endpoint classes to use the new DI-friendly structure.
  - Removed the ".Entities" namespace suffix for generated rows. The fewer namespaces, the better.
  
### Bug Fixes
  - Server typings generator should ignore arguments with the `[FromServices]` attribute while generating code.

## 5.0.45 (2021-07-20)

### Features
  - Sergen generates and uses columnKey on first code generation.
  
### Bug Fixes
  - Fixed typos in API docs.
  - Fixed FieldReadPermission ApplyToLookups option used incorrectly.
  - Fixed PostgreSQL issue with code generation when a schema other than public is used.
  - Fixed jQuery validate options classList add/remove errors if the class is an empty string.

## 5.0.44 (2021-07-12)

### Features
  - Allowed getItemMetadata to return CSS class for slick cells.
  
### Bug Fixes
  - Fixed min value issue in decimal editor.

## 5.0.43 (2021-05-29)

### Features
  - Updated jspdf to the latest version.
  - Speeded up server typings command via caching and improving assembly resolving.

## 5.0.42 (2021-05-22)

### Features
  - Improved error handling for file uploads if the server returns an error message or HTTP error.

## 5.0.41 (2021-05-10)

### Features
  - Ability to disable toastr dialog positioning by not including position-toast in the positionClass option.
  - Better attribute base class for service lookup editor derived client type.

## 5.0.40 (2021-05-10)

### Features
  - Handled stylesheet issue when running slickgrid tests in jsdom environment.
  - Implemented similar logic to service lookup editor like lookup editor for client type base attribute type determination.

## 5.0.39 (2021-04-30)

### Features
  - Added extra plugin interfaces for configure services / background jobs.
  - Handled bootstrap 4 nav tab markup.
  - Used dispose instead of destroy for bootstrap 4.
  - Added full namespaced class name for widgets.

### Bug Fixes
  - SqlQuery union should not emit parens for the first query as it breaks in Sqlite.
  - Fixed operator for max value validation in date editor.
  - Quick filter date time range handle UTC properly.

## 5.0.38 (2021-03-25)

### Features
  - Renamed JsonFilterAttribute back to JsonFilter. Please prefer `[JsonRequest]` attribute as JsonFilter is now obsolete.

## 5.0.37 (2021-03-25)

### Features
  - Added tests for connection ById and TryById methods using mocks.
  - Added SqlQuery tests.
  - Added tests for MasterDetailRelationBehavior.

### Bug Fixes
  - Fixed TwoLevelCache.Get returns null when local cache does not have an item but distributed cache has.
  - Renamed JsonFilter to JsonFilterAttribute, made AllowXYZ properties settable as attribute argument.

## 5.0.36 (2021-01-26)

### Bug Fixes
  - Fixed null reference error while setting FileUploadEditor value to null directly.
  
## 5.0.35 (2021-01-25)

### Features
  - Added documentation for Serenity.Entity.

## 5.0.34 (2021-01-18)

### Features
  - Added IIsSensitiveMessage interface that controls if the exception message can be revealed to the end user in all environments. Implemented it for ValidationError.
  - Added ISaveExceptionBehavior and IDeleteExceptionBehavior which can be used to preview exceptions and throw different exceptions instead of database ones, etc. This replaces ISqlExceptionHumanizer.

### Bug Fixes
  - Sergen transform might fail if typeRoots in tsconfig.json is null.
  - Fixed UI progress bar styling.

## 5.0.33 (2021-01-16)

### Features
  - Also scanned for annotation types in referenced assemblies during Cecil import generation in Sergen.

## 5.0.32 (2021-01-16)

### Features
  - Improved dotnet sergen restore performance.
  - Didn't copy files under wwwroot\Scripts\serenity for submodules. Can now get them through static web assets.

## 5.0.31 (2021-01-16)

### Features
  - Improved base type detection in client types generator.
  - Implemented getFallbackTemplate for Entity/Property dialogs so templates under Views\Templates are no longer necessary.

## 5.0.30 (2021-01-15)

### Bug Fixes
  - Registered nested permission display names in user permissions UI.

## 5.0.29 (2021-01-15)

### Features
  - Output sergen.exe directly to bin directory, exclude Debug/Release and TargetFramework. You may need to update the reference if you are using Serenity as a submodule.
  - Generated XYZColumns.columnsKey just like XYZForm.formKey.
  - Used TypingsToPackage item type to determine typings that will be packaged. Also used msbuild to determine package references in sergen.

## 5.0.28 (2021-01-14)

### Bug Fixes
  - Brought back exception logging in service endpoints.
  - Resolved issue while restoring typings in sergen.

## 5.0.27 (2021-01-13)

### Features
  - Obsoleted ICustomizedFormScript, use ICustomizePropertyItems instead.
  - Added ImplicitPermissionAttribute to Serenity.Net.Core (implementation only in StartSharp currently).  
  - Added ExportColumns to ListRequest which will be used for Excel export column list, etc., instead of IncludeColumns which has a different purpose and is not guaranteed to persist order as it is a hashset.
  - Added IDataReportRenderer abstraction for rendering a data-only report to excel format.
  - Moved IExternalReport interface to Serenity.Services.
  - Moved DynamicDataReport to Serenity.Services but renamed to TabularDataReport as the public interface has changed.
  - Added an IExcelExporter interface to abstract exporting data from List services to Excel.
  - Added ISqlExceptionHumanizer interface abstraction for producing user-friendly exceptions from sql exceptions like FK, PK, etc.
  - Started splitting more features into Razor class libraries, like Northwind, Basic Samples, etc.
  - Improved Sergen to better work with razor class libraries.
  - Added restore option to control file patterns to include/exclude in restore.
  - Updated sergen restore command to handle version variables, and Directory.Build.props.
  - Reused typings folder which will contain index.d.ts files matching @types npm structure, restore, and prefer typings in recursive project references.
  - Modified sergen TSTypeLister to get list of files directly from tsconfig.json instead of hardcoded files if possible.
  - Also produced typings\serenity.corelib\index.d.ts which is compatible with typeRoots option in tsconfig.
  - Used MsBuild.Evaluation library to resolve project references and packed typings.
  - Allowed FormScript / ColumnScript without specifying key, but use type FullName in that case (Module attribute won't be used in any case).
  - Handled "." in folder names, allowed project.name as a root dir by default for Sergen MVC command.
  - Also provided static web assets with Serenity.Scripts (for now optional).

## 5.0.26 (2021-01-09)

### Features
  - Added optional exception logging to image / upload checker.
  - Added localizations for image check results.
  - Also included typing files under wwwroot/Scripts/serenity while scanning script types.
  - Converted include exclude tables for Data Explorer to regex (StartSharp).
  - Added DataProtectionKeysFolder setting (StartSharp). 
  - Improved WkhtmlToPdf location finding, better error message (StartSharp).
  - Added exception logging to file uploads (StartSharp).
  
### Bug Fixes
  - Added missing progress_bar.gif.
  - Fixed mail forward (StartSharp).
  - Fixed password editor in mail client (StartSharp).
  - Fixed component factory prop derived by inherited classes (StartSharp).
  - Fixed data explorer schema provider assembly (StartSharp).
  - Hidden note editor from Other Form One Tab sample (StartSharp).
  - Removed where from Northwind Sales By Category view.

## 5.0.25 (2021-01-07)

### Features
  - Improved CSS relative URL rewrite handling in CssBundleManager, added tests for rewrite logic.

## 5.0.24 (2021-01-06)

### Bug Fixes
  - Fixed error with sergen during new module code generation.

## 5.0.22 (2021-01-05)

### Bug Fixes
  - HtmlContentEditor.CKEditorBasePath had to end with a slash.

## 5.0.21 (2021-01-05)

### Bug Fixes
  - TimeEditor left in Serenity.Serenity namespace due to typo.

## 5.0.20 (2021-01-01)

### Features
  - Serenity.NET 5 which only supports .NET 5+.
  - Embraced dependency injection which makes testing easier, and many integrated features in .NET / ASP.NET Core itself, like caching, options, etc.
  - **`[Breaking Change]`** Due to dependency injection usage and obsoleting of Serenity specific Authentication, Dependency, etc. classes there are many breaking changes. See Serenity docs and GitHub repo for upgrade notes.
  - Prepared Stargen upgrade tool for StartSharp users.
  - Removed "Dependency" class which was a service locator abstraction, and used Dependency Injection (DI) instead (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0).
  - Removed Config class and used Options pattern where possible (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0).
  - There is almost no static classes / state in Serenity framework now.
  - Replaced "ILocalCache" interface with .NET Core's "IMemoryCache".
  - Replaced Serenity specific "IDistributedCache" interface and their implementations Serenity.Caching.Redis / Serenity.Caching.Couchbase with .NET Core's "IDistributedCache".
  - Removed "IAuthenticationService" interface and "AuthenticationService" static class, introduced an injectable "IUserAccessor" abstraction.
  - Removed Serenity specific "Log" class, and used .NET Core's own logging system.
  - Replaced ExtensibilityHelper.SelfAssemblies with a ITypeSource abstraction.
  - Replaced static SqlConnections with ISqlConnections abstraction, it is now theoretically possible to use dynamic connection strings per request (multi-tenancy++).
  - Used DI with lookup scripts, data scripts, etc.
  - Introduced IRequestContext for service handlers.
  - Row base class is replaced with IRow interface, and there is a generic Row< TFields > base class with access to its Fields type.
  - Rows can theoretically have different set of custom fields and attributes per request (multi-tenancy++).
  - Service behaviors rewritten for DI and they can get constructor injection.
  - Script/CSS bundling use options pattern, and bundles can be specified at runtime, also IFileProvider of .NET used so non-physical files can be included in bundles.
  - Default sql dialect can be set per async context.
  - Redesigned upload system, opens way to use different storage providers like Azure, S3, etc.
  - Rewrote core script library with modular typescript.
