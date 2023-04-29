# Serenity Core Library

This is the package containing core TypeScript classes and functions used in Serenity applications.

The main entry for the NPM package is `@serenity-is/corelib`. This should be installed by default 
in your projects created from `Serene` or `StartSharp` template:

```json
{
  "dependencies": {
    // ...
    "@serenity-is/corelib": "6.7.1"
  }
}
```

The version number for this package should be equal or as close as possible to Serenity NuGet package versions in your project file.

`@serenity-is/corelib/q` and `@serenity-is/corelib/slick` are not separately installable NPM packages. They are just sub-modules exported via `subpath exports` feature of the Node/NPM.
