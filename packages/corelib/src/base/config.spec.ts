import { afterEach, describe, expect, it, vi } from "vitest";
import { Config, resetApplicationPath, resetLazyTypes } from "./config";

afterEach(() => {
    document.head.querySelectorAll("*").forEach(el => el.remove());
    resetApplicationPath();
    resetLazyTypes();
});

describe("Config.lazyTypeLoader", () => {
    function addLazyTypes(modules: Record<string, string[]>) {
        const script = document.createElement("script");
        script.type = "application/json";
        script.id = "LazyTypes";
        script.textContent = JSON.stringify(modules);
        document.head.appendChild(script);
        resetLazyTypes();
    }

    it("tells whether a type is in the block for an availability check, without importing its module", () => {
        addLazyTypes({ "/does-not-exist.js": ["App.MyFormatter"] });

        expect(Config.lazyTypeLoader("App.MyFormatter", "formatter", true)).toBe(true);
        expect(Config.lazyTypeLoader("App.Other", "formatter", true)).toBe(false);
    });

    it("says nothing is available for an availability check without the block", () => {
        expect(Config.lazyTypeLoader("App.MyFormatter", "formatter", true)).toBe(false);
    });

    it("returns nothing without the block, so that registries report the type as not found", () => {
        expect(Config.lazyTypeLoader("App.MyEditor", "editor")).toBeNull();
    });

    it("returns nothing for keys that are not lazy types, without trying to import them", () => {
        addLazyTypes({ "/other.js": ["App.Other"] });

        expect(Config.lazyTypeLoader("App.MyEditor", "editor")).toBeNull();
    });

    it("reads the block when a type is first requested, so its place in the page doesn't matter", async () => {
        class LateEditor {
        }

        vi.doMock("lazy-types-spec-late", () => ({
            loaderByKey: { "App.Late": async () => LateEditor }
        }));

        resetLazyTypes();
        const script = document.createElement("script");
        script.type = "application/json";
        script.id = "LazyTypes";
        script.textContent = JSON.stringify({ "lazy-types-spec-late": ["App.Late"] });
        document.head.appendChild(script);

        await expect(Config.lazyTypeLoader("App.Late", "editor")).resolves.toBe(LateEditor);
        vi.doUnmock("lazy-types-spec-late");
    });

    it("loads each type through the module listing it", async () => {
        class MyEditor {
        }

        class OtherEditor {
        }

        vi.doMock("lazy-types-spec-a", () => ({
            loaderByKey: { "App.MyEditor": async () => MyEditor }
        }));
        vi.doMock("lazy-types-spec-b", () => ({
            loaderByKey: { "App.OtherEditor": async () => OtherEditor }
        }));

        addLazyTypes({ "lazy-types-spec-a": ["App.MyEditor"], "lazy-types-spec-b": ["App.OtherEditor"] });

        await expect(Config.lazyTypeLoader("App.MyEditor", "editor")).resolves.toBe(MyEditor);
        await expect(Config.lazyTypeLoader("App.OtherEditor", "editor")).resolves.toBe(OtherEditor);
        vi.doUnmock("lazy-types-spec-a");
        vi.doUnmock("lazy-types-spec-b");
    });
});

describe("Config", () => {
    it("has sensible defaults", async () => {
        resetApplicationPath();
        expect(Config.applicationPath).toBe("/");
        expect(Config.emailAllowOnlyAscii).toBe(true);
        expect(Config.rootNamespaces).toEqual(["Serenity"]);
        expect(Config.notLoggedInHandler).toBeNull();
    });

    it("can read applicationPath from a link with ApplicationPath ID", async () => {
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "ApplicationPath";
        document.head.appendChild(link);
        resetApplicationPath();
        expect(Config.applicationPath).toBe("/mypath/some/");
    });

    it("ignores links with ID other than ApplicationPath", async () => {
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "Another";
        document.head.appendChild(link);
        resetApplicationPath();
        expect(Config.applicationPath).toBe("/");
    });

    it("returns Config.applicationPath as defaultReturnUrl", async () => {
        Config.applicationPath = "/mypath/some/";
        expect(Config.defaultReturnUrl()).toBe("/mypath/some/");
    });

});

export { };

