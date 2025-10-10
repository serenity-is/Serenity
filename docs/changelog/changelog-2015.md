# CHANGELOG 2015

This changelog documents all Serenity versions published in the year 2015 (versions 1.4.7 through 1.8.18).

## 1.8.18 (2015-12-31)

### Features
  - Added sign-up sample `(Serene)`

### Bugfixes
  - Fixed file upload control appearance after theme change
  - Changed Notes permission to Northwind:General from Administration `(Serene)`

## 1.8.17 (2015-12-28)

### Bugfixes
  - Fixed identity insert problem with MySQL and used connection dialect in more places to avoid such errors

## 1.8.16 (2015-12-28)
  
### Features
  - Moved JsonRowConverter attribute to base Row class, so no need to specify it in every row subclass
  - Added warning to rebuild solution after generating code in sergen
  - Added TimeSpan field type matching SQL Server time data type
  
## 1.8.15 (2015-12-26)

### Features
  - Added DialogTitle property to set dialog title explicitly
  - Added notes editor, notes behavior and tab to Customer dialog `(Serene)`

### Bugfixes
  - Fixed quick search CSS when there is a fields dropdown
  - MappedIdField should be set if it is different than ID field name in localized row

## 1.8.14 (2015-12-19)

### Features
  - Added Mousetrap import
  - Tool buttons can have hotkeys
  - If Mousetrap is included, entity dialogs have Alt+S (save), Alt+A (apply changes), Alt+X (delete) shortcuts when form tab is active.

## 1.8.13 (2015-12-18)

### Features
  - Added MySQL dialect
  - Code generator works with MySQL
  - Sample template supports MySQL `(Serene)`

## 1.8.12 (2015-12-17)

### Features
  - Added Throttler class, to throttle logins, logging etc.
  - Added LDAP / Active Directory authentication samples `(Serene)`

## 1.8.11 (2015-12-16)

### Features
  - Used Titleize to set default table identifier in sergen
  - Int16 (short) types set maximum value to 32767 properly
  - User translations are saved under ~/App_Data/texts instead of ~/scripts/site/texts to avoid permission errors `(Serene)`

### Bugfixes
  - Clone mode should be handled by dialog itself as some records might have negative IDs.
  - Fixed error with Firebird primary keys in sergen
  - Set column size properly for Firebird string columns in sergen

## 1.8.9 (2015-12-15)

### Features
  - Sergen can generate code for Firebird databases (no schema support yet)
  - Made top row of numbers in dashboard populate from database (cached), made more info buttons go to relevant pages `(Serene)`
  
### Bugfixes
  - When table schema is null generated row code has syntax error for foreign keys, regression bug from 1.8.5
  - Sort indicator in grids was showing wrong direction

## 1.8.8 (2015-12-14)

### Features
  - Better error message when Kdiff3 couldn't be located by sergen.exe.
  - Used a large buffer for hashing
  - Ability to set per connection dialect with an application setting (ConnectionSettings key)
  - Show a warning explaining why migrations are skipped, and how to enable them `(Serene)`
  - Include site.css in web project to avoid errors on publish `(Serene)`

### Bugfixes
  - Fixed Firebird dialect FIRST keyword naming and positioning

## 1.8.7 (2015-12-12)

### Features
  - Added ISqlDialect.QuoteIdentifier method and always quote column aliases with it as they might be reserved words
  
## 1.8.6 (2015-12-12)

### Bugfixes
  - Fixed assembly load error causing sergen.exe to fail to launch
  
## 1.8.5 (2015-12-12)

### Features
  - **`[Breaking Change]`** Changed enum-based dialect support with interface-based dialect system. Remove SqlSettings.CurrentDialect line from SiteInitialization or replace it with SqlSettings.DefaultDialect = SqlServer2012Dialect.Instance.
  - Added new PostgreSQL dialect and code generator support.
  - Serene and Northwind samples now work with PostgreSQL out of the box. See how-to guide.   
  - Added automatic bracketing with dialect-specific identifier quote replacement
  - **`[Breaking Change]`** Fields are selected with their property name if available, for better compatibility with Dapper. Shouldn't cause a problem if you didn't depend on prior functionality where field name was used.
  - Made capture log handler also log old rows, this resolves possible misidentification of update attribution to a user, in case a record is changed directly from database before.
  - Added Google reCAPTCHA widget and helpers
  - Removed SQLite package from code generator. If you need SQLite or another provider's support, register it in sergen.config.

## 1.8.4 (2015-12-01)

### Features
  - Added Excel export samples to product, customer and order grids `(Serene)`
  
## 1.8.3 (2015-11-30)

### Features
  - Added auto join with aliased fields and their view fields 
  - Cloned query has same parent as source query, so they share params 
  - Made fields with expressions read-only by default (was causing problems with backwards compatibility)
  - Removed alias with fields as no one uses or should use it anymore  
  - Set UI front to z-index 1100 instead of overriding z-index of AdminLTE divs `(Serene)`

