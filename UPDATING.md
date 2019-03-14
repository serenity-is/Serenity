# Updating Serenity

This document outlines steps required to update an existing Serene based project to the latest Serenity version.

If you are looking to update a StartSharp project please see this document (requires access to StartSharp repository):

https://github.com/volkanceylan/StartSharp/blob/master/docs/updating-startsharp.md

> ### Warning
>
> It is possible to upgrade your existing Serene based project, by updating its Serenity NuGet packages using guides here.
>
> But, some of our users ask us how to update their *existing project* to *latest Serene template*. The short answer to this question is *you can't*. 
>
>Your existing project which is created using Serene template is a modifiable copy. Thus, it can not be seamlessly updated to latest Serene version as that would mean losing your changes. 
>
>Of course, it doesn't mean you can't update your existing project to use latest Serene features, but it requires some manual and tedious work.
>
>If you would like to avoid any such work and have newest features / samples in Serene template easily, you would need to create a new project based on latest Serene template.
>
>To manually update your existing Serene based application by transferring code from a new Serene project, it is useful to create the new project with the same name as your existing one. This will make it easier to copy / merge changes between them.
>
>Even though we try to list steps you need to perform under [Upgrade Guides and Breaking Changes](#upgrade-guides-and-breaking-changes), it may not always work as expected. In that case, please create a new Serene project and try to transfer your existing custom code into the new project.

## Step 1 - Backup Existing Project

To be safe in case of an error during update process, first backup your project. You might ZIP your entire solution directory, or if you are using some source control system like Git, you may commit pending changes if any. 

> This step is optional, you may skip it at your own risk.

## Step 2 - Update Prerequisites

Applying latest Visual Studio patches, updating latest TypeScript / Node / NPM versions is usually a good idea.

We list the minimum requirements in this document:

https://github.com/volkanceylan/Serenity/blob/master/INSTALL.md

> Versions listed there might be lower than latest TypeScript / Node versions but more recent versions would probably work.

## Step 2 - Update Serenity NuGet Packages

### Applying Changes in Upgrade Guides

> While updating Serenity packages, there may be cases where you would need do some changes to your existing project based on your prior Serenity version. See [Upgrade Guides and Breaking Changes](#upgrade-guides-and-breaking-changes) section for more information.

Updating NuGet packages in a Serene project has some differences for ASP.NET MVC / ASP.NET Core.

### Serenity Update on ASP.NET MVC

Using package manager console, update following packages:

```
Update-Package Serenity.Web
Update-Package Serenity.CodeGenerator
Update-Package Serenity.Web.Tooling
```

Build and make sure that there are no errors / warnings.

### Serenity Update on ASP.NET Core

Using package manager console, update following package:

```
Update-Package Serenity.Web
```

Edit *YourProject.Web.csproj* by right clicking on project name in VS, and choosing edit.

Find these lines:

```
<PackageReference Include="Serenity.Web" Version="x.y.z" />
<PackageReference Include="Serenity.Web.AspNetCore" Version="..." />
<DotNetCliToolReference Include="Serenity.CodeGenerator" Version="..." />
```

Take a note of *Serenity.Web* package version (x.y.z), and change versions of
Serenity.Web.AspNetCore and Serenity.CodeGenerator to the same value.

## Step 3 - Restoring Static Content in .NET Core Version

If you are using ASP.NET Core version, open a command prompt in project directory (where YourProject.Web.csproj is) and type these commands:

```cmd
dotnet restore
dotnet sergen restore
```

*dotnet sergen restore* is critical as in .NET Core, NuGet can't install any static content, e.g. scripts, css, image etc into a .NET Core project. We worked around this by writing our own restore command. If you don't run this command, you'll have older versions of Serenity scripts.

## Step 4 - Transferring Samples / Features

So far we've updated NuGet packages. Some features are already contained in NuGet packages so you don't have to do anything other than looking at related sample source code and applying similar code lines to your project.

But if you need a new sample or feature that is contained directly in Serene, it might need a bit extra work.

You would need to copy / paste a sample page / module and related files from a new Serene project to your existing project.

In some cases, samples require some extra *CSS* too. In that case, transfer changes in *site.theme.less*, *site.basicsamples.less* etc. files to the same file in your project.

A sample might also require additional scripts as well. Check *Scripts* folder, and *ScriptBundles.json* to see if a sample has extra entries there. Add missing entries to your project and copy relevant files.

[Upgrade Guides and Breaking Changes](#upgrade-guides-and-breaking-changes) might sometimes contain useful information on what you should do to enable a new feature / sample.

## Step 5 - Rebuild and Check for Errors

Build and make sure that there are no errors / warnings.

## Upgrade Guides and Breaking Changes

We try to keep Serenity / Serene backward compatible as much as possible. 

In rare cases where we can't avoid breaking changes, we list them here so that you can safely upgrade your existing project. 

You should apply changes listed here *for versions greater than* your existing one in order from *lowest to highest*.

For example, if your existing Serenity / Serene version is 3.5.0 and updating to 3.9.0, then you should apply upgrade guide for 3.6.0, 3.7.5, 3.8.4 etc. if any in increasing order.

## Serenity 3.6.0

Serenity 3.6.0 comes with React integration but it is currently activated in StartSharp only. Once you update to Serenity 3.6.0, you may get some errors about React, as Serenity.CoreLib scripts has references to it.

```txt
Cannot find type definition file for 'react'.
Cannot find name 'React'.
...
```

> Serenity itself doesn't require React, it is just a TypeScript warning, so, you don't actually need to include React scripts in your site, just need typings.

To resolve such errors, you'll need to include typing for React in your package.json:

```json
  "dependencies": {
    //...
    "@types/react": "^16.1.0",
  }
```

After that save the file and make sure there is a **react** folder at *YourProject.Web\node_modules\@types*. If not, you might have some configuration error with NodeJS integration in Visual Studio.^

If you are still seeing warnings in Visual Studio errors tab about React, you might need to restart Visual Studio.

## Serenity 3.8.0

### Important Security Notice!

During testing we found out that handlers defined in your web.config files also applies to sub folders. This might lead to some problems / security issues, especially for error log. Please put a slash in PATH attribute of these handlers. E.g. if you have these in your web.config:

> This only applies to Asp.Net MVC version, not .NET Core version.

```xml
<handlers>
<add name="ErrorLog" path="errorlog.axd" verb="POST,GET,HEAD" 
  type="StackExchange.Exceptional.HandlerFactory, StackExchange.Exceptional" preCondition="integratedMode" />
<add name="DynamicScriptHandler" verb="POST,GET,HEAD" path="DynJS.axd" 
  type="Serenity.Web.HttpHandlers.DynamicScriptHandler, Serenity.Web" />
<add name="SkipStaticFileForUploadFolder" verb="GET" path="upload/*"   
  type="System.Web.Handlers.TransferRequestHandler"/>    
</handlers>
```

Replace them with

```xml
<handlers>
  <add name="ErrorLog" path="/errorlog.axd" verb="POST,GET,HEAD"   
    type="StackExchange.Exceptional.HandlerFactory, StackExchange.Exceptional" preCondition="integratedMode" />
  <add name="DynamicScriptHandler" verb="POST,GET,HEAD" path="/DynJS.axd" 
    type="Serenity.Web.HttpHandlers.DynamicScriptHandler, Serenity.Web" />
  <add name="SkipStaticFileForUploadFolder" verb="GET" path="/upload/*"    
    type="System.Web.Handlers.TransferRequestHandler"/>    
</handlers>
```

So that these handlers only apply to web root folder.

## Serenity 3.8.2

After you upgrade from < 3.8.2 version to 3.8.2+, you might lose DynamicScriptHandler entry in your web.config file and start to get script errors.

> We had to this change as NuGet creates duplicate handler entries in web.config after doing modifications to handlers sections as shown in 3.8.0 upgrade information.

> This only applies to Asp.Net MVC version, not .NET Core version.

Make sure you have following DynamicScriptHandler entry in your web.config:

```xml
<handlers>
    //...
    <add name="DynamicScriptHandler" verb="POST,GET,HEAD" path="DynJS.axd" 
       type="Serenity.Web.HttpHandlers.DynamicScriptHandler, Serenity.Web" />
    //...
</handlers>
```

## Serenity 3.8.5

This is not actually an issue about Serenity itself, but you might have this problem if you choose to update Microsoft.TypeScript.MsBuild in your Serene/StartSharp project.

TypeScript no longer has a tsc.exe file and it uses NodeJS instead of Chakra (Edge) so you need to update your .CSPROJ file, find CompileTSC section in your project file and replace it like below:

```xml
<Exec Command="&quot;$(NodePath)\node&quot; &quot;$(TSJavaScriptFile.Replace('build\\..\tools\', 'tools\'))&quot; -p ./tsconfig.json" ContinueOnError="true" />
```