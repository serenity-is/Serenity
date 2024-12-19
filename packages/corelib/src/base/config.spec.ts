﻿describe("Config", () => {
    it("has sensible defaults", async () => {
        vi.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/");
        expect(config.emailAllowOnlyAscii).toBe(true);
        expect(config.rootNamespaces).toEqual(["Serenity"]);
        expect(config.notLoggedInHandler).toBeNull();
    });

    it("can read applicationPath from a link with ApplicationPath ID", async () => {
        vi.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "ApplicationPath";
        document.head.appendChild(link);
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/mypath/some/");
    });

    it("ignores links with ID other than ApplicationPath", async () => {
        vi.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "Another";
        document.head.appendChild(link);
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/");
    });

    it("returns Config.applicationPath as defaultReturnUrl", async () => {
        vi.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const config = (await import("./config")).Config;
        config.applicationPath = "/mypath/some/";
        expect(config.defaultReturnUrl()).toBe("/mypath/some/");
    });

});

export { };

