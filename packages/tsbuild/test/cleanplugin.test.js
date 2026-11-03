import { existsSync, mkdirSync, mkdtempSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { cleanPlugin } from "../dist/index.js";

describe("cleanPlugin", () => {
    let root;

    function touch(file) {
        mkdirSync(join(file, ".."), { recursive: true });
        writeFileSync(file, "");
    }

    function runPlugin(initialOptions, outputs) {
        let onEnd;
        cleanPlugin({ logDeletedFiles: false }).setup({
            initialOptions,
            onEnd: (callback) => onEnd = callback
        });
        onEnd({ metafile: { outputs } });
    }

    beforeEach(() => {
        root = mkdtempSync(join(tmpdir(), "tsbuild-clean-"));
    });

    afterEach(() => {
        vi.restoreAllMocks();
        rmSync(root, { recursive: true, force: true });
    });

    it("deletes only extra files, relative to the working directory of the build", () => {
        const project = join(root, "project");
        const kept = join(project, "wwwroot/esm/kept.js");
        const nested = join(project, "wwwroot/esm/sub/nested.js");
        const extra = join(project, "wwwroot/esm/extra.js");
        [kept, nested, extra].forEach(touch);

        runPlugin({ absWorkingDir: project, outdir: "wwwroot/esm" }, {
            "wwwroot/esm/kept.js": {},
            "wwwroot/esm/sub/nested.js": {}
        });

        expect(existsSync(kept)).toBe(true);
        expect(existsSync(nested)).toBe(true);
        expect(existsSync(extra)).toBe(false);
    });

    it("does not touch the same relative folder under the current directory", () => {
        const project = join(root, "project");
        const other = join(root, "other");
        const output = join(project, "wwwroot/esm/output.js");
        const otherFile = join(other, "wwwroot/esm/other.js");
        [output, otherFile].forEach(touch);

        // the current directory has moved on from the project being rebuilt
        vi.spyOn(process, "cwd").mockReturnValue(other);

        runPlugin({ absWorkingDir: project, outdir: "wwwroot/esm" }, {
            "wwwroot/esm/output.js": {}
        });

        expect(existsSync(otherFile)).toBe(true);
        expect(existsSync(output)).toBe(true);
    });

    it("ignores a missing output directory", () => {
        expect(() => runPlugin({ absWorkingDir: root, outdir: "wwwroot/esm" }, {})).not.toThrow();
    });
});
