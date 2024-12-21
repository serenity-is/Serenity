// @ts-ignore
import { type Mock } from "vitest";
import { alertDialog, iframeDialog } from "./dialogs";
import { ErrorHandling } from "./errorhandling";
import { notifyError } from "./notify";

vi.mock(import("./dialogs"), async () => {
    return {
        alertDialog: vi.fn(),
        iframeDialog: vi.fn()
    };
});

vi.mock(import("./notify"), async () => {
    return {
        notifyError: vi.fn()
    };
});

beforeEach(() => {
    vi.clearAllMocks();
});

describe("showServiceError", function () {
    it("shows generic message if error is null", () => {
        ErrorHandling.showServiceError(null);
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("An error occurred while processing your request.");
    });

    it("shows generic message if error message and code is null", () => {
        ErrorHandling.showServiceError({});
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("An error occurred while processing your request.");
    });

    it("shows error code if message is undefined", () => {
        ErrorHandling.showServiceError({ Code: 'Test' });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("Test");
    });

    it("shows message if both message and code is not null", () => {
        ErrorHandling.showServiceError({ Code: 'TestCode', Message: 'TestMessage' });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("TestMessage");
    });

    it("shows message if code is undefined", () => {
        ErrorHandling.showServiceError({ Message: 'TestMessage' });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("TestMessage");
    });

    it("shows iframe dialog if errorInfo has responseText", () => {
        ErrorHandling.showServiceError(null, { responseText: "<html>Some error message</html>" });
        expect(iframeDialog).toHaveBeenCalledTimes(1);
        expect(iframeDialog).toHaveBeenCalledWith({ html: "<html>Some error message</html>" });
    });

    it("shows alert dialog for unknown AJAX connection error", () => {
        ErrorHandling.showServiceError(null, { statusText: "something" });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("An error occured while connecting to the server.");
    });

    it("shows alert dialog for HTTP 500 error", () => {
        ErrorHandling.showServiceError(null, { status: 500 });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("Internal Server Error (500).");
    });

    it("shows alert dialog for other HTTP errors", () => {
        ErrorHandling.showServiceError(null, { status: 404 });
        expect(alertDialog).toHaveBeenCalledTimes(1);
        expect(alertDialog).toHaveBeenCalledWith("HTTP Error 404.");
    });

});

describe("isDevelopmentMode", function () {
    it("returns true for localhost and loopback", () => {
        var oldLocation = window.location.href;
        try {
            if (!changeJSDOMURL("http://localhost"))
                return;
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://localhost/test");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://127.0.0.1:8000");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://127.0.0.1/test");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://[::1]:8000");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://[::1]/test");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
        }
        finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns true for domains ending with .local or .localhost", () => {
        var oldLocation = window.location.href;
        try {
            if (!changeJSDOMURL("http://test.localhost"))
                return;
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            changeJSDOMURL("http://test.local");
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
        }
        finally {
            changeJSDOMURL(oldLocation);
        }
    });

    it("returns false for other hosts", () => {
        var oldLocation = window.location.href;
        try {
            if (!changeJSDOMURL("http://test"))
                return;
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            changeJSDOMURL("http://testlocal");
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            changeJSDOMURL("http://localhost.domain");
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            changeJSDOMURL("http://127.0.0.2");
        }
        finally {
            changeJSDOMURL(oldLocation);
        }
    });
});

describe("runtimeErrorHandler", function () {
    it("ignores if isDevelopmentMode() return false", function () {
        
        const setTimeoutSpy = vi.spyOn(window, "setTimeout").mockImplementation(() => ({} as any));
        vi.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => false);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, new Error("test"));

        expect(notifyError).not.toHaveBeenCalled();
        expect(setTimeoutSpy).not.toHaveBeenCalled();
    });

    it("displayed text contains the passed error details", function () {
        const setTimeoutSpy = vi.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        vi.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test&><'\"", "test&.js", 13579, 24680);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyError).toHaveBeenCalledTimes(1);
        var text = vi.mocked(notifyError).mock.calls[0][0];
        expect(text).toContain("test&amp;&gt;&lt;&#39;&quot");
        expect(text).toContain("test&amp;.js");
        expect(text).toContain("13579");
        expect(text).toContain("24680");
    });

    it("shows error stack if available", function () {
        const setTimeoutSpy = vi.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        vi.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, {
            stack: 'xyz',
            toString: () => 'uvw'
        } as any);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyError).toHaveBeenCalledTimes(1);
        var text = vi.mocked(notifyError).mock.calls[0][0];
        expect(text).toContain("xyz");
        expect(text).not.toContain("uvw");
    });

    it("shows error.toString() if stack not available", function () {
        const setTimeoutSpy = vi.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        vi.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, {
            toString: () => 'uvw'
        } as any);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyError).toHaveBeenCalledTimes(1);
        var text = vi.mocked(notifyError).mock.calls[0][0];
        expect(text).toContain("uvw");
    });

});

describe("unhandledRejectionHandler", function () {
    let preventDefault: Mock;
    beforeEach(() => {
         preventDefault = vi.fn(); 
    });

    it("ignores if err or err.reason is null", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler(null);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).not.toHaveBeenCalled();

        ErrorHandling.unhandledRejectionHandler({ reason: null } as any);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).not.toHaveBeenCalled();
    });

    it("ignores if reason.origin is not 'serviceCall'", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "other" } } as any);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).not.toHaveBeenCalled();
    });

    it("does not log if reason.silent is true and kind is exception", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall", silent: true, kind: "exception" } } as any);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).toHaveBeenCalled();
    });

    it("does not log if reason.silent is true and kind is not exception", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall", silent: true, kind: "other" } } as any);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).toHaveBeenCalled();
    });


    it("does not log if reason.silent is null and kind is not exception", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall", silent: null, kind: "other" } } as any);
        expect(consoleErrorSpy).not.toHaveBeenCalled();
        expect(preventDefault).toHaveBeenCalled();
    });    

    it("logs error if reason.silent is false and kind is 'exception'", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall", silent: false, kind: "exception" } } as any);
        expect(consoleErrorSpy).toHaveBeenCalledTimes(1);
        expect(preventDefault).toHaveBeenCalled();
    });

    it("logs error if reason.silent is undefined and kind is 'exception'", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall", kind: "exception" } } as any);
        expect(consoleErrorSpy).toHaveBeenCalledTimes(1);
        expect(preventDefault).toHaveBeenCalled();
    });    

    it("logs error if reason.silent is undefined and kind is undefined", function () {
        const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => ({} as any));

        ErrorHandling.unhandledRejectionHandler({ preventDefault, reason: { origin: "serviceCall" } } as any);
        expect(consoleErrorSpy).toHaveBeenCalledTimes(1);
        expect(preventDefault).toHaveBeenCalled();
    });   
});

function changeJSDOMURL(url: string) {
    if ((globalThis as any).jsdom?.reconfigure) {
        (globalThis as any).jsdom.reconfigure({ url: url });
        return true;
    }
    else
        return false;
}
