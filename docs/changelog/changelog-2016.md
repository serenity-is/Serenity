# CHANGELOG 2016

This changelog documents all Serenity versions published in the year 2016 (versions 1.8.19 through 2.7.2).

## 2.7.2 (2016-12-25)

### Bugfixes
  - Released assets package containing a fix for the SlickGrid Mac issue.

## 2.7.1 (2016-12-25)

### Features
  - Removed direct checks on the ISqlDialect type and used the new ISqlDialect.ServerType and ISqlDialect.NeedsBoolWorkaround properties, which also resolved problems with the custom Oracle dialect.
  - OracleDialect-like search is now case-insensitive by default.

### Bugfixes
  - Fixed the SlickGrid mouse wheel bug occurring on Mac/Chrome (thanks @mkoval-ua).

## 2.7.0 (2016-12-22)

### Features
  - ConnectionKeyAttribute can now accept a type, e.g., a row type, to get its value from that source type instead of explicitly listing the connection key string.
  - PageAuthorizeAttribute can now accept a source type, e.g., a row type that gets the ReadPermission attribute from it, instead of hardcoding the permission.
  - ServiceAuthorizeAttribute and its new subclasses, AuthorizeCreateAttribute, AuthorizeUpdateAttribute, and AuthorizeDeleteAttribute, can now get a source type parameter, e.g., a row type where they'll read relevant permissions from the source type instead of hardcoding the permission. Their permission determination algorithms closely match relevant handlers.
  - RowLookupScript looks up the inheritance chain for the ReadPermissionAttribute.
  - Request handlers look in the inheritance chain for permission attributes, and save/delete handlers should also look for the ReadPermissionAttribute.
  - Added the HeaderCssClass attribute.

