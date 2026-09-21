import * as corelib from "@serenity-is/corelib";
import { mockDynamicData } from "test-utils";
import { afterEach, beforeAll, beforeEach, describe, expect, it, vi } from "vitest";
import initPage from "../../Modules/Dialogs/DialogBoxes/DialogBoxesPage";

beforeAll(() => {
    mockDynamicData();
});

beforeEach(() => {
    document.body.innerHTML = "";
});

afterEach(() => {
    vi.restoreAllMocks();
});

function setup() {
    const panel = document.body.appendChild(document.createElement("div"));
    panel.id = "PanelDiv";
    initPage();
    return panel;
}

function buttonByText(panel: HTMLElement, text: string) {
    return Array.from(panel.querySelectorAll("button")).find(x => x.textContent.includes(text));
}

describe("DialogBoxesPage", () => {
    it("renders all sample buttons", () => {
        const panel = setup();
        expect(buttonByText(panel, "Confirm Dialog and Buttons")).toBeTruthy();
        expect(buttonByText(panel, "Confirm With Custom Title")).toBeTruthy();
        expect(buttonByText(panel, "Information")).toBeTruthy();
        expect(buttonByText(panel, "Success")).toBeTruthy();
        expect(buttonByText(panel, "Warning")).toBeTruthy();
        expect(buttonByText(panel, "Alert")).toBeTruthy();
        expect(buttonByText(panel, "Alert with HTML Content")).toBeTruthy();
    });

    it("handles confirm dialog buttons", () => {
        const panel = setup();
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((() => ({})) as any);
        const notifySuccess = vi.spyOn(corelib, "notifySuccess").mockImplementation((() => ({})) as any);
        const notifyInfo = vi.spyOn(corelib, "notifyInfo").mockImplementation((() => ({})) as any);
        const notifyError = vi.spyOn(corelib, "notifyError").mockImplementation((() => ({})) as any);

        buttonByText(panel, "Confirm Dialog and Buttons").click();

        expect(confirm).toHaveBeenCalledWith(expect.any(String), expect.any(Function), expect.objectContaining({
            onNo: expect.any(Function),
            onClose: expect.any(Function)
        }));
        const options = confirm.mock.calls[0][2];
        (confirm.mock.calls[0][1] as Function)();
        expect(notifySuccess).toHaveBeenCalled();
        options.onNo();
        expect(notifyInfo).toHaveBeenCalled();
        options.onClose("");
        expect(notifyError).toHaveBeenCalled();
        options.onClose("ok");
        expect(notifyError).toHaveBeenCalledTimes(1);
    });

    it("handles confirm with custom title", () => {
        const panel = setup();
        const confirm = vi.spyOn(corelib, "confirmDialog").mockImplementation((() => ({})) as any);
        buttonByText(panel, "Confirm With Custom Title").click();
        expect(confirm).toHaveBeenCalledWith(expect.any(String), expect.any(Function), expect.objectContaining({
            title: "Some Custom Confirmation Title"
        }));
    });

    it("handles information, success, warning and alert dialogs", () => {
        const panel = setup();
        const info = vi.spyOn(corelib, "informationDialog").mockImplementation((() => ({})) as any);
        const success = vi.spyOn(corelib, "successDialog").mockImplementation((() => ({})) as any);
        const warning = vi.spyOn(corelib, "warningDialog").mockImplementation((() => ({})) as any);
        const alert = vi.spyOn(corelib, "alertDialog").mockImplementation((() => ({})) as any);
        const notifySuccess = vi.spyOn(corelib, "notifySuccess").mockImplementation(() => { });

        buttonByText(panel, "Information").click();
        expect(info).toHaveBeenCalledWith("What a nice day", expect.any(Function));
        (info.mock.calls[0][1] as Function)();
        expect(notifySuccess).toHaveBeenCalled();
        buttonByText(panel, "Success").click();
        expect(success).toHaveBeenCalledWith("Operation complete", expect.any(Function));
        (success.mock.calls[0][1] as Function)();
        buttonByText(panel, "Warning").click();
        expect(warning).toHaveBeenCalledWith("Hey, be careful!");
        buttonByText(panel, "Alert").click();
        expect(alert).toHaveBeenCalledWith("Houston, we got a problem!");
    });

    it("handles alert with html content", () => {
        const panel = setup();
        const alert = vi.spyOn(corelib, "alertDialog").mockImplementation((() => ({})) as any);
        buttonByText(panel, "Alert with HTML Content").click();
        expect(alert).toHaveBeenCalledTimes(1);
        expect(alert.mock.calls[0][0]).toBeTruthy();
    });
});
