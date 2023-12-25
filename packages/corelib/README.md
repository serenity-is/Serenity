# Serenity Core Library (Classic UI)

This is the package containing core TypeScript classes and functions used in Serenity applications.

The main entry for the NPM package is `@serenity-is/corelib`. This should be installed by default 
in your projects created from `Serene` or `StartSharp` template:

```json
{
  "dependencies": {
    // ...
    "@serenity-is/corelib": "./node_modules/.dotnet/serenity.corelib"
  }
}
```

If you have "./node_modules/.dotnet/serenity.corelib" in the version value, it means you are using this library directly via the Serenity.Corelib NuGet package reference in your project file instead of NPM. This is recommended to avoid version inconsistencies.

If you have a version number for this package in package.json, it should be equal or as close as possible to Serenity NuGet package versions in your project file.