### Bugfixes
  - Resolved the latest Chrome bug with the hasOwnProperty method when the key is a negative integer, affecting Saltaralle dictionaries when the hash is negative (https://bugs.chromium.org/p/chromium/issues/detail?id=673008).
  - Fixed the quick search height in the dialog.

## 2.6.10 (2016-12-12)

### Bugfixes
  - Fixed the issue where the date was required even if it was not.

## 2.6.9 (2016-12-12)

### Bugfixes
  - Fixed code generator views.

## 2.6.8 (2016-12-11)

### Features
  - When no category is specified for any of the items, the default category name is not shown, even if useCategories is true, but a category div is created for CSS compatibility.
  - Added inplaceaddpermission to the lookup editor.
  - Disabled inplace add functionality if the Select2 editor is read-only.
  - Called the dialog arrange method on resize and active tab change.
  - Added "d": date only, "g": dd/MM/yyyy HH:mm (culture-specific dmy order), "G": dd/MM/yyyy HH:mm:ss (culture-specific dmy order), "s": yyyy-MM-ddTHH:mm:ss, "u": yyyy-MM-ddTHH:mm:ss.fffZ format specifiers for the Q.formatDate function
  - Q.formatDate can now accept an iso date/time string or normal date string in addition to a Date object
  - Q.parseDate can also parse iso date/time values
  - feature selection wizard while creating a new application with Serene template. you can now optionally exclude Northwind, Meeting, Organization, Samples etc. `(Serene)`
  - added attendee editor to meeting UI `(Serene)`

## 2.6.7 (2016-12-09)

### Bugfixes
  - Fixed datetimeoffset conversion bug affecting json deserialization

## 2.6.6 (2016-12-09)

### Bugfixes
  - Fixed typescript services transform error when node is null somehow
  - Updated typescript services used for t4 transforms to 2.0.6
  - Possible problem with asyncPostProcessCleanup when a column is removed

## 2.6.5 (2016-12-08)

### Features
  - Code generator single & multiple tabs merged into a new datagrid based interface
  - Added DateTimeOffset field type 
  - Added ByteArray field type for small binary column types like timestamp, varbinary(8) etc.
  - Add minbuffer (number of buffered rows on top and bottom), and renderAllCells (render all cells in row, including non visible ones, helps with inline editing tab order) options to slick.grid
  - Give a more informational error message about "query affected N rows while 1 expected"
  - Added IReadOnly class to TypeScript defs
  - Add optional AdminLTE style for login and signup pages (thanks @DucThanhNguyen) `(Serene)`
  - Default timeout of 90 secs for running migrations `(Serene)`
  - Sample for dynamic navigation items (thanks @DucThanhNguyen) `(Serene)`
  
### Bugfixes
  - Delete button in multiple image upload editor gets lost for long file names
  - Fixed android keyboard hiding when search on menu is clicked
  - Oracle sequences should now work (Oracle users, please report)
  - Datetimeeditor fails on empty string

## 2.6.4 (2016-11-26)

### Features
  - Updated font-awesome to 4.7.0
  - Updated simple line icons to 2.4.0
  - LinkingSetRelation and MasterDetailRelation shouldn't delete detail records when master is IIsActiveDeletedRow
  - Local text key fallback registry (thanks @DucThanhNguyen)

### Bugfixes
  - Filter panel incorrect paren handling when a paren comes right after a line without paren
  - Missing closing paren at end for filter panel display text

## 2.6.3 (2016-11-26)

### Features
  - Make capture log work without integer ID
  - Handle double slashes as single slash in navigation items
  - Auto create intermediate menus on secondary or more levels for navigation items
  - Auto created navigation items now has min display order of their children
  - Add FullPath property to NavigationItem which can be used to get localized captions for navigation items. see change in LeftNavigation.cshtml
  - It's now possible to custom handle filters by overriding ApplyFieldEqualityFilter in ListRequestHandler
  - Equality filter multiple values with IN filtering are now supported natively by ListRequestHandler
  - Ability to custom handle and ignore an equality filter in ListRequestHandler by behaviors
  - LinkingSetRelationBehavior handles equality filters by default. set HandleEqualityFilter to false for manual handling.
  - Don't allow sorting for fields with NotMapped or Sortable(false) attribute
  - Set sortable false for fields with NotMapped attribute
  - Don't allow filtering on NotMapped fields
  - Fix some mistakes in Vietnamese translation (thanks @DucThanhNguyen)
  - Add ReportHelper.execute method and related sample in OrderGrid for invoice printing `(Serene)`
  - Move forward 18 years 6 months in Northwind order dates with a migration (sql server only) `(Serene)`
  - Add quick filter to Representatives in customer grid which is handled by LinkingSetRelation `(Serene)`

## 2.6.2 (2016-11-19)

### Features
  - Added tree grid drag & drop sample (thanks @dallemann for sponsoring this sample) `(Serene)`
  - Added entity dialog as panel sample `(Serene)`
  - Added vietnamese language and translation (thanks @DucThanhNguyen)
  - Added ability to inject dynamic navigation items through INavigationItemSource interface (thanks @DucThanhNguyen)
  - Include field level permission keys in permission dialog

### Bugfixes
  - Entity dialog load fails when onSuccess parameter is null
 
## 2.6.1 (2016-11-15)

### Features
  - Ability to use original ID column with GridEditorBase instead of "__id"

### Bugfixes
  - If an item is just readonly in property grid, it should still be serialized
  - Fix tree grid mixin doesn't work when toggle column has no formatter

## 2.6.0 (2016-11-12)

### Features
  - Implemented field level permissions, just add one or more of ReadPermission, ModifyPermission, InsertPermission, UpdatePermission attributes to properties in a row.
  - Added LogicOperatorPermissionService that allows using & (and), | (or) operators in permission checks, e.g. ReadPermission("A&B|C")
  - ListField, RowField, RowListField types has NotMapped flag by default so no need to add `[NotMapped]` attribute explicitly
  - ListField also supports value comparison just like RowListField
  - Added setSelectedKeys method to GridRowSelectionMixin (thanks @estrusco)
  - Added other form in tab with one toolbar sample (thanks @estrusco) `(Serene)`
  - Added a report page for Northwind, more report samples are on the way `(Serene)`

## 2.5.9 (2016-11-07)

### Features
  - Expand category when related link is clicked

### Bugfixes
  - Fields in collapsed categories could be focused

## 2.5.8 (2016-11-06)

### Features
  - TreeGridMixin for tree view like grid functionality
  - Added Collapsible attribute, for collapsible categories in forms (thanks @marcobisio)
  - Added selectKeys method to GridRowSelectionMixin (so no need to access $items directly)
  - Added Tree Grid sample `(Serene)`
  - Filter sample for showing orders containing a specific products in details `(Serene)`
  
### Bugfixes
  - Don't show pdf button in new order dialog `(Serene)`
  - Use connection.InsertAndGetID with row, instead of SqlInsert to avoid errors in postgresql and similar

## 2.5.7 (2016-11-04)

### Features
  - Added innerjoin attribute (thanks @marcobisio)
  - Added tabextesnsions.selectTab helper (thanks @estrusco)

### Bugfixes
  - Fix includecolumns parameter in data grid doesn't get cleared when columns are made hidden
  - Fix category filtering in report repository
  
## 2.5.6 (2016-10-30)

### Bugfixes
  - Avoid multiple populate on quick filter initialization. it was due to usage of change event and async initialization

## 2.5.5 (2016-10-29)

### Features
  - Integrated StackExchange.Exceptional `(Serene)`
  - Added validation to another form in tab sample `(Serene)`
  - Added inline action buttons sample `(Serene)`
  
### Bugfixes
  - Column picker dialog changing height while dragging
  - Fix dashboard link doesn't get active if url doesn't end with '/'
  
## 2.5.4 (2016-10-28)

### Bugfixes
  - Fix paging LIMIT OFFSET statement for MySql dialect

## 2.5.3 (2016-10-24)

### Bugfixes
  - Resolve problem with IE11 and slickgrid layout, that is caused by height() returning non-integer values in jQuery v3, which leads to stack overflows

## 2.5.2 (2016-10-23)

### Bugfixes
  - Fix jquery.event.drag.min.js causing problem when bundling is enabled
  
## 2.5.1 (2016-10-22)

### Features
  - Upgraded to TypeScript 2.0
  - Fix look of ui dialog buttons after jQuery UI update
  - Added date time quick filtering option
  - Better handling when date entered in a quick filter is invalid

## 2.5.0 (2016-10-21)

### Features
  - Updated bootstrap from 3.3.6 to 3.3.7
  - Updated CouchbaseNetClient from 1.3.10 to 2.3.8
  - Updated FakeItEasy, from 1.25.3.0 to 2.3.1
  - Updated jQuery from 2.2.3 to 3.1.1
  - Updated jQuery.TypeScript.DefinitelyTyped from 3.0.5 to 3.1.2
  - Updated jQuery UI from 1.11.4 to 1.12.1
  - Updated jqueryui.TypeScript.DefinitelyTyped from 1.4.6 to 1.5.1
  - Updated jquery.validation from 1.14.0 to 1.15.1
  - Updated jquery.validation.TypeScript.DefinitelyTyped from 0.4.3 to 0.4.5
  - Updated MsieJavascriptEngine from 1.7.0 to 2.0.0
  - Updated Newtonsoft.Json from 8.0.3 to 9.0.1
  - Updated qunit.TypeScript.DefinitelyTyped from 0.3.3 to 0.3.5
  - Updated RazorGenerator.Templating from 2.3.11 to 2.4.7
  - Updated Selenium.WebDriver from 2.48.2 to 3.0.0
  - Updated sortablejs.TypeScript.DefinitelyTyped from 0.1.7 to 0.1.8
  - Updated StackExchange.Redis from 1.0.488 to 1.1.608
  - Updated System.Data.SqlLocalDb from 1.14.0 to 1.15.0
  - Updated toastr.TypeScript.DefinitelyTyped from 0.3.0 to 0.3.1
  - Removed jquery.event.drag 2.2 package and embedded version 2.3 (from 6pac/SlickGrid) in Serene.Web.Assets as it is compatible with jQuery v3
  - Added data-field attribute to input fields in product grid (@wldkrd1) `(Serene)`
  - Added showing another form in tab sample `(Serene)`
  - Removed VS 15 (which is vNext, not 2015) from supported list as it was preventing upload in VSGallery `(Serene)`
  
## 2.4.13 (2016-10-14)

### Features
  - Added FilterField / FilterValue to UpdatableExtensionAttribute for extension tables that might have a constant value in addition to key matching, e.g. an address extension table with CustomerID / AddressType field
  - Added FilterField/FilterValue option to MasterDetailRelation and LinkingSetRelation, which works just like UpdatableRelation
  - Make sure bracket differences don't affect field matching process in UpdatableExtensionBehavior, by removing brackets before expression comparison
  - Made ClientSide flag/attribute obsolete as it was causing confusion, use NotMapped instead
  
### Bugfixes
  - Fix look of static text block sample in IE11 `(Serene)`

## 2.4.12 (2016-10-13)

### Features
  - Added updatable extension attribute and related behavior for 1 to 1 extension tables
  - Added Static Text Block sample `(Serene)`
  - Handle IndexCompare for RowListField type
  - Added RowField type
 
### Bugfixes
  - Fix reference to Q.ErrorHandling.showServiceError in Saltaralle code

## 2.4.11 (2016-10-06)

### Features
  - Increase checkbox column width for row selection mixin by 1 due to ie11 text overflow issue
  - Make source and disabled optional in Select2Item interface
  - Rename addItem method in Select2Editor that takes two strings to addOption to avoid confusion
  - Added enabling row selection sample `(Serene)`
  - Added user image to user table and navigation (thanks @edson)

## 2.4.10 (2016-09-16)

### Features
  - Added quick filter for boolean fields
  - Made dateRangeQuickFilter method public in DataGrid so it can be customized simpler in StoredProcedureGrid sample
  - Update bootstrap-slider and fix clash with jquery slider
  - Add missing cascadeFrom property to declaration of LookupEditorBase in TypeScript

## 2.4.9 (2016-09-14)

### Features
  - Include maskedinput plugin (http://digitalbush.com/projects/masked-input-plugin/) by default in Serene.Web.Assets as it is required by MaskEditor


## 2.4.8 (2016-09-13)

### Bugfixes
  - Increase version of Serenity.Web.Assets and publish as SlickGrid is released through its nuget package

## 2.4.7 (2016-09-13)

### Features
  - Add optional async post render cleanup support to slickgrid. this opens way to use Serenity widgets in slickgrid, though experimental.
  - If async post render delay or async post cleanup delay is less than zero, work synchronously.

### Bugfixes
  - Shouldn't set ID field to null on insert in LinkingSetRelation, as some users has GUID primary keys without Insertable(false) on them
  
## 2.4.6 (2016-09-11)

### Bugfixes

  - If replace fields in fileNameFormat of upload editors are foreign / calculated, they might not be available in create. handle this case by reloading row from database before setting file name.

## 2.4.5 (2016-09-10)

### Features
  - Dialog types specified in LookupEditor attribute can now be found with or without Dialog suffix
  - Can specify lookup cache expiration in LookupScriptAttributes with Expiration attribute in seconds
  - Fix generated dialog code for maximizable option
  - Added stored procedure grid sample (thanks @mrajalko) `(Serene)`
  - Deleted css applies to entire slick grid row (thanks @wldkrd1)

## 2.4.4 (2016-08-18)

### Features
  - Option to generate lookup script and related lookup editor attributes in sergen (thanks @rolembergfilho)
  - Resolve problem with msiejavascriptengine in win10 anniversary update
  - Add quotes to support spaces in uploaded file names
  - Added a 3rd option to file name format in upload editors to reference current date/time, e.g. {3:yyyyMMdd}
  - Ability to reference field values in file name formats of upload editors using |SomeField|

### Bugfixes
  - Call notLoggedInHandler only if received error code is NotLoggedIn

## 2.4.3 (2016-08-08)

### Features
  - Make option name pascal case even if option is not originating from TS in ClientTypesGenerator
  - Turned on xml docs for Serenity.Core and Serenity.Data on request, others assemblies will follow

### Bugfixes
  - Close button was showing close text in a recent jQuery ui version
  - Q.toId not working properly with negative IDs
  - Calling SlickRemoteView.updateItems between beginUpdate / endUpdate might cause problems in some cases
  - Resolve issues with SSDeclarations.ts in some complex legacy projects

## 2.4.2 (2016-07-29)

### Features
  - Added sample for searchable Visual Studio Gallery questions and answers in Serene, which also acts as a sample for using non-SQL data sources with Serenity
  - Fix look of login panel in IE11 (need to not use flexbox for propertygrid, fieldset and form that is not under a dialog)

### Bugfixes
  - Add missing Serenity.Web.Assets package with rtl slick.grid.min.js

## 2.4.1 (2016-07-29)

### Features
  - Serene and SlickGrid now has RTL support (thanks @misafer)

## 2.4.0 (2016-07-27)

### Features
  - Try to resolve issues with lessc file as some users report nuget doesn't copy it under tools folder

## 2.3.7 (2016-07-21)

### Features
  - Added populate linked data sample `(Serene)`
  - Added serial auto numbering sample `(Serene)`
  - Added product excel import sample `(Serene)`
  
### Bugfixes
  - Fix column ordering not restored properly from persistance
  - Use sheet name and table style specified properly in excel generator (thanks @Estrusco)

## 2.3.6 (2016-07-20)

### Bugfixes
  - Fix error in client side criteria value conversion

## 2.3.5 (2016-07-20)

### Features
  - Use an older version of VSSDK.ComponentModelHost to keep compability with VS 2012 & 2013
  - Try to convert criteria values received from client side to field type, to avoid errors in some db engines like Oracle for dates, and get better performance for indexed fields

## 2.3.4 (2016-07-19)

### Bugfixes
  - Fix usage of ROWNUM for Oracle paging queries

## 2.3.3 (2016-07-18)

### Features
  - Rename EditorUtils.setReadOnly to setReadonly for overload that takes jQuery parameter
  - Added readonly dialog sample `(Serene)`

### Bugfixes
  - Fix paging for Oracle queries

## 2.3.2 (2016-07-15)

### Features
  - Make RetrieveRequest interface members optional in TypeScript
  - Added get id of inserted record sample `(Serene)`
  - Added dialog boxes sample `(Serene)`
  - Auto replace 'f' with '0' for excel date/time display formats
  - Changed login design using vegas plugin (thanks @jsbUSMBC)
  - Made login page responsive `(Serene)`

## 2.3.1 (2016-07-13)

### Features
  - Serene template size gets down to 2.5MB from 21MB+, by excluding NuGet packages from VSIX
  - Static assets and code generation tools in Serenity.Web and Serenity.CodeGenerator NuGet packages are moved into new Serenity.Web.Assets and Serenity.Web.Tooling packages, which are separately versioned from other Serenity packages to reduce download sizes.
  - Converted Flot, iCheck and some other parts in Serene to NuGet references
  - Disable tslint by adding an empty tslint.json
  - Added removing add button sample `(Serene)`

## 2.2.8 (2016-07-09)

### Features
  - Added RadioButtonEditor (thanks @Estrusco)
  - Made RadioButtonEditor work with enums and lookups
  - Optional GridEditor and GridEditorDialog generation in sergen (thanks @dfaruque)
  - Added initial values for quick filters sample `(Serene)`

## 2.2.7 (2016-07-07)

### Bugfixes
  - CascadeField property of LookupEditorBase is not converted to an ES5 property

## 2.2.6 (2016-07-07)

### Bugfixes
  - Fix issue with Recaptcha due to their change in response and json deserialization by using tolerant mode

## 2.2.5 (2016-07-04)
 
### Features
  - Ability to generate code for multiple tables at once in sergen (thanks @dfaruque)
  - Ability to choose which files to generate in sergen. e.g. row, endpoint, grid, page... (thanks @dfaruque)

## 2.2.4 (2016-07-02)

### Bugfixes
  - SaveHandler was updating two times in some cases
  - Dropdown filter text is lost in filter bar after editing second time

## 2.2.3 (2016-06-11)

### Features
  - Search for subdirectories when adding translation json files from a directory
  - Added pt-BR translations (thanks @rolemberfilho)
  - Split site texts for Northwind / Samples to separate directories `(Serene)`

### Bugfixes
  - Fix datagrid title can't be set if its not null

## 2.2.2 (2016-06-06)

### Features
  - Added OracleDialect(thanks @dfaruque)
  - Serene and Northwind now works with Oracle `(Serene)`
  - Alternative row generation with RowFieldsSurroundWithRegion config option in Sergen for those who likes regions (by @dfaruque) 

### Bugfixes
  - Resolve automatic trimming problem for NotNull fields

## 2.2.1 (2016-05-28)

### Features
  - Add ListField as an alias for CustomClassField<List<TItem>>. ListField also acts like RowListField when cloning rows, so it clones the list itself.
  
### Bugfixes
  - Options defined as property for formatters or editors written in TypeScript couldn't be set
  - Invalid cast error for Time fields, due to a bug in ADO.NET that converts parameter type to DateTime when you set it to Time!

## 2.2.0 (2016-05-21)

### Features
  - Linking set behavior can now load list of selected items in a list request, so it is possible to show them in a grid column, or use it with in combination with a master detail scenario
  - Showed representative names in customer grid
  - Master detail relation can now work with non integer keys
  - Multi level master detail is now possible
  - Columnselection and includecolumns can be overridden for master detail relation
  - Conditional row formatting sample `(Serene)`

## 2.1.9 (2016-05-19)

### Features
  - Sergen generates files with UTF-8 preamble (BOM) again. BOM was lost after TFS integration work, though files was still UTF-8.

## 2.1.8 (2016-05-18)

### Bugfixes
  - Dialog translation save was broken after a TS conversion

## 2.1.7 (2016-05-17)

### Features
  - Sergen generates StreamField for timestamp / rowversion columns, not Int64
  - Translate image upload editor errors
  - Show row selection column as `[x]` in column picker dialog
  - Don't display row selection column in pdf output `(Serene)`


## 2.1.6 (2016-05-16)

### Bugfixes
  - addValidationRule stopped working after 2.1.3 due to a typo in conversion of CustomValidation.cs to TypeScript


## 2.1.5 (2016-05-15)

### Features
  - We now have a cute, responsive column picker that works also with touch devices
  - Integrate sortable.js (https://github.com/RubaXa/Sortable) for column picker
  - Ability to persist / restore grid settings like visible columns, column widths, sort order, advanced filter (quick filter can't be persisted yet) to local storage, session storage, database or any medium you like. thanks to Mark (@c9482) for sponsoring this feature. how-to is coming.
  - Compile TypeScript files on project build (in addition to compile on save) of Serene.Web, using tsc.exe as a build step, but reporting TypeScript errors as warnings to avoid potential problems with T4 files `(Serene)`
  - Q.centerDialog to center an auto height dialog after open
  - **`[Breaking Change]`** Removed Q.arrayClone helper function as Array.slice does the same thing. replace "Q.arrayClone(this.view.getItems())" with "this.view.getItems().slice()" in GridEditorBase.ts
  - Fixed some flexbox height issues with IE11
  - Port Widget, TemplatedWidget and TemplatedDialog to TypeScript
  - **`[Breaking Change]`** Widget.create method had to be changed to a more TypeScript compatible signature. Please take TranslationGrid.ts createToolbarExtensions method source from latest Serene

## 2.1.4 (2016-05-13)

### Bugfixes
  - Include enums that are not referenced in rows, but only columns/forms in ServerTypings.tt / ServerImports.tt
  - Fix CustomerDialog not opening due to script error in CustomerOrdersGrid `(Serene)`


## 2.1.3 (2016-05-12)

### Features
  - Made refresh button in grids without text to save space
  - Update TypeScript typings to latest versions
  - Added Q.Config.responsiveDialogs parameter to enable responsive for all dialogs without need to add responsive decorator
  - Added separator option to ToolButton, to put a separator line between groups
  - Add missing MsieJavascriptEngine reference to Serenity.Web.nuspec
  - Serenity.Externals.js and Serenity.Externals.Slick.js is merged into Serenity.CoreLib.js
  - Made Note dialog responsive `(Serene)`
  - Invoke TSC (TypeScript compiler) after generating files with ServerTypings.tt

## 2.1.2 (2016-05-11)

### Features
  - Quote file names while calling Kdiff3 and TF.exe to prevent problems with whitespaces
  - Made code generated by sergen more compatible with ones generated by .tt files

## 2.1.1 (2016-05-10)

### Features
  - Sergen will try to locate TSC and execute it after generating TypeScript code to avoid script errors
  - If script project doesn't exist switch to TypeScript generation by default and don't try to generate Saltaralle code

## 2.1.0 (2016-05-09)

### Features
  - Serene no longer comes with Serene.Script project. It's TypeScript only. Developer Guide needs to be updated.
  - Your existing code written in Saltaralle should continue to work. Please report any issues at GitHub repository.
  - All Serene code is ported to TypeScript
  - Start obsoleting mscorlib.js and linq.js to lower dependencies and library size. can't remove yet as Serenity.Script.UI depends on it.
  - Linq like first, tryFirst, single etc. extensions in Q
  - Removed unused jlayout and metisMenu plugins
  - IE8 is no longer supported as now we are targeting ES5 (jQuery 2.0 that we used for long time didn't support it anyway)
  - Make use of Object.defineProperty to make properties like value etc. feel more natural in TypeScript
  - Added EnumType option to EnumEditor, usable instead of EnumKey
  - Ability to define quick filters on columns at server side
  - Quick filters support multiple selection option
  - Added sortable attribute for controlling column sortability at server side 
  - Multiple and or helpers for client side criteria building
  - Remove unused xss validation method
  - Root namespaces doesn't need export keyword to be available in ClientTypes.tt etc.
  - HtmlBasicContentEditor in Serene moved to Serenity.Script.UI as HtmlNoteContentEditor
  - ClientTypes.tt and ServerTypings.tt works with/without Saltaralle libraries
  - All Serene dialogs will use responsive layout, e.g. flexbox by default (requires IE10+, can be turned off)
  - Serene products inline editing sample has dropdowns in category and supplier columns

### Bugfixes
  - Error about restoreCssFromHiddenInit method in Mac/Safari

## 2.0.13 (2016-05-01)

### Features
  - Updated Spanish translations (thanks @ArsenioInojosa)
  - Update dialogExtendQ.min.js as old one didn't use translations
  - Embed uglifyjs and use it with ScriptBundleManager
  - Added scripts/site/ScriptBundles.json to enable script bundling / minification with a simple web.config setting (see how to in Serenity Developer Guide) `(Serene)`

## 2.0.12 (2016-04-30)

### Features
  - Added some linq like helpers to Q for TypeScript, toGrouping (similar toLookup), any and count
  - Added integer editor tests and resolve integer editor allows decimals
  - Made most interface members optional in TypeScript defs
  - Allow using font-awesome, simple line etc. font icons in toolbuttons via new icon property
  - Better look and margins for toolbuttons when wrapped
  - getWidget and tryGetWidget extensions on jQuery.fn for easier access from TypeScript
  - Include json2.min.js in T4 templates for users that have IE8
  - Rewrote PermissionCheckEditor in TypeScript `(Serene)`
  - Rewrote RolePermissionDialog and UserPermissionDialog in TypeScript `(Serene)`
  - Extensive tests with QUnit for User, Role and Language dialogs `(Serene)`
  - Added favicon.ico `(Serene)` 
  
### Bugfixes
  - Handle TFS and site.less append problem with sergen
  - Fix tfpath location search (thanks @wldkrd1)

## 2.0.11 (2016-04-18)

### Features
  - Added italian translations and language option (thanks @Estrusco)
  - Added porteguese translations and language option (thanks @fernandocarvalho)
  - Updated Newtonsoft.Json to 8.0.30
  - Updated jQuery to 2.2.3
  - Updated jQuery typings to 3.0.5
  - Updated jQuery UI typings to 1.4.6
  - Updated Toastr typings to 0.3.01
 
### Bugfixes
  - Fix loop condition in TF location search (thanks @wldkrd1)

## 2.0.10 (2016-04-17)

### Features
  - Added chinese (simplified) zh-CN translations and language option (thanks @billyxing)
  - Ported language dialog, language grid and translation grid to TypeScript
  - Use 24 hour filename format in grid to pdf output files
  - Experimental TFS integration in Sergen using tf.exe. set TFSIntegration to true in CodeGenerator.config

## 2.0.9 (2016-04-15)

### Features
  - Remove useless npm files from Serenity.CodeGenerator

## 2.0.8 (2016-04-15)

### Features
  - Better generated output in ssdeclarations.d.ts for exported namespaces

## 2.0.7 (2016-04-14)

### Features

  - Included jsPDF and jsPDF autoTable plugin `(Serene)`
  - Pdf export samples in Order, Product and Customer grids `(Serene)`
  - Removed fastclick.min.js as it was reportedly causing some problems in IE 11 `(Serene)`

## 2.0.6 (2016-04-12)

### Bugfixes
  - Fix equality filter captions showing ID captions after recent change

## 2.0.5 (2016-04-11)

### Features
  - Ported UserDialog and UserGrid to TypeScript `(Serene)`
  - Started writing isolated script tests for Serene interface `(Serene)`
  - Equality filter title can be overridden
  - Changes to support better TypeScript interop
  - Use method overriding instead of decorators, as TypeScript can't order types properly in combined file

### Bugfixes
  - Generated ts row was causing error in sergen for join fields
  - Groups wasn't updated properly in PermissionCheckEditor after slickgrid update `(Serene)`
  

## 2.0.4 (2016-04-08)

### Features
  - Add displayformat, urlformat and target properties to UrlFormatter
  - Don't use colon string definition for export declare const strings in typescript declarations

### Bugfixes
  - Fix possible null reference error for nodes without heritage clauses in typescript typings generator

## 2.0.3 (2016-04-08)

### Bugfixes
  - Fix null reference error with TextAreaEditorOptions

## 2.0.2 (2016-04-07)

### Features
  - Add group collapse, expand and comparer methods

### Bugfixes
  - Fix problems with SlickGrid grouping in 2.0

## 2.0.1 (2016-04-07)

### Bugfixes
  - Fix TypeScript dialog generated code

## 2.0.0 (2016-04-07)

### Features
  - Serenity now has TypeScript support, make sure you install TypeScript 1.8. Serene and migration guide coming soon...

## 1.9.28 (2016-04-05)

### Bugfixes
  - Resolve bug with json deserialization of guid fields

## 1.9.27 (2016-03-31)

### Bugfixes
  - Error in InsertUpdateLogBehavior was still intact

## 1.9.26 (2016-03-29)

### Features
  - Added DisplayProperty and UrlProperty attributes to UrlFormatter (thanks @wldkrd1)

### Bugfixes
  - Error in InsertUpdateLogBehavior when ID fields are not of integer type
  
## 1.9.25 (2016-03-19)

### Features
  - Short circuit AND (&&) and OR (||) operator overloads can now be used while building criteria objects (thanks @wldkrd1)
  - Select2 version in nuget package is forced to be 3.5.1 as some users updated it to 4+ by mistake and had errors

### Bugfixes
  - Because of `[InstrinsicProperty]` attribute on SlickRemoteView.Row property, Saltarelle was ignoring `[InlineCode]` attribute on get method
  - Forgot to add grouping and summaries sample to navigation `(Serene)`

## 1.9.24 (2016-03-14)

### Bugfixes
  - Include slickgrid css and images under content folder

## 1.9.23 (2016-03-14)

### Bugfixes
  - slick.grid.css wasn't included in nuget file
  - getItemMetadata and getItemCssClass didn't work properly after update

## 1.9.22 (2016-03-14)

### Bugfixes
  - Included SlickGrid scripts in serenity.web nuget package and removed dependency to SlickGrid package as these scripts are now customized. 

## 1.9.21 (2016-03-14)

### Bugfixes
  - Resolved problems with SlickGrid affecting permission dialogs after recent merges with 6pac and x-slickgrid

## 1.9.20 (2016-03-13)

### Features
  - Rewrote slick remote view in typescript
  - Port extra features from x-slickgrid fork (https://github.com/ddomingues/X-SlickGrid)
  - **`[Breaking Change]`** EntityDialog.EntityId is now of type object (instead of long). As Serenity already supports non integer ID values server side, this was required. If you have code depending on EntityId being a long in dialog, you may use ToInt32() or ToInt64() extensions.
  - More tests for expression attribute mapping
  - Experimental grouping and summaries feature (works client side only, no server support so don't use with paging)
  - Experimental frozen columns and rows feature
  - Grouping and summaries in grid sample `(Serene)`
  
### Bugfixes
  - If a field has an expression like CONCAT('a', 'b') it should also have Calculated flag

## 1.9.19 (2016-03-11)

### Features
  - Added mapping for sql text field to StringField in sergen.
  - Upload editors and upload behaviors should now work on tables with string/guid ID columns.

### Bugfixes
  - Convert.changetype doesn't work with guids, so tables with Guid ID columns had problems.

## 1.9.18 (2016-03-08)

### Features
  - Added PageWidth and PageHeight options to HtmlToPdfConvertOptions and add comments to all options
  - Added grid filtered by criteria sample `(Serene)`
  - Added default values in new entity dialog sample `(Serene)`
  - Added filtered lookup in detail dialog sample `(Serene)`
 
### Bugfixes
  - Readonly selector was invalid in DateEditor, leading to script error while typing
  - Can't set min value to negative for DecimalEditor when field has Size attribute
  - Permission keys used with only PageAuthorize attribute wasn't shown in permission dialog `(Serene)`

## 1.9.17 (2016-02-27)

### Features
  - Added DefaultNotifyOptions to set default toastr options for Q.Notify methods
  - Q.Notify methods now gets optional title and toastr options parameters
  - Use "Module-" prefix for generated dialog css if available
  - Better handling for DataGrid.AddDateRangeFilter for date/time fields.
  - Serene no longer tries to determine IsActive field or IsActiveProperty. you may still add related interfaces to row manually.
  - More flex box tuning for grids in dialog tabs and date/time editor
  - Lookup editor filter by multiple values sample `(Serene)`
  
Bugfixes
  - Fixed dialect issue with DateTime2 and AddParamWithValue (thanks @brettbeard)

## 1.9.16 (2016-02-24)

### Features
  - Fine tune flex styles for multi column ability
  - Multi column responsive dialog sample `(Serene)`
  - Language and theme selection cookies has 365 days expiration `(Serene)`
  - Added cloneable dialog sample `(Serene)`

## 1.9.15 (2016-02-24)

### Features
  - Better responsive handling when window resized

Bugfixes
  - toId returns partial value thanks to parseInt for invalid values like '3.5.4'
  - Code generator doesn't remove foreign fields determined in config file on row generation

## 1.9.14 (2016-02-24)

### Features
  - Responsive flex box based dialog feature (experimental)

## 1.9.13 (2016-02-19)

### Features
  - **`[Breaking Change]`** IReport.GetData() returns object instead of IEnumerable. Replace "public IEnumerable GetData()" with "public object GetData()" in DynamicDataReport.cs. Also use cast to (IEnumerable) in ReportRepository.cs or copy latest source from Serene.
  - More options for HtmlToPdfConverter
  - Wkhtmltopdf based reporting system (requires wkhtmltopdf.exe under ~/App_Data/Reporting directory)
  - Order details (invoice) sample in order dialog `(Serene)`
  - Cancellable bulk action sample `(Serene)`
  - Working with view without ID sample `(Serene)`
  - Multi column resizable dialog (form) sample `(Serene)`

## 1.9.12 (2016-02-19)

### Features
  - Datetime2 support
  - Minvalue, maxvalue and sqlminmax options in date/datetime editors
  - Deletedby and deletedate fields are cleaned on undelete
  
### Bugfixes
  - Uploaded files should be deleted when record is deleted and row is not IIsActiveDeletedRow or IDeleteLogRow 

## 1.9.11 (2016-02-14)

### Features
  - Delete auditing also works for IActiveDeletedRow.
  - For delete auditing IUpdateLogRow is used if IDeleteLogRow is not available
  - Data localization samples for product and category dialogs `(Serene)`
  - Pending changes (close without save) confirmation sample for customer dialog `(Serene)`
  - Chart in a dialog sample `(Serene)`

### Bugfixes
  - Mscorlib.js wasn't included in nuget for 1.9.10 somehow

## 1.9.10 (2016-02-10)

### Features
  - Criteria has operator overloads for enums, so no longer need cast to int
  - Use nvarchar(4000) for parameter size, if string is shorter than 4000 (idea from dapper) for better query optimization
  - Replace bracket references in query.DebugText
  - Use embedded mscorlib in Serenity.Web, as one in saltarelle nuget package has compability problem with Edge browser
  - Updated ckeditor package to include all languages available
  - Use html lang attribute to determine ckeditor language
  - Added FileDownloadFormatter to display download link for files uploaded with File/ImageUploadEditor (single only)
  - Serenity has a brand new logo!
  
### Bugfixes
  - Argument out of range error when signed up users try to login `(Serene)`
  
## 1.9.9 (2016-02-09)

### Features
  - Replaced FontAwesome package with embedded v4.5.0 version, please uninstall FontAwesome package from Serene
  - Included ionicons to Serenity.Web

## 1.9.8 (2016-02-09)

### Features
   - Localize restore and maximize buttons 
   - Replace Node in sergen with 32 bit version
   - Use Visible attribute also for forms
   - Add HideOnlnsert and HideOnUpdate attributes that controls field visibility in forms based on edit mode 
   - Binary criteria puts paranthesis always so no longer need ~ operator in most cases 
   - LabelFor option for checkbox editor to make them checkable by clicking on label optionally
   - Removed readonly from field declarations as some users report problems with asp.net medium trust hosting
   - Added comments to PermissionKeys class as most users had confusion with it `(Serene)`
 
### Bugfixes
   - Fix required validation error with HtmlContentEditor when editing an existing record
   - Fix connection assignment in RowValidationContext (unused property)
   - Error when oncriteria is null for outer apply and Fields.As("x") used
   - Script side HasPermission method should return true for "admin" user for any permission `(Serene)`

## 1.9.7 (2016-01-28)

### Bugfixes
  - Make OriginalNameProperty in single file upload editors work again

## 1.9.6 (2016-01-27)

### Features
  - Added german translations (thanks to Toni Riegler)
  - Add now button to DateTimeEditor
  - Change default step minutes to 5 from 30 in DateTimeEditor
  - Pressing space sets value to now in DateTimeEditor and DateEditor
  
## 1.9.5 (2016-01-22)

### Bugfixes
  - Fix nuspec to include extensionless lessc compiler file

## 1.9.4 (2016-01-22)

### Bugfixes
  - Fix cast error with Select2 editor when value is not string
  
## 1.9.3 (2016-01-21)

### Features
  - Add multiple selection option to LookupEditor
  - LinkingSetRelationAttribute and related behavior to store multiple selection items in linking table
  - Make selected quick search field info public in QuickSearchInput

## 1.9.2 (2016-01-19)

### Features
  - Update script package versions in nuspec
  - Set JsonEncodeValue to true by default for MultipleFileUploadEditor
  - Added UrlFormatter
  - Use ViewData to avoid hidden RuntimeBinderExceptions.
  - Update typescript defs
  - Divide ListRequestHandler.ApplyContainsText into submethods to make it easier to override per field logic
 
### Bugfixes
  - Fix DateTimeEditor readonly state
  - Fix DateTimeEditor width in form styles

## 1.9.1 (2016-01-15)

### Features
  - Use lessc from lib folder if available, as bin folder might be ignored in tfs/git

## 1.9.0 (2016-01-14)

### Features
  - Updated jquery to 2.2.0, xunit to 2.1.0, RazorGenerator.Templating to 2.3.11, Selenium.WebDriver to 2.48.2, StackExchange.Redis to 1.0.488, Newtonsoft.Json to 8.0.2

## 1.8.23 (2016-01-13)

### Features
  - Add missing toastr options

### Bugfixes
  - Check only transaction is not null for dbcommand after assigning

## 1.8.22 (2016-01-12)

### Features
  - Safety check to ensure that once a connection is opened, it won't auto open again
  - Allow viewing / downloading non-image files in ImageUploadEditor 

### Bugfixes
  - FileUploadEditor didn't allow non-image files

## 1.8.21 (2016-01-12)

### Bugfixes
  - Rename await to awai in mscorlib.js from saltarelle, as it is a future reserved word for ES6, and creates problems with Edge
  
## 1.8.20 (2016-01-07)

### Features
  - List views in code generator along with tables. it can't determine id field, so you should manually set it in code.

## 1.8.19 (2016-01-06)

### Features
  - Ability to set script side culture settings (date format etc.) using server side culture through script element with id #ScriptCulture.
  - Support for tables with string/guid id columns (limited)
  
### Bugfixes: 
  - Set back check tree editor min-height to 150px
