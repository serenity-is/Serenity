# CHANGELOG 2018

This changelog documents all Serenity versions published in the year 2018 (versions 3.3.15 through 3.8.3).

## 3.8.3 (2018-12-29)

### Features
  - Added idle (session) timeout feature and related sample `(StartSharp)`.
  - Implemented impersonation (login as) functionality in the users page `(StartSharp)`.
  - Backported two-factor authentication sample to .NET Core `(StartSharp)`.
  - Added "from address" setting for mailing in the .NET Core version `(StartSharp)`.
  - Added `selectedItem` getter in `SelectEditor` (thanks @dfaruque).
  - Changed the protection level of `clearItems`, `addItem`, and `addOption` in `LookupEditor` to public (thanks @dfaruque).
  - Enabled specifying the location of the button separator (thanks @Jin).
  - Added `GridRadioSelectionMixin` (thanks @Jin).
  - Moved most of the `LookupEditor` code, such as cascading and filtering, into the `Select2Editor` base class for reuse in custom editors.
  - `EnumEditor` now supports multiple options.
  
### Bugfixes
  - Fixed the UI appearance of `DateTimeEditor` (thanks @adam feng).
  - Resolved fonts folder casing issue in Linux.
  - Fixed `OracleSchemaProvider` based on a pull request by @kilroyFR.
  - Addressed the issue where `hotkeyContext` of `ToolButtonDefinition` was ignored (thanks @hannesb).

## 3.8.2 (2018-12-04)

### Features
  - Skipped enum members with the `Ignore` attribute in `ServerTypingsGenerator`.
  - Triggered the change handler when the "Now" button is clicked in the `DateTimeEditor`.
  - Triggered the change event of the `TimeEditor` hour input when the minute changes.

### Bugfixes
  - Made the minutes section read-only when the `TimeEditor` is read-only.
  - Hid delete buttons on images when `MultipleImageUploadEditor` is read-only.
  - Fixed a typo in `unsubscribe viewOnDataChanged` of `SlickGrid`.

## 3.8.1 (2018-11-03)

### Bugfixes
  - Resolved package downgrade issue with `System.Data.SqlClient`.

