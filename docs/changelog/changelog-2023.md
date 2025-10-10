# CHANGELOG 2023

This changelog documents all Serenity versions published in the year 2023 (versions 6.4.3 through 8.1.5).

## 8.1.5 (2023-12-14)

### Features:
- Removing jQuery.blockUI.js and toastr.js from Serenity.Assets as we no longer use them. Please remove them from appsettings.bundles.json if you still have them there.
- Include source maps of base and base-ui in corelib
- No need for "file:" prefix in package.json for "./node_modules/dotnet/"

## 8.1.4 (2023-12-13)

### Features:
- Use lowercase version of folder name while restoring to node_modules/.dotnet
- Set @serenity-is/corelib to file:./node_modules/.dotnet/serenity.corelib so that it uses the same version of the Serenity.Corelib NuGet package referenced in the project.

## 8.1.3 (2023-12-13)

### Features:
- **`**`[Breaking Change]`**`** Replacing Serenity.Scripts with a new `Serenity.Corelib` package which will be a NuGet version of the NPM package `@serenity-is/corelib`. Replace "Serenity.Scripts/Serenity.Corelib.js" in appsettings.bundles.json with "Serenity.Corelib/index.global.js".
- When a module is not found, for example when npm install is not run, try to resolve modules via "file:" mappings configured in dependencies and also NuGet package folders.

### Bugfixes:
- Fix jquery ui datepicker culture not working

## 8.1.2 (2023-12-10)

### Bugfixes:
- Remove extra comma in generated package.json files, causing failure only with pnpm

## 8.1.1 (2023-12-10)

### Features:
- Implemented a better method for installing node types from NuGet packages and project references. Just remove "prepare" script in package.json and add "preinstall": "dotnet build -target:RestoreNodeTypes". Serenity will automatically install nuget/package refs to node_modules/.dotnet directory and patch your package.json so that there is a reference to every library that is referenced by your project. This will improve TypeScript experience. As the folder name under .dotnet will be created with the project/nuget package names (not npm package id) alternative path mapping for `@serenity-is/*` suggested in 8.0.9 will not be useful, and `@serenity-is/*` path mapping can be removed after upgrading.

## 8.1.0 (2023-12-10)

### Bugfixes:
- Reverted running RestoreNodeTypes in design time build as it might be breaking NuGet restore order

## 8.0.9 (2023-12-10)

### Features:
- Also restore node types for project/package references to "./node_modules/.dotnet/" so that these files don't get erased during npm install, leading to typescript compilation warnings during initial project creation. 
- Try to run RestoreNodeTypes target also in design time builds. It is recommended to replace `"prepare": "dotnet build -target:RestoreTypings"` line with `"prepare": "dotnet build -target:RestoreNodeTypes"` if you only use modular code.
- Changed fetchScriptData logic to better handle exceptions. Increase test coverage for lookup and scriptdata methods.

## 8.0.8 (2023-12-09)

### Features:
- Replace E-Mail with Email and e-mail with email in texts for consistency and it seems like it is now the accepted norm

### Bugfixes:
- Fix error in uiPickerZIndexWorkaround
- Fix MailboxLoginView demo and predefined logins in Serenity.Pro.EmailClient

## 8.0.7 (2023-12-07)

### Bugfixes:
- Special case for distinct lookup scripts returning array of values instead of objects, causing distinct lookup scripts to fail displaying properly when fetched via DynamicData endpoint
- Enum flags editor was broken after recent updates

## 8.0.5 (2023-12-04)

### Bugfixes:
- Issue with async fetch not returning data properly, causing async lookup loading to fail

## 8.0.4 (2023-12-04)

### Features:
- Trigger jQuery ajaxStop and ajaxStart events and increase/decrease jQuery.active during fetch requests so that processes like NProgress can be notified until we implement our own events

## 8.0.3 (2023-12-04)

### Features:
- Using MSBuild's new command-line build property extraction feature in Sergen for .NET 8+ to determine properties like OutDir, TargetFramework, etc. in a more consistent way.
- Made it possible to pass MSBuild properties to Sergen so that it does not have to parse property items from the project file. This helps with cases where it cannot determine a property correctly, such as output path, configuration, etc. This may also speed up transforms as Sergen does not have to parse properties itself. Refer to Directory.Build.targets in the common-features repository for a sample and `-prop` syntax.
- Implemented finding tsconfig extends from node_modules folders when the extends property contains a non-relative path. This is similar to how TypeScript implements extends via the node resolution algorithm.
- Improved Sergen's algorithm for discovering included files from tsconfig.json to match TypeScript itself as closely as possible.
- Cached file system for TypeScript parsing transformations, speeding up transform times in Sergen.
- Added an option to ignore null value for `[Unique]` constraint on a single field.
- Q.serviceCall / Q.serviceRequest methods and generated services now return `PromiseLike` instead of `JQueryXHR`. This is preparation for the replacement of jQuery ajax with fetch in the future. It is still possible to cast the return type to JQueryXHR for now, but don't rely on it always returning such type.
- **`[Breaking Change]`** `IFileSystem` has a new `GetLastWriteTimeUtc` property. This is done to remove the extra `IGeneratorFileSystem` interface and the same property from `ITemporaryFileSystem`. If you have a custom implementation for `IFileSystem`, you should also implement the `GetLastWriteTimeUtc` method.
- **`[Breaking Change]`** No longer shipping TypeScript's `tslib` functions (e.g., helpers) along with Serenity.CoreLib.js. If you still have namespaces (non-modular) TypeScript code, please remove the `"noEmitHelpers": true` line from your tsconfig.json.
- The `localText` function now accepts a second optional parameter that will be used as default if a translation is not found in the registry.
- Moved more functions into the base library from corelib. Removing usages of `Q.isEmptyOrNull`, `Q.startsWith`, `Q.endsWith`, `Q.some`, etc. functions as JavaScript has better alternatives.
- Corelib and templates will target ES2017 instead of ES5. No issues are expected as ES2017 support in browsers is at 98%, and the remaining 2% mostly includes IE, which we haven't supported for a long time.
- `ScriptData` functions like `getLookup`, `getForm`, `getColumns`, etc., will use `~/DynamicData` (e.g., JSON loading) instead of `~/DynJS.axd` (script loading) by default. This will avoid calling `eval`, which is considered harmful, while dynamically loading lookups, etc.
- Using `fetch` instead of jQuery ajax/XHR for async versions of `ScriptData` functions like `getLookupAsync`, etc.
- JSON.NET serialized enums as numbers by default, while we used `JsonStringEnumConverter` with `System.Text.Json`, which broke handling in some cases. We implemented a new `EnumJsonConverter` that is more compatible with the JSON.NET behavior.
- In tsconfig.json, `jsx-dom/min` is set as the default `jsxImportSource` in common-features and pro-features projects, in addition to StartSharp itself.
- Serenity no longer uses the legacy jQuery blockUI plugin even if it is loaded.

### Bugfixes:
- `GetRelativePath` workaround for Pro.Coder is not working exactly the same as `System.IO.Path.GetRelativePath`, which is not available in .NET Standard 2. This was causing strange paths in transformed files from time to time.
- `PopulateObject` failed when the generic type is an interface, causing reports with parameters to not work properly.
- Fixed NullReferenceException during impersonation.

## 8.0.2 (2023-11-22)

### Bugfixes:
  - Sergen not able to read connection strings from appsettings.json due to handling difference by System.Text.Json

## 8.0.1 (2023-11-22)

