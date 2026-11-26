import esbuild from "esbuild";
import { existsSync, mkdirSync, mkdtempSync, readFileSync, rmSync, statSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { buildEsmManifest, esbuildOptions, esmManifestFileName, esmManifestPlugin } from "../dist/index.js";

describe("esm manifest", () => {
    let root;

    function write(file, content) {
        const path = join(root, file);
        mkdirSync(join(path, ".."), { recursive: true });
        writeFileSync(path, content);
    }

    async function buildFixture(plugins = []) {
        write("Modules/Shared/util.ts", "export const util = () => 'util';");
        write("Modules/Shared/lazy.ts", "export const lazy = () => 'lazy';");
        write("Modules/Shared/style.css", ".x { color: red; }");
        write("Modules/Login/LoginPage.ts", "import { util } from '../Shared/util'; console.log(util());");
        write("Modules/Customer/CustomerPage.ts", "import '../Shared/style.css'; import { util } from '../Shared/util'; " +
            "console.log(util()); import('../Shared/lazy').then(m => m.lazy());");
        return await esbuild.build({
            absWorkingDir: root,
            entryPoints: ["Modules/Login/LoginPage.ts", "Modules/Customer/CustomerPage.ts"],
            bundle: true,
            splitting: true,
            format: "esm",
            metafile: true,
            sourcemap: true,
            outbase: "./",
            outdir: "wwwroot/esm",
            chunkNames: "_chunks/[name]-[hash]",
            logLevel: "silent",
            plugins
        });
    }

    function entriesOf(manifest, file) {
        return manifest.files[file].map(i => manifest.entries[i]);
    }

    beforeEach(() => {
        root = mkdtempSync(join(tmpdir(), "tsbuild-manifest-"));
    });

    afterEach(() => {
        rmSync(root, { recursive: true, force: true });
    });

    it("maps each output file to the entries that reach it", async () => {
        const result = await buildFixture();
        const manifest = buildEsmManifest(result.metafile, "wwwroot/esm", root);

        const login = "Modules/Login/LoginPage.js";
        const customer = "Modules/Customer/CustomerPage.js";
        expect(manifest.version).toBe(1);
        expect([...manifest.entries].sort()).toEqual([customer, login]);

        expect(entriesOf(manifest, login)).toEqual([login]);
        expect(entriesOf(manifest, customer)).toEqual([customer]);
        expect(entriesOf(manifest, customer + ".map")).toEqual([customer]);
        expect(entriesOf(manifest, "Modules/Customer/CustomerPage.css")).toEqual([customer]);

        const chunks = Object.keys(manifest.files).filter(x => x.startsWith("_chunks/") && x.endsWith(".js"));
        // the shared util chunk is reached by both, the lazy chunk only by the customer page (dynamic import)
        expect(chunks.some(x => entriesOf(manifest, x).length === 2)).toBe(true);
        const lazyChunk = chunks.find(x => readFileSync(join(root, "wwwroot/esm", x), "utf8").includes("lazy"));
        expect(lazyChunk).toBeDefined();
        expect(entriesOf(manifest, lazyChunk)).toEqual([customer]);
        expect(entriesOf(manifest, lazyChunk + ".map")).toEqual([customer]);

        // the same with the explicit entry points of the build
        const explicit = buildEsmManifest(result.metafile, "wwwroot/esm", root,
            ["Modules/Login/LoginPage.ts", "Modules/Customer/CustomerPage.ts"]);
        expect(explicit).toEqual(manifest);
    });

    it("an entry that is also dynamically imported by another entry stays an entry", () => {
        const output = (imports, entryPoint) => ({ entryPoint, imports, exports: [], inputs: {}, bytes: 1 });
        const metafile = {
            inputs: {},
            outputs: {
                "wwwroot/esm/Modules/A.js": output([{ path: "wwwroot/esm/Modules/B.js", kind: "dynamic-import" }], "Modules/A.ts"),
                "wwwroot/esm/Modules/B.js": output([], "Modules/B.ts")
            }
        };
        expect(buildEsmManifest(metafile, "wwwroot/esm", root, ["Modules/A.ts", "Modules/B.ts"]).entries)
            .toEqual(["Modules/A.js", "Modules/B.js"]);
        // without the entry points, it can only be told from a dynamically imported module by a static import
        expect(buildEsmManifest(metafile, "wwwroot/esm", root).entries).toEqual(["Modules/A.js"]);
    });

    it("ignores outputs outside the output directory and lists unreached outputs with no entries", () => {
        const metafile = {
            inputs: {},
            outputs: {
                "wwwroot/esm/Modules/A.js": { entryPoint: "Modules/A.ts", imports: [{ path: "@serenity-is/corelib", kind: "import-statement", external: true }], exports: [], inputs: {}, bytes: 1 },
                "wwwroot/esm/assets/font.woff2": { imports: [], exports: [], inputs: {}, bytes: 1 },
                "other/B.js": { entryPoint: "B.ts", imports: [], exports: [], inputs: {}, bytes: 1 }
            }
        };
        const manifest = buildEsmManifest(metafile, "wwwroot/esm", root);
        expect(manifest.entries).toEqual(["Modules/A.js"]);
        expect(manifest.files).toEqual({ "Modules/A.js": [0], "assets/font.woff2": [] });
    });

    it("is enabled by default only for esm format builds to an esm folder", () => {
        const hasPlugin = (opt) => esbuildOptions({ entryPoints: [], ...opt }).plugins.some(p => p.name === "esm-manifest");
        expect(hasPlugin({})).toBe(true); // wwwroot/esm by default
        expect(hasPlugin({ outdir: "wwwroot/esm" })).toBe(true);
        expect(hasPlugin({ outdir: "wwwroot\\esm\\" })).toBe(true);
        expect(hasPlugin({ outdir: "dist/" })).toBe(false);
        expect(hasPlugin({ outdir: "wwwroot/esm-old" })).toBe(false);
        expect(hasPlugin({ format: "iife" })).toBe(false);
        expect(hasPlugin({ metafile: false })).toBe(false);
        expect(hasPlugin({ outdir: "dist/", esmManifest: true })).toBe(true);
        expect(hasPlugin({ esmManifest: false })).toBe(false);
    });

    it("plugin writes the manifest to the output directory only when it changes", async () => {
        await buildFixture([esmManifestPlugin()]);
        const file = join(root, "wwwroot/esm", esmManifestFileName);
        expect(existsSync(file)).toBe(true);
        const manifest = JSON.parse(readFileSync(file, "utf8"));
        expect(manifest.entries).toContain("Modules/Login/LoginPage.js");

        writeFileSync(file, readFileSync(file, "utf8")); // same content
        const before = statSync(file).mtimeMs;
        await new Promise(r => setTimeout(r, 20));
        await buildFixture([esmManifestPlugin()]);
        expect(statSync(file).mtimeMs).toBe(before);
    });
});
