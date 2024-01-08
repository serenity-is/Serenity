import * as mockdeps from "@mockdeps";
import { Culture } from "@serenity-is/base";
import { type DecimalEditorOptions } from "./decimaleditor";

beforeEach(() => {
    mockdeps.clearMockGlobals();
});

const newEditor = async (opt: DecimalEditorOptions) => new (await import("./decimaleditor")).DecimalEditor(opt);

describe("DecimalEditor", () => {
    it("adds default values to autonumeric", async () => {
        Culture.decimalSeparator = ".";

        mockdeps.mockJQuery({
            autoNumeric: jest.fn().mockImplementationOnce(options => {
                expect(options.vMin).toBe('0.00');
                expect(options.vMax).toBe('999999999999.99');
                expect(options.aPad).toBe(true);
                expect(options.aDec).toBe('.');
                expect(options.altDec).toBe(',');
                expect(options.aSep).toBe(',');
                return null;
            })
        });

        await newEditor({});
    });

    it("gets current value using autonumeric if exists", async () => {
        var editor = await newEditor({});
        mockdeps.mockJQuery({
            autoNumeric: jest.fn().mockImplementationOnce((request) => {
                expect(request).toBe('get');
                return '123.45';
            })
        });
        expect(editor.value).toBe(123.45);
    });

    it("gets current value directly if autonumeric not exists", async () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        mockdeps.mockJQuery({ autoNumeric: undefined });
        var editor = await newEditor({});
        editor.domNode.value = '543.21';
        expect(editor.value).toBe(543.21);
    });


    it("sets current value using autonumeric if exists", async () => {
        var editor = await newEditor({});

        mockdeps.mockJQuery({
            autoNumeric: jest.fn().mockImplementationOnce((request, value) => {
                expect(request).toBe('set');
                expect(value).toBe(123.45);
            })
        });

        editor.value = 123.45;
    });

    it("sets current value directly if autonumeric not exists", async () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        mockdeps.mockJQuery({ autoNumeric: undefined });

        var editor = await newEditor({});
        editor.value = 123.45;

        expect(editor.domNode.value).toBe('123.45');
    });

    it("sets element value directly if value is null or empty", async () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        mockdeps.mockJQuery({ autoNumeric: undefined });
        var editor = await newEditor({});

        editor.value = null;
        expect(editor.domNode.value).toBe('');

        editor.value = '' as any;
        expect(editor.domNode.value).toBe('');
    });

    it("returns true from isvalid if value is valid", async () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        mockdeps.mockJQuery({ autoNumeric: undefined });

        var editor = await newEditor({});

        editor.domNode.value = '';
        expect(editor.get_isValid()).toBe(true);
        editor.domNode.value = '123';
        expect(editor.get_isValid()).toBe(true);
    });

    it("returns false from isvalid if value is invalid", async () => {
        Culture.decimalSeparator = ".";
        Culture.groupSeparator = ",";
        mockdeps.mockJQuery({ autoNumeric: undefined });

        var editor = await newEditor({});

        editor.domNode.value = 'not valid';
        expect(editor.get_isValid()).toBe(false);
    });
});