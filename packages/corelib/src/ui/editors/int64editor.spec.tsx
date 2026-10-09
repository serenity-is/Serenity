import { describe, expect, it } from "vitest";
import { EditorTypeRegistry } from "../../types/editortyperegistry";
import { Int64Editor } from "./int64editor";

describe("Int64Editor", () => {
    function createEditor(options?: any) {
        return new Int64Editor(options ?? {});
    }

    it("defaults to the unsigned 64-bit range and wires validation class", () => {
        const editor = createEditor();
        expect(editor.domNode.getAttribute("min")).toBe("0");
        expect(editor.domNode.getAttribute("max")).toBe("9223372036854775807");
        expect(editor.domNode.classList.contains("int64")).toBe(true);
        expect(editor.domNode.pattern).toBe("[0-9]*");
        editor.destroy();
    });

    it("allows negatives when configured", () => {
        const editor = createEditor({ allowNegatives: true });
        expect(editor.domNode.getAttribute("min")).toBe("-9223372036854775808");
        expect(editor.domNode.pattern).toBe("-?[0-9]*");
        editor.destroy();
    });

    it("accepts min/max options outside the safe integer range as strings", () => {
        const editor = createEditor({ minValue: "-9007199254740993", maxValue: "9007199254740993" });
        expect(editor.domNode.getAttribute("min")).toBe("-9007199254740993");
        expect(editor.domNode.getAttribute("max")).toBe("9007199254740993");
        editor.destroy();
    });

    it("convertToInt64 returns null for null/empty", () => {
        expect(Int64Editor.convertToInt64(null)).toBeNull();
        expect(Int64Editor.convertToInt64(undefined)).toBeNull();
        expect(Int64Editor.convertToInt64("")).toBeNull();
    });

    it("convertToInt64 accepts the full 64-bit range from strings and bigints", () => {
        expect(Int64Editor.convertToInt64("9223372036854775807")).toBe(9223372036854775807n);
        expect(Int64Editor.convertToInt64("-9223372036854775808")).toBe(-9223372036854775808n);
        expect(Int64Editor.convertToInt64(7)).toBe(7n);
        expect(Int64Editor.convertToInt64(true)).toBe(1n);
    });

    it("convertToInt64 rejects decimals, out-of-range and non-numeric values", () => {
        expect(() => Int64Editor.convertToInt64(1.5)).toThrow();
        expect(() => Int64Editor.convertToInt64("9223372036854775808")).toThrow();
        expect(() => Int64Editor.convertToInt64("-9223372036854775809")).toThrow();
        expect(() => Int64Editor.convertToInt64("abc")).toThrow();
        expect(() => Int64Editor.convertToInt64(Number.MAX_SAFE_INTEGER + 1)).toThrow();
    });

    it("get_value parses large longs and nulls invalid input", () => {
        const editor = createEditor();
        editor.domNode.value = "9223372036854775807";
        expect(editor.get_value()).toBe(9223372036854775807n);
        editor.domNode.value = "not-a-number";
        expect(editor.get_value()).toBeNull();
        editor.domNode.value = "";
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("get_isValid reflects the input", () => {
        const editor = createEditor();
        editor.domNode.value = "123";
        expect(editor.get_isValid()).toBe(true);
        editor.domNode.value = "1.5";
        expect(editor.get_isValid()).toBe(false);
        editor.destroy();
    });

    it("set_value writes digits and clears on null", () => {
        const editor = createEditor();
        editor.set_value(9223372036854775807n);
        expect(editor.domNode.value).toBe("9223372036854775807");
        editor.set_value("42");
        expect(editor.domNode.value).toBe("42");
        editor.set_value(7);
        expect(editor.domNode.value).toBe("7");
        editor.set_value(null);
        expect(editor.domNode.value).toBe("");
        editor.destroy();
    });

    it("getEditValue writes a number within the safe range, string beyond it, null for invalid", () => {
        const editor = createEditor();
        const target: any = {};

        editor.domNode.value = "12345";
        editor.getEditValue({ name: "Id" } as any, target);
        expect(target.Id).toBe(12345);

        editor.domNode.value = "9223372036854775807";
        editor.getEditValue({ name: "Id" } as any, target);
        expect(target.Id).toBe("9223372036854775807");

        editor.domNode.value = "invalid";
        editor.getEditValue({ name: "Id" } as any, target);
        expect(target.Id).toBeNull();

        editor.destroy();
    });

    it("is resolvable by the editor type registry as Int64", () => {
        expect(EditorTypeRegistry.get("Int64")).toBe(Int64Editor);
    });
});
