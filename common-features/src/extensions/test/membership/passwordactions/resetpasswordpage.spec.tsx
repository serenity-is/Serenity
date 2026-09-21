import * as corelib from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, unmockDynamicData, unmockFetch } from "test-utils";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import pageInit, { ResetPasswordPanel } from "../../../Modules/Membership/PasswordActions/ResetPasswordPage";

beforeEach(() => {
    document.body.innerHTML = '<div id="PanelDiv"></div>';
    mockDynamicData();
    mockFetch({ "*": () => ({}) });
    vi.spyOn(corelib, "getRemoteDataAsync").mockResolvedValue({
        MinPasswordLength: 6,
        RequireDigit: false,
        RequireLowercase: false,
        RequireUppercase: false,
        RequireNonAlphanumeric: false
    } as any);
});

afterEach(() => {
    document.body.innerHTML = "";
    unmockDynamicData();
    unmockFetch();
    vi.restoreAllMocks();
});

describe("ResetPasswordPage", () => {
    it("renders the reset password form with a token input", () => {
        pageInit({ token: "abc", minPasswordLength: 6 });
        const panelDiv = document.getElementById("PanelDiv");
        expect(panelDiv.querySelector("h5")).toBeTruthy();
        const token = panelDiv.querySelector<HTMLInputElement>("input[type=hidden]");
        expect(token).toBeTruthy();
        expect(token.value).toBe("abc");
    });

    it("submits the reset password request with the token", () => {
        const panel = new ResetPasswordPanel({ element: "#PanelDiv", token: "tok", minPasswordLength: 6 });
        vi.spyOn(panel as any, "validateForm").mockReturnValue(true);
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({});
            return { then: () => { } } as any;
        });
        vi.spyOn(corelib, "informationDialog").mockReturnValue({} as any);
        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalledTimes(1);
        const opt: any = serviceCall.mock.calls[0][0];
        expect(opt.url).toContain("Account/ResetPassword");
        expect(opt.request.Token).toBe("tok");
        panel.destroy();
    });

    it("redirects home when response asks for it", () => {
        const panel = new ResetPasswordPanel({ element: "#PanelDiv", token: "tok", minPasswordLength: 6 });
        vi.spyOn(panel as any, "validateForm").mockReturnValue(true);
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({ RedirectHome: true });
            return { then: () => { } } as any;
        });
        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalled();
        panel.destroy();
    });

    it("does not submit when validation fails", () => {
        const panel = new ResetPasswordPanel({ element: "#PanelDiv", token: "tok", minPasswordLength: 6 });
        vi.spyOn(panel as any, "validateForm").mockReturnValue(false);
        const serviceCall = vi.spyOn(corelib, "serviceCall");
        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).not.toHaveBeenCalled();
        panel.destroy();
    });
});

