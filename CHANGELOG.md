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
  - Move DataAuditLogAttribute to Serenity.Net.Data
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
## 3.9.14 (2019-12-23)

Features:
  - Int64Field should serialize big values into strings just like JsonSafeInt64Converter to avoid loss of precision due to javascript JSON deserialization
  - call ValidatePermissions in DeleteRequestHandler just before ValidateRequest, so that Request can be accessible there, closes #4785

Bugfixes:
  - latest changes for permissions in Dialog.scriban and Grid.scriban had syntax errors.

## 3.9.13 (2019-12-10)
Features:
  - generate TS code for types with NestedPermissionKeys attribute
  - added ScriptSkip attribute to optionally skip code generation for some types and service methods
  - converted quick filter bar into a separate component, so that it can be used without a data grid
  - added ability for toolbar buttons to control their own visibility and disabled states through updateInterface handler if a visible/disabled option as a function is set
  - added readOnly property and reated interface methods to datagrid, updateInterface calls toolbar.updateInterface for new toolbutton disabled/visible methods
  - added getInsertPermission method to EntityGrid, transfer readOnly property to opened dialogs if any
  - implemented readOnly property for entity dialog
  - added useViewMode method which when true opens entity dialog in view mode initially and shows an edit button
  - added getInsertPermission, getUpdatePermission, getDeletePermission methods which controls button visibility to EntityDialog
  - added EditorUtils.setContainerReadOnly method which is used by entity dialog to make all form editors readonly in readonly/view modes
  - server typings generator generates row permissions in Row.ts
  - use permissions in grid/dialog/row typings generated by sergen
  
Bugfixes:
  - jquery ui tabsshow event is now tabsactivate

## 3.9.12 (2019-11-12)

Features:
  - add support for NOT "!" and Parenthesis "()" to LogicOperatorPermissionService
  - activated thumbnail generation in .NET Core using System.Drawing.Common, make sure to run "sudo apt install libc6-dev " and "sudo apt install libgdiplus" if using Linux
  - update TypeScript to 3.6.4, this is now the recommended version 
  
Bugfixes:
  - use occurred instead of occured in error messages
  - fileSizeDisplay inconsistency
  - file upload editor, maxsize only works for images
  - wait cursor not removed after columnpicker quick search
  - datagrid.itemAt should return an object of TItem type, not any.

## 3.9.11 (2019-08-26)

Features:
  - removing mono.cecil from tooling, please take latest CodeGenerationHelpers.ttinclude from Serene/StartSharp that doesn't require Cecil reference
  - add option to show checkboxes only in some rows for GridRowSelectionMixin

Bugfixes:
  - fix bug with clearing slick event handlers
  - fix property grid item orders might get messed up, if tabs and categories are in mixed order, e.g. same category name used in a tab, but separate locations

## 3.9.10 (2019-07-30)

Features:
  - introduce EntityGridDialog which is a mixture of an EntityGrid and EntityDialog, than allows editing 
    records in a side panel on grid `(StartSharp)`
  - complete missing XML docs for Serenity.Data
  - change error message to suggest IgnoreName instead of NotMapped for skipping property name check with CheckNames
  - update packages in node based sergen cli
  - added a toggle method to Serenity.TabsExtensions
  
Bugfixes:
  - if user changed CustomTemplates via the config UI, it was not used properly in sergen until restart
  - handle issue with template registration when a project directory has a Modules parent directory in full path
  - **Important!** fluentmigrator output might reveal connection string in exception message when migrations fail. 
    please set CustomErrors=RemoteOnly in your web.config, and ASPNETCORE_ENVIRONMENT to PRODUCTION for .NET Core. 
	Also apply changes in SiteInitialization.DataMigrations.cs and DataMigrations.cs.

## 3.9.9 (2019-05-24)

Features:
  - SqlSettings.DefaultCommandTimeout global setting to set command timeouts, and connection.SetCommandTimeout extension method to set timeout for a connection (only works with WrappedConnection)
  - made set of criteria operator constants available in script side as Serenity.Criteria.Operator
  - complete missing XML documentation for Serenity.Core

## 3.9.8 (2019-04-16)

Bugfixes:
  - sergen nuget package missing System.Threading.Tasks.Extensions.dll used by scriban 2.0

## 3.9.7 (2019-04-15)

Features:
  - update to asp.net core 2.2, make sure you install 2.2 sdk for your Visual Studio version (SDK 2.2.106 for VS 2017, SDK 2.2.203 for VS 2019 as of writing), updated many packages including Newtonsoft.Json, FakeItEasy, Microsoft.AspNet.Mvc
  - add better error handling when a property has `[LeftJoin]` or `[InnerJoin]` attribute but no `[ForeignKey]` attribute.

## 3.9.6 (2019-04-05)

Features:
  - added annotation type support, which allows separating undesired dependencies, like UI related attributes from row properties into a annotation class.
  - added AutoColumnWidthMixin which auto sizes grid columns based on content width `(StartSharp)`
  - improve responsive layout algorithm for data grids in mobile devices
  - recommended method to ignore checking a property name when form/columns class has a BasedOnRow(CheckNames = true) is now `[IgnoreName]`, not `[NotMapped]` attribute, though old one still works for compability.

## 3.9.5 (2019-03-27)

Features:
  - handle toastr invalid path casing
  
## 3.9.4 (2019-03-27)

Bugfixes:
  - fix syntax error in DotnetCorePathFinder
  
## 3.9.3 (2019-03-27)

Features:
  - better handling for nuget locations in linux / mac

## 3.9.2 (2019-03-27)

Features:
  - skip packages without id for dotnet sergen restore, e.g. Microsoft.AspNetCore.App

## 3.9.1 (2019-03-27)

Features:
  - check runtimeconfig.dev.json for nuget folders (attempting to fix linux / mac transform issue)

## 3.9.0 (2019-03-15)

Features:
  - added basic attributes to support plugin architecture (work in progress...)
  - **`[Breaking Change]`** Serenity.Web NuGet package no longer contains / has reference to Serenity script, css and image files, so you need to manually install Serenity.Scripts NuGet package in your project after updating. 
  Also update Serenity.Web.Assets package. If using AspNetCore add reference to Serenity.Web.Assets and Serenity.Web.Scripts packages in CSPROJ file next to Serenity.Web reference.

Bugfixes:
  - StackExchange.Exceptional delete, protect etc. actions wasn't working in ASP.NET Core properly due to AntiForgery system `(StartSharp)`

## 3.8.7 (2019-02-27)

Features:
  - update bootstrap dependency to 3.4.1

## 3.8.6 (2019-02-09)

Features:
  - modern flat design for grids, dialogs and toolbars `(StartSharp)`
  - when there are items without a tab, instead of showing an empty tab caption, create those property items before the tab (thanks @dfaruque)
  - ability to register data scripts using classes like lookup scripts
  - row field value name indexed get access should also look field by property name and also in dictionary data if available
  - made target optional for PropertyGrid.save and it returns the new/passed target object. added value property to PropertyGrid.
  
## 3.8.5 (2019-01-03)

Bugfixes:
  - ensure not-mapped fields that are populated by MasterDetailRelation or LinkingSetRelation are only loaded if explicitly included (e.g. column is visible) for ListRequestHandler and loaded in default Details mode for RetrieveRequestHandler

## 3.8.4 (2019-01-02)

Bugfixes:
  - fix inplace add script error in lookup editor

## 3.8.3 (2018-12-29)

Features:
  - added idle (session) timeout feature and related sample `(StartSharp)`
  - implemented impersonate as (login as) functionality in users page `(StartSharp)`
  - backported two factor authentication sample to .NET core `(StartSharp)`
  - added from address setting for mailing in .NET core version `(StartSharp)`
  - added selectedItem getter in SelectEditor (thanks @dfaruque)
  - changed protection level of clearItems, addItem and addOption in lookup editor to public (thanks @dfaruque)
  - ability to specify location of button separator (thanks @Jin)
  - added GridRadioSelectionMixin (thanks @Jin)
  - moved most of lookup editor code like cascading, filtering etc. into select2editor base class so that they can be reused in custom editors
  - enum editor supports multiple option
  
Bugfixes:
  - fix ui look of datetimeeditor (thanks @adam feng)
  - fix fonts folder casing issue in linux
  - fixed OracleSchemaProvider based on pull request by @kilroyFR
  - hotkeyContext of tool button definition is ignored (thanks @hannesb)

## 3.8.2 (2018-12-04)

Features:
  - skip enum members that has Ignore attribute in ServerTypingsGenerator
  - trigger change handler when now button is clicked in date/time editor
  - trigger change event of TimeEditor hour input when minute changes

Bugfixes:
  - set minutes section readonly when time editor is readonly
  - hide delete buttons on images when MultipleImageUploadEditor is readonly
  - fix typo on unsubscribe viewOnDataChanged of SlickGrid

## 3.8.1 (2018-11-03)

Bugfixes:
  - resolve package downgrade issue with System.Data.SqlClient

## 3.8.0 (2018-11-03)

Features:
  - updated to .net core 2.1, updated many packages
  - when a field can't be loaded from database using getFromReader, show the name of the field and row type so that developer can understand which field has an invalid type
  - added local text generation ability (optional) to server typings so that local texts can be accessed with completion and compile time checking. currently only processes row field texts and nested local texts.
  - serenity guide is now hosted at https://serenity.is/docs
  - added readonly option to CheckTreeEditor and subclasses like CheckLookupEditor
  - improve UniversalAssemblyResolver so that more packages can be located / loaded by sergen
  - divide InBrace function of CodeWriter to StartBrace and EndBrace functions so blocks can be manually opened and closed
  
Bugfixes:
  - if allowNegatives is true and minValue / maxValue is not specified, minValue is set to 999999999999.99 instead of -999999999999.99 in decimal editor
  - only switch to main tab if current tab is being made disabled
  - add / to handler paths to make them only available at root, see https://github.com/serenity-is/Serenity/issues/4017
  
## 3.7.7 (2018-09-15)

