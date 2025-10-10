# CHANGELOG 2014

This changelog documents all Serenity versions published in the year 2014 (versions 1.3.0 through 1.4.6).

## 1.4.6 (2014-12-11)

### Features
  - Allow specifying a target for navigation links
  - Added false and true criteria constants
  - Build target files for those who use Serenity as a submodule

### Bugfixes
  - Regression bug in 1.4.1 that causes cache invalidation to fail for lookups/dynamic scripts

## 1.4.5 (2014-11-28)

### Features
  - Fields can be used directly like criteria objects in queries (e.g., p.Id == k.ProductId)
  - In criteria doesn't use parameters for longer than 10 numeric values
  - ConstantCriteria object for values that shouldn't be converted to parameters in generated SQL
  - WithNoLock moved to extensions class
  - Flexify, Maximizable, Resizable attributes for dialog classes
  - Default order for grids is now empty to let the service side decide on the initial sort order
  - **`[Breaking Change]`** Renamed Field._ property to Field.Criteria as it felt unnatural

## 1.4.4 (2014-11-23)

### Features
  - Enabled image uploads and added sample to Products form in Serene

## 1.4.3 (2014-11-23)

### Features
  - New documentation site with indexes on the left (generated with GitBook)
  - Introduced IAlias interface, that is implemented by Alias and RowFieldsBase
  - Added ability to replace t0 aliases with something else for RowFieldsBase objects by .As("x") method (only table fields)
  - AliasName property in RowFieldsBase
  - Better naming for all capital and dashed field names in Serenity code generator
  - Safety checks in GetFromReader method to ensure a field from another row is not used to load entity values
  - SQL query select with field can specify alternate column name
  - Criteria objects can be created from fields with _ shortcut (e.g., UserRow.Fields.UserId._ > 5 instead of new Criteria(UserRow.Fields.Personnel.UserId) > 5)
  - Added better handling for Criteria.In and Criteria.NotIn when an IEnumerable or array is passed
  - Overload for Sql.Coalesce that uses parameters and supports criteria, field, and query objects

## 1.4.2 (2014-11-19)

### Features
  - Updated Bootstrap to 3.3.0, jQuery.UI to 1.11.2, FakeItEasy to 1.25.0, Toastr to 2.1.0
  - Allow specifying default sort order for datagrid columns on server/client side (SortOrder attribute, use negative values for descending sort)
  - Added theme selection option to Serene (only a light theme for now)

### Bugfixes
  - Fixed double HTML encoding with EditLinks (& was shown as &amp;)
  - Fixed script form generator output when full editor namespace is used
  - ServiceAuthorizeAttribute now returns 400 as HTML status code
  - Null reference exception when related ID field for a foreign field can't be determined in PropertyItemHelper
  - Fixed typo in authorization error message for DbLookupScript (it is going to be obsolete, avoid using it)

## 1.4.1 (2014-11-13)

### Features
  - Added Date and Enumeration filterings
  - Added jQuery UI datepicker localization
  - JsonLocalTextRegistration and NestedLocalTextRegistration tests
  - Refactored dynamic scripts
  - It is now possible to define a remote data script on an MVC action
  - Indentation option for Json.StringifyIndented
  - Added roles, role permissions, user permissions tables and interface to Serene sample application

### Bugfixes
  - Filter panel effective filter text is now in English

## 1.4.0 (2014-11-08)

### Features
  - Introduced FilterPanel widget, for advanced customizable filtering with grouping, and/or, lookup editors, etc. Add `[Filterable]` attribute to your grid class to enable
  - TemplateBundle to send templates to client side on page load by including it as a script
  - ColumnsBundle to send column metadata to client side on page load by including it as a script
  - FormsBundle to send column metadata to client side on page load by including it as a script
  - Widget templates no longer loaded asynchronously (it caused more problems than performance bonuses, use TemplateBundle if needed)
  - LookupEditor is not async, use AsyncLookupEditor if required
  - CodeGenerator doesn't produce Grid/Dialog as IAsyncInit by default
  - Added LocalText tests and documentation
  - Updated Newtonsoft.JSON to 6.0.6, PhantomJS to 1.9.7, Select2 to 3.5.1

## 1.3.8 (2014-11-04)

### Bugfixes
  - Fixed edit link problem when grid is not async and columns loaded from server side
  - Button noConflict fix

## 1.3.7 (2014-11-04)

### Features
  - Basic Application Sample is now Serene in its own repository
  - Serene uses FluentMigrator package to create and upgrade sample database
  - SqlConnections.GetConnectionString returns ConnectionStringInfo object instead of Tuple

## 1.3.6 (2014-11-01)

