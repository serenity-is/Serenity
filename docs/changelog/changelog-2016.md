# CHANGELOG 2016

This changelog documents all Serenity versions published in the year 2016 (versions 1.8.19 through 2.7.2).

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
