import { afterEach, describe, expect, it, vi } from "vitest";
import { SelectEditor } from "../editors/selecteditor";
import { QuickFilterBar } from "./quickfilterbar";

function createBar(options: any = {}) {
    const element = document.body.appendChild(document.createElement("div"));
    return new QuickFilterBar({
        filters: [
            { field: "Category", title: "Category", type: SelectEditor, options: { items: ["A", "B"] } },
            { field: "Supplier", title: "Supplier", type: SelectEditor, options: { items: ["X", "Y"] } }
        ],
        element,
        ...options
    } as any);
}

const button = (bar: QuickFilterBar) => bar.domNode.querySelector<HTMLButtonElement>(".quick-filters-clear");

afterEach(() => {
    QuickFilterBar.defaults.clearButton = true;
    vi.useRealTimers();
    document.body.innerHTML = "";
});

describe("the clear button of a quick filter bar", () => {
    it("is shown at the end of the bar while any filter is applied", () => {
        const bar = createBar();
        bar.onSubmit({} as any);
        expect(button(bar).hidden).toBe(true);
        expect(bar.domNode.lastElementChild).toBe(button(bar));

        bar.find(SelectEditor, "Category").value = "A";
        bar.onSubmit({} as any);
        expect(button(bar).hidden).toBe(false);

        // a filter that is added later goes before it
        bar.add({ field: "Other", title: "Other", type: SelectEditor, options: { items: ["Z"] } } as any);
        bar.onSubmit({} as any);
        expect(bar.domNode.lastElementChild).toBe(button(bar));
        bar.destroy();
    });

    it("is after the last filter, and before what else is in the bar, e.g. the view switch of a grid", () => {
        const bar = createBar();
        const viewSwitch = bar.domNode.appendChild(document.createElement("div"));
        bar.find(SelectEditor, "Category").value = "A";
        bar.onSubmit({} as any);
        expect(button(bar).previousElementSibling.getAttribute("data-qffield")).toBe("Supplier");
        expect(button(bar).nextElementSibling).toBe(viewSwitch);

        bar.add({ field: "Other", title: "Other", type: SelectEditor, options: { items: ["Z"] } } as any);
        bar.onSubmit({} as any);
        expect(button(bar).previousElementSibling.getAttribute("data-qffield")).toBe("Other");
        bar.destroy();
    });

    it("clears all the filters, and tells the change once", () => {
        vi.useFakeTimers();
        const bar = createBar();
        const onChange = vi.fn();
        bar.onChange = onChange;
        bar.find(SelectEditor, "Category").value = "A";
        bar.find(SelectEditor, "Supplier").value = "Y";
        vi.runAllTimers();
        onChange.mockClear();
        bar.onSubmit({} as any);

        button(bar).click();
        vi.runAllTimers();
        expect(bar.find(SelectEditor, "Category").value).toBeFalsy();
        expect(bar.find(SelectEditor, "Supplier").value).toBeFalsy();
        expect(onChange).toHaveBeenCalledTimes(1);

        bar.onSubmit({} as any);
        expect(button(bar).hidden).toBe(true);
        bar.destroy();
    });

    it("clears a filter that saves its state by loading an empty one", () => {
        vi.useFakeTimers();
        const loadState = vi.fn();
        const element = document.body.appendChild(document.createElement("div"));
        const bar = new QuickFilterBar({
            filters: [{ field: "Range", title: "Range", type: SelectEditor, options: { items: ["A"] }, loadState }],
            element
        } as any);

        bar.clear();
        vi.runAllTimers();
        expect(loadState).toHaveBeenCalledWith(bar.find(SelectEditor, "Range"), null);
        bar.destroy();
    });

    it("is left out when the bar, or the defaults, say so", () => {
        const own = createBar({ clearButton: false });
        own.find(SelectEditor, "Category").value = "A";
        own.onSubmit({} as any);
        expect(button(own)).toBeNull();
        own.destroy();

        QuickFilterBar.defaults.clearButton = false;
        const byDefault = createBar();
        byDefault.find(SelectEditor, "Category").value = "A";
        byDefault.onSubmit({} as any);
        expect(button(byDefault)).toBeNull();
        byDefault.destroy();
    });
});
