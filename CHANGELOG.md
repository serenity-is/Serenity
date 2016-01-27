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
  - added sign up sample [Serene]

Bugfixes:
  - fix file upload control look after theme change
  - change Notes permission to Northwind:General from Administration [Serene]

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
  - add notes editor, notes behavior and tab to Customer dialog [Serene]

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
  - sample template supports MySql [Serene]

## 1.8.12 (2015-12-17)

Features:
  - added Throttler class, to throttle logins, logging etc.
  - added LDAP / Active Directory authentication samples [Serene]

## 1.8.11 (2015-12-16)

Features:
  - use titleize to set default table indentifier in sergen
  - int16 (short) types sets maximum value to 32767 properly
  - user translations are saved under ~/App_Data/texts instead of ~/scripts/site/texts to avoid permission errors [Serene]

Bugfixes:
  - clone mode should handled by dialog itself as some records might have negative IDs.
  - fix error with firebird primary keys in sergen
  - set column size properly for firebird string columns in sergen

## 1.8.9 (2015-12-15)

Features:
  - sergen can generate code for firebird databases (no schema support yet)
  - made top row of numbers in dashboard populate from database (cached), made more info buttons go to relevant pages [Serene]
  
Bugfixes:
  - when table schema is null generated row code has syntax error for foreign keys, regression bug from 1.8.5
  - sort indicator in grids was showing wrong direction

## 1.8.8 (2015-12-14)

Features:
  - better error message when Kdiff3 couldn't be located by sergen.exe.
  - use a large buffer for hashing
  - ability to set per connection dialect with an application setting (ConnectionSettings key)
  - show a warning explaining why migrations are skipped, and how to enable them [Serene]
  - include site.css in web project to avoid errors on publish [Serene]

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
  - [BREAKING CHANGE] changed enum based dialect support with interface based dialect system. remove SqlSettings.CurrentDialect line from SiteInitialization or replace it with SqlSettings.DefaultDialect = SqlServer2012Dialect.Instance.
  - new PostgreSQL dialect and code generator support.
  - serene and northwind sample now works with PostgreSQL out of the box. see how to guide.   
  - automatic bracket with dialect specific identifier quote replacement
  - [BREAKING CHANGE] fields are selected with their property name if available, for better compability with Dapper. shouldn't cause a problem if you didn't depend on prior funtionality where field name was used.
  - made capture log handler also log old rows, this resolves possible misidentified update attribution to a user, in case a record is changed directly from database before.
  - added Google Recaptcha widget and helpers
  - removed sqlite package from code generator. if you need sqlite or another provider support, register it in sergen.config.

## 1.8.4 (2015-12-01)

Features:
  - added excel export samples to product, customer and order grids [Serene]
  
## 1.8.3 (2015-11-30)

Features:
  - auto join with aliased fields and their view fields 
  - cloned query has same parent as source query, so they share params 
  - undo fields with expressions be read only by default (was causing problems with backwards compability)
  - remove alias with fields as noone uses or should use it anymore  
  - set ui-front to z-index 1100 instead of overriding z-index of AdminLTE divs [Serene]

Bugfixes:
  - fix required attribute in form not overriding one defined in row
  - fix update / insert permission attributes ignored, if specified explicitly along with modify permission
  - fix error 505 and login page links [Serene]

## 1.8.2 (2015-11-28)
  - it is now much easier to create cascaded editors using new CascadeFrom (editor ID), CascadeField (matching field) and CascadeValue (matching value) properties. Just set CascadeFrom and/or CascadeField properties.
  - lookup editors can also be filtered using new FilterField and FilterValue properties, without need to define a new editor type.
  - made permission and other user information like username available from client side, optionally [Serene]
  
## 1.8.1 (2015-11-28)

Features:
  - update slimscroll, pace and simple line icons

## 1.8.0 (2015-11-28)

Features:
  - allow comparison against empty value with StringFiltering, you can search for empty strings (not null) in filter dialog now
  - better handling of FilteringIdField determination for textual fields with complex expressions
  - [BREAKING CHANGE] cleanup serenity less files, less dependency on jQuery UI css (aristo.css), might cause minor problems, see upgrade info in Serenity guide
  - Q$Externals methods are moved under Q but Q$Externals is still an alias to Q, so no breaking changes expected
  - multi level navigation support (up to 10 levels)
  - use shorter namespace for types from other modules for T4 generated code
  - handle type namespaces properly if T4 generated service endpoint uses types from other modules / namespaces
  - integrate free AdminLTE theme [Serene]
  - add AdminLTE sample pages [Serene]
  - develop a MVC.tt file to generate strongly typed view locations (similar but much simpler than T4MVC)
  
