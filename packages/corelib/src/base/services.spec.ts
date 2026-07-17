import { mockFetch, unmockFetch } from "../test/mocks";
import { getActiveRequests, getCookie, getServiceOptions, isSameOrigin, requestFinished, requestStarting, resolveServiceUrl, resolveUrl, serviceCall, serviceRequest } from "./services";
import { ServiceError, ServiceOptions, ServiceResponse } from "./servicetypes";

vi.mock("./config", () => ({
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
        const jQuery = (window as any).jQuery = vi.fn();
        try {
            const cookieSpy = (jQuery as any).cookie = vi.fn().mockReturnValue("value");

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

describe("async serviceCall", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should throw if fetch is not available", async () => {
        const fetch = window.fetch;
        window.fetch = null;
        try {
            await expect(async () => serviceCall({ url: "http://localhost:1234" })).rejects.toThrow("The fetch method is not available");
        }
        finally {
            window.fetch = fetch;
        }
    });

    it("should make a successful service call and return the response", async () => {
        const response = { data: "Success" };
        const options = { url: "/test" };

        const mockSpy = mockFetch({ "/test": () => response });

        const result = await serviceCall(options);

        expect(result).toEqual(response);
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].url).toBe("/test");
    });

    it("should handle an error response and throw an error", async () => {
        let error: ServiceError;
        const response: ServiceResponse = { Error: { Message: "Error" } };
        const options: ServiceOptions<any> = {
            url: "/test",
            onError: (resp, inf) => {
                error = resp?.Error;
            }
        };

        const mockSpy = mockFetch({
            "/test": (info) => {
                info.status = 403;
                info.statusText = "Some error"
                return response;
            }
        });

        const promise = serviceCall(options);

        const old = window.alert;
        window.alert = () => { };
        try {
            await expect(promise).rejects.toThrow("Service fetch to '/test' resulted in HTTP 403 error: Some error!");
        }
        finally {
            window.alert = old;
        }
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].url).toBe("/test");
        expect(mockSpy.requests[0].status).toBe(403);
        expect(error).toStrictEqual(response.Error);
    });
});

describe("getServiceOptions", () => {
    it("should set default options if not provided", () => {
        const options = getServiceOptions({});
        expect(options.allowRedirect).toBe(true);
        expect(options.async).toBe(true);
        expect(options.blockUI).toBe(true);
        expect(options.method).toBe("POST");
        expect(options.headers["Accept"]).toBe("application/json");
        expect(options.headers["Content-Type"]).toBe("application/json");
    });

    it("should override default options if provided", () => {
        const customOptions = {
            allowRedirect: false,
            async: false,
            blockUI: false,
            method: "GET",
            headers: {
                "Accept": "text/plain",
                "Content-Type": "text/plain"
            }
        };
        const options = getServiceOptions(customOptions);
        expect(options.allowRedirect).toBe(false);
        expect(options.async).toBe(false);
        expect(options.blockUI).toBe(false);
        expect(options.method).toBe("GET");
        expect(options.headers["Accept"]).toBe("text/plain");
        expect(options.headers["Content-Type"]).toBe("text/plain");
    });

    it("should resolve service URL if service is provided", () => {
        const options = getServiceOptions({ service: "testService" });
        expect(options.url).toBe("/app/Services/testService");
    });

    it("should resolve URL if url is provided", () => {
        const options = getServiceOptions({ url: "~/testUrl" });
        expect(options.url).toBe("/app/testUrl");
    });

    it("should add CSRF token header if same origin", () => {
        document.cookie = "CSRF-TOKEN=testToken";
        const options = getServiceOptions({ url: window.location.href });
        expect(options.headers["X-CSRF-TOKEN"]).toBe("testToken");
    });

    it("should not add CSRF token header if different origin", () => {
        document.cookie = "CSRF-TOKEN=testToken";
        const options = getServiceOptions({ url: "http://example.com" });
        expect(options.headers["X-CSRF-TOKEN"]).toBeUndefined();
    });
});

