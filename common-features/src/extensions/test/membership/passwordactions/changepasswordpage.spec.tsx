import * as corelib from "@serenity-is/corelib";
import { PropertyPanel, tryGetWidget } from "@serenity-is/corelib";
import { mockDynamicData, mockFetch, unmockDynamicData, unmockFetch } from "test-utils";
import pageInit from "../../../Modules/Membership/PasswordActions/ChangePasswordPage";

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

describe("ChangePasswordPage", () => {
    it("renders the change password form", () => {
        pageInit();
        const panelDiv = document.getElementById("PanelDiv");
        expect(panelDiv.querySelector("h3.page-title")).toBeTruthy();
        expect(panelDiv.querySelector("form")).toBeTruthy();
        expect(panelDiv.querySelector("button[type=submit]")).toBeTruthy();
    });

    it("submits the change password request", () => {
        pageInit();
        const panel = tryGetWidget(document.getElementById("PanelDiv"), PropertyPanel) as any;
        vi.spyOn(panel, "validateForm").mockReturnValue(true);
        const serviceCall = vi.spyOn(corelib, "serviceCall").mockImplementation((opt: any) => {
            opt.onSuccess?.({});
            return { then: () => { } } as any;
        });
        vi.spyOn(corelib, "informationDialog").mockReturnValue({} as any);

        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).toHaveBeenCalledTimes(1);
        expect((serviceCall.mock.calls[0][0] as any).url).toContain("Account/ChangePassword");
        panel.destroy();
    });

    it("does not submit when validation fails", () => {
        pageInit();
        const panel = tryGetWidget(document.getElementById("PanelDiv"), PropertyPanel) as any;
        vi.spyOn(panel, "validateForm").mockReturnValue(false);
        const serviceCall = vi.spyOn(corelib, "serviceCall");
        (document.querySelector("button[type=submit]") as HTMLElement).click();
        expect(serviceCall).not.toHaveBeenCalled();
        panel.destroy();
    });
});

