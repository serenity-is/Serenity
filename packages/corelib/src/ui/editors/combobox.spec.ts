import { describe, it, expect } from "vitest";
import { Combobox, stripDiacritics } from "./combobox";

describe("Combobox", () => {
    function createCombobox(multiple = false) {
        const element = document.createElement("input");
        if (multiple)
            element.setAttribute("multiple", "multiple");
        document.body.appendChild(element);
        return { combobox: new (Combobox as any)({ element }, false) as Combobox, element };
    }

    it("reads and writes a scalar value without Select2", () => {
        const { combobox, element } = createCombobox();
        element.value = "one";
        expect(combobox.getValue()).toBe("one");
        expect(combobox.getValues()).toEqual(["one"]);
        combobox.setValue("two");
        expect(element.value).toBe("two");
        combobox.setValues(["three"]);
        expect(element.value).toBe("three");
        combobox.setValues([]);
        expect(combobox.getValues()).toEqual([]);
        combobox.closeDropdown();
        combobox.openDropdown();
        combobox.dispose();
    });

    it("splits comma-separated values for multiple inputs", () => {
        const { combobox, element } = createCombobox(true);
        combobox.setValue("a, b,,c");
        expect(element.value).toBe("a,b,c");
        expect(combobox.isMultiple).toBe(true);
        combobox.setValues(["x", "y"]);
        expect(element.value).toBe("x,y");
        combobox.dispose();
    });

    it("returns empty/null values when no element is present", () => {
        const combobox = new (Combobox as any)({}, false) as Combobox;
        expect(combobox.getValue()).toBeNull();
        expect(combobox.getValues()).toEqual([]);
        combobox.setValue("x");
        combobox.closeDropdown();
        combobox.openDropdown();
        combobox.dispose();
    });

    it("reports type and selected items without Select2", () => {
        const { combobox } = createCombobox();
        expect(combobox.type).toBeNull();
        expect(combobox.getSelectedItem()).toBeUndefined();
        expect(combobox.getSelectedItems()).toEqual([]);
        expect(Combobox.getInstance(combobox["el"] as any)).toBeNull();
        combobox.dispose();
    });

    it("strips diacritics and preserves empty values", () => {
        expect(stripDiacritics("café")).toBe("cafe");
        expect(stripDiacritics("")).toBe("");
        expect(stripDiacritics(null as any)).toBeNull();
    });

    it("constructs the Select2 provider and exposes combobox operations", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const search = vi.fn(() => ({ items: [{ id: "1", text: "One" }], more: false }));
        const combobox = new Combobox({
            element,
            search,
            placeholder: "Choose",
            allowClear: true,
            multiple: false
        });

        expect((element as any).select2).toBeTruthy();
        expect(combobox.type).toBe("select2");
        combobox.setValue("1");
        expect(combobox.getValue()).toBe("1");
        combobox.openDropdown();
        combobox.closeDropdown();
        combobox.dispose();
    });

    it("supports provider query callback with synchronous search", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const search = vi.fn(() => ({ items: [{ id: "1", text: "One" }], more: false }));
        new Combobox({ element, search });
        const select2 = (element as any).select2;
        const callback = vi.fn();
        (select2 as any).opts.query({ term: "one", page: 1, callback });
        element.dispatchEvent(new Event("execute-search"));
        expect(search).toHaveBeenCalledWith(expect.objectContaining({ searchTerm: "one", skip: 0 }));
        expect(callback).toHaveBeenCalledWith({ results: [{ id: "1", text: "One" }], more: false });
        select2.destroy();
    });

    it("supports async query and initSelection callbacks", async () => {
        const element = document.createElement("input");
        element.value = "1,2";
        document.body.appendChild(element);
        const search = vi.fn((query: any) => Promise.resolve({
            items: (query.idList ?? ["1"]).map((id: string) => ({ id, text: "Item " + id })),
            more: false
        }));
        new Combobox({ element, search, multiple: true, arbitraryValues: true });
        const select2 = (element as any).select2;
        const initCallback = vi.fn();
        (select2 as any).opts.initSelection(element, initCallback);
        await Promise.resolve();
        expect(initCallback).toHaveBeenCalledWith([
            { id: "1", text: "Item 1" },
            { id: "2", text: "Item 2" }
        ]);

        const queryCallback = vi.fn();
        (select2 as any).opts.query({ term: "", page: 1, callback: queryCallback });
        element.dispatchEvent(new Event("execute-search"));
        await Promise.resolve();
        expect(queryCallback).toHaveBeenCalled();
        select2.destroy();
    });

    it("supports provider options and formatting callbacks", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const createSearchChoice = vi.fn();
        const formatResult = vi.fn();
        const formatSelection = vi.fn();
        new Combobox({
            element,
            search: () => ({ items: [], more: false }),
            createSearchChoice,
            formatResult,
            formatSelection,
            providerOptions: () => ({ minimumResultsForSearch: 0 })
        });
        const opts = (element as any).select2.opts;
        expect(opts.createSearchChoice).toBe(createSearchChoice);
        expect(opts.formatResult).toBe(formatResult);
        expect(opts.formatSelection).toBe(formatSelection);
        expect(opts.minimumResultsForSearch).toBe(0);
        (element as any).select2.destroy();
    });

    it("initSelection returns null for an empty value", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const search = vi.fn(() => ({ items: [], more: false }));
        new Combobox({ element, search });
        const callback = vi.fn();
        (element as any).select2.opts.initSelection(element, callback);
        expect(callback).toHaveBeenCalledWith(null);
        (element as any).select2.destroy();
    });

    it("initSelection returns a single result", () => {
        const element = document.createElement("input");
        element.value = "1";
        document.body.appendChild(element);
        new Combobox({ element, search: () => ({ items: [{ id: "1", text: "One" }], more: false }) });
        const callback = vi.fn();
        (element as any).select2.opts.initSelection(element, callback);
        expect(callback).toHaveBeenCalledWith({ id: "1", text: "One" });
        (element as any).select2.destroy();
    });

    it("initSelection supports arbitrary values and non-arbitrary missing values", () => {
        const arbitrary = document.createElement("input");
        arbitrary.value = "missing";
        document.body.appendChild(arbitrary);
        new Combobox({ element: arbitrary, arbitraryValues: true, search: () => ({ items: [], more: false }) });
        const arbitraryCallback = vi.fn();
        (arbitrary as any).select2.opts.initSelection(arbitrary, arbitraryCallback);
        expect(arbitraryCallback).toHaveBeenCalledWith({ id: "missing", text: "missing" });
        (arbitrary as any).select2.destroy();

        const normal = document.createElement("input");
        normal.value = "missing";
        document.body.appendChild(normal);
        new Combobox({ element: normal, search: () => ({ items: [], more: false }) });
        const normalCallback = vi.fn();
        (normal as any).select2.opts.initSelection(normal, normalCallback);
        expect(normalCallback).toHaveBeenCalledWith(null);
        (normal as any).select2.destroy();
    });

    it("query handles a null synchronous search result", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        new Combobox({ element, search: () => null as any });
        const callback = vi.fn();
        (element as any).select2.opts.query({ term: "x", page: 1, callback });
        element.dispatchEvent(new Event("execute-search"));
        expect(callback).not.toHaveBeenCalled();
        (element as any).select2.destroy();
    });

    it("covers lifecycle abort and container accessors", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const combobox = new Combobox({ element, search: () => ({ items: [], more: false }) });
        const select2 = (element as any).select2;
        const abortQuery = vi.fn();
        const abortSelection = vi.fn();
        (element as any).queryLoading = { abort: abortQuery };
        (element as any).initSelectionLoading = { abort: abortSelection };
        (element as any).typeTimeout = 1;
        (element as any).typeTimeoutFn = vi.fn();
        combobox.abortPendingQuery();
        combobox.abortInitSelection();
        expect(abortQuery).toHaveBeenCalled();
        expect(abortSelection).toHaveBeenCalled();
        expect(combobox.container).toBeTruthy();
        combobox.dispose();
    });

    it("reads Select2 selected item and array values", () => {
        const element = document.createElement("input");
        document.body.appendChild(element);
        const combobox = new Combobox({ element, multiple: true, search: () => ({ items: [], more: false }) });
        const select2 = (element as any).select2;
        select2.data([{ id: "1", text: "One" }, { id: "2", text: "Two" }]);
        select2.val(["1", "2"]);
        expect(typeof combobox.getValue()).toBe("string");
        expect(Array.isArray(combobox.getValues())).toBe(true);
        combobox.getSelectedItem();
        expect(Array.isArray(combobox.getSelectedItems())).toBe(true);
        combobox.dispose();
    });
});