### Bugfixes
  - Fixed required attribute in form not overriding one defined in row
  - Fixed update / insert permission attributes ignored, if specified explicitly along with modify permission
  - Fixed error 505 and login page links `(Serene)`

## 1.8.2 (2015-11-28)

### Features
  - It is now much easier to create cascaded editors using new CascadeFrom (editor ID), CascadeField (matching field) and CascadeValue (matching value) properties. Just set CascadeFrom and/or CascadeField properties.
  - Lookup editors can also be filtered using new FilterField and FilterValue properties, without need to define a new editor type.
  - Made permission and other user information like username available from client side, optionally `(Serene)`
  
## 1.8.1 (2015-11-28)

### Features
  - Updated SlimScroll, Pace and Simple Line Icons

## 1.8.0 (2015-11-28)

### Features
  - Allow comparison against empty value with StringFiltering, you can search for empty strings (not null) in filter dialog now
  - Better handling of FilteringIdField determination for textual fields with complex expressions
  - **`[Breaking Change]`** Cleanup Serenity LESS files, less dependency on jQuery UI CSS (Aristo.css), might cause minor problems, see upgrade info in Serenity guide
  - Q$Externals methods are moved under Q but Q$Externals is still an alias to Q, so no breaking changes expected
  - Added multi-level navigation support (up to 10 levels)
  - Used shorter namespace for types from other modules for T4 generated code
  - Handled type namespaces properly if T4 generated service endpoint uses types from other modules / namespaces
  - Integrated free AdminLTE theme `(Serene)`
  - Added AdminLTE sample pages `(Serene)`
  - Developed a MVC.tt file to generate strongly typed view locations (similar but much simpler than T4MVC)
  
### Bugfixes
  - Added IDateTimeProvider to fix Appveyor tests failing sometimes on time-related tests

## 1.7.0 (2015-11-20)

### Features
  - Added PropertyPanel and TemplatedPanel similar to PropertyDialog and TemplatedDialog (no need to use Panel attribute)
  - Added change password page `(Serene)`
  - Added forgot password and reset password page `(Serene)`
  - Template can now detect SQL LocalDB 2014 (VS2015) and set connection strings automatically `(Serene)`
  
### Bugfixes
  - Fixed validator hints not showing after TypeScript conversion


## 1.6.8 (2015-11-16)

### Features
  - Added grid row selection mixin


## 1.6.7 (2015-11-16)

### Bugfixes
  - Fixed critical regression bug in TemplatedWidget.GetTemplateName method, please update if you are using 1.6.4, 1.6.5 or 1.6.6!


## 1.6.6 (2015-11-15)

### Features
  - Converted file and image upload to behaviors that can be used in a row field
  - Added ability to generate multiple thumb sizes
  - Added better symbol for binary files in upload editors

  
## 1.6.5 (2015-11-15)

### Features
  - Don't raise on criteria join (and, or, xor) if one of criteria is null, assume empty
  - Added SetEquality extension for list request
  - Added SetEquality helper in DataGrid
  

## 1.6.4 (2015-11-14)

### Features
  - Added date range quick filtering
  - Added ability to register a permission key with RegisterPermissionKeyAttribute to be shown in permissions dialog `(Serene)`
  - Used a better datepicker trigger image


## 1.6.3 (2015-11-09)

### Features
  - Implemented behavior (mixin) functionality for service request handlers (list/retrieve/save/delete)
  - Extracted capture log, audit log, insert/update logs, unique constraint checks as behaviors
  - Added MasterDetailRelationAttribute and MasterDetailRelationBehavior 
  - If a field is auto created and has a foreign / calculated expression, it shouldn't be updatable / insertable by default
  - Added SingleField field type
  - Added ability to revoke a permission which is granted to roles from users `(Serene)`
  - Used tree mode for permission editing and displayed effective permission `(Serene)`

### Bugfixes
  - Fixed order tab in customer dialog was showing all orders in new customer mode `(Serene)`


## 1.6.2 (2015-10-31)

### Features
  - Code generator determines root namespace and script/web project locations at first run for Serene projects
  - Added a welcome page to Serene template


## 1.6.1 (2015-10-29)

### Features
  - Code generator will remove ID suffix when generating DisplayName attribute for foreign key fields.
  - **`[Breaking Change]`** Title for foreign ID fields are no longer determined by their TextualField. You may have to edit ID fields DisplayName attributes if you did depend on this feature. It was generally causing confusion when a developer changes ID field display name but it is not reflected in the form.


## 1.6.0 (2015-10-29)