### Features:
- We are migrating from `Newtonsoft.Json` to `System.Text.Json`, as it is the recommended and more performant library in .NET. To make the switch, please remove the `.AddNewtonsoftJson` statement from `Startup.cs`, add the line `services.Configure<JsonOptions>(options => JSON.Defaults.Populate(options.JsonSerializerOptions));`, and optionally remove `Microsoft.AspNetCore.Mvc.NewtonsoftJson` from the project file. The transition should go smoothly as we've addressed many compatibility issues between the two JSON libraries. You only need to add `System.Text.Json` specific attributes, such as `JsonPropertyName`, to properties/classes that already have `Newtonsoft.Json` specific attributes. For more information, refer to https://github.com/serenity-is/Serenity/issues/7021.
- Serenity and feature package sources have been updated to leverage C# 12 features like primary constructors and collection initializers.
- The StartSharp dashboard page has been rewritten using jsx-dom. All HTML markup in the `DashboardIndex.cshtml` file has been converted into functional components. This serves as a useful sample for jsx-dom and will simplify the creation/customization of your own dashboards.
- Our sample pages now utilize the `jsx-dom/min` runtime, as full SVG support in jsx-dom is typically unnecessary. We will soon change the default jsx runtime in `tsconfig.json` for new StartSharp/Serene projects to `jsx-dom/min` and remove the explicit pragma statement from our sample pages. This change won't affect your existing projects. However, be aware that our samples will assume this jsx runtime by default. So, if you copy-paste our samples into your application but have a different jsx runtime configured in your `tsconfig.json`, such as preact, remember to add `/** jsxImportSource jsx-dom/min */` to those `.tsx` files. Alternatively, you can change your `jsxImportSource` in `tsconfig.json` to `jsx-dom/min`.
- We are currently developing a new `@serenity-is/base` NPM package that will include core functions and classes from `@serenity-is/corelib`, mostly from the old `Q` namespace. This new base library will be shared between `@serenity-is/corelib` and a new UI library that we plan to develop. The upcoming UI library, likely named `@serenity-is/ui`, is in the planning phase and will probably be based on `jsx-dom`, with no dependencies on `jQuery` or `jQuery UI`. Presently, `@serenity-is/base` is packed into `@serenity-is/corelib`, so no changes are required in this release.

### Bugfixes:
- Addressed an issue with esbuild watch not functioning in the latest versions of tsbuild. Please update `tsbuild` to version 8.0.1 or later in your `package.json` file.

## 8.0.0 (2023-11-16)

### Features:
- This is a major update that supports .NET 8. Before switching to this version, ensure that your Visual Studio 2022 version is `17.8.0` or later. .NET SDK 8 should be installed through the Visual Studio update or manually if you are not using VS. In your project file, change the target to `net8.0`:
  - `<TargetFramework>net8.0</TargetFramework>`
  
  Next, update the following packages to at least the listed versions below:
  ```xml
  <PackageReference Include="Microsoft.AspNetCore.Mvc.NewtonsoftJson" Version="8.0.0" />
  <PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.0" />
  <PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.4" />
  ```

  Next, update Serenity and Pro packages to version 8.0. This should be enough to transition to .NET 8 as there aren't many breaking changes affecting Serenity-based applications.
  
  Note that Serenity 8.0+ will no longer support .NET 6/.NET 7. 
  
  We won't release a .NET 8 version of Serene yet, as most users who download it won't have the required Visual Studio 17.8.0 update, causing issues for newcomers. Serene users should apply the changes noted in these release notes to their apps after updating their Visual Studio.

- Updated `Microsoft.TypeScript.MSBuild` to 5.2.2, now the recommended TypeScript version.
- Updated several NuGet package references to their latest versions, including `Newtonsoft.Json`, `Scriban`, `Mono.Cecil`, `Microsoft.Data.SqlClient`, `Dapper`, `MailKit`, `Nuglify`, `SixLabors.ImageSharp`, etc.
- **`[Breaking Change]`** `serenity.css` is moved to `Serenity.Extensions` as a static asset for easy updates. After updating, delete `wwwroot/Content/serenity/serenity.css` and replace the `serenity.css` reference in your `appsettings.json` with `~/Serenity.Extensions/common-style.css`. Serene users should also replace their `~/Content/site/common-theme.css` reference with `~/Serenity.Extensions/common-theme.css`. These changes will make it easier for Serene users to update their core CSS files.
- Removed the following legacy scripts and CSS files from the Serenity.Assets package: 
  - `Content/aristo/aristo.css`
  - `Content/bootstrap-theme.css` (Bootstrap 3)
  - `Content/bootstrap.css` (Bootstrap 3)
  - `Content/ionicons.css`
  - `Content/page.css`
  - `Content/simple-line-icons.css`
  - `Content/slick-default-theme.css`
  - `Content/slick.columnpicker.css`
  - `Content/slick.headerbuttons.css`
  - `Content/slick.headermenu.css`
  - `Content/slick.pager.css`
  - `Scripts/bootstrap.js` (Bootstrap 3)
  - `Scripts/jquery/jquery.slim.js`
  - `Scripts/jquery.cropzoom.js`
  - `Scripts/jquery.iframe-transport.js`
  - `Scripts/jquery.json.js`
  - `Scripts/jquery.scrollintoview.js`
  - `Scripts/jquery.slimscroll.js`
  - `Scripts/jsrender.js`
  - `Scripts/pace.js`
  - `Scripts/react*.js`
  - `Scripts/rsvp.js`
  - `Scripts/saltarelle/linq.js` 
  - `Scripts/saltarelle/mscorlib.js`

  These files have not been used by Serene/StartSharp for a long time. If you still have references to any of the above files in your `appsettings.bundles.json`, remove those references or install those files via `libman.json` or similar.
