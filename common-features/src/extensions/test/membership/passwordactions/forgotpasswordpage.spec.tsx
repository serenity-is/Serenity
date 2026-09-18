import * as corelib from "@serenity-is/corelib";
import { PropertyPanel } from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import pageInit, { ForgotPasswordPanel } from "../../../Modules/Membership/PasswordActions/ForgotPasswordPage";

beforeEach(() => {
    document.body.innerHTML = '<div id="PanelDiv"></div>';
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    document.body.innerHTML = "";
    unmockFetch();
    vi.restoreAllMocks();
});

describe("ForgotPasswordPage", () => {
    it("renders the forgot password form", () => {
        pageInit();
        const panelDiv = document.getElementById("PanelDiv");
        expect(panelDiv.querySelector("h5")).toBeTruthy();
        expect(panelDiv.querySelector("form")).toBeTruthy();
        expect(panelDiv.querySelector("button[type=submit]")).toBeTruthy();
    });

    it("submits the forgot password request", () => {
        const panel = new ForgotPasswordPanel({ element: "#PanelDiv" });
        vi.spyOn(panel as any, "validateForm").mockReturnValue(true);
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({});
            return { then: () => { } } as any;
        });
        vi.spyOn(corelib, "informationDialog").mockReturnValue({} as any);

        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalledTimes(1);
        expect((serviceCall.mock.calls[0][0] as any).url).toContain("Account/ForgotPassword");
        panel.destroy();
    });

    it("does not submit when validation fails", () => {
        const panel = new ForgotPasswordPanel({ element: "#PanelDiv" });
        vi.spyOn(panel as any, "validateForm").mockReturnValue(false);
        const serviceCall = vi.spyOn(corelib, "serviceCall");
        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).not.toHaveBeenCalled();
        panel.destroy();
    });

    it("exposes a form key", () => {
        const panel = new ForgotPasswordPanel({ element: "#PanelDiv" });
        expect((panel as any).getFormKey()).toBeTruthy();
        panel.destroy();
    });
});