Features:
  - enabled StackExchange.Exceptional (e.g. exception logging) in ASP.NET Core version `(StartSharp)`
  - added docs for migrating Serene project to StartSharp inplace / to a new project `(StartSharp)`
  - added sample daily background task that generates PDF from a report and sends it by e-mail `(StartSharp)`
  - better handling when header filters mixin is working client side with formatted column text instead of underlying value `(StartSharp)`
  - add cke_dialog class to allow header filter search input work under dialogs `(StartSharp)`
  - use timeout give cascaded dropdowns a chance to update / clear themselves, also fixes quick filter clear problem
  - updated Scriban to 1.2.3
  - implemented IReadOnly in RadioButtonEditor (thanks @dfaruque)
  - made ApplyDefaultValues return row for chaining, added unassignedOnly option to the method
  - modified css classes used for OneThirdWidthAttribute, QuarterWidthAttribute, ThreeQuarterWidthAttribute and TwoThirdWidthAttribute so that they stay around 250-350 pixels in worst case. might be a breaking change for your existing layouts.
  - added JustThis option to form layout attributes like HalfWidth etc, so that you won't have to cancel form widths to just set width for one item
  - added JustThis option to LabelWidthAttribute just like FormWidthAttribute
  - added VariantField which should correspond to sql_variant (sergen doesn't auto use it yet)
  - stop auto numeric from raising errors when an out of range value set manually, only fix value on tab out if some action performed to change value
  - added AllowNegatives property to IntegerEditor and DecimalEditor, which alongside AllowNegativesByDefault static property controls ability to edit negative values in editors when MinValue is not explicitly set
  - added widthset to PropertyItem which determines if an explicit width value is assigned to property in columns.cs etc.
  - split part that loads persisted settings when settings = null in DataGrid.restoreSettings into its own function, getPersistedSettings

Bugfixes:
  - if there was an exception, shouldn't commit transaction but dispose (rollback) it in .net core service endpoints
  - resolve issue that fields with a space inside name can't be removed from grouping `(StartSharp)`
  - check value of `[ReadOnly(false)]` attribute before setting item as read only
  - ResetLabelWidth attribute shouldn't require a value
  - gray out now button in DateTimeEditor when readonly
  - handle issue with extra whitespaces in generated Row.cs after scriban update
  - fix typo for tabbingDirections in slick.grid.js (thanks @globtech1)
  - delete .orig file in DeleteFileAndRelated as well (thanks @globtech1)
  
## 3.7.6 (2018-07-10)

Features:
  - updated Serenity.Web.Assets package

## 3.7.5 (2018-07-10)

Features:
  - added drag & drop grouping mixin and related sample `(StartSharp)`
  - added customizable summaries mixin and related sample `(StartSharp)`
  - group headers and summary footers uses column formatter if available
  - ignore when one or more of requested distinct fields are not allowed and instead of raising exception return null to gracefully handle issue
  
Bugfixes:
  - handle ckeditor warning about upload plugin

## 3.7.4 (2018-07-05)

Features:
  - introduced AllowHideAttribute which when set on a property to false, doesn't let that column to be hidden in column picker dialog
  - added collapseAll() and expandAll() to TreeGridMixin.ts, thanks @dfaruque
  - Bengali translation, thanks @dfaruque
  - remove quick filter if user doesn't have permission to column
  - advanced filter should only show columns that user has read permission to (thanks @kilroyFR)
  - renamed ColumnFiltersMixin to HeaderFiltersMixin `(StartSharp)`
  - better positioning for header filter popup `(StartSharp)`
  - added ability to check for roles like permissions with Role:RoleKey if role has a key set (a new field added to Roles table) `(StartSharp)`
  - fix SqlErrorStore error logging in non sql server type database servers
  
Bugfixes:
  - fix typos in `[DefaultHandler]` implementation for Save and Delete handlers
  - avoid duplicate key exception while adding implict permissions in UserPermissionService
  - fix some ClientTypes conditional files left behind even if related features are not selected

## 3.7.3 (2018-06-28)

Features:
  - updated Serenity.Web.Assets package


## 3.7.2 (2018-06-28)

Features:
  - added ability to select distinct fields to ListRequest and its handler by using DistinctFields which works similar to Sort property
  - excel style column filtering grid mixin and related sample `(StartSharp)`
  - call init async on widget if the widget is async as well

Bugfixes:
  - removed BW operator from StringFiltering (thanks @marcobisio)
  - added missing return statement that causes switch on advanced filter contains to fall back to startsWith operator (thanks @edwardch)
  - replaced default keyPrefix with config.KeyPrefix in RedisDistributedCache (thanks @MungoWang)
  - handle null reference exception gracefully when row type doesn't have a nested fields type
  - .net core memory cache raises an exception if timespan is less than zero (#3514)

## 3.7.1 (2018-05-16)

Bugfixes:
  - fix sergen generating empty files due to a change in template engine (scriban) we use

## 3.7.0 (2018-05-14)

Features:
  - support for UNION including INTERSECT, EXCEPT and/or ALL (where DB support is available) to SqlQuery using .Union method
  - added ability to determine Name field by using new `[NameProperty]` attribute instead of INameRow interface if name field is of non-string type
  - introduced `[DefaultHandler]` attribute, which when placed on a service handler, e.g. ListRequestHandler, SaveHandler etc, allows behaviors like MasterDetailRelationBehavior, LinkingSetRelationBehavior, UpdatableExtensionBehavior etc to use your custom handlers (MySaveHandler etc.) instead of generic ones for that row type. This allows logic in your custom handlers to be reused for related records, e.g. detail rows for MasterDetailRelation without having to write a behavior.  
  - added `[DataAuditLog]` attribute which allows simple audit logging for change operations to any entity (StartSharp)
  - set filename field after upload so that audit logging can take the final value
  - data audit log viewer sample (StartSharp)
  - split master details grid sample (StartSharp)
  - add MigrationAttribute to enforce migration versioning
  - switched to SourceLink from GitLink
  - removed .Net45.csproj versions for some Serenity libraries like Core, Data, Entity, Services...
  - updated all packages including AspNetCore, AspNet.Mvc, Newtonsoft.Json, Nuglify, Selenium, CKEditor etc.
  - use double right arrow character for organization dropdown (StartSharp)
  - added documentation about upgrading from Serene to StartSharp, inplace or by migration (StartSharp)
  - handle *.ts *.cs includes for ServerTypings / ClientTypes to get less merge conflicts on .csproj files
  - added debounce function
  - got rid of .Net45.csproj files for Serenity.Core, Serenity.Data, Serenity.Data.Entity, Serenity.Services and Serenity.Caching packages, switched to SourceLink
  
Bugfixes:
  - resolve intellisense issue on project creation (StartSharp)
  - presencefield bug on updatable extension (thanks @marcobisio)
  - fix missing element in Recaptcha Widget Editor (thanks @edson)
  - fix typo in closequote for sql2000dialect (thanks @hannesb)
  - FastReflection should try to skip visibility checks
  
## 3.6.0 (2018-03-31)

Features:
  - changes to widget for React integration (currently StartSharp only), you'll need to add @types/react to your package.json even if you'll not use React at all
  - include react scripts in Serenity.Web.Assets
  - full featured e-mail client (IMAP) sample written with React and MailKit (StartSharp)
  - rewrote CardViewMixin with React (StartSharp)
  - introduced Serenity.Pro.Scripts nuget package which makes it possible to update StartSharp scripts. It contains UI, App and EmailClient scripts now.
  - allow viewing / editing with inplace button even if the lookup editor itself is readonly
  - use inline source maps for better script debugging experience with corelib
  - add helpers required for spread operator
  - grouping helper in corelib
  - ability to do replacements in ScriptBundles.json, e.g. replace development version of some script with prod using web.config setting Replacements
  - improve typing of widget class, make editor discovery more tolerant even if type doesn't have a registerEditor attribute
  - don't try to minimize files ending in min.js when script bundling is on
  - deprecating Saltaralle, no longer Serenity.Script package

Bugfixes:
  - fix some slickgrid compability issues in chrome related to jquery changes
  - fix return type declarations of some script data functions
## 3.5.5 (2018-02-20)

Bugfixes:
  - null reference exception during servertypings generation of types with generic parameters

## 3.5.4 (2018-02-19)

Features:
  - add reference to Serenity.Web.Tooling package so that it auto updates as well
  - **`[Breaking Change]`** need to replace "externalType.Interfaces.Add(intf.FullName)" with "externalType.Interfaces.Add(intf.InterfaceType.FullName)" in CodeGenerationHelpers.ttinclude
  - MasterKeyField option to use another field as master key in master row, thanks @hannesb
  
## 3.5.3 (2018-02-19)

Bugfixes:
  - resolve possible problem with loadScriptAsync
  - `[InstrinicProperty]` in legacy script form import generator is causing issues

## 3.5.2 (2018-02-18)

Bugfixes:
  - fix Row.scriban sergen template

## 3.5.1 (2018-02-18)

Bugfixes:
  - add missing Mono.Cecil.dll

## 3.5.0 (2018-02-18)

Features:
  - rewrote servertypings generator using mono.cecil to reduce assembly loading errors on .net core sergen transform
  - added CheckLookupEditor which is similar to lookup editor with multiple option but uses checkboxes instead
  - updated AspNetCore, jQuery, Redis, Couchbase, Nuglify, Dapper and some other packages
  - added module attribute that will be used to auto determine local text prefix and lookup script keys
  - `[LookupScript]` attribute can now be used without specifying a lookup key. In that case lookup key will be auto generated from row module / name and / or class namespace.
  - no need to set localTextPrefix as it will now be determined by RowIdentifier, e.g. module identifier dot row type name without row suffix
  - added IIsDeletedRow which works similar to IIsActiveDeletedRow but as a Boolean fields
  - added IgnoreDeleted option to UniqueConstraintAttribute and UniqueAttribute to skip soft deleted records on check
  - added CheckNames option to BasedOnRowAttribute so that property name matching with row can be validated optionally to ensure valid / exact case matching property names. can turn check off on a property by adding `[NotMapped]`
  - ability to skip minification for specific files using NoMinimize option in ScriptBundling settings
  - added ResolvePath function to ContentHashCache resolve virtual paths to absolute or cdn urls when enabled
  - seek to page 1 after a change in filters / sort orders / quick filter in slick grid
  - more descriptive error message when lookup script is not found
  - try to give more info when lookup script fails to load due to permissions or another exception
  - error handler to show runtime errors in browser console as toast on localhost / 127.0.0.1
  - make sure there is only one type with a lookup key, raise an error otherwise to warn user
  - overflow hidden to prevent double scrollbars in iframedialog
  - removing responsive() attribute from dialog template as its should be default now in all except legacy apps
  - validate cache on commit even if row doesn't have TwoLevelCachedAttribute, so that attribute is not required anymore
  - added ForceCascadeDelete option to LinkingSetRelationAttribute and MasterDetailRelationAttribute that forces deletion of sub records even if master row uses soft delete
  - removing old ResponsiveDialog and MultiColumnResponsiveDialog samples as we now have a different way to handle them. removed  responsiveDialog decorators as it is no longer needed. `(Serene)`
  - added CheckNames = true to all BasedOnRow attributes so that property / field name matching can be validated `(Serene)`
  - increase upload request limits to 50mb `(Serene)`
  - make sure colorbox scale properly for very large images `(Serene)`
  - finalize agenda and decision tabs in meeting module `(StartSharp)`
  - include ckeditor in pages where required to improve first time startup time `(StartSharp)`
  - use tablename and module attributes for all rows, move external lookups to Lookups namespace instead of Scripts, remove explicit lookup keys from all lookups as it can be auto generated now. `(StartSharp)`

  
Bugfixes:
  - possible race condition in css / script bundling at first startup
  - fix quick search input property/method reference, closes #3248
  - check-box vertical alignment issue
  - fix northwind employee symbols `(Serene)`
  - fix missing navigation icons `(Serene)`

## 3.4.4 (2018-01-30)

Bugfixes:
  - fix issue with formatter displayFormat option not working after TS port, and a few other options in other editor / formatters

## 3.4.3 (2018-01-28)

Features:
  - implemented sorting and text search functionality in DataExplorer sample `(StartSharp)`

Bugfixes:
  - fix email and image upload editor value properties are readonly in TS typings
  - added new EmailAddressEditor with one input for e-mail editing (unlike EmailEditor)
  - fix MinuteFormatter returns empty text

## 3.4.2 (2018-01-26)

Bugfixes:
  - fix possible script registration issue with enum types

## 3.4.1 (2018-01-26)

Bugfixes:
  - use categories default was changed during TypeScript port
  - persist sort order wasn't working

## 3.4.0 (2018-01-24)

Features:
  - 4 new themes: Azure, Azure Light, Cosmos, Cosmos Light `(StartSharp)`
  - ported all code left in Saltaralle (12K+ lines) to TypeScript
  - Optimized ServerTypings generation so that it produces a MyProject.Web.js file that is up to %50 less in minified size.
  - **`[Breaking Change]`** used const enum feature in TypeScript 2.4 to generate field names in ServerTypings Row.ts. Replace "var fld = SomeRow.Fields" with "import fld = SomeRow.Fields" and move that line to just under "namespace" declaration.
  - don't validate CSRF token when user is not logged in or cookie token is null (e.g. json service client)
  - changed icon for alert dialog (thanks @Jin)
  - added descending property in DateYearEditorAttribute (thanks @dfaruque)
  
Bugfixes:
  - fix enum field cast error
  - CompareValue() of field types based on list/array returns wrong value (#3156, thanks @hannesb)

## 3.3.15 (2018-01-04)

Bugfixes:
  - missing closing double quote in site.less for new module generated by sergen

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
  
## 2.7.2 (2016-12-25)

Bugfixes:
  - release assets package containing slickgrid mac fix

## 2.7.1 (2016-12-25)

Features:
  - remove direct checks on ISqlDialect type and use new ISqlDialect.ServerType and ISqlDialect.NeedsBoolWorkaround properties, which also resolves problems with custom Oracle dialect
  - OracleDialect like search is now case insensitive by default

Bugfixes:
  - fix slickgrid mousewheel bug occuring on mac / chrome (thanks @mkoval-ua)

## 2.7.0 (2016-12-22)

Features:
  - ConnectionKeyAttribute can now accept a type, e.g. a row type, to get its value from that source type, instead of explicitly listing the connection key string.
  - PageAuthorizeAttribute can now accept a source type, e.g. a row type that gets ReadPermission attribute from, instead of hardcoding the permission
  - ServiceAuthorizeAttribute and its new subclasses, AuthorizeCreateAttribute and AuthorizeUpdateAttribute and AuthorizeDeleteAttribute can now get a source type parameter, e.g. a row type where they'll read relevant permissions from source type, instead of hard coding the permission. Their permission determination algorithms closely matches relevant handlers.
  - RowLookupScript lookups inheritance chain for ReadPermissionAttribute 
  - Request handler looks in inheritance chain for permission attributes, and save/delete handlers should also look for readpermissionattribute
  - added HeaderCssClass attribute
  
Bugfixes:
  - resolved latest Chrome bug with hasOwnProperty method when key is a negative integer, affecting Saltaralle dictionaries when hash is negative (https://bugs.chromium.org/p/chromium/issues/detail?id=673008)
  - fix quick search height in dialog

## 2.6.10 (2016-12-12)

Bugfixes:
  - fix date required even if not
  
## 2.6.9 (2016-12-12)

Bugfixes:
  - fix code generator views

## 2.6.8 (2016-12-11)

Features:
  - when no category is specified for any of items, don't show default category name, even if useCategories is true, but create category div for CSS compability
  - added inplaceaddpermission to lookup editor
  - disable inplace add functionality if select2 editor is readonly
  - call dialog arrange method on resize and active tab change
  - add "d": date only, "g": dd/MM/yyyy HH:mm (culture specific dmy order), "G": dd/MM/yyyy HH:mm:ss (culture specific dmy order), "s": yyyy-MM-ddTHH:mm:ss, "u": yyyy-MM-ddTHH:mm:ss.fffZ format specifiers for Q.formatDate function
  - Q.formatDate can now accept an iso date/time string or normal date string in addition to a Date object
  - Q.parseDate can also parse iso date/time values
  - feature selection wizard while creating a new application with Serene template. you can now optionally exclude Northwind, Meeting, Organization, Samples etc. `(Serene)`
  - added attendee editor to meeting UI `(Serene)`

## 2.6.7 (2016-12-09)

Bugfixes:
  - fix datetimeoffset conversion bug affecting json deserialization

## 2.6.6 (2016-12-09)

Bugfixes:
  - fix typescript services transform error when node is null somehow
  - update typescript services used for t4 transforms to 2.0.6
  - possible problem with asyncPostProcessCleanup when a column is removed

## 2.6.5 (2016-12-08)

Features:
  - code generator single & multiple tabs merged into a new datagrid based interface
  - added DateTimeOffset field type 
  - added ByteArray field type for small binary column types like timestamp, varbinary(8) etc.
  - add minbuffer (number of buffered rows on top and bottom), and renderAllCells (render all cells in row, including non visible ones, helps with inline editing tab order) options to slick.grid
  - give a more informational error message about "query affected N rows while 1 expected"
  - added IReadOnly class to TypeScript defs
  - add optional AdminLTE style for login and signup pages (thanks @DucThanhNguyen) `(Serene)`
  - default timeout of 90 secs for running migrations `(Serene)`
  - sample for dynamic navigation items (thanks @DucThanhNguyen) `(Serene)`
  
Bugfixes:
  - delete button in multiple image upload editor gets lost for long file names
  - fix android keyboard hiding when search on menu is clicked
  - oracle sequences should now work (Oracle users, please report)
  - datetimeeditor fails on empty string

## 2.6.4 (2016-11-26)

Features:
  - updated font-awesome to 4.7.0
  - updated simple line icons to 2.4.0
  - LinkingSetRelation and MasterDetailRelation shouldn't delete detail records when master is IIsActiveDeletedRow
  - local text key fallback registry (thanks @DucThanhNguyen)

Bugfixes:
  - filter panel incorrect paren handling when a paren comes right after a line without paren
  - missing closing paren at end for filter panel display text

## 2.6.3 (2016-11-26)

Features:
  - make capture log work without integer ID
  - handle double slashes as single slash in navigation items
  - auto create intermediate menus on secondary or more levels for navigation items
  - auto created navigation items now has min display order of their children
  - add FullPath property to NavigationItem which can be used to get localized captions for navigation items. see change in LeftNavigation.cshtml
  - it's now possible to custom handle filters by overriding ApplyFieldEqualityFilter in ListRequestHandler
  - equality filter multiple values with IN filtering are now supported natively by ListRequestHandler
  - ability to custom handle and ignore an equality filter in ListRequestHandler by behaviors
  - LinkingSetRelationBehavior handles equality filters by default. set HandleEqualityFilter to false for manual handling.
  - don't allow sorting for fields with NotMapped or Sortable(false) attribute
  - set sortable false for fields with NotMapped attribute
  - don't allow filtering on NotMapped fields
  - fix some mistakes in Vietnamese translation (thanks @DucThanhNguyen)
  - add ReportHelper.execute method and related sample in OrderGrid for invoice printing `(Serene)`
  - move forward 18 years 6 months in Northwind order dates with a migration (sql server only) `(Serene)`
  - add quick filter to Representatives in customer grid which is handled by LinkingSetRelation `(Serene)`

## 2.6.2 (2016-11-19)

Features:
  - added tree grid drag & drop sample (thanks @dallemann for sponsoring this sample) `(Serene)`
  - added entity dialog as panel sample `(Serene)`
  - added vietnamese language and translation (thanks @DucThanhNguyen)
  - added ability to inject dynamic navigation items through INavigationItemSource interface (thanks @DucThanhNguyen)
  - include field level permission keys in permission dialog

Bugfixes:
  - entity dialog load fails when onSuccess parameter is null
 
## 2.6.1 (2016-11-15)

Features:
  - ability to use original ID column with GridEditorBase instead of "__id"

Bugfixes:
  - if an item is just readonly in property grid, it should still be serialized
  - fix tree grid mixin doesn't work when toggle column has no formatter

## 2.6.0 (2016-11-12)

Features:
  - implemented field level permissions, just add one or more of ReadPermission, ModifyPermission, InsertPermission, UpdatePermission attributes to properties in a row.
  - added LogicOperatorPermissionService that allows using & (and), | (or) operators in permission checks, e.g. ReadPermission("A&B|C")
  - ListField, RowField, RowListField types has NotMapped flag by default so no need to add `[NotMapped]` attribute explicitly
  - ListField also supports value comparison just like RowListField
  - added setSelectedKeys method to GridRowSelectionMixin (thanks @estrusco)
  - added other form in tab with one toolbar sample (thanks @estrusco) `(Serene)`
  - added a report page for Northwind, more report samples are on the way `(Serene)`

## 2.5.9 (2016-11-07)

Features:
  - expand category when related link is clicked

Bugfixes:
  - fields in collapsed categories could be focused

## 2.5.8 (2016-11-06)

Features:
  - TreeGridMixin for tree view like grid functionality
  - added Collapsible attribute, for collapsible categories in forms (thanks @marcobisio)
  - added selectKeys method to GridRowSelectionMixin (so no need to access $items directly)
  - added Tree Grid sample `(Serene)`
  - filter sample for showing orders containing a specific products in details `(Serene)`
  
Bugfixes:
  - don't show pdf button in new order dialog `(Serene)`
  - use connection.InsertAndGetID with row, instead of SqlInsert to avoid errors in postgresql and similar

## 2.5.7 (2016-11-04)

Features:
  - added innerjoin attribute (thanks @marcobisio)
  - added tabextesnsions.selectTab helper (thanks @estrusco)

Bugfixes:
  - fix includecolumns parameter in data grid doesn't get cleared when columns are made hidden
  - fix category filtering in report repository
  
## 2.5.6 (2016-10-30)

Bugfixes:
  - avoid multiple populate on quick filter initialization. it was due to usage of change event and async initialization

## 2.5.5 (2016-10-29)

Features:
  - integrated StackExchange.Exceptional `(Serene)`
  - added validation to another form in tab sample `(Serene)`
  - added inline action buttons sample `(Serene)`
  
Bugfixes:
  - column picker dialog changing height while dragging
  - fix dashboard link doesn't get active if url doesn't end with '/'
  
## 2.5.4 (2016-10-28)

Bugfixes:
  - fix paging LIMIT OFFSET statement for MySql dialect

## 2.5.3 (2016-10-24)

Bugfixes:
  - resolve problem with IE11 and slickgrid layout, that is caused by height() returning non-integer values in jQuery v3, which leads to stack overflows

## 2.5.2 (2016-10-23)

Bugfixes:
  - fix jquery.event.drag.min.js causing problem when bundling is enabled
  
## 2.5.1 (2016-10-22)

Features:
  - upgraded to TypeScript 2.0
  - fix look of ui dialog buttons after jQuery UI update
  - added date time quick filtering option
  - better handling when date entered in a quick filter is invalid

## 2.5.0 (2016-10-21)

Features:
  - updated bootstrap from 3.3.6 to 3.3.7
  - updated CouchbaseNetClient from 1.3.10 to 2.3.8
  - updated FakeItEasy, from 1.25.3.0 to 2.3.1
  - updated jQuery from 2.2.3 to 3.1.1
  - updated jQuery.TypeScript.DefinitelyTyped from 3.0.5 to 3.1.2
  - updated jQuery UI from 1.11.4 to 1.12.1
  - updated jqueryui.TypeScript.DefinitelyTyped from 1.4.6 to 1.5.1
  - updated jquery.validation from 1.14.0 to 1.15.1
  - updated jquery.validation.TypeScript.DefinitelyTyped from 0.4.3 to 0.4.5
  - updated MsieJavascriptEngine from 1.7.0 to 2.0.0
  - updated Newtonsoft.Json from 8.0.3 to 9.0.1
  - updated qunit.TypeScript.DefinitelyTyped from 0.3.3 to 0.3.5
  - updated RazorGenerator.Templating from 2.3.11 to 2.4.7
  - updated Selenium.WebDriver from 2.48.2 to 3.0.0
  - updated sortablejs.TypeScript.DefinitelyTyped from 0.1.7 to 0.1.8
  - updated StackExchange.Redis from 1.0.488 to 1.1.608
  - updated System.Data.SqlLocalDb from 1.14.0 to 1.15.0
  - updated toastr.TypeScript.DefinitelyTyped from 0.3.0 to 0.3.1
  - removed jquery.event.drag 2.2 package and embedded version 2.3 (from 6pac/SlickGrid) in Serene.Web.Assets as it is compatible with jQuery v3
  - added data-field attribute to input fields in product grid (@wldkrd1) `(Serene)`
  - added showing another form in tab sample `(Serene)`
  - removed VS 15 (which is vNext, not 2015) from supported list as it was preventing upload in VSGallery `(Serene)`
  
## 2.4.13 (2016-10-14)

Features:
  - added FilterField / FilterValue to UpdatableExtensionAttribute for extension tables that might have a constant value in addition to key matching, e.g. an address extension table with CustomerID / AddressType field
  - added FilterField/FilterValue option to MasterDetailRelation and LinkingSetRelation, which works just like UpdatableRelation
  - make sure bracket differences don't affect field matching process in UpdatableExtensionBehavior, by removing brackets before expression comparison
  - made ClientSide flag/attribute obsolete as it was causing confusion, use NotMapped instead
  
Bugfixes:
  - fix look of static text block sample in IE11 `(Serene)`

## 2.4.12 (2016-10-13)

Features:
  - added updatable extension attribute and related behavior for 1 to 1 extension tables
  - added Static Text Block sample `(Serene)`
  - handle IndexCompare for RowListField type
  - added RowField type
 
Bugfixes:
  - fix reference to Q.ErrorHandling.showServiceError in Saltaralle code

## 2.4.11 (2016-10-06)

Features:
  - increase checkbox column width for row selection mixin by 1 due to ie11 text overflow issue
  - make source and disabled optional in Select2Item interface
  - rename addItem method in Select2Editor that takes two strings to addOption to avoid confusion
  - added enabling row selection sample `(Serene)`
  - added user image to user table and navigation (thanks @edson)

## 2.4.10 (2016-09-16)

Features:
  - added quick filter for boolean fields
  - made dateRangeQuickFilter method public in DataGrid so it can be customized simpler in StoredProcedureGrid sample
  - update bootstrap-slider and fix clash with jquery slider
  - add missing cascadeFrom property to declaration of LookupEditorBase in TypeScript

## 2.4.9 (2016-09-14)

Features:
  - include maskedinput plugin (http://digitalbush.com/projects/masked-input-plugin/) by default in Serene.Web.Assets as it is required by MaskEditor


## 2.4.8 (2016-09-13)

Bugfixes:
  - increase version of Serenity.Web.Assets and publish as SlickGrid is released through its nuget package

## 2.4.7 (2016-09-13)

Features:
  - add optional async post render cleanup support to slickgrid. this opens way to use Serenity widgets in slickgrid, though experimental.
  - if async post render delay or async post cleanup delay is less than zero, work synchronously.

Bugfixes:
  - shouldn't set ID field to null on insert in LinkingSetRelation, as some users has GUID primary keys without Insertable(false) on them
  
## 2.4.6 (2016-09-11)

Bugfixes:

  - if replace fields in fileNameFormat of upload editors are foreign / calculated, they might not be available in create. handle this case by reloading row from database before setting file name.

## 2.4.5 (2016-09-10)

Features:
  - dialog types specified in LookupEditor attribute can now be found with or without Dialog suffix
  - can specify lookup cache expiration in LookupScriptAttributes with Expiration attribute in seconds
  - fix generated dialog code for maximizable option
  - added stored procedure grid sample (thanks @mrajalko) `(Serene)`
  - deleted css applies to entire slick grid row (thanks @wldkrd1)

## 2.4.4 (2016-08-18)

Features:
  - option to generate lookup script and related lookup editor attributes in sergen (thanks @rolembergfilho)
  - resolve problem with msiejavascriptengine in win10 anniversary update
  - add quotes to support spaces in uploaded file names
  - added a 3rd option to file name format in upload editors to reference current date/time, e.g. {3:yyyyMMdd}
  - ability to reference field values in file name formats of upload editors using |SomeField|

Bugfixes:
  - call notLoggedInHandler only if received error code is NotLoggedIn

## 2.4.3 (2016-08-08)

Features:
  - make option name pascal case even if option is not originating from TS in ClientTypesGenerator
  - turned on xml docs for Serenity.Core and Serenity.Data on request, others assemblies will follow

Bugfixes:
  - close button was showing close text in a recent jQuery ui version
  - Q.toId not working properly with negative IDs
  - calling SlickRemoteView.updateItems between beginUpdate / endUpdate might cause problems in some cases
  - resolve issues with SSDeclarations.ts in some complex legacy projects

## 2.4.2 (2016-07-29)

Features:
  - added sample for searchable Visual Studio Gallery questions and answers in Serene, which also acts as a sample for using non-SQL data sources with Serenity
  - fix look of login panel in IE11 (need to not use flexbox for propertygrid, fieldset and form that is not under a dialog)

Bugfixes:
  - add missing Serenity.Web.Assets package with rtl slick.grid.min.js

## 2.4.1 (2016-07-29)

Features:
  - serene and slickgrid now has RTL support (thanks @misafer)

## 2.4.0 (2016-07-27)

Features:
  - try to resolve issues with lessc file as some users report nuget doesn't copy it under tools folder

## 2.3.7 (2016-07-21)

Features:
  - added populate linked data sample `(Serene)`
  - added serial auto numbering sample `(Serene)`
  - added product excel import sample `(Serene)`
  
Bugfixes:
  - fix column ordering not restored properly from persistance
  - use sheet name and table style specified properly in excel generator (thanks @Estrusco)

## 2.3.6 (2016-07-20)

Bugfixes:
  - fix error in client side criteria value conversion

## 2.3.5 (2016-07-20)

Features:
  - use an older version of VSSDK.ComponentModelHost to keep compability with VS 2012 & 2013
  - try to convert criteria values received from client side to field type, to avoid errors in some db engines like Oracle for dates, and get better performance for indexed fields

## 2.3.4 (2016-07-19)

Bugfixes:
  - fix usage of ROWNUM for Oracle paging queries

## 2.3.3 (2016-07-18)

Features:
  - rename EditorUtils.setReadOnly to setReadonly for overload that takes jQuery parameter
  - added readonly dialog sample `(Serene)`

Bugfixes:
  - fix paging for Oracle queries

## 2.3.2 (2016-07-15)

Features:
  - make RetrieveRequest interface members optional in TypeScript
  - added get id of inserted record sample `(Serene)`
  - added dialog boxes sample `(Serene)`
  - auto replace 'f' with '0' for excel date/time display formats
  - changed login design using vegas plugin (thanks @jsbUSMBC)
  - made login page responsive `(Serene)`

## 2.3.1 (2016-07-13)

Features:
  - Serene template size gets down to 2.5MB from 21MB+, by excluding NuGet packages from VSIX
  - static assets and code generation tools in Serenity.Web and Serenity.CodeGenerator NuGet packages are moved into new Serenity.Web.Assets and Serenity.Web.Tooling packages, which are separately versioned from other Serenity packages to reduce download sizes.
  - converted Flot, iCheck and some other parts in Serene to NuGet references
  - disable tslint by adding an empty tslint.json
  - added removing add button sample `(Serene)`

## 2.2.8 (2016-07-09)

Features:
  - added RadioButtonEditor (thanks @Estrusco)
  - made RadioButtonEditor work with enums and lookups
  - optional GridEditor and GridEditorDialog generation in sergen (thanks @dfaruque)
  - added initial values for quick filters sample `(Serene)`

## 2.2.7 (2016-07-07)

Bugfixes:
  - CascadeField property of LookupEditorBase is not converted to an ES5 property

## 2.2.6 (2016-07-07)

Bugfixes:
  - fix issue with Recaptcha due to their change in response and json deserialization by using tolerant mode

## 2.2.5 (2016-07-04)
 
Features:
  - ability to generate code for multiple tables at once in sergen (thanks @dfaruque)
  - ability to choose which files to generate in sergen. e.g. row, endpoint, grid, page... (thanks @dfaruque)

## 2.2.4 (2016-07-02)

Bugfixes:
  - SaveHandler was updating two times in some cases
  - dropdown filter text is lost in filter bar after editing second time

## 2.2.3 (2016-06-11)

Features:
  - search for subdirectories when adding translation json files from a directory
  - added pt-BR translations (thanks @rolemberfilho)
  - split site texts for Northwind / Samples to separate directories `(Serene)`

Bugfixes:
  - fix datagrid title can't be set if its not null

## 2.2.2 (2016-06-06)

Features:
  - added OracleDialect(thanks @dfaruque)
  - Serene and Northwind now works with Oracle `(Serene)`
  - alternative row generation with RowFieldsSurroundWithRegion config option in Sergen for those who likes regions (by @dfaruque) 

Bugfixes:
  - resolve automatic trimming problem for NotNull fields

## 2.2.1 (2016-05-28)

Features:
  - add ListField as an alias for CustomClassField<List<TItem>>. ListField also acts like RowListField when cloning rows, so it clones the list itself.
  
Bugfixes:
  - options defined as property for formatters or editors written in TypeScript couldn't be set
  - invalid cast error for Time fields, due to a bug in ADO.NET that converts parameter type to DateTime when you set it to Time!

## 2.2.0 (2016-05-21)

Features:
  - linking set behavior can now load list of selected items in a list request, so it is possible to show them in a grid column, or use it with in combination with a master detail scenario
  - showed representative names in customer grid
  - master detail relation can now work with non integer keys
  - multi level master detail is now possible
  - columnselection and includecolumns can be overridden for master detail relation
  - conditional row formatting sample `(Serene)`

## 2.1.9 (2016-05-19)

Features:
  - sergen generates files with UTF-8 preamble (BOM) again. BOM was lost after TFS integration work, though files was still UTF-8.

## 2.1.8 (2016-05-18)

Bugfixes:
  - dialog translation save was broken after a TS conversion

## 2.1.7 (2016-05-17)

Features:
  - sergen generates StreamField for timestamp / rowversion columns, not Int64
  - translate image upload editor errors
  - show row selection column as `[x]` in column picker dialog
  - don't display row selection column in pdf output `(Serene)`


## 2.1.6 (2016-05-16)

Bugfixes:
  - addValidationRule stopped working after 2.1.3 due to a typo in conversion of CustomValidation.cs to TypeScript


## 2.1.5 (2016-05-15)

Features:
  - we now have a cute, responsive column picker that works also with touch devices
  - integrate sortable.js (https://github.com/RubaXa/Sortable) for column picker
  - ability to persist / restore grid settings like visible columns, column widths, sort order, advanced filter (quick filter can't be persisted yet) to local storage, session storage, database or any medium you like. thanks to Mark (@c9482) for sponsoring this feature. how-to is coming.
  - compile TypeScript files on project build (in addition to compile on save) of Serene.Web, using tsc.exe as a build step, but reporting TypeScript errors as warnings to avoid potential problems with T4 files `(Serene)`
  - Q.centerDialog to center an auto height dialog after open
  - **`[Breaking Change]`** removed Q.arrayClone helper function as Array.slice does the same thing. replace "Q.arrayClone(this.view.getItems())" with "this.view.getItems().slice()" in GridEditorBase.ts
  - fixed some flexbox height issues with IE11
  - port Widget, TemplatedWidget and TemplatedDialog to TypeScript
  - **`[Breaking Change]`** Widget.create method had to be changed to a more TypeScript compatible signature. Please take TranslationGrid.ts createToolbarExtensions method source from latest Serene

## 2.1.4 (2016-05-13)

Bugfixes:
  - include enums that are not referenced in rows, but only columns/forms in ServerTypings.tt / ServerImports.tt
  - fix CustomerDialog not opening due to script error in CustomerOrdersGrid `(Serene)`


## 2.1.3 (2016-05-12)

Features:
  - made refresh button in grids without text to save space
  - update TypeScript typings to latest versions
  - added Q.Config.responsiveDialogs parameter to enable responsive for all dialogs without need to add responsive decorator
  - added separator option to ToolButton, to put a separator line between groups
  - add missing MsieJavascriptEngine reference to Serenity.Web.nuspec
  - Serenity.Externals.js and Serenity.Externals.Slick.js is merged into Serenity.CoreLib.js
  - made Note dialog responsive `(Serene)`
  - invoke TSC (TypeScript compiler) after generating files with ServerTypings.tt

## 2.1.2 (2016-05-11)

Features:
  - quote file names while calling Kdiff3 and TF.exe to prevent problems with whitespaces
  - made code generated by sergen more compatible with ones generated by .tt files

## 2.1.1 (2016-05-10)

Features:
  - Sergen will try to locate TSC and execute it after generating TypeScript code to avoid script errors
  - if script project doesn't exist switch to TypeScript generation by default and don't try to generate Saltaralle code

## 2.1.0 (2016-05-09)

Features:
  - Serene no longer comes with Serene.Script project. It's TypeScript only. Developer Guide needs to be updated.
  - your existing code written in Saltaralle should continue to work. Please report any issues at GitHub repository.
  - all Serene code is ported to TypeScript
  - start obsoleting mscorlib.js and linq.js to lower dependencies and library size. can't remove yet as Serenity.Script.UI depends on it.
  - linq like first, tryFirst, single etc. extensions in Q
  - removed unused jlayout and metisMenu plugins
  - IE8 is no longer supported as now we are targeting ES5 (jQuery 2.0 that we used for long time didn't support it anyway)
  - make use of Object.defineProperty to make properties like value etc. feel more natural in TypeScript
  - added EnumType option to EnumEditor, usable instead of EnumKey
  - ability to define quick filters on columns at server side
  - quick filters support multiple selection option
  - added sortable attribute for controlling column sortability at server side 
  - multiple and or helpers for client side criteria building
  - remove unused xss validation method
  - root namespaces doesn't need export keyword to be available in ClientTypes.tt etc.
  - HtmlBasicContentEditor in Serene moved to Serenity.Script.UI as HtmlNoteContentEditor
  - ClientTypes.tt and ServerTypings.tt works with/without Saltaralle libraries
  - all Serene dialogs will use responsive layout, e.g. flexbox by default (requires IE10+, can be turned off)
  - Serene products inline editing sample has dropdowns in category and supplier columns

Bugfixes:
  - error about restoreCssFromHiddenInit method in Mac/Safari

## 2.0.13 (2016-05-01)

Features:
  - updated Spanish translations (thanks @ArsenioInojosa)
  - update dialogExtendQ.min.js as old one didn't use translations
  - embed uglifyjs and use it with ScriptBundleManager
  - added scripts/site/ScriptBundles.json to enable script bundling / minification with a simple web.config setting (see how to in Serenity Developer Guide) `(Serene)`

## 2.0.12 (2016-04-30)

Features:
  - added some linq like helpers to Q for TypeScript, toGrouping (similar toLookup), any and count
  - added integer editor tests and resolve integer editor allows decimals
  - made most interface members optional in TypeScript defs
  - allow using font-awesome, simple line etc. font icons in toolbuttons via new icon property
  - better look and margins for toolbuttons when wrapped
  - getWidget and tryGetWidget extensions on jQuery.fn for easier access from TypeScript
  - include json2.min.js in T4 templates for users that have IE8
  - rewrote PermissionCheckEditor in TypeScript `(Serene)`
  - rewrote RolePermissionDialog and UserPermissionDialog in TypeScript `(Serene)`
  - extensive tests with QUnit for User, Role and Language dialogs `(Serene)`
  - added favicon.ico `(Serene)` 
  
Bugfixes:
  - handle TFS and site.less append problem with sergen
  - fix tfpath location search (thanks @wldkrd1)

## 2.0.11 (2016-04-18)

Features:
  - added italian translations and language option (thanks @Estrusco)
  - added porteguese translations and language option (thanks @fernandocarvalho)
  - updated Newtonsoft.Json to 8.0.30
  - updated jQuery to 2.2.3
  - updated jQuery typings to 3.0.5
  - updated jQuery UI typings to 1.4.6
  - updated Toastr typings to 0.3.01
 
Bugfixes:
  - fix loop condition in TF location search (thanks @wldkrd1)

## 2.0.10 (2016-04-17)

Features:
  - added chinese (simplified) zh-CN translations and language option (thanks @billyxing)
  - ported language dialog, language grid and translation grid to TypeScript
  - use 24 hour filename format in grid to pdf output files
  - experimental TFS integration in Sergen using tf.exe. set TFSIntegration to true in CodeGenerator.config

## 2.0.9 (2016-04-15)

Features:
  - remove useless npm files from Serenity.CodeGenerator

## 2.0.8 (2016-04-15)

Features:
  - better generated output in ssdeclarations.d.ts for exported namespaces

## 2.0.7 (2016-04-14)

Features:

  - included jsPDF and jsPDF autoTable plugin `(Serene)`
  - pdf export samples in Order, Product and Customer grids `(Serene)`
  - removed fastclick.min.js as it was reportedly causing some problems in IE 11 `(Serene)`

## 2.0.6 (2016-04-12)

Bugfixes:
  - fix equality filter captions showing ID captions after recent change

## 2.0.5 (2016-04-11)

Features:
  - ported UserDialog and UserGrid to TypeScript `(Serene)`
  - started writing isolated script tests for Serene interface `(Serene)`
  - equality filter title can be overridden
  - changes to support better TypeScript interop
  - use method overriding instead of decorators, as TypeScript can't order types properly in combined file

Bugfixes:
  - generated ts row was causing error in sergen for join fields
  - groups wasn't updated properly in PermissionCheckEditor after slickgrid update `(Serene)`
  

## 2.0.4 (2016-04-08)

Features:
  - add displayformat, urlformat and target properties to UrlFormatter
  - don't use colon string definition for export declare const strings in typescript declarations

Bugfixes:
  - fix possible null reference error for nodes without heritage clauses in typescript typings generator

## 2.0.3 (2016-04-08)

Bugfixes:
  - fix null reference error with TextAreaEditorOptions

## 2.0.2 (2016-04-07)

Features:
  - add group collapse, expand and comparer methods

Bugfixes:
  - fix problems with SlickGrid grouping in 2.0

## 2.0.1 (2016-04-07)

Bugfixes:
  - fix TypeScript dialog generated code

## 2.0.0 (2016-04-07)

Features:
  - Serenity now has TypeScript support, make sure you install TypeScript 1.8. Serene and migration guide coming soon...

## 1.9.28 (2016-04-05)

Bugfixes:
  - resolve bug with json deserialization of guid fields

## 1.9.27 (2016-03-31)

Bugfixes:
  - error in InsertUpdateLogBehavior was still intact

## 1.9.26 (2016-03-29)

Features:
  - added DisplayProperty and UrlProperty attributes to UrlFormatter (thanks @wldkrd1)

Bugfixes:
  - error in InsertUpdateLogBehavior when ID fields are not of integer type
  
## 1.9.25 (2016-03-19)

Features:
  - short circuit AND (&&) and OR (||) operator overloads can now be used while building criteria objects (thanks @wldkrd1)
  - select2 version in nuget package is forced to be 3.5.1 as some users updated it to 4+ by mistake and had errors

Bugfixes:
  - because of `[InstrinsicProperty]` attribute on SlickRemoteView.Row property, Saltarelle was ignoring `[InlineCode]` attribute on get method
  - forgot to add grouping and summaries sample to navigation `(Serene)`

## 1.9.24 (2016-03-14)

Bugfixes:
  - include slickgrid css and images under content folder

## 1.9.23 (2016-03-14)

Bugfixes:
  - slick.grid.css wasn't included in nuget file
  - getItemMetadata and getItemCssClass didn't work properly after update

## 1.9.22 (2016-03-14)

Bugfixes:
  - included SlickGrid scripts in serenity.web nuget package and removed dependency to SlickGrid package as these scripts are now customized. 

## 1.9.21 (2016-03-14)

Bugfixes:
  - resolved problems with SlickGrid affecting permission dialogs after recent merges with 6pac and x-slickgrid

## 1.9.20 (2016-03-13)

Features:
  - rewrote slick remote view in typescript
  - port extra features from x-slickgrid fork (https://github.com/ddomingues/X-SlickGrid)
  - **`[Breaking Change]`** EntityDialog.EntityId is now of type object (instead of long). As Serenity already supports non integer ID values server side, this was required. If you have code depending on EntityId being a long in dialog, you may use ToInt32() or ToInt64() extensions.
  - more tests for expression attribute mapping
  - experimental grouping and summaries feature (works client side only, no server support so don't use with paging)
  - experimental frozen columns and rows feature
  - grouping and summaries in grid sample `(Serene)`
  
Bugfixes:
  - if a field has an expression like CONCAT('a', 'b') it should also have Calculated flag

## 1.9.19 (2016-03-11)

Features:
  - added mapping for sql text field to StringField in sergen.
  - upload editors and upload behaviors should now work on tables with string/guid ID columns.

Bugfixes:
  - convert.changetype doesn't work with guids, so tables with Guid ID columns had problems.

## 1.9.18 (2016-03-08)

Features:
  - added PageWidth and PageHeight options to HtmlToPdfConvertOptions and add comments to all options
  - added grid filtered by criteria sample `(Serene)`
  - added default values in new entity dialog sample `(Serene)`
  - added filtered lookup in detail dialog sample `(Serene)`
 
Bugfixes:
  - readonly selector was invalid in DateEditor, leading to script error while typing
  - can't set min value to negative for DecimalEditor when field has Size attribute
  - Permission keys used with only PageAuthorize attribute wasn't shown in permission dialog `(Serene)`

## 1.9.17 (2016-02-27)

Features:
  - added DefaultNotifyOptions to set default toastr options for Q.Notify methods
  - Q.Notify methods now gets optional title and toastr options parameters
  - use "Module-" prefix for generated dialog css if available
  - better handling for DataGrid.AddDateRangeFilter for date/time fields.
  - sergen no longer tries to determine IsActive field or IsActiveProperty. you may still add related interfaces to row manually.
  - more flex box tuning for grids in dialog tabs and date/time editor
  - lookup editor filter by multiple values sample `(Serene)`
  
Bugfixes
  - fixed dialect issue with DateTime2 and AddParamWithValue (thanks @brettbeard)

## 1.9.16 (2016-02-24)

Features:
  - fine tune flex styles for multi column ability
  - multi column responsive dialog sample `(Serene)`
  - language and theme selection cookies has 365 days expiration `(Serene)`
  - added cloneable dialog sample `(Serene)`

## 1.9.15 (2016-02-24)

Features:
  - better responsive handling when window resized

Bugfixes
  - toId returns partial value thanks to parseInt for invalid values like '3.5.4'
  - code generator doesn't remove foreign fields determined in config file on row generation

## 1.9.14 (2016-02-24)

Features:
  - responsive flex box based dialog feature (experimental)

## 1.9.13 (2016-02-19)

Features:
  - **`[Breaking Change]`** IReport.GetData() returns object instead of IEnumerable. Replace "public IEnumerable GetData()" with "public object GetData()" in DynamicDataReport.cs. Also use cast to (IEnumerable) in ReportRepository.cs or copy latest source from Serene.
  - more options for HtmlToPdfConverter
  - wkhtmltopdf based reporting system (requires wkhtmltopdf.exe under ~/App_Data/Reporting directory)
  - order details (invoice) sample in order dialog `(Serene)`
  - cancellable bulk action sample `(Serene)`
  - working with view without ID sample `(Serene)`
  - multi column resizable dialog (form) sample `(Serene)`

## 1.9.12 (2016-02-19)

Features:
  - datetime2 support
  - minvalue, maxvalue and sqlminmax options in date/datetime editors
  - deletedby and deletedate fields are cleaned on undelete
  
Bugfixes:
  - uploaded files should be deleted when record is deleted and row is not IIsActiveDeletedRow or IDeleteLogRow 

## 1.9.11 (2016-02-14)

Features:
  - delete auditing also works for IActiveDeletedRow.
  - for delete auditing IUpdateLogRow is used if IDeleteLogRow is not available
  - data localization samples for product and category dialogs `(Serene)`
  - pending changes (close without save) confirmation sample for customer dialog `(Serene)`
  - chart in a dialog sample `(Serene)`

Bugfixes:
  - mscorlib.js wasn't included in nuget for 1.9.10 somehow

## 1.9.10 (2016-02-10)

Features:
  - criteria has operator overloads for enums, so no longer need cast to int
  - use nvarchar(4000) for parameter size, if string is shorter than 4000 (idea from dapper) for better query optimization
  - replace bracket references in query.DebugText
  - use embedded mscorlib in Serenity.Web, as one in saltarelle nuget package has compability problem with Edge browser
  - updated ckeditor package to include all languages available
  - use html lang attribute to determine ckeditor language
  - added FileDownloadFormatter to display download link for files uploaded with File/ImageUploadEditor (single only)
  - Serenity has a brand new logo!
  
Bugfixes:
  - argument out of range error when signed up users try to login `(Serene)`
  
## 1.9.9 (2016-02-09)

Features:
  - replaced FontAwesome package with embedded v4.5.0 version, please uninstall FontAwesome package from Serene
  - included ionicons to Serenity.Web

## 1.9.8 (2016-02-09)

Features:
   - localize restore and maximize buttons 
   - replace Node in sergen with 32 bit version
   - use Visible attribute also for forms
   - add HideOnlnsert and HideOnUpdate attributes that controls field visibility in forms based on edit mode 
   - binary criteria puts paranthesis always so no longer need ~ operator in most cases 
   - labelFor option for checkbox editor to make them checkable by clicking on label optionally
   - removed readonly from field declarations as some users report problems with asp.net medium trust hosting
   - added comments to PermissionKeys class as most users had confusion with it `(Serene)`
 
Bugfixes:
   - fix required validation error with HtmlContentEditor when editing an existing record
   - fix connection assignment in RowValidationContext (unused property)
   - error when oncriteria is null for outer apply and Fields.As("x") used
   - script side HasPermission method should return true for "admin" user for any permission `(Serene)`

## 1.9.7 (2016-01-28)

Bugfixes:
  - make OriginalNameProperty in single file upload editors work again

## 1.9.6 (2016-01-27)

Features:
  - added german translations (thanks to Toni Riegler)
  - add now button to DateTimeEditor
  - change default step minutes to 5 from 30 in DateTimeEditor
  - pressing space sets value to now in DateTimeEditor and DateEditor
  
## 1.9.5 (2016-01-22)

Bugfixes:
  - fix nuspec to include extensionless lessc compiler file

## 1.9.4 (2016-01-22)

Bugfixes:
  - fix cast error with Select2 editor when value is not string
  
## 1.9.3 (2016-01-21)

Features:
  - add multiple selection option to LookupEditor
  - LinkingSetRelationAttribute and related behavior to store multiple selection items in linking table
  - make selected quick search field info public in QuickSearchInput

## 1.9.2 (2016-01-19)

Features:
  - update script package versions in nuspec
  - set JsonEncodeValue to true by default for MultipleFileUploadEditor
  - added UrlFormatter
  - use ViewData to avoid hidden RuntimeBinderExceptions.
  - update typescript defs
  - divide ListRequestHandler.ApplyContainsText into submethods to make it easier to override per field logic
 
Bugfixes:
  - fix DateTimeEditor readonly state
  - fix DateTimeEditor width in form styles

## 1.9.1 (2016-01-15)

Features:
  - use lessc from lib folder if available, as bin folder might be ignored in tfs/git

## 1.9.0 (2016-01-14)

Features:
  - updated jquery to 2.2.0, xunit to 2.1.0, RazorGenerator.Templating to 2.3.11, Selenium.WebDriver to 2.48.2, StackExchange.Redis to 1.0.488, Newtonsoft.Json to 8.0.2

## 1.8.23 (2016-01-13)

Features:
  - add missing toastr options

Bugfixes:
  - check only transaction is not null for dbcommand after assigning

## 1.8.22 (2016-01-12)

Features:
  - safety check to ensure that once a connection is opened, it won't auto open again
  - allow viewing / downloading non-image files in ImageUploadEditor 

Bugfixes:
  - FileUploadEditor didn't allow non-image files

## 1.8.21 (2016-01-12)

Bugfixes:
  - rename await to awai in mscorlib.js from saltarelle, as it is a future reserved word for ES6, and creates problems with Edge
  
## 1.8.20 (2016-01-07)

Features:
  - list views in code generator along with tables. it can't determine id field, so you should manually set it in code.

## 1.8.19 (2016-01-06)

Features:
  - ability to set script side culture settings (date format etc.) using server side culture through script element with id #ScriptCulture.
  - support for tables with string/guid id columns (limited)
  
Bugfixes:  
  - set back check tree editor min-height to 150px

## 1.8.18 (2015-12-31)

Features:
  - added sign up sample `(Serene)`

Bugfixes:
  - fix file upload control look after theme change
  - change Notes permission to Northwind:General from Administration `(Serene)`

## 1.8.17 (2015-12-28)

Bugfixes:
  - fix identity insert problem with mysql and use connection dialect in more places to avoid such errors

## 1.8.16 (2015-12-28)
  
Features:
  - moved JsonRowConverter attribute to base Row class, so no need to specify it in every row subclass
  - added warning to rebuild solution after generating code in sergen
  - added TimeSpan field type matching Sql Server time data type
  
## 1.8.15 (2015-12-26)

Features:
  - add DialogTitle property to set dialog title explicitly
  - add notes editor, notes behavior and tab to Customer dialog `(Serene)`

Bugfixes:
  - fix quick search css when there is a fields dropdown
  - MappedIdField should be set if it is different than id field name in localized row

## 1.8.14 (2015-12-19)

Features:
  - add mousetrap import
  - tool buttons can have hotkeys
  - if mousetrap is included, entity dialogs has alt+s (save), alt+a (apply changes), alt+x (delete) shortcuts when form tab is active.

## 1.8.13 (2015-12-18)

Features:
  - added MySql dialect
  - code generator works with MySql
  - sample template supports MySql `(Serene)`

## 1.8.12 (2015-12-17)

Features:
  - added Throttler class, to throttle logins, logging etc.
  - added LDAP / Active Directory authentication samples `(Serene)`

## 1.8.11 (2015-12-16)

Features:
  - use titleize to set default table indentifier in sergen
  - int16 (short) types sets maximum value to 32767 properly
  - user translations are saved under ~/App_Data/texts instead of ~/scripts/site/texts to avoid permission errors `(Serene)`

Bugfixes:
  - clone mode should handled by dialog itself as some records might have negative IDs.
  - fix error with firebird primary keys in sergen
  - set column size properly for firebird string columns in sergen

## 1.8.9 (2015-12-15)

Features:
  - sergen can generate code for firebird databases (no schema support yet)
  - made top row of numbers in dashboard populate from database (cached), made more info buttons go to relevant pages `(Serene)`
  
Bugfixes:
  - when table schema is null generated row code has syntax error for foreign keys, regression bug from 1.8.5
  - sort indicator in grids was showing wrong direction

## 1.8.8 (2015-12-14)

Features:
  - better error message when Kdiff3 couldn't be located by sergen.exe.
  - use a large buffer for hashing
  - ability to set per connection dialect with an application setting (ConnectionSettings key)
  - show a warning explaining why migrations are skipped, and how to enable them `(Serene)`
  - include site.css in web project to avoid errors on publish `(Serene)`

Bugfixes:
  - fix firebird dialect FIRST keyword naming and positioning

## 1.8.7 (2015-12-12)

Features:
  - add ISqlDialect.QuoteIdentifier method and always quote column aliases with it as they might be reserved words
  
## 1.8.6 (2015-12-12)

Bugfixes:
  - fix assembly load error causing sergen.exe fail to launch
  
## 1.8.5 (2015-12-12)

Features:
  - **`[Breaking Change]`** changed enum based dialect support with interface based dialect system. remove SqlSettings.CurrentDialect line from SiteInitialization or replace it with SqlSettings.DefaultDialect = SqlServer2012Dialect.Instance.
  - new PostgreSQL dialect and code generator support.
  - serene and northwind sample now works with PostgreSQL out of the box. see how to guide.   
  - automatic bracket with dialect specific identifier quote replacement
  - **`[Breaking Change]`** fields are selected with their property name if available, for better compability with Dapper. shouldn't cause a problem if you didn't depend on prior funtionality where field name was used.
  - made capture log handler also log old rows, this resolves possible misidentified update attribution to a user, in case a record is changed directly from database before.
  - added Google Recaptcha widget and helpers
  - removed sqlite package from code generator. if you need sqlite or another provider support, register it in sergen.config.

## 1.8.4 (2015-12-01)

Features:
  - added excel export samples to product, customer and order grids `(Serene)`
  
## 1.8.3 (2015-11-30)

Features:
  - auto join with aliased fields and their view fields 
  - cloned query has same parent as source query, so they share params 
  - undo fields with expressions be read only by default (was causing problems with backwards compability)
  - remove alias with fields as noone uses or should use it anymore  
  - set ui-front to z-index 1100 instead of overriding z-index of AdminLTE divs `(Serene)`

Bugfixes:
  - fix required attribute in form not overriding one defined in row
  - fix update / insert permission attributes ignored, if specified explicitly along with modify permission
  - fix error 505 and login page links `(Serene)`

## 1.8.2 (2015-11-28)
  - it is now much easier to create cascaded editors using new CascadeFrom (editor ID), CascadeField (matching field) and CascadeValue (matching value) properties. Just set CascadeFrom and/or CascadeField properties.
  - lookup editors can also be filtered using new FilterField and FilterValue properties, without need to define a new editor type.
  - made permission and other user information like username available from client side, optionally `(Serene)`
  
## 1.8.1 (2015-11-28)

Features:
  - update slimscroll, pace and simple line icons

## 1.8.0 (2015-11-28)

Features:
  - allow comparison against empty value with StringFiltering, you can search for empty strings (not null) in filter dialog now
  - better handling of FilteringIdField determination for textual fields with complex expressions
  - **`[Breaking Change]`** cleanup serenity less files, less dependency on jQuery UI css (aristo.css), might cause minor problems, see upgrade info in Serenity guide
  - Q$Externals methods are moved under Q but Q$Externals is still an alias to Q, so no breaking changes expected
  - multi level navigation support (up to 10 levels)
  - use shorter namespace for types from other modules for T4 generated code
  - handle type namespaces properly if T4 generated service endpoint uses types from other modules / namespaces
  - integrate free AdminLTE theme `(Serene)`
  - add AdminLTE sample pages `(Serene)`
  - develop a MVC.tt file to generate strongly typed view locations (similar but much simpler than T4MVC)
  
Bugfixes:
  - IDateTimeProvider to fix Appveyor tests failing sometimes on time related tests

## 1.7.0 (2015-11-20)

Features:
  - added PropertyPanel and TemplatedPanel similar to PropertyDialog and TemplatedDialog (no need to use Panel attribute)
  - change password page `(Serene)`
  - forgot password and reset password page `(Serene)`
  - template can now detect Sql LocalDB 2014 (VS2015) and set connection strings automatically `(Serene)`
  
Bugfixes:
  - fix validator hints not showing after typescript conversion


## 1.6.8 (2015-11-16)

Features:
  - added grid row selection mixin


## 1.6.7 (2015-11-16)

Bugfixes:
  - fix critical regression bug in TemplatedWidget.GetTemplateName method, please update if you rare using 1.6.4, 1.6.5 or 1.6.6!


## 1.6.6 (2015-11-15)

Features:
  - converted file and image upload to behaviors that can be used in a row field
  - ability to generate multiple thumb sizes
  - better symbol for binary files in upload editors

  
## 1.6.5 (2015-11-15)

Features:
  - don't raise on criteria join (and, or, xor) if one of criteria is null, assume empty
  - SetEquality extension for list request
  - SetEquality helper in DataGrid
  

## 1.6.4 (2015-11-14)

Features:
  - date range quick filtering
  - ability to register a permission key with RegisterPermissionKeyAttribute to be shown in permissions dialog `(Serene)`
  - use a better datepicker trigger image


## 1.6.3 (2015-11-09)

Features:
  - implement behavior (mixin) functionality for service request handlers (list/retrieve/save/delete)
  - extract capturelog, auditlog, insert/update log, unique constraint checks as behaviors
  - MasterDetailRelationAttribute and MasterDetailRelationRelationBehavior 
  - if a field is auto created and has a foreign / calculated expression, it shouldn't be updatable / insertable by default
  - SingleField field type
  - ability to revoke a permission which is granted to roles from users `(Serene)`
  - use tree mode for permission editing and display effective permission `(Serene)`

Bugfixes:
  - order tab in customer dialog was showing all orders in new customer mode `(Serene)`


## 1.6.2 (2015-10-31)

Features:
  - code generator determines root namespace and script/web project locations at first run for Serene projects
  - added a welcome page to Serene template


## 1.6.1 (2015-10-29)

Features:
  - code generator will remove ID suffix when generating DisplayName attribute for foreign key fields.
  - **`[Breaking Change]`** title for foreign ID fields are no longer determined by their TextualField. you may have to edit ID fields DisplayName attributes if you did depend on this feature. it was generally causing confusion when a developer changes ID field display name but it is not reflected in the form.


## 1.6.0 (2015-10-29)

Features:
  - when a toast shown inside a dialog, then dialog closed right away, toast is moved to next dialog or body instead of getting lost
  - a compacter imported format for service endpoints in client side
  - code generator now produces code that is compatible (same location/contents) with T4 templates, so no need to remove that partial leftover class from XGrid.cs
  - code generator sets TextualField attribute for foreign keys
  - code generator also generates script side service and form objects
  - better error message if a row type is used with a LookupEditorAttribute, but it doesn't have a LookupScript


## 1.5.9 (2015-10-21)

Bugfixes:
  - FormContext.tt was failing after recent PropertyItemHelper changes
  - fix typo in serenity.texts.tr.json

  
## 1.5.8 (2015-10-20)

  - PropertyItemHelper is now extensible
  - 30% compacter property item scripts
  - localize server side validation texts in DataValidation

Bugfixes:
  - fix client side Select2 validation error by downgrading to 3.5.1
  - fix qunit.js not found sometimes while running tests


## 1.5.7 (2015-10-14)

Features:
  - CascadedWidgetLink to make it easier to implement cascaded select editors
  - determine quick filter title using local texts automatically
  - decimal field better conversion handling
  - sample for order and detail (in-memory) editing and updating within a unit of work (transaction) `(Serene)`
  - provide tabbed interface to edit orders in customer dialog `(Serene)`
  - country / city cascaded editors and filtering sample `(Serene)`
  - better Sql LocalDB error handling `(Serene)`

Bugfixes:
  - fix validation message hint not showing after recent jquery.validate update


## 1.5.6 (2015-10-08)

Features:
  - imported google maps using Script# import library at https://gmapsharp.codeplex.com/

Bugfixes:
  - fix typo in ScriptDtoGenerator that resolves lookup script reference generation for outside-row lookup scripts


## 1.5.5 (2015-09-25)

Features:
  - generate script code form enum members in form / column definitions, even if they are not used in any row
  - add integrated inplace item add / edit ability to LookupEditor  


## 1.5.4 (2015-09-24)

Features:
  - changed columns namespace to App.Module.Columns
  - updated razor generator
  - textarea font size changed to make it consistent with other editors in Serene
  - disable browser link feature in Visual Studio for Serene


## 1.5.3 (2015-09-14)

Features:
  - updated nuget packages
  - visual studio 2015 support
  - if all localized fields are null, delete localization row
  - introduced quick filters bar
  - quick search input no result animation
  - sqlconnections.newfor
  - dictionary get default extension
  - ability to get database name from connection string
  - bracket database reference replacer that replaces `[DB^]` style references with catalog names before sql execution

## 1.5.2 (2015-07-27)

Features:
  - support specifying a static ILocalCache and IDistributedCache provider where cache performance is critical (only use in non unit test environment)

## 1.5.1 (2015-07-27)

Features:
  - allow ordering by row fields even if field is not used in query of ListRequestHandler.
  - cache db script hash in dboverrides for even more performance in database tests
  - faster database copying in database tests
  - number formatter default format is "0.##."
  - form generator handles different namespaces better than before
  - allow filtering of navigation items in NavigationHelper
  - downgrade select2 3.5.2 to 3.5.1 as 3.5.2 had problems with jQuery validate plugin
  - quicksearchinput type delay default is now 500 ms (instead of 250 ms)
  - no ui blocking for select2ajaxeditor
  - make sure MultipleImageUploadEditor works properly


Bugfixes:
  - number formatter shows empty string for non-numeric values.
  - filter panel's not equal operator wasn't encoded properly

## 1.5.0 (2015-06-21)

Features:
  - better filter handling detection for textual fields that are connected to an ID field
  - sort filter fields by localized title
  - provide column formatters ability to initialize themselves and related column if required, and set referenced columns
  - added TreeOrdering helper
  - use closest anchor for edit link if target doesn't have the class (fixes problem when a anchor has inner elements)
  - use ServiceEndpoint base url from route attributes if GetServiceUrl is not specified for ScriptEndpointGenerator
  - add members to IDataGrid interface to access grid, view, element and filter store objects
  - put database schema name in brackets for generated code
  - date input automatic formatting for DMY cultures.
  - date editor two digit year parsing
  - debug text for SqlUpdate, SqlInsert, and SqlDelete like SqlSelect already had.
  - single / multiple field unique validation on save handler level with UniqueConstraint and Unique attributes.
  - date time editor minvalue and maxvalue is set by default according to SQL server rules
  - add url option for PostToServiceOptions
  - better handling for non-row fields in GetFromReader. their values can be read from DictionaryData.
  - StringHelper.Join method to put separator between two strings if only both is non empty or null.
  - DateTimeKind attribute to set date type on datetime fields.
  - Checkbox readonly background in forms
  - q.deepclone / q.deepextend methods that doesn't merge arrays (unlike jquery extend)
  - get required attribute from row first if available
  - allow non-integer CultureId fields for localization rows
  - introduce IIsActiveDeletedRow. IIsActiveRow doesn't have deleted state (-1) now
  - IDeleteLogRow for row types that store deleting user id and date.
  - refresh button no text (hint only) option
  - generate form key in form scripts
  - add css class with full type name to widgets too (solves problem when two modules has same widget type)
  - allow editing multiple localization in one screen
  - allow editing localizations before inserting row
  - redirect to login page with denied flag, so login page can know difference between unauthenticated and permission denied requests
  - show last jQuery selector in error console log if can't find widget on element in GetWidget
  - dispose dialog if LoadByIdAndOpenDialog fails
  - use jquery ui dialogs own close button, as dialogExtend one was causing problems in tests and mobile.
  - execute-search trigger for quick search input (to avoid delays in tests)

Bugfixes:
  - code generator preserves source table field order as it was before
  - select editor empty option text fix
  - make sure toastr is shown only on visible dialogs
  - fix error handling for grid data source

## 1.4.14 (2015-04-14)

Features:
  - paging optimization for select2 ajax editor (no need to get total record count)
  - ability to generate formatter types server side, just like editor types
  - define key constant for imported editor and formatter types
  - define baseurl and method urls for service endpoint and method imports
  - isnan extension for double and double?
  - isvalid property for decimal editor
  - date / datetimeeditor ValueAsDate property
  - phone editor auto formatting, extension support
  - filter panel validation messages are hint only now
  - scriptdtogenerator creates reference to row lookup data and id, name, local text prefix  properties


## 1.4.13 (2015-04-07)

Features:
  - fix datetimeditor time list when tohour is not specified

## 1.4.12 (2015-04-06)

Features:
  - add interval support to datetimeeditor

## 1.4.11 (2015-03-28)

Features:
  - add flush support to SqlLogger
  - add Redis caching library using StackExchange.Redis
  - sql case now supports field objects in when or value
  - minimumresultforsearch option for select2 is now zero by default
  - generic dbtestcontext objects
  - connection key and database alias for dbscript can be specified with attributes
  - connection count and list extensions without any criteria or query parameter
  - impersonating authorization service and transient granting permission service

## 1.4.10 (2015-02-27)

Features:
  - ListRequestHandler uses SortOrder attributes on row for GetNativeSort if available, name field otherwise
  - ScriptDtoGenerator, ScriptEndPointGenerator, ScriptFormGenerator constructors accepts multiple assemblies
  - Column attribute is listed after DisplayName attribute by code generator to prevent user errors
  - **`[Breaking Change]`** Renamed HiddenAttribute to IgnoreAttribute and InvisibleAttribute to HiddenAttribute to prevent confusion

Bugfixes:
  - unable to deserialize enumeration fields due to Enum.IsDefined type incompability error

## 1.4.9 (2015-02-19)

Features:
  - abstracted logging functions, added SqlLogger class to Serenity.Data
  - ability to expire cached databased and end transaction scope in database tests if data is modified in another thread / application
  - updated Newtonsoft.Json to 6.0.8, jQuery to 2.1.3, FakeItEasy to 1.25.1
  - use 32 bit Node instead of 64 for those who need to work on 32 bit windows
  - added TimeFormatter

Bugfixes:
  - fix problem with SQL 2012 dialect when skip > 0 and take = 0

## 1.4.8 (2015-02-09)

Features:
  - allow object values in Permission attributes instead of string to be able to use Enums, Integers.
  - added TimeEditor
  - updated Newtonsoft.Json to 6.0.8, jQuery to 2.1.3, FakeItEasy to 1.25.1

Bugfixes:
  - filter panel dialog was showing multiple times sometimes

## 1.4.7 (2015-01-16)

Features:
  - EmailEditor better copy paste
  - added FileBrowserBrowseUrl for CKEditorOptions
  - added Visible and Invisible attributes for columns
  - TryGetWidget and GetWidget works with non-widget types (e.g. interface or base class)
  - include schema name in generated table names and foreign keys
  - hidecheckbox option for CheckListBox items

Bugfixes:
  - Readonly implementation for EmailEditor
  - DateTime editor CSS width fixed
  - fix editor filtering options
  - fix filter panel restore problem with parenthesis, and/or

## 1.4.6 (2014-12-11)

Features:
  - allow specifying target for navigation links
  - added false and true criteria constants
  - build targets files for those who use serenity as a submodule

Bugfixes:
  - regression bug in 1.4.1 that causes cache invalidation to fail for lookups / dynamic scripts

## 1.4.5 (2014-11-28)

Features:
  - Fields can be used directly like criteria objects in queries e.g. p.Id == k.ProductId
  - In criterias doesn't use parameters for longer than 10 numeric values
  - ConstantCriteria object for values that shouldn't be converted to parameters in generated SQL
  - WithNoLock moved to extensions class
  - Flexify, Maximizable, Resizable attributes for dialog classes
  - Default order for grids are now empty to let service side decide on the initial sort order
  - **`[Breaking Change]`** Rename Field._ property to Field.Criteria as it felt unnatural.

## 1.4.4 (2014-11-23)

Features:
  - Enabled image uploads and added sample to Products form in Serene

## 1.4.3 (2014-11-23)

Features:
  - new documentation site with indexs on left (generated with gitbook)
  - introduce IAlias interface, that is implemented by Alias and RowFieldsBase
  - add ability to replace t0 aliases with something else for RowFieldsBase objects by .As("x") method (only table fields)
  - AliasName property in RowFieldsBase
  - better naming for all capital and dashed field names in serenity code generator
  - safety checks in GetFromReader method to ensure a field from another row is not used to load entity values
  - sqlquery select with field can specify alternate column name
  - criteria objects can be created from fields with _ shortcut (e.g. UserRow.Fields.UserId._ > 5 instead of new Criteria(UserRow.Fields.Personel.UserId) > 5)
  - added better handling for Criteria.In and Criteria.NotIn when an IEnumerable or array is passed
  - overload for Sql.Coalesce that uses parameters and supports criteria, field and query objects

## 1.4.2 (2014-11-19)

Features:
  - updated bootstrap to 3.3.0, jQuery.UI to 1.11.2, FakeItEasy to 1.25.0, toastr to 2.1.0
  - allow specifying default sort order for datagrid columns on server / client side (SortOrder attribute, use negative values for descending sort)
  - add theme selection option to Serene (only a light theme for now)

Bugfixes:
  - fix double html encoding with EditLinks (& was shown as &amp;).
  - fix script form generator output when full editor namespace is used
  - ServiceAuthorizeAttribute now returns 400 as HTML status code
  - null reference exception when related id field for a foreign field can't be determined in PropertyItemHelper
  - fix typo in authorization error message for DbLookupScript (it is gonna be obsolete, avoid using it)

## 1.4.1 (2014-11-13)

Features:
  - added Date and Enumeration filterings
  - added jQuery UI datepicker localization
  - JsonLocalTextRegistration and NestedLocalTextRegistration tests
  - refactored dynamic scripts
  - it is now possible to define a remote data script on a MVC action
  - indentation option for Json.StringifyIndented
  - added roles, role permissions, user permissions tables and interface to Serene sample application

Bugfixes:
  - Filter panel effective filter text is English now

## 1.4.0 (2014-11-08)

Features:
  - introduced FilterPanel widget, for advanced customizable filtering with grouping, and/or, lookup editors etc. Add `[Filterable]` attribute to your grid class to enable.
  - TemplateBundle to send templates to client side on page load by including it as a script
  - ColumnsBundle to send column metadata to client side on page load by including it as a script
  - FormsBundle to send column metadata to client side on page load by including it as a script
  - Widget templates no longer loaded asynchronously (it caused more problems than performance bonuses, use TemplateBundle if needed)
  - LookupEditor is not async, use AsyncLookupEditor if required
  - CodeGenerator doesn't produce Grid / Dialog as IAsyncInit by default
  - added LocalText tests, and documentation
  - updated Newtonsoft.JSON to 6.0.6, phantomJS to 1.9.7, select2 to 3.5.1

## 1.3.8 (2014-11-04)
Bugfixes:
  - fix edit link problem when grid is not async and columns loaded from server side
  - button noconflict fix

## 1.3.7 (2014-11-04)

Features:
  - Basic Application Sample is now Serene in its own repository
  - Serene uses FluentMigrator package to create and upgrade sample database
  - SqlConnections.GetConnectionString returns ConnectionStringInfo object instead of Tuple

## 1.3.6 (2014-11-01)

Features:
  - basic application has a language dropdown and preferred language can be changed using it (no need to play with browser settings or web.config)
  - basic application has an interface to localize all texts in any defined language (stores files in ~/scripts/site/texts/user.texts.{languageID}.json)
  - added spanish translations for serenity and basic application sample (google translate)
  - added optional LanguageID and Prefix properties to NestedLocalTextsAttribute.
  - use simple sorted dictionary format for localization json files instead of hierarchical (easier to manage and merge)
  - defined and used InitializedLocalText class for NestedLocalTextRegistration and EntityLocalTexts classes to avoid duplicate or invalid registrations when their Initialize methods called twice.
  - translated filter panel texts to english
  - **`[Breaking Change]`** moved NestedLocalTextRegistration.AddFromScripts To JsonLocalTextRegistration.AddFromJsonFiles
  - **`[Breaking Change]`** ILocalTextRegistry.TryGet now takes a language ID parameter (was using CultureInfo.CurrentUICulture.Name before)

Bugfixes:
  - Set enum key attribute for CustomFieldType enumeration

## 1.3.5 (2014-10-30)

Features:
  - make form categories sorted by their item order in form definition (no need for CategoryOrder anymore)
  - ability to localize form category titles (e.g. Forms.Northwind.Customer.Categories.General)
  - **`[Breaking Change]`** LocalTextRegistry.SetLanguageParent is changed to SetLanguageFallback
  - make LocalText.Empty read only


## 1.3.4 (2014-10-29)

Bugfixes:
  - Make FormatterType constructor public
  - Promise.Then method should return a Promise without value


## 1.3.3 (2014-10-28)

Bugfixes:

  - fix default location of select2.js in CommonIncludes


## 1.3.2 (2014-10-28)

Features:
  - CommonIncludes class for easier script/css includes
  - support {version} variable in script include statements (e.g. jquery-{version}).
  - editors can be referenced with full namespace from server side (e.g. Serenity.StringEditor). EditorTypes.tt now uses full namespace to avoid confusion.
  - embed bootstrap / jquery ui button conflict resolution to Serenity.Externals.js
  - added tests and xml doc comments for service locator, configuration, authorization, caching features.
  - TwoLevelCache now has a Get overload that takes only one expiration instead of two.
  - **`[Breaking Change]`** moved ILocalTextRegistry interface to Serenity.Abstraction for consistency
  - **`[Breaking Change]`** renamed ApplicationConfigurationRepository to AppSettingsJsonConfigRepository and moved it to Serenity.Data (from Serenity.Web)
  - **`[Breaking Change]`** removed unused DevelopmentSettings and used ASP.NET settings like compilation debug=true,
    customErrors etc.
  - **`[Breaking Change]`** Distributed.Set now takes a TimeSpan instead of DateTime, for consistency with LocalCache class.

Bugfixes:

  - fixed an interop problem with glimpse for service endpoints (ServiceModelBinder should be registered)
  - PropertyGrid is no longer an async initialized widget. It will be async widget's own responsibility to attach to another async widget.
  - EntityConnectionExtension First and Single methods no longer require an entity with IIdRow interface.


## 1.3.1 (2014-10-14)

Bugfixes:

  - dialog fills page correctly when maximized (breaking change in jQuery UI 1.11)
  - make select editor non-abstract. required for enums to work


## 1.3.0 (2014-10-09)

Features:

  - add ability to define grid columns server side (just like form definitions)
  - extensible column formatter objects (string, number, enum, date etc.)
  - reduce number of attributes and repetitive code required in service endpoints (JsonFilter, Result<T>, etc) through new ServiceEndpoint base class.
  - new EnumKey attribute for enum types so same enum names in different namespaces doesn't get mixed
  - async versions versions of Q.GetLookup, Q.GetForm etc.
  - include promise library for async operations (make sure your LayoutHead.cshtml contains ~/Scripts/rsvp.js before jQuery)
  - widgets that do async initialization. TemplatedWidget, EntityDialog, DataGrid etc. supports this new system.
    to provide backward compability, this feature is only enabled by adding IAsyncInit interface to a widget class.
  - allow criteria objects to be used on client side too (with some limitations to prevent SQL injections etc.)
  - list requests can now specify filtering through client side criteria
  - generate field names with entity classes in ServiceContracts.tt
  - merge ServiceContracts.tt and ServiceEndpoints.tt (refer to Serene sample to update your own ServiceContracts.tt file)
  - upgrade to Saltarelle Compiler 2.6.0
  - upgrade to jQuery UI 1.11.1, jQuery Validation 1.13, FontAwesome 4.2, Bootstrap 3.2

Bugfixes:
  - enum editors use integer keys instead of string keys
  - fix slick grid column sort indicators in firefox
