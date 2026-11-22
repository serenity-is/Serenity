import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { BooleanFiltering } from "../filtering/booleanfiltering";
import { DateFiltering } from "../filtering/datefiltering";
import { DateTimeFiltering } from "../filtering/datetimefiltering";
import { FilteringTypeRegistry } from "../filtering/filteringtyperegistry";
import { addLocalText } from "../../base";
import { getDefaultSortBy, getItemCssClass, propertyItemToQuickFilter, sleekGridOnSort } from "./datagrid-internal";
import { QuickFilterBar } from "./quickfilterbar";
import "../filtering/stringfiltering";

describe("getDefaultSortBy", () => {
    it("returns empty array for null grid", () => {
        expect(getDefaultSortBy(null)).toEqual([]);
    });

    it("returns empty when no columns have sortOrder", () => {
        const grid = { getColumns: vi.fn(() => [{ field: "A" }, { field: "B" }]) } as any;
        expect(getDefaultSortBy(grid)).toEqual([]);
    });

    it("sorts columns by absolute sortOrder", () => {
        const grid = {
            getColumns: vi.fn(() => [
                { field: "A", sortOrder: 2 },
                { field: "B", sortOrder: -1 },
                { field: "C", sortOrder: 3 }
            ])
        } as any;
        expect(getDefaultSortBy(grid)).toEqual(["B DESC", "A", "C"]);
    });
});

describe("getItemCssClass", () => {
    it("returns null when both active and deleted fields are set", () => {
        expect(getItemCssClass({}, "A", "D")).toBeNull();
    });

    it("returns null when active value is null", () => {
        expect(getItemCssClass({ A: null }, "A", "")).toBeNull();
    });

    it("returns deleted for negative number", () => {
        expect(getItemCssClass({ A: -1 }, "A", "")).toBe("deleted");
    });

    it("returns inactive for zero", () => {
        expect(getItemCssClass({ A: 0 }, "A", "")).toBe("inactive");
    });

    it("returns deleted for false boolean", () => {
        expect(getItemCssClass({ A: false }, "A", "")).toBe("deleted");
    });

    it("returns null for positive number", () => {
        expect(getItemCssClass({ A: 5 }, "A", "")).toBeNull();
    });

    it("uses deleted field when no active field", () => {
        expect(getItemCssClass({ D: true }, "", "D")).toBe("deleted");
        expect(getItemCssClass({ D: false }, "", "D")).toBeNull();
    });
});

describe("sleekGridOnSort", () => {
    it("handles multi-column sort and refreshes view", () => {
        const view = { populateLock: vi.fn(), populateUnlock: vi.fn(), seekToPage: 0, sortBy: [], getLocalSort: vi.fn(() => true), sort: vi.fn(), populate: vi.fn() } as any;
        const p = { multiColumnSort: true, sortCols: [{ sortCol: { field: "A" }, sortAsc: true }, { sortCol: { field: "B" }, sortAsc: false }] } as any;
        sleekGridOnSort(view, p);
        expect(view.populateLock).toHaveBeenCalled();
        expect(view.populateUnlock).toHaveBeenCalled();
        expect(view.seekToPage).toBe(1);
        expect(view.sortBy).toEqual(["A", "B DESC"]);
        expect(view.sort).toHaveBeenCalled();
    });

    it("handles single-column sort and calls populate when no local sort", () => {
        const view = { populateLock: vi.fn(), populateUnlock: vi.fn(), seekToPage: 0, sortBy: [], getLocalSort: vi.fn(() => false), sort: vi.fn(), populate: vi.fn() } as any;
        const p = { multiColumnSort: false, sortCol: { field: "A" }, sortAsc: false } as any;
        sleekGridOnSort(view, p);
        expect(view.sortBy).toEqual(["A DESC"]);
        expect(view.populate).toHaveBeenCalled();
    });

    it("handles null sort col", () => {
        const view = { populateLock: vi.fn(), populateUnlock: vi.fn(), seekToPage: 0, sortBy: [], getLocalSort: vi.fn(() => false), sort: vi.fn(), populate: vi.fn() } as any;
        sleekGridOnSort(view, { multiColumnSort: true, sortCols: [{ sortCol: null, sortAsc: true }] } as any);
        expect(view.sortBy).toEqual(["undefined"]);
    });
});

