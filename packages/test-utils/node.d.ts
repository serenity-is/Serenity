// The declarations of node.js: only the Node APIs that the specs use, so a test project needs no types of Node. Importing
// "test-utils/node" also declares import.meta.dirname and import.meta.filename, which vitest has in Node.

declare global {
    interface ImportMeta {
        /** The folder of the current module, e.g. of a spec. */
        dirname: string;
        /** The full path of the current module. */
        filename: string;
    }
}

/** Whether a file or a folder exists. */
export function existsSync(path: string): boolean;

/** An entry of a folder, see readdirSync with withFileTypes. */
export interface Dirent {
    /** The name of the entry. */
    name: string;
    /** The folder of the entry. */
    parentPath: string;
    isFile(): boolean;
    isDirectory(): boolean;
}

/** The names of the entries of a folder, also of its subfolders with recursive. */
export function readdirSync(path: string, options?: { recursive?: boolean }): string[];
/** The entries of a folder, also of its subfolders with recursive. */
export function readdirSync(path: string, options: { withFileTypes: true, recursive?: boolean }): Dirent[];

/** The text of a file. */
export function readFileSync(path: string, encoding: "utf8" | "utf-8"): string;
/** The text of a file. */
export function readFileSync(path: string, options: { encoding: "utf8" | "utf-8" }): string;
/** The bytes of a file. */
export function readFileSync(path: string): Uint8Array;

/** What a path is. */
export function statSync(path: string): { isFile(): boolean; isDirectory(): boolean; size: number; mtimeMs: number };

/** The last part of a path, without the extension when it is given. */
export function basename(path: string, suffix?: string): string;

/** The folder of a path. */
export function dirname(path: string): string;

/** The extension of a path, with the dot. */
export function extname(path: string): string;

/** Joins the parts of a path. */
export function join(...paths: string[]): string;

/** The path from one to another. */
export function relative(from: string, to: string): string;

/** Resolves the parts of a path to an absolute one. */
export function resolve(...paths: string[]): string;

/** The separator of the parts of a path of the platform, "\\" or "/". */
export const sep: "\\" | "/";

/** The path of a file URL. */
export function fileURLToPath(url: string | URL): string;

/** The file URL of a path. */
export function pathToFileURL(path: string): URL;
