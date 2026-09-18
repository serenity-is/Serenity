import { DialogUtils } from "../../Modules/Widgets/DialogUtils";
import * as corelib from "@serenity-is/corelib";
import { Fluent } from "@serenity-is/corelib";

describe("DialogUtils.pendingChangesConfirmation", () => {
    let el: HTMLElement;

    beforeEach(() => {
        el = document.createElement("div");
        document.body.appendChild(el);
    });

    afterEach(() => {
        Fluent.off(window, "beforeunload");
        el.remove();
        vi.restoreAllMocks();
    });

    it("does nothing when element is null", () => {
        expect(() => DialogUtils.pendingChangesConfirmation(null, () => true)).not.toThrow();
    });

    it("does nothing when array-like element is empty", () => {
        expect(() => DialogUtils.pendingChangesConfirmation([] as any, () => true)).not.toThrow();
    });

    it("accepts an array-like element", () => {
        const hasPending = vi.fn(() => true);
        const confirmSpy = vi.spyOn(corelib, "confirmDialog").mockReturnValue({} as any);
        DialogUtils.pendingChangesConfirmation([el] as any, hasPending);
        const e = new Event("panelbeforeclose", { cancelable: true });
        el.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(true);
        expect(confirmSpy).toHaveBeenCalledTimes(1);
    });

    it("removes the beforeunload handler when a close handler fires", () => {
        el.getClientRects = () => [{}] as any;
        DialogUtils.pendingChangesConfirmation(el, () => true);
        el.dispatchEvent(new Event("panelclose"));
        const e = new Event("beforeunload", { cancelable: true });
        window.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(false);
    });

    it("does not prevent close when there are no pending changes", () => {
        const hasPending = vi.fn(() => false);
        const confirmSpy = vi.spyOn(corelib, "confirmDialog");
        DialogUtils.pendingChangesConfirmation(el, hasPending);
        const e = new Event("panelbeforeclose", { cancelable: true });
        el.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(false);
        expect(confirmSpy).not.toHaveBeenCalled();
    });

    it("asks for confirmation and prevents close when there are pending changes", () => {
        const confirmSpy = vi.spyOn(corelib, "confirmDialog").mockReturnValue({} as any);
        DialogUtils.pendingChangesConfirmation(el, () => true);
        const e = new Event("panelbeforeclose", { cancelable: true });
        el.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(true);
        expect(confirmSpy).toHaveBeenCalledTimes(1);
    });

    it("does not ask again within ack timeout", () => {
        let onYes: () => void;
        const confirmSpy = vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg, yes) => {
            onYes = yes;
            return {} as any;
        });
        DialogUtils.pendingChangesConfirmation(el, () => true);
        el.dispatchEvent(new Event("panelbeforeclose", { cancelable: true }));
        expect(confirmSpy).toHaveBeenCalledTimes(1);
        onYes();

        const e = new Event("panelbeforeclose", { cancelable: true });
        el.dispatchEvent(e);
        expect(confirmSpy).toHaveBeenCalledTimes(1);
        expect(e.defaultPrevented).toBe(false);
    });

    it("does not prevent close for known result codes", () => {
        const handlers: ((result?: string, e?: Event) => void)[] = [];
        vi.spyOn(corelib.Dialog, "getInstance").mockReturnValue({
            result: "save",
            onClose: (handler: any) => handlers.push(handler),
            close: vi.fn()
        } as any);
        const confirmSpy = vi.spyOn(corelib, "confirmDialog");
        DialogUtils.pendingChangesConfirmation(el, () => true);
        handlers[0]("save", {} as any);
        expect(confirmSpy).not.toHaveBeenCalled();
    });

    it("onYes acknowledges and clicks the save-and-close button", () => {
        let onYes: () => void;
        vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg, yes) => {
            onYes = yes;
            return {} as any;
        });
        const button = document.createElement("div");
        button.className = "save-and-close-button";
        el.appendChild(button);
        const clickSpy = vi.spyOn(button, "click");

        DialogUtils.pendingChangesConfirmation(el, () => true);
        el.dispatchEvent(new Event("panelbeforeclose", { cancelable: true }));
        onYes();
        expect(clickSpy).toHaveBeenCalledTimes(1);
        expect(el.dataset.ackuntil).toBeTruthy();
    });

    it("onNo acknowledges and closes the dialog via instance", () => {
        let onNo: () => void;
        vi.spyOn(corelib, "confirmDialog").mockImplementation((_msg, _yes, opt: any) => {
            onNo = opt.onNo;
            return {} as any;
        });
        const closeSpy = vi.fn();
        DialogUtils.pendingChangesConfirmation(el, () => true);
        el.dispatchEvent(new Event("panelbeforeclose", { cancelable: true }));
        vi.spyOn(corelib.Dialog, "getInstance").mockReturnValue({ close: closeSpy } as any);
        onNo();
        expect(closeSpy).toHaveBeenCalledWith(undefined);
        expect(el.dataset.ackuntil).toBeTruthy();
    });

    it("warns on beforeunload when there are pending changes", () => {
        el.getClientRects = () => [{}] as any;
        DialogUtils.pendingChangesConfirmation(el, () => true);
        const e = new Event("beforeunload", { cancelable: true });
        window.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(true);
    });

    it("does not warn on beforeunload when element is not visible", () => {
        el.getClientRects = () => [] as any;
        DialogUtils.pendingChangesConfirmation(el, () => true);
        const e = new Event("beforeunload", { cancelable: true });
        window.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(false);
    });

    it("does not warn on beforeunload when there are no pending changes", () => {
        el.getClientRects = () => [{}] as any;
        DialogUtils.pendingChangesConfirmation(el, () => false);
        const e = new Event("beforeunload", { cancelable: true });
        window.dispatchEvent(e);
        expect(e.defaultPrevented).toBe(false);
    });
});
