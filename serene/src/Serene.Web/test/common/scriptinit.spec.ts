import { describe, expect, it, vi } from "vitest";

describe("ScriptInit", () => {
    it("localizes using the full culture when available", async () => {
        vi.resetModules();
        document.documentElement.lang = "tr";
        const corelib = await import("@serenity-is/corelib");
        const { gridDefaults } = await import("@serenity-is/sleekgrid");
        await import("../../Modules/Common/ScriptInit");

        expect(corelib.Config.rootNamespaces).toContain("Serene");
        expect(typeof corelib.TranslationConfig.getLanguageList).toBe("function");
        expect(typeof gridDefaults.sanitizer).toBe("function");
        expect(typeof window.onerror).toBe("function");
    });

    it("falls back to the base language for regional cultures", async () => {
        vi.resetModules();
        document.documentElement.lang = "tr-TR";
        const corelib = await import("@serenity-is/corelib");
        await import("../../Modules/Common/ScriptInit");
        expect(corelib.Config.rootNamespaces).toContain("Serene");
    });
});

