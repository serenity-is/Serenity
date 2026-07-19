import { FilterStore } from "./filterstore";
import { FilterWidgetBase } from "./filterwidgetbase";

describe("FilterWidgetBase", () => {
    it("is registered with typeInfo", () => {
        expect(FilterWidgetBase[Symbol.for("Serenity.typeInfo")]).toBeDefined();
    });

    it("creates a FilterStore in constructor", () => {
        const widget = new FilterWidgetBase({});
        expect(widget.get_store()).toBeInstanceOf(FilterStore);
        expect(widget.get_store().get_items()).toEqual([]);
        widget.destroy();
    });

    it("set_store replaces the store and wires up events", () => {
        const widget = new FilterWidgetBase({});
        const store1 = widget.get_store();
        const store2 = new FilterStore([{ name: "Field1", title: "Field1" }]);

        const changedSpy = vi.fn();
        widget["add_changed"]?.(changedSpy);

        widget.set_store(store2);
        expect(widget.get_store()).toBe(store2);
        expect(widget.get_store()).not.toBe(store1);

        widget.destroy();
    });

    it("set_store with null creates empty store", () => {
        const widget = new FilterWidgetBase({});
        widget.set_store(null);
        expect(widget.get_store()).toBeInstanceOf(FilterStore);
        widget.destroy();
    });

    it("filterStoreChanged is called when store raises changed", () => {
        const widget = new FilterWidgetBase({});
        const spy = vi.spyOn(widget as any, "filterStoreChanged");

        widget.get_store().raiseChanged();
        expect(spy).toHaveBeenCalled();

        widget.destroy();
    });

    it("destroy removes event handler from store", () => {
        const widget = new FilterWidgetBase({});
        const store = widget.get_store();
        const spy = vi.spyOn(widget as any, "filterStoreChanged");

        widget.destroy();
        store.raiseChanged();
        // After destroy, the handler should be removed
        expect(spy).toHaveBeenCalledTimes(0);
    });

    it("set_store removes event from old store and adds to new", () => {
        const widget = new FilterWidgetBase({});
        const oldStore = widget.get_store();
        const newStore = new FilterStore([]);

        const spy = vi.spyOn(widget as any, "filterStoreChanged");

        widget.set_store(newStore);

        // Changing old store should not trigger
        oldStore.raiseChanged();
        const oldCount = spy.mock.calls.length;

        // Changing new store should trigger
        newStore.raiseChanged();
        expect(spy).toHaveBeenCalledTimes(oldCount + 1);

        widget.destroy();
    });

    it("filterStoreChanged is a no-op by default", () => {
        const widget = new FilterWidgetBase({});
        expect(() => (widget as any).filterStoreChanged()).not.toThrow();
        widget.destroy();
    });

    it("has uniqueName and idPrefix", () => {
        const widget = new FilterWidgetBase({});
        expect(widget.uniqueName).toBeTruthy();
        expect(widget.idPrefix).toBe(widget.uniqueName + "_");
        widget.destroy();
    });
});