- Updated jQuery to 3.7.1
- Removed jQuery 3.5.1 files from Serenity.Assets. If you still have a reference in your `appsettings.bundles.json` like `"~/Serenity.Assets/Scripts/jquery-{version}.js"`, please change it to `"~/Serenity.Assets/jquery/jquery.js"` or install it via libman.
- Removed the full version of jQuery UI from Serenity.Assets, e.g., `jquery-ui-1.12.1.js`, as it does not have the patches we applied to `jquery-ui.js`, which is our trimmed custom version. If you still have a reference in `appsettings.bundles.json` like `"~/Serenity.Assets/Scripts/jquery-ui-{version}.js"`, replace it with `"~/Serenity.Assets/Scripts/jquery-ui.js"` or install it via libman/npm (though you won't have our patches in that case).
- Updated CKEditor to 4.22.1, the last open-source version. Switched to moono-lisa skin, which is flatter than moono.
- Updated Bootstrap to 5.3.2, offering more customization options via CSS variables and supporting a dark theme.  
- Updated esbuild to 0.19.15, which has some breaking changes related to watch mode. Please update tsbuild to `8.0.0` and apply changes in StartSharp repository's `tsbuild.js` to your project.
- TSBuild now has a type definition so that you can see what options it provides. Serenity.Pro.Theme rules are updated to comply with this change. Apply changes in `_Layout.cshtml` and `_LayoutHead.cshtml` so that a `data-bs-theme="dark"` attribute is added to the HTML element when the `cosmos-dark` or another similar dark theme is used.
- Implemented Select2 localization using Q.localText and Q.format functions. Removing the Select2-locales folder from Serenity.Assets, as they were not suitable to use with Serenity localization.
- StartSharp template now comes with a `.gitignore` file with ignore patterns suitable for general Serenity applications.

### Bugfixes:
- Allow `Fields.XYZ` while determining the `IsActive` property and others in addition to `fields.XYZ`, but the `Fields` property is only there for compatibility and is effectively obsolete.
- Background image was not shown in no-layout pages like login in the glassy-light theme.
- Add the hidden class to wizard dialog panes so that validation doesn't run for inactive steps, unlike usual tab forms.
- PuppeteerHtmlToPdf fix affecting Base64 data URIs.


## 6.9.6 (2023-11-09)

### Features:

- The default ignore option for jQuery validation has been changed to include elements matching `[style*="display:none"], [style*="display: none"] *, .hidden *, input[type=hidden], .no-validate`. This change replaces the previous default of `:hidden, .no-validate`. The reason for this update is that the previous default settings required a workaround for validating inputs in hidden tabs. The workaround involved altering the visibility of hidden tabs by setting their display properties to `flex` or `block`, and visibility property to `hidden` to make them considered as `not(:hidden)`. However, this approach occasionally caused issues, especially when dealing with inputs in inactive tabs, as they were not effectively hidden due to the display property manipulation. Simply setting the ignore rule to empty and validating all inputs was not a viable solution, as some fields may intentionally be hidden using methods like `.hide()`, indicating that they should not be subject to validation. This change also necessitated modifications in `serenity.css`, `pro-theme.css`, and `jquery-ui`, so make sure to update your versions of `Serenity.Pro.Theme`, `Serenity.Assets`, and `Serenity.Scripts` accordingly.
- Enhancements have been made to improve the performance of methods utilized by source generators, such as `EnumerateBaseClasses`, `IsOrSubClassOf`, and `IsAssignableFrom`.
- An experimental `IDiskUploadFileSystem` implementation in `Serenity.Pro.Extensions` package has been introduced. This implementation enables the storage of files and directories in a database table named `FileSystem`. Additionally, a `SqlUploadStorage` implementation has been provided, which leverages the aforementioned `IDiskUploadFileSystem` for file uploads.
- A new attribute, `HideLabelAttribute`, has been added. It corresponds to the `LabelWidth("0", JustThis = true)` setting.

### Bug Fixes:
- The `BasedOnRowPropertyNameAnalyzer` has been enhanced to also process properties of the base entity class.
- A bug where switching to an inactive jQueryUI/Bootstrap tab with validation errors occurred has been resolved.

## 6.9.5 (2023-10-28)

### Features:
  - New BasedOnRowPropertyNameAnalyzer and its associated fix that raises an error in development time if a column/form type with `BasedOnRow` and `CheckNames = true` has a property that does not match any property name in the row type. It offers to add ``[IgnoreName]`` attribute as a fix, or if only the case mismatch, renaming to the correct property name (Serenity.Pro.Coder/StartSharp).
  - New RowPropertyAccessorsAnalyzer that can generate getters/setters of style `fields.SomeField[this]` at development time if the property is a simple `{ get; set; }`, `{ get; }` or just empty braces `{}`. It can also convert a simple field member like `SomeProperty;` to a field accessor property. The analyzer also raises errors if the accessors use a mismatching field (Serenity.Pro.Coder/StartSharp). See 6.9.5 release notes for more information about these analyzers and code fixes.

### Bugfixes:
  - Name property determined from existing entity uses "Field" instead of "StringField"
  - Fix typo in chunking authentication cookie handling

## 6.9.4 (2023-10-26)

### Bugfixes:
  - Workaround in HtmlReportCallbackUrlBuilder for ASP.NET chunking cookies when authentication cookie contains too many claims
  - Missing line break while sergen updates module exports under ServerTypes

## 6.9.3 (2023-10-22)

### Bugfixes:
  - Improve the declaration of classic namespace Q in Serenity.Corelib.d.ts via declare import Q = Serenity;

## 6.9.2 (2023-10-22)

### Bugfixes:
  - Issue with new Serenity.CoreLib.d.ts while using classic namespace mode (non modular)
  - Don't generate column properties in classic namespace mode (non modular) as it is causing some issues
  - Try Serenity.LookupEditor first instead of Serenity.Lookup while generating form properties

## 6.9.1 (2023-10-22)

### Features:
  - Sergen now utilizes the `IncludeGlobalUsings` property when specified in `sergen.json` during client type generation.
  - Removed unused `System.Collections` references from generated client type files.
  - Introduced a new `MigrationKeyAttribute` that derives from `MigrationBaseAttribute`, offering a direct option for migrations, eliminating the need for defining a local `MigrationAttribute`.
  - Added the `TargetDBAttribute` and `DefaultDBAttribute`, allowing for simplified usage without the necessity of locally defining a `DefaultDBAttribute` class.
  - Relocated the logic for the `EnsureDatabase` function to the `MigrationUtils` class within Serenity.Extensions, enabling calls from `DataMigrations.cs`.
  - Transferred all feature-specific migrations in StartSharp to their respective projects.

### Bugfixes:
  - Fixed a missing equal sign in the `Service typing Methods` constant.

## 6.9.0 (2023-10-20)

### Features:

- ****`[Breaking Change]`**** Removed the "@serenity-is/corelib/q" and "@serenity-is/corelib/slick" submodules. After updating the corelib NPM package and Serenity packages to 6.9.x, please search and replace them with "@serenity-is/corelib" in all *.ts and *.tsx files within your project. All types from those modules are now available in the `@serenity-is/corelib` module. This change simplifies migration from namespaces in TypeScript, as the previous structure was causing confusion.

- **Sergen Enhancements:**
  - Sergen now reuses existing entities when generating code for a new entity.
  - When generating code for multiple entities simultaneously, Sergen can also reuse information from rows that are going to be generated.
  - Sergen generates `ForeignKey(typeof(ExistingRow))` instead of `ForeignKey("ExistingTable", "SomeId")` for foreign key (FK) properties if the primary key (PK) entity is already available.
  - Sergen generates `Origin(jSome, nameof(SomeRow.SomeField))` instead of `Expression("jSome.[SomeFieldName]")` if the origin entity and property are available.
  - Sergen can automatically determine the name property of the joined table related to an FK property if the PK entity is available and has a `[NameProperty]`.
  - Sergen automatically adds a `LookupEditor` attribute to the generated property if the primary key entity has a `LookupScript` attribute, using `ServiceLookupEditor` if no lookup script attribute is present.
  - Sergen adds a `ServiceLookupPermission` attribute to generated rows, allowing fine-tuning of access to lookup columns via ServiceLookupEditor.
  - `Columns.ts` files generated by Sergen and Pro.Coder now include all properties as `Column<TRow>`, making it easy to access them. For example, you can create `var columns = new SomeColumns(super.getColumns())` in a grid and access a property like `columns.MyProperty` without searching by field name. Columns classes also include a static `Fields` property similar to `XYZRow.Fields`.
  - Added a `MigrationAttributeBase` class that can be reused by feature packages to define their specific `MigrationAttribute` classes while checking for migration version correctness.

- **User Experience Improvements:**
  - The "_What to Generate_" option is now preselected during code generation.
  - Generated import statements are ordered similarly to how Visual Studio Code's "Organize Imports" does.

- **Added Features:**
  - Added a "ConfigureSections" extension that can automatically configure all settings with the "DefaultSectionKey" attribute by discovering them from a type source. This eliminates the need to manually call "Configure" or "ConfigureSection" for each package and Serenity packages to 6.9.x.
  - Optimized calls to `loadScriptAsync` so that calling `getLookupAsync` multiple times successively results in only one service call.

- **StartSharp Updates:**
  - Moved the DataAuditLog migration to its own project. Changed the version key, as if an existing project already has the same migration, FluentMigrator would raise an error.
  - Changed the key for Email Queue Job in appsettings to `"EmailQueueJob"` from `"MailingService,"` which was incorrect.
  - Prefer IMAP/SMTP username instead of the user's email for Email Client.

- **Other Updates:**
  - Updated SleekGrid to version 1.6.2.
  - Added `*`, `?`, `<`, and `>` to the list of invalid characters for file and path names in Linux, especially for use in uploads.

### Bugfixes:
  - Addressed an issue with Brotli compression if .NET 7 Runtime is used, as it no longer allows custom compression levels.

## 6.8.3 (2023-10-09)

Bugfixes:
  - Don't generate ListExcel method if only Row & Services are selected as Columns type is not available
  - Date editor persist / restore results in dates in format yyyy-MM-dd

## 6.8.2 (2023-10-05)

Features:
  - **`[Breaking Change]`** Changed namespace of IRolePermissionService from Serenity.Serenity.Net.Core.Authorization to Serenity.Abstractions
  - Used single line getter/setter style for rows, also for sergen
  - Switching to tags instead of namespace for determining which migrations to run. So, instead of checking for a particular namespace like "Serenity.Demo.Northwind.Migrations", checking if the migration has "NorthwindDB" tag. For "Default" database, we allow migrations with "DefaultDB" tag, in addition to migrations without any tags to avoid skipping legacy migrations or migrations that user added but forgot to add a tag.
  - Removed migration skipping feature when database name does not match what was expected, as it seems to have helped no-one, while causing confusion for users that are not aware of it
  - Updated Microsoft.Data.Sqlite 7.0.11 and Microsoft.Data.SqlClient to 5.1.1
  - Updated Microsoft.TypeScript.MSBuild to 4.9.5, which is the last version before v5
  - Made use of C# global usings feature both in Serene and StartSharp. Please note that while generating code, Sergen can't yet parse global usings. So, if you expect generated code to omit such usings you need to set them manually in sergen.json for now.
  - Switched to file scoped namespaces also in Serene
  - Defined a new TypeSource class implementing ITypeSource, and used that instead of listing assemblies in Startup.cs `(StartSharp)`
  - Moved application specific service implementations into AppServices namespace.
  - Set RootNamespace instead of AssemblyName in project file, so that newly added files don't include ".Web" suffix.
  - Backported account password and some other changes to Serene from StartSharp.

Bugfixes:
  - Fix click event in Card View sample

## 6.8.1 (2023-09-23)

Features:
  - New GenerateFields attribute that can be used instead of RowTemplate, and more stable as it does not need to copy properties and their attributes from the template class to the Row class `(StartSharp)`. This is now the recommended approach, though RowTemplate still work but may be removed in the future. Set `EnableGenerateFields` to true in sergen.json to use this while generating code via sergen (recommended for Pro users). You still need to write getters and setters as usual instead of a simple `{ get; set; }`, until C# introduces partial properties.
  - Changed the base class for UploadResponse to ServiceResponse.
  - Added more tests for corelib including formatters.
  - New `IRolePermissionService` interface abstraction allowing checking a role's permission separately.
  - New experimental code generator for Flutter apps (StartSharp/Enterprise) for transforming form / columns etc. from C# to Dart/Flutter code similar to what sergen does for TypeScript/C#.
  - New OpenId Scope to Permission mapping so that external apps connecting via a user account can be limited to a subset of permissions (StartSharp). You will need to register `IRolePermissionService` implementation and `OpenIdScopePermissionWrapper` for it to work. See `Startup.cs` and `RolePermissionService.cs` in latest StartSharp code.

Bugfixes:
  - Some Editor/formatter type parameters could not be passed from the RowTemplate to the source generated Row class, as the row fields source generator could not access the code generated by client types source generator, causing Roslyn to error. Workaround was to internally call the client types source generator, but the recommended way is to use new GenerateFields.

## 6.8.0 (2023-08-14)

Features:
  - New Serenity.Pro.OpenIdClient Package which is exclusively available to Business/Enterprise customers, this update introduces the Serenity.Pro.OpenIdClient package, enabling seamless external login via Google, Microsoft, GitHub, and more. The implementation includes robust account link and unlink actions. `(StartSharp)`
  - **Warning!** To thwart potential XSS attacks via email links, a critical security measure has been implemented. The `LoginPage.tsx` now incorporates a defensive check to only accept return URLs that begin with a forward slash (`/`). Our gratitude to Tasfaout Abderrahim for bringing this to our attention. Please apply the check in `LoginPage.tsx` that looks like `if (returnUrl && /^\//.test(returnUrl)) {`.
  - **`[Breaking Change]`** The `SettingStorage` module has undergone modifications to enhance performance and security. The `getItem` and `setItem` methods may now return Promises, eliminating synchronous XHR calls. Consequently, similar adjustments extend to the `DataGrid`'s `restoreSettings`, `persistSettings`, and `getPersistedSettings` methods. Users with custom code using these methods must ensure compatibility with the Promise return type (e.g., utilize async operations). Furthermore, a new `restoreSettingsFrom` method takes precedence over `restoreSettings` in mixins, derived classes, etc. It's important to note that `restoreSettingsFrom` does not handle null settings parameters. **`[Breaking Change]`**
  - Added .mpp (MS Project), .vsd, .vsdx (Visio) to default extension whitelist
  - Set SameSite.Lax for CSRF-TOKEN cookie
  - Enhanced `initFullHeightGridPage` with flex layout and responsive handling instead of trying to calculate grid height from other divs in page
  - Adapted pro theme css for new `initFullHeightGridPage` version
  - Added a setHeight option for cases where initFullHeightGridPage is used with another container type like in split master detail sample
  - Implemented reCAPTCHA for signup form `(StartSharp)`
  - Restored reCAPTCHA functionality with a special property processor. Need to `services.ConfigureSection<RecaptchaSettings>(Configuration);` in Startup.cs, and set Recaptcha:SiteKey and Recaptcha:SecretKey in appsettings.json by generating them from https://www.google.com/recaptcha/admin (use v2 ones, not sure newer ones work). Unless they are set, Recaptcha property will be made invisible in the form. See Signup form in StartSharp for an example.
  - Enhanced form item handling by clearing category when lacking Category but having Tab attribute
  - Also get base row type properties with BasedOnRow attribute
  - Introduced IUserClaimCreator interface and its default implementation DefaultUserClaimCreator. You should register it in Startup.cs and use it instead of the static implementation in UserRetrieveService class. If you have any custom claims you can inherit DefaultUserClaimCreator and override AddClaims method then register your class instead of DefaultUserClaimCreator.
  Replace UserRetrieveService.CreatePrincipal with IUserClaimCreator dependency whereever it is used.
  - Applied patch for jquery ui touch events so that grid columns can be dragged in mobile
  - New HandleControllerExceptionAttribute that can redirect errors to ValidationError page.
  - Generated a more compact and merge friendly version of Texts.ts file.
  - Introduced IHasPassword interface that allows to check if user has a password set via IUserDefinition
  - Added a LocalTextPrefix property to FormScript and ColumnsScript attribute that may be used to override the local text prefix calculated from form key and namespace
  - Updated jsx-dom to 8.0.7
  - If reportParams is passed as null, JSON serialize the report instance in HTML report callback url builder
  - Set Content-Disposition header for preview as well so save name for the generated pdf will not be Render.pdf always
  - Moved password related page / service codes like Forgot/Reset/Change password to Serenity.Extensions to make them easier to update and reduce amount of code in StartSharp/Serene.
  - Introduced an IElevationHandler and RequiresElevation attribute so that user will have to enter password again before performing some critical actions.
  - Added hidden-xs css rule for compatibility with bs4

Bugfixes:
  - Fix sergen code generation errors for form, columns etc. in non-modular projects
  - Fix several invalid source links in Demo
  - Don't auto fill demo username/pass for activation page
  - Possible issue with Postgress in SqlErrorStore while inserting exceptions
  - Fix styling for grouping bar when group columns are longer than the width via x-scrolling

## 6.7.6 (2023-05-24)

Features:
  - Abstracted script and css minifier dependencies, so that another minifier can be used instead of NUglify for bundling
  - Added experimental `esbuild` based minifier `(StartSharp)`
  - Updated preact in Serenity.Assets to 10.14.1
  - Added `/** jsxImportSource jsx-dom */` to files where jsx-dom are used, so that it can understood which library is used without relying on the default in tsconfig.json
  - Rewrote EmailClient with ES modules and preact, removed Serenity.Pro.UI package as it was only ever used by EmailClient. `(StartSharp)`
  - Improve look and feel of e-mail client `(StartSharp)`
  - Added a mock IMAP server implementation to EmailClient for demo purposes
  - **`[Breaking Change]`** `Serenity.Pro.UI` package is now obsoleted as it was only used by the old email client. It will not get any new versions and you should remove its reference from your project file.
  - Changed default jsxImportSource back to `preact` in tsconfig.json `(StartSharp)`
  - `<environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Development" />` line which was in `web.config` for development purposes is removed. If you deploy to IIS and did not remove that line in your production site after publishing, you should.

Bugfixes:
  - Resolve issue with enum / select editors causing display text to be saved instead of value in dropdown editors `(StartSharp)`

## 6.7.5 (2023-05-21)

Features:
  - added ExtensionBlacklistInclude, ExtensionBlacklistExclude, ExtensionWhitelistInclude and ExtensionWhitelistExclude that allows including, excluding extensions from defaults without having to write all the extensions again
  - update bootstrap icons to 1.10.5

Bugfixes:
  - email client now working due to preact not included in appsettings.bundles.json `(StartSharp)`
  - enable System.Drawing.Common 6.0 in linux by adding runtimeconfig.template.json file `(StartSharp)`
  - fix failing source gen tests caused by line endings `(StartSharp)`
  
## 6.7.2 (2023-05-13)

Features:
  - Generated API documentation for `@serenity-is/corelib` and `@serenity-is/sleekgrid` packages via `TypeDoc` and made them available at `https://serenity.is/docs` under `Api Reference (TypeScript)` section.
  - Added `jsx-dom` integration which is a library similar to React. but which directly creates HTML elements instead of using a `VDOM`. It offers better compatibility with existing Serenity `Widget` structure that use `jQuery` and contains code that modify `DOM` directly. It will mostly be used in `renderContents()` method instead of the `getTemplate()` so that events can be bound at widget creation, and references to elements can be acquired. There is currently no dependency to `jsx-dom` in `@serenity-is/corelib` itself but it exposes a `jsxDomWidget` method than can be used to create a `jsx-dom` compatible functional component from a widget type, like `var StringEditor_ = jsxDomWidget(StringEditor); <StringEditor_ readonly={true} maxlength={50} />`.
  - Switched to JSX automatic runtime (https://www.typescriptlang.org/tsconfig#jsxImportSource) in `StartSharp` by setting `"jsx": "react-jsx"` and `"jsxImportSource": "jsx-dom"` in `tsconfig.json`. If you need to use `React` or `Preact` etc. in one your `.tsx` files, you may switch the runtime by adding a pragma comment on top of the file like `/** @jsxImportSource react */`.
  - `**/*Page.tsx` files are also considered as ESM entry points for `esbuild` in `tsbuild` and `sergen`.
  - Adapted membership page codes in StartSharp to use `jsx-dom` by renaming them first from `XYZPage.ts` to `XYZPage.tsx` and using `renderContents` instead of `getTemplate`.
  - Added `useIdPrefix()` and `Widget.useIdPrefix()` methods to make it easier to generate unique ID's based on the Widget's idPrefix in `jsx-dom` based code similar to `~_` prefix in classic templates.
  - `findElementWithId` method can optionally accept a context parameter that controls where the search is performed. This is useful for detached elements (e.g. not added to document yet) created by `jsx-dom` to locate cascading etc. elements.
  - Replaced `toastr` with an embedded version rewritten in TypeScript adapted from `https://raw.githubusercontent.com/JPeer264/toastr2/master/src/Toastr.ts`. This will make it possible to show notifications which failed when `toastr.js` is not loaded in the page. You may remove `toastr.js` from `appsettings.bundles.json` but leave `toastr.css` for now as it is still required.
  - CardViewMixin can accept `jsx-dom` based card templates in addition to React ones.
  - Moved `corelib` and `tsbuild` to root `packages/` folder from `src/Serenity.Scripts/` in Serenity repository.
  - Improved `globalExternals` optimization in `tsbuild` so that it generates less number of chunks on build.
  - Increased test coverage for `@serenity-is/corelib/q` methods and refactored test files (`.spec.ts`) to be next to the files (`.ts`) they have tests for.
  - Added async versions of Authorization script methods like `Q.Authorization.userDefinitionAsync` which should be preffered over sync counterparts where possible.
  - IExceptionLogger interface is obsolete! Please log exceptions directly via .NET's ILogger interface and its LogError method. StartSharp users should replace `app.UseExceptional()` line with `app.UseExceptionLogger()` in Startup.cs, which also enables implicit logging to `StackExchange.Exceptional` via `InformationalException` type.
  - Added InformationalException type that will be used to log via ILogger interface to Exceptional
  - Updated `@serenity-is/sleekgrid` to 1.6.0.
  - **`[Breaking Change]`** parseDate and parseISODateTime functions returns an Invalid Date instance (e.g. its `.valueOf()` method returns `NaN`) instead of "false" or "null" as they used to be, which was a wrong decision at time. It was causing invalid dates to be considered equal to empty dates sometimes. Javascript `Date` constructor also returns a Date instance with `NaN` value for invalid dates.
  - Round calculated width when scale / delta parameters are specified in DataGrid, which is true for latest theme.
  - Normalized all the line endings to LF in Serenity, common-features and StartSharp repository via a `.gitattributes` file.
  - Made DefaultRowTypeRegistry use typeSource on every call to the AllRowTypes (#6776) so that if the ITypeSource implementation returns a dynamic list of types, it can pick them.
  - Introduced new `UI Elements` sample pages containing Bootstrap components, Icon classes etc. `(StartSharp)`
  - Removed RuntimeCompilation as Hot Reload does the same thing `(StartSharp)`

Bugfixes:
  - Referencing generic types in generated form type short name assignments which caused problems with `Widget<any>` in TypeScript
  - AutoValidateAntiforgeryIgnoreBearerFilter were logging another message when antiforgery validation failed
  - Fix lazy android check in jquery.maskedinput.js introduced a few versions back.

## 6.7.1 (2023-04-07)

Features:
  - Added IUploadFileResponder interface, whose default implementation will only send text/plain, application/pdf, and ones whose mime type starting with image/ as inline, and rest as application/octet-stream with content-disposition attachment for security purposes. Please use it in your FilePage.cs Read method. See the latest `FilePage.cs` in StartSharp/Serene repository.
  - **`[Breaking Change]`** moving ExtensionBlacklist to UploadSettings from IUploadOptions. Added an ExtensionWhitelist that only allows usual extensions by default (.3gp;.7z;.ai;.avi;.bmp;.csv;.doc;.docx;.eps;.jpg;.jpeg;.json;.gif;.gz;.ico;.mpg;.mpeg;.mp3;.mp4;.mkv;.pdf;.png;.ppt;.pptx;.psd;.rar;.rtf;.svg;.tif;.tiff;.txt;.wav;.webm;.webp;.xls;.xlsx;.xml;.xps;.zip;). You'll need to override them in appsettings.json if you need more extensions (UploadSettings:ExtensionWhitelist, UploadSettings:ExtensionBlacklist)
  - updated missing translations in Serene/StartSharp/Serenity
  - use ASP.NET Core's FileExtensionContentTypeProvider instead of manually listing known mime types

## 6.7.0 (2023-04-06)

Features:

  - ****Warning!**** This release focuses on addressing potential security issues that were recently discovered by Fabian Densborn from SEC Consult. Serene/StartSharp users must either create a new project from the 6.7.0+ template or manually apply the relevant changes from this commit to their existing applications after updating Serenity packages to 6.7.0+: https://github.com/serenity-is/serene/commit/6dce8162f4382badd429a9f0f1470acb64e8c4fd
    
  - Serenity.is would like to express gratitude to Fabian Densborn for his discovery, analysis, and coordination, as well as the SEC Consult Vulnerability Lab (https://www.sec-consult.com) for responsibly reporting the identified issues and collaborating with us as we addressed them.
    
  - Added the option to use ClamAV (https://www.clamav.net/) as an antivirus scanner for temporary uploads. To enable it, add `services.ConfigureSection<Serenity.Extensions.ClamAVSettings>(Configuration);` and `services.AddSingleton<IUploadAVScanner, Serenity.Extensions.ClamAVUploadScanner>();` to your Startup.cs after upgrading to Serenity/Serenity.Extensions 6.7.0+. Consult ClamAV documentation on how to install it on your platform. This feature will be enabled by default once these changes have been made in Startup.cs. If you want to disable it for development purposes, set ClamAV:Enabled to false in your appsettings.Development.json (not recommended for production!).
    
  - Added extensionless and `.htm`/`.html` to the upload file extensions blacklist by default. An attacker can include malicious scripts in such an HTML file, send an administrative user a link to that file via email, and if the administrative user is already logged in to the site while clicking the link, the script can call services, perform actions, etc. on behalf of the user as the cookies are sent by the browser.
    
  - Ensured that the Forgot password page does not reveal information to identify whether a user with the entered email exists.
    
  - Ensured that reset password tokens can only be used once. They already expired in 3 hours, but if an attacker could see the link within that time frame (e.g., by eavesdropping), they could use it to change the password again.

## 6.6.6 (2023-04-04)

Bugfixes:
  - Fix empty text can't be used as DisplayName in forms/columns

## 6.6.5 (2023-04-04)

Bugfixes:
  - **`[Breaking Change]`** removing legacy AsyncLookupEditor from corelib as it is getting mixed up with AsyncLookupEditorAttribute in server side which is just a LookupEditor with Async = true
  
## 6.6.4 (2023-04-03)

Features:
  - Added default support for new languages including Arabic, Bangla, Czech, French, Hindi, Indonesian, Japanese, Korean, Dutch, Romanian, Swedish, Chinese Traditional. Used machine translation for all these languages in addition to the existing languages. As these may not always be the best possible translations, any pull requests with improved texts are welcome.
  - Used embedded resources under texts/resources folder for JSON translation files instead of static web assets under wwwroot/texts as these files are not meant to be directly accessed via web. This will also reduce number of published files, and simplify deployment.
  - Introduced quick filter option in Translations page to show/hide user translated and text that has any translation in the target language. There are also buttons to export translated values / original values to make translation easier. Each translation resource folder also contains a template JSON file in English language that can be used as source.
  - ITypeSource may return its assemblies if available via the new IGetAssemblies interface
  - Introduce LanguageIdKeyPair to use as dictionary key in LocalTextRegistry.
  - Added IGetAllTexts interface to LocalTextRegistry to return all registered texts
  - Added ILanguageFallbacks interface for LocalTextRegistry to get/set language fallbacks.
  - Added initialization support to LocalText directly, so that readonly localtexts can be used in nested text classes without having analyzer warnings
  - PropertyItemsLocalTextRegistration to register texts defined implicitly via DisplayName, Tab, Category, Hint, Placeholder attributes in Forms/Columns so that they can be seen/translated in Translations page
  - Also handle Hint, Placeholder, Category, Tab attributes for Row/Entity local text registration
  - NavigationLocalTextRegistration to register texts for navigation items from attributes
  - Moved NavigationItemAttribute down to Serenity.Core from Serenity.Web
  - Site local text package has a default regex that will be included in addition to anything you define in appsettings.bundles.json, so you may remove LocalTextPackages section from your appsettings.json unless you included some additional texts there.

Bugfixes:
  - Fix fields without displayname attributes are shown with their local text keys in grids/forms. Use the propertyname as implicit display name.
  - Crash in Arabic culture in the constructor of ScriptCulture

## 6.6.3 (2023-03-24)

Features:
  - Adjust brotli and gzip compression levels based on content length, caching duration and bundle type etc. as brotli with its default level is too slow, sometimes 100 times slower than gzip default level
  - Add getRowDefinition() method to return row type and read idProperty etc. from there.

Bugfixes:
  - Inline editing required fields that does not have NotNull/Required flag.

## 6.6.2 (2023-03-23)

Bugfixes:
  - Generated navigation link referred to the old page class name ending with Controller

## 6.6.1 (2023-03-18)

Bugfixes:
  - Criteria parser with params failed in some cases, especially with IN statements like 'a in @values'. Note that you can't use parens there, e.g. `a in (@values)` is invalid unlike SQL.

## 6.6.0 (2023-03-14)

Features:
  - Added a new Criteria builder functionality, so that criteria can be defined easier without having to remember the JSON array structure. E.g. instead of `[['SomeField'], Criteria.Operator.in, [[1, 2, 3]]]` you can write `Criteria('SomeField').in([1, 2, 3])`, or instead of `[Criteria.Operator.isNull, ['SomeField']]` you can write `Criteria('SomeField').isNull()` with intellisense support.
  - Introduced ability to parse criteria from an SQL like expression at client side, e.g. `request.Criteria = Criteria.and(request.Criteria, Criteria.parse('A = 'b' and B > 5 and (C in (1, 2, 3) or D like 'x%'))`
  - It can also parse parameterized expressions similar to Dapper: `Criteria.parse("xyz = @p1 and abc like @p2", { p1: 5, p2: 'test%' })`
  - There is also a third recommended option using tagged string literals: `` Criteria.parse`xyz = ${15} and abc like ${'b%'}` ``
  - Converted all basic samples pages (Serenity.Demo.BasicSamples) to fully modular code, e.g. ESM
  - Converted all advanced samples pages (Serenity.Demo.AdvancedSamples) to fully modular code, e.g. ESM `(StartSharp)`
  - Renamed all page controllers so that they use Page.cs suffix instead of Controller suffix. This made it more consistent, as their file names ended with Page.cs while the type they contained was named Controller.
  - Renamed all service endpoint controllers so that they use Endpoint.cs suffix
  - ServiceEndpoint now derives from ControllerBase instead of Controller as services should not normally return views. If you returned view from a service endpoint for any reason, you'll need to move it to a controller. The endpoints generated by Sergen will have Endpoint suffix instead of Controller suffix. Similarly, pages will have Page suffix.  
  - Updated sergen and Pro.Coder so that it works fine with controllers without Controller suffix, e.g. XYZPage or XYZEndpoint
  - There is now a new source generator in `Serenity.Pro.Coder` that generates full paths to ESM modules, just like MVC does it for view paths. This will allow referencing ESM module scripts with compile time check and intelli-sense `(StartSharp)`.
  - Sergen now has a `ForeignFieldSelection` option that defaults to `All`. When it is `NameOnly` it will bring only the name field from foreign rows instead of all fields, so that your entities will be smaller and will cause less issues when a joined row's fields are renamed / removed. When it is `None` no foreign view fields will be generated.
  - There is now a `IncludeForeignFields` option in sergen.json that allows including more fields by their field names, when the `ForeignFieldSelection` is `None` or `Name`.
  - Added `FileScopedNamespaces` option to sergen.json so that the generated code can use the file scoped namespace feature in C# 10.
  - Added `DeclareJoinConstants` option to sergen.json. When true generated rows will have join constants like `const string jCustomer = nameof(Customer)` and it will be used in expressions. This will make it easier to rename a join alias and use Origin etc. attributes.
  - Added `EnableRowTemplates` option to sergen.json so that the generated row will define the `RowTemplate` class supported by `Serenity.Pro.Coder` `(StartSharp)`
  - Added `OmitDefaultSchema` option to sergen.json. When it is true, the default schema name, e.g. "dbo" for SQL Server won't be emitted in generated field expressions in Row.cs
  - Added `SaveGeneratedTables` option to sergen.json. When set to false, sergen will not save connection settings and table options like Identifier, Module etc. back to sergen.json after code generation.
  - Added TSBuild -> EntryPoints setting to sergen.json. This is a list of globs for ESM entry points which defaults to `**/*Page.ts` and `**/*ScriptInit.ts`. This globs if specified are read by `tsbuild` script and the new ESM module paths source generator.
  - There is a new JSON schema for sergen.json that will be picked up by Visual Studio automatically so that you can have intellisense support and descriptions while editing sergen.json files.
  - It is now possible to use "Extends" function in sergen.json similar to tsconfig.json to extend the settings from a base file.
  - A special type of extends value can be used to extend from defaults. E.g. when you set `"Extends": "defaults@6.6.0"` your settings will be extended from the defaults we set for 6.6.0 version of Sergen. The values for `defaults6.6.0` are: `DeclareJoinConstants: true, EndOfLine: LF, FileScopedNamespaces: true, ForeignFieldSelection: NameOnly, OmitDefaultSchema: true, SaveGeneratedTables: false, Restore: Exclude Files/Typings, ServerTypings -> LocalTexts: true`. This way you can use modern recommended defaults, while keeping backward compatibility for those who still want to use old defaults.
  - Simplified scriban templates in Sergen
  - **`[Breaking Change]`** CKEditorBasePath default is changed to "~/Serenity.Assets/Scripts/ckeditor/" instead of the obsolete one "~/Scripts/ckeditor/".
- Added more hooks and injection points in `GridEditController` so that it is now possible to dynamically set editor parameters, use a different editor type in a column for different rows, implement cascaded dropdowns etc. `(StartSharp)`
  - Moved GridPage related extensions and types to Serenity.Extensions so that it can be used in feature projects
  - It is now recommended to remove GridPageModel, GridPage.cshtml from your Serene/StartSharp and use the one in Serenity.Extensions instead
  - The module for GridPage, e.g. SomePage.ts should have a default export function pageInit() instead of a jQuery ready script so that it can be passed options from controller/model and tested properly. jQuery ready is also generally not necessary for ES modules.
  - There is now a PanelPage extension that creates a PanelDiv instead of a GridDiv.
  - GridPage and PanelPage uses ModelPage extension which accepts a ModulePageModel that includes additional parameters like HtmlMarkup, Options to pass to the default export in page module, Layout, PageId etc. parameters.
  - Obsoleted EnvironmentSettings.IsPublicDemo and used preprocessor directives instead
  - Introduced s-site-logo-img class which allows specifiying the site logo for a img via its content property, so that the site logo for all pages like login, sidebar etc. can be changed from one location by overriding css rule like .s-site-logo-img { content: url(.../mylogo.png); } etc.
  - There is also a s-form-title-logo class that adjusts the look of logo in the forms, e.g. rounded with sidebar band background. It might be used to adjust logo styling by overriding its css rules.
  - Removed all the .cshtml files for membership related pages like Login, Signup, Reset etc. and moved their script code into Page.ts files so that it can be checked at compile time.
  - **`[Breaking Change]`** There is no longer a globally defined Q.Lookup interface while using ES modules. If you referenced it in some modular code, you should `import { Lookup } from "serenity-is/corelib/q"` and replace `Q.Lookup` with `Lookup` in your project.
  - IEditDialog.load methods fail argument made optional. It was normally optional but did not seem so in the typing.
  - Arrange method in dialog type made public instead of protected
  - Sergen prefers importing from `@serenity-is/corelib/q` instead of `@serenity-is/corelib` for types that are exported from both modules, like ServiceResponse, PropertyItem etc. This helps with `dts-generator` tool as it has problems with types with same names that are imported from multiple modules.
  - Introduced DefaultSectionKeyAttribute so that settings can be configured by their default section key via the new ConfigureSection extension

Bugfixes:
  - Fixed styling for EntityGridDialog panel
  - jQuery UI datepicker was showing under the dialog when using it in a inline editing grid inside a dialog `(StartSharp)`
  - Remove loading class from parent element as select2 will only remove the class from the input
  - fix error when trying to to use Q.formatDate outside of 10:00 and 19:59 with format 'dd.mm.yyyy hh:mm' (#6723)

## 6.5.4 (2023-02-28)

Features:
  - Implemented OpenID Authorization Code Flow, e.g. logging via the web site instead of with user/password in mobile / desktop apps ``(StartSharp)``.
  - **`[Breaking Change]`** The services required for OpenIdAuthorizationControllerBase is changed, so you should get latest version of `OpenIdAuthorizationController.cs` from the StartSharp repository if you have that in your project.

Bugfixes:
  - Handle the case where select2 is destroyed before the request is completed or timer fires
  - Fixed icon for FileDownloadFormatter
  - SelectEditor with async source should not update/clear its items when cascade value change etc.
  - Resolve issue with group by in oracle due to alias starting with underscore

## 6.5.3 (2023-02-27)

Features:
  - Add virtual methods / protected members to DefaultSqlConnections / DefaultConnectionStrings for better extensibility
  - Changed update button text in german to Aktualisieren instead of Bearbeitung which was reported to be incorrect (#6689)
  
Bugfixes:
  - Fix field size not set to the correct variable, possibly affecting precision/scale
  - BasePermissionAttribute should accept null values for compatibility

## 6.5.2 (2023-02-21)

Features:
  - Added SqlNow, SqlUtcNow, SqlDateTimeOffset, Case, CaseSwitch, Coalesce dynamic expression attributes
  - Added ability to use another type of expression attribute as arguments to the dynamic expression attributes, e.g. use SqlNow etc. in a DateDiff attribute like: `[DateDiff(DateParts.Day, typeof(SqlNowAttribute), "t0.ReleaseDate")]`
  - Add ability to get/set the groupitemmetadataprovider in RemoteView
  - DraggableGroupingMixin will try to create and auto register GroupItemMetadataProvider itself if it is not already registered and pass it to the view
  - Make SqlConnections available to use for the derived classes of RowLookupScripts
  - Added `ISiteAbsoluteUrl` abstraction to access web site's internal (local) and external (public) url's and `EnviromentSettings:InternalSiteUrl` setting that takes precedence over request's base URI, and SiteExternalUrl for report callbacks.  
  - Refactored HTML to PDF conversion process via new `IHtmlToPdfConverter`, `IHtmlReportPdfRenderer`, `IHtmlReportRenderUrlBuilder` abstractions
  - **`[Breaking Change]`** To register the default implementations for `IHtmlToPdfConverter`, `IHtmlReportPdfRenderer` etc. services.AddReporting should be called instead of services.AddExcelExporter in Startup.cs.
  - Added new `HtmlToPdfOptions` class that implements `IHtmlToPdfOptions` like the converters.
  - Completely redesigned and refactored reporting system and abstractions via new `IReportFactory`, `IReportRenderer`, `IReportRetrieveHandler`, `IReportTreeFactory` interfaces. Make sure to call `services.AddReporting()` in Startup.cs.
  - Added Puppeteer based HTML to PDF converter implementation with Chrome and experimental Firefox option `(StartSharp)`. `services.AddPuppeteerHtmlToPdf` should be called before `services.AddReporting` line to use Puppeteer instead of WKHtmlToPdf. To make some old reports keep using old WKHtmlToPdf based converter, add `[UseWKHtmlToPdf(true)]` attribute on the report classes.
  - `TemplateHelper.RenderViewToString` can access the current request/action context if available
  - Added experimental `HtmlReportFileUrlBuilder` that can use a temporary folder, instead of a callback for HTML report rendering.

Bugfixes:
  - MultipleUploadEditor should selecting multiple files in upload dialog
  - Fix Criteria.Exist for sqlite as it does not like double parenthesis (#6687)
  - Group summaries was not displayed in some cases like when the group item metadata provider is not passed to the view options
  - File watcher should not try to create watcher if the directory does not exist
  - Add section.content for proper layout when opening panels if the navigation is hidden e.g. hideNav=1

## 6.5.1 (2023-02-03)

Features:
 - Introduced new BaseExpressionAttribute which allows inherited expression attributes to perform translation based on target dialect. Also the dialect itself can customize the translation by implementing the new ISqlExpressionTranslator interface.
 - New ConcatAttribute, DatePartAttribute and DateDiffAttribute dynamic SQL expression types that generates expression based on server type.
 - Optional Format for expression attribute types
 - Allow overriding field size via attribute
 - Allow specifying join alias in OriginAttribute indirectly via propery name of a property with LeftJoinAttribute
 - New useCssVars option in SleekGrid that allows using inline css variables at grid container element to position columns instead of a CSS stylesheet appended to the head
 - Don't swallow exception in source generators so that issues can be identified if any `(StartSharp)`
 
Bugfixes:
  - LookupScriptRegistration should not try to run for abstract classes.
  - Fix typo in Sum aggregator name and aggregator display texts

## 6.5.0 (2023-01-26)

Features:
 - **`[Breaking Change]`** Updating Microsoft.Data.SqlClient to 5.1.0. As Encrypt=true became the default in 4.x+ (https://techcommunity.microsoft.com/t5/sql-server-blog/released-general-availability-of-microsoft-data-sqlclient-4-0/ba-p/2983346) it may cause connections to fail. Please see the linked doc, and either install a trusted certificate in your SQL server (recommended!), or set TrustServerCertificate=true or Encrypt=false in your connection strings both in development and deployment/production!
 - Updated packages including Newtonsoft.json to 13.0.2, SixLabors.ImageSharp to 2.1.3, NUglify to 1.20.4, Microsoft.Build to 17.3.2, Spectre.Console to 0.46.0, Npgsql to 7.01, MySqlConnector to 2.2.5, Microsoft.Data.Sqlite to 7.0.2, FirebirdSql.Data.FirebirdClient to 9.11, Scriban to 5.5.2
 - Html encode jquery validator messages in showLabel function
 - Updated SleekGrid to 1.5.3 which includes useCssVars option to use css vars for cell positioning instead of dynamic stylesheet (up to 50 columns). This should reduce amount of css reflows on page load etc. It is not enabled by default yet.

## 6.4.10 (2023-01-24)

Bugfixes:
 - propertygrid should htmlEncode field captions
 - draggabble grouping mixin should htmlEncode grouped column captions

## 6.4.9 (2023-01-21)

Bugfixes:
  - **Warning!** If htmlEncode is not called when using local texts in ascript method (e.g. `<div>{ text("SomeKey") }</div>` instead of `<div>{htmlEncode(text("SomeKey")) }</div>`, like getTemplate etc. an attackermay use the translations screen to inject javascript. This affected partsof our demo as everyone can login as admin and can use the translationscreen. The translations are reset every 30 mins in our demo so it was nota big issue. But you are strongly recommended to check your own code inaddition to standard screens like login, reset password etc. See ourlatest commits in Serene/StartSharp/Common Features, etc. repositories forfixes we applied. Even if no one other than the admin can entertranslations screen, it is still a good idea to mitigate the risk as youshould not trust translators, even the admin himself. We can't change the"text" or "tryGetText" functions to htmlEncode by default, as they may beused by others in contexts other than HTML like functions which expectsraw text like notify messages, dialog titles, column titles etc., whichwould result in double HTML encoding. Prefer localText() function insteadof obsolete text() function for better discoverability (but it does notencode as well)

  - **`[Breaking Change]`** htmlEncode now also encodes single and double quotes just like attrEncode. This is done to avoid cases where a developer might use htmlEncode in an attribute instead of attrEncode by mistake.

## 6.4.8 (2023-01-18)

Bugfixes:
  - set Widget.idPrefix to the value passed from property grid options, https://github.com/serenity-is/common-features/issues/21, so that the invalid examples in common-features like OtherFormInTab etc. work
  
## 6.4.7 (2023-01-16)

Bugfixes:
  - fix sleekgrid groupitemmetadataprovider group header totals

## 6.4.6 (2023-01-16)

Bugfixes:
  - fix sleekgrid groupitemmetadataprovider colspan calculation

## 6.4.4 (2023-01-08)

Bugfixes:
  - Removed corelib/coverage folder from Serenity.Scripts nuget contents
  

## 6.4.3 (2023-01-07)

> Release Notes: https://serenity.is/docs/release-notes/6.4.3

Features:
  - Flutter based mobile applications for Android/iOS `(StartSharp Enterprise)`
  - OpenIddict integration and JWT authentication options `(StartSharp)`
  - Introduced AutoValidateAntiforgeryIgnoreBearerFilter attribute which can be used in place of AutoValidateAntiforgeryAttribute and will skip anti-forgery validation when the request contains an `Authentication: Bearer` header and there are no cookie headers.
  - New Glassy Light theme `(StartSharp)`
  - Improved handling for type registries like `EnumTypeRegistry`, `EditorTypeRegistry`, and `FormatterTypeRegistry`. Improved error messages that are displayed when a type cannot be found. They can now discover types that are registered after the type initialization process is completed. Use a common base type registry for all type registries to make it consistent.
  - **`[Breaking Change]`** `Q.getTypeName` is renamed to `Q.getTypeShortName`. Got rid of some static type properties like __typeName$, __metadata$ etc. that is no longer necessary. Used defineproperty for system related props like __typeName etc. to make them non enumerable
  - **`[Breaking Change]`** toastr notification functions like `Q.notifyError` and `Q.notifyInfo` will escape HTML by default. This is to avoid possible script injection attacks. Pass `escapeHtml: false` if you need the old behavior.
  - Removed jQuery ScrollIntoView hard-coded dependency and use it only if it is available. You may remove `jquery.scrollintoview.js` from `appsettings.bundles.json`.
  - Removed `jquery.iframe-transport.js` dependency. You may remove it from your `appsettings.bundles.json` as it is only used by jquery.upload plugin for very old browsers like IE9 etc.
  - Serenity.Pro.UI.js dependency as it is only used for legacy `EmailClient` sample.
  - Removed TemplateBundle from `appsettings.bundles.json`, introduced ColumnAndFormBundle that combines `ColumnsBundle` and `FormBundle`.
  - Rewrote Dashboard with ES Modules `(StartSharp)`
  - GroupItemMetadataProvider SleekGrid plugin is ported to ES Modules. Slick.Data.GroupItemMetadataProvider is now obsolete, use GroupItemMetadataProvider from "@serenity-is/sleekgrid" or Slick.GroupItemMetadataProvider
  - Q.alert, Q.confirm, etc. methods are suffixed with `Dialog` to avoid mixing up with the browser methods, e.g. `Q.alertDialog`, `Q.confirmDialog`. Old methods are still available but obsolete.
  - TsBuild trigger argument is now obsolete. You should change `prepare` script in `package.json` file with `dotnet build -target:RestoreTypings`
  - **`[Breaking Change]`** Some row properties are only available via IRow or IEditableRow interface. Introduced IEditableRow and instead of making desktop UI editing (e.g. WPF INotifyPropertyChanged, IsAnyFieldChanged, IsEditing) etc. related properties / methods of row instances only accessible via this interface. They were confusing and polluting the row interface in web applications where they were almost never used. Cast the row instance like ((IEditableRow)row).BeginEdit() etc to access them if ever used. Similary, "IdField, IgnoreConstraints, NameField, Table, TrackAssignments, TrackWithChecks, GetDictionaryData, GetDictionaryDataKeys, SetDictionaryData" properties and fields can only be accessed via IRow interface. This is done to avoid polluting intellisense list as they are very rarely used, and as a preparation to fix swagger / apiexplorer integration
  - **`[Breaking Change]`** rename MethodChainingExtensions to ChainableExtensions and only support single action overload as that is the only one which is used anywhere so far
  - **`[Breaking Change]`** make rarely used ValidateEnum and ValidateDateRange methods non-extension methods so they don't come up in the intellisense for rows. Not a breaking change unless you used them as extension methods.
  - improved binding convention of IDbConnection and IUnitOfWork and ServiceRequest types in ServiceEndpoints so that api browsers like swagger etc. can analyse it better and not crash
  - **`[Breaking Change]`** `Slick.Event` is renamed to `Slick.EventEmitter`, and `Slick.EventHandler` is renamed to `Slick.EventSubscriber`, `Slick.Handler` is renamed to `Slick.EventListener` to avoid mixing up with browser types with same names.
  - Introduced TransactionSettingsAttribute and added a TransactionSettings which can be configured via options pattern. ServiceEndpoint will check the attribute or transaction settings like IsolationLevel. The attribute, settings and the UnitOfWork object has a DeferStart option that defers starting the transaction until the Connection is opened. This flag might have undesired side effects, so use it at your own risk if you know what you are doing.
  - Added the TransactionlessUnitOfWork which is an implementation of IUnitOfWork that does not actually start any transaction. Should only be used in edge cases to call methods that expect a IUnitOfWork instance, but you are sure that the whole operation will be readonly, or you don't really need a transaction. If using with save handlers etc it might be risky. Make sure there are no service behaviors that run behind the scenes which may write or require a transaction.
  - Improved `TSTypeLister` performance by making it not dive into huge directories like `node_modules` if not needed.
  
Bugfixes:
  - use Object.prototype.hasOwnProperty and fix field proxy detected as class
  - fix handling for dynamic property, https://github.com/serenity-is/Serenity/issues/6624
