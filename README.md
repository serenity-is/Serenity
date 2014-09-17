Serenity Application Platform
=============================

## What is Serenity Platform

Serenity is a Javascript / .NET application platform which has been built on open source technologies. 

It aims to make development easier while reducing maintenance costs by avoiding boiler-plate code, reducing the time spent on repetitive tasks and applying best software design practices. 

## Quick Start

The fastest way to get your hands dirty on Serenity is by using a sample application template from Visual Studio Gallery. 

Just go to http://visualstudiogallery.msdn.microsoft.com/ and search for *Serenity Basic Application Sample*.

You can also install it directly from Visual Studio, by entering File -> New -> Project -> Online and searching for *Serenity*.

The default application sample has two projects (common for Serenity applications). One for server side (BasicApplication.Web) and another one for script side (BasicApplication.Script). The script project looks like a .NET project but it actually generates javascript code (using Saltarelle Compiler). 

Web project is an ordinary MVC application. 

Both projects have references to Serenity NuGet packages, so you can update them using package manager console any time you need.

Basic Application Sample automatically creates its database in SQL local db, so just press F5 and you are ready to go.

When application launches use `admin` user and `serenity` password to login. You can create more users from *Administration / User Management* section.

The sample application includes old and famous Nortwind data along with services and user interface for editing it that is mostly produced by Serenity Code Generator.

You can take a look at the code for this application while more documentation is on the way...

## What's In The Name

Serenity has dictionary meanings of *peace*, *comfort* and *calmness*. 

This is what we are trying to achieve with Serenity. We hope that after installing and using it you will feel this way too...

## Who/What This Platform Is For
Serenity is best suited to business applications with many data entry forms or administrative interface of public facing web sites. 

This doesn't mean that it can't be used for other types of applications. Some features like script versioning/bundling, caching, code generation, fluent sql builders etc. can be helpful for any kind of application.

## What Features Does It Provide

* A modular, service based web application model
* Code generator to produce initial services / user interface code for an SQL table
* T4 based code generation on server to reference script widgets with intellisense / compile time validation
* T4 based code generation to provide compile time type safety and intellisense while calling AJAX services from script side.
* An attribute based form definition system (prepare UI in server side with a simple C# class)
* Automatic seamless data-binding through form definitions (form <-> entity <-> service).
* Caching Helpers (Local / Distributed) 
* Automatic cache validation
* Configuration System (storage medium independent. store settings in database, file, whatever...)
* Simple Logging
* Reporting (reports just provide data, has no dependency on rendering, similar to MVC)
* Script bundling, minification (making use of Node / UglifyJS / CleanCSS) and content versioning (no more F5 / clear browser cache)
* Fluent SQL Builder (SELECT/INSERT/UPDATE/DELETE)
* Micro ORM (also Dapper is integrated)
* Customizable handlers for REST like services that work by reusing information in entity classes and do automatic validation.
* Attribute based navigation menu
* UI Localization (store localized texts in json files, embedded resource, database, in memory class, anywhere)
* Data Localization (using an extension table mechanism helps to localize even data entered by users,  like lookup tables)
* Script widget system (inspired by jQueryUI but more suitable for C# code)
* Client side and server side validation (based on jQuery validate plugin, but abstracts dependency)
* Audit logging (where CDC is not available)
* System for data based integration tests
* Dynamic scripts
* Script side templates

## Used Open Source Tools and Libraries

Serenity platform makes use of some valuable open source tools and libraries that are listed below (in alphabetic order):

* Autonumeric (https://github.com/BobKnothe/autoNumeric)
* BlockUI (https://github.com/malsup/blockui/)
* Bootstrap (https://github.com/twbs/bootstrap)
* Cake Build (https://github.com/cake-build/cake)
* Cecil (https://github.com/jbevain/cecil)
* Clean-CSS [Node] (https://github.com/jakubpawlowicz/clean-css)
* Colorbox (https://github.com/jackmoore/colorbox)
* Dapper (https://github.com/StackExchange/dapper-dot-net)
* DialogExtend (https://github.com/ROMB/jquery-dialogextend)
* jLayout (https://github.com/bramstein/jlayout)
* Json.NET (https://github.com/JamesNK/Newtonsoft.Json)
* JSON2 (https://github.com/douglascrockford/JSON-js)
* JSRender (https://github.com/BorisMoore/jsrender)
* jQuery (https://github.com/jquery/jquery)
* jQuery Cookie (https://github.com/carhartl/jquery-cookie)
* jQuery Validation (https://github.com/jzaefferer/jquery-validation)
* jQuery UI (https://github.com/jquery/jquery-ui)
* jQuery.event.drag (http://threedubmedia.com/code/event/drag)
* Less.JS (Node) (https://github.com/less/less.js)
* Linq.js (http://linqjs.codeplex.com/)
* metisMenu (https://github.com/onokumus/metisMenu)
* Munq (https://munq.codeplex.com/)
* NodeJS (https://github.com/joyent/node)
* Pace (https://github.com/HubSpot/pace)
* PhantomJS (https://github.com/ariya/phantomjs)
* RazorGenerator (https://razorgenerator.codeplex.com/)
* Saltarelle Compiler (https://github.com/erik-kallen/SaltarelleCompiler)
* Select2 (https://github.com/ivaynberg/select2)
* SlickGrid (https://github.com/mleibman/SlickGrid)
* Toastr (https://github.com/CodeSeven/toastr)
* UglifyJS2 (Node) (https://github.com/mishoo/UglifyJS2)
* XUnit (https://github.com/xunit/xunit)
 
This list might seem a bit long, but not all of them are direct dependencies for a Serenity Application. 

Some of them are only used during development of Serenity platform itself, while some are dependencies for optional features. 

We tried to reuse open source libraries, where there is a quality one available to avoid reinventing the wheel.