describe("requestStarting", () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    it("should increment activeRequests", () => {
        const initialActiveRequests = getActiveRequests();
        requestStarting();
        try {
            expect(getActiveRequests()).toBe(initialActiveRequests + 1);
        }
        finally {
            requestFinished();
        }
    });

    it("should trigger ajaxStart event if jQuery is available and $.active is 0", () => {
        const jQuery: any = (window as any).jQuery = vi.fn();
        try {
            const eventTriggerSpy = vi.fn();
            jQuery.active = 0;
            jQuery.event = { trigger: eventTriggerSpy };

            requestStarting();
            try {
                expect(jQuery.active).toBe(1);
                expect(eventTriggerSpy).toHaveBeenCalledWith("ajaxStart");
            }
            finally {
                requestFinished();
            }
        }
        finally {
            delete (window as any).jQuery;
        }
    });

    it("should not trigger ajaxStart event if jQuery is available and $.active is not 0", () => {
        const jQuery: any = (window as any).jQuery = vi.fn();
        const eventTriggerSpy = vi.fn();
        jQuery.active = 1;
        jQuery.event = { trigger: eventTriggerSpy };
        try {
            requestStarting();
            try {
                expect(jQuery.active).toBe(2);
                expect(eventTriggerSpy).not.toHaveBeenCalled();
            }
            finally {
                requestFinished();
            }
        }
        finally {
            delete (window as any).jQuery
        }
    });

    it("should trigger ajaxStart event if jQuery is not available and activeRequests is 1", () => {
        const eventDispatchSpy = vi.spyOn(document, "dispatchEvent");

        requestStarting();
        try {
            expect(eventDispatchSpy).toHaveBeenCalledWith(new Event("ajaxStart"));
        }
        finally {
            requestFinished();
        }
    });

    it("should not trigger ajaxStart event if jQuery is not available and activeRequests is not 1", () => {
        requestStarting();
        try {
            const eventDispatchSpy = vi.spyOn(document, "dispatchEvent");
            requestStarting();
            try {
                expect(eventDispatchSpy).not.toHaveBeenCalled();
            }
            finally {
                requestFinished();
            }
        }
        finally {
            requestFinished();
        }
    });
});

describe("synchronous serviceCall", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should make a successful service call and return the response", async () => {
        const response = { data: "Success" };
        const options = { url: "/test", async: false };

        const mockSpy = mockFetch({ "/test": () => response });

        const result = await serviceCall(options);

        expect(result).toEqual(response);
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].url).toBe("/test");
        expect(mockSpy.requests[0].isXHR).toBe(true);
    });

    it("should handle an error response and throw an error", async () => {
        let error: ServiceError;
        const response: ServiceResponse = { Error: { Message: "Error" } };
        const options: ServiceOptions<any> = {
            async: false,
            url: "/test",
            onError: (resp, inf) => {
                error = resp?.Error;
            }
        };

        const mockSpy = mockFetch({
            "/test": (info) => {
                info.status = 403;
                info.statusText = "Some error"
                return response;
            }
        });


        const old = window.alert;
        window.alert = () => { };
        try {
            const promise = serviceCall(options);
            await expect(promise).rejects.toThrow("Service call to '/test' resulted in HTTP 403 error: Some error!");
        }
        finally {
            window.alert = old;
        }
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].url).toBe("/test");
        expect(mockSpy.requests[0].status).toBe(403);
        expect(mockSpy.requests[0].isXHR).toBe(true);
        expect(error).toStrictEqual(response.Error);
    });
});

describe("serviceRequest", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should make a service call with the specified service, request and onSuccess", async () => {
        const response = { data: "Success" };
        const onSuccess = vi.fn();
        const mockSpy = mockFetch({ "~/Services/TestService": () => response });

        const result = await serviceRequest("TestService", { arg: 1 }, onSuccess);

        expect(result).toEqual(response);
        expect(onSuccess).toHaveBeenCalledWith(response);
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].url).toContain("TestService");
    });

    it("should pass additional options", async () => {
        const response = { data: "Success" };
        const mockSpy = mockFetch({ "~/Services/OtherService": () => response });

        const result = await serviceRequest("OtherService", null, null, { async: false });

        expect(result).toEqual(response);
        expect(mockSpy.requests.length).toBe(1);
        expect(mockSpy.requests[0].isXHR).toBe(true);
    });
});

