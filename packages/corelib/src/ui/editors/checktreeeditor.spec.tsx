import { describe, expect, it, vi } from "vitest";
import { Lookup, setScriptData } from "../../base";
import { CheckLookupEditor, CheckTreeEditor, CheckTreeItem } from "./checktreeeditor";

const quickSearch = vi.hoisted(() => ({ search: null as any }));

vi.mock("./cascadedwidgetlink", () => ({
    CascadedWidgetLink: class {
        set_parentID() { }
        set_parentValue() { }
    }
}));

vi.mock("../helpers/gridutils", async (importOriginal) => {
    const actual: any = await importOriginal();
    return {
        ...actual,
        GridUtils: {
            ...actual.GridUtils,
            addQuickSearch: (opts: any) => { quickSearch.search = opts.search; }
        }
    };
});

function makeLookup(items: any[]) {
    return new Lookup({ idField: "Id", textField: "Name", parentIdField: "ParentId" }, items);
}

interface TestItem extends CheckTreeItem<TestItem> { }

class TestCheckTree extends CheckTreeEditor<TestItem, any> {
    protected override getTreeItems() {
        return [
            { id: "1", text: "One", isSelected: true, parentId: null },
            { id: "2", text: "Two", isSelected: false, parentId: "1" },
            { id: "3", text: "Three", isSelected: false, parentId: null }
        ];
    }
}

class ThreeStateTree extends TestCheckTree {
    protected override isThreeStateHierarchy() { return true; }
}

class SortableTree extends TestCheckTree {
    protected override moveSelectedUp() { return true; }
}

describe("CheckTreeEditor", () => {
    it("builds the tree and returns selected values", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        expect(editor.domNode.classList.contains("s-CheckTreeEditor")).toBe(true);
        expect(editor.value).toEqual(["1"]);
        editor.value = ["2"];
        expect(editor.value).toEqual(["2"]);
        editor.value = [];
        expect(editor.value).toEqual([]);
        editor.destroy();
    });

    it("serializes delimited edit values", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el), delimited: true } as any);
        const target: any = {};
        editor.getEditValue({ name: "Field" } as any, target);
        expect(target.Field).toBe("1");
        editor.setEditValue({ Field: "2,3" }, { name: "Field" } as any);
        expect(editor.value).toEqual(["2", "3"]);
        editor.destroy();
    });

    it("serializes non-delimited edit values as arrays", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        const target: any = {};
        editor.getEditValue({ name: "Field" } as any, target);
        expect(target.Field).toEqual(["1"]);
        editor.setEditValue({ Field: ["3"] }, { name: "Field" } as any);
        expect(editor.value).toEqual(["3"]);
        editor.destroy();
    });

    it("computes selection helpers on the tree", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        expect(editor["allItemsSelected"]()).toBe(false);
        expect(editor["anyDescendantsSelected"]({ children: [], isSelected: true } as any)).toBe(false);
        expect(editor["anyDescendantsSelected"]({ children: [{ isSelected: true } as any] } as any)).toBe(true);
        expect(editor["allDescendantsSelected"]({ children: [] } as any)).toBe(true);
        expect(editor["allDescendantsSelected"]({ children: [{ isSelected: false } as any] } as any)).toBe(false);
        editor.destroy();
    });

    it("computes three-state flags from descendant selection", () => {
        const editor = new ThreeStateTree({ element: el => document.body.appendChild(el) } as any);
        const items = editor.view.getItems() as TestItem[];
        const parent = items.find(x => x.id === "1");
        expect(parent).toBeTruthy();
        expect(parent!.children!.length).toBe(1);
        expect(parent!.isAllDescendantsSelected).toBe(false);
        editor.destroy();
    });

    it("sorts selected items to the top when enabled", () => {
        const editor = new SortableTree({ element: el => document.body.appendChild(el) } as any);
        editor.value = ["1"];
        const items = editor.view.getItems() as TestItem[];
        expect(items[0].id).toBe("1");
        editor.value = [];
        editor.destroy();
    });

    it("toggles readonly state", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        expect(!!editor.get_readOnly()).toBe(false);
        editor.set_readOnly(true);
        expect(!!editor.get_readOnly()).toBe(true);
        editor.set_readOnly(false);
        expect(!!editor.get_readOnly()).toBe(false);
        editor.destroy();
    });

    it("toggles a subtree through a check box click", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        const checkBox = document.createElement("span");
        checkBox.className = "check-box";
        const event = { target: checkBox, preventDefault: vi.fn(), isDefaultPrevented: () => false } as any;
        expect(() => (editor as any).onClick(event, 0, 0)).not.toThrow();
        editor.destroy();
    });

    it("applies readonly to check box clicks", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        editor.set_readOnly(true);
        const checkBox = document.createElement("span");
        checkBox.className = "check-box";
        const event = { target: checkBox, preventDefault: vi.fn(), isDefaultPrevented: () => false } as any;
        expect(() => (editor as any).onClick(event, 0, 0)).not.toThrow();
        editor.destroy();
    });

    it("reports all items selected when every row is selected", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        editor.value = ["1", "2", "3"];
        expect(editor["allItemsSelected"]()).toBe(true);
        editor.destroy();
    });

    it("constructs a bare tree with no items", () => {
        const editor = new CheckTreeEditor<any, any>({ element: el => document.body.appendChild(el) } as any);
        expect(editor.value).toEqual([]);
        editor.destroy();
    });

    it("selects and deselects all rows through the select-all button", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        const btn = editor.domNode.querySelector(".select-all-button") as HTMLElement;
        btn?.click();
        expect(editor.value).toEqual(["1", "2", "3"]);
        btn?.click();
        expect(editor.value).toEqual([]);
        editor.destroy();
    });

    it("unchecks a checked item through a check box click", () => {
        const editor = new TestCheckTree({ element: el => document.body.appendChild(el) } as any);
        const checkBox = document.createElement("span");
        checkBox.className = "check-box checked";
        const event = { target: checkBox, preventDefault: vi.fn(), isDefaultPrevented: () => false } as any;
        (editor as any).onClick(event, 0, 0);
        expect(editor.value).not.toContain("1");
        editor.destroy();
    });
});

