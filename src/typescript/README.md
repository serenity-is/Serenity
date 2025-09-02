TypeScript Scanner and Parser (AST) Implementation
==================================================

This project is a one to one conversion of TypeScript Compiler's Scanner and Parser modules to C#. 

It is meant to be used in source generators to obtain an AST of TypeScript files without having to call a Javascript runtime like Node.

It produces the same set of tokens / nodes for about 6k+ test cases generated from TypeScript repository's own test cases under `test/cases/compiler` directory.

While it takes about 10+ minutes to parse and generate the test cases via TypeScript compiler itself, the tests run in a few seconds in .NET version. This means the performance should be much better which is important for source generators.

