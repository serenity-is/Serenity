<!-- Repository-level Copilot instructions for Serenity platform -->
# Copilot instructions — Serenity (repo root)

Key points (quick):
- Root build is driven by MSBuild/Visual Studio (`Serenity.sln`) for .NET projects and `pnpm`/`node` for TypeScript packages under `packages/` and `src/typescript`.
- Many packages (e.g., `packages/corelib`, `packages/sleekgrid`) contain their own `README.md` and a `.github/copilot-instructions.md` with package-specific details — check those for specialized rules.
- Use project scripts (`pnpm -r build`, `pnpm -r tsc`, `pnpm -r test`) from the repository root where appropriate; .NET builds are via `build\build.csproj` or opening `Serenity.sln` in Visual Studio.

Repository architecture (concise):
- .NET backend: projects live under `src/` (e.g., `src/core/Serenity.Net.Core.csproj`, `src/web/Serenity.Net.Web.csproj`) and are composed into `Serenity.sln`. These projects provide services, code-generation hooks, and MSBuild targets that integrate TypeScript builds.
- TypeScript frontend and libraries: under `packages/` and `serene/` (app template). Notable package: `packages/corelib` — core TypeScript utilities, event system, UI widgets, and runtime registration conventions.
- Common-features: reusable .NET features and demo projects in `common-features/`.
- Build/release orchestration: top-level `build/` contains MSBuild helpers; `build.cmd` invokes `dotnet run --project build\build.csproj`.

Critical developer workflows (explicit commands)
- Full repo (Windows/pwsh):
  - Install Node deps (root):
    ```powershell
    pnpm install
    ```
  - Build TypeScript packages and .NET projects (recommended):
    ```powershell
    # from repo root
    dotnet run --project .\build\build.csproj --
    # or use the simpler wrapper (Windows)
    .\build.cmd
    ```

    Note that `build` does not do type checking; use `tsc` for that. `pnpm -r dts` uses typescript to generate bundled declaration files so running it is recommended when changing public APIs and it also covers type checking. `test` also runs `build` first.

  - Run an individual .NET project (example web):
    ```powershell
    dotnet run --project .\serene\src\Serene.Web\Serene.Web.csproj
    ```

  - Run script tests for individual package from the package folder
    ```powershell
    pnpm test
    ```

- Don't use ` -- ` to pass args to any of the pnpm commands, e.g. use `pnpm test -t MyTest` instead of `pnpm test -- -t MyTest`, use `pnpm test --coverage` instead of `pnpm test -- --coverage`.
- Check current directory with `pwd` if unsure where you are in the repo or an individual package.
- Assume working in a windows environment unless otherwise specified, so tools like grep, sed, awk may not be available. Powershell equivalents are preferred.

Important conventions and patterns (repo-specific)
- Global registration & runtime metadata: The TypeScript corelib relies on symbol-based registration for enums, types and script data. See `packages/corelib/src/base/system.ts` and `scriptdata.ts` (package `@serenity-is/corelib`) for examples. When adding runtime metadata follow existing register/unregister helpers.
- Event and UI patterns: UI components and editors use a custom event system (look in `packages/corelib/src/base/` for event utilities and `src/ui/` for editor patterns). Prefer reusing existing factories (dialogs, editors) rather than creating ad-hoc DOM manipulation.
- .NET ↔ TypeScript integration: MSBuild targets call TypeScript build steps via `RunTSBuild` and related targets in `src/Directory.Build.targets` and `build/Package.Build.props`. Changing package outputs often requires updating the corresponding `.csproj` or MSBuild props.
- Packaging: Some packages are consumed via NuGet wrappers that place JS under `node_modules/.dotnet/<package>`; check `packages/*/README.md` which documents the hybrid NuGet/NPM usage.
- Data binding patterns: Use lookup system for dropdowns/editors (`LookupEditor` in `packages/corelib/src/ui/editors/lookupeditor.ts`). Script data system handles dynamic content loading via `~/DynamicData/` endpoints.
- Testing: Vitest workspace configured in `vitest.workspace.json` runs tests across packages. Use `pnpm -r test` from root.

Examples to copy patterns from
- Adding a new editor widget: model after `packages/corelib/src/ui/editors/lookupeditor.ts` and `src/ui/editors/*.tsx` — the pattern uses `ScriptData` binding and global registration.
- Runtime metadata registration: use `registerEnum(myEnum, "MyEnum")` or `registerClass(MyClass, "MyClass")` from `packages/corelib/src/base/system.ts`.
- MSBuild integration: examine `src/Directory.Build.targets` and `build/Package.Build.props` for targets that run TS build and package artifacts.
- Event handling: see fluent-events in `packages/corelib/src/base/fluent-events.ts` for the custom event system.

