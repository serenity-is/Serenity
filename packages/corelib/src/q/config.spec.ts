describe("Q.config", () => {
    it("has sensible defaults", async () => {
        jest.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/");
        expect(config.emailAllowOnlyAscii).toBe(true);
        expect(config.responsiveDialogs).toBe(true);
        expect(config.bootstrapMessages).toBe(false);
        expect(config.rootNamespaces).toEqual(["Serenity"]);
        expect(config.notLoggedInHandler).toBeNull();
    });

    it("can read applicationPath from a link with ApplicationPath ID", async () => {
        jest.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "ApplicationPath";
        document.head.appendChild(link);
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/mypath/some/");
    });

    it("ignores links with ID other than ApplicationPath", async () => {
        jest.resetModules();
        document.head.childNodes.forEach(el => el.remove());
        const link = document.createElement("link");
        link.href = "/mypath/some/";
        link.id = "Another";
        document.head.appendChild(link);
        const config = (await import("./config")).Config;
        expect(config.applicationPath).toBe("/");
    });    

});

export {}