describe("propertyItemToQuickFilter", () => {
    beforeEach(() => { vi.restoreAllMocks(); });
    afterEach(() => { vi.restoreAllMocks(); });

    it("returns date range filter for DateFiltering", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(DateFiltering as any);
        const result = propertyItemToQuickFilter({ name: "Date", title: "Date" } as any);
        expect(result.field).toBe("Date");
        expect(result.type).toBeDefined();
    });

    it("returns the date range filter that the application gives, when it gives one", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(DateFiltering as any);
        const own = vi.fn((field: string, title?: string) => ({ field, title, type: class { } as any }));
        QuickFilterBar.defaults.dateRangeFilter = own;
        try {
            const result = propertyItemToQuickFilter({ name: "Date", title: "Order Date" } as any);
            expect(own).toHaveBeenCalledWith("Date", "Order Date");
            expect(result.title).toBe("Order Date");
        }
        finally {
            delete QuickFilterBar.defaults.dateRangeFilter;
        }
    });

    it("returns date time range filter for DateTimeFiltering", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(DateTimeFiltering as any);
        const result = propertyItemToQuickFilter({ name: "Date", title: "Date" } as any);
        expect(result.field).toBe("Date");
    });

    it("returns boolean filter for BooleanFiltering", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(BooleanFiltering as any);
        const result = propertyItemToQuickFilter({ name: "Active", title: "Active" } as any);
        expect(result.field).toBe("Active");
    });

    it("returns null when filtering type is not IQuickFiltering", () => {
        class NotQuick { }
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(NotQuick as any);
        expect(propertyItemToQuickFilter({ name: "X", title: "X" } as any)).toBeNull();
    });

    it("sets separator and cssClass from item", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(DateFiltering as any);
        const result = propertyItemToQuickFilter({ name: "Date", title: "Date", quickFilterSeparator: true, quickFilterCssClass: "c" } as any);
        expect(result.separator).toBe(true);
        expect(result.cssClass).toBe("c");
        expect(result.pinned).toBeUndefined();
    });

    it("pins the quick filter that the server pins, see QuickFilterAttribute.Pinned", () => {
        vi.spyOn(FilteringTypeRegistry, "get").mockReturnValue(DateFiltering as any);
        const result = propertyItemToQuickFilter({ name: "Date", title: "Date", quickFilterPinned: true } as any);
        expect(result.pinned).toBe(true);
    });
});

describe("the placeholder of a quick filter", () => {
    beforeEach(() => { vi.restoreAllMocks(); });

    it("is resolved from its local text key, which the server gives, see QuickFilterAttribute.Placeholder", () => {
        addLocalText("Columns.Test.City_QuickFilter_Placeholder", "Any city");
        const result = propertyItemToQuickFilter({
            name: "City", title: "City", filteringType: "String",
            quickFilterParams: { placeholder: "Columns.Test.City_QuickFilter_Placeholder" }
        } as any);
        expect(result.options.placeholder).toBe("Any city");
    });

    it("is the text itself when it is not a key, or an empty text to show none", () => {
        const text = propertyItemToQuickFilter({
            name: "City", title: "City", filteringType: "String", quickFilterParams: { placeholder: "Any city" }
        } as any);
        expect(text.options.placeholder).toBe("Any city");

        const empty = propertyItemToQuickFilter({
            name: "City", title: "City", filteringType: "String", quickFilterParams: { placeholder: "" }
        } as any);
        expect(empty.options.placeholder).toBe("");
    });

    it("is the placeholder of the editor in the quick filter bar", () => {
        addLocalText("Columns.Test.Region_QuickFilter_Placeholder", "Any region");
        const filter = propertyItemToQuickFilter({
            name: "Region", title: "Region", filteringType: "String",
            quickFilterParams: { placeholder: "Columns.Test.Region_QuickFilter_Placeholder" }
        } as any);

        const bar = new QuickFilterBar({ filters: [filter], element: document.body.appendChild(document.createElement("div")) } as any);
        expect(bar.domNode.querySelector("[data-qffield=Region] input").getAttribute("placeholder")).toBe("Any region");
        bar.destroy();
    });
});
