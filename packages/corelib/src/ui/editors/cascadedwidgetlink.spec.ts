import { invokeDisposingListeners } from "@serenity-is/domwise";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";

class ParentWidget extends Widget<any> {
    static override [Symbol.typeInfo] = this.registerClass("Test.ParentWidget");
}
class ChildWidget extends Widget<any> {
    static override [Symbol.typeInfo] = this.registerClass("Test.ChildWidget");
}

describe("CascadedWidgetLink", () => {
    beforeEach(() => { document.body.innerHTML = ""; });
    afterEach(() => { document.body.innerHTML = ""; vi.restoreAllMocks(); });

    it("set_parentID with unresolvable parent calls notifyError", async () => {
        const notifyErrorSpy = vi.spyOn(await import("../../base"), "notifyError").mockImplementation(() => { });
        const child = new ChildWidget({ element: document.createElement("div") });
        const link = new CascadedWidgetLink(ParentWidget, child, () => { });
        link.set_parentID("NotFound");
        expect(notifyErrorSpy).toHaveBeenCalled();
        expect(link.get_parentID()).toBe("NotFound");
        child.destroy();
        notifyErrorSpy.mockRestore();
    });

    it("set_parentID with same value does not rebind", () => {
        const child = new ChildWidget({ element: document.createElement("div") });
        const link = new CascadedWidgetLink(ParentWidget, child, () => { });
        link.set_parentID("X");
        const spy = vi.spyOn(link as any, "bind");
        link.set_parentID("X");
        expect(spy).not.toHaveBeenCalled();
        child.destroy();
    });

    it("disposing the widget unbinds and clears references", () => {
        const child = new ChildWidget({ element: document.createElement("div") });
        const link = new CascadedWidgetLink(ParentWidget, child, () => { });
        invokeDisposingListeners(child.domNode);
        expect((link as any).widget).toBeNull();
        expect((link as any).parentChange).toBeNull();
    });
});