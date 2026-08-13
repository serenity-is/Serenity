import { describe, it, expect, vi, beforeEach } from "vitest";
import { AutoNumeric } from "./autonumeric";
import { IntegerEditor, type IntegerEditorOptions } from "./integereditor";

describe("IntegerEditor", () => {
    beforeEach(() => { vi.restoreAllMocks(); });

    function createEditor(options?: IntegerEditorOptions) {
        vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        return new IntegerEditor(options ?? {});
    }

    it("get_value returns int from AutoNumeric", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("42");
        const editor = createEditor();
        expect(editor.get_value()).toBe(42);
        editor.destroy();
    });

    it("get_value returns null for empty AutoNumeric value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("");
        const editor = createEditor();
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("get_value parses domNode when no AutoNumeric instance", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(false);
        const editor = createEditor();
        editor.domNode.value = "7";
        expect(editor.get_value()).toBe(7);
        editor.destroy();
    });

    it("get_value returns null when domNode empty", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(false);
        const editor = createEditor();
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("set_value clears domNode when null", () => {
        const editor = createEditor();
        editor.set_value(null as any);
        expect(editor.domNode.value).toBe("");
        editor.destroy();
    });

    it("set_value uses AutoNumeric when instance exists", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        const setValueSpy = vi.spyOn(AutoNumeric, "setValue").mockImplementation(() => { });
        const editor = createEditor();
        editor.set_value(7);
        expect(setValueSpy).toHaveBeenCalledWith(editor.domNode, 7);
        editor.destroy();
    });

    it("set_value formats when no AutoNumeric instance", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(false);
        const editor = createEditor();
        editor.set_value(42);
        expect(editor.domNode.value).toBe("42");
        editor.destroy();
    });

    it("value setter delegates to set_value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        const setValueSpy = vi.spyOn(AutoNumeric, "setValue").mockImplementation(() => { });
        const editor = createEditor();
        editor.value = 9;
        expect(setValueSpy).toHaveBeenCalled();
        editor.destroy();
    });

    it("get_isValid reflects value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("5");
        const editor = createEditor();
        expect(editor.get_isValid()).toBe(true);
        editor.destroy();
    });

    it("getAutoNumericOptions applies allowNegatives and min/max", () => {
        const initSpy = vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        const editor = new IntegerEditor({ allowNegatives: true, maxValue: 100 } as any);
        const opts = initSpy.mock.calls[0][1];
        expect(opts.vMin).toBe("-100");
        expect(opts.vMax).toBe(100);
        expect(opts.aSep).toBeNull();
        editor.destroy();
    });

    it("destroy calls AutoNumeric.destroy", () => {
        const destroySpy = vi.spyOn(AutoNumeric, "destroy");
        const editor = createEditor();
        editor.destroy();
        expect(destroySpy).toHaveBeenCalled();
    });
});
