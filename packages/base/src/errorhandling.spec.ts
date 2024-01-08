import * as dialogs from "./dialogs";
import * as notify from "./notify";
import { ErrorHandling } from "./errorhandling";

function changeJSDOMURL(url: string) {
    (globalThis as any).jsdom.reconfigure({ url: url });
}

beforeEach(() => {
    jest.restoreAllMocks();
});

describe("showServiceError", function () {
    it("shows ??ERROR?? if error is null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError(null);
        expect(alertSpy).toHaveBeenCalledTimes(1);
        expect(alertSpy).toHaveBeenCalledWith("??ERROR??");
    });

    it("shows ??ERROR?? if error message and code is null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({});
        expect(alertSpy).toHaveBeenCalledTimes(1);
        expect(alertSpy).toHaveBeenCalledWith("??ERROR??");
    });

    it("shows error code if message is undefined", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({ Code: 'Test' });
        expect(alertSpy).toHaveBeenCalledTimes(1);
        expect(alertSpy).toHaveBeenCalledWith("Test");
    });

    it("shows message if both message and code is not null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({ Code: 'TestCode', Message: 'TestMessage' });
        expect(alertSpy).toHaveBeenCalledTimes(1);
        expect(alertSpy).toHaveBeenCalledWith("TestMessage");

    });

    it("shows message if code is undefined", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();
        ErrorHandling.showServiceError({ Message: 'TestMessage' });
        expect(alertSpy).toHaveBeenCalledTimes(1);
        expect(alertSpy).toHaveBeenCalledWith("TestMessage");
    });

});

describe("isDevelopmentMode", function () {
    it("returns true for localhost and loopback", () => {
        var oldLocation = window.location.href;
        try {
            changeJSDOMURL("http://localhost");
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
            changeJSDOMURL("http://test.localhost");
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
            changeJSDOMURL("http://test");
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
        const notifyErrorSpy = jest.spyOn(notify, "notifyError").mockImplementation();
        const setTimeoutSpy = jest.spyOn(window, "setTimeout").mockImplementation();
        jest.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => false);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, new Error("test"));

        expect(notifyErrorSpy).not.toBeCalled();
        expect(setTimeoutSpy).not.toBeCalled();
    });

    it("displayed text contains the passed error details", function () {
        const notifyErrorSpy = jest.spyOn(notify, "notifyError").mockImplementation();
        const setTimeoutSpy = jest.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        jest.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test&><'\"", "test&.js", 13579, 24680);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyErrorSpy).toHaveBeenCalledTimes(1);
        var text = notifyErrorSpy.mock.calls[0][0];
        expect(text).toContain("test&amp;&gt;&lt;&#39;&quot");
        expect(text).toContain("test&amp;.js");
        expect(text).toContain("13579");
        expect(text).toContain("24680");
    });

    it("shows error stack if available", function () {
        const notifyErrorSpy = jest.spyOn(notify, "notifyError").mockImplementation();
        const setTimeoutSpy = jest.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        jest.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, {
            stack: 'xyz',
            toString: () => 'uvw'
        } as any);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyErrorSpy).toHaveBeenCalledTimes(1);
        var text = notifyErrorSpy.mock.calls[0][0];
        expect(text).toContain("xyz");
        expect(text).not.toContain("uvw");
    });

    it("shows error.toString() if stack not available", function () {
        const notifyErrorSpy = jest.spyOn(notify, "notifyError").mockImplementation();
        const setTimeoutSpy = jest.spyOn(window, "setTimeout").mockImplementation((f) => (f as any)());
        jest.spyOn(ErrorHandling, "isDevelopmentMode").mockImplementation(() => true);

        ErrorHandling.runtimeErrorHandler("test", "test.js", 1, 2, {
            toString: () => 'uvw'
        } as any);

        expect(setTimeoutSpy).toHaveBeenCalledTimes(1);
        expect(notifyErrorSpy).toHaveBeenCalledTimes(1);
        var text = notifyErrorSpy.mock.calls[0][0];
        expect(text).toContain("uvw");
    });

});