Where to look first (high-signal files)
- `Serenity.sln` — overall project composition and which projects are built together.
- `build/build.csproj` and `build.cmd` — orchestrates higher-level build steps for the repo.
- `packages/corelib/.github/copilot-instructions.md` and `packages/corelib/README.md` — package-specific agent guidance and examples.
- `package.json` (repo root) and `pnpm-workspace.yaml` — workspace and scripts for TypeScript tooling.
- `vitest.workspace.json` — test configuration across packages.
- `src/Directory.Build.targets` — MSBuild-TypeScript integration patterns.

Editing guidance (do's and don'ts for AI agents)
- Do: follow existing registration/factory patterns in `packages/corelib` when introducing new runtime behavior (events, lookups, formatters).
- Do: run `pnpm -r build` and `pnpm -r tsc` after TS changes; run `dotnet build` or the `build` target for .NET changes that impact packaging.
- Do: use `registerEnum()` and `registerClass()` for runtime metadata instead of ad-hoc global assignments.
- Do: prefer `LookupEditor` subclasses for data-bound dropdowns with automatic script data loading.
- Don't: modify MSBuild props or solution structure without confirming where the change is required — these files affect packaging and CI.
- Don't: assume NPM-only packaging — some packages are wired to NuGet and MSBuild; check `packages/*/README.md`.
- Don't: create custom DOM manipulation when existing UI factories exist in `packages/corelib/src/ui/`.

CI and release notes
- CI is wired to the solution and runs tests/builds (see `.github/workflows` for workflow definitions). For releases, artifacts are produced in `artifacts/bin/` (check `build/` targets).

Troubleshooting
- **Build fails with "pnpm not found"**: Ensure pnpm is installed globally (`npm install -g @pnpm/exe`) and the workspace file `pnpm-workspace.yaml` is present.
- **TypeScript errors after changes**: Run `pnpm -r tsc` to check types; if issues persist, check `tsconfig.json` in affected packages.
- **NuGet packaging issues**: Verify `node_modules/.dotnet/<package>` paths match `.csproj` expectations; run `dotnet restore` first.
- **Runtime registration not working**: Ensure enums/classes are registered with `registerEnum()`/`registerClass()` and check symbol exports.

Repository architecture (concise):
- .NET backend: projects live under `src/` (e.g., `src/core/Serenity.Net.Core.csproj`, `src/web/Serenity.Net.Web.csproj`) and are composed into `Serenity.sln`. These projects provide services, code-generation hooks, and MSBuild targets that integrate TypeScript builds.
- TypeScript frontend and libraries: under `packages/` and `serene/` (app template). Notable package: `packages/corelib` — core TypeScript utilities, event system, UI widgets, and runtime registration conventions.
- Common-features: reusable .NET features and demo projects in `common-features/`.
- Build orchestration: top-level `build/` contains MSBuild helpers; `build.cmd` invokes `dotnet run --project build\build.csproj`.

Critical developer workflows (explicit commands)
- Full repo (Windows/pwsh):
  - Install Node deps (root):
    ```powershell
    pnpm install
    ```
  - Build TypeScript packages and .NET projects (recommended):
    ```powershell
    # from repo root
    dotnet run --project .\build\build.csproj --
    # or use the simpler wrapper (Windows)
    .\build.cmd
    ```
  - Quick TypeScript-only work:
    ```powershell
    pnpm -r build
    pnpm -r tsc
    pnpm -r test
    ```

    Note that `build` does not do type checking; use `tsc` for that. `pnpm -r dts` uses typescript to generate bundled declaration files so running it is recommended when changing public APIs and it also covers type checking. `test` also runs `build` first.

  - Run an individual .NET project (example web):
    ```powershell
    dotnet run --project .\serene\src\Serene.Web\Serene.Web.csproj
    ```

