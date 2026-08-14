import { Culture } from "../../base";
import { AutoNumeric } from "./autonumeric";
import { DecimalEditor, type DecimalEditorOptions } from "./decimaleditor";

beforeEach(() => {
    vi.clearAllMocks();
});

const newEditor = async (opt: DecimalEditorOptions) => new (await import("./decimaleditor")).DecimalEditor(opt);

describe("DecimalEditor", () => {
    it("adds default values to autonumeric", async () => {
        Culture.decimalSeparator = ".";

        vi.spyOn(AutoNumeric, "init").mockImplementation((_, options) => {
            expect(options.vMin).toBe('0.00');
            expect(options.vMax).toBe('999999999999.99');
            expect(options.aPad).toBe(true);
            expect(options.aDec).toBe('.');
            expect(options.altDec).toBe(',');
            expect(options.aSep).toBe(',');
            return null;
        });

        await newEditor({});
    });
});

describe("DecimalEditor value handling", () => {
    beforeEach(() => { Culture.decimalSeparator = "."; vi.restoreAllMocks(); });

    function createEditor(options?: DecimalEditorOptions) {
        vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        return new DecimalEditor(options ?? {});
    }

    it("get_value returns AutoNumeric value when instance exists", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("1234.5");
        const editor = createEditor();
        expect(editor.get_value()).toBe(1234.5);
        editor.destroy();
    });

    it("get_value returns null for empty AutoNumeric value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("");
        const editor = createEditor();
        expect(editor.get_value()).toBeNull();
        editor.destroy();
    });

    it("get_value parses domNode value when no AutoNumeric instance", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(false);
        const editor = createEditor();
        editor.domNode.value = "42.5";
        expect(editor.get_value()).toBe(42.5);
        editor.destroy();
    });

    it("set_value clears domNode value when null", () => {
        const editor = createEditor();
        editor.set_value(null as any);
        expect(editor.domNode.value).toBe("");
        editor.destroy();
    });

    it("set_value uses AutoNumeric when instance exists", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        const setValueSpy = vi.spyOn(AutoNumeric, "setValue").mockImplementation((() => { }) as any);
        const editor = createEditor();
        editor.set_value(1234.5);
        expect(setValueSpy).toHaveBeenCalledWith(editor.domNode, 1234.5);
        editor.destroy();
    });

    it("set_value formats when no AutoNumeric instance", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(false);
        const editor = createEditor();
        editor.set_value(42.5);
        expect(editor.domNode.value).toBe("42.5");
        editor.destroy();
    });

    it("value setter delegates to set_value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        const setValueSpy = vi.spyOn(AutoNumeric, "setValue").mockImplementation((() => { }) as any);
        const editor = createEditor();
        editor.value = 9.5;
        expect(setValueSpy).toHaveBeenCalled();
        editor.destroy();
    });

    it("get_isValid reflects value", () => {
        vi.spyOn(AutoNumeric, "hasInstance").mockReturnValue(true);
        vi.spyOn(AutoNumeric, "getValue").mockReturnValue("12.3");
        const editor = createEditor();
        expect(editor.get_isValid()).toBe(true);
        editor.destroy();
    });

    it("getAutoNumericOptions applies decimals, padDecimals and allowNegatives", () => {
        const initSpy = vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        const editor = new DecimalEditor({ decimals: 3, padDecimals: false, allowNegatives: true } as any);
        const opts = initSpy.mock.calls[0][1];
        expect(opts.mDec).toBe(3);
        expect(opts.aPad).toBe(false);
        expect(opts.vMin).toBe('-999999999999.99');
        editor.destroy();
    });

    it("getAutoNumericOptions builds symmetric vMin from maxValue with allowNegatives", () => {
        const initSpy = vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        const editor = new DecimalEditor({ allowNegatives: true, maxValue: "100" } as any);
        const opts = initSpy.mock.calls[0][1];
        expect(opts.vMin).toBe("-100");
        editor.destroy();
    });

    it("getAutoNumericOptions guards negative maxValue when building vMin", () => {
        const initSpy = vi.spyOn(AutoNumeric, "init").mockImplementation(() => null);
        const editor = new DecimalEditor({ allowNegatives: true, maxValue: "-100" } as any);
        const opts = initSpy.mock.calls[0][1];
        expect(opts.vMin).toBe("-100");
        editor.destroy();
    });

    it("destroy calls AutoNumeric.destroy", () => {
        const destroySpy = vi.spyOn(AutoNumeric, "destroy");
        const editor = createEditor();
        editor.destroy();
        expect(destroySpy).toHaveBeenCalled();
    });
});