Bugfixes:
  - IDateTimeProvider to fix Appveyor tests failing sometimes on time related tests

## 1.7.0 (2015-11-20)

Features:
  - added PropertyPanel and TemplatedPanel similar to PropertyDialog and TemplatedDialog (no need to use Panel attribute)
  - change password page [Serene]
  - forgot password and reset password page [Serene]
  - template can now detect Sql LocalDB 2014 (VS2015) and set connection strings automatically [Serene]
  
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
  - ability to register a permission key with RegisterPermissionKeyAttribute to be shown in permissions dialog [SERENE]
  - use a better datepicker trigger image


## 1.6.3 (2015-11-09)

Features:
  - implement behavior (mixin) functionality for service request handlers (list/retrieve/save/delete)
  - extract capturelog, auditlog, insert/update log, unique constraint checks as behaviors
  - MasterDetailRelationAttribute and MasterDetailRelationRelationBehavior 
  - if a field is auto created and has a foreign / calculated expression, it shouldn't be updatable / insertable by default
  - SingleField field type
  - ability to revoke a permission which is granted to roles from users [SERENE]
  - use tree mode for permission editing and display effective permission [SERENE]

Bugfixes:
  - order tab in customer dialog was showing all orders in new customer mode [SERENE]


## 1.6.2 (2015-10-31)

Features:
  - code generator determines root namespace and script/web project locations at first run for Serene projects
  - added a welcome page to Serene template


## 1.6.1 (2015-10-29)

Features:
  - code generator will remove ID suffix when generating DisplayName attribute for foreign key fields.
  - [BREAKING CHANGE] title for foreign ID fields are no longer determined by their TextualField. you may have to edit ID fields DisplayName attributes if you did depend on this feature. it was generally causing confusion when a developer changes ID field display name but it is not reflected in the form.


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
  - sample for order and detail (in-memory) editing and updating within a unit of work (transaction) [Serene]
  - provide tabbed interface to edit orders in customer dialog [Serene]
  - country / city cascaded editors and filtering sample [Serene]
  - better Sql LocalDB error handling [Serene]

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
  - bracket database reference replacer that replaces [DB^] style references with catalog names before sql execution

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
  - [BREAKING CHANGE] Renamed HiddenAttribute to IgnoreAttribute and InvisibleAttribute to HiddenAttribute to prevent confusion

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
  - [BREAKING CHANGE] Rename Field._ property to Field.Criteria as it felt unnatural.

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
  - introduced FilterPanel widget, for advanced customizable filtering with grouping, and/or, lookup editors etc. Add [Filterable] attribute to your grid class to enable.
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
  - [BREAKING CHANGE] moved NestedLocalTextRegistration.AddFromScripts To JsonLocalTextRegistration.AddFromJsonFiles
  - [BREAKING CHANGE] ILocalTextRegistry.TryGet now takes a language ID parameter (was using CultureInfo.CurrentUICulture.Name before)

Bugfixes:
  - Set enum key attribute for CustomFieldType enumeration

## 1.3.5 (2014-10-30)

Features:
  - make form categories sorted by their item order in form definition (no need for CategoryOrder anymore)
  - ability to localize form category titles (e.g. Forms.Northwind.Customer.Categories.General)
  - [BREAKING CHANGE] LocalTextRegistry.SetLanguageParent is changed to SetLanguageFallback
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
  - [BREAKING CHANGE] moved ILocalTextRegistry interface to Serenity.Abstraction for consistency
  - [BREAKING CHANGE] renamed ApplicationConfigurationRepository to AppSettingsJsonConfigRepository and moved it to Serenity.Data (from Serenity.Web)
  - [BREAKING CHANGE] removed unused DevelopmentSettings and used ASP.NET settings like compilation debug=true,
    customErrors etc.
  - [BREAKING CHANGE] Distributed.Set now takes a TimeSpan instead of DateTime, for consistency with LocalCache class.

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
