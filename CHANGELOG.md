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
