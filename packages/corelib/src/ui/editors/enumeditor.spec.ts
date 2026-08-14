import { describe, it, expect, vi } from "vitest";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { EnumEditor } from "./enumeditor";

enum TestEnum {
    Option1 = 1,
    Option2 = 2,
    Option3 = 3
}

describe("EnumEditor", () => {
    it("builds options from enumType", () => {
        const editor = new EnumEditor({ enumType: TestEnum } as any);
        const items = editor["_items"];
        expect(items.length).toBe(3);
        expect(items.map((x: any) => x.id)).toEqual(["1", "2", "3"]);
        editor.destroy();
    });

    it("allowClear defaults to true", () => {
        const editor = new EnumEditor({ enumType: TestEnum } as any);
        expect((editor as any).allowClear()).toBe(true);
        editor.destroy();
    });

    it("allowClear reflects option", () => {
        const editor = new EnumEditor({ enumType: TestEnum, allowClear: false } as any);
        expect((editor as any).allowClear()).toBe(false);
        editor.destroy();
    });

    it("does not throw when the enum type cannot be resolved", () => {
        const getOrLoad = vi.spyOn(EnumTypeRegistry, "getOrLoad").mockReturnValue(undefined as any);
        try {
            const editor = new EnumEditor({} as any);
            expect(editor["_items"]).toEqual([]);
            editor.destroy();
        }
        finally {
            getOrLoad.mockRestore();
        }
    });
});