### Features
  - When a toast is shown inside a dialog, then dialog is closed right away, toast is moved to next dialog or body instead of getting lost
  - Added a more compact imported format for service endpoints in client side
  - Code generator now produces code that is compatible (same location/contents) with T4 templates, so no need to remove that partial leftover class from XGrid.cs
  - Code generator sets TextualField attribute for foreign keys
  - Code generator also generates script side service and form objects
  - Added better error message if a row type is used with a LookupEditorAttribute, but it doesn't have a LookupScript


## 1.5.9 (2015-10-21)

### Bugfixes
  - Fixed FormContext.tt was failing after recent PropertyItemHelper changes
  - Fixed typo in serenity.texts.tr.json

  
## 1.5.8 (2015-10-20)

### Features
  - PropertyItemHelper is now extensible
  - 30% more compact property item scripts
  - Localized server side validation texts in DataValidation

### Bugfixes
  - Fixed client side Select2 validation error by downgrading to 3.5.1
  - Fixed qunit.js not found sometimes while running tests


## 1.5.7 (2015-10-14)

### Features
  - Added CascadedWidgetLink to make it easier to implement cascaded select editors
  - Added determine quick filter title using local texts automatically
  - Added decimal field better conversion handling
  - Added sample for order and detail (in-memory) editing and updating within a unit of work (transaction) `(Serene)`
  - Added provide tabbed interface to edit orders in customer dialog `(Serene)`
  - Added country / city cascaded editors and filtering sample `(Serene)`
  - Added better Sql LocalDB error handling `(Serene)`

### Bugfixes
  - Fixed validation message hint not showing after recent jQuery.validate update


## 1.5.6 (2015-10-08)

### Features
  - Added imported Google Maps using Script# import library at https://gmapsharp.codeplex.com/

### Bugfixes
  - Fixed typo in ScriptDtoGenerator that resolves lookup script reference generation for outside-row lookup scripts


## 1.5.5 (2015-09-25)

### Features
  - Generate script code from enum members in form / column definitions, even if they are not used in any row
  - Added integrated in-place item add / edit ability to LookupEditor  


## 1.5.4 (2015-09-24)

### Features
  - Changed columns namespace to App.Module.Columns
  - Updated razor generator
  - Changed textarea font size to make it consistent with other editors in Serene
  - disable browser link feature in Visual Studio for Serene


## 1.5.3 (2015-09-14)

### Features
  - Updated NuGet packages
  - Added Visual Studio 2015 support
  - If all localized fields are null, delete localization row
  - Introduced quick filters bar
  - Added quick search input no result animation
  - Added SqlConnections.NewFor
  - Added dictionary get default extension
  - Added ability to get database name from connection string
  - Added bracket database reference replacer that replaces `[DB^]` style references with catalog names before SQL execution

## 1.5.2 (2015-07-27)

### Features
  - Added support specifying a static ILocalCache and IDistributedCache provider where cache performance is critical (only use in non unit test environment)

## 1.5.1 (2015-07-27)

### Features
  - Allow ordering by row fields even if field is not used in query of ListRequestHandler.
  - Cache DB script hash in DBOverrides for even more performance in database tests
  - Added faster database copying in database tests
  - Number formatter default format is "0.##."
  - Form generator handles different namespaces better than before
  - Allow filtering of navigation items in NavigationHelper
  - Downgraded Select2 3.5.2 to 3.5.1 as 3.5.2 had problems with jQuery validate plugin
  - QuickSearchInput type delay default is now 500 ms (instead of 250 ms)
  - Added no UI blocking for Select2AjaxEditor
  - Made sure MultipleImageUploadEditor works properly

### Bugfixes
  - Fixed number formatter shows empty string for non-numeric values.
  - Fixed filter panel's not equal operator wasn't encoded properly

## 1.5.0 (2015-06-21)

### Features
  - Added better filter handling detection for textual fields that are connected to an ID field
  - Added sort filter fields by localized title
  - Added provide column formatters ability to initialize themselves and related column if required, and set referenced columns
  - Added TreeOrdering helper
  - Used closest anchor for edit link if target doesn't have the class (fixes problem when an anchor has inner elements)
  - Used ServiceEndpoint base URL from route attributes if GetServiceUrl is not specified for ScriptEndpointGenerator
  - Added members to IDataGrid interface to access grid, view, element and filter store objects
  - Put database schema name in brackets for generated code
  - Added date input automatic formatting for DMY cultures.
  - Added date editor two digit year parsing
  - Added debug text for SqlUpdate, SqlInsert, and SqlDelete like SqlSelect already had.
  - Added single / multiple field unique validation on save handler level with UniqueConstraint and Unique attributes.
  - Date time editor minValue and maxValue is set by default according to SQL server rules
  - Added URL option for PostToServiceOptions
  - Added better handling for non-row fields in GetFromReader. Their values can be read from DictionaryData.
  - Added StringHelper.Join method to put separator between two strings if only both are non-empty or null.
  - Added DateTimeKind attribute to set date type on datetime fields.
  - Added Checkbox readonly background in forms
  - Added Q.deepClone / Q.deepExtend methods that doesn't merge arrays (unlike jQuery extend)
  - Changed get required attribute from row first if available
  - Allow non-integer CultureId fields for localization rows
  - Introduced IIsActiveDeletedRow. IIsActiveRow doesn't have deleted state (-1) now
  - Added IDeleteLogRow for row types that store deleting user ID and date.
  - Added refresh button no text (hint only) option
  - Generate form key in form scripts
  - Added CSS class with full type name to widgets too (solves problem when two modules have same widget type)
  - Allow editing multiple localizations in one screen
  - Allow editing localizations before inserting row
  - Redirect to login page with denied flag, so login page can know difference between unauthenticated and permission denied requests
  - Show last jQuery selector in error console log if can't find widget on element in GetWidget
  - Dispose dialog if LoadByIdAndOpenDialog fails
  - Use jQuery UI dialogs own close button, as dialogExtend one was causing problems in tests and mobile.
  - Added execute-search trigger for quick search input (to avoid delays in tests)

