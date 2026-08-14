import { describe, it, expect, vi, beforeEach } from "vitest";

const { findElementWithRelativeId, tryGetWidget } = vi.hoisted(() => ({
    findElementWithRelativeId: vi.fn(),
    tryGetWidget: vi.fn()
}));

vi.mock("../../compat", async importOriginal => ({
    ...(await importOriginal() as any),
    findElementWithRelativeId
}));
vi.mock("../widgets/widgetutils", async importOriginal => ({
    ...(await importOriginal() as any),
    tryGetWidget
}));

import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";

class ParentWidget extends Widget<any> {
    static override [Symbol.typeInfo] = this.registerClass("Test.MockParentWidget");
}
class ChildWidget extends Widget<any> {
    static override [Symbol.typeInfo] = this.registerClass("Test.MockChildWidget");
}

describe("CascadedWidgetLink resolved parent", () => {
    beforeEach(() => {
        document.body.innerHTML = "";
        findElementWithRelativeId.mockReset();
        tryGetWidget.mockReset();
    });

    it("binds change, unbinds, and clears on disposal", () => {
        const parent = new ParentWidget({ element: document.createElement("div") });
        const child = new ChildWidget({ element: document.createElement("div") });
        const parentChange = vi.fn();
        findElementWithRelativeId.mockReturnValue(parent.domNode);
        tryGetWidget.mockReturnValue(parent);

        const link = new CascadedWidgetLink(ParentWidget, child, parentChange);
        link.set_parentID("Parent");
        parent.domNode.dispatchEvent(new Event("change"));
        expect(parentChange).toHaveBeenCalledWith(parent);
        expect(link.get_parentID()).toBe("Parent");
        expect((link as any).unbind()).toBe(parent.domNode);

        child.destroy();
        parent.destroy();
    });
});
