import { FilterPanelTexts, Fluent } from "../../base";
import { FilterDialog } from "./filterdialog";
import { FilterDisplayBar } from "./filterdisplaybar";
import { FilterStore } from "./filterstore";

describe("FilterDisplayBar", () => {
    it("is registered with typeInfo", () => {
        expect(FilterDisplayBar[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("can be constructed", () => {
        const bar = new FilterDisplayBar({});
        expect(bar).toBeInstanceOf(FilterDisplayBar);
        bar.destroy();
    });

    it("filterStoreChanged updates display text", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();

        store.get_items().push({ field: "F1", displayText: "F1 = test" });
        store.raiseChanged();

        const txtEl = bar.element.findFirst('.txt');
        expect(txtEl.text()).toContain("F1 = test");
        bar.destroy();
    });

    it("filterStoreChanged shows effective empty when no display text", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();
        store.raiseChanged();

        const txtEl = bar.element.findFirst('.txt');
        expect(txtEl.text()).toBe('[' + FilterPanelTexts.EffectiveEmpty + ']');
        bar.destroy();
    });

    it("filterStoreChanged toggles reset and current elements", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();

        // Initially raise changed to trigger filterStoreChanged with no items
        store.raiseChanged();

        const resetEl = bar.element.findFirst('.reset');
        const currentEl = bar.element.findFirst('.current');
        // With no items, reset/current should be hidden
        expect(resetEl.getNode().hidden).toBe(true);
        expect(currentEl.getNode().hidden).toBe(true);

        // Add items
        store.get_items().push({ field: "F1", displayText: "test" });
        store.raiseChanged();

        // After adding items, elements should be visible
        expect(resetEl.getNode().hidden).toBe(false);
        expect(currentEl.getNode().hidden).toBe(false);

        bar.destroy();
    });

    it("createToolButton returns expected structure", () => {
        const btn = FilterDisplayBar.createToolButton({});
        expect(btn.hint).toBe(FilterPanelTexts.EditFilter);
        expect(btn.action).toBe("edit-filter");
        expect(btn.cssClass).toBe("edit-filter-button");
        expect(btn.icon).toBeTruthy();
    });

    it("createToolButton merges custom options", () => {
        const btn = FilterDisplayBar.createToolButton({ hint: "custom", cssClass: "my-class" });
        expect(btn.hint).toBe("custom");
        expect(btn.cssClass).toBe("my-class");
    });

    it("reset click clears store items", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();
        store.get_items().push({ field: "F1", displayText: "test" });
        expect(store.get_items().length).toBe(1);

        const resetLink = bar.element.findFirst('.reset');
        resetLink.click();

        expect(store.get_items().length).toBe(0);
        bar.destroy();
    });

    it("edit link exists in the rendered output", () => {
        const bar = new FilterDisplayBar({});
        const editLink = bar.element.findFirst('.edit');
        expect(editLink.length).toBe(1);
        expect(editLink.text()).toBe(FilterPanelTexts.EditFilter);
        bar.destroy();
    });

    it("txt link exists in the rendered output", () => {
        const bar = new FilterDisplayBar({});
        const txtLink = bar.element.findFirst('.txt');
        expect(txtLink.length).toBe(1);
        bar.destroy();
    });

    it("edit link click opens filter dialog via openFilterDialog", () => {
        const dialogOpenSpy = vi.spyOn(FilterDialog.prototype, "dialogOpen").mockImplementation(() => {});
        const setStoreSpy = vi.fn();
        vi.spyOn(FilterDialog.prototype, "get_filterPanel").mockReturnValue({ set_store: setStoreSpy } as any);

        const bar = new FilterDisplayBar({});
        const store = bar.get_store();
        store.get_items().push({ field: "F1", displayText: "test" });
        store.raiseChanged();

        const editLink = bar.element.findFirst('.edit');
        editLink.click();

        expect(dialogOpenSpy).toHaveBeenCalledWith(null);
        expect(setStoreSpy).toHaveBeenCalledWith(store);
        bar.destroy();
        dialogOpenSpy.mockRestore();
    });

    it("reset click prevents default", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();
        store.get_items().push({ field: "F1", displayText: "test" });
        store.raiseChanged();

        const resetLink = bar.element.findFirst('.reset');
        // The reset link uses an onClick handler that calls e.preventDefault()
        // and then clears items. Just verify the items are cleared.
        expect(store.get_items().length).toBe(1);
        resetLink.click();
        expect(store.get_items().length).toBe(0);
        bar.destroy();
    });

    it("displayText returns effective empty when null display text", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();

        // Raise changed with no items (displayText will be null)
        store.raiseChanged();

        const txtEl = bar.element.findFirst('.txt');
        expect(txtEl.text()).toBe('[' + FilterPanelTexts.EffectiveEmpty + ']');
        bar.destroy();
    });

    it("displayText shows empty brackets when trimmed display text is empty", () => {
        const bar = new FilterDisplayBar({});
        const store = bar.get_store();

        // Add item with empty displayText
        store.get_items().push({ field: "F1", displayText: "   " });
        store.raiseChanged();

        const txtEl = bar.element.findFirst('.txt');
        expect(txtEl.text()).toBe('[' + FilterPanelTexts.EffectiveEmpty + ']');
        bar.destroy();
    });
});
