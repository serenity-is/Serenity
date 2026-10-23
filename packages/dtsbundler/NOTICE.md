# Third-party notices and license attributions

Package: @serenity-is/dtsbundler

Primary license: MIT (see `LICENSE.md`)

This package which is a part of the Serenity project (https://github.com/serenity-is/Serenity)
incorporates or is derived from third-party material from the projects listed below. The original
copyright notice and the license under which Serenity received such third party material are set
forth below. Serenity project reserves all other rights not expressly granted, whether by implication,
estoppel or otherwise.


-----------------------------------------------------------------
TypeScript

Original source: https://github.com/microsoft/TypeScript

Apache License

Version 2.0, January 2004

http://www.apache.org/licenses/

TypeScript is a runtime dependency of this package and is not bundled with it. It is installed
by the consumer through the `typescript` dependency declared in `package.json`. The full license
text is distributed with the `typescript` package (`LICENSE.txt`).


-----------------------------------------------------------------
dts-bundle-generator

Original source: https://github.com/timocov/dts-bundle-generator

This package is a reimplementation of the bundling approach of dts-bundle-generator on top of
the TypeScript 7 native API: inline the declarations of local files, keep references to external
packages as imports, and resolve name collisions by renaming. No source code was copied.

It is deliberately **not** a command line compatible replacement. Some options were renamed or
inverted to fit this workspace's conventions (`--follow-symlinks` instead of
`--disable-symlinks-following`, `--no-inline-declare-*` because those blocks are kept by
default), some defaults differ (no banner, `exportReferencedTypes` off, output at
`dist/index.d.ts`), and options this package does not implement are rejected instead of being
ignored. Where the output is observable it does follow that tool's conventions: the trailing
`export {};`, one import statement per package, statements separated by a single newline with a
blank line between sections.

MIT License

Copyright (c) 2017 Evgeniy Timokhov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
