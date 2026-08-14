import { describe, expect, it } from "vitest";
import {
    AdvancedFilteringAttribute,
    Attributes,
    CloseButtonAttribute,
    ElementAttribute,
    FilterableAttribute,
    MaximizableAttribute,
    OptionAttribute,
    PanelAttribute,
    ResizableAttribute,
    StaticPanelAttribute
} from "./attributes";

describe("Attributes", () => {
    it("creates attribute instances through factory helpers", () => {
        expect(Attributes.closeButton().value).toBe(true);
        expect(Attributes.closeButton(false).value).toBe(false);
        expect(Attributes.advancedFiltering().value).toBe(true);
        expect(Attributes.resizable().value).toBe(true);
        expect(Attributes.maximizable().value).toBe(true);
        expect(Attributes.panel().value).toBe(true);
        expect(Attributes.staticPanel().value).toBe(true);
    });

    it("marks factory helpers as attribute factories", () => {
        expect((Attributes as any).closeButton.isAttributeFactory).toBe(true);
        expect((Attributes as any).resizable.isAttributeFactory).toBe(true);
        expect((Attributes as any).panel.isAttributeFactory).toBe(true);
    });

    it("exposes the attribute classes with defaults", () => {
        expect(new CloseButtonAttribute().value).toBe(true);
        expect(new CloseButtonAttribute(false).value).toBe(false);
        expect(new ElementAttribute("div").value).toBe("div");
        expect(new AdvancedFilteringAttribute().value).toBe(true);
        expect(new MaximizableAttribute(false).value).toBe(false);
        expect(new OptionAttribute()).toBeInstanceOf(OptionAttribute);
        expect(new PanelAttribute().value).toBe(true);
        expect(new ResizableAttribute().value).toBe(true);
        expect(new StaticPanelAttribute().value).toBe(true);
        expect(FilterableAttribute).toBe(AdvancedFilteringAttribute);
    });
});
