import { beforeEach, describe, expect, it } from "vitest";
import { Aggregators } from "./aggregators";
import { RemoteView } from "./remoteview";

describe("RemoteView row stripes", () => {
    let view: RemoteView<any>;

    function stripes() {
        const result: string[] = [];
        for (let i = 0; i < view.getLength(); i++)
            result.push(view.getRowStripe(i) ?? "-");
        return result.join(" ");
    }

    beforeEach(() => {
        view = new RemoteView({ idField: "id" });
        view.setItems([
            { id: 1, cat: "a", sub: "x" },
            { id: 2, cat: "a", sub: "x" },
            { id: 3, cat: "a", sub: "y" },
            { id: 4, cat: "b", sub: "x" },
            { id: 5, cat: "b", sub: "x" },
            { id: 6, cat: "b", sub: "x" }
        ]);
    });

    it("stripes the rows by their index without grouping", () => {
        expect(stripes()).toBe("even odd even odd even odd");
    });

    it("starts the data rows of every group with the stripe of the grid's first row and leaves group headers unstriped", () => {
        view.setGrouping([{ getter: "cat", collapsed: false }]);
        expect(stripes()).toBe("- even odd even - even odd even");
    });

    it("counts the rows of nested groups from their own header", () => {
        view.setGrouping([{ getter: "cat", collapsed: false }, { getter: "sub", collapsed: false }]);
        expect(stripes()).toBe("- - even odd - even - - even odd even");
    });

    it("leaves totals rows unstriped and restarts the count after them", () => {
        view.setGrouping([{ getter: "cat", collapsed: false, displayTotalsRow: true, aggregators: [new Aggregators.Sum("id")] }]);
        expect(stripes()).toBe("- even odd even - - even odd even -");
    });

    it("keeps the stripes of the rows below a group when it collapses", () => {
        view.setGrouping([{ getter: "cat", collapsed: false }]);
        view.collapseGroup(["a"]);
        expect(stripes()).toBe("- - even odd even");
        view.expandGroup(["a"]);
        expect(stripes()).toBe("- even odd even - even odd even");
    });

    it("goes back to striping by index when grouping is removed", () => {
        view.setGrouping([{ getter: "cat", collapsed: false }]);
        view.setGrouping([]);
        expect(stripes()).toBe("even odd even odd even odd");
    });

    it("reports a row whose item stays at the same index but whose stripe changes as changed", () => {
        view.setGrouping([{ getter: "cat", collapsed: false }]);
        // rows: [A, 1, 2, 3, B, 4, 5, 6]; moving item 3 to group "b" gives [A, 1, 2, B, 3, 4, 5, 6]
        // where items 4, 5 and 6 stay at the same index but each one is one row further from its header
        const changed: number[] = [];
        view.onRowsChanged.subscribe((_, args) => changed.push(...args.rows));
        view.updateItem(3, { id: 3, cat: "b", sub: "y" });
        expect(stripes()).toBe("- even odd - even odd even odd");
        expect(changed).toEqual(expect.arrayContaining([5, 6, 7]));
    });
});
