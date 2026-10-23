import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const here = dirname(fileURLToPath(import.meta.url));

const extLibFiles: Record<string, string> = {
    "package.json": JSON.stringify({
        name: "ext-lib",
        version: "1.0.0",
        types: "./index.d.ts",
        exports: { ".": { types: "./index.d.ts" } },
    }, null, 2),
    "index.d.ts": `export interface External { n: number; }
export interface ExternalDetail { m: string; }
`,
};

/**
 * Materializes the tiny `node_modules/ext-lib` fixture. The directory is
 * git-ignored globally, so fresh checkouts don't have it on disk.
 */
export function ensureFixtureNodeModules(): string {
    const extLibDir = join(here, "fixtures", "basic", "node_modules", "ext-lib");
    for (const [name, content] of Object.entries(extLibFiles)) {
        mkdirSync(extLibDir, { recursive: true });
        writeFileSync(join(extLibDir, name), content);
    }
    return extLibDir;
}