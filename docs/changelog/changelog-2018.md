# CHANGELOG 2018

This changelog documents all Serenity versions published in the year 2018 (versions 3.3.15 through 3.8.3).

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