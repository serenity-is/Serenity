# @serenity-is/dtsbundler

Bundle TypeScript declaration files into a single `.d.ts` file — built on the
**TypeScript 7 (native)** API.

Successor to `dts-bundle-generator` for the Serenity workspace. The whole
pipeline runs in-process against the `typescript` package; no `tsc` subprocess
and no separate JS compiler, so the workspace can drop the
`dts-bundle-generator>typescript` version override.

It is **not** a drop-in replacement for that tool's command line: options were
renamed and some defaults were inverted (see the flag table below), so the
`dts` scripts had to be updated when switching.

## Install

```sh
pnpm add @serenity-is/dtsbundler
```

Requires TypeScript `>= 7.1.0-beta` (the version that stabilized the
`typescript/sync`, `typescript/ast` and `typescript/fs` entry points), declared
as a regular dependency.

## Usage

### CLI

The package ships a `dtsbundler` executable. It has no required arguments — run
it in a package folder and it uses the conventions of this workspace:

```sh
cd packages/corelib
dtsbundler          # src/index.ts -> dist/index.d.ts
```

**Defaults**

| | |
|---|---|
| entry | the single one of `src/index.ts`, `index.ts`, `Modules/index.ts` that exists. If none or several exist, the input file has to be given explicitly. |
| output | `dist/index.d.ts` |
| project | `tsconfig.json` discovered upwards from the entry |

Nothing else is needed: declaration emit happens in-process, so there is no
`tsc --build` step and no intermediate `out/` directory. No banner comment is
ever written to the output.

Inside this repo, run it without installing anything:

```sh
node ../dtsbundler/dist/cli.js                  # from a package folder
node packages/dtsbundler/dist/cli.js packages/sleekgrid/src/index.ts -o packages/sleekgrid/dist/index.d.ts
```

To get a plain `dtsbundler` command in a package's scripts, add it as a dev
dependency (`"@serenity-is/dtsbundler": "workspace:^"`) and run `pnpm install`;
pnpm then links the binary. Once published, `npx @serenity-is/dtsbundler` works
as well.

```sh
dtsbundler --help
```

| Flag | Meaning |
|---|---|
| `-o, --out-file <file>` | output file (default `dist/index.d.ts`) |
| `--project <path>` | `tsconfig.json` to compile with (default: discovered from the entry) |
| `--no-check` | skip type-checking the generated bundle (checked by default) |
| `--no-inline-declare-global` | leave out `declare global` blocks of inlined files (included by default) |
| `--no-inline-declare-externals` | leave out `declare module "..."` augmentation blocks of inlined files (included by default) |
| `--follow-symlinks` | resolve symlinks to their real location (off by default; see below) |
| `--export-referenced-types` | also export declarations that are only referenced (default: off) |
| `--external-inlines <pkg...>` | packages to inline instead of importing |
| `-v, --verbose`, `--silent`, `--version`, `-h, --help` | diagnostics |

Options of dts-bundle-generator that we do not implement
(`--sort`, `--fail-on-class`, `--respect-preserve-const-enum`,
`--external-imports`, `--external-types`, `--umd-module-name`, `--config`) are
rejected with an explicit error rather than silently ignored, so a command line
copied from that tool fails loudly instead of quietly producing a different
bundle. Multiple entry
files are not supported yet either.

When switching, note the renamed/inverted flags:

| dts-bundle-generator | here |
|---|---|
| `--no-banner` | (never writes a banner, nothing to pass) |
| `--disable-symlinks-following` | default behavior; `--follow-symlinks` is the opposite |
| `--inline-declare-global` | default behavior; `--no-inline-declare-global` disables |
| `--inline-declare-externals` | default behavior; `--no-inline-declare-externals` disables |
| `--export-referenced-types` (default on) | off by default; the flag turns it on |
| `-o` required-ish | defaults to `dist/index.d.ts` |
| entry file required | `src/index.ts`, `index.ts` or `Modules/index.ts` by convention |

So a package's script is usually just:

```json
"dts": "dtsbundler"
```

### API