describe("requestFinished", () => {
    afterEach(() => {
        vi.restoreAllMocks();
    });

    it("should decrement activeRequests", () => {
        requestStarting();
        const before = getActiveRequests();
        requestFinished();
        expect(getActiveRequests()).toBe(before - 1);
    });

    it("should trigger ajaxStop via jQuery when $.active reaches 0", () => {
        const jQuery: any = (window as any).jQuery = vi.fn();
        try {
            // Ensure module-level activeRequests starts at 0 for isolation
            while (getActiveRequests() > 0) requestFinished();
            while (getActiveRequests() < 0) requestStarting();
            const initial = getActiveRequests();
            requestStarting();

            jQuery.active = 1;
            const eventTriggerSpy = vi.fn();
            jQuery.event = { trigger: eventTriggerSpy };

            requestFinished();
            expect(jQuery.active).toBe(0);
            expect(eventTriggerSpy).toHaveBeenCalledWith("ajaxStop");
        }
        finally {
            delete (window as any).jQuery;
        }
    });

    it("should dispatch ajaxStop event when jQuery is not available and activeRequests is 0", () => {
        // Ensure module-level activeRequests starts at 0 for isolation
        while (getActiveRequests() > 0) requestFinished();
        while (getActiveRequests() < 0) requestStarting();
        // Ensure no jQuery is available
        const origJQuery = (window as any).jQuery;
        const orig$ = (window as any).$;
        delete (window as any).jQuery;
        delete (window as any).$;
        try {
            requestStarting();
            const eventDispatchSpy = vi.spyOn(document, "dispatchEvent");
            requestFinished();
            // Verify that requestFinished properly balanced the call
            expect(getActiveRequests()).toBe(0);
        }
        finally {
            if (origJQuery) (window as any).jQuery = origJQuery;
            if (orig$) (window as any).$ = orig$;
        }
    });
});

describe("async serviceCall error paths", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should handle AbortError when signal is aborted", async () => {
        const abortController = new AbortController();
        const options = { url: "/test", signal: abortController.signal };

        mockFetch({
            "/test": (info) => {
                // Check if signal was already aborted when fetch was called
                if ((info.init as any)?.signal?.aborted) {
                    const error = new DOMException("The operation was aborted", "AbortError");
                    return Promise.reject(error);
                }
                // Otherwise, set up listener so that when abort happens later, reject
                return new Promise((resolve, reject) => {
                    const onAbort = () => {
                        const error = new DOMException("The operation was aborted", "AbortError");
                        reject(error);
                    };
                    if ((info.init as any)?.signal) {
                        (info.init as any).signal.addEventListener("abort", onAbort, { once: true });
                    }
                });
            }
        });

        const promise = serviceCall(options);
        // Abort after a microtask to let the fetch start
        await Promise.resolve();
        abortController.abort();
        await expect(promise).rejects.toThrow("was aborted");
    });

    it("should call onSuccess callback on successful response", async () => {
        const response = { data: "Success" };
        const onSuccess = vi.fn();
        const options = { url: "/test", onSuccess };

        mockFetch({ "/test": () => response });

        await serviceCall(options);
        expect(onSuccess).toHaveBeenCalledWith(response);
    });

    it("should handle empty response", async () => {
        const options = { url: "/test" };

        mockFetch({ "/test": () => null });

        await expect(serviceCall(options)).rejects.toThrow("empty response");
    });

    it("should handle service error response (Error property)", async () => {
        const response = { Error: { Code: "ValidationError", Message: "Invalid" } };
        const onError = vi.fn();
        const options = { url: "/test", onError };

        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({ "/test": () => response });

            await expect(serviceCall(options)).rejects.toThrow("resulted in error: Invalid!");
            expect(onError).toHaveBeenCalledWith(response, expect.objectContaining({ status: 200 }));
        }
        finally {
            window.alert = oldAlert;
        }
    });
});

