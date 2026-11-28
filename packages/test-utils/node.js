// The Node APIs that the specs use, e.g. to read the CSS files of a package, for test projects that don't have the
// types of Node, whose globals, e.g. setTimeout that returns NodeJS.Timeout, conflict with the ones of the DOM. The
// declarations, which are only of what is here, are in node.d.ts, with import.meta.dirname and import.meta.filename.
export { existsSync, readdirSync, readFileSync, statSync } from "fs";
export { basename, dirname, extname, join, relative, resolve, sep } from "path";
export { fileURLToPath, pathToFileURL } from "url";