```ts
import { bundleDts, type DtsBundlerOptions } from "@serenity-is/dtsbundler";

const result = bundleDts({
    // may be a .ts source (declaration emit runs in-process) or a .d.ts file
    entry: "src/index.ts",
    outFile: "dist/index.d.ts",
    project: "tsconfig.json",        // optional, discovered from the entry

    inlineDeclareGlobals: true,      // default true
    inlineDeclareExternals: true,    // default true
    exportReferencedTypes: false,    // default false
    externalInlines: ["@preact/signals-core"],
    followSymlinks: false,           // default false
});

result.dts;      // the bundle text
result.outFile;  // absolute path, or undefined when no outFile was given
result.written;  // false when the file already had exactly this content
result.timings;  // { compile, bundle } in milliseconds
```

The output file is only written when its content actually changes, so repeated
builds leave the file (and its mtime) untouched and do not retrigger downstream
watchers.

### Options

| Option | CLI flag | Default |
|---|---|---|
| `entry` | positional (or the convention lookup) | — |
| `outFile` | `-o, --out-file` | none (bundle is only returned) |
| `project` | `--project` | discovered upwards from the entry |
| `followSymlinks` | `--follow-symlinks` | `false` |
| `inlineDeclareGlobals` | `--no-inline-declare-global` | `true` |
| `inlineDeclareExternals` | `--no-inline-declare-externals` | `true` |
| `exportReferencedTypes` | `--export-referenced-types` | `false` |
| `externalInlines` | `--external-inlines` | `[]` |

### Why symlinks are not followed

`--follow-symlinks` is opt-in, and turning it on is usually a mistake. A
workspace dependency (`"@serenity-is/corelib": "workspace:*"`) is a symlink to a
sibling folder, so resolving it produces a path outside `node_modules` — which
looks like an ordinary local source file, so it gets inlined into the bundle
instead of staying an `import`. Measured on this repo:

| package | default (`followSymlinks: false`) | `--follow-symlinks` |
|---|---|---|
| domwise | 165,815 chars | 165,815 chars (no workspace deps inlined) |
| sleekgrid | 166,571 chars | 293,628 chars — `@serenity-is/domwise` inlined |
| corelib | 716,926 chars | 813,921 chars — `@serenity-is/sleekgrid` inlined |

### Augmentations are kept by default

`declare global { … }` and `declare module "package" { … }` blocks are pure
augmentations: nothing references them by name, so they are not reachable
through the export graph. They are carried into the bundle by default, because
dropping one loses API **silently** — the bundle still type-checks, consumers
just stop seeing the merged members.

Both can be turned off with `--no-inline-declare-global` and
`--no-inline-declare-externals`. Relative augmentations
(`declare module "./x"`) are unaffected either way; they are promoted to
top-level declarations.

### What is exported

By default the bundle exports exactly what the entry file exports, and nothing
else. Declarations that are only in the bundle because the public API refers to
them are emitted without an `export` keyword, so they cannot be imported by
name (they still resolve structurally for consumers).

`--export-referenced-types` opts into exporting those too, which makes every
type that shows up in your API importable. Measured impact in this repo:

| package | default | `--export-referenced-types` |
|---|---|---|
| domwise | 82 exports | 297 exports (215 internal JSX attribute interfaces) |
| corelib | 670 | 671 |
| sleekgrid | 141 | 141 (no difference) |

Only type declarations are affected — classes, functions, variables, enums and
namespaces are exported only when the entry exports them, in both modes.

## How it works

1. `api.parseConfigFile` + `api.createProgram` over the entry sources.
2. `program.getDeclarationEmit(fileNames)` produces per-file `.d.ts` in memory.
3. A second program is created over those declarations on a virtual host
   filesystem (the original `.ts` files report as missing, so module resolution
   picks the declarations).
4. The checker walks the entry's exports and transitively inlines every
   declaration that lives in a local file, keeps external packages as
   `import { ... } from "pkg"`, and follows re-export chains
   (`export { x } from "./m"`, `export *`, `export * as ns`) to the real
   declarations.
