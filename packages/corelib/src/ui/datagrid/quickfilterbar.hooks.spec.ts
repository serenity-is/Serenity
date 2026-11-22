import { afterEach, describe, expect, it, vi } from "vitest";
import { SelectEditor } from "../editors/selecteditor";
import { QuickFilterBar } from "./quickfilterbar";

afterEach(() => {
    delete QuickFilterBar.defaults.created;
    delete QuickFilterBar.defaults.filterAdded;
    document.body.innerHTML = "";
});

describe("the hooks of the quick filter bars", () => {
    it("tell when a bar is created, before its filters are added, and when each filter is added", () => {
        const calls: string[] = [];
        QuickFilterBar.defaults.created = bar => calls.push("created:" + bar.domNode.querySelectorAll(".quick-filter-item").length);
        QuickFilterBar.defaults.filterAdded = args => calls.push("added:" + args.filter.field + ":" +
            args.label.textContent + ":" + (args.element.contains(args.widget.domNode) ? "in" : "out") + ":" +
            (args.bar.domNode.contains(args.element) ? "bar" : "-"));

        const element = document.body.appendChild(document.createElement("div"));
        const bar = new QuickFilterBar({
            filters: [
                { field: "Category", title: "Category", type: SelectEditor, options: { items: ["A"] } },
                { field: "Supplier", title: "Supplier", type: SelectEditor, options: { items: ["X"] } }
            ],
            element
        } as any);

        expect(calls).toEqual(["created:0", "added:Category:Category:in:bar", "added:Supplier:Supplier:in:bar"]);
        bar.destroy();
    });

    it("tell when a filter whose type is loaded later is added, once its widget is created", async () => {
        const filterAdded = vi.fn();
        QuickFilterBar.defaults.filterAdded = filterAdded;
        let resolve: (type: any) => void;
        const type = new Promise(r => resolve = r);

        const element = document.body.appendChild(document.createElement("div"));
        const bar = new QuickFilterBar({ filters: [{ field: "Lazy", title: "Lazy", type, options: { items: ["A"] } }], element } as any);
        expect(filterAdded).not.toHaveBeenCalled();

        resolve(SelectEditor);
        await type;
        await Promise.resolve();
        expect(filterAdded).toHaveBeenCalledTimes(1);
        expect(filterAdded.mock.calls[0][0].widget).toBeInstanceOf(SelectEditor);
        bar.destroy();
    });
});
