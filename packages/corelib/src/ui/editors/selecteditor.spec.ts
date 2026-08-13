import { describe, it, expect } from "vitest";
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
        expect((editor as any).emptyItemText()).toBeTruthy();
        editor.destroy();
    });
});
