import { isSameOrigin, resolveServiceUrl, resolveUrl } from "./services";
import { getCookie } from "./services";

jest.mock("./config", () => ({
    __esModule: true,
    Config: {
        applicationPath: "/app/"
    }
}));

describe("resolveUrl", () => {
    it("should resolve URL that starts with ~/", () => {
        const url = "~/test";
        const resolvedUrl = resolveUrl(url);
        expect(resolvedUrl).toBe("/app/test");
    });

    it("should return the same URL if it starts with /", () => {
        const url = "/test";
        const resolvedUrl = resolveUrl(url);
        expect(resolvedUrl).toBe(url);
    });

    it("should return the same URL if it contains ://", () => {
        const url = "http://example.com/test";
        const resolvedUrl = resolveUrl(url);
        expect(resolvedUrl).toBe(url);
    });
});

describe("resolveServiceUrl", () => {
    it("should return URL as is if URL is undefined", () => {
        const url: string = undefined;
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBeUndefined();
    });

    it("should return URL as is if URL is null", () => {
        const url: string = null;
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBeNull();
    });

    it("should return URL as is if URL is empty", () => {
        const url: string = "";
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBe("");
    });

    it("should return URL as is if URL starts with /", () => {
        const url: string = null;
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBe(url);
    });

    it("should use resolveUrl if URL starts with ~/", () => {
        const url = "~/test";
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBe("/app/test");
    });

    it("should return URL as is if URL contains ://", () => {
        const url = "http://example.com/test";
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBe(url);
    });

    it("should resolve service URL if does not start with / or ~/ and does not contain ://", () => {
        const url = "test";
        const resolvedUrl = resolveServiceUrl(url);
        expect(resolvedUrl).toBe("/app/Services/test");
    });
});

describe("getCookie", () => {
    it("should return the value of the specified cookie", () => {
        document.cookie = "cookie1=value1";
        document.cookie = "cookie2=value2";

        const cookieValue = getCookie("cookie2");

        expect(cookieValue).toBe("value2");
    });

    it("should return an empty string if the specified cookie does not exist", () => {
        const cookieValue = getCookie("nonexistentCookie");

        expect(cookieValue).toBeUndefined();
    });

    it("should use jQuery.cookie plugin if available", () => {
        const jQuery = (window as any).jQuery = jest.fn();
        try {
            const cookieSpy = (jQuery as any).cookie = jest.fn().mockReturnValue("value");

            const cookieValue = getCookie("cookie");

            expect(cookieValue).toBe("value");
            expect(cookieSpy).toHaveBeenCalledWith("cookie");
        }
        finally {
            delete (jQuery as any).cookie;
        }
    });
});

describe("isSameOrigin", () => {
    it("should return true if the URL has the same origin as the current page", () => {
        const url = window.location.protocol + "//" + window.location.host + "/test";
        const isSame = isSameOrigin(url);
        expect(isSame).toBe(true);
    });

    it("should return false if the URL has a different origin than the current page", () => {
        const url = "http://example.com/test";
        const isSame = isSameOrigin(url);
        expect(isSame).toBe(false);
    });

    it("should return false if the URL has the same origin as the current page with a different port", () => {
        const url = window.location.protocol + "//" + window.location.hostname + ":9991/test";
        const isSame = isSameOrigin(url);
        expect(isSame).toBe(false);
    });

    it("should return false if the URL has the same origin as the current page with a different protocol", () => {
        const url = "https://" + window.location.hostname + ":4431/test";
        const isSame = isSameOrigin(url);
        expect(isSame).toBe(false);
    });

    it("should return false if the URL has the same origin as the current page with a different subdomain", () => {
        const url = window.location.protocol + "//" + "subdomain." + window.location.hostname + "/test";
        const isSame = isSameOrigin(url);
        expect(isSame).toBe(false);
    });
});
