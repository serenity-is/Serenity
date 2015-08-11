## 1.5.2 (2015-07-27)
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
