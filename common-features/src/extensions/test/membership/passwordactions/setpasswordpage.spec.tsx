import * as corelib from "@serenity-is/corelib";
import { mockFetch, unmockFetch } from "test-utils";
import pageInit from "../../../Modules/Membership/PasswordActions/SetPasswordPage";

beforeEach(() => {
    document.body.innerHTML = '<div id="PanelDiv"></div>';
    mockFetch({ "*": () => ({}) });
});

afterEach(() => {
    document.body.innerHTML = "";
    unmockFetch();
    vi.restoreAllMocks();
});

describe("SetPasswordPage", () => {
    it("renders the set password form", () => {
        pageInit();
        const panelDiv = document.getElementById("PanelDiv");
        expect(panelDiv.querySelector("h3.page-title")).toBeTruthy();
        expect(panelDiv.querySelector("form")).toBeTruthy();
        expect(panelDiv.querySelector("button")).toBeTruthy();
    });

    it("renders the elevate message when reason is elevate", () => {
        window.history.pushState({}, "", "?reason=elevate");
        pageInit();
        const panelDiv = document.getElementById("PanelDiv");
        expect(panelDiv.querySelector("p")).toBeTruthy();
        window.history.pushState({}, "", "/");
    });

    it("submits the send reset password request", () => {
        pageInit();
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({});
            return { then: () => { } } as any;
        });
        vi.spyOn(corelib, "informationDialog").mockReturnValue({} as any);
        (document.querySelector("button") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalledTimes(1);
        expect((serviceCall.mock.calls[0][0] as any).url).toContain("Account/SendResetPassword");
    });

    it("shows the demo link when provided", () => {
        pageInit();
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({ DemoLink: "~/Account/ResetPassword?token=x" });
            return { then: () => { } } as any;
        });
        const infoSpy = vi.spyOn(corelib, "informationDialog").mockReturnValue({} as any);
        (document.querySelector("button") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalled();
        expect(infoSpy).toHaveBeenCalled();
    });
});