### Bugfixes
  - Fixed code generator preserves source table field order as it was before
  - Fixed select editor empty option text fix
  - Fixed make sure toastr is shown only on visible dialogs
  - Fixed error handling for grid data source

## 1.4.14 (2015-04-14)

### Features
  - Added paging optimization for select2 ajax editor (no need to get total record count)
  - Added ability to generate formatter types server side, just like editor types
  - Define key constant for imported editor and formatter types
  - Define baseURL and method URLs for service endpoint and method imports
  - Added IsNaN extension for double and double?
  - Added IsValid property for decimal editor
  - Added date / datetime editor ValueAsDate property
  - Added phone editor auto formatting, extension support
  - Filter panel validation messages are hint only now
  - ScriptDtoGenerator creates reference to row lookup data and ID, name, local text prefix properties


## 1.4.13 (2015-04-07)

## 1.4.13 (2015-04-07)

### Features
  - Fixed DateTimeEditor time list when ToHour is not specified

## 1.4.12 (2015-04-06)

### Features
  - Added interval support to DateTimeEditor

## 1.4.11 (2015-03-28)

### Features
  - Added flush support to SqlLogger
  - Added Redis caching library using StackExchange.Redis
  - SQL case now supports field objects in when or value
  - MinimumResultForSearch option for select2 is now zero by default
  - Added generic DBTestContext objects
  - Connection key and database alias for DBScript can be specified with attributes
  - Connection count and list extensions without any criteria or query parameter
  - Added impersonating authorization service and transient granting permission service

## 1.4.10 (2015-02-27)

### Features
  - ListRequestHandler uses SortOrder attributes on row for GetNativeSort if available, name field otherwise
  - ScriptDtoGenerator, ScriptEndPointGenerator, ScriptFormGenerator constructors accept multiple assemblies
  - Column attribute is listed after DisplayName attribute by code generator to prevent user errors
  - **`[Breaking Change]`** Renamed HiddenAttribute to IgnoreAttribute and InvisibleAttribute to HiddenAttribute to prevent confusion

### Bugfixes
  - Fixed unable to deserialize enumeration fields due to Enum.IsDefined type incompatibility error

## 1.4.9 (2015-02-19)

### Features
  - Abstracted logging functions, added SqlLogger class to Serenity.Data
  - Added ability to expire cached databases and end transaction scope in database tests if data is modified in another thread / application
  - Updated Newtonsoft.Json to 6.0.8, jQuery to 2.1.3, FakeItEasy to 1.25.1
  - Used 32 bit Node instead of 64 for those who need to work on 32 bit Windows
  - Added TimeFormatter

### Bugfixes
  - Fixed problem with SQL 2012 dialect when skip > 0 and take = 0

## 1.4.8 (2015-02-09)

### Features
  - Allow object values in Permission attributes instead of string to be able to use Enums, Integers.
  - Added TimeEditor
  - Updated Newtonsoft.Json to 6.0.8, jQuery to 2.1.3, FakeItEasy to 1.25.1

### Bugfixes
  - Fixed filter panel dialog was showing multiple times sometimes

## 1.4.7 (2015-01-16)

### Features
  - Improved EmailEditor better copy paste
  - Added FileBrowserBrowseUrl for CKEditorOptions
  - Added Visible and Invisible attributes for columns
  - TryGetWidget and GetWidget works with non-widget types (e.g. interface or base class)
  - Include schema name in generated table names and foreign keys
  - Added HideCheckbox option for CheckListBox items

### Bugfixes
  - Fixed Readonly implementation for EmailEditor
  - Fixed DateTime editor CSS width fixed
  - Fixed editor filtering options
  - Fixed filter panel restore problem with parenthesis, and/or