Important conventions and patterns (repo-specific)
- Global registration & runtime metadata: The TypeScript corelib relies on symbol-based registration for enums, types and script data. See `packages/corelib/src/system.ts` and `scriptdata.ts` (package `@serenity-is/corelib`) for examples. When adding runtime metadata follow existing register/unregister helpers.
- Event and UI patterns: UI components and editors use a custom event system (look in `packages/corelib/src/base/` for event utilities and `src/ui/` for editor patterns). Prefer reusing existing factories (dialogs, editors) rather than creating ad-hoc DOM manipulation.
- .NET ↔ TypeScript integration: MSBuild targets call TypeScript build steps via `RunTSBuild` and related targets in `src/Directory.Build.targets` and `build/Package.Build.props`. Changing package outputs often requires updating the corresponding `.csproj` or MSBuild props.
- Packaging: Some packages are consumed via NuGet wrappers that place JS under `node_modules/.dotnet/<package>`; check `packages/corelib/README.md` which documents the hybrid NuGet/NPM usage. The files for referenced projects / NPM packages are automatically copied to the node_modules/.dotnet paths by the `RestoreNodeTypes` target defined in `Serenity.Net.Web.targets` which is executed as `dotnet build -target:RestoreNodeTypes` via the preinstall/pnpm:devPreinstall script in the consuming project's package.json. This target also updates package.json's dependencies list if necessary. Check serene/src/Serene.Web/package.json for an example.

Testing and verification
- Unit tests for TypeScript use Vitest. See `package.json` dev scripts and `pnpm -r test` to run across workspace. Coverage outputs are in per-package `coverage/` folders. Use clover.xml or coverage-final.json files. Uncovered lines can be extracted from coverage/coverage-final.json using Get-Content and ConvertFrom-Json via powershell. It is also possible to extract line coverage for an individual source file by searching for a corresponding `.html` file under `coverage/` folder. For example, the coverage for `packages/corelib/src/base/authorization.ts` can be found under `packages/corelib/coverage/base/authorization.ts.html` file.
  - To extract uncovered lines for a specific TypeScript file from `coverage-final.json`:
    ```powershell
    # Run from the package directory (e.g., packages/corelib) after running tests with coverage
    $json = Get-Content "coverage/coverage-final.json" | ConvertFrom-Json
    # The JSON key is the full absolute path to the file (Windows-style with backslashes)
    # Example for packages/corelib/src/slick/remoteview.ts:
    $fileKey = "P:\startsharp\Serenity\packages\corelib\src\slick\remoteview.ts"
    $data = $json.$fileKey
    $uncovered = @()
    foreach ($key in $data.s.PSObject.Properties.Name) {
        if ($data.s.$key -eq 0) {
            $stmt = $data.statementMap.$key
            for ($line = $stmt.start.line; $line -le $stmt.end.line; $line++) {
                $uncovered += $line
            }
        }
    }
    $uncovered | Sort-Object -Unique
    ```
    This outputs a sorted list of line numbers with zero coverage. Adjust `$fileKey` to match the target file's path.
- .NET tests live under `tests/` and can be run via `dotnet test` for the specific test project.

Examples to copy patterns from
- Adding a new editor widget: model after `packages/corelib/src/ui/editors/lookupeditor.spec.tsx` and `src/ui/editors/*.tsx` — the pattern uses `ScriptData` and global registration.
- Adding runtime metadata: use `system.ts` registration helpers (search for `.register` and `Symbol.for('...')` in `packages/corelib/src`).
- MSBuild integration: examine `src/Directory.Build.targets` and `build/Package.Build.props` for targets that run TS build and package artifacts.

Where to look first (high-signal files)
- `Serenity.sln` — overall project composition and which projects are built together.
- `build/build.csproj` and `build.cmd` — orchestrates higher-level build steps for the repo.
- `packages/corelib/.github/copilot-instructions.md` and `packages/corelib/README.md` — package-specific agent guidance and examples.
- `package.json` (repo root) and `pnpm-workspace.yaml` — workspace and scripts for TypeScript tooling.

Editing guidance (do's and don'ts for AI agents)
- Do: follow existing registration/factory patterns in `packages/corelib` when introducing new runtime behavior (events, lookups, formatters).
- Do: run `pnpm -r build` and `pnpm -r tsc` after TS changes; run `dotnet build` or the `build` target for .NET changes that impact packaging.
- Don't: modify MSBuild props or solution structure without confirming where the change is required — these files affect packaging and CI.
- Don't: assume NPM-only packaging — some packages are wired to NuGet and MSBuild; check `packages/*/README.md`.

CI and release notes
- CI is wired to the solution and runs tests/builds (see `.github/workflows` for workflow definitions). For releases, artifacts are produced in `artifacts/bin/` (check `build/` targets).

Troubleshooting
- **Build fails with "pnpm not found"**: Ensure pnpm is installed globally (`npm install -g @pnpm/exe`) and the workspace file `pnpm-workspace.yaml` is present.
- **TypeScript errors after changes**: Run `pnpm -r tsc` to check types; if issues persist, check `tsconfig.json` in affected packages.
- **NuGet packaging issues**: Verify `node_modules/.dotnet/<package>` paths match `.csproj` expectations; run `dotnet restore` first.