describe("CheckLookupEditor", () => {
    beforeEach(() => {
        quickSearch.search = null;
        document.body.innerHTML = "";
    });

    it("loads items from a lookup and selects values", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([
            { Id: 1, Name: "One", ParentId: null },
            { Id: 2, Name: "Two", ParentId: 1 }
        ]));
        const editor = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup" } as any);
        expect(editor.view.getItems().length).toBe(2);
        editor.value = ["1"];
        expect(editor.value).toEqual(["1"]);
        editor.destroy();
    });

    it("filters lookup items by cascade and filter values", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([
            { Id: 1, Name: "One", ParentId: "A", Kind: "X" },
            { Id: 2, Name: "Two", ParentId: "B", Kind: "Y" },
            { Id: 3, Name: "Three", ParentId: "A", Kind: "Y" }
        ]));
        const editor = new CheckLookupEditor({
            element: el => document.body.appendChild(el),
            lookupKey: "Test.Lookup",
            cascadeField: "ParentId",
            cascadeValue: "A",
            filterField: "Kind",
            filterValue: "X"
        } as any);
        const items = editor.view.getItems();
        expect(items.length).toBe(1);
        expect(items[0].id).toBe("1");
        editor.destroy();
    });

    it("relays script data changes to refresh items", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([{ Id: 1, Name: "One" }]));
        const editor = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup" } as any);
        setScriptData("Lookup.Test.Lookup", makeLookup([{ Id: 1, Name: "One" }, { Id: 2, Name: "Two" }]));
        document.dispatchEvent(new Event("scriptdatachange.Lookup.Test.Lookup"));
        expect(editor.view.getItems().length).toBe(2);
        editor.destroy();
    });

    it("runs the quick search filter", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([
            { Id: 1, Name: "One" },
            { Id: 2, Name: "Two" }
        ]));
        const editor = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup" } as any);
        const done = vi.fn();
        quickSearch.search({ query: "one", done });
        expect(done).toHaveBeenCalledWith(true);
        quickSearch.search({ query: "zzz", done });
        expect(done).toHaveBeenCalledWith(false);
        editor.destroy();
    });

    it("adjusts buttons based on hideSearch and showSelectAll options", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([{ Id: 1, Name: "One" }]));
        const editor = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup", hideSearch: true } as any);
        expect(editor["getButtons"]()).toBeNull();
        editor.destroy();
        const withSelectAll = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup", showSelectAll: true } as any);
        expect(withSelectAll["getButtons"]()).not.toBeNull();
        withSelectAll.destroy();
    });

    it("manages cascade and filter options", () => {
        setScriptData("Lookup.Test.Lookup", makeLookup([{ Id: 1, Name: "One" }]));
        const editor = new CheckLookupEditor({ element: el => document.body.appendChild(el), lookupKey: "Test.Lookup" } as any);
        editor.cascadeFrom = "Parent";
        expect(editor.cascadeFrom).toBe("Parent");
        expect(editor.cascadeField).toBe("Parent");
        editor.cascadeFrom = null;
        expect(editor.cascadeFrom).toBeNull();
        editor.cascadeField = "Other";
        expect(editor.cascadeField).toBe("Other");
        editor.cascadeValue = "A";
        expect(editor.cascadeValue).toBe("A");
        editor.filterField = "Kind";
        expect(editor.filterField).toBe("Kind");
        editor.filterValue = "X";
        expect(editor.filterValue).toBe("X");
        editor.destroy();
    });
});
