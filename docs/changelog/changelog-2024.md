# CHANGELOG 2024

This changelog documents all Serenity versions published in the year 2024 (versions 8.2.1 through 8.8.3).

## 8.8.3 (2024-12-15)

### Bugfixes
- Resolve issue with static web assets and content files in nuget packages

## 8.8.2 (2024-12-15)

### Features

- New `services.AddApplicationPartsFeatureToggles` extension that should be used in Startup instead of `services.AddFeatureToggles`. This also scans enum types with [FeatureKeySet] attribute in application parts, and disables features that have a [DefaultValue(false)] attribute on them by default. It also discovers dependencies between features themselves (for example a DataAuditLog_Services sub feature might depend on a DataAuditLog feature, e.g. disabling it also disables all sub features) so it is recommended to switch to AddApplicationPartsFeatureToggles.
- Add ability to declare dependencies between feature flags themselves by adding RequiresFeature attribute to the FeatureKeys enum members, or the enum itself. For it to work, AddApplicationPartsFeatureToggles should be used, or the dependencyMap argument of AddFeatureToggles must be passed manually.
- Add a new argument (disabledByDefault) to AddFeatureToggles method to pass a list of feature toggle keys (enums or strings) to disable by default without having to set in configuration. The default will be used if the flag is not specified in the configuration, e.g. it will not override the configuration setting. It is also possible to pass ["*"] to disable all features by default, but is not recommended as it would mean when we add a new feature flag in the future (not necessarily a new feature, just a new toggle to disable some part of a feature), all users would have to enable it manually in configuration.
- Take distinct over assemblies for classic type source (DefaultTypeSource) if the passed enumerable of assemblies is an array.
- Automatically add common namespaces like System.Collections.Generic, Serenity, Serenity.ComponentModel etc. to usings from Serenity.Net.Web.targets (e.g. any project referencing Serenity.Web) unless SerenityUsings property is explicitly set to false. This means most `Using` items from StartSharp/Serene projects might be removed.
- Add a helper function (GetDataConnectionString) to get data connections string from IConfiguration in Startup where IConnectionStrings is not yet available. Use it only for initialization of exception log.
- Remove react patching function that checks React is loaded after corelib, as we no longer use global react object
- Only create databases under App_Data for SqlLocalDB (e.g. `(localdb)`), not for local SQL server etc.
- Use MySqlConnector instead of MySql.Data which should be faster
- Add code generation support for Oracle to Sergen
- Create Northwind tables via fluent syntax instead of manually written SQL scripts. There is no separate autoincrement ID column in Customers table now (e.g. only CustomerID which is a string), and space in "Order Details" is removed, so there are some breaking changes. Northwind sample database should be recreated.

### Bugfixes

- Remove invalid RequiresFeature attribute from Pro.Extensions assembly.
- Side effect import Region dialog from TerritoryGrid to resolve issue with clicking secondary edit link on Region

## 8.8.1 (2024-12-01)

### Bugfixes

- Fix casing for `Corelib` during build (should not be `CoreLib`) which happened during switch to cake frosting, affecting case sensitive file systems

## 8.8.0 (2024-11-30)

### Features

