import { execFileSync } from "child_process";
import { createHash } from "crypto";
import { existsSync, readdirSync, readFileSync, readlinkSync } from "fs";
import { basename, dirname, join, resolve, sep } from "path";
import { fileURLToPath } from 'url';
import { defineConfig } from "vitest/config";

const testUtils = resolve(join(fileURLToPath(new URL('.', import.meta.url)), './'));
const serenityRoot = resolve(join(testUtils, "../../"));

/**
 * Collects the pnpm links in a node_modules folder as "path|name|target" lines.
 * Workspace links (to source folders) are skipped, only links into a pnpm store matter.
 */
function collectLinks(nodeModules, lines) {
    let entries;
    try {
        entries = readdirSync(nodeModules);
    }
    catch {
        return;
    }

    const add = (full, name) => {
        try {
            const target = readlinkSync(full);
            if (target.includes(sep + ".pnpm" + sep) || target.includes("/.pnpm/"))
                lines.push(nodeModules + "|" + name + "|" + target);
        }
        catch {
            // not a link, ignore
        }
    };

    for (const name of entries) {
        if (name.startsWith("."))
            continue;

        const full = join(nodeModules, name);
        if (name.startsWith("@")) {
            let scoped;
            try {
                scoped = readdirSync(full);
            }
            catch {
                continue;
            }
            for (const sub of scoped)
                add(join(full, sub), name + "/" + sub);
        }
        else
            add(full, name);
    }
}

/**
 * Fingerprints the pnpm links the project resolves its dependencies through,
 * walking up to the workspace root. Cached transforms embed absolute module paths,
 * so a cache filled before "pnpm i" from the other workspace rewrote these links
 * would not match the mocked module ids anymore. Keying the cache by the link state
 * makes such a switch use a separate cache folder instead of stale transforms.
 */
function linkFingerprint(projectRoot) {
    const lines = [];
    let dir = resolve(projectRoot);
    while (true) {
        collectLinks(join(dir, "node_modules"), lines);
        if (existsSync(join(dir, "pnpm-workspace.yaml")))
            break;

        const parent = dirname(dir);
        if (parent === dir)
            break;

        dir = parent;
    }

    if (lines.length === 0)
        return "";

    return createHash("sha1")
        .update(lines.sort().join("\n"))
        .digest("hex")
        .substring(0, 8);
}

/**
 * Options every project shares, exported separately so packages that build their
 * config inline (corelib, domwise, sleekgrid, tsbuild, dtsbundler) do not have to
 * repeat them. Keyed by folder name, which is unique within a workspace.
 */
export function serenityTestOptions(projectRoot) {
    projectRoot = resolve(projectRoot ?? "./");
    const store = linkFingerprint(projectRoot);
    return {
        // one transform cache folder per project and per pnpm store: a shared cache
        // is wiped when the lockfile changes, and parallel "pnpm -r" runs race on it
        fsModuleCachePath: join(serenityRoot, "artifacts", ".vitest-cache",
            basename(projectRoot) + (store ? "-" + store : "")),
        // the first run after a lockfile change is cold for every project, and
        // "pnpm -r" runs them concurrently, so the 5s default is too tight
        testTimeout: 20_000,
        hookTimeout: 20_000
    };
}

export default (opt) => {

    const projectRoot = resolve(opt?.projectRoot ?? "./");

    if ((opt?.dynamicData ?? true)) {
        const isWebProject = projectRoot.indexOf('Serene.Web') >= 0 || projectRoot.indexOf('StartSharp.Web') >= 0;
        if ((isWebProject && !existsSync(resolve(`./dynamic-data/Columns.Administration.Language.json`))) ||
            (!isWebProject && !existsSync(resolve(`${testUtils}/dynamic-data/Columns.Administration.Language.json`)))) {
            if (projectRoot.indexOf('Serene.Web') >= 0 || !tryProject(`${serenityRoot}/..`, "StartSharp"))
                tryProject(`${serenityRoot}/serene`, "Serene");
        }
    }

    const provide = {};
    if (opt?.dynamicData ?? true) {
        for (var folder of [join(testUtils, "dynamic-data"), join(projectRoot, "dynamic-data")]) {
            if (existsSync(folder)) {
                for (var file of readdirSync(folder)) {
                    if (file.endsWith(".json")) {
                        provide["dynamic-data/" + basename(file)] = readFileSync(join(folder, file), "utf8");
                    }
                }
            }
        }
    }

    return defineConfig({
        server: {
            fs: {
                allow: [serenityRoot, testUtils, projectRoot]
            }
        },
        test: {
            name: opt?.name,
            environment: "jsdom",
            ...serenityTestOptions(projectRoot),
            execArgv: [
                Number(process.versions.node.split('.')[0]) >= 25 ? '--no-webstorage' : null
            ].filter(x => x != null),
            pool: "vmThreads",
            provide,
            coverage: {
                reporter: ["json", "html", "text"],
                include: ["Modules/**/*.{ts,tsx}"]
            }
        }
    });
}

function tryProject(root, name) {
    const target = "net10.0";
    const folder = `${root}/src/${name}.Web`;
    const csproj = `${folder}/${name}.Web.csproj`;
    if (!existsSync(csproj))
        return false;

    const debugDll = `${folder}/bin/Debug/${target}/${name}.Web.dll`;
    const releaseDll = `${folder}/bin/Release/${target}/${name}.Web.dll`;

    let debugExists = existsSync(debugDll);
    let releaseExists = !debugExists && existsSync(releaseDll);
    if (!debugExists && !releaseExists) {
        console.info("Building " + csproj + "...");
        execFileSync("dotnet", ["build", csproj, "--p:SkipTSBuild=true"], { timeout: 120000 });
    }

    debugExists = existsSync(debugDll);
    releaseExists = !debugExists && existsSync(releaseDll);
    if (debugExists || releaseExists) {
        console.info("Preparing dynamic data for " + name + " via dotnet " + (debugExists ? debugDll : releaseDll) + " dynamic-data ...");
        execFileSync("dotnet", [debugExists ? debugDll : releaseDll, "dynamic-data"], {
            timeout: 120000,
            cwd: resolve(".").indexOf(name + ".Web") >= 0 ? resolve("./") : testUtils
        });
    }

    return true;
}