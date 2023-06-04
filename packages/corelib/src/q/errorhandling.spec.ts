import * as dialogs from "./dialogs";
import * as notify from "./notify";
import { ErrorHandling } from "./errorhandling";

beforeEach(() => jest.restoreAllMocks());

describe("showServiceError", function () {
    it("shows ??ERROR?? if error is null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError(null);
        expect(alertSpy).toBeCalledTimes(1);
        expect(alertSpy).toBeCalledWith("??ERROR??");
    });

    it("shows ??ERROR?? if error message and code is null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({});
        expect(alertSpy).toBeCalledTimes(1);
        expect(alertSpy).toBeCalledWith("??ERROR??");
    });

    it("shows error code if message is undefined", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({ Code: 'Test' });
        expect(alertSpy).toBeCalledTimes(1);
        expect(alertSpy).toBeCalledWith("Test");
    });

    it("shows message if both message and code is not null", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();;
        ErrorHandling.showServiceError({ Code: 'TestCode', Message: 'TestMessage' });
        expect(alertSpy).toBeCalledTimes(1);
        expect(alertSpy).toBeCalledWith("TestMessage");

    });

    it("shows message if code is undefined", () => {
        var alertSpy = jest.spyOn(dialogs, "alertDialog").mockImplementation();
        ErrorHandling.showServiceError({ Message: 'TestMessage' });
        expect(alertSpy).toBeCalledTimes(1);
        expect(alertSpy).toBeCalledWith("TestMessage");
    });

});

describe("isDevelopmentMode", function () {
    it("returns true for localhost and loopback", () => {
        var oldLocation = window.location;
        try {
            Object.defineProperty(window, "location", { value: new URL("http://localhost"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("https://localhost/test"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("http://127.0.0.1:8000"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("https://127.0.0.1/test"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("http://[::1]:8000"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("https://[::1]/test"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
        }
        finally {
            window.location = oldLocation;
        }
    });

    it("returns true for domains ending with .local or .localhost", () => {
        var oldLocation = window.location;
        try {
            Object.defineProperty(window, "location", { value: new URL("http://test.localhost"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
            Object.defineProperty(window, "location", { value: new URL("https://test.local"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(true);
        }
        finally {
            window.location = oldLocation;
        }
    });

    it("returns false for other hosts", () => {
        var oldLocation = window.location;
        try {
            Object.defineProperty(window, "location", { value: new URL("http://test"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            Object.defineProperty(window, "location", { value: new URL("https://testlocal"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            Object.defineProperty(window, "location", { value: new URL("https://localhost.domain"), writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
            Object.defineProperty(window, "location", { value: new URL("https://127.0.0.2"), writable: true });
            Object.defineProperty(window, "location", { value: { hostname: null }, writable: true });
            expect(ErrorHandling.isDevelopmentMode()).toBe(false);
        }
        finally {
            window.location = oldLocation;
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

        expect(setTimeoutSpy).toBeCalledTimes(1);
        expect(notifyErrorSpy).toBeCalledTimes(1);
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

        expect(setTimeoutSpy).toBeCalledTimes(1);
        expect(notifyErrorSpy).toBeCalledTimes(1);
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

        expect(setTimeoutSpy).toBeCalledTimes(1);
        expect(notifyErrorSpy).toBeCalledTimes(1);
        var text = notifyErrorSpy.mock.calls[0][0];
        expect(text).toContain("uvw");
    });

});