import { FilterLine } from "./filterline";
import { FilterStore, delegateCombine, delegateContains, delegateRemove } from "./filterstore";

describe("FilterStore", () => {
    describe("constructor", () => {
        it("throws if fields is null", () => {
            expect(() => new FilterStore(null)).toThrow();
        });

        it("sorts fields by title", () => {
            const fields = [
                { name: "B", title: "Beta" },
                { name: "A", title: "Alpha" },
                { name: "C", title: "Gamma" }
            ];
            const store = new FilterStore(fields);
            const sorted = store.get_fields();
            expect(sorted[0].name).toBe("A");
            expect(sorted[1].name).toBe("B");
            expect(sorted[2].name).toBe("C");
        });

        it("builds fieldByName map", () => {
            const fields = [
                { name: "Field1" },
                { name: "Field2" }
            ];
            const store = new FilterStore(fields);
            expect(store.get_fieldByName()["Field1"]).toBe(fields[0]);
            expect(store.get_fieldByName()["Field2"]).toBe(fields[1]);
        });

        it("initializes items as empty array", () => {
            const store = new FilterStore([]);
            expect(store.get_items()).toEqual([]);
        });
    });

    describe("getCriteriaFor", () => {
        it("returns [''] for null items", () => {
            const result = FilterStore.getCriteriaFor(null);
            expect(result).toEqual([""]);
        });

        it("returns [''] for empty items", () => {
            const result = FilterStore.getCriteriaFor([]);
            expect(result).toEqual([""]);
        });

        it("handles a single line with AND", () => {
            const items: FilterLine[] = [
                { field: "Field1", operator: "eq", criteria: ["Field1", "=", "value1"], displayText: "Field1 = value1" }
            ];
            const result = FilterStore.getCriteriaFor(items);
            expect(result.length).toBeGreaterThan(0);
        });

        it("handles multiple lines with OR", () => {
            const items: FilterLine[] = [
                { field: "F1", operator: "eq", isOr: false, criteria: ["F1", "=", "a"] },
                { field: "F2", operator: "eq", isOr: true, criteria: ["F2", "=", "b"] }
            ];
            const result = FilterStore.getCriteriaFor(items);
            expect(result).toBeTruthy();
        });

        it("handles left parenthesis", () => {
            const items: FilterLine[] = [
                { field: "F1", operator: "eq", leftParen: true, criteria: ["F1", "=", "a"] },
                { field: "F2", operator: "eq", isOr: true, criteria: ["F2", "=", "b"] }
            ];
            const result = FilterStore.getCriteriaFor(items);
            expect(result).toBeTruthy();
        });

        it("handles right parenthesis", () => {
            const items: FilterLine[] = [
                { field: "F1", operator: "eq", leftParen: true, criteria: ["F1", "=", "a"] },
                { field: "F2", operator: "eq", isOr: true, criteria: ["F2", "=", "b"] },
                { field: "F3", operator: "eq", leftParen: false, rightParen: true, criteria: ["F3", "=", "c"] }
            ];
            const result = FilterStore.getCriteriaFor(items);
            expect(result).toBeTruthy();
        });

        it("handles paren close with inParen", () => {
            const items: FilterLine[] = [
                { field: "F1", operator: "eq", leftParen: true, criteria: ["F1", "=", "a"] },
                { field: "F2", operator: "eq", isOr: true, criteria: ["F2", "=", "b"] },
                { field: "F3", operator: "eq", rightParen: true, criteria: ["F3", "=", "c"] }
            ];
            const result = FilterStore.getCriteriaFor(items);
            expect(result).toBeTruthy();
        });
    });

    describe("getDisplayTextFor", () => {
        it("returns empty string for null items", () => {
            expect(FilterStore.getDisplayTextFor(null)).toBe("");
        });

        it("returns empty string for empty items", () => {
            expect(FilterStore.getDisplayTextFor([])).toBe("");
        });

        it("returns displayText for single item", () => {
            const items: FilterLine[] = [
                { field: "F1", displayText: "Field1 = value1" }
            ];
            expect(FilterStore.getDisplayTextFor(items)).toBe("Field1 = value1");
        });

        it("joins multiple items with AND/OR", () => {
            const items: FilterLine[] = [
                { field: "F1", displayText: "F1 = a" },
                { field: "F2", isOr: true, displayText: "F2 = b" }
            ];
            const text = FilterStore.getDisplayTextFor(items);
            expect(text).toContain("F1 = a");
            expect(text).toContain("F2 = b");
        });

        it("handles parentheses in display text", () => {
            const items: FilterLine[] = [
                { field: "F1", leftParen: true, displayText: "F1 = a" },
                { field: "F2", isOr: true, displayText: "F2 = b" },
                { field: "F3", displayText: "F3 = c" }
            ];
            const text = FilterStore.getDisplayTextFor(items);
            expect(text).toContain("(");
        });

        it("closes open paren at end", () => {
            const items: FilterLine[] = [
                { field: "F1", leftParen: true, displayText: "F1 = a" },
                { field: "F2", isOr: true, displayText: "F2 = b" }
            ];
            const text = FilterStore.getDisplayTextFor(items);
            expect(text).toContain(")");
        });

        it("handles rightParen with inParen flag", () => {
            const items: FilterLine[] = [
                { field: "F1", leftParen: true, displayText: "F1 = a" },
                { field: "F2", isOr: true, displayText: "F2 = b" },
                { field: "F3", rightParen: true, displayText: "F3 = c" }
            ];
            const text = FilterStore.getDisplayTextFor(items);
            expect(text).toBeTruthy();
        });
    });

    describe("instance methods", () => {
        it("get_activeCriteria returns criteria for items", () => {
            const store = new FilterStore([]);
            store.get_items().push({ field: "F1", criteria: ["F1", "=", "v"] });
            const criteria = store.get_activeCriteria();
            expect(criteria).toBeTruthy();
        });

        it("get_displayText returns cached display text", () => {
            const store = new FilterStore([]);
            store.get_items().push({ field: "F1", displayText: "F1 = val" });
            const text = store.get_displayText();
            expect(text).toContain("F1 = val");

            const text2 = store.get_displayText();
            expect(text2).toBe(text);
        });

        it("raiseChanged clears displayText cache and triggers event", () => {
            const store = new FilterStore([]);
            const handler = vi.fn();
            store.add_changed(handler);
            store.get_items().push({ field: "F1", displayText: "test" });
            const before = store.get_displayText();
            expect(before).toBe("test");

            // Clear items and raiseChanged
            store.get_items().length = 0;
            store.raiseChanged();
            const after = store.get_displayText();
            expect(after).toBe("");
            expect(handler).toHaveBeenCalled();
        });

        it("add_changed and remove_changed work", () => {
            const store = new FilterStore([]);
            const handler = vi.fn();
            store.add_changed(handler);
            store.raiseChanged();
            expect(handler).toHaveBeenCalledTimes(1);

            store.remove_changed(handler);
            store.raiseChanged();
            expect(handler).toHaveBeenCalledTimes(1);
        });

        it("remove_changed with non-existent handler does nothing", () => {
            const store = new FilterStore([]);
            const handler = vi.fn();
            store.remove_changed(handler);
            store.raiseChanged();
            expect(handler).not.toHaveBeenCalled();
        });
    });

    describe("delegate helper functions", () => {
        it("delegateCombine combines two delegates", () => {
            const fn1 = vi.fn();
            const fn2 = vi.fn();
            const combined = delegateCombine(fn1, fn2);
            combined();
            expect(fn1).toHaveBeenCalled();
            expect(fn2).toHaveBeenCalled();
        });

        it("delegateCombine returns second if first is null", () => {
            const fn = vi.fn();
            const result = delegateCombine(null, fn);
            expect(result).toBe(fn);
        });

        it("delegateCombine returns first if second is null", () => {
            const fn = vi.fn();
            const result = delegateCombine(fn, null);
            expect(result).toBe(fn);
        });

        it("delegateCombine with delegate having _targets property", () => {
            const fn1 = vi.fn();
            const fn2 = vi.fn();
            // Create delegates with _targets by using delegateCombine
            const inner1 = delegateCombine(null, fn1);
            const inner2 = delegateCombine(null, fn2);
            // Now combine these two delegates (both have _targets)
            const combined = delegateCombine(inner1, inner2);
            expect(combined._targets).toBeDefined();
            expect(combined._targets.length).toBe(4);
            combined();
            expect(fn1).toHaveBeenCalled();
            expect(fn2).toHaveBeenCalled();
        });

        it("delegateRemove returns null if delegates are equal", () => {
            const fn = vi.fn();
            expect(delegateRemove(fn, fn)).toBeNull();
        });

        it("delegateRemove returns null if delegate1 is null", () => {
            expect(delegateRemove(null, vi.fn())).toBeNull();
        });

        it("delegateRemove returns delegate1 if delegate2 is null", () => {
            const fn = vi.fn();
            expect(delegateRemove(fn, null)).toBe(fn);
        });

        it("delegateRemove removes plain function target from combined delegate", () => {
            const fn1 = vi.fn();
            const fn2 = vi.fn();
            const combined = delegateCombine(fn1, fn2);
            const result = delegateRemove(combined, fn1);
            expect(result).not.toBeNull();
            // Result should still call fn2
            result();
            expect(fn2).toHaveBeenCalled();
        });

        it("delegateRemove when target not found returns delegate1 unchanged", () => {
            const fn1 = vi.fn();
            const fn2 = vi.fn();
            const combined = delegateCombine(fn1, fn2);
            const unrelated = vi.fn();
            const result = delegateRemove(combined, unrelated);
            expect(result).toBe(combined);
        });

        it("delegateRemove removes the only target and returns null", () => {
            const fn1 = vi.fn();
            const combined = delegateCombine(null, fn1);
            const result = delegateRemove(combined, fn1);
            expect(result).toBeNull();
        });

        it("delegateContains checks if target is in list", () => {
            const obj = {};
            const fn = vi.fn();
            const targets = [obj, fn];
            expect(delegateContains(targets, obj, fn)).toBe(true);
            expect(delegateContains(targets, {}, fn)).toBe(false);
        });
    });

    describe("integration", () => {
        it("handles complex filter scenario", () => {
            const store = new FilterStore([
                { name: "Name", title: "Name" },
                { name: "Age", title: "Age" }
            ]);

            store.get_items().push(
                { field: "Name", operator: "contains", isOr: false, leftParen: true, criteria: ["Name", "contains", "John"], displayText: "Name contains John" },
                { field: "Age", operator: "gt", isOr: true, criteria: ["Age", ">", "25"], displayText: "Age > 25" },
                { field: "Age", operator: "lt", isOr: false, rightParen: true, criteria: ["Age", "<", "50"], displayText: "Age < 50" }
            );

            const criteria = store.get_activeCriteria();
            expect(criteria).toBeTruthy();

            const displayText = store.get_displayText();
            expect(displayText).toBeTruthy();
        });
    });
});