## 3.8.0 (2018-11-03)
### Features
  - Updated to .NET Core 2.1 and upgraded many packages.
  - Improved error messages when a field cannot be loaded from the database using `getFromReader`, showing the field name and row type.
  - Added local text generation capability (optional) to server typings, allowing local texts to be accessed with completion and compile-time checking. Currently, this only processes row field texts and nested local texts.
  - Serenity Guide is now hosted at [https://serenity.is/docs](https://serenity.is/docs).
  - Added a read-only option to `CheckTreeEditor` and its subclasses, such as `CheckLookupEditor`.
  - Enhanced `UniversalAssemblyResolver` to locate and load more packages via `sergen`.
  - Divided the `InBrace` function of `CodeWriter` into `StartBrace` and `EndBrace` functions, enabling manual block opening and closing
  
### Bugfixes
  - Corrected the `allowNegatives` behavior in `DecimalEditor` when `minValue`/`maxValue` is not specified.
  - Ensured switching to the main tab only if the current tab is being disabled.
  - Added `/` to handler paths to restrict their availability to the root (see [GitHub issue #4017](https://github.com/serenity-is/Serenity/issues/4017)).
  
## 3.7.7 (2018-09-15)
### Features
  - Enabled StackExchange.Exceptional (e.g. exception logging) in ASP.NET Core version `(StartSharp)`
  - Added docs for migrating Serene project to StartSharp inplace / to a new project `(StartSharp)`
  - Added sample daily background task that generates PDF from a report and sends it by e-mail `(StartSharp)`
  - Better handling when header filters mixin is working client side with formatted column text instead of underlying value `(StartSharp)`
  - Add `cke_dialog` class to allow header filter search input work under dialogs `(StartSharp)`
  - Use timeout to give cascaded dropdowns a chance to update / clear themselves, also fixes quick filter clear problem
  - Updated Scriban to 1.2.3
  - Implemented `IReadOnly` in `RadioButtonEditor` (thanks @dfaruque)
  - Made `ApplyDefaultValues` return row for chaining, added `unassignedOnly` option to the method
  - Modified CSS classes used for `OneThirdWidthAttribute`, `QuarterWidthAttribute`, `ThreeQuarterWidthAttribute`, and `TwoThirdWidthAttribute` so that they stay around 250-350 pixels in worst case. Might be a breaking change for your existing layouts.
  - Added `JustThis` option to form layout attributes like `HalfWidth` etc, so that you won't have to cancel form widths to just set width for one item
  - Added `JustThis` option to `LabelWidthAttribute` just like `FormWidthAttribute`
  - Added `VariantField` which should correspond to `sql_variant` (sergen doesn't auto use it yet)
  - Stop auto numeric from raising errors when an out of range value is set manually, only fix value on tab out if some action performed to change value
  - Added `AllowNegatives` property to `IntegerEditor` and `DecimalEditor`, which alongside `AllowNegativesByDefault` static property controls ability to edit negative values in editors when `MinValue` is not explicitly set
  - Added `widthset` to `PropertyItem` which determines if an explicit width value is assigned to property in `columns.cs` etc.
  - Split part that loads persisted settings when settings = null in `DataGrid.restoreSettings` into its own function, `getPersistedSettings`

### Bugfixes
  - If there was an exception, shouldn't commit transaction but dispose (rollback) it in .NET Core service endpoints
  - Resolve issue that fields with a space inside name can't be removed from grouping `(StartSharp)`
  - Check value of `[ReadOnly(false)]` attribute before setting item as read only
  - `ResetLabelWidth` attribute shouldn't require a value
  - Gray out now button in `DateTimeEditor` when readonly
  - Handle issue with extra whitespaces in generated `Row.cs` after scriban update
  - Fix typo for `tabbingDirections` in `slick.grid.js` (thanks @globtech1)
  - Delete `.orig` file in `DeleteFileAndRelated` as well (thanks @globtech1)
  
## 3.7.6 (2018-07-10)
### Features
  - Updated `Serenity.Web.Assets` package

## 3.7.5 (2018-07-10)
### Features
  - Added drag & drop grouping mixin and related sample `(StartSharp)`
  - Added customizable summaries mixin and related sample `(StartSharp)`
  - Group headers and summary footers use column formatter if available
  - Ignore when one or more of requested distinct fields are not allowed and instead of raising exception return null to gracefully handle issue
  
### Bugfixes
  - Handle ckeditor warning about upload plugin

## 3.7.4 (2018-07-05)
### Features
  - Introduced `AllowHideAttribute` which when set on a property to false, doesn't let that column to be hidden in column picker dialog
  - Added `collapseAll()` and `expandAll()` to `TreeGridMixin.ts`, thanks @dfaruque
  - Bengali translation, thanks @dfaruque
  - Remove quick filter if user doesn't have permission to column
  - Advanced filter should only show columns that user has read permission to (thanks @kilroyFR)
  - Renamed `ColumnFiltersMixin` to `HeaderFiltersMixin` `(StartSharp)`
  - Better positioning for header filter popup `(StartSharp)`
  - Added ability to check for roles like permissions with `Role:RoleKey` if role has a key set (a new field added to Roles table) `(StartSharp)`
  - Fix `SqlErrorStore` error logging in non-SQL Server type database servers
  
### Bugfixes
  - Fix typos in `[DefaultHandler]` implementation for Save and Delete handlers
  - Avoid duplicate key exception while adding implicit permissions in `UserPermissionService`
  - Fix some `ClientTypes` conditional files left behind even if related features are not selected

## 3.7.3 (2018-06-28)
### Features
  - Updated `Serenity.Web.Assets` package


## 3.7.2 (2018-06-28)
### Features
  - Added ability to select distinct fields to `ListRequest` and its handler by using `DistinctFields` which works similar to `Sort` property
  - Excel style column filtering grid mixin and related sample `(StartSharp)`
  - Call `init` async on widget if the widget is async as well

### Bugfixes
  - Removed BW operator from `StringFiltering` (thanks @marcobisio)
  - Added missing return statement that causes switch on advanced filter contains to fall back to startsWith operator (thanks @edwardch)
  - Replaced default `keyPrefix` with `config.KeyPrefix` in `RedisDistributedCache` (thanks @MungoWang)
  - Handle null reference exception gracefully when row type doesn't have a nested fields type
  - .NET Core memory cache raises an exception if timespan is less than zero (#3514)

## 3.7.1 (2018-05-16)

### Bugfixes
  - Fix `sergen` generating empty files due to a change in template engine (scriban) we use

## 3.7.0 (2018-05-14)
### Features
  - Support for UNION including INTERSECT, EXCEPT and/or ALL (where DB support is available) to `SqlQuery` using `.Union` method
  - Added ability to determine Name field by using new `[NameProperty]` attribute instead of `INameRow` interface if name field is of non-string type
  - Introduced `[DefaultHandler]` attribute, which when placed on a service handler, e.g. `ListRequestHandler`, `SaveHandler` etc, allows behaviors like `MasterDetailRelationBehavior`, `LinkingSetRelationBehavior`, `UpdatableExtensionBehavior` etc to use your custom handlers (`MySaveHandler` etc.) instead of generic ones for that row type. This allows logic in your custom handlers to be reused for related records, e.g. detail rows for `MasterDetailRelation` without having to write a behavior.  
  - Added `[DataAuditLog]` attribute which allows simple audit logging for change operations to any entity (StartSharp)
  - Set filename field after upload so that audit logging can take the final value
  - Data audit log viewer sample (StartSharp)
  - Split master details grid sample (StartSharp)
  - Add `MigrationAttribute` to enforce migration versioning
  - Switched to SourceLink from GitLink
  - Removed .Net45.csproj versions for some Serenity libraries like Core, Data, Entity, Services...
  - Updated all packages including AspNetCore, AspNet.Mvc, Newtonsoft.Json, Nuglify, Selenium, CKEditor etc.
  - Use double right arrow character for organization dropdown (StartSharp)
  - Added documentation about upgrading from Serene to StartSharp, inplace or by migration (StartSharp)
  - Handle *.ts *.cs includes for ServerTypings / ClientTypes to get less merge conflicts on .csproj files
  - Added debounce function
  - Got rid of .Net45.csproj files for Serenity.Core, Serenity.Data, Serenity.Data.Entity, Serenity.Services and Serenity.Caching packages, switched to SourceLink
  
### Bugfixes
  - Resolve intellisense issue on project creation (StartSharp)
  - PresenceField bug on updatable extension (thanks @marcobisio)
  - Fix missing element in Recaptcha Widget Editor (thanks @edson)
  - Fix typo in closequote for sql2000dialect (thanks @hannesb)
  - `FastReflection` should try to skip visibility checks
  
## 3.6.0 (2018-03-31)
### Features
  - Changes to widget for React integration (currently StartSharp only), you'll need to add `@types/react` to your `package.json` even if you'll not use React at all
  - Include react scripts in `Serenity.Web.Assets`
  - Full-featured e-mail client (IMAP) sample written with React and MailKit (StartSharp)
  - Rewrote `CardViewMixin` with React (StartSharp)
  - Introduced `Serenity.Pro.Scripts` nuget package which makes it possible to update StartSharp scripts. It contains UI, App and EmailClient scripts now.
  - Allow viewing / editing with inplace button even if the lookup editor itself is readonly
  - Use inline source maps for better script debugging experience with corelib
  - Add helpers required for spread operator
  - Grouping helper in corelib
  - Ability to do replacements in `ScriptBundles.json`, e.g. replace development version of some script with prod using web.config setting `Replacements`
  - Improve typing of widget class, make editor discovery more tolerant even if type doesn't have a registerEditor attribute
  - Don't try to minimize files ending in `min.js` when script bundling is on
  - Deprecating `Saltaralle`, no longer `Serenity.Script` package

### Bugfixes
  - Fix some slickgrid compatibility issues in chrome related to jquery changes
  - Fix return type declarations of some script data functions
## 3.5.5 (2018-02-20)

### Bugfixes
  - Null reference exception during servertypings generation of types with generic parameters

## 3.5.4 (2018-02-19)
### Features
  - Add reference to `Serenity.Web.Tooling` package so that it auto updates as well
  - **`[Breaking Change]`** Need to replace `externalType.Interfaces.Add(intf.FullName)` with `externalType.Interfaces.Add(intf.InterfaceType.FullName)` in `CodeGenerationHelpers.ttinclude`
  - `MasterKeyField` option to use another field as master key in master row, thanks @hannesb
  
## 3.5.3 (2018-02-19)

### Bugfixes
  - Resolve possible problem with `loadScriptAsync`
  - `[InstrinicProperty]` in legacy script form import generator is causing issues

## 3.5.2 (2018-02-18)

### Bugfixes
  - Fix `Row.scriban` sergen template

## 3.5.1 (2018-02-18)

### Bugfixes
  - Add missing `Mono.Cecil.dll`

## 3.5.0 (2018-02-18)
### Features
  - Rewrote servertypings generator using mono.cecil to reduce assembly loading errors on .NET Core sergen transform
  - Added `CheckLookupEditor` which is similar to lookup editor with multiple option but uses checkboxes instead
  - Updated AspNetCore, jQuery, Redis, Couchbase, Nuglify, Dapper and some other packages
  - Added module attribute that will be used to auto determine local text prefix and lookup script keys
  - `[LookupScript]` attribute can now be used without specifying a lookup key. In that case, the lookup key will be auto-generated from row module / name and / or class namespace.
  - No need to set `localTextPrefix` as it will now be determined by `RowIdentifier`, e.g. module identifier dot row type name without row suffix
  - Added `IIsDeletedRow` which works similar to `IIsActiveDeletedRow` but as a Boolean field
  - Added `IgnoreDeleted` option to `UniqueConstraintAttribute` and `UniqueAttribute` to skip soft deleted records on check
  - Added `CheckNames` option to `BasedOnRowAttribute` so that property name matching with row can be validated optionally to ensure valid / exact case matching property names. can turn check off on a property by adding `[NotMapped]`
  - Ability to skip minification for specific files using `NoMinimize` option in `ScriptBundling` settings
  - Added `ResolvePath` function to `ContentHashCache` to resolve virtual paths to absolute or cdn urls when enabled
  - Seek to page 1 after a change in filters / sort orders / quick filter in slick grid
  - More descriptive error message when lookup script is not found
  - Try to give more info when lookup script fails to load due to permissions or another exception
  - Error handler to show runtime errors in browser console as toast on localhost / 127.0.0.1
  - Make sure there is only one type with a lookup key, raise an error otherwise to warn user
  - Overflow hidden to prevent double scrollbars in iframedialog
  - Removing `responsive()` attribute from dialog template as its should be default now in all except legacy apps
  - Validate cache on commit even if row doesn't have `TwoLevelCachedAttribute`, so that attribute is not required anymore
  - Added `ForceCascadeDelete` option to `LinkingSetRelationAttribute` and `MasterDetailRelationAttribute` that forces deletion of sub records even if master row uses soft delete
  - Removing old `ResponsiveDialog` and `MultiColumnResponsiveDialog` samples as we now have a different way to handle them. removed  responsiveDialog decorators as it is no longer needed. `(Serene)`
  - Added `CheckNames = true` to all `BasedOnRow` attributes so that property / field name matching can be validated `(Serene)`
  - Increase upload request limits to 50mb `(Serene)`
  - Make sure colorbox scales properly for very large images `(Serene)`
  - Finalize agenda and decision tabs in meeting module `(StartSharp)`
  - Include ckeditor in pages where required to improve first time startup time `(StartSharp)`
  - Use tablename and module attributes for all rows, move external lookups to Lookups namespace instead of Scripts, remove explicit lookup keys from all lookups as it can be auto generated now. `(StartSharp)`

  
### Bugfixes
  - Possible race condition in css / script bundling at first startup
  - Fix quick search input property/method reference, closes #3248
  - Check-box vertical alignment issue
  - Fix northwind employee symbols `(Serene)`
  - Fix missing navigation icons `(Serene)`

## 3.4.4 (2018-01-30)

### Bugfixes
  - Fix issue with formatter `displayFormat` option not working after TS port, and a few other options in other editor / formatters

## 3.4.3 (2018-01-28)
### Features
  - Implemented sorting and text search functionality in DataExplorer sample `(StartSharp)`

### Bugfixes
  - Fix email and image upload editor value properties are readonly in TS typings
  - Added new `EmailAddressEditor` with one input for e-mail editing (unlike `EmailEditor`)
  - Fix `MinuteFormatter` returns empty text

## 3.4.2 (2018-01-26)

### Bugfixes
  - Fix possible script registration issue with enum types

## 3.4.1 (2018-01-26)

### Bugfixes
  - Use categories default was changed during TypeScript port
  - Persist sort order wasn't working

## 3.4.0 (2018-01-24)
### Features
  - 4 new themes: Azure, Azure Light, Cosmos, Cosmos Light `(StartSharp)`
  - Ported all code left in Saltaralle (12K+ lines) to TypeScript
  - Optimized ServerTypings generation so that it produces a MyProject.Web.js file that is up to %50 less in minified size.
  - **`[Breaking Change]`** Used const enum feature in TypeScript 2.4 to generate field names in ServerTypings Row.ts. Replace "var fld = SomeRow.Fields" with "import fld = SomeRow.Fields" and move that line to just under "namespace" declaration.
  - Don't validate CSRF token when user is not logged in or cookie token is null (e.g. json service client)
  - Changed icon for alert dialog (thanks @Jin)
  - Added descending property in `DateYearEditorAttribute` (thanks @dfaruque)
  
### Bugfixes
  - Fix enum field cast error
  - `CompareValue()` of field types based on list/array returns wrong value (#3156, thanks @hannesb)

## 3.3.15 (2018-01-04)

### Bugfixes
  - Missing closing double quote in `site.less` for new module generated by sergen