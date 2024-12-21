import { resetApplicationPath, Config } from "./config";

afterEach(() => {
    document.head.childNodes.forEach(el => el.remove());
    resetApplicationPath();
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

