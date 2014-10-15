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

## Documentation


See [Serenity Guide](https://github.com/volkanceylan/SerenityGuide/blob/master/SUMMARY.md) for documentation.

* [https://github.com/volkanceylan/SerenityGuide/blob/master/SUMMARY.md](https://github.com/volkanceylan/SerenityGuide/blob/master/SUMMARY.md)