5. Name collisions are resolved by renaming one side. Declarations exported by
   the entry always keep their name, and two declarations of the *same* symbol
   (interface/namespace merging) are never renamed.
6. Output is emitted from the **original source text** of each statement, with
   renames and `export`/`declare` modifiers applied as text edits, so comments and
   indentation are preserved exactly as written in the declaration file.
   `import("pkg").Type` is rewritten to the plain `Type` (the import statement is
   already there), which matters for generated code that spells its references
   that way.

### Why text instead of the printer

The TypeScript 7 printer cannot emit comments for these nodes: there is no
`setCommentRange`/emit-flags API, and `printer.printNode` drops every JSDoc
block, including the ones on interface members. Emitting the original source
text is the only way to keep documentation comments, which are what users of a
bundled `.d.ts` see in their editor.

The trade-off is that the output keeps the source formatting. It does not
re-wrap long type aliases the way dts-bundle-generator's printer does, so a
line-by-line diff shows layout differences even where the declarations are
identical; compare with `git diff -w` to ignore them. Semicolons and
indentation are taken from the source as well, so a declaration file that omits
`semi` will produce a bundle that omits it too.

## Comparing with an existing bundle

`scripts/compare.mjs` bundles a package with the same flags its `dts` script
uses, overwrites its `dist/index.d.ts`, and reports how the top-level
declarations and their order compare with the committed bundle (read before
overwriting, so `git diff` shows the real change):

```sh
node scripts/compare.mjs ../domwise/src/index.ts ../domwise/tsconfig.json
git -C ../domwise diff -w -- dist/index.d.ts   # -w ignores re-wrapping differences
```

Set `DTSBUNDLER_DEBUG=1` to log the per-statement export decisions.

`DTSBUNDLER_TIMINGS=1` prints how long each stage of the bundling pass takes
(reference discovery, name collision resolution, statement emission, ...), plus
the number of checker round trips, which is what dominates it:

```
[timing] collect entry exports    130.1 ms  (checker calls so far: 0)
[timing] crawl references         166.7 ms  (checker calls so far: 3186)
[timing] emit statements           14.9 ms  (checker calls so far: 3186)
```

## Current status / limitations

- Namespaces and `declare module` blocks are copied through as whole
  statements; relative `declare module "./x"` augmentations are promoted to
  top-level declarations, external ones and `declare global` blocks are carried
  over (on by default).
- `export =`, `export default` and `export * as ns` handling is incomplete.
- A symbol that the entry re-exports under a name that collides with another
  symbol is emitted under its own name; dts-bundle-generator renames the
  private side and appends `export { x$1 as x };`. Both type-check, the shape
  differs.
- Output formatting follows the source (see above), so it will not match
  dts-bundle-generator byte for byte; use `git diff -w` when comparing.

## tsconfig layout

| File | Covers | Purpose |
|---|---|---|
| `tsconfig.json` | `src` | typecheck (`noEmit`), same options as the other packages |
| `test/tsconfig.json` | `test` | extends the root config, adds `node` types for the specs |

## Dogfooding

This package builds its own published types with itself. `pnpm dts` runs the
freshly built `dist/cli.js` over `src/index.ts` and writes the bundled
`dist/index.d.ts` — so the `types` entry point is exactly what the bundler
produces, and any regression shows up immediately:

```json
"dts": "node --run build && node dist/cli.js"
```

The CLI type-checks the result after writing it (`--no-check` disables that),
so a broken bundle fails the `dts` script instead of being published. The
output is only written when it changes, so a no-op `pnpm dts` prints
`Unchanged ...` and leaves the file alone.
`test/bundle.spec.ts` additionally asserts that the self-bundle exposes only
the public API and no internals.

## Development

```sh
pnpm install        # installs the TypeScript 7 beta inside the package
pnpm tsc            # typecheck (root config)
pnpm test           # vitest (bundle + CLI specs, runs against dist)
pnpm build          # esbuild bundle -> dist/index.js, dist/cli.js
pnpm dts            # builds, then bundles the package's own types -> dist/index.d.ts
```
