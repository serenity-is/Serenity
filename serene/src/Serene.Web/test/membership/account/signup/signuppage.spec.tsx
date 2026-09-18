import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, typeText } from "test-utils";
import pageInit from "../../../../Modules/Membership/Account/SignUp/SignUpPage";
import { SignUpForm } from "../../../../Modules/ServerTypes/Membership";

let calls: any[];

beforeAll(() => {
    mockDynamicData();
});

beforeEach(() => {
    document.body.innerHTML = '<div id="SignUpPanel"></div>';
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
    const input = document.querySelector('#SignUpPanel input[id$="DisplayName"]') as HTMLInputElement;
    const prefix = input.id.substring(0, input.id.length - "DisplayName".length);
    return new SignUpForm(prefix);
}

function submitButton() {
    return document.querySelector('#SignUpPanel [id$="SubmitButton"]') as HTMLElement;
}

function fillValid(form: SignUpForm) {
    typeText(form.DisplayName, "John");
    typeText(form.Email, "john@example.com");
    typeText(form.ConfirmEmail, "john@example.com");
    typeText(form.Password, "secret1");
    typeText(form.ConfirmPassword, "secret1");
}

describe("SignUpPage", () => {
    it("renders the sign up panel", () => {
        pageInit({});
        expect(getForm().DisplayName).toBeTruthy();
    });

    it("does not submit when the form is invalid", () => {
        pageInit({});
        submitButton().click();
        expect(calls.length).toBe(0);
    });

    it("does not submit when emails do not match", () => {
        pageInit({});
        const form = getForm();
        fillValid(form);
        typeText(form.ConfirmEmail, "other@example.com");
        submitButton().click();
        expect(calls.length).toBe(0);
    });

    it("does not submit when passwords do not match", () => {
        pageInit({});
        const form = getForm();
        fillValid(form);
        typeText(form.ConfirmPassword, "otherpass");
        submitButton().click();
        expect(calls.length).toBe(0);
    });

    it("submits a valid form and shows the success dialog", () => {
        const info = vi.spyOn(corelib, "informationDialog").mockImplementation(((...args: any[]) => {
            args[1]?.();
        }) as any);
        pageInit({});
        const form = getForm();
        fillValid(form);
        submitButton().click();
        expect(calls.length).toBe(1);
        expect(calls[0].url).toContain("Account/SignUp");
        expect(calls[0].request.ConfirmEmail).toBeUndefined();
        expect(calls[0].request.ConfirmPassword).toBeUndefined();
        calls[0].onSuccess?.({});
        expect(info).toHaveBeenCalled();
    });
});