### Features
  - Basic application has a language dropdown and preferred language can be changed using it (no need to play with browser settings or web.config)
  - Basic application has an interface to localize all texts in any defined language (stores files in ~/scripts/site/texts/user.texts.{languageID}.json)
  - Added Spanish translations for Serenity and basic application sample (Google Translate)
  - Added optional LanguageID and Prefix properties to NestedLocalTextsAttribute
  - Use simple sorted dictionary format for localization JSON files instead of hierarchical (easier to manage and merge)
  - Defined and used InitializedLocalText class for NestedLocalTextRegistration and EntityLocalTexts classes to avoid duplicate or invalid registrations when their Initialize methods are called twice
  - Translated filter panel texts to English
  - **`[Breaking Change]`** Moved NestedLocalTextRegistration.AddFromScripts to JsonLocalTextRegistration.AddFromJsonFiles
  - **`[Breaking Change]`** ILocalTextRegistry.TryGet now takes a language ID parameter (was using CultureInfo.CurrentUICulture.Name before)

### Bugfixes
  - Set enum key attribute for CustomFieldType enumeration

## 1.3.5 (2014-10-30)

### Features
  - Make form categories sorted by their item order in form definition (no need for CategoryOrder anymore)
  - Ability to localize form category titles (e.g., Forms.Northwind.Customer.Categories.General)
  - **`[Breaking Change]`** LocalTextRegistry.SetLanguageParent is changed to SetLanguageFallback
  - Make LocalText.Empty read-only

## 1.3.4 (2014-10-29)

### Bugfixes
  - Make FormatterType constructor public
  - Promise.Then method should return a Promise without value

## 1.3.3 (2014-10-28)

### Bugfixes
  - Fixed default location of select2.js in CommonIncludes

## 1.3.2 (2014-10-28)

### Features
  - CommonIncludes class for easier script/CSS includes
  - Support {version} variable in script include statements (e.g., jquery-{version})
  - Editors can be referenced with full namespace from server side (e.g., Serenity.StringEditor). EditorTypes.tt now uses full namespace to avoid confusion
  - Embedded Bootstrap/jQuery UI button conflict resolution to Serenity.Externals.js
  - Added tests and XML doc comments for service locator, configuration, authorization, caching features
  - TwoLevelCache now has a Get overload that takes only one expiration instead of two
  - **`[Breaking Change]`** Moved ILocalTextRegistry interface to Serenity.Abstraction for consistency
  - **`[Breaking Change]`** Renamed ApplicationConfigurationRepository to AppSettingsJsonConfigRepository and moved it to Serenity.Data (from Serenity.Web)
  - **`[Breaking Change]`** Removed unused DevelopmentSettings and used ASP.NET settings like compilation debug=true, customErrors, etc.
  - **`[Breaking Change]`** Distributed.Set now takes a TimeSpan instead of DateTime, for consistency with LocalCache class

### Bugfixes
  - Fixed an interop problem with Glimpse for service endpoints (ServiceModelBinder should be registered)
  - PropertyGrid is no longer an async initialized widget. It will be async widget's own responsibility to attach to another async widget
  - EntityConnectionExtension First and Single methods no longer require an entity with IIdRow interface

## 1.3.1 (2014-10-14)

### Bugfixes
  - Dialog fills page correctly when maximized (breaking change in jQuery UI 1.11)
  - Make select editor non-abstract. Required for enums to work

## 1.3.0 (2014-10-09)

### Features
  - Added ability to define grid columns server side (just like form definitions)
  - Extensible column formatter objects (string, number, enum, date, etc.)
  - Reduced number of attributes and repetitive code required in service endpoints (JsonFilter, Result<T>, etc.) through new ServiceEndpoint base class
  - New EnumKey attribute for enum types so same enum names in different namespaces don't get mixed
  - Async versions of Q.GetLookup, Q.GetForm, etc.
  - Included promise library for async operations (make sure your LayoutHead.cshtml contains ~/Scripts/rsvp.js before jQuery)
  - Widgets that do async initialization. TemplatedWidget, EntityDialog, DataGrid, etc. support this new system.
    To provide backward compatibility, this feature is only enabled by adding IAsyncInit interface to a widget class
  - Allow criteria objects to be used on client side too (with some limitations to prevent SQL injections, etc.)
  - List requests can now specify filtering through client-side criteria
  - Generate field names with entity classes in ServiceContracts.tt
  - Merged ServiceContracts.tt and ServiceEndpoints.tt (refer to Serene sample to update your own ServiceContracts.tt file)
  - Upgraded to Saltarelle Compiler 2.6.0
  - Upgraded to jQuery UI 1.11.1, jQuery Validation 1.13, FontAwesome 4.2, Bootstrap 3.2

### Bugfixes
  - Enum editors use integer keys instead of string keys
  - Fixed SlickGrid column sort indicators in Firefox
  