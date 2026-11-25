import { describe, expect, it } from "vitest";
import { SelectEditor } from "./selecteditor";

describe("SelectEditor", () => {
    it("builds options from string items", () => {
        const editor = new SelectEditor({ items: ["a", "b", "c"] } as any);
        expect(editor["_items"].map((x: any) => x.id)).toEqual(["a", "b", "c"]);
        editor.destroy();
    });

    it("builds options from array items", () => {
        const editor = new SelectEditor({ items: [["k1", "Text1"], ["k2"], ["k3", "Text3"]] } as any);
        const items = editor["_items"];
        expect(items[0].id).toBe("k1");
        expect(items[0].text).toBe("Text1");
        expect(items[1].id).toBe("k2");
        expect(items[1].text).toBe("k2"); // item[1] ?? item[0]
        expect(items[2].id).toBe("k3");
        expect(items[2].text).toBe("Text3");
        editor.destroy();
    });

    it("getItems returns options.items", () => {
        const items = ["a", "b"];
        const editor = new SelectEditor({ items } as any);
        expect(editor.getItems()).toBe(items);
        editor.destroy();
    });

    it("emptyItemText uses emptyOptionText when set", () => {
        const editor = new SelectEditor({ items: [], emptyOptionText: "Choose" } as any);
        expect((editor as any).emptyItemText()).toBe("Choose");
        editor.destroy();
    });

    it("emptyItemText falls back to base when not set", () => {
        const editor = new SelectEditor({ items: [] } as any);
        // the default of the base, see ComboboxEditor.defaults, which is an empty text, and not null
        expect((editor as any).emptyItemText()).toBe("");
        editor.destroy();
    });

    it("setItems replaces the items, and keeps the value only while it is one of them", () => {
        const editor = new SelectEditor({ items: ["a", "b"] } as any);
        editor.value = "b";
        editor.setItems([["b", "Bee"], ["c", "Cee"]]);
        expect(editor["_items"].map((x: any) => x.text)).toEqual(["Bee", "Cee"]);
        expect(editor.value).toBe("b");
        editor.setItems(["c"]);
        expect(editor.value).toBeFalsy();
        editor.destroy();
    });

    it("itemFilter limits the items it offers, but not the selected one", () => {
        const editor = new SelectEditor({ items: ["a", "b", "c"] } as any);
        editor.value = "b";
        editor.itemFilter = item => item.id !== "b";
        const options = (editor as any).getComboboxOptions();
        expect(options.search({ searchTerm: "", skip: 0, take: 10 }).items.map((x: any) => x.id)).toEqual(["a", "c"]);
        expect(options.search({ initSelection: true, idList: ["b"], skip: 0 }).items[0].id).toBe("b");
        expect(editor.value).toBe("b");
        editor.itemFilter = null;
        expect(options.search({ searchTerm: "", skip: 0, take: 10 }).items).toHaveLength(3);
        editor.destroy();
    });
});
