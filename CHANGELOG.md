## 10.7.0 (2026-11-17)

### Features
- Ship `Serenity.Pro.Dashboards` (Enterprise): SQL and JSON data sources with datasets, parameters and list service sources, a SQL entity catalog per connection with joins and schema-aware SQL editing, access rules with object and column-level permissions, regex object filters and row limits.
- Dashboards offer configurable widgets (charts with configurable series and trends, number cards with server aggregates, tables with virtual or infinite scroll, gauge, funnel, list, activity, text and links) and a designer that can also edit widget properties and table columns from the grid header. Dashboard, menu and widget texts are localizable.
- Dashboard filters and parameters are passed through the page address, dataset parameters are resolved per widget, widget errors are isolated in place, dashboards are versioned with save conflict handling, and they support templates with browse-page previews, per-dashboard navigation icons and a per-user default dashboard. The StartSharp template gets `DataSources` / `SqlDataConnector` settings with a `Dashboards` template option enabled by default (StartSharp, Enterprise).
- Add aggregate, group-by and entity-exclusion options to list requests. `ListRequest` gains `Aggregates`, `GroupBy`, `ExcludeEntities` and `TimeZone`, `ListResponse` returns `Aggregates`, and handlers declare the new `Aggregates`, `GroupBy`, `ExcludeEntities` and `TimeZone` capabilities, combined in `AggregateOptions` and named `ListHandler` / `ProListHandler` sets (`SourceAdapter` supports them too).
- Add `AggregateBy` / `AggregateGroup`, which describe values calculated from all the rows a request matches, so grids can show totals without loading every row, and let `GroupBy` group date fields by a `DateGroupInterval` (year, quarter, month, week, day, hour) whose interval starts follow the request's `TimeZone`. `ExcludeEntities` lets a caller ask only for the count and the aggregates. `SummaryType` and `AggregateType` members now have localized texts.
- Teach `RemoteView` (`serverAggregates`, also exposed in `DataGrid` defaults so grids can opt in or override it in `getViewOptions`) to request grand and per-group totals from the server, falling back to the loaded page rows when the server does not return them or a local filter hides rows, ordering rows by the grouping fields in the view's sort order so rows of a group stay together, and reloading only when aggregates, groupings or sort change. Add `IListResponseTotalCount` so a behavior that already counted rows can supply the count and avoid a second count.
- The base `ListRequestHandler` only honors `ExcludeEntities` (it reads a single row, clears the entities and returns just the count, without calculating aggregates); the aggregates, grouping and time zone options are filled in by the `ListAggregatesBehavior` of Serenity.Pro.Extensions or by a list source adapter that supports them, so Serene projects without pro features ignore those request options and grids keep calculating totals from the loaded page.
- Add `ListAggregatesBehavior` to Pro.Extensions, which fills the aggregates and groups that the core handlers don't, from the whole filtered result set, validating aggregate and group fields against field permissions (dropping grouping when a group field is not allowed and skipping sums/averages over non-numeric fields), capping each group level at `MaxGroupsPerLevel` with `AggregateGroup.More`, and setting `TotalCount` / `More` from the totals query so rows are not counted twice.
- Group all levels with a single `ROLLUP` query on SQL Server, PostgreSQL, Oracle and MySQL (per-level queries elsewhere), translate date intervals with dialect-specific expressions that start weeks on Monday, and mirror the same aggregates, groups and intervals in the in-memory list source adapter when the target does not support them. StartSharp grids request server aggregates by default (StartSharp).
- Unify server and client validation rules so a property's constraints are enforced both in the browser and in save handlers. Add `ValidateRange`, `ValidateLength`, `ValidatePattern`, `ValidateEmail`, `ValidateUrl` and `ValidateCompare` attributes that also describe themselves to the client (`IClientValidationRule`, messages localizable by an implied key like `Quantity_ValidateRange`), send them as `PropertyItem.validationRules` and let `PropertyGrid` attach them through `Validator.addCustomRule`, skipping read-only and hidden fields.
- Add a `ValidationRuleAttribute` base class with built-in rules and a `ValidationRuleTypeRegistry`, so custom rules are found by their type key and can be loaded on demand with `LazyLoadInclude` (or registered by a plain function). The server typings generator imports the rule classes into the forms that use them, and the missing `Validation.Compare`, `Validation.Invalid`, `Validation.Pattern` and `Validation.RangeLength` texts were added to all locales.
- Add `ValidationRules` / `ValidateObject` and `FormValidationAttribute`, so save handlers validate a row by a form's rules, including `RequiredWhen` and async rules (`IAsyncValidationRule`), with a service provider available to rules so they can reach the connection and the localizer.
- **[Breaking Change]** `ICustomValidator`, `IValidationContext` / `ValidationContext` and `RowValidationContext` are replaced by the DataAnnotations-based `ObjectValidator`; migrate custom validators to `ValidationRuleAttribute` (with a client-side rule class for the forms) or plain `ValidationAttribute`s. Projects also get global using aliases so unqualified `[Required]`, `[MaxLength]` and `[DisplayFormat]` in application code bind to the Serenity attributes.
- Serve project and package `wwwroot` files through Serenity static assets instead of the .NET static web assets SDK. Add `SerenityStaticAssets.targets` with `UseSerenityStaticAssets()` and `PrefixedFileProvider`, which serve, publish and pack the assets of libraries that don't use the SDK (under `StaticWebAssetBasePath` or the assembly name) from a build output manifest; corelib, domwise, sleekgrid, assets and the common features switched to the plain SDK.
- The static assets manifest also carries the publish include/exclude patterns, so files that would not be published or packed are no longer served (`StaticAssetPatternFilter`, `FilteredFileProvider`), and the long default pattern lists are shared as pattern sets instead of being repeated for every folder.
- Publish precompresses eligible files to `.br` / `.gz` (`CompressStaticAssetsTask`, cached in the intermediate output, Brotli only when MSBuild supports it, with a visible notice otherwise), and the host serves the matching variant with `Vary: Accept-Encoding`. Build tasks now ship compiled in the `Serenity.Net.Web` package instead of being recompiled with `RoslynCodeTaskFactory`, and the TypeScript and sergen transforms became incremental with input lists and stamp files.
- The Serene and StartSharp templates set `StaticWebAssetsEnabled=false` and call `UseSerenityStaticAssets()` instead of `UseStaticWebAssets()`, and the build emits `SERSA001` / `SERSA002` warnings when a referenced project or package still needs the SDK (silence with `SerenityStaticWebAssetsCheck`) (Serene, StartSharp).
- **[Breaking Change]** Existing projects must call `UseSerenityStaticAssets()` instead of `UseStaticWebAssets()` (or keep static web assets enabled), and projects referencing a package or project that uses the static web assets SDK must set `StaticWebAssetsEnabled` back to `true`. `wwwroot` files are now `None` items instead of `Content`, so custom content rules may need adjusting.
- Add indeterminate (load-more) paging for grids that would rather not count every row: `RemoteView.indeterminatePaging` asks the server for `IncludeMore` instead of `ExcludeTotalCount`, `PagingInfo` exposes `indeterminate` / `hasMore`, and `SlickPager` hides the total page count and last-page button, disables the next button when there are no more rows and accepts any page number. A new `RangeStatus` pager text ("Showing {from} to {to} records") covers the unknown-total case and is translated for all locales.
- Extract `applyResponse`, `setRequestPaging` and `keepServerTotals` hooks in `RemoteView`, make `page`, `totalCount` and `more` protected and track `totalsKept`, so a view that loads rows in another way, e.g. by appending them, can reuse the paging logic.
- Add `InfiniteScrollView` / `InfiniteScrollPager` to Pro.Extensions. The view appends the next chunk when the grid nears its last row, preserves rows and scroll position on refresh for the same query, deduplicates rows returned again and keeps the server totals and aggregates from the initial request.
- Add `VirtualScrollView` / `VirtualScrollPager`, which exposes a row for every row the request matches while only loading the ones near the viewport (sorting, grouping and aggregating on the server, listing groups even when none of their rows are loaded, and releasing rows far from the viewport past `maxLoadedRows`), so the scrollbar covers all rows and any place can be reached without loading everything before it. Their pagers accept any view that can bind to a grid via the new `ScrollLoadingView` interface (StartSharp).
- Add query capture mode to list handlers, so the query a handler builds can be reused as the source of another query (e.g. an entity query of a data source). `IListQueryCapture` / `IListQueryCaptureAsync` expose `CaptureQuery`, found through the new `IListRequestProcessorWrapper` by `GetQueryCapture` (with a sync adapter), and a captured query skips key order, sort, paging, total count and post-execution steps. `IListRequestHandler.ExecutionMode` (`ListExecutionMode.Normal` / `QueryCapture`) lets handlers and behaviors know why they run, e.g. to apply filters to the query instead of post-processing rows.
- Add `EntityQueryMode` / `[EntityQuery]` to declare how an entity may be read as a query source, and `GetPostQueryOverrides` to report the post-query methods (e.g. `ProcessEntity`, `OnReturn`) a list handler overrides, since those row-dropping filters do not run on a captured query. `GetRequestType` now unwraps the processor wrappers.
- Add `Serenity.StringEnum` for sets stored and transferred as strings, with cached `StringEnumInfo` / `StringEnumMember` lookups (name, value, text, visibility) and `IsStringEnum`, `GetName`, `GetValue`, `GetValues`, `GetNames`, `IsDefined` and `GetText` helpers. Add `StringEnumField<T>`, which rejects JSON values that are not members through a new `ConvertJsonValue` hook on `StringField`.
- `EnumEditorAttribute` now takes a `Type` or an enum key, so a plain string property can use a set; `EnumKey` / `IgnoreUIField` work on string enum members, local texts register under the usual `Enums.<key>.<name>` keys, and generated TypeScript is a const object plus a union type and a `registerEnum` call. `EnumSelectFormatter`, `EnumEditor` and `RadioButtonEditor` handle string values instead of their numeric-only workarounds.
- Add flags support to enum editors: `registerEnum` takes `enumFlags` (emitted for `[Flags]` enums), `Enum.toString` returns a named member or the fewest members that make the value like .NET, `EnumEditor` has a `flags` option so a multiple editor stores the OR of selected members and skips `None` while combined members exist, and `ServiceLookupEditorBaseAttribute` edits `Capabilities` with such an editor.
- Accept any valid flags combination in `EnumJsonConverter` (including `ulong`-backed enums). Add `IconClass` metadata to enum members, emitted as `enumIcons` and exposed through `Enum.getIconClass`, extract `EnumEditor.mapEnum` / `mapIcon` hooks for subclasses, and register `ColumnSelection` / `RetrieveColumnSelection` under their server-side full names.
- Add `VisibleWhen`, `ReadOnlyWhen` and `RequiredWhen` attributes with the `PropertyCondition` model, projecting conditional rules into `PropertyItem` metadata. `PropertyGrid` evaluates them when values load and change, updating field visibility, read-only and required state, hiding categories and tabs whose fields are all hidden, and supporting `clearWhenHidden` so values hidden by a condition are saved as empty.
- Add `LazyLoadInclude` (optionally `LazyLoadInclude<"Group">`) so types only referenced by key, such as editors of dynamically generated property items, are registered by code generation and loaded when a type registry first asks for them. Generated `LazyTypeLoader.mts` uses literal dynamic imports so bundlers can split types into chunks, and grouped types share one `LazyTypeGroup.<Group>.ts` chunk.
- Add `LazyLoadTypesAttribute` / `Html.LazyTypes()`, which render the content-hashed module URLs and keys as JSON (added to the Serene and StartSharp layout heads), with `Config.lazyTypeLoader` as the default loader, which can also answer availability checks without importing a module. `QuickFilterBar` and `EditorFiltering` accept promise-like widget types with placeholders, preserving saved state, values and handlers until the real widget is ready. Demo Northwind formatters are lazily loaded.
- Support editor attribute templates in the property grid: a custom editor attribute inherits defaults from an editor attribute declared on its class while explicit options still win, service lookup templates only supply item type, endpoint type and action name when the attribute defines no endpoint, and lookup template options are copied into filtering (now including capabilities and quick search fields).
- The server typings generator emits `<Editor>Defaults` constants for lookup and service lookup attributes with a template, resolving service/action, capabilities (as named enum members like `FilteringOptions | Sort | PagingOptions`), id/text/icon fields and lookup keys like the server does. Add `QuickSearchFields` and a typed `Sort` (string array) to `ServiceLookupEditorBaseAttribute`, make `SetServiceLookupParams` public, and expose `PropertyGrid.getEditorsLoading()` so callers can wait for lazily loaded editors before saving.
- Add `ColorEditor` with a swatch that opens a `ColorPickerPanel` popover (saturation/brightness area, hue and alpha sliders, hex/RGBA fields and an eyedropper), plus `ColorUtils` for parsing and formatting hex and `rgb()` colors, RGB/HSV conversion and readable text colors. The popup follows its swatch on scroll and resize.
- Add `DateRangeEditor`, which edits a range as text or from a flatpickr range calendar with predefined ranges that follow the current date (storing the range name when it has one, so it stays correct as days pass), `DateRange` parsing, formatting and naming with localized names, `PreviousStart` / `PreviousEnd` / `PreviousEndExclusive` parts for comparing with the previous period, and an `EndExclusive` part so `OrderDate < @EndExclusive` covers the last day of a date-time range. Both editors and their texts are localizable (StartSharp).
- Add `InferKey` to `FormScript`, `ColumnsScript` and `EnumKey` attributes (plus a parameterless `EnumKeyAttribute`), so form, columns and enum keys are derived from the `[Module]` attribute, the `BasedOnRow` type or the namespace instead of being spelled out everywhere.
- The server typings generator, `FormScriptRegistration`, `ColumnsScriptRegistration` and `PropertyItemsLocalTextRegistration` resolve keys the same way, so generated keys and runtime script/text prefixes match, and codegen templates plus the demo Northwind forms emit `InferKey = true`. The StartSharp pro feature modules (meeting, worklog, email client, data audit log) now use inferred, shorter keys (e.g. `Serenity.Pro.Meeting.Forms.MeetingForm` becomes `Pro.Meeting.Meeting`), so regenerate server types and texts after updating.
- Honor attributes declared on the interfaces a property implements through `GetPropertyAttribute` / `GetPropertyAttributes` and `WrappedProperty`, used by validation, property grids, row mapping, localization, reporting and server typings, so shared interface properties behave like normal property items. Own and base attributes still win, repeatable attributes (`AllowMultiple`) from interfaces are appended, explicit implementations are ignored and interfaces are ordered deterministically.
- Combination attributes are read from metadata without running their constructors (fixing recursion through attributes that read their own row), and own attributes now win over intrinsic ones. The RowFields source generator applies the same resolution for `[IdProperty]`, `[NameProperty]`, `[Column]` and `[Size]` (StartSharp).
- Add `BasePropertyOrderAttribute` with `BasePropertyPosition` (`Default`, `Before`, `After`), `TypesBefore` / `TypesAfter` and `WhenInherited`, so a type controls where inherited properties (or specific base types' properties, in list order) appear in forms and property grids instead of relying on declaration order; the option composes across multi-level hierarchies.
- `BooleanEditorAttribute` / `BooleanEditorOptions` now take the `LabelPosition` / `labelPosition` enum (`CheckLabelPosition.Start`, `End`, `EndAligned`); `EndAligned` leaves the caption area empty so a checkbox alone in its row lines up with other editors.
- Add `Cast`, `Replace`, `Substring` and `Trim` / `TrimLeft` / `TrimRight` expression attributes, plus `DateAdd`, translated for every supported dialect (SQL Server pre-2017 `TRIM`, MySQL cast types, Oracle date truncation, SQLite type affinity, Firebird's missing `LTRIM` / `RTRIM`). Rework `DateDiff` so every dialect counts the part boundaries crossed like SQL Server's `DATEDIFF` instead of complete units (SQLite `STRFTIME`, PostgreSQL `DATE_TRUNC`, Oracle `TRUNC`, MySQL `DATE_FORMAT`), and clamp end-of-month dates for month/year addition.
- Fix `SqlDateTimeOffset` translations (PostgreSQL `CURRENT_TIMESTAMP`, SQL Server `SYSDATETIMEOFFSET()`) and use Firebird 4 time zone support for `SqlUtcNow`. Bind Oracle parameters by name (`BindByName`) so repeated or reordered expression parameters get the right values, and cast single parameters for Firebird in `DatePart` and `Concat`.
- Add `SqlCollationSpec` / `SqlCollationMapper`, which translate ordinal, invariant and culture comparers with case/accent options into engine-specific collation names, and accept `automap`, `tryautomap` and `tolerant` keywords (plus `IgnoreAccents`) on the connection `Collation`, resolved from the connection's `Comparer`.
- Allow creating databases with a collation: `EnsureDatabase` takes an optional collation (SQL Server/MySQL/Firebird `COLLATE`, PostgreSQL `TEMPLATE template0` with locale or ICU, rejected for SQLite and Oracle), defaulting to the connection's collation, with `PartialContainment` support and no `COLLATE` / `CONTAINMENT` when attaching existing SQL Server files. Add `MapCollation` / `TryMapCollation` migration helpers and `MigrationUtils.ClearAllPools`, which clears only the migrated database's pools instead of every pool in the process.
- Add `INormalizedUsernameRow` / `INormalizedEmailRow` and `BaseUserRetrieveService.NormalizeUsername` / `NormalizeEmail`, which compose Unicode and fold case (including Turkish ı/İ) so username and email lookups no longer depend on database collation.
- The Serene and StartSharp templates add `NormalizedUsername` (unique index, not null) and `NormalizedEmail` (non-unique index) columns with backfill migrations that use the SQL equivalent of the normalizer, keep them in sync in user save, sign-up and LDAP user sync, and use them for duplicate checks and login throttling (Serene, StartSharp).
- Add `IUserListService` / `IRoleListService` with `BaseUserListService` / `BaseRoleListService` implementations that list users and roles for pickers while selecting only what a list item needs, so nothing like a password hash is loaded; ids and usernames are chunked to stay below parameter limits, normalized usernames are used when available, and users requested by id or username are listed whatever their state.
- Add `IRoleKeyRow` and `GetKeyOrName`, so renamed roles keep a stable key for the permissions stored against them, and register the Serene and StartSharp implementations with `AddUserListService` / `AddRoleListService` (Serene, StartSharp).
- Match migrations by tag dimension instead of a flat tag set. `MigrationTagUtils` groups tags by the part before `:` and requires every declared dimension to match, while keeping FluentMigrator's RequireAny/RequireAll rules within a dimension; the `db:` dimension is treated as a partition and legacy tags like `DefaultDB` normalize to `db:Default` only when that database is requested. `TargetDBAttribute` declares both forms and StartSharp's migration runner uses the new conventions.
- Add `SampleDataAttribute` to gate sample data migrations behind `sample:All` or per-module tags such as `sample:Dashboards`, and `UserEntityOptions.SystemUserId` with `GetSystemUserId` / `HasIdentityUserId`, so migrations create the Users table with a plain primary key and seed the admin row with the configured id when user IDs are GUID or string instead of database-generated (Serene, StartSharp).
- Allow two-level cache entries to depend on several cache groups through `Get` / `GetLocalStoreOnly` overloads taking group key lists, so an item expires when any of its groups changes; single-group entries stay compatible. Add `NavigationCaching.ItemsGroupKey` and make the navigation model depend on both permissions and that key, so navigation items are rebuilt when the item group changes and sources can invalidate items without knowing factory cache details (Serene, StartSharp).
- Detect unknown feature toggle keys. `ConfigurationFeatureToggles` gains `knownFeatures`, `disableUnknown` and `onUnknownFeature`, tracks queried keys that are not declared anywhere and can log a warning or disable them, treats a key as declared when it is in `knownFeatures`, `disableByDefault`, the dependency map or the configuration, matches feature keys case insensitively everywhere and tolerates dependency map entries that differ only by case or have null values. `AddApplicationPartsFeatureToggles` collects known keys from scanned `FeatureKeySetAttribute` enums. Serene and StartSharp disable unknown keys (Serene, StartSharp).
- Warn in `sergen` when the running tool version differs from the project's `Serenity.Net.Web` package version (when run as `dotnet sergen`), so generated code does not come from a mismatched version. `sergen` also refuses `mvc`, `clienttypes` and `transform` with a new `GeneratedOnBuild` exit code (14) when the project's Pro.Coder source generators already produce those files during build, warning when the project runs a different sergen location; `--force` overrides the refusal.
- Add `IDynamicLocalTextSource` so texts kept in application data are registered at initialization time, after base and JSON texts and again after texts are reset, and `ITranslationKeyProvider` so such keys (e.g. dashboard titles) can be surfaced on the translation page, reporting only the keys and their source texts. StartSharp's translation list merges the provider keys with the registry (StartSharp).
- Extend basic types from corelib-typed base classes in generated client types: a project's basic type now extends the nearest base class from a Serenity.Net assembly that corelib has a type of the same name for (e.g. `PropertyItem`) instead of repeating that base class's members; generic base types and enums are skipped, and project base classes still list their members since they have no script type.
- Expand Data Explorer's SQL type mappings for Oracle (`varchar2`, `nvarchar2`, `number`, `clob`, `nclob`, `raw`, `long raw`, `rowid`, `urowid`, `binary_float`, `binary_double`, `timestamp with local time zone`, `xmltype`, `bfile`), PostgreSQL (`uuid`, `json`, `jsonb`, `bytea`, `boolean`, `double precision`), MySQL (`longtext`, `mediumtext`, `tinytext`, `blob`) and DB2 intervals, including stream- and byte-backed types.
- Default table exclusions in Data Explorer now apply to every connection (`UseDefaultExcludeTables`, true by default) instead of only configured ones, `ExcludeTablesRegex` adds to the default pattern instead of replacing it, the pattern matches sensitive tables with or without a schema prefix and case-insensitively, and it also hides `DistributedCache`, `FileSystem`, `dsrc_*` and `dash_*` tables (StartSharp).
- Add `Serenity.Testing` and `Serenity.Testing.Playwright` packages with the shared mocks, null mocks, test rows and in-memory SQLite helpers (namespace `Serenity.TestUtils` is now `Serenity.Testing`), `ISerenityTestApp` / `FunctionalTestContext<TStartup>` to boot an application in-process on a random HTTPS port with an in-memory database, snapshots, test login endpoints and configurable test users, and Playwright page objects for dialogs, grids and inputs with auth, AJAX and failure-artifact helpers. Serene and StartSharp test projects use them.
- Bump template and shared NuGet package versions: `Microsoft.Data.Sqlite`, `Microsoft.Extensions.*`, `System.Text.Json`, `System.Security.Cryptography.Pkcs` and `Microsoft.AspNetCore.DataProtection` to 10.0.15, `Microsoft.Data.SqlClient` 7.1.2, `Microsoft.Build.*` 18.12.2, MailKit 5.0.0, Markdig 1.6.0, NUglify 1.23.7, Scriban 7.6.1, OpenIddict 7.9.0, Oracle.ManagedDataAccess.Core 23.26.401, MySqlConnector 3.0.0, `NuGet.Packaging` / `NuGet.Protocol` 8.0.0, PuppeteerSharp 25.17.1 and SkiaSharp 4.156.1. `EmailSender` uses the MailKit 5 `SendRequest` API, as the old `Send` overload was removed.
- Bump template JS dependencies: daterangepicker 4.0.2 (which now declares `jquery` and `moment` as peers, and hides its calendars when `ranges` is set, so the dashboard date range picker passes `alwaysShowCalendars`), preact 11.2.0, dompurify 3.4.18, `@preact/signals-core` 1.14.5, bootstrap 5.3.9, tiptap 3.34.0, fullcalendar 7.1.5, `datatables.net-bs5` 3.1.5, gridstack 14.1.0, CodeMirror SQL editor packages for dashboards, and vite 8.3.8, vitest / `@vitest/coverage-v8` 5.1.0, jsdom 31.0.1 and es-module-lexer 3.0.5 (Serene, StartSharp).

### Bugfixes
- Require POST and row-level authorization on read endpoints. `[HttpPost]` plus `[AuthorizeRetrieve]` / `[AuthorizeList]` are added to the retrieve and list (including list Excel) actions of the Northwind demo modules, the Serene administration endpoints and the StartSharp/pro-features endpoints (Serene, StartSharp).
- Fix grid interaction issues: cancelling a cell edit with Escape now consumes the keystroke so it does not close a containing dialog, drag plugins no longer intercept presses inside an open editor (which made text selection impossible), a column whose `maxWidth` is null, undefined or 0 is treated as unlimited when autosizing, and scrollbar thickness is measured from bounding rects (cached dimensions are invalidated on `devicePixelRatio` changes) so zoomed pages do not get a spurious horizontal scrollbar.
- Keep the grid editor open during auto save: the active edit (column, row, item id, add-row flag and typed value) is captured before the save and restored afterwards, the item's new row is followed when the save reorders rows, `grid.refresh()` is skipped while an editor is open, and the grid edit value is now selected after it is loaded so typing replaces it (StartSharp).
- Fix editor and field layout issues: the Select2 container no longer copies the `editor` class (it gets `editor-like` instead), so code looking up a field's editor no longer finds the container; `ComboboxOptions.formatResult` receives the result, container and `Select2QueryOptions` like the Select2 formatter; `CheckLookupEditor` and `ComboboxEditor` unbind their cascade link on destroy instead of keeping a handler on the parent widget; an editor with addons and the validation icon share the row with a label instead of overflowing it.
- Fix `RemoteView` aborting a request that a data processing callback starts itself: `onProcessData` now runs after the loading state of the finished request is cleared, and cleanup only clears the loading flag when it still belongs to the finished request.
- Fix migration edge cases: use a raw `ALTER TABLE` on Oracle when setting an already nullable column, skip the oversized composite `FileSystem` index on Firebird, execute the Oracle concurrency version trigger through `WithConnection` (FluentMigrator's splitter cuts PL/SQL blocks), quote Firebird/Oracle identifiers through the dialect, reset Firebird identity generators after seeded rows so later inserts do not collide, and match provider-specific SQL by prefix so versioned dialect names like `Postgres15_0` are not silently skipped.
- Reparent header menu and header filter dropdowns into the closest modal or `document.body`, so modal overflow and stacking contexts no longer clip or hide them (StartSharp).
- Map additional SQL types for schema generation (`boolean`, `bytea`, `double precision`, `json`, `jsonb`, `uuid`) and handle `DateOnly` values when reading and converting `DateTime` fields, since providers such as Npgsql return `DateOnly` for date columns.
- Fix the local (in-process) service lookup dropping custom request properties that are not list request options. Request keys are now mapped to capabilities explicitly instead of looked up by name in the capabilities enum, so unrelated option names are no longer matched and unsupported ones are still removed.
- Keep the Northwind sample data relevant on every dialect: `NorthwindSampleDates.Shift` moves the seeded sample orders (only ids up to the last sample order, so orders entered by users are untouched) and `CustomerDetails.LastContactDate` to the present using `DateAdd`, is idempotent, and ignores null dates. StartSharp runs it during startup migrations only when sample data is requested, so dashboards showing the last 30 days are not empty (demo Northwind, StartSharp).
- Correct localization problems across the framework, common features and pro feature locales: restore translations that were cut off mid-word, restore missing punctuation and placeholder markers (All, ThenBy, ClickToDefine, NoResultsClickToDefine), fix Dutch and Romanian strings that lost their text, correct Turkish password/permission terminology, translate previously untranslated terms and align page titles with permission names. Complete and correct the demo Northwind and basic sample translations (Serene, StartSharp).
- Skip the expensive diagnostics scan in Pro.Coder server typings when the run would not delete stale outputs, keeping the cheap unresolved-attribute check and deferring the full compile diagnostics behind a memoized fallback (StartSharp).

## 10.6.3 (2026-10-27)

### Features
- Pin `Microsoft.TypeScript.MSBuild` to 7.0.1 instead of the floating `[6.0.3,7.0.0)` range, so projects created from the Serene and StartSharp templates are built with the TypeScript 7 compiler. Existing projects keep restoring the 6.x compiler from their own reference until they update it to 7.0.1.
- Bump template and shared NuGet package versions: `Microsoft.Data.Sqlite`, `Microsoft.Extensions.*`, `System.Text.Json`, `System.Security.Cryptography.Pkcs` and `Microsoft.AspNetCore.DataProtection` to 10.0.14, MailKit 4.18.2, Markdig 1.5.0, NUglify 1.23.5, Scriban 7.5.3, PuppeteerSharp 25.15.0 and Microsoft.Playwright.Xunit.v3 1.64.1.

### Bugfixes
- Fix `sergen doctor` reporting a false `@serenity-is/tsbuild` version error or warning. The Serenity-to-tsbuild version table is now treated as a minimum required version instead of an exact one, so Serenity patches that don't have a matching tsbuild release no longer fail the check, and a newer tsbuild version only produces a warning when its minor version is ahead of the project's Serenity version. The recommended version comes from the npm registry `latest` tag when it is newer than the last table entry and the project's Serenity version is at least as new, falling back to the table when the registry is unreachable.

## 10.6.2 (2026-10-25)

### Bugfixes
- Fix a version mismatch in common-features packages.

## 10.6.1 (2026-10-25)

### Features
- Add a new icon browser at https://serenity.is/icons that lets you search ~175,000 icons from Iconify packages and copy the matching markup or CSS rule for the new `sv-` / `svbg-` icon classes. The `sv-`, `svbg-`, `svcontent-` and `svaccent-` helpers render an icon as a color-following mask, a background image, intrinsic-size content or a two-layer accent, themed through `--sv` / `--svaccent` / `--svaccent-color` custom properties. 
- Replace the Font Awesome icon font glyphs in the framework and StartSharp stylesheets with these mask icons and a `--s-ico` fallback chain per component. **[Breaking Change]** Stylesheets or scripts that override icons through the old font glyphs (`content: "\f..."` with `font-family` / `font-weight`) need to set `--s-ico` or the matching `--s-icon-*` variable instead.
- Add `DisplayText`, `CategoryName` and `DescriptionText` attributes with an explicit `TextKey`, and `TextKey` support on `Hint`, `Placeholder`, `Tab` and `InstanceName` attributes, so captions can be localized without a literal string. Local text registration honors `TextKey` first, otherwise registers the value as literal invariant text, and the code generator emits `DisplayText` (e.g. `DisplayText(TextKey = "Db.Shared.RecordId")` for the ID column) instead of `DisplayName`. Add `IImplicitLocalTextRegistrar` / `AddImplicitTexts` so attributes can register implicit text keys and defaults, and share the resolution logic via `LocalText.ResolveFromAttribute`. **[Breaking Change]** `Tab`, `Hint`, `Placeholder` and `InstanceName` no longer treat key-looking strings as local text keys; set `TextKey` instead. Legacy `DisplayName` / `Description` / `Category` attributes keep the old inference so existing code is unaffected.
- Add `Field.PublicName` and the unified `FindFieldByPublicName` / `FindFieldByPropertyOrFieldName` row helpers (also `IRow` extensions) so the `PropertyName ?? Name` fallback lives in one place, and route row serialization, SQL, request handlers, lookup/upload, projections, validation and dynamic script call sites through them.
- Add layout CSS and script file bundles for dynamically loaded libraries. Add `LayoutCssFileAttribute` / `LayoutScriptFileAttribute` with `ILayoutCssFileProvider` / `ILayoutScriptFileProvider` discovery (feature filtering, ordering, deduplication) and `Html.LayoutCssFiles()` / `Html.LayoutScriptFiles()`, which emit content-hashed links when bundling is off or a single dynamic bundle per feature set when it is on. Expose them as the predefined `dynamic://CssBundle.LayoutCssFiles` / `dynamic://Bundle.LayoutScriptFiles` bundles, extract `ICssBundleBuilder` / `IScriptBundleBuilder` so the concatenation and minification logic is reused, and add `IWebRootFileWatcher` for shared file watching (used by the content hash cache and layout bundles). Add `CssBundlingOptions.ProcessImports` / `DedupImports` to inline relative `@import` statements in bundles, with cycle detection, url rewriting and per-file minification (StartSharp).
- Improve combobox and lookup editors. Add `Combobox.ensureSelectionLoaded()` and `selectionLoadState` (also on `ComboboxEditor`), which now resolve instead of racing the pending initial selection load, reject waiters with an `AbortError` when the load is aborted or superseded, and replay the last failure. Add `getSelectedItemAsync()` / `getSelectedItemsAsync()`, and mark the `change` event raised by `setValue` with a dataset flag so listeners can tell it apart from user edits. Add an optional `iconField` to lookups and combobox editors (also inferrable from `IconClass` / `Icon`), rendering icons in Select2 results and selections. Export `AbstractSelect2` / `SingleSelect2` / `MultiSelect2` and tighten the Select2 typings. Do not autofocus the search input on touch devices and route hardware keyboard input (Escape, Enter, arrows, printable characters) to the open dropdown instead.
- Add `getWindowLocation()` to corelib and use it instead of assigning `window.location` directly in the built-in pages (login, sign-up, password actions, idle timeout, two-factor, external logins). Application code can adopt it so tests can replace the returned location, as jsdom does not implement navigation. Use `getWindowLocation({ top: true })` where the redirect must break out of an iframe, as the service call 403 redirect does.
- Build an equality criteria instead of an `IN` list when a `ServiceLookupEditor` is given a single id, and return early without a service round trip when `idList` is empty.
- Allow `RenderableContent` for grid titles: `DataGrid.getInitialTitle()` / `setTitle()` no longer require a string. Show the active sidebar item's icon next to the entity grid title, scoped to the main `#GridDiv` inside `section.content` (StartSharp).
- Add a non-interactive mode to `sergen g` / `--generate` for use in CI. `--no-interactive` (aliases `--ni`, `--noninteractive`) skips every prompt, so values with a default fall back to it and values without one (`--table`, and `--connection-key` when the project has several connections) fail with a message naming the missing switch instead of blocking. Add `--overwrite[:mode]` (`yes`, `no`, `skip`, `force`, with the usual aliases) to decide what happens to existing files whose content would change: a bare `--overwrite` means `yes` (replace, keep a `*.bak`), `skip` leaves the file alone and continues, `no` refuses. Without the switch it stays interactive, and in non-interactive mode it defaults to failing rather than silently discarding hand edits. A refused overwrite reports the new `OverwriteRefused` exit code (13) so scripts can detect it, and `--help` can now be combined with a command, with a dedicated help text for `generate` documenting the new switches.
- Add a `migrate` command and exit-code helpers so an application can run migrations without starting the web host. Add `HostMigrationExtensions.RunMigrationsIf` (and a generic overload) which returns `null` for other arguments and `0` / `1` / `2` for success / failure / extra arguments, leaving process termination to the caller. Add `BaseDynamicDataGenerator.RunIf` with the same contract and keep `RunAndExitIf` as an obsolete wrapper. `IDataMigrations` is now shared under `Serenity.Extensions` so Serene and StartSharp use a single definition, and both templates' `Program.Main` return the resulting exit code (`dynamic-data` / `migrate` invocations now report failures instead of exiting in-process, so `using var host` still disposes) (Serene, StartSharp). **[Breaking Change]** Existing projects must delete their local `IDataMigrations.cs` copy, or DI will fail at runtime.
- Update the TypeScript parser port to TypeScript 6.0.3 (add `defer` / import phase modifiers, ES2023-ES2025 targets, import attribute and modifier fixes), and reduce its memory and CPU cost: `SyntaxKind` is stored as `short`, node flags are packed into 12 bits, identifier text is materialized lazily, scanner keywords are looked up without allocating substrings, and AST parent pointers and the unused incremental parsing code are gone.
- Replace `dts-bundle-generator` with the new in-repo `@serenity-is/dtsbundler`, which uses the workspace TypeScript 7 and native defaults; bundled `.d.ts` files now use named imports for `ClassTypeInfo` / `EditorTypeInfo` / `FormatterTypeInfo` / `JSXElement` / `Column` instead of inline `import("...")` types (also in StartSharp).
- Add a `pnpm cleanup` script that removes only well-known regenerable folders (`node_modules`, `.vs`, `obj`, `*.Web/bin`, `artifacts`), never follows symlinks or junctions, skips `.git`, supports `--dry-run` / `--root`, removes deepest targets first and falls back to a chmod pass for read-only files. It replaces StartSharp's `git-clean.cmd`, which also deleted untracked per-checkout files (Serenity, StartSharp).
- Inherit the shared pnpm catalog, `allowBuilds` and `overrides` from `Serenity/pnpm-workspace.yaml` in StartSharp via a `.pnpmfile.mjs` hook, keeping only StartSharp-specific entries local and failing when a `serenity` catalog entry is defined in both places (StartSharp).
- Skip the expensive diagnostics scan in Pro.Coder server typings by inspecting global usings directly when the project uses Serenity's global usings, and track unresolved Serenity attribute symbols so incomplete design-time compilations still block deletion of generated output (StartSharp).
- Bump xunit.v3 to 4.0.2 with `global.json` selecting Microsoft.Testing.Platform, and migrate the CI test steps to the MTP argument syntax (also in StartSharp). Bump Preact to 11.0.2 (class component refs are no longer forwarded, so `MailboxLoginView` captures its root node with a ref callback) (StartSharp). Bump NuGet dependencies (Markdig 1.4.2, Oracle.ManagedDataAccess.Core 23.26.400, PuppeteerSharp 25.14.0, Microsoft.Extensions.* 10.0.13, Microsoft.Build.* 18.11.0) and enable central transitive pinning; JS dependencies (tiptap 3.32.0, vitest 5.0.5, vite to 8.3.5, jsdom 30.1.3, dompurify 3.4.17, es-module-lexer 3.0.4, chart.js 4.6.1, fullcalendar 7.1.3, datatables.net-bs5 3.1.4). Rename `NuGet.config` to `nuget.config` for case-sensitive file systems and include `SkiaSharp.NativeAssets.Linux.NoDependencies` on Linux builds.
- Mark the legacy plugin system types (CSS/script files, navigation, service/background job and `PrecompiledViews` attributes) as obsolete in their documentation and point to the supported replacements (`LayoutCssFile` / `LayoutScriptFile`, `NavigationLink` / `NavigationMenu`, dependency injection).

### Bugfixes
- Emit double-quoted module specifiers in generated TypeScript files, matching the style used by server typings.
- Fix the `Enums.ImageCheckResult.HeightTooLow` texts, which talked about width, and add the missing `Validation.Int64` message to the web locales; clean up punctuation in the default English service strings.
- Fix `UpdatableExtension` reporting `OtherKey` instead of the `FilterField` value in its error message.
- Require explicit permissions for dynamic scripts, pages and endpoints. A missing permission previously skipped the check entirely, so dynamic scripts without one were public; they now default to denied, pages and services to logged in, an empty permission is treated as deny, and built-in bundles/scripts are marked `SpecialPermissionKeys.Public`. **[Breaking Change]** Use `SpecialPermissionKeys.Public` (`*`), `SpecialPermissionKeys.LoggedIn` (`?`) or `SpecialPermissionKeys.Deny` instead of magic strings, and set a permission on dynamic scripts or pages that should stay accessible.

## 10.6.0 (2026-10-11)

### Features
- Add `ListRequestCapabilities` and `[ListRequestCapabilities]` attribute so list handlers can declare supported operations. `ServiceLookupEditor` now uses these capabilities, can infer lookup endpoints from row metadata, and supports primitive list responses.
- Add `IncludeMore` to `ListRequest` and `More` to `ListResponse`, allowing callers to check for another page without requiring a total count. Add configurable default and maximum page sizes (`RequestHandlerSettings.DefaultPageSize` / `MaxPageSize`, bound from the `RequestHandlerSettings` configuration section), `ExcludeTotalCountByDefault`, `ListRequest.SuppressPagingLimits()`, and validation of negative skip / take.
- Add a list request source adapter to Pro.Extensions that can apply unsupported filtering, sorting, paging, distinct-field and deleted-row operations locally, with field permission checks and a configurable item limit (StartSharp).
- Add optimistic concurrency support with `IConcurrencyVersionRow`, `IgnoreConcurrencyVersion`, save-time version checks, trigger migration helpers for supported SQL dialects, and client round-tripping of the concurrency token.
- Add `UserEntityOptions` to configure the user row, table and ID column, and support `int`, `long`, `Guid` and `string` user IDs in rows and migrations. Add `UserIdFieldType` / `UserIdJoinKey` attributes and `AsUserIdType` / `UserIdForeignKey` migration helpers. **[Breaking Change]** Custom row-fields providers must pass the new options to `RowFieldsBase.Initialize`; `LoggingRow` user ID properties and fields are now `object` and `Field` rather than `int` and `Int32Field`.
- Add an optional `undeletePermission` to `IRowDefinition`, with `getUndeletePermission` / `hasUndeletePermission` on `EntityGrid` and `EntityDialog`. The undelete button and the include-deleted toggle now check this permission separately from delete permission.
- **[Breaking Change]** Permission checks now fail closed: rows without explicit permissions are denied by request handlers, and permission keys cannot be empty. Use `SpecialPermissionKeys.Public` (`*`) or `SpecialPermissionKey.LoggedIn` (`?`) when access is intended.
- Automatically protect fields with potentially sensitive names by excluding mapped fields from selection, filtering, sorting, and client writes unless explicitly configured; add `FilterableAttribute` / `NotFilterableAttribute` to override filtering. `IPasswordRow` hash and salt fields are also protected regardless of their names.
- Add an `Int64` editor and filtering, plus `BigIntFormatter`, to handle 64-bit values without losing precision in JavaScript; large Int64 values are serialized as JSON strings and `NumberFormatter.formatValue` is exposed for reuse.
- Add `ListProjected` / `QueryProjected` helpers for mapping SQL queries to typed or anonymous results, with `Sql.Expr`, row-aware `From` and `SubQueryFrom`, `JoinVia`, foreign-row projection helpers and the `AsReusableProjected` extension for repeated execution.
- Cache SQL generated by fluent query builders and add `Freeze` / `FreezeParams` for reusable, immutable queries. **[Breaking Change]** `QueryWithParams.Params` and SQL helper parameter inputs are now read-only dictionaries; SQL and row interceptors now receive argument records.
- **[Breaking Change]** `IFilterableQuery.Where` now accepts `ICriteria?`; custom implementations must provide `GetWhereCriteria` and `GetWhereClause`. The `SqlUpdate.Where(params string[])` helper and legacy `IChainable.With` API are removed, and row-based `From` now sets the query's `INTO` row once.
- Escape literal text passed to `Criteria.StartsWith`, `EndsWith` and `Contains` by default; add `LikeEscaped` / `NotLikeEscaped` for explicit escape characters while keeping `Like` / `NotLike` as raw-pattern operations. Client-side `FunctionCallCriteria` JSON (e.g. `UPPER`) is also supported for case-insensitive `LIKE`.
- Add per-field comparers via `ComparerAttribute` and `ComparerResolver`, per-dialect / per-connection comparers (`ISqlDialect.WithComparer`, `ConnectionStringEntry.Comparer`) and async-local `SqlSettings` overrides, so change detection, unique checks and local (in-memory) list operations honor a database's collation. **[Breaking Change]** The in-memory adapter's `Culture` / `IgnoreCase` options were renamed to `SearchCulture` / `SearchIgnoreCase` (StartSharp).
- Add `WithTableHint` and general table-hint support (SQL Server), an `Exists(BaseCriteria)` overload, immutable `ValueCriteria` snapshots, case-insensitive column lookup, per-dialect `IN`-list parameter budgets, and upsert fallback when all fields are keys.
- **[Breaking Change]** Remove the ImageSharp dependency and the legacy `ThumbnailGenerator` / `UploadProcessor` classes. Image validation and resizing now require an `IImageProcessor` registration; StartSharp and Serene provide a SkiaSharp implementation with JPEG, PNG, GIF and WebP support.
- Add localized validation errors for common SQL constraint violations across SQL Server, PostgreSQL, MySQL, Oracle, Firebird and SQLite, replacing Serene and StartSharp's duplicate SQL exception helper. `ISqlErrorExtractor`, `HumanizeSqlExceptionBehavior` and `[Unique]`'s `DisableDefaultBehavior` are configurable.
- Normalize display-order values by default after reordering (closing gaps), with a `preserveGaps` option; reorders of unique display-order fields use per-record updates.
- Extend DomWise signal support with conditional `Show` rendering, iterable and selection bindings, `withLifecycleRoot`, and automatic disposal integrated with widget lifecycles.
- Introduce feature-specific connection and module attributes like DefaultConnection, MeetingConnection etc. **[Breaking Change]** pro features `ConnectionKeys` classes are removed; use the corresponding connection attribute's `Key` constant instead (StartSharp).
- **[Breaking Change]** `DeltaLister` now defaults to strict mode, so a new item id that is not present in the old list throws instead of being treated as an insert; duplicate ids in the old list also throw.
- Add `Serenity.Pro.Dashboards` with data source abstractions and a Datasets module (connections and datasets) including UI, permissions and migrations (StartSharp). This is a work in progress for dynamic dashboards (Enterprise), it will not be released yet.
- Improve performance by compiling the row factory with expression trees, lazily caching type scans / behavior and handler registries (invalidated via change tokens), and removing quadratic paging in relation handlers.
- Adopt central NuGet package management in StartSharp/Serenity repos. Templates are still released without central package management.
- Add validation for module and identifier names in sergen; module names are treated as namespaces and dots are converted to `/` for URL and file path generation.
- Logging rows now use UTC for `InsertDate` / `UpdateDate` fields, and capture log `ValidFrom` / `ValidUntil` values are stored in UTC unless annotated with `[DateTimeKind(DateTimeKind.Local)]`.
- Increase script test coverage across Serene.Web, demo.basicsamples, demo.northwind and Serenity.Extensions, and StartSharp.Web, dataexplorer, emailclient, meeting, pro.extensions, worklog and Demo.AdvancedSamples (mostly over 95%); remove vitest globals in favor of explicit imports (StartSharp).
- Bump NuGet dependencies (Microsoft.Extensions.*, Microsoft.Data.Sqlite / SqlClient, SkiaSharp 4.153.1, YamlDotNet 18.1.0, Scriban 7.5.0, Microsoft.CodeAnalysis 5.9.0, xUnit / Playwright / Cake tooling) and JS dependencies (dompurify 3.4.16, vitest 5.0.1, vite 8.3.0, moment 2.31.0, es-module-lexer 3.0.2, jsdom 30.1.0).

### Bugfixes
- Improve SQL generation and schema metadata handling across supported dialects, including provider-specific paging, identity discovery (PostgreSQL 9.x, Oracle before 12c, SQLite `WITHOUT ROWID`), foreign-key queries, MySQL `information_schema`, dialect-aware debug dumps, and quoted identifier / parameter scanning.
- Improve connection and transaction cleanup, dispose commands with returned readers, and preserve the original provider exception when SQL errors are converted to validation errors.
- Enforce field read permissions for sorting, equality filters, quick search and criteria operands, and apply field permissions to localization values. Denied users now receive `EntityNotFound` instead of `AccessDenied` for delete/undelete/save so entity existence is not leaked, and the `DENY` permission is never granted.
- Preserve generated server types when source generators are invoked with incomplete design-time compilations, and improve Pro.Coder diagnostics and error detection (StartSharp).
- Honor custom `DateFormatString` values during JSON conversion and preserve date-only values with custom time-format patterns.
- Fix master-detail saves to delete removed detail rows before updates (avoiding unique constraint failures), use `DeleteDate` to detect delete-log rows, return the entity id for no-op updates, restrict auto save-type detection to identity ids and honor `Request.EntityId`, return a validation error for malformed entity ids, and reject mismatched ids on non-updatable fields.
- Fix field conversion and JSON handling: project `DBNull` into non-nullable members as the default, parse `DateOnly` / `TimeSpan` in `ConvertValue`, validate base64 length against field size, and restore stream position after serializing.
- Fix tree ordering for cycles, self-parented rows and deep chains; dedupe keys in linking-set relations; and correct the localization and updatable-extension behaviors.
- Fix SleekGrid issues (top row resolution in `getCanvasNode`, unused `oneOff` event option, frozen layout) and resolve scoped handler / behavior dependencies from the current request scope instead of the root provider.
- Quote identifiers in display-order queries and include the row type in filter validation error messages.

## 10.5.2 (2026-09-16)

### Features
- Enable nullable reference types across the framework and common, pro and business feature projects (`<Nullable>enable</Nullable>`), and `<Nullable>annotations</Nullable>` for tests / Serene / StartSharp. New projects created from the Serene and StartSharp templates now use `<Nullable>annotations</Nullable>` by default (previously nullable was disabled). This only enables nullable annotations, not warnings, so existing code keeps compiling without nullable warnings. If you want full nullable reference type checking, set `<Nullable>enable</Nullable>` in the project file yourself; you may need to clear any resulting nullable warnings first.
- sergen and the code generators now treat `annotations` the same as `enable` for emitting nullable annotations (`?`, `= null!`), so generated code stays annotation correct if you later switch a project to `<Nullable>enable</Nullable>` without having to regenerate everything. Code that is regenerated (client types, row fields, server typings and other transform output) emits `#nullable enable` for `enable` and `#nullable enable annotations` for `annotations` (annotation context without turning on nullable warnings). Files generated by `sergen g` (e.g. Row, Columns, Form) belong to you, so they do not get any directive, only the annotations. When nullable is disabled (or not set), generated row field declarations (e.g. `public StringField Name = null;`) are now explicitly assigned `null` to avoid CS0649 "field is never assigned" warnings, which show up for private nested test rows and were previously suppressed via `#pragma warning disable` or `.editorconfig`.
- Add `CommitAsync` and `DisposeAsync` to `UnitOfWork` (and `TransactionlessUnitOfWork`), make `ServiceEndpoint` implement `IAsyncDisposable`, prefer async commit / dispose in the endpoint pipeline where possible, and add an `InTransactionAsync` controller extension.
- Add an application part assemblies recovery algorithm to `ApplicationPartsTypeSource`, for the rare cases where `ApplicationPartAssembly` attributes are not properly generated during incremental or parallel builds.
- `TransformInclude` now accepts a generic argument to specify the namespace / type name just like `registerEditor`, e.g. `TransformInclude<"My.Namespace.MyType">` or `TransformInclude<typeof nsSomething>`.
- Add async local (`SetLocal...`) variants of legacy static settings like `DecimalEditorAttribute.AllowNegativesByDefault`, `IntegerEditorAttribute.AllowNegativesByDefault`, `SqlSettings.DefaultCommandTimeout` and the `RowJsonConverter` hooks, so tests and background tasks can set them locally without affecting other threads.
- Replace `@deprecated` JSDoc tags with `**deprecated**` comments in generated `Form.ts` files and corelib declarations, as Visual Studio reports them as warnings with no way to turn them off.
- Use the `GetIdField()` extension method instead of the `IdField` property to get a row's ID field. Unlike the `IdField` property, `GetIdField()` throws if the row's `IdField` is null, making it easier to catch misconfigured rows.
- Increase test coverage: `Serenity.Net.Core`, `Serenity.Net.Services` (~97%), `Serenity.Net.Web` (~95%), code generator commands (client types, entity model, generate, mvc, server typings), `ClientTypesGenerator`, `TSModuleResolver` and others, and add async local versions of legacy settings to avoid flaky tests.

### Bugfixes
- Fix the `I(Delete/List/Retrieve/Undelete)RequestProcessorAsync` companion wrapper direction in the `CompanionHandlerType` attribute, which was inverted.
- Read the `ESMAssetBasePath` project property from the correct casing (previously looked up `EsmAssetBasePath`).

## 10.5.1 (2026-09-08)

### Features
- Serenity Docs (https://serenity.is/docs) now allows viewing documentation for older versions.

### Bugfixes
- ImageUploadEditor not allowing normal files even when allowNonImage option is true (e.g. when using [FileUploadEditor]).

## 10.5.0 (2026-09-06)

### Features
- Add full async request handler support for Save, Delete, Retrieve, List and Undelete request handlers. `SaveRequestHandler`, `DeleteRequestHandler`, `RetrieveRequestHandler`, `ListRequestHandler` and `UndeleteRequestHandler` are split into sync base classes and new `...Async` variants (e.g. `SaveRequestHandlerAsync`, `ListRequestHandlerAsync`). New interfaces like `ISaveHandlerAsync`, `IDeleteHandlerAsync`, `IRetrieveHandlerAsync`, `IListHandlerAsync`, `IUndeleteHandlerAsync` and `I...RequestProcessorAsync` expose async `Process` methods. **[Breaking Change]** `ISaveBehavior`, `IDeleteBehavior`, `IRetrieveBehavior`, `IListBehavior` and `IUndeleteBehavior` are now just marker interfaces. Implement the corresponding `I...BehaviorSync` / `I...BehaviorAsync` interface, or derive from `BaseSaveBehavior` / `BaseSaveBehaviorAsync` (or the other `Base...Behavior(Async)` classes) instead.
- Add auto-wrapping between sync and async behaviors and handlers for compatibility. `SyncToAsync...` / `AsyncToSync...` wrappers allow synchronous behaviors to run in async handlers and async behaviors to run in sync handlers, and the default handler factory can resolve a custom handler of either mode when the other mode is requested.
- Add default empty implementations to behavior interface methods (e.g. async interface methods return `Task.CompletedTask` by default), and make integrated behaviors like CaptureLog, InsertUpdateLog, LinkingSetRelation, Localization, MasterDetailRelation, UniqueConstraintSave, UniqueFieldSave, UpdatableExtension and ValidateParent implement both sync and async behavior interfaces.
- Add async SQL / data access support. Add async variants of `SqlHelper` methods (`ExecuteNonQueryAsync`, `ExecuteAndGetIDAsync`, `ExecuteAsync`, `ExecuteUpsertAsync`, `ExecuteReaderAsync`, `ExecuteScalarAsync`, `ExistsAsync`), async variants of `EntityConnectionExtensions` methods (`ByIdAsync`, `TryByIdAsync`, `FirstAsync`, `TryFirstAsync`, `SingleAsync`, `TrySingleAsync`, `ListAsync`, `CountAsync`, `ExistsByIdAsync`, `ExistsAsync`, `InsertAsync`, `InsertAndGetIDAsync`, `UpdateByIdAsync`, `DeleteByIdAsync`), async `EntitySqlHelper` methods (`GetFirstAsync`, `GetSingleAsync`, `ListAsync`, `ForEachAsync`) and async Dapper `ExecuteAsync` / `QueryAsync` overloads in `DapperCore`.
- Add `EnsureOpenAsync` connection extension and implement `OpenAsync` in `WrappedConnection`, which now extends `DbConnection` instead of implementing `IDbConnection`. Add `DataReaderExtensions.ReadAsync` / `NextResultAsync` which use the native async method on `DbDataReader` and fall back to the synchronous method otherwise, instead of using `Task.Run`.
- Add async variants of `ISqlOperationInterceptor` and `IRowOperationInterceptor` methods (`ExecuteNonQueryAsync`, `ExecuteReaderAsync`, `ExecuteScalarAsync`, `FindRowAsync`, `ListRowsAsync`, `ManipulateRowAsync`) that route to their sync variants by default, so existing interceptor implementations continue to work.
- Update sergen scriban templates so generated handlers derive from the async request handler base classes, generated handler interfaces derive from the async handler interfaces (e.g. `IListHandlerAsync`), and generated endpoint methods are async and take a `CancellationToken`.
- Update the `Pro.Coder` interface source generator to recognize async request handler base classes (e.g. `SaveRequestHandlerAsync`) so it generates the correct async handler interfaces (StartSharp).
- Add async variants of the mock handlers / `MockDbConnection` in test utils, and add tests for the integrated behaviors, particularly for async handling.
- Make `SqlHelper.LogCommand` public again for compatibility and add a null check for the command.
- Convert Serene sample handlers (administration: Language, Role, User) and Northwind handlers (Category, Customer, Order, OrderDetail, Product, Region, Territory, Shipper, Supplier, note handlers) to async, and convert StartSharp sample handlers in administration, meeting, demo.advancedsamples, pro.extensions and worklog modules. Endpoints were updated to call the async handler methods (StartSharp).
- Add a pnpm override for `fflate` to resolve a consistent version across the workspace (also applied in StartSharp).

### Bugfixes
- Fix `getNewId` being passed as `true` for interceptor methods that do not generate a new ID.
- Fix `SqlHelper.ExecuteScalar` to use the interceptor's `ExecuteScalar` instead of `ExecuteReader`, and make `MockDbConnection.interceptExecuteScalar` take an `object` argument.

## 10.4.0 (2026-09-01)

### Features
- Add connection key fallback support (`IConnectionKeyFallbacks`). `DefaultConnectionStrings` and `DefaultSqlConnections` are now driven by assembly-level `ConnectionKeyFallback` attributes, so feature modules can declare their own logical connection key (e.g. `ProMeeting`) while still working in applications that only configure a fallback key (e.g. `Default`). A fallback can also be set via configuration with `Data:(ConnectionKey):FallbackFor`, which accepts a semicolon separated list of connection keys. Migrations for fallback connections only run when `TagBehavior.RequireAny` is used. Pro.Extensions and other feature projects now use this feature (StartSharp).
- Add SQL-backed distributed cache implementation (`SqlDistributedCache`) and `AddSqlDistributedCache` extension. Define feature keys (`SqlFileSystemMigrations`, `SqlDataProtectionMigrations`, `SqlDistributedCacheMigrations`) to optionally disable migrations for FileSystem, DataProtectionKeys and DistributedCache tables (StartSharp).
- Add new `DataProtectionSettings` option type and `AddDataProtectionWithSettings` extension, used in Startup to configure DataProtection and its optional persistence encryption via a certificate loaded by `EncryptionKeyLoader`. Add `ApplicationName` option to `DataProtectionSettings`; if not set, the application name of `IWebHostEnvironment` or the entry assembly is used (StartSharp).
- Add `DataProtectionKeys` migration and `SqlXmlRepository` type that can be used for storing data protection keys in SQL databases, via the `PersistKeysToSqlConnection` extension (StartSharp).
- Add `EncryptionKeySpec` which provides more options to specify how to load an encryption key or certificate, e.g. from a BASE64 encoded PEM text, DER blob, or a file, or via a certificate thumbprint. Add `IEncryptionKeyLoader` and its default implementation `EncryptionKeyLoader` which handles the operation. `Password` is now used instead of `CertificatePassword`/`PrivateKeyPassword`, and a better error is thrown when the password is incorrect (StartSharp).
- Add `SqlInsert.FormatUpsert` static method and `ToUpsertString` method to convert an `SqlInsert` query to a combination of INSERT/UPDATE, e.g. an UPSERT statement based on dialect. Also added `SqlHelper.ExecuteUpsert`, which handles unknown dialects with a basic workaround.
- Define `FieldExpressionPair` and return it from the `GetFieldExpressions` method in `SqlInsert` and `SqlUpdate`. `SqlUpdate` now has `GetWhereConditions` and `GetWhereClause` methods. **[Breaking Change]** Removed the confusing and unused `Where(params string[] conditions)` overload from `SqlQuery` and `SqlUpdate`; call `Where` multiple times instead.
- Remove legacy overloads from `DapperCore` and add comments to clarify non-interception behavior.
- Add change token support to type sources and cache property item processors.
- Add `UseNodeScriptRunner` extension and NodeScriptRunner argument parsing, defaulting `pkgManagerCommand` to `node` instead of `npm`, e.g. use `node --run`. StartSharp feature projects switched to it and removed the `build:watch` script.
- Add `EditLaunchOptions` and `EditPdfOptions` callbacks to `IHtmlToPdfOptions`, which can only be used by Puppeteer.
- Convert the dialogExtend plugin into a proper Serenity widget. Apply CSS changes to make jQuery UI 1.13+ dialog close buttons look properly, e.g. close text hidden with proper alignment. Adjust jqueryui related compat css for close buttons.
- **[Breaking Change]** Treat strings returned from Select2 formatters as text, not HTML markup. If you specified any of the format options like `formatResult`, `formatSelection` etc. for select2 and returned HTML strings, you should modify them to return HTML elements / fragments instead.
- Use jsx syntax for creating select2 containers (select2.ts renamed to select2.tsx), use `e.key` instead of the deprecated `e.which`, and use an alternative method with a smaller special case table for `Select2.stripDiacritics`.
- Add auto generated comment and pragma support to `CodeWriter`, and add XML doc comments to files generated by sergen and the generated ESM file. `*Columns.cs`, `*Row.cs`, `*Form.cs` etc. are added to the ignore list for CS1591 missing XML comment warnings, with pragma warning restore at the end of suppressions. XML comment warnings are also suppressed in source generated row/interface files (StartSharp).
- Add XML doc comments to core, services, web, extensions and feature projects (dataexplorer, pro.extensions, meeting, emailclient, openiddict, worklog), and update JSDoc comments for `@serenity-is/corelib`, `@serenity-is/sleekgrid` and `@serenity-is/domwise`.
- New design for Serenity Docs (https://serenity.is/docs) and dark theme support. Updated client and server API docs generated from XML / JSDoc comments.
- Added comprehensive documentation for both server side and client side framework
- Add support for `DateTimeOffsetField` generation in RowFieldsSourceGenerator and add tests for other field types (StartSharp).
- Add `OmitComments` option for source generator tests, along with comment related tests (StartSharp).
- Increase test coverage: corelib, domwise and sleekgrid now has +90% coverage with new tests for editors, datagrid and dialogs; extensive new tests for sleekgrid (events, formatting, formatters, tree columns, draggable, column sorting/resizing, rendering, layout refs, group item metadata provider, basic layout lifecycle etc.), and a v8 coverage config for sleekgrid vitest.
- Use `import.meta.dirname` instead of the obsolete `__dirname` in vitest configs.
- Bump dompurify to 3.4.14, vitest to 4.1.11, es-module-lexer to 2.3.2, tiptap to 3.30.5, preact/signals to 2.11.1, datatables to 3.0.3
- Bump PuppeteerSharp to 25.8.0, OpenIddict.AspNetCore to 7.6.1, WaffleGenerator.Bogus to 4.3.1

### Bugfixes
- Fix issue where a server type generated at root folder imports a type from a sub folder and instead of `./Module/..`, `../Module/..` is generated.
- Fix several SleekGrid issues: frozen-end column filtering in FrozenLayout, guide top unit, `getRowsRange` descending order, column resizing guard, `getScrollBarDimensions` height, render-row pinned-end break, render-cell addAttrs handling, editors, and `flashCell` method when jQuery is not available.
- Remove invalid closing tag from `.select2-searching`.
- Fix date/time editors to respect min/max/sqlMinMax options, return after null in `set_valueAsDate`, and remove a stray double semicolon in timeeditor value getter.
- Fix decimal/integer editors: preserve string format for AutoNumeric vMin, guard vMin against negative maxValue, and use abs maxValue for vMin when allowNegatives.
- Fix radiobuttoneditor to preserve value during async enum load and support string enums.
- Fix combobox/comboboxeditor: null-prototype item id map and abort init selection via correct property.
- Fix servicelookupeditor to stop mutating the includeColumns option, and editorfiltering to stop re-adding cascadeFrom and default editor type to String.
- Fix basefiltering to map combobox item text in getEditorText, editorutils to localize primitive boolean display text, and datetimefiltering le fall-through and ne boundary criteria.
- Fix datagrid issues: clean up `setInitialSortOrder` and `needsRefresh`, cssEscape quick filter selector, simplify loop condition and quick filter check.
- Fix grid row/radio selection mixins to use prototype-safe include maps and simplify selection toggle.
- Fix slickpager to no-op on boundary page navigation.
- Fix dialogs: correct icon closing tag in bootstrap button, guard dialog instance before responsive handling, and insert buttons correctly when no close button exists.
- Fix propertygrid to throw Error for unloaded editors, and propertyitemcolumnconverter to show a warning icon when a lazy formatter fails.
- Fix propertypanel to strip Panel suffix from form key after namespace.
- Fix criteria to throw Error for mismatched parentheses.
- Fix system helpers: guard `getNested` and throw Errors, correct hasOwnProperty guard for implemented interfaces, and preserve regex dotAll flag on deep clone.
- Fix html helpers: tolerate malformed percent-encoding in parseQueryString and guard outerHtml against null element.
- Fix services to bind header getter for 403 redirect, and scriptdata to build Lookup from resolved hook data with null-safe items access in getColumns/getForm.
- Fix uploader to merge passed request into upload defaults and require container in createUploadInput; correct image editor css classes and dead branch in uploadeditors.
- Fix htmlcontenteditor to apply readonly to the ckeditor instance, and dedupe interface / tidy extension filter in htmlcontenteditor-tiptap.
- Fix filterpanel css class, fluent style guard with CSSStyleDeclaration check, and router onDialogOpen condition precedence.
- Fix checktreeeditor bool check and null-prototype maps, and tabsextensions to guard null href and dispose all tab instances.
- Fix emaileditor to validate user input on domain blur with readonly consistency.
- Fix formatting-compat to parse day.hour:min with zero time and fix Turkish lowercase.
- Fix jquery-compat to call `isMobileView()` for dialog focus handling, and basedialog to guard dialog instance.
- Fix config to fall back to `/` for empty application path.
- Fix debounce so flush returns the last result when there is no pending call.
- Fix cascadedwidgetlink to remove no-op constructor bind and unbind from the stored parent node; dateyeareditor no longer mutates options when building the year list.
- Fix quicksearchinput timer checks and option access, errorhandling dead filename fallback in script error report, and layout helpers to guard null element and non-finite height.
- Fix widgetutils error message and symbol-safe id prefix proxy.
- Fix corelib small-module bugs in formatters, blockui and enumeditor; null-guard issues in tree helpers and tooltip; `toGrouping` null-prototype lookup; layouttimer to throw Error and return number|null; `tryGetText` returns `string | undefined`.
- Fix validator to throw Error and tidy rules/dataRules; remoteview to use null-prototype id/group maps; mocks to guard init.body and normalize response header.
- Fix application name ternary in DataProtection configuration (StartSharp).
- Fix pro.coder tests (StartSharp).
- Serenity submodule update for a migration running behavior fix (StartSharp).

## 10.3.7 (2026-08-12)

### Features
- Add more service collection extensions to simplify service registration in Startup.cs: `AddClamAVUploadScanner`, `AddElevationHandler`, `AddEmailSender`, `AddPasswordStrengthValidator`, `AddHttpContextItemsAccessor`, `AddCssBundling`, `AddCssAndScriptBundling`, `AddScriptBundling`, `AddLocalTextInitializer` and an `AddUploadStorage` overload. Add `AddBackgroundJobs`, `AddPuppeteerHtmlToPdf` and `AddSecureUploadFileResponder` (StartSharp).
- `TranslationRepository` now uses `ILocalTextInitializer` in Serene instead of the static `InitializeLocalTexts` method in Startup.
- Apply RTL related changes for SleekGrid from sleekgrid PR #16, and adjust `slick.base.css` so that it works properly in RTL mode, including in the Pro theme which runs RTL CSS. Add a basic RTL SleekGrid sample, and manually apply the patch from unmerged SortableJS PR #2368 for RTL support.
- Correct RTL drag constraints in SleekGrid by swapping shrink/stretch leeways (#7420).
- Also generate a namespace constant for root namespaces even if there is no server type there.
- Use a namespace for `Criteria` as TypeScript 6/7 produces an incorrect dts file.
- Add `noUncheckedSideEffectImports: false` to Serene tsconfig.json.
- Switch to FullCalendar 7.0.2 (unified package), which has breaking changes; the Calendar sample is updated accordingly (StartSharp).
- Replace the X.PagedList NuGet package reference, which is not updated for some time, with a minimal internal version of X.PagedList (StartSharp).
- Implement better indentation for markup generated from cshtml file for sidebar (StartSharp).
- Bump dompurify to 3.14.13, jsdom to 30.0.1, vite to 8.2.1, tiptap to 3.30.0, esbuild to 0.28.2.
- Bump preact to 10.29.8, vite to 8.2.1, esbuild to 0.28.2, @preact/signals to 2.11.0, @types/google.maps to 3.65.5, highlight.js to 11.11.2, datatables to 3.0.1 (StartSharp).
- Bump SQLitePCLRaw.bundle_e_sqlite3 to 3.0.5, NUglify to 1.22.3, Scriban to 7.2.6, ClosedXML to 0.105.1, Microsoft.Data.Sqlite to 10.0.11, MySqlConnector to 2.6.2, System libs to 10.0.11, PuppeteerSharp to 25.5.0, and Microsoft.Playwright.Xunit.v3 to 1.62.0.
- Remove Microsoft.TypeScript.MSBuild from Feature.Build.props and sample Directory.Build.props.
- Recommended `@serenity-is/tsbuild` version is now `10.3.7` which uses `esbuild 0.28.2`.

### Bugfixes
- Reject protocol-relative URLs in `isSafeReturnUrl`, so inputs like `//evil.example` are no longer considered safe, fixing an open redirect issue (CWE-601, GHSA-9qr4-c5fv-3vwm).
- Ignore `CancellationToken` parameters when generating service typings, so service endpoint methods taking a `CancellationToken` no longer fail silently and now get a client proxy (#7421).
- Also listen for the `input` event in QuickSearchInput, as Playwright only triggers the input event.

## 10.3.6 (2026-08-02)

### Features
- Modified the `PrefixedContext` constructor to optionally accept an object with `{ idPrefix: string, domNode: HTMLElement }`, e.g. a `Widget`, and a second optional `context: HTMLElement` argument. This allows `Form` objects to still work with detached widgets like dialogs. Generated `Form.ts` constructors now use this constructor. When creating form objects, instead of `new CategoryForm(this.idPrefix)`, use `new CategoryForm(this)` to use this ability. Also removed the legacy `w0`, `w1` etc. aliases in generated forms.
- Support stable decimal ordering for dynamic navigation items, e.g. for a CMS. Changed navigation order storage to decimal. Heals duplicate static orders deterministically, preserving `int.MaxValue` as the unset-order sentinel, resolving generated unset orders near their siblings, and using depth-based spacing for healed orders.
- Use `IMPORTFROMCORELIB` instead of `QIMPORT`, `SERENITYIMPORT` etc. in scriban templates when importing types from `@serenity-is/corelib`. The old names still work for compatibility, but custom scriban templates should be updated to use the new unified function.

### Bugfixes
- Fix issue where C# code generated by `sergen mvct` during build is not included in Compile items in Serene, causing the build to fail first time when a new editor type etc. is defined.

## 10.3.5 (2026-07-25)

### Features

- Make EntityDialog methods like `save`, `loadById`, `delete`, etc. return a promise. This might be a breaking change if you have any code that overrides these methods and does not return a promise.
- Apply EntityDialog promise return changes to EntityGridDialog (StartSharp)
- Bring back custom component overload for the `jsx` function.
- Switching to TypeScript 7. `dts-bundle-generator` still needs to use TypeScript 6 as TS7 does not have an API yet.
- Allow `editSettings` function in `AddExceptionLogger` extension (StartSharp)
- Add tests for `autonumeric`, `ui/dialogs`, `ui/filtering`, `ui/helpers`, `ui/widgets`, `validator`, `layout`, `router`, `strings-compat`, `remoteview`, `fluent`, `fluent-events`, `services`, `pubsub`, and `init-global-mappings`.
- Increase test coverage across corelib and UI modules.
- Generate/update dts files for new test additions.

### Bugfixes

- Fix domwise SVG presentation attributes never removed on re-assignment (`removeAttributeNS` called with swapped arguments).
- Fix `<select>` value broken with signals — `assignProp` now handles `HTMLSelectElement` (single & multi-select).
- Fix Show component memory leak — `lifecycleNode` no longer set to non-EventTarget values (e.g. string children).
- Fix `derivedSignal` fallback crash when `subscribe` doesn't fire synchronously.
- Fix `unsignalizePrevClass` to recursively handle nested arrays.
- Fix spellcheck cast from `HTMLInputElement` to `HTMLElement`.
- Fix `getContent` parameter type in Show.
- Fix some potential issues in `ui/helpers` and `ui/widgets`.
- Fix some minor bugs in validator.

## 10.3.4 (2026-07-02)

### Bugfixes

- applyFormatterResultToCellNode sets innerHTML instead of innerText when enableHtmlRendering is false, causing wrong content to be rendered on cell updates

## 10.3.3 (2026-07-01)

### Features
- Add RegisterScoped, RegisterSingleton, RegisterTransient attributes that allow an implementation type to be auto registered for any interface (excluding ones deriving from IRequestHandler) in Startup.cs ConfigureServices method by calling  services.AddAutoRegisteredServices(); which should be the last line in that method.
- Use `node --run scriptname` instead of `pnpm scriptname` or `npm run scriptname`
- Allow passing dialogType option of combo/lookup editors as a type / promise instead of string
- Remove all `javascript:` urls for links as they are incompatible with strict CSP, or replace them with a role="button".
- Expose uploader object via new createUploadInput helper
- Add Blog sample under `Application Samples` (StartSharp)
- Add missing `commitEdits` function to EntityGridDialog. Add test to catch missing EntityDialog prototype functions in EntityGridDialog.

### Bugfixes

- Clear cssColRulesL/R and cssVarRules to avoid issue when switching back from useCssVars in SleekGrid
- Remove `\n` and `\t` from `<span class="counter"/>` texts and make sure display text shown properly even if they have such characters.
- Ensure `HeaderFiltersPlugin.getFilterValue` does not fail when value is null / undefined.

## 10.3.2 (2026-05-05)

### Bugfixes
- Validate the language ID passed is a valid ISO language ID (e.g. en-GB) before saving translations.

## 10.3.1 (2026-04-20)

- Rename useGridEditorSave to useGridEditorServices to better match intent (e.g. save and delete)
- Define hasDeletePermission, hasInsertPermission etc. also in EntityGrid
- Export isAddRowSymbol from corelib
- Auto register function for GridEditController (StartSharp)
- Add hasInsertPermission, hasUpdatePermission and canEditItem options to GridEditController
- Use hasDeletePermission method from the grid if available, or use hasDeletePermission option of GridEditController if specified (StartSharp)
- Add isReadOnly option to GridEditController, if not specified check grid's get_readOnly method / readOnly property (StartSharp)

## 10.3.0 (2026-04-14)

- Set grid's options.editorLock to a new one if default options editorLock is set to null. This allows setting gridDefaults.editorLock to null to automatically create a new one when required. Using GlobalEditorLock may cause some issues between editable grids in different unrelated dialogs.
- Add tabbable option (default true) to columns which controls navigation via tab key. If a column is focusable: false, this property has no effect.
- New TabbableAttribute that sets tabbable option for columns.
- Rename SelectableAttribute to ShowSelectionAttribute to better identify intent and usage in SleekGrid, as it only controls if "selected" css class should be applied to cells, not selectability of a cell.

## 10.2.9 (2026-04-14)

### Bugfixes
- Null reference issue when grid inline editor's inplace add button is clicked and another inplace editor grid is opened

## 10.2.8 (2026-04-14)

### Features
- Add Focusable(false), Selectable(false) to RowEditActionsColumnAttribute
- Derive IPropertySource from IPropertyInfo
- Add origin argument to IPropertyInfo.GetAttribute and IPropertyInfo.GetAttributes method to selectively exclude inherited, intrinsic, based on field attributes.
- Make ReflectedType available through IPropertySource so that IPropertySource.Property does not need to be used and might be removed in the future to allow dynamic/mock props.

## 10.2.7 (2026-04-13)

### Features
- Add SelectableAttribute and FocusableAttribute and use them in setting grid columns corresponding props.
- Add GetAttributesWithIntrinsic helper and use it instead of GetAttributes to also check intrinsic attributes added via IIntrinsicPropertyAttributeProvider in sergen and Pro.Coder for code generation.
- Ensure BasedOnRowPropertyNameAnalyzer takes intrinsic attributes provided via an IIntrinsicPropertyAttributeProvider into account

## 10.2.6 (2026-04-13)

### Bugfixes
- Deprecated GetAttribute method had `inherit: false` as default, instead of GetCustomAttribute which has `inherit: true` by default, resulting in [DefaultHandler] issues when a base class with [DefaultHandler] attribute is used.

## 10.2.5 (2026-04-13)

### Features
- Improve error handling for unhandledRejection (e.g. async) errors
- Add IIntrinsicPropertyAttributeProvider which allows creating combination attributes for usage in forms / columns. 
- RowEditActionsColumnAttribute for grid editors which is a combination attribute implementing IIntrinsicPropertyAttributeProvider. 
- ReflectionExtensions.GetAttribute is now obsolete and will be removed, use GetCustomAttribute.
- Better async error handling for inline editing, also validate invisible columns and show a notification if a required column is invisible (StartSharp)
- Add ability to change autoEdit option in GridEditController (StartSharp)
- Commit current row changes via Ctrl+Enter key in GridEditController (StartSharp)
- Changed default for GridEditControllerOptions.actionsColumn (previously actionColumn) to `RowEditActions` (StartSharp). 
- Add a GridEditController.createActionsColumn function to manually create an `RowEditActions` column (StartSharp).

## 10.2.4 (2026-04-07)

### Features

- Pass the full entity, e.g. merged with existing item to the validateEntity method, instead of only the request.Entity fields which might only include changed fields in inline editing mode.
- Update modified css classes after cancelling row adding (StartSharp)

### Bugfixes
- GridEditController.deleteHandler receives a wrong defaultHandler argument

## 10.2.3 (2026-04-07)

### Features
- GridEditController now support inline row adding. The add row can be placed at the top, bottom and optinally be a fixed/sticky row.
- Switched to TypeScript 6, which has several breaking changes. Most notably, you should set `"strict": false` in your tsconfig.json as `strict` is `true` by default now.
- Support `${configDir}` variable in `tsconfig.json` files which is useful for `extends` and config file relative paths.
- Add `RemoteView.onRecalcRows` event to intercept recalc method before getting the row diffs, e.g. to inject some special rows like special / fixed location ones which are ignored in filtering / grouping calculations. 
- Make it possible to get/set RemoteView.itemMetadataCallback.
- Add a DeleteRowActionFormatter (Serenity.Extensions) and use it in OrderDetailsEditor to add a delete row action column, add getCreateServiceMethod, getDeleteServiceMethod, getUpdateServiceMethod methods to the GridEditorBase.
- Add an async commitEdits method to property grid so that any pending edits can be asynchrously committed before the dialog itself is saved, useful for inline grid editors etc.
- Bootstrap tooltip workaround for https://github.com/twbs/bootstrap/issues/37474
- Switched to xUnit v3 for unit testing
- Switched to Playwright from Selenium for browser functional tests
- Added Recaptcha urls to CSP directives (StartSharp)

### Bugfixes
- Use initial value passed to the addIncludeDeletedToggle callback

## 10.2.2 (2026-03-10)

### Features
- Use logic for always visible / hidden columns also for data grid persistence
- Add sample for adding maximize button to modal in ChartInDialog sample
- Also add dragging ability to the modal in ChartInDialog sample. Split modal utility code.
- Add onSetViewParams event to DataGrid and attach to it instead of onSubmitting event in HeaderFiltersMixin, as if onViewSubmit is manually called instead of prepareSubmit, ExcelExportHelper won't be able to set its filters as onSubmitting won't be triggered.
- Add Criteria and Distinct support to DataExplorer

### Bugfixes
- Fix Columns with FilterOnly attribute or the ones that does not have the ReadPermission should not be shown in the column picker
- Revert unintentional UserPage and AccountPage changes in StartSharp which was made to reproduce a bug

## 10.2.1 (2026-02-17)

### Features
- Skip types from non-local modules in node_modules in addition to types from declaration files in client types generator to avoid generating duplicate editor, formatter attributes etc. for types in project package references.

## 10.2.0 (2026-02-16)

### Features
- New Serenity Developer Tools extension for Chrome
- New `DENY` special permission that denies access to everyone including super admins.
- Define UploadPathHelper.TemporaryFilePrefix and UploadPathHelper.IsTemporaryFile and use them instead of hardcoding temporary/
- Add thumbnail metadata and helpers to find a primary file from the thumbnail file
- Add FileReadAccessAttribute and FileReadAccessSettings that will be used by SecureUploadFileResponder (currently experimental in pro.extensions)
- New SecureUploadFileResponder (in pro.extensions) that checks for related entity access, and optionally permissions while returning files from FilePage/Read.
- Update jQuery to 4.0.0 (served via Serenity.Assets for compat). Note that it has some breaking changes like removing $.isArray, so stay at 3.7.0 via npm if you have problems
- Remove unused global type helpers (Action, Predicate etc.) that sometimes clash with other globals. Include ESNext.Disposable as signals-core requires it
- Try more package names like `@company/package`, `company.package` for an assembly name like `Company.Package` in ServerTypingsGenerator. Also try to parse assembly to package name mappings from package.json.

### Bugfixes
- Issue with generated MVC file when C# keywords like `decimal` are used as `.cshtml` file names.
- Add disposing listener to the target element during addValidationRule to resolve issue when the editor with custom validation rule itself is disposed in inline grid editors and validation callbacks are executed. Also add null coalescing to DateEditor, DateTimeEditor get_value methods for this.domNode? to avoid issues for similar cases.

## 10.1.6 (2026-01-27)

### Bugfixes
- Empty header filter dropdown contents before appending the search results

## 10.1.5 (2026-01-27)

### Bugfixes
- Fix issue with DateOnly field due to the format string extra quote
- Revert Sergen to generate DateTime fields as before for SQL date columns as DateOnlyField is not yet stable

## 10.1.4 (2026-01-27)

### Features
- Add a PropertyProcessor:DefaultSummaryType setting that can be set to None, Disabled, etc., to override the default Sum aggregate type for numeric columns. For example: `services.Configure<PropertyProcessorOptions>(options => options.DefaultSummaryType = SummaryType.None)` sets the default to None but still allows users to choose via the header menu, etc.

### Bugfixes
- The summary footer value is not shown the first time when no columns have summaries and one is set via the column header menu (e.g., when the footer row is toggled).
- Change the min/max values for date/time editor attributes to strings, as DateTime cannot be used as an attribute property value.

## 10.1.3 (2026-01-26)

### Bugfixes
- Handle the issue when a `Sortable(false)` column is included in the server-side generated sorted column list.

## 10.1.2 (2026-01-22)

### Features
- Introduce a new GridUtils.addQuickSearch method with named arguments, replacing the deprecated addQuickSearchInput and addQuickSearchInputCustom methods.
- Handle Shift+Wheel for horizontal scrolling in SleekGrid.

### Bugfixes
- Fix formatting issues with CheckTreeEditor/CheckLookupEditor and the quick search indicator.

## 10.1.1 (2026-01-15)

### Features
- Override Equals and GetHashCode for PageAuthorizeAttribute, as the default implementation of Attribute.Equals throws a stack overflow when the permission keys are the same for the action and controller, causing Swagger to fail when enumerating attributes for the action.

## 10.1.0 (2026-01-12)

### Features
- Add AntiforgeryFilter options: SkipValidationHeaderName (default "X-CSRF-SKIP") and SkipValidationHeaderValue (default "true"). When present in the header, these will skip CSRF validation when using AutoValidateAntiforgeryIgnoreBearerFilter. This makes it easier to work with clients that use cookie authentication (not bearer tokens) by adding the header.

### Bugfixes
- The Serenity.Pro.Extensions global init script passes an invalid case `proExtensions` instead of `proextensions` to initGlobalMappings.

## 2025

All Serenity versions published in the year 2025 (versions 8.8.4 through 10.0.9) can be found in [changelog-2025](docs/changelog/changelog-2025.md).

## 2024

All Serenity versions published in the year 2024 (versions 8.2.1 through 8.8.3) can be found in [changelog-2024](docs/changelog/changelog-2024.md).

## 2023

All Serenity versions published in the year 2023 (versions 6.4.3 through 8.1.5) can be found in [changelog-2023](docs/changelog/changelog-2023.md).

## 2022

All Serenity versions published in the year 2022 (versions 5.1.2 through 6.4.2) can be found in [changelog-2022](docs/changelog/changelog-2022.md).

## 2021

All Serenity versions published in the year 2021 (versions 5.0.20 through 5.1.1) can be found in [changelog-2021](docs/changelog/changelog-2021.md).

## 2020

All Serenity versions published in the year 2020 (versions 3.10.0 through 3.14.5) can be found in [changelog-2020](docs/changelog/changelog-2020.md).

## 2019

All Serenity versions published in the year 2019 (versions 3.8.4 through 3.9.14) can be found in [changelog-2019](docs/changelog/changelog-2019.md).

## 2018

All Serenity versions published in the year 2018 (versions 3.3.15 through 3.8.3) can be found in [changelog-2018](docs/changelog/changelog-2018.md).

## 2017

All Serenity versions published in the year 2017 (versions 2.8.0 through 3.3.14) can be found in [changelog-2017](docs/changelog/changelog-2017.md).

## 2016

All Serenity versions published in the year 2016 (versions 1.8.19 through 2.7.2) can be found in [changelog-2016](docs/changelog/changelog-2016.md).

## 2015

All Serenity versions published in the year 2015 (versions 1.4.7 through 1.8.18) can be found in [changelog-2015](docs/changelog/changelog-2015.md).

## 2014

All Serenity versions published in the year 2014 (versions 1.3.0 through 1.4.6) can be found in [changelog-2014](docs/changelog/changelog-2014.md).