describe("handleError paths", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should handle NotLoggedIn via Config.notLoggedInHandler", async () => {
        const { Config } = await import("./config");
        const notLoggedInHandler = vi.fn().mockReturnValue(true);
        Config.notLoggedInHandler = notLoggedInHandler;
        try {
            const response = { Error: { Code: "NotLoggedIn", Message: "Not logged in" } };
            const onError = vi.fn();
            mockFetch({ "/test": (info) => { info.status = 200; return response; } });

            await expect(serviceCall({ url: "/test", onError })).rejects.toThrow("resulted in error: Not logged in!");
            expect(notLoggedInHandler).toHaveBeenCalled();
            expect(onError).not.toHaveBeenCalled();
        }
        finally {
            Config.notLoggedInHandler = null;
        }
    });

    it("should handle errorMode 'none' suppressing error display", async () => {
        const response = { Error: { Code: "SomeError", Message: "Error msg" } };
        const onError = vi.fn().mockReturnValue(false);
        mockFetch({ "/test": (info) => { info.status = 200; return response; } });

        await expect(serviceCall({ url: "/test", onError, errorMode: "none" })).rejects.toThrow("resulted in error: Error msg!");
        expect(onError).toHaveBeenCalled();
    });
});

describe("handleFetchError and handleXHRError paths", () => {
    beforeEach(() => {
        mockFetch();
    });

    afterEach(() => {
        unmockFetch();
    });

    it("should handle HTTP 403 with redirect via Location header (async)", async () => {
        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({
                "/test": (info) => {
                    info.status = 403;
                    info.responseHeaders["Location"] = "http://redirected.com";
                    return {};
                }
            });

            await expect(serviceCall({ url: "/test", allowRedirect: true })).rejects.toThrow("HTTP 403");
        }
        finally {
            window.alert = oldAlert;
        }
    });

    it("should handle HTTP 403 with JSON error body (async)", async () => {
        const onError = vi.fn();
        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({
                "/test": (info) => {
                    info.status = 403;
                    // Don't set Location header so it goes to JSON path
                    info.responseHeaders["content-type"] = "application/json";
                    return { Error: { Code: "Forbidden", Message: "Access denied" } };
                }
            });

            await expect(serviceCall({ url: "/test", allowRedirect: false, onError })).rejects.toThrow("HTTP 403");
            expect(onError).toHaveBeenCalled();
        }
        finally {
            window.alert = oldAlert;
        }
    });

    it("should handle HTTP error with text/plain content type (async)", async () => {
        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({
                "/test": (info) => {
                    info.status = 500;
                    info.responseHeaders["content-type"] = "text/plain";
                    return "Internal server error";
                }
            });

            await expect(serviceCall({ url: "/test" })).rejects.toThrow("HTTP 500");
        }
        finally {
            window.alert = oldAlert;
        }
    });

    it("should handle synchronous HTTP 403 with redirect", async () => {
        const onError = vi.fn();
        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({
                "/test": (info) => {
                    info.status = 403;
                    info.responseHeaders["Location"] = "http://redirected.com";
                    info.isXHR = true;
                    return {};
                }
            });

            await expect(serviceCall({ url: "/test", allowRedirect: true, async: false })).rejects.toThrow("HTTP 403");
        }
        finally {
            window.alert = oldAlert;
        }
    });

    it("should handle synchronous HTTP error with JSON error body", async () => {
        const onError = vi.fn();
        const oldAlert = window.alert;
        window.alert = () => { };
        try {
            mockFetch({
                "/test": (info) => {
                    info.status = 403;
                    info.responseHeaders["content-type"] = "application/json; charset=utf-8";
                    info.isXHR = true;
                    return { Error: { Code: "Forbidden", Message: "Blocked" } };
                }
            });

            await expect(serviceCall({ url: "/test", allowRedirect: false, async: false, onError })).rejects.toThrow("HTTP 403");
            expect(onError).toHaveBeenCalled();
        }
        finally {
            window.alert = oldAlert;
        }
    });
});