import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, typeText } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import pageInit from "../../../../Modules/Membership/Account/Login/LoginPage";
import { LoginForm } from "../../../../Modules/ServerTypes/Membership";

let calls: any[];

beforeAll(() => {
    mockDynamicData();
});

beforeEach(() => {
    document.body.innerHTML = '<div id="LoginPanel"></div>';
    calls = [];
    vi.spyOn(corelib, "serviceCall").mockImplementation(((options: any) => {
        calls.push(options);
        return { then: () => void 0 } as any;
    }) as any);
});

afterEach(() => {
    vi.restoreAllMocks();
    document.body.innerHTML = "";
});

function getForm() {
    const input = document.querySelector('#LoginPanel input[id$="Username"]') as HTMLInputElement;
    const prefix = input.id.substring(0, input.id.length - "Username".length);
    return new LoginForm(prefix);
}

function loginButton() {
    return document.querySelector('#LoginPanel [id$="LoginButton"]') as HTMLElement;
}

describe("LoginPage", () => {
    it("renders and pre-fills the activated user", () => {
        pageInit({ activated: "john" });
        expect(getForm().Username.value).toBe("john");
    });

    it("does not call the service when the form is invalid", () => {
        pageInit();
        loginButton().click();
        expect(calls.length).toBe(0);
    });

    it("calls the service on valid login and redirects on success", () => {
        pageInit();
        const form = getForm();
        typeText(form.Username, "john");
        typeText(form.Password, "secret");
        loginButton().click();
        expect(calls.length).toBe(1);
        expect(calls[0].url).toContain("Account/Login");
        expect(() => calls[0].onSuccess?.()).not.toThrow();
    });

    it("handles RedirectUserTo errors", () => {
        pageInit();
        const form = getForm();
        typeText(form.Username, "john");
        typeText(form.Password, "secret");
        loginButton().click();
        expect(() => calls[0].onError?.({ Error: { Code: "RedirectUserTo", Arguments: "/somewhere" } })).not.toThrow();
    });

    it("notifies on message errors", () => {
        const notify = vi.spyOn(corelib, "notifyError").mockImplementation(() => void 0);
        pageInit();
        const form = getForm();
        typeText(form.Username, "john");
        typeText(form.Password, "secret");
        loginButton().click();
        calls[0].onError?.({ Error: { Message: "bad credentials" } });
        expect(notify).toHaveBeenCalled();
    });

    it("shows service error for generic errors", () => {
        const show = vi.spyOn(corelib.ErrorHandling, "showServiceError").mockImplementation(() => void 0);
        pageInit();
        const form = getForm();
        typeText(form.Username, "john");
        typeText(form.Password, "secret");
        loginButton().click();
        calls[0].onError?.({ Error: {} });
        expect(show).toHaveBeenCalled();
    });
});


