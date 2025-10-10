# CHANGELOG 2015

This changelog documents all Serenity versions published in the year 2015 (versions 1.4.7 through 1.8.18).

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