- Added a new [File Explorer sample](https://demo.serenity.is/AppSamples/FileExplorer) (Pro.AdvancedSamples).
- Introduced a new [FullCalendar integration sample](https://demo.serenity.is/AppSamples/Calendar) (Pro.AdvancedSamples).
- Implemented a feature toggle system (`IFeatureToggles`) with `RequiresFeatureAttribute` and `FeatureBarrierAttribute` to disable features at runtime. Features can be toggled via the `FeatureToggles` configuration section, which:
  - Returns 404 for disabled pages.
  - Removes navigation links, permissions, and UI elements related to those features.
  - Initially applied to `DataAuditLog` and `EmailQueue` features, now merged into the `Pro.Extensions` package. These individual features can now be disabled using feature flags.
  - **Setup note**: Register the feature flags service in `Startup.cs` and ensure the type source references it. See the updated `Startup.cs` for guidance.
  
- **Breaking Changes**:
  - **Package Mergers**:
    - `Serenity.Net.Data` and `Serenity.Net.Entity` packages are now merged into `Serenity.Net.Services`. Remove references to these packages and update `TypeSource` implementations accordingly. Switching to `ApplicationPartsTypeSource` is recommended.
    - `Serenity.Pro.DataAuditLog` and `Serenity.Pro.EmailQueue` are merged into the `Serenity.Pro.Extensions` package. Remove their assembly references if using a classic type source.
    - `Serenity.Pro.Theme` package contents are merged into `Serenity.Pro.Extensions`. Update references in `appsettings.bundles.json`:
      - Replace `~/Serenity.Pro.Theme/pro-theme.js` with `~/Serenity.Pro.Extensions/pro-theme.js`.
      - Replace `~/Serenity.Pro.Theme/pages/dashboard.css` with `~/Serenity.Pro.Extensions/pages/dashboard.css`.
      - Replace `~/Serenity.Pro.Theme/pro-theme{.rtl}.css` with `~/Serenity.Pro.Extensions/pro-theme{.rtl}.css`.
      - Remove `~/Serenity.Pro.Extensions/pro-extensions{.rtl}.css`, as it is merged into `pro-theme.css`.
    - `Serenity.Pro.Organization` is merged into `Serenity.Pro.Meeting`. Remove its references from your project and `TypeSource`.
    
  - **Excel Report Generator**:
    - Transitioned from EPPlus to ClosedXML due to licensing issues. The change primarily affects `ExcelReportGenerator` users, with updates to helper function arguments and return types (e.g., `XLWorkbook` instead of `ExcelPackage`). The only impacted screen was the `ProductExcelImport` sample. Other Excel export and report functionalities remain unaffected. If EPPlus is still required, manually add its reference and retain a copy of the old `ExcelReportGenerator`.
    
  - **Localization Updates**:
    - `ILocalizationRow.CultureIdField` type is changed to `StringField` (culture codes like `en`, `en-GB`, etc., instead of integers). Update your `XYZLangRow` definitions and database types accordingly. A migration script (`NorthwindDB_20241120_1203_LanguageIdToCode.cs`) is provided for guidance. Language-related UI and database mapping improvements are included to streamline localization processes.
    
- Added `FieldLocalizationListBehavior` to support localization during list requests. Currently, only `ServiceLookupEditor` can display localized dropdowns.
- Enhanced localization UI by adding a dropdown to select the target language, displaying inputs for the selected language only. The selection persists in local storage.
- Introduced `TranslationConfig` class with `getLanguageList` and `translateTexts` configuration options, enabling machine translation features in forms. Update your `ScriptInit.ts` or `languages-init.ts` to use this new configuration.
- Introduced a `Localize` property in `ListRequest` to support localization during list requests.
- Added `IListMapFieldExpressionBehavior` for custom field expression mappings in list requests.
- Enhanced AI-powered translation for entity localization.
- Updated `ILanguageFallbacks.GetLanguageFallbacks` arguments and return type for improved fallback calculations.
- Refactored entity dialog tool button properties into reusable functions (`saveAndCloseToolButton`, `applyChangesToolButton`, etc.).
- Enhanced `onSaveSuccess` functionality to display success messages and identify the action initiator.
- Updated `StringField.ValueFromJson` to consider the ISO date converter from Newtonsoft.Json.
- Added a `ref` option to `ToolButtonProps` for improved button handling in `EntityDialog`.
- Merged `MultipleFileUploadBehavior` into `FileUploadBehavior` to unify handling of both single and multiple file uploads.
- Removed legacy request/response types: `SaveRequestWithAttachment`, `SaveWithLocalizationRequest`, `RetrieveLocalizationRequest`, and `RetrieveLocalizationResponse`.
- Updated translations for all 23 supported languages.
- Dependency updates:
  - Microsoft.TypeScript.MSBuild to 5.7.1
  - Microsoft.Data.Sqlite to 9.0.0
  - Scriban to 5.12.0
  - Microsoft.Extensions.* packages to 9.0.0
  - System.Text.Json to 9.0.0
  - NUglify to 1.21.10
  - Selenium.WebDriver to 4.27.0
  - PuppeteerSharp to 20.0.5

### BugFixes

- Fix check for NPM version in sergen doctor
- Seems like browsers changed the logic for beforeunload event, we need to call preventDefault to get a confirmation when navigating away from the page.
- Fix date time range quick filter end date pick does not trigger change event due to extra dot in ".change"
- Fix issue with TwoFactorAuthenticator preprocessor directives

## 8.7.9 (2024-11-12)

### Bugfixes

- Fix records cannot be added to the grid editor for in-memory mode

## 8.7.8 (2024-11-12)

### Bugfixes

- Fix issue when document.documentElement.lang is empty string

## 8.7.7 (2024-11-12)

### Features

- Use texts.__.json as a file name for template texts, instead of texts.json, and change the comment to clarify intent.
- Prefix field name with @ if the generated private field member name is a C# reserved keyword, e.g. for properties like `return`, `int` etc.
- Add IsReservedKeyword method to `ISqlDialect`, and implement it for dialects. There is also an `SqlSyntax.IsReservedKeywordForAny` method that returns if an identifier is reserved in any of the known dialects. 
- Add AutoQuotedIdentifiers to the ISqlDialect interface which is null by default. If null, its value is determined from SqlSettings.DefaultDialect.AutoQuotedIdentifiers or SqlSettings.AutoQuotedIdentifiers. This will make changing this setting per connection / dialect possible by subclassing one of the dialects. Regardless of this setting, keywords will always be quoted by `SqlSyntax.AutoBracket` and `SqlSyntax.AutoBracketValid` methods. If this is not desired for some reason, dialect might return false for its IsReservedKeyword method.
- Set `SqlSettings.AutoQuotedIdentifiers` to `true` by default. This had been set to true in both the templates for years and running Serenity tests in the same context should be better for consistency. The setting was originally introduced as a workaround for Postgres, and the way FluentMigrator quotes identifiers in migrations by default, so probably it may even be no longer necessary.
- Modify EditorUtils.setReadonly so that it supports both widgets and elements. It also auto searches for an attached widget if an element is passed in. EditorUtils.setReadOnly and EditorUtils.setReadonly now point to the same function. There is also corelib's getElementReadOnly and setElementReadOnly functions but they only accept elements and don't search for widgets. All widgets deriving from EditorWidget now also has a readOnly property that is directly settable instead of using EditorUtils.
- Sort OrganizationEditor items by their display texts

### Bugfixes

- Fix TimeSpanEditorAttribute having wrong key so it is generated as TimeEditor

## 8.7.6 (2024-11-04)

### Features

- Add checks for common issues encountered while developing an in-memory grid editor. Throw an error from GridEditorBase.getDialogType() method if it is not overridden. Check if the dialog type returned from getDialogType() is a subclass of GridEditorDialog, not EntityDialog.  Ensure that id property name returned from getIdProperty() methods of the GridEditorDialog and GridEditorBase are same.
- Add ability to use connected mode (e.g. calling services) for in-memory grid editors by setting connectedMode to true. This may be used with some grid editors so that if the parent dialog is in edit mode (e.g. the master id is available) the grid editor directly loads and updates the records directly via the service (without waiting for the main dialog to save). For this to work, the grid editor and its dialog must both return the actual ID property from their getIdProperty method or use getRowDefinition. Also, the grid editor should have a masterId property that in its setter should set itself to connectedMode if the masterId is assigned. See changes in Northwind OrderRow, Order Dialog / OrderDetailsEditor and its OrderDetailDialog for a sample.
- Add getCreateServiceMethod, getUpdateServiceMethod, getDeleteServiceMethod, getRetrieveServiceMethod functions to entity dialog to override individual services if required.
- Add protected getServiceMethod and getServiceUrl methods to entitygrid to customize List service url without having to override getViewOptions.
- Split Serene/StartSharp Texts class into small targeted classes
- Only export generic `Texts` constant from `Texts.ts` if there is a class with name "Texts" is available. This should prevent mixing up Texts classes from other packages.

### Bugfixes

- Fix missing user.GetIdentifier() causing cached roles to mixed up in BasePermissionService<T> implementation

## 8.7.5 (2024-10-31)

### Bugfixes

- Fix element returned from format function gets prepended to inner html of slick cell if the formatter also returns one of addClass, addAttrs, or tooltip properties.

## 8.7.4 (2024-10-30)

### Features

- Introduce a new ApplicationPartsTypeSource which gets list of assemblies from ApplicationPartManager and concats Serenity.Net assemblies. This will make manually written type source implementations obsolete and will allow dynamically including assemblies by adding to application parts. Note that even though it sorts assemblies topologically, the order of assemblies might not exactly match a manually written type source, which may have some side effects for code that expects a specific order, for example service behaviors.
- Add ExtensionsTypeSource and ProExtensionsTypeSource that may be used in Serene/StartSharp typeSource. They also include Serenity.Net.* and Serenity.Extensions assembly reference properties. ApplicationPartsTypeSource is preferred over them, but you may subclass them in your TypeSource if you want to preserve previous assembly order.
- Use queueMicrotask only for dialog types, as only they may have circular dependencies via forms causing swc to fail in tests.

### Bugfixes

- Fix auto column tooltip plugin not working without jQuery
- Fix uploader not reading multiple property from the input element properly (#7276)
- Fix scroll bar for full-height pages in Serene (#7277)

## 8.7.3 (2024-10-26)

### Features

- Use better marker types in TypeSource.cs that are less likely to move between assemblies in the future
- Removed .sln files and use .slnf files (solution filters) generated from a single Serenity.sln at root for partial builds. Use Serenity.sln instead of Serene.sln.
- Add a BaseDynamicDataGenerator that may be used in other projects to create dynamic-data folder for javascript tests
- Moved dynamic script abstractions up to Serenity.Core and base types to Serenity.Services
- Added a SetSergenTransformArgs target to Serenity.Net.Web.targets and use that instead of setting SergenTransformArgs in Feature.Build.targets and template .csproj files

### Bugfixes

- Fixed toolbar button with disabled class still clickable via its .button-inner element (#7272)

## 8.7.2 (2024-10-23)

### Features

- The Serene repository has been merged into the Serenity repository as a subfolder at `/serene`.
- The common features repository has been merged into the Serenity repository as a subfolder at `/common-features`.
- The `Toolbar.createButton` method has been made public and static again, as it is still being used.
- Moved rules from `common-style.css` to `common-theme.css`. The `common-style.css` file can now be removed from `Serene's appsettings.bundles.json`.
- Introduced a new `BasePermissionKeyLister` class, which can be overridden in Serene/StartSharp by `PermissionKeyLister` classes to reduce the amount of code in templates and apps.
- Introduced a new `BaseUserRetrieveService` to simplify the implementation of `UserRetrieveService`.
- Introduced a new `BaseRolePermissionService` to simplify the implementation of `RolePermissionService` in Serene/StartSharp.
- Added an `IUserProvider` abstraction that combines `IUserAccessor`, `IImpersonator`, `IUserClaimCreator`, `IUserRetrieveService`, and `IUserCacheInvalidator`. Use the generic version of the `AddUserProvider` extension to register it and its dependencies in one step, e.g., `services.AddUserProvider<AppServices.UserAccessor, AppServices.UserRetrieveService>()`.
- Added an `IRemoveCachedUser` abstraction to replace the static method used for removing cached users from `IUserRetrieveService`. The static method was not accessible from other service implementations in referenced libraries.
- Shared code between the MVC source generator and MVC command. Now generates partial static classes and always uses the project root namespace, avoiding a top-level `.MVC` namespace. It also generates internal instead of public MVC/ESM classes if `MVC:InternalAccess` is true in sergen.json, to prevent naming conflicts that could break the build.
- Introduced new `IdentityKey` and `AutoIncrement` migration builder extensions, which should be used instead of `CreateTableWithId32/64` (now deprecated). These extensions also resolve issues with how identity and primary key columns are handled differently in Oracle and MySQL.

### Bugfixes

- Fixed an issue in `SlickFormatting.treeToggle` related to formatters returning elements.
- Resolved data migration issues for Oracle and MySQL servers.

## 8.7.1 (2024-10-16)

### Features

- `ComboboxEditor` now preserves the order during `initSelection` for async/sync sources.
- Added an optional hook for `fetchScriptData` to intercept script data loading via fetch/xhr (sync), primarily for testing purposes.
- Utilized `queueMicrotask` for side-effect imports of referenced types, preventing issues with circular dependencies (noticed with SWC-based Jest tests).
- Enabled passing `callback` and `fail` arguments to `loadByIdAndOpenDialog`.
- Introduced an `Html.ModulePageInit` extension that generates a `pageInit` script and automatically includes related CSS if it exists.
- Added the `Html.AutoIncludeModuleCss` extension to auto-include an ES module's related CSS file (generated by esbuild) into pages, e.g., `GridPage`.
- Added a `layout` option to grid/panel page extensions.
- Implemented esbuild's CSS import/bundling functionality in samples like Northwind. Page-specific CSS rules are split into their own `XYZPage.css` files and imported by `XYZPage.ts`.
- Configured the TSBuild clean plugin to delete extra `.css` and `.css.map` files generated by esbuild.
- Updated various dependencies:
  - `Microsoft.Extensions.Caching.Memory` to 8.0.1
  - `System.Text.Json` to 8.0.5
  - `Microsoft.TypeScript.MSBuild` to 5.6.2
  - `FluentMigrator` to 6.2.0
  - `MailKit` to 4.8.0
  - `Mono.Cecil` to 0.11.6
  - `Npgsql` to 8.0.4
  - `Selenium.WebDriver` to 4.25.0
  - `xunit` to 2.9.2
  - `@swc/core` to 1.7.26
  - `typescript` to 5.6.2
  - `dompurify` to 3.1.7
  - `preact` to 10.24.2
  - `tsbuild` to 8.7.4

## 8.7.0 (2024-10-07)

### Features

- Completely rewritten 2FA authentication system supporting Email, SMS, Authenticator apps, and other methods by implementing relevant interfaces in `Serenity.Pro.Extensions`.
- Introduced a new 2FA settings UI to configure, enable, and disable 2FA methods for the current user.
- Added a `TwoFactorData` field to the Users table. This migration also removes the `MobilePhoneVerified` and `TwoFactorAuth` columns, as they are no longer used.
- New `Authenticator` two-factor method (`Serenity.Pro.TwoFactorAuthenticator`) allows using Authenticator apps (TOTP) for 2FA (available only in Business/Enterprise versions).
- Rewritten `~/Serenity.Extensions/common-style.css` and `~/Serenity.Extensions/common-theme.css` to align closely with the premium theme. These now include SlickGrid, Select2, and jQuery File Upload styles. Serene users should remove `slick.grid.css`, `jquery.fileupload.css`, and `select2.css` from their `appsettings.bundles.json` files.
- Added AI translation options to the Translation page. The default implementation works with any local or remote API compatible with OpenAI. Other APIs can be integrated by implementing an interface.
- Built-in types from corelib and SleekGrid are now included in the code generation process (Sergen/Pro.Coder). This reduces the occurrence of errors when dependencies are missing, such as when `npm install` fails or isn't run.
- Added more checks to the `sergen doctor` command, including version mismatches between `Serenity.Net.*` and `sergen`.
- Updated various libraries: Microsoft.Data.SqlClient to 5.2.2, Microsoft.Data.Sqlite to 8.0.8, MailKit to 4.7.1.1, X.PagedList.Mvc.Core to 10.1.2, Bogus to 35.6.1, @swc/core to 1.7.23, esbuild to 0.23.1, rollup to 4.21.2, terser to 5.31.6, tslib to 2.7.0, typescript to 5.5.4, chart.js to 4.4.4, dompurify to 3.1.6, tsbuild to 8.6.6, @preact/signals to 1.3.0, and preact to 10.23.2.
- Switched to `X.Web.PagedList` from `X.PagedList.Mvc.Core`, as the latter is deprecated.
- Replaced `StackExchange.Exceptional.AspNetCore` with a custom fork, `Serenity.Exceptional.AspNetCore` (https://github.com/serenity-is/exceptional), removing the remaining `System.Data.SqlClient` reference. This reduces platform-specific assemblies under `bin/runtime` by about 4MB.
- Removed the `Microsoft.Data.Sqlite` reference from `StartSharp.Web`. Users can add the reference manually if needed, saving 20MB of platform-specific assemblies.
- Sergen now generates an ESM helper (e.g., ESM entry points generator in Pro.Coder) in addition to an MVC helper with the `MVC` command.
- Shortcut constants are now generated directly under the ESM helper class for entry points with unique filenames matching root folders.
- Sergen now processes `.mts` files when parsing TypeScript code.
- You can now extend default EntryPoints in `sergen.json` by specifying `"+"` as the first item.
- Added a generic version of the `GridPage` extension that retrieves the page title from the Row type.
- Set `moduleResolution` to `bundler` (supported in TypeScript 5.0.2+) since esbuild/rollup are used for bundling.
- Added an `itemId` function to `DataGrid` that returns the ID property value of a given item.
- `ToolButton` now accepts `HTMLElement` as a title, simplifying the rendering of Bootstrap dropdowns and other buttons in the toolbar.
- Converted most corelib code using `document.createElement`/Fluent to JSX syntax.
- Set `ServerTypings` to use `PreferRelativePaths = true` (defaults@6.6.0) to avoid issues when no `@/*` mapping is found in `tsconfig.json`, or it is defined in a base file in another directory without a `baseUrl: "."` configuration.
- Code is now generated for abstract types (like base endpoints) that have a `ScriptInclude` attribute.
- Switched the `Texts.ts` declaration class to use the `texts` namespace instead of the root namespace.
- Generated shortcut constants for individual nested local text classes in `Texts.ts`, such as `ValidationTexts`, in addition to just `Texts`.
- Introduced a `TransformInclude` interface in corelib. When extended by an interface, it allows generating client types for those types during transformation, useful for compile-time checking of page props and other script interfaces referenced server-side.
- Improved class member ordering in code generation for source generators when a service/row class is split into partial files by ordering members by originating source file.
- Added distributed cache support to `Throttler`.
- Added a `Config.defaultReturnUrl` setting and `getReturnUrl` function in corelib, which can be used on the login page to get the return URL from the query string or fallback to a default. The `Config.defaultReturnUrl` function can be overridden for custom logic.
- Added a `ValidationError` constructor that accepts `localizer` and `local text message` arguments.
- Added `DataProtectorBinaryTokenExtensions` to streamline `IDataProtector` usage with `BinaryReader/BinaryWriter` and `Base64UrlEncode/Base64UrlDecode`, reducing repeated code when encoding/decoding tokens.
- Added a new `FunctionCallCriteria` base type and an `UpperFunctionCriteria` subclass to handle LIKE statements in case-sensitive databases by automatically applying `UPPER` for `StringField` types.
- Added several SleekGrid samples to `Serenity.Demo.AdvancedSamples`.
- Removed the direct dependency of `Serenity.Data` on `Microsoft.Data.SqlClient` by using reflection where necessary.
- Switched to esbuild for generating `corelib/wwwroot/index.global.js` instead of Rollup.
- Added `IconClassAttribute` to specify icons for UI elements, currently used for the 2FA interface.

### Breaking Changes

- Removed legacy namespace-based server typings generation support for Sergen/Pro.Coder. Existing generated files under `Imports/ServerTypings` will not be deleted, but migrating to modern ESM modules is strongly recommended.
- Removed legacy namespace mode code generation support in Sergen. It now only generates ESM-style code, even in projects with namespace-based code.
- The legacy Open Sans font under `~/Serenity.Assets/Content/font-open-sans.css` has been removed. Serene users should update `appsettings.bundles.json` to reference `~/Serenity.Assets/fonts/open-sans/open-sans.css`.
- The legacy `~/Serenity.Assets/Content/font-awesome.css` has been removed. Serene users should replace it with `~/Serenity.Assets/line-awesome/css/line-awesome-fa.min.css` to switch to the Line Awesome font.
- Removed unused assets such as `bootstrap-icons`, `tabler-icons`, `jquery.fileupload.css`, `jspdf.js`, `jquery.autoNumeric.js`, `jquery.validate.js`, and legacy font versions. 
- Removed `toastr.css` and `toastr.js` from `Serenity.Assets`. They are now integrated into corelib and pro-theme/common-style files.
- Removed `select2.css` and related files from `Serenity.Assets`. They are now integrated into pro-theme/common-style files.
- Sergen no longer supports the "restore" command as all assets are delivered as static web assets or via npm. Legacy namespace typings restore is no longer supported.

### Bug Fixes

- Fixed an issue where `this.time` could be null when `DateTimeEditor` uses flatpickr or the browser's default input.
- Fixed a script error in the `BasicProgressDialog` cancel function.

## 8.6.4 (2024-08-29)

### Features

- A new `dotnet sergen doctor` command that will check a project for issues related to environment and NPM/NuGet package versions.
- Update jsx-dom to 8.1.5

### Bugfixes
- Fix conversion issue with Sql VarBinary columns and setting to null (Implicit conversion from data type nvarchar to varbinary(max) is not allowed. Use the CONVERT function to run this query). This could effect SqlUploadStorage.

## 8.6.3 (2024-08-24)

### Features

- Introducing a new `ServiceEndpointModelBinderProvider` that can be added to `options.ModelMetadataDetailsProviders` in `Startup.cs` as `options.ModelMetadataDetailsProviders.Add(new ServiceEndpointBindingMetadataProvider());` instead of using `options.ModelBinderProviders.Insert(0, new ServiceEndpointModelBinderProvider());`.
- Added a `sanitizeUrl` method that can be used with JSX `href` attributes to sanitize URLs rendered from user content.
- Added a third optional `options` argument for extensions of `GridPage` and `PanelPage`.

### Bugfixes
- Fixed an issue where `DateTimeEditor` couldn't be set back to `readonly=false`.
- Corrected the text for the validation rule of lowercase characters in passwords.

## 8.6.2 (2024-08-11)

### Features
- **`[Breaking Change]`** Enhanced security against XSS attacks: HTML strings returned from formatters and format functions are now automatically sanitized by default. While formatters should already use `ctx.escape()` to handle any user-provided or dynamic content, this change adds an extra layer of protection. The default sanitizer is a basic regex-based solution, but if DOMPurify is available globally, it will be used instead. You can also specify a custom sanitizer by setting `gridDefaults.sanitizer` (in `@serenity-is/sleekgrid`) via `ScriptInit.ts`. Keep in mind that most sanitizers will strip out unsafe content, including JavaScript URLs like `javascript:void(0)`. To ensure security and compatibility, it is recommended to update existing formatters to use `jsx-dom` or `Fluent` instead of returning raw HTML strings. If necessary (though not advised), you can disable sanitization by setting `gridDefaults.sanitizer` to a pass-through function, such as `(dirtyHtml: string) => dirtyHtml`.
- Sergen now generates files with a `.tsx` extension instead of `.ts` for `Dialog`, `Grid`, and `Page` components, simplifying the use of JSX syntax (e.g., jsx-dom) in your projects.

### Bugfixes
- Fixed Pro.Coder ESM/MVC source generators do not operate properly with Visual Studio in some cases.
- Fixed an issue where the tooltip toggle method did not return the instance if it was disposed.

## 8.6.1 (2024-08-07)

### Features
- Add SleekGrid option to renderAllRows, similar to renderAllCels for columns, which effectively disables virtualization and preserves natural cell / row order when both is true, though it can be expensive for many rows / cells
- Set the default for SleekGrid useLegacyUI to false
- Use declare for class fields to avoid them becoming properties (e.g. defineProperty) when useDefineForClassFields is true (https://www.typescriptlang.org/docs/handbook/release-notes/typescript-3-7.html#the-usedefineforclassfields-flag-and-the-declare-property-modifier)
- Update TypeScript Scanner code to TypeScript current main (https://github.com/microsoft/TypeScript/commit/9757109cafcb771a35ad9fe09855373cdd82005a)
- Move service implementations under StartSharp.AppServices namespace to Modules/Common/AppServices folder, and move abstractions and models to Modules/Common/AppServices/Abstractions and Modules/Common/AppServices/Models folders
- Added Bootstrap buttons sample under UI Samples

### Bugfixes
- Fix frozen columns not rendered when scrolling vertically if horizontal scroll is bigger than frozen cols

## 8.6.0 (2024-07-13)

### Features
- Updated esbuild to 0.23.0 in tsbuild 8.6.0 which is necessary for modern (non experimental) decorator support in TypeScript. Please remove experimentalDecorators line from tsconfig.json after updating "@serenity-is/tsbuild" to 8.6.0 or later and update Visual Studio (for TypeScript update).
- Added experimental editor addon support, which provides ability to add content like icons, text, buttons inside and next to editors. See [Editor Addons Sample](https://demo.serenity.is/AdvancedSamples/EditorAddons) for more info.
- New EditorCssClass attribute that adds a css class to the editor element.
- Added value option to PropertyGrid, which if not "false", loads the given value as initial entity
- PropertyGrid is divided into functional sub components like PropertyFieldElement, PropertyFieldCaption, PropertyGridCategory, PropertyTabs etc.
- PropertyGrid defaultCategory and categoryOrder options which was unused
- Remove first-category class from the property grid's first as it can be done via css
- Add a vertical gap between categories
- Remove category links generation in property grid (they were hidden before)
- Aded data-category attribute to category divs in property grid
- Allow passing editor type to the propertygrid item via its type reference instead of string
- Removed unused toastr options like closeMethod, closeDuration, closeEasing, showDuration, hideDuration etc. which was related to animation but were not implemented.
- By setting timeOut option to -1 for toastr, the notification can be made sticky. It required setting both timeOut and extendedTimeOut to 0 before.
- Add ability to lazily load types via Config.lazyTypeLoader callback. For it to work getOrLoad / tryGetOrLoad functions of corresponding type registry (e.g. DialogTypeRegistry / FormatterTypeRegistry etc.) should be used instead of get/tryGet which may result in async behavior that should be handled by the caller.
- Handle passive event listener warnings for combobox editor
- Try to avoid Router closing dialogs when clicking a hash link that does not look like a handled route (e.g. #edit/..., #new etc.)
- Drag drop in tree grid sample no longer requires jquery.event.drag
- Determine the default editor type as TimeSpanEditor for TimeSpan typed properties. (#7206). Add multiplier option to legacy TimeEditor (integer value) that allows storing seconds or milliseconds instead of the minutes which was the default.
- Avoid signal aborted without reason errors in console for service calls

### Bugfixes
- Fix criteria DateTimeOffset and DateTime JSON parsing (#7180) caused by difference in System.Text.Json
- DateTimeFormatter does not use the passed DisplayFormat.
- Don't return empty string for Fluent.val on non-existing inputs, e.g. when this.el is null. Return undefined instead like it was for jQuery.
- Fix filter display value not showing for enum editor, #7193
- Add back css for .drag-helper which is used in drag drop in tree grid sample

## 8.5.6 (2024-06-13)

### Features
- Replace the internal "remove" event, triggered by Fluent when an element is cleaned up (e.g., by calling `remove()` on itself or `empty()` on its parent), with a new "disposing" event. Note: This may be a breaking change if you were using this undocumented event.
- Enable the space key in the date-time editor to set the current date and time again (only when the field is empty or all text is selected).
- Added support for `ByteArrayField` and `RowField` in the `GenerateRowFields` attribute.

### Bugfixes
- Fix the esbuild minifier locks for empty code blocks.
- Resolve the issue where the column picker dialog closes when dragging and dropping.

## 8.5.5 (2024-06-08)

### Bugfixes

- Fix static Dialog.onClose for modals when the modal is not initialized as the event triggered on body is not regular bs modal events

## 8.5.4 (2024-06-08)

### Bugfixes

- Fix assigned fields array for more than 64 fields is not cleared during clone if only one of the first 64 fields were assigned before cloning

## 8.5.3 (2024-06-07)

### Features

- **`[Breaking Change]`** Moved image processing methods like `ScaleImage` from `UploadStorageExtensions` to `DefaultUploadProcessor` as virtual methods. Removed `FileUploadBehavior.CheckUploadedImageAndCreateThumbs` and used the existing `IUploadProcessor` interface instead.
- Added additional metadata such as `EntityType` (row full name), `EntityTable` (table name), `EntityId` (ID property value), `EntityField`/`EntityProperty` (field/property where the upload editor is placed) to enable tracing files back to their original entities. Also set generated `ImageSize` and `ThumbSize` as metadata to avoid loading image files to get actual sizes when required. This prepares for adding custom metadata like tags, descriptions, etc., to files if desired.
- `IUploadProcessor` no longer returns a `Success` flag and raises an exception instead. The check for `.Success` should be removed from `FilePage` after updating to Serenity 8.5.3+. The property still exists but is obsolete and will always return true, so this is not a breaking change.
- There is no need to set the original name in `FilePage` as the upload processor handles it now.
- Improved handling of situations where a script or CSS file is locked by a process when trying to get the content hash.
- Made EsBuild the default script/CSS minifier, as Nuglify has issues with some script syntax.
- Added `allStart` / `allStop` events to the uploader and triggered `allStart`, `allStop`, `batchStart`, `batchStop` events on upload input if available.
- Updated Google Maps editor sample for the latest API changes.

### Bugfixes

- Fixed an issue where the widget constructor would fail when an empty array is passed as an element.

## 8.5.2 (2024-06-05)

### Features

- Added a "More Information" link (https://serenity.is/docs/av) for the failed AV scan message.
- Improved the combobox editor by attempting to transfer the text typed into the inplace-add dialog, if the dialog provides a name property via its getNameProperty function.

### Bugfixes

- Fixed an issue where upload editors did not display errors returned from TemporaryUpload and incorrectly assumed the upload was successful, even in cases of blacklist or other validation errors.
- Resolved a problem where the data audit log migration added in version 8.5.1 failed in SQLite (e.g., tests/Linux) due to SQLite's lack of support for altering columns.

## 8.5.1 (2024-06-04)

### Features

- Added optional generic type arguments to several Fluent functions, such as `closest`.
- `Fluent.findEach` now passes the index as the second argument to the callback.
- Introduced static versions of `Dialog.onOpen` and `Dialog.onClose` that function even when the dialog is not yet initialized, as the mode (modal, panel, UI) is unknown at that time.
- Added `onOpen` and `onClose` methods to `BaseDialog` to attach event handlers for dialogs, which operate even before the dialog is initialized.
- Changed the second argument (previously `before`) to an options object for `Dialog.onOpen` and `Dialog.onClose` to allow the `oneOff` option. This change may be breaking if the second argument, such as `before`, was used.
- Switched to using a bitmask instead of a boolean array for assigned field tracking. If the number of fields is less than 64, this reduces the amount of allocations.
- Released `tsbuild` 8.5.0, which updates esbuild to 0.21.4. After updating `tsbuild`, the `experimentalDecorators` option in `tsconfig.json` is no longer needed when using TypeScript 5.0+.
- Added a context argument to the `Serenity` decorator function to support JavaScript decorators in addition to TypeScript experimental decorators.
- Utilized `CSS.escape` to prevent issues when a field name contains special characters while searching for quick filters.
- Enabled pending changes confirmation to work even when the dialog is not yet initialized. Normally, `DialogUtils.pendingChangesConfirmation` should be called from the `initDialog` method, but some instances call it from the constructor where the dialog is not yet created.
- Increased the size of the `TableName`, `RecordId`, and `FieldName` columns to 200 in the `DataAuditLog` table (StartSharp).

### Bugfixes

- Fixed: Replaced `select *` with `select datname` in PostgreSQL database queries for data migrations.

## 8.5.0 (2024-05-16)

### Features

- Allow `permissionService` to be passed as `null` to the `TransientGrantingPermissionService`. This enables wrapping within another permission service. If `permissionService` is `null` and no transient permissions are granted, it returns `false`.
- Add `ITransientGrantor` interface to `LogicOperatorPermissionService` to ensure the wrapped permission service retains transient granting support.
- Add `ITransientGrantor` interface to `OpenIdScopePermissionWrapper` to preserve the `ITransientGrantor` interface if the wrapped permission service implements it (StartSharp).
- Add `ITransientGrantor` support to the default `IPermissionService` implementation (`PermissionService`), eliminating the need for separate configuration in `Startup.cs`.
- **`[Breaking Change]`** Added `IsAllGranted` and `GetGranted` methods to the `ITransientGrantor` interface. Custom implementations not using `TransientGrantingPermissionService` should be updated to include these methods.
- Add impersonation/transient grant support during report callback execution based on current user/transient granting status. This is useful for background task report generation.
- Introduce `AddSingletonWrapped` extension to simplify wrapping services like `IPermissionService`.
- Added `AsObjectNoCheck` function to skip `TrackWithChecks` errors for behaviors, etc. Use with caution.
- Add `FileName` property to `ReportRenderResult` to determine the download name from the result without accessing the rendered report.
- Introduce `IReportCallbackInterceptor` interface.
- Allow passing `bubbles`/`cancelable` via event args through `Fluent.trigger`.
- **`[Breaking Change]`** Rename previously exported `SleekGrid` compat editor types, such as changing `IntegerEditor` to `IntegerCellEdit`, to avoid clashes with corelib editor types. These legacy editor types are rarely used in Serenity apps, so this change should not cause issues.

### Bugfixes

- Fixed batch ending condition in the `Uploader` class affecting multiple uploads (#7139).
- Added support for jQuery UI tabs active/link selectors in `TabExtensions` methods.
- Restored color application to category titles as previously implemented.

## 8.4.8 (2024-05-03)

### Features:

- New AuditedAttribute which when placed on a row property like Audited(false), can skip auditing for that particular field
- Add option to prefer relative paths in sergen.json ("../ServerTypes/" instead of "@/ServerTypes/") which is better when directly referencing TypeScript source code in feature projects from Jest tests etc.
- Hide category links in Serene just like in StartSharp. Category links are deprecated and will be eventually removed.

### Bugfixes:
- Remove extra folder for RequestHandlers while generating code with sergen
- Fix clicking on category link click closing dialog, due to renderContents called before derived initializers (e.g. arrow functions) run, causing this.categoryLinkClick to be undefined.
- Fix upload issue in Serene due to HandleUploadRequest in FilePage returning ServiceResponse type instead of UploadResponse, resulting in missing value in JSON serialization.

## 8.4.7 (2024-04-27)

### Features:

- Introduced a new IInitializeLocalTexts abstraction, which should be used in Startup.cs instead of an InitializeLocalTexts method by calling services.AddLocalTextInitializer() in ConfigureServices, and calling app.InitializeLocalTexts in Configure method.
- Add option to show service errors as notification by passing third arg as true. Show errors in slick remote view as notification instead of alert dialog like it was before. 
- Made messages shown in showServiceError function localizable
- Set UseHardlinksIfPossible to true for copying files under node_modules/.dotnet
- Removing get_entity/set_entity/get_entityId/set_entityId methods in entity/property dialogs, which were remaining from Saltaralle times and using properties instead
- Moved byId and findById methods up to Widget class as preparation for obsoleting/removing TemplatedWidget
- Removing TemplateScript type and registration. .Template.html and ts.html files were already deprecated before along with getTemplate which will be removed soon.
- TemplatedWidget is deprecated, use Widget instead. TemplatedPanel is deprecated, use BasePanel. TemplatedDialog is deprecated, use BaseDialog. These changes reflect deprecating legacy template mechanism in 8.2.0. Use renderContents method to return markup via tsx/jsx-dom, or Fluent(...).getNode(). Even though legacy getTemplate() will be supported internally, it is not recommended.
- Allow passing editorType as type in addition to string for PromptDialog
- Pad column names with spaces to avoid unique column errors while exporting to Excel (if multiple columns has the same display name), and also add _numbers to table names
- Set IgnoreHTTPSErrors to true for PuppeteerHtmlToPdf to overcome development certificate issues

### Bugfixes:

- Fix TemporaryFileHelper.TryDeleteOrMark not working properly with virtual file systems
- Fix prompt dialog validateValue callback
- Fix SqlFileSystem null reference exception during GetFiles
- Fix impersonation links sometimes failing due to base64 / url encoding and caching issues
- Add missing top/left to select2-offscreen which was in the original css. This was causing unnecessary scroll bars.

## 8.4.6 (2024-04-16)

### Bugfixes:

- Fix sortablejs removing draggable attribute from .slick-resizable-handle after columns change, stopping column resizing
- DataExplorer might fail on Postgres as migrator create ID field with quotes
- Fix sorting issue when data explorer grid is sorted manually by ID, and clear sort columns when changing table
- Missing brackets in DisplayName expression for EmailQueueRow causing issue in Postgres due to the way FluentMigrator quotes field names by default

## 8.4.5 (2024-04-16)

## Features:

- Update SixLabors.ImageSharp to 2.1.8 (2.1.7 seems to have a vulnerability)

### Bugfixes:

- Contains operator not working properly in filter dialog
- Select2 positioning does not work inside a modal when page is scrolled down

## 8.4.4 (2024-04-06)

### Features:

- Improve drag drop group by handling StartSharp
- Added an unhandledRejectionHandler which should be installed via ScriptInit.ts or errorhandling-init.ts file via `window.addEventListener("unhandledrejection", ErrorHandling.unhandledRejectionHandler);` so that it does not result in unhandled promise errors for serviceCall/serviceFetch methods.
- **`[Breaking Change]`** As jsx-dom has recently fixed an issue (before version 8.1.4) where ref callbacks were not executed for React style class components like Serenity Widgets, we will no longer call the ref callback from the Widget itself to avoid double execution. This might cause problems when an older version of jsx-dom is used. So, after updating to Serenity 8.4.4+, please update jsx-dom in your package.json to 8.1.4+ and run npm install.
- Widget.element is no longer deprecated, it just returns a Fluent object wrapping this.domNode
- Added a Fluent.findEach function that executes a callback for each element found with a Fluent object
- Added Fluent.after and Fluent.before methods similar to jQuery ones.
- Added frequently used click/focus methods to Fluent to make it easier to port from jQuery
- Add static versions of Fluent.findAll, Fluent.findEach, Fluent.findFirst that works with the document
- Removing typings for jQuery extensions getWidget and tryGetWidget as they are causing a global jQuery object to be available in TypeScript context even if it doesn't. The methods are still there. If you still use them cast jQuery object to any or add a global.d.ts with contents of old jquery-globals.d.ts that is removed in the commit.
- Message dialog types are now ignored for routing, e.g. they don't modify the URL hash when opening/closing
- Updated System.Text.Json to 8.0.3, Dapper to 2.1.35, Microsoft.Data.SqlClient to 5.2.0, System.IO.Abstractions.TestingHelpers, coverlet.collector, Selenium.WebDriver and db connectors in sergen
- Updated SleekGrid to 1.8.0

### Bugfixes:

- Select2 input was not readonly for multi select even if the select itself is readonly
- Fix select all button function in CheckLookupEditor
- Label style width was set to XYZpxpx instead of XYZpx breaking form label widths
- Missing parens in coalesce for vMin calculation in IntegerEditor
- Allow clearing by backspace in autonumeric based editors when minValue > 0

## 8.4.3 (2024-03-25)

### Bugfixes:

- Fix possible closure variable issue with edit link formatting

## 8.4.2 (2024-03-24)

### Bugfixes:

- Fix select2 null ref error in some cases

## 8.4.1 (2024-03-22)

### Features:

- The url method in jquery validate port made functional again.
- JsonField.ValueFromJson is now be able to read from non string fields just like Newtonsoft version did before the System.Text.Json switch

## 8.4.0 (2024-03-20)

### Bugfixes

- Panel root element is not removed on destroy

## 8.3.9 (2024-03-20)

### Bugfixes

- Fluent.getWidget and Fluent.tryGetWidget uses wrong target and fails
- Order for reason / message in service call / fetch Promise reject reason

## 8.3.8 (2024-03-19)

### Features:

- Improve generated dynamic script code behavior when Serenity is not pre-loaded
- Make getWidget and tryGetWidget available on Fluent. It was normally available but declaration merging did not properly change "@serenity-is/base" to "@serenity-is/corelib" although base was bundled into corelib
- Improve router resolve handling when pending requests are in progress
- **`[Breaking change]`** Panel events will fire at the .panel-body instead of .s-Panel. This will make it compatible with ui dialog behavior and will allow attaching panelopen events before the panel body is wrapped with a s-Panel. We also now fire new modalbeforeopen modalopen etc. events at .modal-body for Bootstrap modal to make behavior consistent between all dialog types. This is required as it is possible for our dialogs to be opened later as a panel, ui-dialog or modal, but this information is not available on Dialog class creation and the body element is not yet wrapped with any of dialog types yet.

## 8.3.7 (2024-03-10)

### Features:
- Added `Fluent.matches` method which is like $.is but don't support any custom selectors and calls element.matches
- Added `Fluent.class` method which directly sets the className property. This should be preferred to addClass for element creation
- Added a `Fluent.isDefaultPrevented` helper to overcome issues when jQuery exists or not so that we don't have to check both e.defaultPrevented and e.isDefaultPrevented?.()
- Added a `Fluent.eventProp` helper to read a property from event, event.originalEvent or event.detail as jQuery does not pass custom properties like route etc. to the synthetic event.
- `SubDialogHelper.bindToDataChange` auto extracts the original event if used with jQuery so that data change info can be easily accessed
- Also trigger ajaxStart/ajaxStop events on document when jquery is not available
- NProgress works when jQuery is not available (StartSharp)
- Allow directly passing formatSelection and formatResult from ComboboxOptions to Select2, so that providerOptions don't need to be set explicitly.

## 8.3.6 (2024-03-03)

### Features:
- Add Fluent.byId which is shortcut for Fluent(document.getElementById)
- Add Fluent.each and Fluent.style which executes a callback with the node or css style declaration if element is not null

### Bugfixes:

- Column resizing does not work in Firefox without jQuery resizable due to longstanding Firefox issue

## 8.3.5 (2024-02-28)

### Bugfixes:

- Avoid using js reserved words while generating variable names for compacting property item scripts
- Fix code for disabling combobox inplace button dialog's delete button via Fluent (https://github.com/serenity-premium/startsharp/issues/915)

## 8.3.4 (2024-02-24)

### Features:

- New actions dropdown in grid sample for StartSharp.

### Bugfixes:

- ComboboxEditor.get_text returning `[object Object]` affecting inline editing etc.
- CKEditor container class assignment
- Fix Drag Drop in Tree Grid sample which requires jquery and jquery.event.drag scripts.

## 8.3.3 (2024-02-21)

### Bugfixes:

- Fix select2/combobox paging more results calculation

## 8.3.2 (2024-02-20)

### Features:

- Implemented the use of AsyncLocal instead of ThreadLocal for ImpersonatingUserAccessor and TransientGrantingPermissionService. This change enables background tasks to work with async code even when HttpContext is unavailable.
- Changed event handling from touchstart/mousedown to click for mobile browsers, preventing the select2 dropdown from opening while touch scrolling.
- Applied focus to select2 when the hidden input is focused, such as by clicking its label.
- Removed left/top rules from select2-offscreen to prevent the hidden input from scrolling to the top of the page when its label is clicked.
- Updated the following packages:
  - Microsoft.Data.SqlClient to 5.1.5
  - Dapper to 2.1.28
  - Nuglify to 1.21.4
  - System.Text.Json to 8.0.2
  - Microsoft.Data.Sqlite to 8.0.2
  - Microsoft.TypeScript.MSBuild to 5.3.3
  - Markdig to 0.35.0
  - Bogus to 35.4.0
  - PuppeteerSharp to 14.1.0
  - Updated Sergen's references Scriban, Microsoft.Data.Sqlite, MySqlConnector, Npgsql, Spectre.Console to the latest versions.
  - Several packages in test projects including Selenium.WebDriver to 4.17.0, WebDriverManager to 2.17.2, System.IO.Abstractions.TestingHelpers to 2.0.15.
- Added `fullscreen-lg-down` class to FilterDialog.
- Updated OpenIddict to 5.2.0 in StartSharp.
- **`[Breaking Change]`** Removed external Select2 script usage since 8.3.0 and embedded it. Replaced Select2.util.stripDiacritics with the `stripDiacritics` method from `@serenity-is/corelib`.

### Bugfixes:

- Ensured destruction of flatpickr instance for dateeditor/datetimeeditor.
- Fixed Flatpickr clearing the input value on focus out of readonly input as disable returns true for the current date.
- Addressed an issue where some nodes may not have the hasChildNodes method (https://github.com/orgs/serenity-is/discussions/7080) and Fluent.empty is called on the wrong element in Select2.
- Resolved a scenario where Comboboxeditor was sometimes returning `more: true` while there are no more pages.
- Corrected WarningDialog to show WarningTitle instead of SuccessTitle (#7082).
- Added a check for null before Fluent.prependTo.
- Applied class toggle for category collapse icon in propertygrid.ts (#7084).

## 8.3.1 (2024-02-09)

### Bugfixes:

- Fix potential stack overflow with Fluent.remove

## 8.3.0 (2024-02-09)

### Features:

- In this version, we are removing the jQuery dependency, along with other scripts like `Select2`, `jQuery Validate`, `autoNumeric`. These three plugins have been converted to TypeScript without jQuery dependencies and are embedded into Serenity Corelib. The jQuery mask input plugin and Colorbox are also removed, but they do not have any alternatives, and we do not plan to introduce any. MaskedInput is not used anywhere in our templates/samples and had very limited usage. Colorbox was used for viewing larger images by clicking upload editor thumbnails, but they will simply open in a new tab for now.
- Fluent removes the attribute if the value passed to the Fluent.setAttribute is null.
- Added prevSibling and nextSibling functions to Fluent, which work similarly to prevAll and nextAll except for a single element, and when the selector is null, they act like .prev/.next.
- **`[Breaking Change]`** jQuery Validator is converted to TypeScript by removing jQuery dependency and is embedded in the Serenity corelib. You should remove the one in appsettings.bundles.json as it will no longer be used.
- The default date format for upload format date is `yyyyMMddhhmmss` if not specified. (#7067)
- **`[Breaking Change]`** Select2 is converted to TypeScript by removing jQuery dependency and is embedded in the Serenity corelib. You should remove the one in appsettings.bundles.json as it will no longer be used.
- **`[Breaking Change]`** The base Select2Editor is renamed to `ComboboxEditor`, and a `Combobox` abstraction class is introduced. This change will make it possible to replace Select2 with another library in the future if desired. Several Serenity specific types like Select2SearchQuery etc. are renamed to `ComboboxSearchQuery` and similar. Some methods like getSelect2Options etc are renamed to `getComboboxOptions`. To pass Select2 specific options, you should override that method and provide a `getProviderOptions` callback.
- Flatpickr will no longer open on click by default, only on button click just like it was the case with UI datepicker. Flatpickr options can be changed for DateEditor/DateTimeEditor by overriding the getFlatpickrOptions method.
- Bootstrap Modal does not allow focusing on any element outside the modal, so any inputs in flatpickr/ui datepicker/select2 etc. could not be focused while inside a modal. We applied some workarounds/patches to those components and Bootstrap itself.
- Use Fluent.toggle for hiding grid field in the property grid instead of toggling the hidden class, so that it can be shown with Fluent.toggle again (https://github.com/serenity-premium/startsharp/issues/892).
- **`[Breaking Change]`** jquery.autoNumeric is converted to TypeScript by removing jQuery dependency and is embedded in the Serenity corelib (We evaluated the latest version of autoNumeric which does not have jQuery dependency, but it is almost 10 times the size). You should remove the one in appsettings.bundles.json as it will no longer be used.
- **`[Breaking Change]`** MaskedEditor, which depends on a very old jquery.maskedinput plugin, is deprecated. You may still use it by including jQuery and the plugin, but it is not recommended. The option is to add a validation rule or find another mask input library.
- **`[Breaking Change]`** Colorbox is removed from StartSharp/Serene as we no longer have jQuery. You may still use it by including it together with jQuery, but it is not recommended. Currently, the images will open in a new tab for upload editor thumbnails.
- Added Tooltip.isAvailable method to check if bootstrap tooltip is available.
- **`[Breaking Change]`** `@Decorators.option` is deprecated and will be removed completely in a future version as it is not compatible with JSX. Please convert them to regular constructor options/props.
- Replaced several jQuery plugins used in StartSharp Dashboard with other options like Sortable.

### Bugfixes:

- Fix TextAreaEditor displaying undefined on new record mode.
- Bootstrap 5 Tooltip display issue when jQuery is not loaded.
- The flatpickr trigger button now has type "button" so that it does not cause submit/validation.
- Space key does not set the date editor to today with flatpickr.


## 8.2.2 (2024-01-21)

### Features:

- Change autoOpen to true for Dialog class (not TemplatedDialog/EntityDialog etc.)
- Support returning false or a promise that return false from Dialog button click event to prevent closing for dialog with buttons that have a "result"

### Bugfixes:
- Wrong check for getting readonly state in select2editor

## 8.2.1 (2024-01-20)

- This release incorporates significant changes, notably the removal of jQuery UI dependency from our libraries. Although jQuery is still employed for select2, validation, and a few other widgets, these will also be replaced or removed in the near future. Please refer to the release notes at [https://serenity.is/docs](https://serenity.is/docs) for guidance on migrating your code.
- Introducing a `Fluent` object/function, similar to `jQuery` but designed for a single element, offering a subset of functions. Its aim is not 100%, not even half compatibility, and it serves to simplify the migration of existing code. The Widget's `element` property now returns a `Fluent` object instead of a `jQuery` object. It is recommended to use `.domNode` instead of the `.element` property where possible, with the latter marked as deprecated for easier identification of its usage.
- Serenity widgets can now be seamlessly used with jsx-dom without requiring wrapping. Widget components can be created without passing a jQuery element, as the component will automatically create its element, akin to calling `Widget.create`.
- The TypeScript scanner and parser are entirely rewritten based on the latest TypeScript source code, passing over 12,000 test cases and resulting in the same set of tokens/AST nodes.
- **`[Breaking Change]`** SleekGrid scripts (e.g., `slick.core.js`, `slick.grid.js`, etc.) will now be shipped via the new `Serenity.SleekGrid` package instead of `Serenity.Assets`. Update all references from `~/Serenity.Assets/Scripts/slick.*` to a single reference of `~/Serenity.SleekGrid/index.global.js` containing all those scripts bundled together.
- The outdated `jquery.fileupload.js`, which had a jQuery UI dependency, is no longer required as we now have a custom uploader component. Remove it from `appsettings.bundles.json`.
- Introducing `~/Serenity.Assets/Scripts/jquery-ui-datepicker.js`, which should replace `~/Serenity.Assets/Scripts/jquery-ui.js`. It exclusively contains the jQuery DatePicker widget. StartSharp users should refer to the latest source code to integrate flatpicker instead.
- SleekGrid column formatters now support returning HTML elements, either through `document.createElement` or jsx-dom. This feature is currently experimental.
- Merged the `jQuery.dialogExtendQ` plugin into corelib. Remove the inclusion in `appsettings.bundles.json`.
- Implemented a compact version of the column/property script bundle, reducing uncompressed script size by about 65%.
- Introducing `faIcon` and `fabIcon` helpers providing autocompletion for `Line Awesome` (Font Awesome) icons.
- Added `gridPageInit` and `panelPageInit` functions for use in GridPage and PanelPage, replacing `initFullHeightGridPage`.
- Sergen now has limited support for type literals and intersection types for options/props in addition to simple type aliases and interfaces.
- Introduced `IPasswordStrengthValidator` interface for abstraction of password strength rules and validation (#7054).
- To obtain widgets directly from an HTML element, use the new `tryGetWidget` and `getWidgetFrom` helpers instead of their jQuery counterparts.
- **`[Breaking Change]`** All dialogs deriving from `EntityDialog` are panels by default. Add `@Decorators.panel(false)` to make one open as a modal/dialog. All those deriving from `GridEditorDialog`/`PropertyDialog` are modal/dialog by default.
- Panels can now have dialog buttons. Use `@Decorators.staticPanel` to remove dialog buttons inherited from `PropertyDialog`, along with the close button.
- Adjusted `TabExtensions` to handle Bootstrap tabs when jQuery UI is not available.
- Ensure that if another reset password token is requested, the prior one becomes invalid by updating the `LastUpdateDate` of the user.
- Also handle `SingleField` in Pro.Coder `RowFields` source generator.
- Hide category links by default. Use CSS `.category-links { display: flex }` if you want to bring them back.
- Improved wrapping behavior of toolbar buttons. The extra divs with `.buttons-outer`, `.buttons-inner`, `.button-outer` are all removed.
- - **`[Breaking Change]`** Templates, e.g. `.Template.html` or `.ts.html` files are deprecated. Use tsx and `renderContents`. Even though it is still possible to use `getTemplate()` method for now, it will be removed in the next version. TemplatedDialog, TemplatedPanel etc. classes will be removed or replaced with versions that does not use templates.

